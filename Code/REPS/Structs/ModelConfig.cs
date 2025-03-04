using REPS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Structs {
    public struct ModelConfig {
        public int id;
        public SupportedTypes type;
        public string function;
        public List<string> parameters; //the value is gathered from the node, the first value from node belongs to the first value of parameters and so on...
        public object testvalue;
        public List<double> TestParameterValues;
        public string testTopic;
        public string modelType;
    }
}
