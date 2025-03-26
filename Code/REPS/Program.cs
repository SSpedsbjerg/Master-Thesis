using REPS;
using REPS.Connections;
using REPS.Interfaces;
using REPS.Nodes;
using REPS.Structs;

public class Program {
    private static List<INode> nodes = new List<INode> ();
    private static JSONInterpreter? interpreter;
    private static int i = 0;

    private static void start(string path, string filename) {
        interpreter = new JSONInterpreter(path, filename);
        interpreter.init();
        nodes = interpreter.GetNodes();
        NodeController.init(nodes);
        while(true) {
            process();
        }
    }

    private static async void process() {
        List<Task> tasks = new List<Task>();
        
        foreach(INode node in nodes) {
            if(node.GetType() == typeof(SensorNode)) {
                ((SensorNode)node).OverrideMessage($"{i++}");
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

    static void Main(string[] args) {
        DirectoryInfo info = new DirectoryInfo("./");
        FileInfo[] files = info.GetFiles();
        foreach(FileInfo file in files) {
            Console.WriteLine(file.FullName);
        }
        //start(args[0], args[1]);
        if(args.Length == 0) {
            Console.WriteLine("No arguments given");
            start("C:/Users/simon/Documents/GitHub/Master-Thesis/Code/REPS/", "TestConfig.json");
        }
        else {
            Console.WriteLine($"Arguments: {args[0]} {args[1]}");
            start(args[0], args[1]);
        }

    }
}