using REPS.Enums;
using REPS.Interfaces;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Models {
    public class TimedModel : SimpleModel {
        private List<(object, DateTime)> formerValues = new List<(object, DateTime)>();
        
        private object newestValue;
        private float timedTrigger;
        public TimedModel(ModelConfig config) : base(config) {
            if(config.timedTrigger == null)
                throw new Exception("Error in config: Missing TimedTrigger in the time model");
            this.timedTrigger = (float)config.timedTrigger;
        }

        public override bool UpdateValue(string parameter, INode node) {
            if(node is null) return false;
            newestValue = node.Output;
            return base.UpdateValue(parameter, node);
        }

        public override async Task<State> Process() {
            bool alreadyProcessed = false;
            foreach(var item in formerValues) {
                if(newestValue == item.Item1) {
                    alreadyProcessed = true; break;
                }
            }
            if(!alreadyProcessed) {
                if(formerValues.Count > 50) {
                    formerValues.RemoveAt(0);
                }
                formerValues.Add((newestValue, DateTime.Now));
            }
            DateTime newestTime = DateTime.MaxValue;
            foreach(var item in formerValues) {
                if(newestTime > item.Item2) {
                    newestTime = item.Item2;
                }
            }
            List<double> times = new List<double>();
            foreach(var item in formerValues) {
                times.Add(item.Item2.Ticks - newestTime.Ticks);
            }

            this.output = (float)times.Average();
            if((float)this.output > this.timedTrigger)
                return State.Unstable;
            return State.Stable;
        }
    }
}