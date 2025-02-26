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
    public class EventNode : INode {

        private int id;
        private string name;
        private SuportedTypes type;
        private List<int> nodeIDs;
        private IModel model;
        private string modelType;
        private string reportTopic;
        private Client client;
        private Task<Connection> connectionTask;
        private object output;
        private Connection connection;
        private List<INode> nodes;
        private State state = State.Stable;

        public EventNode(EventConfig config) {
            this.id = config.id;
            this.name = config.name;
            this.type = config.type;
            this.model = config.model;
            this.reportTopic = config.reportTopic;
            this.modelType = config.modelType;
            this.nodeIDs = config.nodeIDs;
            this.client = new Client(config.host);
            this.connectionTask = client.CreateConnectionAsync(Enumerable.Repeat(reportTopic, 1).ToArray(), config.routingKey);
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
            connectionTask.Dispose();
            return true;
        }

        public async Task Process() {
            _ = initAsync();
            //TODO: IMPLEMENT MODEL STUFF HERE
            if(state == State.Stable) {
                await connection.SendMessageAsync($"EventNode {id}, {name}, reports stable condition : Statelevel: {State.Stable}");
            }
            else {
                await connection.SendMessageAsync($"EventNode {id}, {name}, reports unstable condition : Statelevel: {state}");
            }
        }
    }
}
