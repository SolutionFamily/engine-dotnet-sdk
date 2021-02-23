using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SolutionFamily
{
    public abstract class Node
    {
        protected Engine Engine { get; }
        public string Name { get; private set; }
        public string ID { get; private set; }
        public DataItemCollection DataItems { get; private set; }
        public Component[] Components { get; private set; }
        public EngineMethod[] Methods { get; private set; }

        protected Node(Engine parent, XElement element)
        {
            Engine = parent;
            Name = element.Attribute("name")?.Value ?? string.Empty;
            ID = element.Attribute("id")?.Value ?? string.Empty;

            GetDataItems(element);
            GetComponents(element);

            _ = RefreshValues();
        }

        public override string ToString()
        {
            return Name;
        }

        protected void GetDataItems(XElement element)
        {
            XNamespace mtc = "urn:mtconnect.com:MTConnectDevices:1.1";
            var dataitems = element.Element(mtc + "DataItems");

            var list = new Dictionary<string, DataItem>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var e in dataitems.Elements(mtc + "DataItem"))
            {
                var di = new DataItem(Engine, e);
                list.Add(di.ID, di);
            }
            DataItems = new DataItemCollection(list);
        }

        protected void GetComponents(XElement element)
        {
            XNamespace mtc = "urn:mtconnect.com:MTConnectDevices:1.1";
            var components = element.Element(mtc + "Components");

            var list = new List<Component>();
            foreach (var e in components.Elements(mtc + "Component"))
            {
                list.Add(new Component(Engine, e));
            }
            Components = list.ToArray();
        }

        public async Task RefreshValues()
        {
            var response = await Engine.Client.GetAsync($"/mtc/current?path={this.ID}");

            if (response.IsSuccessStatusCode)
            {
                XNamespace sns = "urn:mtconnect.com:MTConnectStreams:1.1";

                var doc = XDocument.Parse(await response.Content.ReadAsStringAsync());

                var streams = doc
                    .Element(sns + "MTConnectStreams")
                    .Elements(sns + "Streams");

                var list = new List<Device>();

                foreach (var device in streams.Elements(sns + "DeviceStream"))
                {
                    var deviceid = device.Attribute("id").Value;

                    foreach(var evt in device.Elements(sns + "Events"))
                    {
                        foreach (var v in evt.Elements())
                        {
                            var a = v.Elements().ToArray();

                            var diid = v.Attribute("dataItemId").Value;
                            var ts = v.Attribute("timestamp").Value;
                            var val = v.Value;

                            if (DataItems[diid].Value == null)
                            {
                                DataItems[diid].Value = new DataItemValue(ts, val);
                            }
                            else
                            {
                                DataItems[diid].Value.TimeStamp = ts;
                                DataItems[diid].Value.Value = val;
                            }
                        }
                    }
                }

            }
        }
    }
}
