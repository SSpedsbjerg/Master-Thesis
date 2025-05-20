using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using REPS.Enums;
using REPS.ExtractionModels;
using System.Reflection.Emit;
using System.Data;


//TODO: Update to support more than 2 data vectors
namespace REPS.Models.SVM {
    public class SupportVectorMachineModel : MachineLearningsModel {
        //TransformerChain<BinaryPredictionTransformer<Microsoft.ML.Trainers.LinearBinaryModelParameters>>? model = null;
        TransformerChain<BinaryPredictionTransformer<Microsoft.ML.Trainers.LdSvmModelParameters>>? model = null;

        PredictionEngine<Data, Prediction>? predictionEngine = null;

        private bool lastPrediction;

        public SupportVectorMachineModel(ModelConfig config) : base(config) {
            context = new MLContext();
            this.debugMode = config.debugMode;
            if(debugMode == true) {
                this.dataSet = GenerateTestData(2000, 2f);
                Train(this.dataSet);
            }
            else {
                List<Data> datas = new List<Data>();
                using(StreamReader reader = new StreamReader(config.trainingsDataLocation)) {
                    while(!reader.EndOfStream) {
                        string line = reader.ReadLine();
                        if(line.Contains("Username")) {
                            continue;
                        }
                        else {
                            string[] values = line.Split(';');
                            string label = values.Last();
                            values = values[1..^1]; //remove the label and name
                            float[] parsedValues = values.Select(x => float.Parse(x)).ToArray();
                            datas.Add(new Data { Features = parsedValues, Label = bool.Parse(label) });
                        }
                    }
                    SetData(datas.ToArray());
                }
            }
        }

        public override object Predict(object data) {
            Prediction prediction = predictionEngine.Predict((Data)data);
            Console.WriteLine($"Prediction: {prediction.PredictedLabel} | Score: {prediction.Score}");
            lastPrediction = prediction.PredictedLabel;
            return prediction.Score;
        }

        public override bool Train(object[] inputData) {
            try {
                List<Data> trainingData = new List<Data>();
                foreach(object input in inputData) {
                    trainingData.Add((Data)input);
                }
                var vectorSize = trainingData[0].Features.Length;
                Console.WriteLine(trainingData[0].Features.GetType());
                trainingData = trainingData.Where(x => x.Features.Length == vectorSize).ToList();
                IDataView? data = context.Data.LoadFromEnumerable(trainingData);
                
                var pipeline = context.Transforms.NormalizeMinMax("Features").Append(context.Transforms.NormalizeMinMax("Features")).Append(context.BinaryClassification.Trainers.LdSvm(labelColumnName: "Label", featureColumnName: "Features"));


                Console.WriteLine($"Training...");
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
            Data data;
            float[] stringComparison;
            if(values[0] is string) {
                stringComparison = UsernameExtraction.ExtractRealName((string)values[0]);
                data = new() {
                    Features = stringComparison
                };
            }
            else {
                data = new() {
                    Features = new float[] { 0, 0, 0, 0 }
                };
            }
            output = Predict(data: data);
            if(lastPrediction == false) {
                return State.Stable;
            }
            else return State.Unstable;
        }
    }
}
