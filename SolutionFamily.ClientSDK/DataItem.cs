using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SolutionFamily
{
    public class DataItem
    {
        private Engine Engine { get; }
        public string Category { get; private set; }
        public string MTConnectType { get; private set; }
        public string Name { get; private set; }
        public string ID { get; private set; }
        public bool Writable { get; private set; }
        public string ValueType { get; private set; }
        public DataItemValue Value { get; set; }

        internal DataItem(Engine engine, XElement element)
        {
            Engine = engine;
            Category = element.Attribute("category")?.Value ?? string.Empty;
            MTConnectType = element.Attribute("type")?.Value ?? string.Empty;
            Name = element.Attribute("name")?.Value ?? string.Empty;
            ID = element.Attribute("id")?.Value ?? string.Empty;
            Writable = bool.Parse(element.Attribute("writable")?.Value ?? "false");
            ValueType = element.Attribute("valueType")?.Value ?? string.Empty;
        }

        public async Task SetValue(object value)
        {
            if(!Writable)
            {
                throw new Exception("Item is read-only");
            }

            var e = new XElement("DataItems",
                new XElement("DataItem",
                    new XAttribute("dataItemId", this.ID),
                    new XElement("Value", value.ToString())
                ));

            var content = new StringContent(e.ToString());
            var result = await Engine.Client.PostAsync($"/agent/data", content);
        }

        public override string ToString()
        {
            return ID;
        }
    }
}
