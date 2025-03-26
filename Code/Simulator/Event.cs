using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Simulator {
    abstract class Event {
        public abstract void Start();

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
            for(int i = 0; i < numberOfAttacks; i++) {
                _ = await client.GetStringAsync(this.site);
            }
        }
    }

    class WebSiteActionEvent : WebSiteRequestEvent {
        protected JObject message;
        public WebSiteActionEvent(string site, int seed, int minimumRequests, int maximumRequests, JObject message) : base(site, seed, minimumRequests, maximumRequests) {
            this.message = message;
        }

        public override async void Start() {
            for(int i = 0; i < numberOfAttacks; i++) {
                _ = await client.PostAsync(site, new StringContent(message.ToString()));
            }
        }
    }

    class WebSiteLoginEvent : WebSiteActionEvent {
        protected char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z' };
        public WebSiteLoginEvent(string site, int seed, int minimumRequests, int maxmimumRequests) : base(site, seed, minimumRequests, maxmimumRequests, null) {
            this.message = new JObject();
        }

        public override async void Start() {
            for(int j = 0; j < numberOfAttacks; j++) {
                string name = "";
                string password = "";
                int length = random.Next(20);
                for(int i = 0; i < length; i++) {
                    name += random.Next(2) == 1 ? letters[random.Next(letters.Length)] : letters[random.Next(letters.Length)].ToString().ToUpper();
                }
                for(int i = 0; i < length; i++) {
                    password += random.Next(2) == 1 ? letters[random.Next(letters.Length)] : letters[random.Next(letters.Length)].ToString().ToUpper();
                }
                this.message["username"] = name;
                this.message["password"] = password;
                _ = await client.PostAsync(site, new StringContent(message.ToString()));
            }
        }
    }

    class WebSiteBotCreationEvent : WebSiteActionEvent {
        string[] names;
        string[] emails;
        string[] passwords;

        public WebSiteBotCreationEvent(string site, int seed, int minimumRequests, int maximumRequests) : base(site, seed, minimumRequests, maximumRequests, message: null) {
            this.message = new JObject();
            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'y', 'z' };
            string[] emailDomains = { "@gmail.com", "@yahoo.com" };
            names = new string[maximumRequests];
            emails = new string[maximumRequests];
            passwords = new string[maximumRequests];

            for(int j = 0; maximumRequests > 0; j++) {
                int nameLength = random.Next(10) + 2;
                for(int i = 0; nameLength > 0; i++) {
                    names[j] += letters[random.Next(letters.Length)];
                }
                for(int i = 0; i < 20; i++) {
                    passwords[j] += letters[random.Next(letters.Length)];
                }
                emails[j] += names[j] + emailDomains[random.Next(emailDomains.Length)];
            }
        }

        public override async void Start() {
            for(int i = 0; i < this.numberOfAttacks; i++) {
                this.message["username"] = names[i];
                this.message["email"] = emails[i];
                this.message["password"] = passwords[i];

                _ = await client.PostAsync(site, new StringContent(message.ToString()));
            }
        }
    }
}
