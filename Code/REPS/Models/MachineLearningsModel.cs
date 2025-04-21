using Microsoft.ML;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Models {
    public abstract class MachineLearningsModel : AdaptivModel {
        protected bool? debugMode = false;
        public MachineLearningsModel(ModelConfig config) : base(config) {

        }

        public virtual bool Train(object[] data) {
            throw new NotImplementedException();
        }

        public virtual object Predict(object data) {
            throw new NotImplementedException();
        }
    }
}
