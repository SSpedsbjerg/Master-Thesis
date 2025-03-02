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

        Task<bool> Process();
        Task<bool> Test();

        public bool UpdateValue(string valueID, object value);
    }
}
