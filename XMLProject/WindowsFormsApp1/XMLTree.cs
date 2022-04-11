using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XMLFormsApp.Model;

namespace XMLFormsApp
{
    public partial class XMLTree : UserControl
    {

        private Dictionary<string, Feature> leefNodes;
        public Action<string, Feature, Dictionary<string, Feature>, XmlDocument> loadFeature;
        public Action<string> featureClick;
        public XmlDocument xmlDocument = null;
        private string description;
        bool isEdit = true;
        List<string> nodeNames;
        string Name;
        List<string> attritubeName;
        public string rootName { get { return treeView.Nodes[0].Text; } }
        public XMLTree()
        {
            InitializeComponent();
        }

        private void XMLTree_Load(object sender, EventArgs e)
        {
        }
        public void SetXML(XmlDocument xmlDocument)
        {
            leefNodes.Clear();
            nodeNames = new List<string>();
            RecursionTreeControl(xmlDocument.DocumentElement, treeView.Nodes);

            this.Refresh();
        }
        public void SetXML(XmlDocument xmlDocument, string Name, List<string> attritubeName)
        {
            leefNodes.Clear();
            treeView.Nodes.Clear();
            nodeNames = new List<string>();
            this.Name = Name;
            this.attritubeName = attritubeName;
            leefNodes = new Dictionary<string, Feature>();
            //topNodeTextBox.Text = xmlDocument.DocumentElement.Name;
            foreach (XmlNode item in xmlDocument.DocumentElement.ChildNodes)
            {
                if (item.Name == Name)
                {
                    Feature feature = RecursionTreeControl(item, treeView.Nodes, attritubeName);
                    if (loadFeature != null)
                    {
                        loadFeature(item.FirstChild.Attributes["name"].Value, feature, leefNodes, xmlDocument);
                    }
                }
            }

            this.Refresh();
        }

        public void SetXML(string path)
        {
            treeView.Nodes.Clear();
            nodeNames = new List<string>();

            xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            RecursionTreeControl(xmlDocument.DocumentElement, treeView.Nodes);

            this.Refresh();
        }

