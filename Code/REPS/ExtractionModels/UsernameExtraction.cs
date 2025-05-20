using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace REPS.ExtractionModels {
    public static class UsernameExtraction {
        public static List<string> Usernames = new List<string>();
        //I've attempted to follow this page: https://rosettacode.org/wiki/Entropy#C for it, there seemed to be some major errors in the function which is why my version is quite different
        private static double CalculateShannonEntropy(string text) {
            if(string.IsNullOrEmpty(text)) return 0.0;
            double entropy = 0.0;
            Dictionary<char, int> frequency = new Dictionary<char, int>();

            foreach(char c in text) {
                if(frequency.ContainsKey(c)) frequency[c]++;
                else frequency[c] = 1;
            }

            foreach(var kvp in frequency) {
                double p = (double)kvp.Value / text.Length;
                entropy -= p * Math.Log(p, 2);
            }
            return entropy;
        }

        //extracts 2 features, might need to upgrade if determined
        public static List<double[]> ExtractRealNames(List<string> usernames) {
            List<double[]> featureList = new List<double[]>();
            foreach(string name in usernames) {
                string uName = name.ToLower();
                int length = uName.Length;
                int VowelCount = uName.Count(c => "aeiou".Contains(c));
                int ConsonantsCount = uName.Length - VowelCount;
                double vowelRatio = (double)VowelCount / (double)uName.Length;
                double ShannonEntropy = CalculateShannonEntropy(uName);
                featureList.Add(new double[] { vowelRatio, ShannonEntropy });
            }
            return featureList;
        }

        public static float[] ExtractRealName(string username) {
            float[] features;
            string uName = username.ToLower();
            int length = uName.Length;
            int VowelCount = uName.Count(c => "aeiou".Contains(c));
            int ConsonantsCount = uName.Length - VowelCount;
            double vowelRatio = (double)VowelCount / (double)uName.Length;
            double ShannonEntropy = CalculateShannonEntropy(uName);
            double specialCharRatio = 0;
            foreach(char letter in uName) {
                if(!char.IsLetter(letter))
                    specialCharRatio++;
            }
            specialCharRatio = specialCharRatio / (double)uName.Length;


            features = new float[] { (float)vowelRatio, (float)ShannonEntropy, length, (float)specialCharRatio, };
            return features;
        }

        public static void WriteFeaturesToCSV(List<string> usernames, List<double[]> featureList, string fileName) {
            using(StreamWriter writer = new StreamWriter(fileName)) {
                writer.WriteLine("Username,VowelRatio,ShannonEntropy");
                for(int i = 0; i < usernames.Count; i++) {
                    string name = usernames[i];
                    double[] features = featureList[i];
                    string line = string.Join(",", new[] {name}.Concat(features.Select(f => f.ToString("F4"))));
                    writer.WriteLine(line);
                }
            }
        }


        //saves the data gathered before closing the terminal
        public static void SaveDataOnClose(object sender, EventArgs args) {
            if(Usernames.Count > 0) {
                Console.WriteLine("Saving data...");
                WriteFeaturesToCSV(Usernames, ExtractRealNames(Usernames), $"TrainingSet_{new Guid("N")}");
            }
            Console.WriteLine("Press Enter to close");
            Console.ReadLine();
        }
    }
}
