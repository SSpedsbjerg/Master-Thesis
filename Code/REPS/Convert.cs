﻿using REPS.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace REPS {
    public static class Convert {
        private static void HandleException(Exception exception) {
            if(exception is InvalidCastException) {
                StackFrame sf = new StackTrace().GetFrame(1);
                if(sf is null)
                    _ = Log.Error(new Exception("How did you manage to make a stacktrace null?"), "Convert", "Stacktrace was null somehow, it occured during the 'InvalidCastException' exception.");
                else
                    _ = Log.Error((InvalidCastException)exception, $"Class : {sf.GetType()}", $"Method : {sf.GetMethod()}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(((InvalidCastException)exception).ToString());
                Console.ResetColor();
            }
            else {
                StackFrame sf = new StackTrace().GetFrame(1);
                if(sf is null)
                    _ = Log.Error(new Exception("How did you manage to make a stacktrace null?"), "Convert", "Stacktrace was null somehow, it occured during the 'Exception' exception.");
                else
                    _ = Log.Error(exception, $"Class : {sf.GetType()}", $"Method : {sf.GetMethod()}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.ToString());
                Console.ResetColor();
            }
        }

        public static T GetValue<T>(object input) {
            switch(typeof(T)) {
                case (typeof(int)):
                    try {
                        if(input is string) {
                            return int.Parse(input as string);
                        }
                        return (int)input;
                    }
                    catch(InvalidCastException ICE) {
                        HandleException(ICE);
                        return -1;
                    }
                    catch(NullReferenceException NRE) {
                        HandleException(NRE);
                        return -1;
                    }
                    catch(Exception e) {
                        HandleException(e);
                        return -1;
                    }
                    break;
                case (typeof(double)):
                    try {
                    if(input is string) {
                        return double.Parse(input as string);
                        }
                    return (double)input;
                    }
                    catch(InvalidCastException ICE) {
                        HandleException(ICE);
                        return -1;
                    }
                    catch(NullReferenceException NRE) {
                        HandleException(NRE);
                        return -1;
                    }
                    catch (Exception e) {
                        HandleException(e);
                        return -1;
                    }
                    break;
                case (typeof(float)):
                    try {
                        if(input is string) {
                            return float.Parse(input as string);
                        }
                        return (float)input;
                    }
                    catch(InvalidCastException ICE) {
                        HandleException(ICE);
                        return -1;
                    }
                    catch(NullReferenceException NRE) {
                        HandleException(NRE);
                        return -1;
                    }
                    catch(Exception e) {
                        HandleException(e);
                        return -1;
                    }
                    break;
                case (typeof(bool)):
                    try {
                        if(input is string) {
                            return bool.Parse(input as string);
                        }
                        return (bool)input;
                    }
                    catch(InvalidCastException ICE) {
                        HandleException(ICE);
                        return -1;
                    }
                    catch(NullReferenceException NRE) {
                        HandleException(NRE);
                        return -1;
                    }
                    catch(Exception e) {
                        HandleException(e);
                        return -1;
                    }
                    break;
            }
        }
    }
}
