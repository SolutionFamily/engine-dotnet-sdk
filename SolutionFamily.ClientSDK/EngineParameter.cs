using System;
using System.Xml.Linq;

namespace SolutionFamily
{
    public class EngineParameter
    {
        internal string Name { get; }
        public Type ParameterType { get; }

        internal EngineParameter(XElement element)
        {
            Name = element.Attribute("name").Value;
            ParameterType = Type.GetType(element.Attribute("type").Value);
        }
    }
}
