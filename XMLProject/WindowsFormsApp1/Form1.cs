using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using XMLFormsApp.Common;
using XMLFormsApp.Model;

namespace XMLFormsApp
{
    public partial class Form1 : Form
    {
        int CLOSE_SIZE = 15;
        private int currentOrderIndex;
        private List<string> orders;
        private bool isPost = false;
        private string xmlString = "product.xml";
        private string formalString = "specification";
        private string path;
        private List<FeatureRow> featureRows;
        private string initialXml =null;
        private XmlDocument currentXml =null;
        Dictionary<string, Feature> leefNodes;
        Feature rootFeature;
        string name;
        XmlDocument productXML;
        Font f = new Font("Time New Roman",9);
        string matchOrder;
        public Form1() 
        {
            InitializeComponent();
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
            ProcessNodeHelper.model = Common.Model.NEW;
            this.xmlTree1.featureClick += ShowFeature;

            GetControls(this);
            menuStrip1.Font = f;
        }
        private void GetControls(Control fatherControl)
        {
            
            fatherControl.Font = f;
            if (fatherControl.Controls == null)
            {
                return;
            }
            Control.ControlCollection sonControls = fatherControl.Controls;
            //遍历所有控件
            foreach (Control control in sonControls)
            {

                    GetControls(control);

            }
        }
        private void UpdateFeaturePage(TabPage page,string feature)
        {
            FeaturePage featurePage = page.Controls[feature] as FeaturePage;
            string featureXML = ProcessNodeHelper.Standardization(currentXml, feature);
            featurePage.SetString(featureXML);
        }
        private void ShowFeature(string feature)
        {
            if (leefNodes.ContainsKey(feature))
            {
                if (featureTabControl.TabPages.ContainsKey(feature) == false)
                {
                    string featureXML = ProcessNodeHelper.Standardization(currentXml, feature);
                    FeaturePage featurePage = new FeaturePage(featureXML);
                    featurePage.Name = feature;
                    //TabPage tabPage = new TabPage();
                    //tabPage.Controls.Add(featurePage);
                    //tabPage.Font = f;
                    //featurePage.Dock = DockStyle.Fill;
                    //tabPage.Text = feature;
                    featureTabControl.TabPages.Add(feature, feature);
                    TabPage tabPage = featureTabControl.TabPages[feature];
                    tabPage.Controls.Add(featurePage);
                    tabPage.Font = f;
                    featurePage.Dock = DockStyle.Fill;
                    tabPage.Text = feature + "    ";
                    featureTabControl.SelectedTab = tabPage;
                }
                featureTabControl.SelectedTab = featureTabControl.TabPages[feature];
                updataScenarioPath();
            }
        }
        private void updataScenarioPath()
        {
            CurrencyManager cm = (CurrencyManager)BindingContext[featureDataGridView.DataSource];
            char[] split = { ',', ')', '(', ' ', '\r', '\n', '\0' };
            cm.SuspendBinding();
            int count = 0;
            foreach (DataGridViewRow item in featureDataGridView.Rows)
            {
                if (Convert.ToInt32(item.Cells["id"].Value) != -1)
                {
                    string path = item.Cells["NameColumn"].Value.ToString();
                    string[] s = path.Split(split, StringSplitOptions.RemoveEmptyEntries);
                    List<string> slits = new List<string>(s);
                    int c = 0;
                    foreach (TabPage tab in featureTabControl.TabPages)
                    {
                       string result = slits.Find(a => (tab.Text.Trim().Equals(a)));
                       if(result != null)
                        {
                            c++;
                        }
                    }
                    item.Visible = c == slits.Count && !Convert.ToBoolean(item.Cells["isDelete"].Value);
                    if (item.Visible)
                    {
                        count = 1;
                    }
                }
            }
            cm.ResumeBinding();
            if (count == 1)
            {
                addButton.Enabled = true;
                editButton.Enabled = true;
                delButton.Enabled = true;
            }
            featureDataGridView.ClearSelection();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            //dialog.RootFolder = Environment.SpecialFolder.Programs;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                if (File.Exists(foldPath +"/"+ xmlString))
                {
                    currentXml = null;
                    path = foldPath;
                    xmlTree1.SetXML(foldPath + "/" + xmlString,"struct",new List<string>() {"name" });
                    fileTree1.SetRootPath(foldPath);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            xmlTree1.loadFeature += loadFormalFile;
            featureDataGridView.AutoGenerateColumns = false;
        }
        #region readFormat
        private void updateProcess()
        {
            webBrowser1.DocumentText = currentXml.InnerXml;
            rootFeature.ClearProcesses();
            if (this.productXML == null)
            {
                xmlTree1.SetXML(path + "/" + xmlString, "struct", new List<string>() { "name" });
            }
            else
            {
                xmlTree1.SetXML(productXML, "struct", new List<string>() { "name" });
            }
            updateScenariose();
        }
        private void updateScenariose()
        {
            foreach (TabPage tab in featureTabControl.TabPages)
            {
                UpdateFeaturePage(tab, tab.Text);
            }
        }
        private void loadFormalFile(string name,Feature feature,Dictionary<string,Feature> leefNodes, XmlDocument productXML)
        {
            this.leefNodes = leefNodes;
            this.rootFeature = feature;
            this.name = name;
            this.productXML = productXML;
            XmlDocument xmlDocument = currentXml;
            if (currentXml == null)
            {
                string formalPath = string.Join("/", path, formalString, name + ".formal");
                string xml = File.ReadAllText(formalPath);
                xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.LoadXml(xml.Trim());
                }
                catch
                {
                    try
                    {
                        xml = ProcessNodeHelper.FilterXML(xml, "types");
                        xml = ProcessNodeHelper.FilterXML(xml, "var");
                        xml = ProcessNodeHelper.FilterXML(xml, "inv");
                        //xml = ProcessNodeHelper.FilterXML(xml, "post");
                        xmlDocument.LoadXml(xml.Trim());
                    }
                    catch (Exception exx)
                    {
                        MessageBox.Show(exx.Message);
                    }
                }
                initialXml = xml.Trim();
            }
            LoadFeatures(xmlDocument,feature, leefNodes);
        }

