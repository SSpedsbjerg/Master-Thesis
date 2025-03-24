using REPS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Structs {
    public struct SensorConfig {
        public int id;
        public string name;
        public string host;
        public string routingKey;
        public string topic;
        public SupportedTypes type;
    }
}
