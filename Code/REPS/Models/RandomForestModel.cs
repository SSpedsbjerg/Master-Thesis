using Microsoft.ML.Data;
using Microsoft.ML;
using REPS.Models.SVM;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REPS.Enums;
using Microsoft.ML.Trainers.FastTree;

namespace REPS.Models {
    class RandomForestModel : MachineLearningsModel {
        TransformerChain<BinaryPredictionTransformer<FastForestBinaryModelParameters>>? model = null;
        PredictionEngine<Data, Prediction>? predictionEngine = null;
        private int numberOfTrees = 0;
        FastForestBinaryTrainer.Options options;
        public RandomForestModel(ModelConfig config) : base(config) {
            context = new MLContext();
            this.debugMode = config.debugMode;
            this.options = new FastForestBinaryTrainer.Options {
                // Only use 80% of features to reduce over-fitting.
                FeatureFraction = 0.8,
                // Create a simpler model by penalizing usage of new features.
                FeatureFirstUsePenalty = 0.1,
                NumberOfTrees = (int)config.numberOfTrees
            };
            if(debugMode == true) {
                this.dataSet = GenerateTestData(2000, 2f);
                Train(this.dataSet);
            }
        }

        public override object Predict(object data) {
            Prediction prediction = predictionEngine.Predict((Data)data);
            Console.WriteLine($"Prediction: {prediction.PredictedLabel} | Score: {prediction.Score}");
            return prediction.Score;
        }

        public override bool Train(object[] inputData) {
            try {
                List<Data> trainingData = new List<Data>();
                foreach(object input in inputData) {
                    trainingData.Add((Data)input);
                }
                IDataView? data = context.Data.LoadFromEnumerable(trainingData);
                var pipeline = context.Transforms.Concatenate("Features", nameof(Data.Feature1), nameof(Data.Feature2)).Append(context.BinaryClassification.Trainers.FastForest(this.options));
                this.model = pipeline.Fit(data);
                this.predictionEngine = context.Model.CreatePredictionEngine<Data, Prediction>(model);
                return true;
            }
            catch(Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return false;
            }
        }

        public override async Task<State> Process() {
            State state = State.Stable;
            var values = parameters.Values.ToArray();
            float value0 = System.Convert.ToSingle(values[0]);
            Data data = new() {
                Feature1 = System.Convert.ToSingle(values[0]),
                Feature2 = System.Convert.ToSingle(values[1])
            };
            output = Predict(data: data);
            return State.Stable; //fix up a trigger once I'm sure everything else works as intended
        }
    }
}

