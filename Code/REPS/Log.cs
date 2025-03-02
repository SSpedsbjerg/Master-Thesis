using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS {
    public static class Log {
        public static string Broker = "";
        public static async Task<bool> Error(Exception exception, string className, string description) {
            try {
                StreamWriter streamWriter = new StreamWriter("ErrorLog.txt", append: true);
                var write = streamWriter.WriteLineAsync($"----------{DateTime.Now:yyyy-MM-dd hh:mm:ss}----------\n{exception}\n{className}\n{description}");
                await write;
                streamWriter.Close();
                Console.WriteLine(exception + " " + className + " " + description);
                return true;
            }
            catch(IOException IO) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Race condition when writing to error log, printing to console istead: {exception.Message} {className} {description}");
                Console.ResetColor();
                return false;
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static async Task<bool> Warning(string warning, string className, string description) {
            try {
                StreamWriter streamWriter = new StreamWriter("WarningLog.txt", append: true);
                var write = streamWriter.WriteLineAsync($"----------{DateTime.Now:yyyy-MM-dd hh:mm:ss}----------\n{warning}\n{className}\n{description}");
                await write;
                streamWriter.Close();
                Console.WriteLine(warning + " " + className + " " + description);
                return true;
            }
            catch(IOException IO) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Race condition when writing to warning log, printing to console istead: {warning} {className} {description}");
                Console.ResetColor();
                return false;
            }
            catch(Exception ex) {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public static async Task<bool> NotifyBroker(string message, string topic) {
            throw new NotImplementedException();
        }
    }
}
