using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace REPS.Connections {
    public class Connection {
        private string[] topics;
        private IConnection? connection;
        private IChannel? channel;
        private string routingKey;
        private int id;
        private string returnMessage;
        public Connection(int id, string[] topics, string routingKey) {
            this.topics = topics;
            this.routingKey = routingKey;
            if(topics.Length == 0) {
                throw new ArgumentException("Topic length has to be longer than 0");
            }
            this.id = id;
        }

        public int ID {
            get => this.id;
        }

        public string Message { get => this.returnMessage; }

        public async Task<bool> CreateConnectionAsync(ConnectionFactory factory) {
            try {
                this.connection = await factory.CreateConnectionAsync();
                this.channel = await connection.CreateChannelAsync();
                foreach(var topic in this.topics) {
                    await this.channel.ExchangeDeclareAsync(exchange: topic, ExchangeType.Topic);
                }
                return true;
            }
            catch(Exception ex) {
                _ = Log.Error(ex, "Connection", "Failed to create Connection Async");
                return false;
            }
        }

        public async Task<bool> CloseConnectionAsync() {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Warning, connections har not supposed to be opened and closed");
            Console.ResetColor();
            try {
                await channel.CloseAsync();
                await connection.CloseAsync();
                return true;
            }
            catch(Exception ex) {
                _ = Log.Error(ex, "Connection", $"Failed to close connection: Topics: {string.Join(", ", topics)}, routingKey: {routingKey}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to close connection: Topics: {string.Join(", ", topics)}, routingKey: {routingKey}");
                Console.ResetColor();
                return false;
            }
        }

        public async Task<bool> SendMessageAsync(string message) {
            try {
                foreach(string topic in this.topics) {
                    byte[] body = Encoding.UTF8.GetBytes(message);
                    await channel.BasicPublishAsync(exchange: topic, routingKey: this.routingKey, body: body);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Sent: 'routing key': {routingKey}, 'message': {message} to the topic: {topic}");
                    Console.ResetColor();
                }
                return true;
            }
            catch(Exception e) {
                _ = Log.Error(e, "Connection", $"Failed to send message in SendMessageAsync with the following message: {message}, IConnection: {connection.ToString()}, IChannel: {channel.ToString()}, RoutingKey: {routingKey}, Topics: {string.Join(", ", topics)}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to send message: 'routing key': {routingKey}, 'message': {message}");
                Console.ResetColor();
                return false;
            }
        }


        public async void Subscribe(string topic) {
            QueueDeclareOk queueDeclareResult = null;
            try {
                queueDeclareResult = await channel.QueueDeclareAsync();
            }
            catch(NullReferenceException ex) {
                _=Log.Error(ex, "Connection", "Did you forget to startup the docker");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unable to create connection to the broker, did you remeber to start the broker up? If so, start broker and restart this program");
                Console.ResetColor();
                return;
            }
            string queueName = queueDeclareResult.QueueName;
            //TODO: test running multiple topics
            await channel.QueueBindAsync(queue: queueName, exchange: topic, routingKey: routingKey);
            Console.WriteLine("Awaiting for message");
            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) => {
                byte[] body = ea.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Recieved message: {message}");
                Console.ResetColor();
                this.returnMessage = message;
                return Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        }
    }
}
