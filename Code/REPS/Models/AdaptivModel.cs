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
            if(!(config.updatePercentage is null || config.QuantileCutoff is null)) {
                this.updatePercentage = (float)config.updatePercentage;
                if(updatePercentage > 1 || updatePercentage <= 0) {
                    throw new ArgumentException($"UpdatePercentage should be between 0 and 1, you gave {updatePercentage}");
                }
                this.QuantileCutoff = (float)config.QuantileCutoff;
                if(QuantileCutoff >= 0.5 || QuantileCutoff <= 0) {
                    throw new ArgumentException($"QuantileCutoff should be less than 0.5 and more than 0, you gave {QuantileCutoff}");
                }
            }
        }

        protected override async Task<Func<object[], object?>> CompileTrigger(string function) {
            string guid = Guid.NewGuid().ToString("N");
            string code = $@"
                using System;           

                public class DynamicTrigger_{guid} {{
                    public static bool Compute_{guid}(object output, float lower, float upper){{
                        int value = System.Convert.ToInt32(output);
                        {function}
                        bool triggered = false;
                        if(lower > value || upper < value) triggered = true;
                        return triggered;
                        }}
                    }}
                ";
            MethodInfo method = MethodTriggerConstructor(code, guid);
            return await Task.FromResult(((object[]a) => method.Invoke(null, new object[] { a[0], a[1], a[2] })));
        }

        protected virtual void UpdateEqulibrium<T>((object, TimeOnly)[] values) {
            if(typeof(T) == typeof(int)) {
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
                this.triggerUpperEqulibrium = this.values[(int)(this.values.Count * (1 - QuantileCutoff))].Item1;
            }
            //implement the rest of the updates
        }

        public override async Task<State> Process() {
            var _func = CompileFunctionAsync(function, parameters.Keys.ToArray());
            var _trigger = CompileTrigger(this.triggerFunction);
            State state = State.Stable;
            if(type == SupportedTypes.INT) {
                object[] inputs = (object[])parameters.Values.Cast<object>(); //get the values from the dictionary
                var func = await _func;
                output = func(inputs);
                Console.WriteLine("Output is :" + output.ToString());
                var trigger = await _trigger;
                try {
                    object eval = trigger([output, triggerLowerEqulibrium, triggerUpperEqulibrium]);
                    if((bool)eval) {
                        state = State.Stable;
                    }
                    else {
                        state = State.Unstable;
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine($"Error: {ex.Message}");
                    state = State.Unknown;
                }

                return state;
            }
            else
                return state;
        }
    }
}
