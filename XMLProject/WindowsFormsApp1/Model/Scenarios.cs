using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLFormsApp.Model
{
   public class Scenarios
    {
        public string D { get { return d; } set { d = value; } }
        private string d;
        public string G { get { return g; } set { g = value; } }
        private string g;

        public XmlNode DXmlNode { get { return dXmlNode; } set { dXmlNode = value; } }
        private XmlNode dXmlNode;
        public XmlNode GXmlNode { get { return gXmlNode; } set { gXmlNode = value; } }
        private XmlNode gXmlNode;
        public int Id { get { return id; } set { id = value; } }
        private int id;


    }
}
