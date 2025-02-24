using REPS;
using REPS.Interfaces;
using REPS.Nodes;

public class Program {
    private static List<INode> nodes = new List<INode> ();
    private static JSONInterpreter? interpreter;


    private static void start(string path, string filename) {
        interpreter = new JSONInterpreter(path, filename);
        nodes = interpreter.GetNodes();
        while(true) {
            process();
        }
    }

    private static async void process() {
        List<Task> tasks = new List<Task>();
        foreach(INode node in nodes) {
            if(node.GetType() == typeof(SensorNode)) {
                var sensorProcess = node.Process();
                tasks.Add(sensorProcess);
            }
        }
        foreach(Task task in tasks) {
            await task;
        }
        tasks.Clear();
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

    static void Main(string[] args) {
        start(args[0], args[1]);
    }
}