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
    public partial class OrderForm : Form
    {
        string path;
        public OrderForm(string path,string context,string title)
        {
            InitializeComponent();
            textBox1.Text = context;
            this.path = path;
            Text = title;
        }

        private void OrderForm_Load(object sender, EventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            string initialPath = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            saveFileDialog.InitialDirectory = initialPath;
            saveFileDialog.FileName = fileName;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.Filter = "文本文件|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, textBox1.Text);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
