using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Bogus;
using System.Text.RegularExpressions;
using Bogus.DataSets;

namespace Simulator {
    abstract class Event {
        public abstract void Start();
        protected char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z', ' ' };


        //I've attempted to follow this page: https://rosettacode.org/wiki/Entropy#C for it, there seemed to be some major errors in the function which is why my version is quite different
        protected double CalculateShannonEntropy(string text) {
            if(string.IsNullOrEmpty(text))
                return 0.0;
            double entropy = 0.0;
            Dictionary<char, int> frequency = new Dictionary<char, int>();

            foreach(char c in text) {
                if(frequency.ContainsKey(c))
                    frequency[c]++;
                else
                    frequency[c] = 1;
            }

            foreach(var kvp in frequency) {
                double p = (double)kvp.Value / text.Length;
                entropy -= p * Math.Log(p, 2);
            }
            return entropy;
        }

        protected List<double[]> ExtractRealNames(List<string> usernames) {
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

        protected double[] ExtractRealName(string username) {
            double[] features;
            string uName = username.ToLower();
            int length = uName.Length;
            int VowelCount = uName.Count(c => "aeiou".Contains(c));
            int ConsonantsCount = uName.Length - VowelCount;
            double vowelRatio = (double)VowelCount / (double)uName.Length;
            double ShannonEntropy = CalculateShannonEntropy(uName);
            double specialCharRatio = 0;
            foreach(char letter in uName) {
                if(!char.IsLetter(letter)) specialCharRatio++;
            }
            specialCharRatio = specialCharRatio / (double)uName.Length;


            features = new double[] { vowelRatio, ShannonEntropy, length, specialCharRatio, };
            return features;
        }

        protected void SaveData(List<(string, double[], bool)> values, string fileName) {
            using(StreamWriter writer = new StreamWriter(fileName)) {
                writer.WriteLine("Username;VowelRatio;ShannonEntropy;length;specialCharRatio;isBot");
                for(int i = 0; i < values.Count; i++) {
                    string name = values[i].Item1;
                    double[] features = values[i].Item2;
                    string line = string.Join(";", new[] { name }.Concat(features.Select(f => f.ToString("F4"))));
                    line += $";{values[i].Item3}";
                    writer.WriteLine(line);
                }
            }
        }
    }

    class WebSiteRequestEvent : Event {
        protected string site;
        protected Random random;
        protected int numberOfAttacks = 0;
        protected HttpClient client;
        public WebSiteRequestEvent(string site, int seed, int minimumRequests, int maximumRequests) {
            random = new Random(seed);
            numberOfAttacks = random.Next(minimumRequests, maximumRequests);
            this.site = site;
            client = new HttpClient();
        }

        public override async void Start() {
            Console.WriteLine("Started WebSiteRequestEvent");
            for(int i = 0; i < numberOfAttacks; i++) {
                await client.GetStringAsync(this.site);
            }
        }
    }

    class WebSiteActionEvent : WebSiteRequestEvent {
        protected JObject message;
        public WebSiteActionEvent(string site, int seed, int minimumRequests, int maximumRequests, JObject message) : base(site, seed, minimumRequests, maximumRequests) {
            this.message = message;
        }

        public override async void Start() {
            Console.WriteLine("Started WebSiteActionEvent");
            for(int i = 0; i < numberOfAttacks; i++) {
                Console.WriteLine($"Sending message: {message.ToString()}");
                await client.PostAsync(site, new StringContent(message.ToString(), Encoding.UTF8, "application/json"));
            }
        }
    }

    class WebSiteLoginEvent : WebSiteActionEvent {
        public WebSiteLoginEvent(string site, int seed, int minimumRequests, int maxmimumRequests) : base(site, seed, minimumRequests, maxmimumRequests, null) {
            this.message = new JObject();
        }

        public override async void Start() {
            Console.WriteLine("Started WebSiteLoginEvent");
            for(int j = 0; j < numberOfAttacks; j++) {
                string name = "";
                string password = "";
                int length = random.Next(20) + 2;
                for(int i = 0; i < length; i++) {
                    name += random.Next(2) == 1 ? letters[random.Next(letters.Length)] : letters[random.Next(letters.Length)].ToString().ToUpper();
                }
                for(int i = 0; i < length; i++) {
                    password += random.Next(2) == 1 ? letters[random.Next(letters.Length)] : letters[random.Next(letters.Length)].ToString().ToUpper();
                }
                this.message["username"] = name;
                this.message["password"] = password;
                Console.WriteLine($"Sending message: {message.ToString()}");
                await client.PostAsync(site + "/login", new StringContent(message.ToString(), Encoding.UTF8, "application/json"));
            }
        }
    }

    class WebSiteBotCreationEvent : WebSiteActionEvent {
        string[] names;
        string[] emails;
        string[] passwords;
        Bogus.Faker faker;

        public WebSiteBotCreationEvent(string site, int seed, int minimumRequests, int maximumRequests) : base(site, seed, minimumRequests, maximumRequests, message: null) {
            this.message = new JObject();
            string[] emailDomains = { "@gmail.com", "@yahoo.com" };
            names = new string[maximumRequests];
            emails = new string[maximumRequests];
            passwords = new string[maximumRequests];
            faker = new Bogus.Faker("en");

            for(int j = 0; maximumRequests > j; j++) {
                int nameLength = random.Next(10) + 2;
                for(int i = 0; nameLength > i; i++) {
                    names[j] += letters[random.Next(letters.Length)];
                }
                for(int i = 0; i < 20; i++) {
                    passwords[j] += letters[random.Next(letters.Length)];
                }
                emails[j] += names[j] + emailDomains[random.Next(emailDomains.Length)];
            }
            List<(string, double[], bool)> values = new();
            foreach(string name in names) {
                values.Add((name, ExtractRealName(name), true));
            }
            
            for(int i = 0; i < names.Count(); i++) {
                string name = faker.Name.FullName();
                values.Add((name, ExtractRealName(name), false));
            }
            Random rand = new Random();
            List<(string, double[], bool)> shuffled = values.OrderBy(x => rand.Next()).ToList();
            SaveData(shuffled, "BotCreationEvent.csv");
        }

        public override async void Start() {
            Console.WriteLine("Started WebSiteBotCreationEvent");
            for(int i = 0; i < this.numberOfAttacks; i++) {
                this.message["username"] = names[i];
                this.message["email"] = emails[i];
                this.message["password"] = passwords[i];
                Console.WriteLine($"Sending message: {message.ToString()} To: {site + "/Register"}");
                try {
                    var response = await client.PostAsync(site + "/Register", new StringContent(message.ToString(), Encoding.UTF8, "application/json"));
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Status: {response.StatusCode}, Body: {responseBody}");
                }
                catch(HttpRequestException e) {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
