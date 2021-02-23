using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SolutionFamily
{
    public class EngineMethod
    {
        protected Engine Engine { get; }
        public string Name { get; }
        public string ID { get; }
        internal string Component { get; }
        internal string AdapterName { get; }
        public Type ReturnType { get; }
        public EngineParameter[] Parameters { get; }

        internal EngineMethod(Engine engine, XElement element)
        {
            Engine = engine;
            Name = element.Attribute("name").Value;            
            ReturnType = Type.GetType(element.Attribute("returnType").Value);
            AdapterName = element.Attribute("adapterName").Value;

            Component = element.Attribute("component").Value;
            if(!string.IsNullOrEmpty(Component))
            {
            }

            var parms = element.Element("Parameters");
            var list = new List<EngineParameter>();
            if(parms != null)
            {
                foreach(var p in parms.Elements("Parameter"))
                {
                    list.Add(new EngineParameter(p));
                }
            }

            Parameters = list.ToArray();
        }

        public async Task<object> Invoke(params object[] parameters)
        {
            if (Parameters.Length > 0)
            {
                if (Parameters.Length != parameters.Length)
                {
                    throw new Exception("Wrong number of parameters");
                }
            }

            var e = 
                new XElement("CallMethod",
                    new XAttribute("methodName", Name));

            for(int p = 0; p < Parameters.Length; p++)
            {
                e.Add(new XElement("Parameter",
                    new XAttribute("name", Parameters[p].Name),
                    parameters[p]
                    ));
            }

            var content = new StringContent(e.ToString());
            var result = await Engine.Client.PutAsync($"/agent/adapters/{AdapterName}", content);

            return await result.Content.ReadAsStringAsync();
        }
    }
}
