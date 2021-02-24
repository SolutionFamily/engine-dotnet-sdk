using SolutionFamily;
using System;
using System.Threading.Tasks;

namespace show_structure
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var address = "http://localhost:8080";
            var engine = new Engine(address);

            Console.WriteLine("Querying Engine Structure...");

            await engine.Refresh();

            void PrintNode(Node node, int indent)
            {
                Console.WriteLine($"{new string(' ', indent)}_[{node.Name}]");

                foreach (var di in node.DataItems)
                {
                    Console.WriteLine($"{new string(' ', indent)}  | -[name: {di.Name} id: {di.ID} ({di.ValueType}) {(di.Writable ? "writable" : "not writable")}]");

                    // TODO: show methods - they need to be re-parented in the SDK
                    foreach (var c in node.Components)
                    {
                        PrintNode(c, indent + 3);
                    }
                }
            }

            // print the entire Engine structure
            foreach (var d in engine.Devices)
            {
                PrintNode(d, 1);
            }

            Console.ReadKey();
        }
    }
}
