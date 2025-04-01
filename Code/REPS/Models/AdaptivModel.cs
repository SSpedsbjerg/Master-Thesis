using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using REPS.Enums;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Models {
    public class AdaptivModel : SimpleModel {

        //decreases or increases based on data garthered during runtime.
        protected object triggerUpperEqulibrium = 1.5;
        protected object triggerLowerEqulibrium = 0.5;
        List<(object, TimeOnly)> values = new List<(object, TimeOnly)> ();
        protected float updatePercentage;
        protected float QuantileCutoff;

        public AdaptivModel(ModelConfig config) : base(config) {
            this.updatePercentage = config.updatePercentage;
            if(updatePercentage > 1 || updatePercentage <= 0) {
                throw new ArgumentException($"UpdatePercentage should be between 0 and 1, you gave {updatePercentage}");
            }
            this.QuantileCutoff = config.QuantileCutoff;
            if(QuantileCutoff >= 0.5 || QuantileCutoff <= 0) {
                throw new ArgumentException($"QuantileCutoff should be less than 0.5 and more than 0, you gave {QuantileCutoff}");
            }
        }

        protected override async Task<Func<List<int>, int>> CompileFunctionAsync(string function, List<string> parameters) {
            if(this.type == SupportedTypes.INT) {
                string code = $@"
                using System;

                public class DynamicFunction {{
                    public static int Compute({string.Join(", ", parameters.Select(p => $"int {p}"))}) {{
                        {function}
                        return value;
                    }}
                }}";
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
                MetadataReference[] references = {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                    };
                CSharpCompilation compilation = CSharpCompilation.Create(
                    "DynamicModel",
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
#pragma warning disable CS8605 // Unboxing a possibly null value.
                return await Task.FromResult((List<int> values) =>
                    (int)method.Invoke(null, new object[] { values }
                    ));
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }
            return null;
        }

        protected override async Task<Func<object[], object>> CompileTrigger(string function) {
            string code = $@"
                using System;           

                public class DynamicTrigger {{
                    public static int Compute(object output, float lower, float upper){{
                        int value = (int)output;
                        {function}
                        bool triggered = false;
                        if(lower > value || upper < value) triggered = true;
                        return triggered;
                        }}
                    }}
                ";
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);
            MetadataReference[] references = {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
                    };
            CSharpCompilation compilation = CSharpCompilation.Create(
                "DynamicTrigger",
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
            Type type = assembly.GetType("DynamicTrigger");
            MethodInfo method = type.GetMethod("Compute");
            return await Task.FromResult(((object[]a) => method.Invoke(null, new object[] { a[0], a[1], a[2] })));
        }

        protected virtual void UpdateEqulibrium<T>((object, TimeOnly)[] values) {
            if(this.type == SupportedTypes.INT) {
                int cutOffPoint = (int)(this.values.Count * updatePercentage);
                TimeOnly timeCutoff = this.values[cutOffPoint].Item2;
                (object, TimeOnly)[]? toBeReplaced = ((object, TimeOnly)[])this.values.Where(data => data.Item2 > timeCutoff);
                foreach((object, TimeOnly) value in this.values) {
                    if(toBeReplaced.Contains(value)) {
                        this.values.Remove(value);
                    }
                }
                this.values.AddRange(values);
                this.triggerLowerEqulibrium = this.values[(int)(this.values.Count * QuantileCutoff)].Item1;
                this.triggerLowerEqulibrium = this.values[(int)(this.values.Count * (1-QuantileCutoff))].Item1;
            }
        }

        public override async Task<bool> Process() {
            var _func = CompileFunctionAsync(function, parametersNames);
            var _trigger = CompileTrigger(this.triggerFunction);
            if(type == SupportedTypes.INT) {
                List<int> inputs = parameters.Values.Cast<int>().ToList(); //get the values from the dictionary
                var func = await _func;
                output = func(inputs);
                Console.WriteLine("Output is :" + output.ToString());
                var trigger = await _trigger;
                try {
                    object eval = trigger([output, triggerLowerEqulibrium, triggerUpperEqulibrium]);
                    if((bool)eval) {
                        Console.WriteLine("Triggered"); //TODO: rewrite to message the mqtt broker
                    }
                    else {
                        Console.WriteLine("Not Triggered"); //TODO: rewrite to message the mqtt broker
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error: {ex.Message}");

                }

                return true;
            }
            else
                return false;
        }
    }
}
