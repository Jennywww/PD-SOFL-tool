using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLFormsApp.Model
{
    public class Process
    {
        public string ProcessName { get { return _processName; } set { _processName = value; } }
        public string FeaturnName { get { return _featurnName; } set { _featurnName = value; } }
        public string Input { get { return _input; } set { _input = value; } }
        public string Output { get { return _output; } set { _output = value; } }
        public bool Pre { get { return _pre; } set { _pre = value; } }
        public string post { get { return _post; } set { _post = value; } }

        public List<Scenarios> Scenarioses { get { if (scenarioses == null) { scenarioses = new List<Scenarios>(); } return scenarioses; } }
        public List<ProcessNode> ProcessNodes { get { if (processNodes == null) { processNodes = new List<ProcessNode>(); } return processNodes; } }

        public void AddPost(XmlNode xmlNode)
        {
            if (scenarioses == null)
            {
                scenarioses = new List<Scenarios>();
            }
            foreach (XmlNode item in xmlNode.ChildNodes)
            {
                XmlNodeList secnarioses = item.SelectNodes("scenarios");
                foreach (XmlNode secnarios in secnarioses)
                {
                    XmlNode D = item.SelectSingleNode("D");
                    XmlNode G = item.SelectSingleNode("G");
                    Scenarioses.Add(new Scenarios()
                    {
                        G = G.InnerText,
                        D = D.InnerText,
                        DXmlNode = D,
                        GXmlNode = G
                    });
                }
            }
        }
        public Scenarios Match(string input,string output)
        {
            if (scenarioses == null)
            {
                return null;
            }
            Scenarios scenarios = scenarioses.Find((a) => (a.G.Contains(input) && a.D.Contains(output)));
            return scenarios;
        }
        public XmlNode XmlNode { get { return xmlNode; } set { xmlNode = value; } }
        private List<ProcessNode> processNodes;
        private string _featurnName;
        private string _processName;
        private string _input;
        private string _output;
        private bool _pre;
        private string _post;
        private XmlNode xmlNode;
        private List<Scenarios> scenarioses;
    }
}