        public void SetXML(string path, string Name, List<string> attritubeName)
        {
            treeView.Nodes.Clear();
            nodeNames = new List<string>();
            this.Name = Name;
            this.attritubeName = attritubeName;
            xmlDocument = new XmlDocument();
            leefNodes = new Dictionary<string, Feature>();
            xmlDocument.Load(path);
            //topNodeTextBox.Text = xmlDocument.DocumentElement.Name;
            foreach (XmlNode item in xmlDocument.DocumentElement.ChildNodes)
            {
                if (item.Name == Name)
                {
                    Feature feature = RecursionTreeControl(item, treeView.Nodes, attritubeName);
                    if (loadFeature != null)
                    {
                        loadFeature(item.FirstChild.Attributes["name"].Value, feature, leefNodes, xmlDocument);
                    }
                }
            }

            this.Refresh();
        }
        /// <summary>
        /// RecursionTreeControl:表示将XML文件的内容显示在TreeView控件中
        /// </summary>
        /// <param name="xmlNode">将要加载的XML文件中的节点元素</param>
        /// <param name="nodes">将要加载的XML文件中的节点集合</param>6
        private Feature RecursionTreeControl(XmlNode xmlNode, TreeNodeCollection nodes, List<string> attributeName, int level = 0, string namenode = "root")
        {
            Feature feature = new Feature()
            {
                Level = level,
                Name = namenode,
            };
            nodeNames.Add(namenode);
            if (attributeName.Count == 0)
            {
                RecursionTreeControl(xmlNode, nodes);
                return feature;
            }
            if (xmlNode.ChildNodes.Count == 0)
            {
                foreach (XmlAttribute item in xmlNode.Attributes)
                {
                    if (attributeName.Contains(item.Name))
                    {
                        leefNodes.Add(item.Value, feature);
                        return feature;
                    }
                }
            }
            foreach (XmlNode node in xmlNode.ChildNodes)//循环遍历当前元素的子元素集合
            {
                string name = "";
                if (node.Attributes != null)
                {
                    foreach (XmlAttribute item in node.Attributes)
                    {
                        if (attributeName.Contains(item.Name))
                        {
                            name = item.Value;
                        }
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        TreeNode new_child = new TreeNode();
                        new_child.Tag = level;
                        new_child.Text = name;
                        nodes.Add(new_child);//向当前TreeNodeCollection集合中添加当前节点
                        feature.AddChild(RecursionTreeControl(node, new_child.Nodes, attributeName, level + 1, name));//调用本方法进行递归
                    }
                }
                //定义一个TreeNode节点对象
                //new_child.Name = node.Attributes["Name"].Value;
                //new_child.Text = node.Attributes["Text"].Value;
            }
            return feature;
        }
        private void RecursionTreeControl(XmlNode xmlNode, TreeNodeCollection nodes,int level=0)
        {
            if (isEdit)
            {
                foreach (XmlNode node in xmlNode.ChildNodes)//循环遍历当前元素的子元素集合
                {

                    TreeNode new_child = new TreeNode();
                    new_child.Tag = level;
                    new_child.Text = node.Name;
                    nodes.Add(new_child);//向当前TreeNodeCollection集合中添加当前节点
                    RecursionTreeControl(node, new_child.Nodes,level+1);//调用本方法进行递归
                                                                //定义一个TreeNode节点对象
                                                                //new_child.Name = node.Attributes["Name"].Value;
                                                                //new_child.Text = node.Attributes["Text"].Value;
                }
            }
        }
        //双击时修改
        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (featureClick != null)
                {
                    featureClick(treeView.SelectedNode.Text);
                }
                //Point ClickPoint = new Point(e.X, e.Y);
                //TreeNode CurrentNode = treeView.GetNodeAt(ClickPoint);
                //if (CurrentNode != null)//判断你点的是不是一个节点
                //{
                //    treeView.SelectedNode = CurrentNode;//选中这个节点
                //    treeView.LabelEdit = true;
                //    treeView.SelectedNode.BeginEdit();
                //}
            }
        }

        private void TreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
        }

        private void addSubNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int level= Convert.ToInt32(treeView.SelectedNode.Tag);
                Console.WriteLine(level);
                treeView.SelectedNode.Nodes.Add("新建子节点");
                //toolStripStatusLabel1.Text = "新建子节点";
            }
            catch (Exception ex)
            {
                //toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void deleteNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode treeNode = treeView.SelectedNode;
                treeNode.Remove();
            }
            catch (Exception ex)
            {

                //toolStripStatusLabel1.Text = ex.Message;
            }
        }
        string xmlLine = "";




        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (xmlDocument != null && e.Button == MouseButtons.Right)
                {
                    TreeNode treeNode = treeView.SelectedNode;
                    //if (!leefNodes.ContainsKey(treeNode.Text))
                    //{
                    //    featureToolStripMenuItem.Enabled = false;
                    //}
                    //else
                    //{
                    //    featureToolStripMenuItem.Enabled = true;
                    //}
                    contextMenuStrip1.Show(this, new Point(e.X, e.Y));
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("treeView", ex.ToString());
            }

        }


        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string desription = xmlDocument.SelectSingleNode("featureModel/struct/and/description").InnerText;
            DescriptionForm descriptionForm = new DescriptionForm(desription);
            descriptionForm.okAction += UpdateXML;
            descriptionForm.ShowDialog();
        }

        private void GetTreeNode(TreeNode treeNode)
        {
            nodeNames.Add(treeNode.Text);
            foreach (TreeNode item in treeNode.Nodes)
            {
                GetTreeNode(item);
            }
        }
        private bool CheckUpdate()
        {
            nodeNames.Clear();
            GetTreeNode(treeView.Nodes[0]);
            foreach (var item in nodeNames)
            {
                if (nodeNames.FindAll(a=>(a.Equals(item))).Count != 1)
                {
                    return false;
                }
            }
            return true;
        }
        private void UpdateXML(string description)
        {
            if (treeView.Nodes.Count == 1)
            {
                if (CheckUpdate())
                {
                    XmlNode rootxml = xmlDocument.SelectSingleNode("featureModel/struct");
                    rootxml.RemoveAll();
                    XmlNode xmlNode = UpdateNode(treeView.Nodes[0], true, description);
                    rootxml.AppendChild(xmlNode);
                    Console.WriteLine(xmlDocument.InnerXml);
                    SetXML(xmlDocument, Name, attritubeName);
                }
                else
                {
                    MessageBox.Show("If duplicate nodes exist, check them");
                }
            }
        }
        private XmlNode UpdateNode(TreeNode treeNode,bool isRoot,string Descriptio= "")
        {
            XmlElement xmlElement = null;
            if (treeNode.Nodes.Count == 0)
            {
                xmlElement = xmlDocument.CreateElement("feature");
                //mandatory = "true" name = "E"
                xmlElement.SetAttribute("mandatory", "true");
                xmlElement.SetAttribute("name", treeNode.Text);
                return xmlElement;
            }
            xmlElement = xmlDocument.CreateElement("and");
            if (isRoot)
            {
                xmlElement.SetAttribute("abstract", "true");
                XmlElement xmlElementDescription = xmlDocument.CreateElement("description");
                xmlElementDescription.InnerText = description;
                xmlElement.AppendChild(xmlElementDescription);
            }
            //mandatory = "true" name = "E"
            xmlElement.SetAttribute("mandatory", "true");
            xmlElement.SetAttribute("name", treeNode.Text);

            foreach (TreeNode item in treeNode.Nodes)
            {
                xmlElement.AppendChild(UpdateNode(item,false));
            }
            return xmlElement;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void featureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (featureClick != null)
            {
                if (leefNodes.ContainsKey(treeView.SelectedNode.Text))
                {
                    featureClick(treeView.SelectedNode.Text);
                }
            }
        }
    }

}
