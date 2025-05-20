using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using REPS.Enums;
using REPS.Interfaces;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using REPS.Connections;
using Newtonsoft.Json.Linq;

namespace REPS.Models {
    public class SimpleModel : IModel {
        int id;
        protected SupportedTypes type;
        protected object output;
        protected string function;
        protected string triggerFunction = "if(value > 40) return 3; else if (value > 30) return 2; else if (value > 20) return 1; else if (value > 10) return 0;";
        protected object testValue;
        protected List<double> testParameterValues;
        protected string testTopic;
        public Dictionary<string, object> parameters;
        protected string name;


        public SimpleModel(ModelConfig config) {
            this.id = config.id;
            this.type = config.type;
            this.function = config.function;
            this.testValue = config.testvalue;
            this.testParameterValues = config.TestParameterValues;
            this.testTopic = config.testTopic;
            this.name = config.name;
            this.triggerFunction = config.triggerFunction;
            parameters = new Dictionary<string, object>();
            foreach(string parameter in config.parameters) {
                parameters.Add(parameter, 0);
            }
        }

        public object Output {
            get => output;
            set => output = value;
        }
        public int NumberOfInputs {
            get => parameters.Keys.Count;
        }

        public virtual bool UpdateValue(string parameter, INode node) {
            if(node == null) return false;
            if(node.Output != null) {
                if(parameters.ContainsKey(parameter)) {
                    parameters[parameter] = node.Output;
                }
                else {
                    parameters.Add(parameter, node.Output);
                }
                return true;
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{node.ToString()} has set their value as of yet, all nodes which makes use of this will suffer in their prediction ability");
                Console.ResetColor();
                if(parameters.ContainsKey(parameter)) {
                    parameters[parameter] = 0;
                }
                else {
                    parameters.Add(parameter, 0);
                }
                return false;
            }
        }

        protected virtual MethodInfo MethodFunctionConstructor(string code, string guid) {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            MetadataReference[] references = {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                    };
            CSharpCompilation compilation = CSharpCompilation.Create(
                $"DynamicModel_{guid}",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );
            using var ms = new System.IO.MemoryStream();
            var result = compilation.Emit(ms);

            if(!result.Success) {

                throw new Exception($"Compilation failed,\n{result.Diagnostics[0]},\n{code}");
            }

            ms.Seek(0, System.IO.SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());
            Type type = assembly.GetType($"DynamicFunction_{guid}");
            MethodInfo method = type.GetMethod($"Compute_{guid}");
            return method;
        }

#pragma warning disable CS1998 // Must have the async here
        protected virtual async Task<Func<object[], object?>> CompileFunctionAsync(string function, string[] parameters) {
            string guid = Guid.NewGuid().ToString("N");
            string code = $@"
                using System;

                public class DynamicFunction_{guid} {{
                    public static {REPS.Convert.GetStringType(type)} Compute_{guid}({string.Join(", ", parameters.Select(p => $"object {p}"))}) {{
                        {function};
                    }}
                }}";
            MethodInfo method = MethodFunctionConstructor(code, guid);
            return args => {
                object[] argumentArray = args.Select(x => (object)x).ToArray();
                object? invokation = null;
                try {
                    invokation = method.Invoke(null, argumentArray);
                }
                catch(Exception e) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.WriteLine(function + " " + e.Message);
                    Console.WriteLine("This may be due to a config error");
                    Console.ResetColor();
                }
                return invokation;
            };
        }
#pragma warning restore CS1998


        protected virtual MethodInfo MethodTriggerConstructor(string code, string guid) {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            MetadataReference[] references = {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                    };
            CSharpCompilation compilation = CSharpCompilation.Create(
                $"DynamicTrigger_{guid}",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );
            using var ms = new System.IO.MemoryStream();
            var result = compilation.Emit(ms);

            if(!result.Success) {
                throw new Exception($"Compilation failed for the trigger!\n{code}\n{result.Diagnostics[0]}");
            }

            ms.Seek(0, System.IO.SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());
            Type type = assembly.GetType($"DynamicTrigger_{guid}");
            MethodInfo method = type.GetMethod($"Compute_{guid}");
            return method;
        }

        //unlike the other function compiler, this must return an int which can be converted to the severity level
#pragma warning disable CS1998 //must be async
        protected virtual async Task<Func<object[], object?>> CompileTrigger(string function) {
            string guid = Guid.NewGuid().ToString("N");
            string code = $@"
                using System;           

                public class DynamicTrigger_{guid} {{
                    public static int Compute_{guid}(object output){{
                        {function}
                        }}
                    }}
                ";
            return args => MethodTriggerConstructor(code, guid).Invoke(null, args);
        }
#pragma warning restore CS1998

        public virtual async Task<State> Process() {
            var _func = CompileFunctionAsync(function, parameters.Keys.ToArray());
            var _trigger = CompileTrigger(triggerFunction);

            State state = State.Stable;

            object[] inputs = parameters.Values.ToArray();
            for(int i = 0; i < inputs.Length; i++) {
                if(inputs[i] == null) {
                    inputs[i] = (object)0;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"A input value was null!\nFix following function:\n{function}");
                    Console.ResetColor();
                }
            }
            var func = await _func;
            output = func(inputs);
            if(output is null) {
                output = string.Empty;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Output is null {function}");
                Console.ResetColor();
            }
            Console.WriteLine($"Output of model {this.name} is : {output.ToString()} with the state : {state}");
            var trigger = await _trigger;
            try {
                state = (State)trigger(new object[] { output });
            }
            catch(Exception ex) {
                Console.WriteLine($"Error: {ex.Message} with the Trigger!");
                state = State.Unknown;
            }
            return state;
        }

        public async Task<bool> Test() {
            if(testTopic == null || testParameterValues == null || testValue == null) {
                Console.WriteLine("Could not test due to undefined test variables");
                return true;
            }
            int i = 0;
            foreach(var key in parameters.Keys) {
                if(type == SupportedTypes.INT) {

                    try {
                        parameters[key] = testParameterValues[i];
                    }
                    catch(ArgumentOutOfRangeException aoore) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{aoore.Message} : Do you have more TestParameterValues than parameters?");
                        Console.ResetColor();
                    }
                    catch(Exception ex) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                    i++;
                }
            }
            _ = await Process();
            if(type == SupportedTypes.INT) {
                _ = Log.NotifyBroker($"Model {id} has processed with value of: {(int)System.Convert.ToSingle(output) == (int)System.Convert.ToSingle(testValue)}", testTopic);
            }
            return true;
        }

        public bool UpdateValue(string valueID, object value) {
            parameters[valueID] = value;
            return true;
        }
    }
}