        private void LoadFeatures(XmlDocument xml, Feature rootFeature, Dictionary<string,Feature> leefNodes)
        {
            currentXml = xml;
            List<XmlNode> deleteNode = new List<XmlNode>();
            List<Feature> allFeature = new List<Feature>();
            foreach (XmlNode item in xml.DocumentElement.ChildNodes)
            {
                if (item.Name == "feature")
                {
                    string Name = item.Attributes["name"].Value;
                    Feature  feature = rootFeature.GetFeature(Name);
                    if (feature == null)
                    {
                        feature = new Feature()
                        {
                            Name = Name
                        };
                    }
                    feature.XmlNode = item;
                    XmlNode module = item.FirstChild;
                    bool haveChild = false;
                    if (module != null)
                    {
                        foreach (XmlNode processNode in module.ChildNodes)
                        {
                            if (processNode.Name == "process")
                            {
                                haveChild = true;
                                Process process = new Process();
                                process.ProcessName = processNode.Attributes["name"].InnerText;
                                process.FeaturnName = feature.Name;
                                process.Input = processNode.SelectNodes("inputs")[0].InnerText;
                                process.Output = processNode.SelectNodes("outputs")[0].InnerText;
                                process.Pre = Convert.ToBoolean(processNode.SelectNodes("pre")[0].InnerText);
                                process.post = processNode.SelectNodes("post")[0].InnerText;
                                //process.ProcessNodes.AddRange(ProcessNodeHelper.CreateProcessNode(process));
                                if(ProcessNodeHelper.model == Common.Model.NEW)
                                {
                                    XmlNodeList xmlNodeList = processNode.SelectNodes("post/scenarios");
                                    foreach (XmlNode node in xmlNodeList)
                                    {
                                        XmlNode D = node.SelectSingleNode("D");
                                        XmlNode G = node.SelectSingleNode("G");
                                        process.Scenarioses.Add(new Scenarios()
                                        {
                                            D = D.InnerText,
                                            G = G.InnerText,
                                            GXmlNode = G,
                                            DXmlNode = D
                                        });
                                    }
                                }
                                process.XmlNode = processNode;
                                //feature.AddProcesses(process.ProcessNodes);
                                feature.Processes.Add(process.ProcessName, process);
                            }
                        }
                    }
                    feature.setForceNotIsLeafNode(haveChild);
                    if (!leefNodes.Any(a => (a.Key.Equals(feature.Name))))
                    {
                        //ReserveForBus
                        //ReserveforBus
                        deleteNode.Add(item);
                    }
                    else
                    {
                        allFeature.Add(feature);
                    }
                }
            }
            foreach (var item in leefNodes)
            {
                if ((allFeature.Find(a => (a.Name.Equals(item.Key))))==null)
                {
                    item.Value.XmlNode = AddFeature(item.Key);
                    allFeature.Add(item.Value);
                }
            }
            foreach (var item in deleteNode)
            {
                xml.DocumentElement.RemoveChild(item);
            }
            Dictionary<string, List<Dictionary<string, ProcessNode>>> featureLinkMap = null;
            string retFeatureName = ProcessNodeHelper.LeafNodeCombine(allFeature,rootFeature,ref featureLinkMap);
            if(featureLinkMap==null || string.IsNullOrEmpty(retFeatureName))
            {
                MessageBox.Show("解析错误","无法正确解析formal文件");
                return;
            }
            webBrowser1.DocumentText = xml.InnerXml;
            xmlTextBox.Text = ProcessNodeHelper.Standardization(xml);
            DataFridViewBindingData(retFeatureName,featureLinkMap,allFeature);
            //addButton.Enabled = false;
            //editButton.Enabled = false;
            //delButton.Enabled = false;
            updataScenarioPath();

            //orders = new List<string>();
            //foreach (var item in featureRows)
            //{
            //    if (!leefNodes.ContainsKey(item.Name))
            //    {
            //        orders.Add(item.Name);
            //    }
            //}
            orders = ProcessNodeHelper.GetCalculate(matchOrder);
            currentOrderIndex = 0;
        }

