using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SolutionFamily
{
    public class Engine
    {
        internal HttpClient Client { get; }

        public Device[] Devices { get; private set; }

        // TODO: re-parent these intot he proper components
        public EngineMethod[] Methods { get; private set; }

        private Engine()
        {
            Client = new HttpClient();
        }

        public Engine(string address)
            : this()
        {
            if(!address.StartsWith("http://"))
            {
                Init(new Uri($"http://{address}"));
            }
            else
            {
                Init(new Uri(address));
            }
        }

        public Engine(Uri endpoint)
            : this()
        {
            Init(endpoint);
        }

        private void Init(Uri endpoint)
        {
            Client.BaseAddress = endpoint;
        }

        public async Task SetDataItem(string id, object value)
        {
            DataItem FindDataItem(Node node, string id)
            {
                if (node.DataItems.Contains(id))
                {
                    return node.DataItems[id];
                }
                else
                {
                    foreach (var n in node.Components)
                    {
                        var di = FindDataItem(n, id);
                        if (di != null) return di;
                    }
                }
                return null;
            }

            foreach (var d in Devices)
            {
                var di = FindDataItem(d, id);
                if(di != null)
                {
                    await di.SetValue(value);
                    return;
                }
            }

            throw new Exception($"Data Item with ID '{id}' not found");
        }

        public async Task Refresh()
        {
            await LoadMethods();
            await LoadDevices();
        }

        private async Task LoadMethods()
        {
            var response = await Client.GetAsync("/agent/adapters");

            if (response.IsSuccessStatusCode)
            {
                var doc = XDocument.Parse(await response.Content.ReadAsStringAsync());

                var list = new List<EngineMethod>();

                foreach(var adapter in doc.Element("Adapters").Elements("Adapter"))
                {
                    var deviceID = adapter.Attribute("deviceID");
                    var methods = adapter.Element("Methods");
                    if(methods?.HasElements ?? false)
                    {
                        foreach(var m in methods.Elements("Method"))
                        {
                            list.Add(new EngineMethod(this, m));
                        }
                    }
                }

                Methods = list.ToArray();
            }
        }

        private async Task LoadDevices()
        {
            var response = await Client.GetAsync("/mtc/probe");

            if(response.IsSuccessStatusCode)
            {
                XNamespace mtc = "urn:mtconnect.com:MTConnectDevices:1.1";

                var doc = XDocument.Parse(await response.Content.ReadAsStringAsync());

                var devices = doc
                    .Element(mtc + "MTConnectDevices")
                    .Element(mtc + "Devices");

                var list = new List<Device>();

                foreach(var device in devices.Elements(mtc + "Device"))
                {
                    list.Add(new Device(this, device));
                }

                Devices = list.ToArray();
            }
        }
    }
}
