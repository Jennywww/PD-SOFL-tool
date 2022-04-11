using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLFormsApp.Model
{
    public class ProcessRow
    {
        public List<ProcessNode> ProcessNodes { get { if (processNodes == null) processNodes = new List<ProcessNode>(); return processNodes; } }
        public FeatureRow FeatureRow { get { return featureRow; }  set { featureRow = value; } }

        public string Post { get { return post; } set { post = value; } }
        private FeatureRow featureRow;

        private List<ProcessNode> processNodes;
        private string post;
    }
}