        private XmlNode AddFeature(string name)
        {
            XmlElement newNode = currentXml.CreateElement("feature");
            newNode.SetAttribute("name", name);
            XmlElement module = currentXml.CreateElement("module");
            module.SetAttribute("name", name);
            newNode.AppendChild(module);
            XmlElement consts = currentXml.CreateElement("consts");
            newNode.AppendChild(consts);
            XmlElement inv = currentXml.CreateElement("inv");
            newNode.AppendChild(inv);
            currentXml.SelectSingleNode("featureFormaldes").AppendChild(newNode);
            return newNode;
        }
        #endregion
        private void DataFridViewBindingData(string retFeatureName,Dictionary<string, List<Dictionary<string, ProcessNode>>> featureLinkMap,List<Feature> features)
        {
            matchOrder = retFeatureName;
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add(new DataColumn("isEx"));
            dataTable.Columns.Add(new DataColumn("isPre"));
            dataTable.Columns.Add(new DataColumn("id"));
            dataTable.Columns.Add(new DataColumn("Name"));
            dataTable.Columns.Add(new DataColumn("Post"));
            dataTable.Columns.Add(new DataColumn("Number"));
            dataTable.Columns.Add(new DataColumn("isDelete"));
            int id = 0;

            foreach (var item in featureLinkMap)
            {
                Console.WriteLine(item.Key);
            }
            featureRows = new List<FeatureRow>();
            foreach (var item in featureLinkMap)
            {

                List<ProcessRow> result = ProcessNodeHelper.PrintSingleFeatureLinkMapList(item.Value);
                FeatureRow featureRow = new FeatureRow();
                Feature f = features.Find((a) => { return a.XmlNode.Attributes["name"].Value.Equals(item.Key); });
                featureRow.Name = item.Key;
                featureRow.Feature = f;
                DataRow dataRow = dataTable.NewRow();
                dataRow[0] = false;
                dataRow[1] = -1;
                dataRow[2] = id;
                dataRow[3] = item.Key;
                dataRow[4] = "";
                dataRow[5] = "";
                dataRow[6] = false;
                dataTable.Rows.Add(dataRow);
                int index = 1;
                foreach (ProcessRow process in result)
                {
                    DataRow subdataRow = dataTable.NewRow();
                    subdataRow[0] = false;
                    subdataRow[1] = id;
                    subdataRow[2] = -1;
                    subdataRow[3] = "";
                    subdataRow[4] = process.Post;
                    subdataRow[5] = index.ToString();
                    subdataRow[6] = false;
                    subdataRow.SetParentRow(dataRow);
                    dataTable.Rows.Add(subdataRow);
                    index++;
                }
                featureRows.Add(featureRow);
                id++;
            }
            featureDataGridView.DataSource = dataTable;
        }
        private void DeleteProcess(Process processNode)
        {
            if (ProcessNodeHelper.model == Common.Model.OLD)
            {
                XmlNode xmlNode = processNode.XmlNode;
                XmlNode pre = xmlNode.ParentNode;
                pre.RemoveChild(xmlNode);
                if (pre.ChildNodes.Count <= 3)
                {
                    currentXml.DocumentElement.RemoveChild(pre.ParentNode);
                }
            }
            else if (ProcessNodeHelper.model == Common.Model.NEW)
            {

            }
            updateProcess();
        }
        private void EditProcess(Process processNode)
        {
            //XmlNode inputs = process.SelectSingleNode("inputs");
            //XmlNode outputs = process.SelectSingleNode("outputs");
            //XmlNode ext = process.SelectSingleNode("ext");
            //XmlNode pre = process.SelectSingleNode("pre");
            XmlNode xmlNode = processNode.XmlNode;
            xmlNode.Attributes["name"].Value = processNode.ProcessName;
            xmlNode.SelectSingleNode("inputs").InnerText = processNode.Input;
            xmlNode.SelectSingleNode("outputs").InnerText = processNode.Output;
            xmlNode.SelectSingleNode("pre").InnerText = processNode.Pre.ToString();
            if (ProcessNodeHelper.model == Common.Model.OLD)
            {
                xmlNode.ChildNodes[3].FirstChild.Value = processNode.post;
            }
            else if (ProcessNodeHelper.model == Common.Model.NEW)
            {

                XmlNode Scenariose = xmlNode.SelectSingleNode("post");
                foreach (var item in processNode.Scenarioses)
                {
                    if (item.GXmlNode != null && item.DXmlNode != null)
                    {
                        item.GXmlNode.InnerText = item.G;
                        item.DXmlNode.InnerText = item.D;
                    }
                    else
                    {
                        XmlElement DNode = currentXml.CreateElement("D");
                        XmlElement CNode = currentXml.CreateElement("G");
                        XmlElement scenariosNode = currentXml.CreateElement("scenarios");

                        scenariosNode.AppendChild(CNode);
                        scenariosNode.AppendChild(DNode);
                        DNode.InnerText = item.D;
                        CNode.InnerText = item.G;
                        Scenariose.AppendChild(scenariosNode);
                    }
                }
            }
            updateProcess();
        }
        private void AddProcess(Process processNode)
        {
            Feature f = rootFeature.GetFeature(processNode.FeaturnName, true);
            XmlNode xmlNode = f.XmlNode.FirstChild;
            XmlElement newNode = currentXml.CreateElement("process");
            newNode.SetAttribute("name", processNode.ProcessName);

            XmlElement inputsNode = currentXml.CreateElement("inputs");
            inputsNode.InnerText = processNode.Input;
            newNode.AppendChild(inputsNode);
            XmlElement outputsNode = currentXml.CreateElement("outputs");
            outputsNode.InnerText = processNode.Output;

            newNode.AppendChild(outputsNode);
            XmlElement preNode = currentXml.CreateElement("pre");
            preNode.InnerText = processNode.Pre.ToString();
            newNode.AppendChild(preNode);
            XmlElement postNode = currentXml.CreateElement("post");

            if (ProcessNodeHelper.model == Common.Model.OLD)
            {
              
                postNode.InnerText = processNode.post;
                newNode.AppendChild(postNode);

                xmlNode.AppendChild(newNode);
            }
            else if (ProcessNodeHelper.model == Common.Model.NEW)
            {
                newNode.AppendChild(postNode);

                xmlNode.AppendChild(newNode);
            }
            updateProcess();
        }
        private void featureDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                CurrencyManager cm = (CurrencyManager)BindingContext[featureDataGridView.DataSource];

