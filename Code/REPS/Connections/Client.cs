using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;


namespace REPS.Connections {
    public class Client {
        private string hostname;
        private ConnectionFactory factory;
        private List<Connection> connections;
        private int id;
        public Client(string hostname) {
            this.hostname = hostname;
            this.factory = new ConnectionFactory { HostName = this.hostname };
            id = 0;
            connections = new List<Connection>();
        }

        public string Hostname { get { return hostname; } }

        public async Task<Connection> CreateConnectionAsync(string[] topics, string routingKey) {
            Connection connection = new Connection(id, topics, routingKey);
            id++;
            await connection.CreateConnectionAsync(this.factory);
            connections.Add(connection);
            return connection;
        }

    }
}
