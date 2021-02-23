using System.Xml.Linq;

namespace SolutionFamily
{
    public class Device : Node
    {
        internal Device(Engine engine, XElement element)
            : base(engine, element)
        {
        }
    }
}
