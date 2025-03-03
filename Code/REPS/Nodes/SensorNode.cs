using REPS.Connections;
using REPS.Enums;
using REPS.Interfaces;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Nodes {
    public class SensorNode : INode {
        private int id;
        private string host;
        private string routingKey;
        private string topic;
        private SuportedTypes type;
        private Task<Connection> connectionTask;
        private Connection connection;
        private string preProcessedMessage;
        private bool overriddenMessage = false;

        private object output; 

        public SensorNode(SensorConfig config) {
            host = config.host;
            topic = config.topic;
            routingKey = config.routingKey;
            type = config.type;
            id = config.id;
            Client client = new Client(host);
            connectionTask = client.CreateConnectionAsync(Enumerable.Repeat(topic, 1).ToArray(), routingKey);
        }

        public void OverrideMessage(string message) {
            this.preProcessedMessage = message;
            this.overriddenMessage = true;
        }

        public void ClearOverride() {
            this.overriddenMessage = false;
        }

        public object Output {
            get => output;
            set => output = value;
        }

        public int ID {
            get => id;
        }

        private async Task<bool> initAsync() {
            connection = connectionTask.Result;
            connectionTask.Dispose();
            connection.Subscribe(topic);
            return true;
        }

        public async Task Process() {
            if(connection is null) {
                _ = await initAsync();
            }
            if(connection.Message is null && !overriddenMessage) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Connection message was null, sensor node: {id}");
                Console.ResetColor();
                return;
            }
            if(!overriddenMessage) {
                preProcessedMessage = connection.Message;
            }
            Console.WriteLine($"SensorNode {id} have recieved {preProcessedMessage}");
            try {
                this.output = Convert.GetValue(preProcessedMessage);
            }
            catch(InvalidCastException ex) {
                _ = Log.Error(ex, "SensorNode", "Attempted to cast message to correct type");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex} SensorNode: Attempted to cast message to correct type");
                Console.ResetColor();
            }
            catch(Exception ex) {
                _ = Log.Error(ex, "SensorNode", "Attempted to cast message to correct type");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{ex} SensorNode: Attempted to cast message to correct type");
                Console.ResetColor();
            }
        }
    }
}
