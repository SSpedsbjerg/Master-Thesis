public class Program {
    static void Main(string[] args) {
        Simulator.Simulator simulator = new Simulator.Simulator(int.Parse(args[0]), int.Parse(args[1]), args[2]);
    }
}