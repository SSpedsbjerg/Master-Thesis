using Microsoft.ML;
using Microsoft.ML.Data;
using REPS.Models.SVM;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Models {
    //has to be a class type and not a struct
    public class Data {
        [VectorType(4)]//defines length, for some reason I cannot seem to make my ml adjust this during run-time
        public float[] Features { get; set; } = default!; //loads from column zero to the second last column
        public bool Label;
    }

    //has to be a class type and not a struct
    public class Prediction {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel;

        public float Score;
    }

    public abstract class MachineLearningsModel : AdaptivModel {
        protected bool? hasUsernames = false;
        protected bool? debugMode = false;
        protected Data[] dataSet;
        protected MLContext context;
        public MachineLearningsModel(ModelConfig config) : base(config) {
            hasUsernames = config.isUsername;
        }

        public virtual void SetData(Data[] data) {
            this.dataSet = data;
            Train(this.dataSet);
        }

        public virtual bool Train(object[] data) {
            throw new NotImplementedException();
        }

        public virtual object Predict(object data) {
            throw new NotImplementedException();
        }

        protected virtual Data[] GenerateTestData(int count, float seperation) {
            Random random = new Random();
            Data[] data = new Data[count];
            for(int i = 0; i < count; i++) {
                bool label = i % 2 == 0;
                float center = label ? seperation : 0;
                float x = (float)(random.NextDouble() * 1.5 + center);
                float y = (float)(random.NextDouble() * 1.5 + center);
                data[i] = new Data { Features = [x, y], Label = label };
            }
            return data;
        }
    }
}
