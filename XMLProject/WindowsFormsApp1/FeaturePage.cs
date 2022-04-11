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
    public partial class FeaturePage : UserControl
    {
        public FeaturePage(string Feature)
        {
            InitializeComponent();
            featureTextBox.Text = Feature;
        }

        public void SetString(string str)
        {
            featureTextBox.Text = str;
            Update();
        }
        private void FeaturePage_Load(object sender, EventArgs e)
        {

        }
    }
}