                cm.SuspendBinding();

                DataGridViewRow item = featureDataGridView.Rows[e.RowIndex];
                int id = Convert.ToInt32(item.Cells["id"].Value);
                if (id != -1)
                {
                    bool isEx = !Convert.ToBoolean(item.Cells["isEx"].Value);
                    foreach (DataGridViewRow row in featureDataGridView.Rows)
                    {
                        if (Convert.ToInt32(row.Cells["isPre"].Value) == id)
                        {
                            row.Visible = isEx &&!Convert.ToBoolean(row.Cells["isDelete"].Value);
                            row.Cells["isExColumn"].Value = XMLFormsApp.Properties.Resources.delete;
                        }
                    }
                    item.Cells["isEx"].Value = isEx;
                    if (isEx)
                    {
                        item.Cells["isExColumn"].Value = XMLFormsApp.Properties.Resources.zhankai2;
                    }
                    else
                    {
                        item.Cells["isExColumn"].Value = XMLFormsApp.Properties.Resources.shouqi_1;
                    }
                }
                else if (id == -1 && e.ColumnIndex == 0)
                {
                    if (MessageBox.Show("Is delete this path?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        item.Cells["isDelete"].Value = true;
                        item.Visible = false;
                        string path = item.Cells["Post"].Value.ToString();
                        foreach (DataGridViewRow row in featureDataGridView.Rows)
                        {
                            if (row.Cells["Post"].Value.ToString().Equals(path))
                            {
                                row.Cells["isDelete"].Value = true;
                                row.Visible = false;
                            }
                        }
                    }

                }
                cm.ResumeBinding();
            }
        }

