using REPS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Interfaces {
    public interface IModel {
        object Output {
            get;
            set;
        }

        int NumberOfInputs {
            get;
        }

        Task<State> Process();
        Task<bool> Test();

        public bool UpdateValue(string valueID, INode node);
    }
}
