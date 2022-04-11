using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLFormsApp.Model
{
    public class ProcessNode
    {

		public HashSet<ProcessNode> PreNode { get { return preNode; }}
        public int InputIndex { get { return inputIndex; } set { inputIndex = value; } }

        public List<string> Input { get { return input; } set { input = value; } }

        public string FeatureName { get { return featureName; } set { featureName = value; } }

        public string ProcessName { get { return processName; } set { processName = value; } }

        public int OutputIndex { get { return outputIndex; } set { outputIndex = value; } }

        public List<string> Output { get { return output; } set { output = value; } }

        public HashSet<ProcessNode> NextNodes { get { return nextNode; } }
		
		public Process Process { get { return process; } set { process = value; } }
		public string str { get { return _str; } set { _str = value; } }

		public XmlNode XmlNode { get { return xmlNode; } set { xmlNode = value; } }

		public Scenarios Scenarios { get { return scenarios; } set { scenarios = value; } }
		// 前节点
		HashSet<ProcessNode> preNode = new HashSet<ProcessNode>();
		// 输入索引
		int inputIndex;
		// 输入，之所以是集合，是因为有多个输入可以表示为同一个输入
		List<String> input;
		// 当前feature的名字
		string featureName;
		// 当前process的名字
		string processName;
		// 输出索引
		int outputIndex;
		// 输出，同理
		List<String> output;
		// 当前节点
		HashSet<ProcessNode> nextNode = new HashSet<ProcessNode>();
		private Scenarios scenarios;

		private Process process; 
		private string _str;
        private XmlNode xmlNode;
	}
}
