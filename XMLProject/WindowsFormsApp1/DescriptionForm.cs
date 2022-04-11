using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XMLFormsApp
{
    public partial class DescriptionForm : Form
    {
        public Action<string> okAction;
        public DescriptionForm(string descirption)
        {
            InitializeComponent();
            descriptionTextBox.Text = descirption;
        }

        private void DescriptionForm_Load(object sender, EventArgs e)
        {

        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (okAction != null)
            {
                okAction(descriptionTextBox.Text);
                Close();
            }
        }
    }
}
