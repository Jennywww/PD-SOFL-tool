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

namespace XMLFormsApp
{
    public partial class FileTree : UserControl
    {
        string rootPath = "";
        public FileTree()
        {
            InitializeComponent();
        }
        public void SetRootPath(string path)
        {
            treeView1.Nodes.Clear();
            rootPath = path;
            UpdateFileCatalog();
        }

        private void FileTree_Load(object sender, EventArgs e)
        {

        }
        public void UpdateFileCatalog()
        {
            treeView1.Nodes.Clear();
            if (!string.IsNullOrEmpty(rootPath))
            {
                loadFileCatalog(rootPath, treeView1.Nodes);
            }
            Update();
        }
        private void loadFileCatalog(string path, TreeNodeCollection nodes)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            DirectoryInfo[] dics = root.GetDirectories();
            foreach (var item in dics)
            {
                TreeNode treenode = CreateTreeNode(item.Name);
                nodes.Add(treenode);
                loadFileCatalog(item.FullName, treenode.Nodes);
            }
            FileInfo[] files = root.GetFiles();
            foreach (var item in files)
            {
                TreeNode treenode = CreateTreeNode(item.Name);
                nodes.Add(treenode);
            }
        }
        private TreeNode CreateTreeNode(string name, int image = 1)
        {
            TreeNode treeNode = new TreeNode();
            treeNode.Text = name;
            return treeNode;
        }
    }
}
