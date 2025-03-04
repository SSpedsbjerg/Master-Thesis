using REPS.Enums;
using REPS.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Structs {
    public struct EventConfig {
        public int id;
        public string name;
        public SupportedTypes type;
        public List<int> sensorNodeIDs;
        public List<int> eventNodeIDs;
        public ModelConfig modelConfig;
        public string modelType;
        public string reportTopic;
        public string host;
        public string routingKey;
    }
}
