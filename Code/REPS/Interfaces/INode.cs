using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Interfaces {
    public interface INode {
        object Output {
            get;
            set;
        }

        public Task Process();

    }
}
