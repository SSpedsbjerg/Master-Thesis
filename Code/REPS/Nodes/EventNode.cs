using REPS.Connections;
using REPS.Enums;
using REPS.Interfaces;
using REPS.Models;
using REPS.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPS.Nodes {
    public class EventNode : INode {

        private int id;
        private string name;
        private SupportedTypes type;
        private List<int> sensorNodeIDs;
        private List<int> eventNodeIDs;
        private IModel model;
        private string modelType;
        private string reportTopic;
        private Client client;
        private Task<Connection> connectionTask;
        private object output;
        private Connection connection;
        private List<INode> nodes;
        private State state = State.Stable;
        private ModelConfig modelConfig;
        private bool initiated = false;

        public EventNode(EventConfig config) {
            this.id = config.id;
            this.name = config.name;
            this.type = config.type;
            this.modelConfig = config.modelConfig;
            this.reportTopic = config.reportTopic;
            this.modelType = config.modelType;
            this.sensorNodeIDs = config.sensorNodeIDs;
            this.eventNodeIDs = config.eventNodeIDs;
            this.client = new Client(config.host);
            this.nodes = new List<INode>();
            string[] topics = { this.reportTopic };
            this.connectionTask = client.CreateConnectionAsync(topics, config.routingKey);
            switch(config.modelConfig.modelType.ToLower()) {
                case "simple":
                    this.modelType = "simple";
                    break;
                default:
                    _ = Log.Error(new Exception("Invalid Modeltype"), "EventNode", "Constructor");
                    this.modelType = "simple";
                    break;
            }
        }
        public object Output {
            get => output;
            set => output = value;
        }

        public int ID {
            get => id;
        }

        public bool AssignNodes(List<INode> nodes) {
            this.nodes = nodes;
            return true;
        }

        public bool AddNodes(List<INode> nodes) {
            this.nodes.AddRange(nodes);
            return true;
        }

        private async Task<bool> initAsync() {
            this.connection = await connectionTask;
            this.initiated = true;
            connectionTask.Dispose();
            if(this.modelType == "simple") {
                model = new SimpleModel(config: modelConfig);
                if(await model.Test() | await model.Process()) {
                    _ = connection.SendMessageAsync(message: output as string ?? output?.ToString() ?? "null"); //if output is already a string type we avoid casting and the problems that follow that, if it is not a string we cast it, as well as this 
                }
                else {
                    _ = connection.SendMessageAsync($"EventNode {id}, {name}, is unable to launch its model");
                    _ = Log.Warning($"EventNode {id}, {name}, is unable to launch its model", "EventNode", "initAsync");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"EventNode {id}, {name}, is unable to launch its model");
                    Console.ResetColor();
                }
            }
            else {
                Console.ForegroundColor= ConsoleColor.Red;
                Console.WriteLine("Event Nodes ModelType is invalid or non existent");
                Console.ResetColor();
            }
            foreach(int id in sensorNodeIDs) {
                this.nodes.Add(NodeController.GetNodeByID<SensorNode>(id));
            }
            foreach(int id in eventNodeIDs) {
                this.nodes.Add(NodeController.GetNodeByID<EventNode>(id));
            }
            
            return true;
        }

        public async Task Process() {
            if(!initiated) {
                _ = await initAsync();
            }
            else {
                int i = 0;
                foreach(string parameter in modelConfig.parameters) {
                    model.UpdateValue(parameter, nodes[i].Output);
                    i++;
                }
                if(await model.Process()) {
                    return;
                }
                else {
                    _ = connection.SendMessageAsync($"EventNode {id}, {name}, failed to process");
                }
            }

            if(state == State.Stable) {
                await connection.SendMessageAsync($"EventNode {id}, {name}, reports stable condition : Statelevel: {State.Stable}");
            }
            else {
                await connection.SendMessageAsync($"EventNode {id}, {name}, reports unstable condition : Statelevel: {state}");
            }
        }
    }
}
