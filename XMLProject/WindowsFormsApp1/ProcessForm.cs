using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XMLFormsApp.Common;
using XMLFormsApp.Model;


namespace XMLFormsApp
{
    public enum ProcessType{
        DEL,
        ADD,
        EDIT
    }
    public partial class ProcessForm : Form
    {
        public Action<Process> operationAction = null;
        FeatureRow featureRow = null;
        Process selectProcess;
        int selectindex = 0;
        ProcessType type;
        string confirmText;
        public ProcessForm(FeatureRow featureRow,ProcessType processType)
        {
            InitializeComponent();
            this.featureRow = featureRow;
            type = processType;
        }

        private void InitialDELFrom()
        {
            inputTextBox.Enabled = false;
            outputTextBox.Enabled = false;
            preRadioButton.Enabled = false;
            postTextBox.Enabled = false;
            nameTextBox.Enabled = false;
            Text = "Delete Process";
            operationButton.Text = "delete";
            confirmText = "Are you sure Delete this?";
        }

        private void InitialEDITFrom()
        {
            Text = "Edit Process";
            operationButton.Text = "edit";
            confirmText = "Are you sure Edit this?";
        }
        private void InitialADDFrom()
        {
            selectProcess = new Process();
            selectProcess.FeaturnName = featureRow.Name;
            preButton.Enabled = false;
            NextButton.Enabled = false;
            inputTextBox.Text = "";
            outputTextBox.Text = "";
            postTextBox.Text = "";
            preRadioButton.Checked = true;
            nameTextBox.Text = "";
            Text = "Add Process";
            operationButton.Text = "Add";
            confirmText = "Are you sure Add this?";
        }
        private void ProcessForm_Load(object sender, EventArgs e)
        {

            switch (type)
            {
                case ProcessType.DEL:
                    Updata();
                    InitialDELFrom();
                    break;
                case ProcessType.ADD:
                    InitialADDFrom();
                    break;
                case ProcessType.EDIT:
                    Updata();
                    InitialEDITFrom();
                    break;
                default:
                    break;
            }
        }

        private void Updata()
        {
            selectProcess = featureRow.Processes[selectindex];
            inputTextBox.Text = selectProcess.Input;
            outputTextBox.Text = selectProcess.Output;
            postTextBox.Text = selectProcess.post.Trim();
            preRadioButton.Checked = selectProcess.Pre;  
            nameTextBox.Text = selectProcess.ProcessName;
        }

        private void preButton_Click(object sender, EventArgs e)
        {
            selectindex -= 1;
            if (selectindex <=0)
            {
                selectindex = 0;
            }
            Updata();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            selectindex += 1;
            if (selectindex >= featureRow.Processes.Count)
            {
                selectindex = featureRow.Processes.Count - 1;
            }
            Updata();
        }

        private void operationButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(confirmText,"waring", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                selectProcess.post = postTextBox.Text;
                selectProcess.Input = inputTextBox.Text;
                selectProcess.Output = outputTextBox.Text;
                selectProcess.Pre = preRadioButton.Checked;
                selectProcess.ProcessName = nameTextBox.Text;
                operationAction(selectProcess);
                Close();
            }
        }
    }
}
