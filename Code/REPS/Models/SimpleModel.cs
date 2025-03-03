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

namespace REPS.Models {
    class SimpleModel : IModel {
        int id;
        SuportedTypes type;
        object output;
        string function;
        public List<string> parametersNames;
        private object testValue;
        private List<double> testParameterValues;
        private string testTopic;
        public Dictionary<string, object> parameters;


        public SimpleModel(ModelConfig config) {
            this.id = config.id;
            this.type = config.type;
            this.function = config.function;
            this.parametersNames = config.parameters;
            this.testValue = config.testvalue;
            this.testParameterValues = config.TestParameterValues;
            this.testTopic = config.testTopic;
            parameters = new Dictionary<string, object>();
        }

        public object Output {
            get => output;
            set => output = value;
        }
        public int NumberOfInputs {
            get => parametersNames.Count;
        }

        public void UpdateValues(string parameter, object value) {
            parameters[parameter] = value;
        }

        private async Task<Func<List<int>, int>> CompileFunctionAsync(string function, List<string> parameters) {
            if(type == SuportedTypes.INT) {
                string code = $@"
                using System;

                public class DynamicFunction {{
                    public static int Compute({string.Join(", ", parameters.Select(p => $"int {p}"))}) {{
                        return {function};
                    }}
                }}";
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
                MetadataReference[] references = {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                    };
                CSharpCompilation compilation = CSharpCompilation.Create(
                    "DynamicAssembly",
                    new[] { syntaxTree },
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    );
                using var ms = new System.IO.MemoryStream();
                var result = compilation.Emit(ms);

                if(!result.Success) {
                    throw new Exception("Compilation failed!");
                }

                ms.Seek(0, System.IO.SeekOrigin.Begin);
                Assembly assembly = Assembly.Load(ms.ToArray());
                Type type = assembly.GetType("DynamicFunction");
                MethodInfo method = type.GetMethod("Compute");
                return args => (int)method.Invoke(null, args.Cast<object>().ToArray());
            }
            return null;
        }

        public async Task<bool> Process() {
            var _func = CompileFunctionAsync(function, parametersNames);
            if(type == SuportedTypes.INT) {
                List<int> inputs = parameters.Values.Cast<int>().ToList(); //get the values from the dictionary
                var func = await _func;
                output = func(inputs);
                Console.WriteLine(output.ToString());
                return true;
            }
            else return false;
        }

        public async Task<bool> Test() {
            for (int i = 0; i < testParameterValues.Count(); i++) {
                if(type == SuportedTypes.INT) {
                    int intValue = (int)testParameterValues[i];
                    try {
                        parameters[parametersNames[i]] = intValue;
                    }
                    catch(ArgumentOutOfRangeException aoore) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{aoore.Message} : Do you have more TestParameterValues than parameters?");
                        Console.ResetColor();
                    }
                    catch(Exception ex) {
                        Console.ForegroundColor= ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }
                }
            }
            _ = await Process();
            if(type == SuportedTypes.INT) {
                _ = Log.NotifyBroker($"Model {id} has processed with value of: {(int)output == (int)testValue}", testTopic);
            }
            return true;
        }

        public bool UpdateValue(string valueID, object value) {
            parameters[valueID] = value;
            return true;
        }
    }
}
