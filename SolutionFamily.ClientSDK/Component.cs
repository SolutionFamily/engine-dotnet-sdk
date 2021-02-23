using System.Xml.Linq;

namespace SolutionFamily
{
    public class Component : Node
    {
        public string UUID { get; }

        internal Component(Engine engine, XElement element)
            : base(engine, element)
        {
            UUID = element.Attribute("uuid")?.Value ?? string.Empty;
        }
    }
}
