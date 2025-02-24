using REPS.Enums;
using REPS.Interfaces;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Nodes {
    public class SensorNode : INode {
        private int id;
        private string host;
        private string routingKey;
        private string topic;
        private SuportedTypes type;

        private object output; 

        public SensorNode(SensorConfig config) {
            host = config.host;
            topic = config.topic;
            routingKey = config.routingKey;
            type = config.type;
            id = config.id;
        }

        public object Output {
            get => output;
            set => Output = value;
        }

        public int ID {
            get => id;
        }

        public Task Process() {
            throw new NotImplementedException();
        }
    }
}