        private void featureDataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            CurrencyManager cm = (CurrencyManager)BindingContext[featureDataGridView.DataSource];

            cm.SuspendBinding(); 

            foreach (DataGridViewRow item in featureDataGridView.Rows)
            {
                if(Convert.ToInt32(item.Cells["id"].Value) == -1)
                {
                    item.Visible = false;
                }
                else
                {
                    item.Visible = false;
                    item.Cells["isExColumn"].Value =  XMLFormsApp.Properties.Resources.shouqi_1;
                }
            }
            cm.ResumeBinding();
            updataScenarioPath();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void delButton_Click(object sender, EventArgs e)
        {
            if (featureDataGridView.SelectedRows.Count != 0)
            {
                DataGridViewRow row = featureDataGridView.SelectedRows[0];
                int id = Convert.ToInt32(row.Cells["id"].Value);
                int preid;
                if (id == -1)
                {
                    preid = Convert.ToInt32(row.Cells["isPre"].Value);
                }
                else
                {
                    preid = id;
                }
                if (preid < featureRows.Count)
                {
                    if (featureRows[preid].Processes.Count > 0)
                    {
                        ShowProcessFrom(featureRows[preid], ProcessType.DEL, DeleteProcess);
                    }
                    else
                    {
                        if(MessageBox.Show("There is no process under the current node whether to add a new process", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ShowProcessFrom(featureRows[preid], ProcessType.ADD, AddProcess);
                        }
                    }
                }
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
           
            if (featureDataGridView.SelectedRows.Count!=0)
            {
                DataGridViewRow row = featureDataGridView.SelectedRows[0];
                int id = Convert.ToInt32(row.Cells["id"].Value);
                int preid;
                if (id == -1)
                {
                    preid = Convert.ToInt32(row.Cells["isPre"].Value);
                }
                else
                {
                    preid = id;
                }
                if (preid < featureRows.Count)
                {
                    if (featureRows[preid].Processes.Count > 0)
                    {
                        ShowProcessFrom(featureRows[preid], ProcessType.EDIT, EditProcess);
                    }
                    else
                    {   
                        if (MessageBox.Show("There is no process under the current node whether to add a new process", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ShowProcessFrom(featureRows[preid], ProcessType.ADD, AddProcess);
                        }
                    }
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (featureDataGridView.SelectedRows.Count != 0)
            {
                DataGridViewRow row = featureDataGridView.SelectedRows[0];
                int id = Convert.ToInt32(row.Cells["id"].Value);
                int preid;
                if (id == -1)
                {
                    preid = Convert.ToInt32(row.Cells["isPre"].Value);
                }
                else
                {
                    preid = id;
                }
                if (preid < featureRows.Count)
                {
                    ShowProcessFrom(featureRows[preid], ProcessType.ADD, AddProcess);
                }
            }
        }

        private void ShowProcessFrom(FeatureRow featureRow,ProcessType type,Action<Process> action)
        {
            if (featureRow.Feature != null)
            {
                if (ProcessNodeHelper.model == Common.Model.OLD)
                {
                    ProcessForm processForm = new ProcessForm(featureRow, type);
                    processForm.operationAction += action;
                    processForm.ShowDialog();
                }
                else if (ProcessNodeHelper.model == Common.Model.NEW)
                {
                    ScenariosForm scenariosForm = new ScenariosForm(featureRow, type);
                    scenariosForm.operationAction += action;
                    scenariosForm.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("You can only operate on a single node");
            }
        }
        private void Save(string foldPath)
        {
            if (!Directory.Exists(string.Join("/", foldPath, formalString)))
            {
                Directory.CreateDirectory(string.Join("/", foldPath, formalString));
                File.WriteAllText(foldPath + "/" + xmlString, xmlTree1.xmlDocument.InnerXml);
                string formalPath = string.Join("/", foldPath, formalString, name + ".formal");
                File.WriteAllText(formalPath, currentXml.InnerXml);
                path = foldPath;
                xmlTree1.SetXML(foldPath + "/" + xmlString, "struct", new List<string>() { "name" });
                fileTree1.SetRootPath(foldPath);
            } 
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(path);
        }
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentXml != null)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";
                //dialog.RootFolder = Environment.SpecialFolder.Programs;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string foldPath = dialog.SelectedPath;
                    Save(foldPath);
                }
            }
            else
            {

            }
        }
        bool LoadOk = false;
        private void featureDataGridView_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void leftCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (featureDataGridView.SelectedRows.Count != 0)
            {
                Clipboard.SetDataObject(featureDataGridView.SelectedRows[0].Cells["Post"].Value);
                MessageBox.Show("Copy Success");
            }
            else
            {
                MessageBox.Show("Copy Fail");
            }
        }

        private void featureDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(featureDataGridView,new Point(e.X, e.Y));
            }
        }


        private void featureTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {

                Rectangle myTabRect = featureTabControl.GetTabRect(e.Index);

                //先添加TabPage属性     
                e.Graphics.DrawString(featureTabControl.TabPages[e.Index].Text
                , this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);

                //再画一个矩形框  
                using (Pen p = new Pen(Color.Transparent))
                {
                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                    myTabRect.Width = CLOSE_SIZE;
                    myTabRect.Height = CLOSE_SIZE;
                    e.Graphics.DrawRectangle(p, myTabRect);
                }

                //填充矩形框  
                Color recColor = e.State == DrawItemState.Selected ? Color.Transparent : Color.Transparent;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTabRect);
                }

                //画关闭符号  
                using (Pen objpen = new Pen(Color.Black))
                {
                    //"\"线  
                    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                    e.Graphics.DrawLine(objpen, p1, p2);

                    //"/"线  
                    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                    e.Graphics.DrawLine(objpen, p3, p4);
                }

                e.Graphics.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void featureTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;

                //计算关闭区域     
                Rectangle myTabRect = this.featureTabControl.GetTabRect(featureTabControl.SelectedIndex);

                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;

                //如果鼠标在区域内就关闭选项卡     
                bool isClose = x > myTabRect.X && x < myTabRect.Right
                 && y > myTabRect.Y && y < myTabRect.Bottom;

                if (isClose == true)
                {
                    featureTabControl.TabPages.Remove(featureTabControl.SelectedTab);
                    updataScenarioPath();
                }
            }
        }

        private void scenarioPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showAll(true);
        }
        private void showAll(bool isVisible) 
        {
            CurrencyManager cm = (CurrencyManager)BindingContext[featureDataGridView.DataSource];
            cm.SuspendBinding();
            foreach (DataGridViewRow item in featureDataGridView.Rows)
            {
                if (Convert.ToInt32(item.Cells["id"].Value) != -1)
                {
                    item.Visible = isVisible;
                }
            }
            addButton.Enabled = true;
            editButton.Enabled = true;
            cm.ResumeBinding();
        }

        private void matchingOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (matchOrder != null)
            {
                MessageBox.Show(matchOrder);
            }
        }

        //private void MergeToolStripButton_Click(object sender, EventArgs e)
        //{
        //    showMergeFeature(matchOrder,xmlTree1.rootName);
        //}

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (featureDataGridView.SelectedRows.Count != 0)
            {
                DataGridViewRow dataGridViewRow = featureDataGridView.SelectedRows[0];
                showMergeFeature(dataGridViewRow.Cells["NameColumn"].Value.ToString(), dataGridViewRow.Cells["NameColumn"].Value.ToString());
            }
        }
        private void showMergeFeature(string order,string name)
        {
            string mes = ProcessNodeHelper.MergeFeature(currentXml, order,name);
            OrderForm orderForm = new OrderForm(Path.Combine(path, formalString, name + ".txt"), mes, order);
            orderForm.ShowDialog();
            fileTree1.UpdateFileCatalog();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            showAll(true);
        }

        private void productDerivationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showMergeFeature(matchOrder, xmlTree1.rootName);
        }

        private void MergeToolStripButton_Click(object sender, EventArgs e)
        {
            showMergeFeature(orders[currentOrderIndex], xmlTree1.rootName);
            currentOrderIndex = (currentOrderIndex + 1) % orders.Count;
        }

        private void matchingOrderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }
    }
}
