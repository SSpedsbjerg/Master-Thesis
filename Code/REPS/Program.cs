using REPS;
using REPS.Connections;
using REPS.Interfaces;
using REPS.Nodes;
using REPS.Structs;

public class Program {
    private static List<INode> nodes = new List<INode> ();
    private static JSONInterpreter? interpreter;


    private static void start(string path, string filename) {
        interpreter = new JSONInterpreter(path, filename);
        interpreter.init();
        nodes = interpreter.GetNodes();
        while(true) {
            process();
        }
    }

    private static async void process() {
        List<Task> tasks = new List<Task>();
        
        foreach(INode node in nodes) {
            if(node.GetType() == typeof(SensorNode)) {
                Task sensorProcess = node.Process();
                tasks.Add(sensorProcess);
            }
        }
        foreach(Task task in tasks) {
            await task;
        }
        //tasks.Clear();
        //does not reach this point
        if(interpreter.updateRate == REPS.Enums.UpdateRate.Fast) {
            foreach(INode node in nodes) {
                if(node is EventNode) {
                    tasks.Add(node.Process());
                }
            }
            foreach(Task task in tasks) {
                await task;
            }
        }
        else if(interpreter.updateRate == REPS.Enums.UpdateRate.Sequencial) {
            foreach(INode node in nodes) {
                if(node is EventNode) {
                    await node.Process();
                }
            }

        }
    }

    public static async Task run(Client client) {
        string[] topics = { "test" };
        Connection connection = await client.CreateConnectionAsync(topics, string.Empty);
        var succes = connection.SendMessageAsync("Hello World");
        Console.WriteLine(succes);
        //await connection.CloseConnectionAsync();
    }

    private 

    static void Main(string[] args) {
        //start(args[0], args[1]);
        Client client = new Client("localhost");

        /*
        SensorConfig sensorConfig = new SensorConfig();
        sensorConfig.id = 0;
        sensorConfig.topic = "test";
        sensorConfig.host = "localhost";
        sensorConfig.type = REPS.Enums.SuportedTypes.INT;
        sensorConfig.routingKey = string.Empty;
        SensorNode sensor = new SensorNode(sensorConfig);
        Task task = sensor.Process();
        task.Wait();
        run(client).Wait();
        Console.WriteLine("Waiting...");
        Thread.Sleep(10000);
        task = sensor.Process();
        task.Wait();
        Console.WriteLine(sensor.Output);
        Console.ReadLine();
        */

        start("C:/Users/simon/Documents/GitHub/Master-Thesis/Code/REPS/", "TestConfig.json");//real test starts here
    }
}