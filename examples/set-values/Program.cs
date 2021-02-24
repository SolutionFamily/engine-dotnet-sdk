using SolutionFamily;
using System;
using System.Threading.Tasks;

namespace set_values
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var address = "http://localhost:8080";
            var engine = new Engine(address);
            await engine.Refresh();

            var valueSet = new []
            {
                new { ID = "EngineInfo.EngineName", Value = (object)"New Engine Name" },
                new { ID = "EngineInfo.Location.Latitude", Value = (object)41.888332 },
                new { ID = "EngineInfo.Location.Longitude", Value = (object)-87.602566 }
            };

            Console.WriteLine($"Setting {valueSet.Length} values...");

            foreach(var v in valueSet)
            {
                await engine.SetDataItem(v.ID, v.Value);
                Console.WriteLine($"Set {v.ID} = {v.Value}");
            }

            Console.WriteLine($"done.");
            Console.ReadKey();
        }
    }
}
