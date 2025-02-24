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
        public SuportedTypes type;
        public List<int> nodeIDs;
        public IModel model;
        public string modelType;
        public string reportTopic;
    }
}
