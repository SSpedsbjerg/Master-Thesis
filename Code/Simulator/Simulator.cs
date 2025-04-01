using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator {
    public class Simulator {
        private Random random;
        private DateTime startTime;
        private DateTime endTime;
        private Dictionary<Event, DateTime> events;
        public Simulator(int seed, int time, string site) {
            Console.WriteLine($"Seed: {seed}, time: {time}, site: {site}");
            this.random = new Random(seed);
            this.startTime = DateTime.Now;
            this.endTime = DateTime.Now.AddMinutes(time);
            Event[] unTimedEvents = {
                new WebSiteRequestEvent(site: site, seed: seed, minimumRequests: 100, maximumRequests: 500),
                new WebSiteLoginEvent(site: site, seed: seed, minimumRequests: 100, maxmimumRequests: 500),
                new WebSiteBotCreationEvent(site: site, seed: seed, minimumRequests: 100, maximumRequests: 500)
            };
            events = new Dictionary<Event, DateTime>();
            foreach(Event e in unTimedEvents) {
                events[e] = DateTime.Now.AddMinutes(random.Next(time));
            }
        }

        public void Start() {
            startTime = DateTime.Now;
            Console.WriteLine($"Started : {startTime.ToString()}, planned to end : {endTime.ToString()}");
            while(DateTime.Now < endTime) {
                foreach(Event e in events.Keys) {
                    if(events[e] < DateTime.Now) {
                        Console.WriteLine("Started Event " + e.ToString());
                        e.Start();
                        if(random.Next(2) == 1) events.Remove(e);
                    }
                }
            }
        }
    }
}