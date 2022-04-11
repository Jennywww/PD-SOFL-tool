using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XMLFormsApp.Common;

namespace XMLFormsApp.Model
{
    public class Feature
    {
        public XmlNode XmlNode { get { return xmlNode; } set { xmlNode = value; } }
        public int Level { get { return level; } set { level = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public Dictionary<string,Process> Processes { get { if (processes == null) processes = new Dictionary<string, Process>();return processes; } }
        public List<Feature> ChildFeatures { get { if (childFeatures == null) childFeatures = new List<Feature>(); return childFeatures; } }


        public Feature ParentFeature { get { return parentNode; } }

        public void AddChild(Feature feature)
        {
            ChildFeatures.Add(feature);
            feature.parentNode = this;
        }
        public bool haveChild()
        {
            if (ChildFeatures == null || childFeatures.Count == 0)
            {
                return false;
            }
            return true;
        }
        public void setForceNotIsLeafNode(bool isTure)
        {
            _isLeafNode = isTure;
        }

        public Feature GetFeature(string name,bool isNodeName =false)
        {
            if (!isNodeName)
            {
                if (this.Name.Equals(name))
                {
                    return this;
                }
            }
            else
            {
                if (this.xmlNode!=null&&this.xmlNode.Attributes["name"].Value.Equals(name))
                {
                    return this;
                }
            }
            if (childFeatures != null) {
                Feature result = null;
                foreach (var item in childFeatures)
                {
                    result = item.GetFeature(name,isNodeName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
        public void AddProcesses(List<ProcessNode> processNodes)
        {
            if (this.processNodes == null)
            {
                this.processNodes = new List<Dictionary<string, ProcessNode>>();
            }
            Dictionary<string, ProcessNode> pairs = new Dictionary<string, ProcessNode>();
            foreach (var item in processNodes)
            {
                string name = item.ProcessName + item.InputIndex + item.OutputIndex;
                pairs.Add(name,item);
            }
            this.processNodes.Add(pairs);
        }
        public void ClearProcesses()
        {
            if (processNodes != null)
            {
                processNodes.Clear();
            }
            if (processes != null)
            {
                processes.Clear();
            }
            if (childFeatures != null) { 
                foreach (var item in childFeatures)
                {
                    item.ClearProcesses();
                }
            }
        }
        public Feature()
        {

        }
        public Feature(Feature feature1, Feature feature2) : base()
        {
            Name = feature1.Name +"_" +feature2.Name;
            processNodes = ProcessNodeHelper.FeatureCombine(feature1.GetProessNodes(), feature2.GetProessNodes(), true);
        }
        public List<Dictionary<string, ProcessNode>> GetProessNodes()
        {
            return processNodes;
        }
        private List<Dictionary<string,ProcessNode>> processNodes = null;
        private Dictionary<string, Process> processes;
        private string _name;
        private int level = -1;
        private Feature parentNode = null;
        private List<Feature> childFeatures=null;
        private XmlNode xmlNode;
        private bool _isLeafNode = true;
    }
}
