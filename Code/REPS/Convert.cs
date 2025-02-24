using REPS.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REPS {
    public static class Convert {
        public static int GetValue(object input) {
            try {
                return (int)input;
            }
            catch (InvalidCastException ICE) {
                StackFrame sf = new StackTrace().GetFrame(1);
                _ = Log.Error(ICE, $"Class : {sf.GetType()}", $"Method : {sf.GetMethod()}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ICE.ToString());
                Console.ResetColor();
                return -1;
            }
            catch (Exception e) {
                StackFrame sf = new StackTrace().GetFrame(1);
                _ = Log.Error(e, $"Class : {sf.GetType()}", $"Method : {sf.GetMethod()}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.ToString());
                Console.ResetColor();
                return -1;
            }
        }
    }
}
