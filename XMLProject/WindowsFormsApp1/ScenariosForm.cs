using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XMLFormsApp.Model;

namespace XMLFormsApp
{
    public partial class ScenariosForm : Form
    {
        public Action<Process> operationAction = null;
        FeatureRow featureRow = null;
        Process selectProcess;
        int selectProcessIndex = 0;
        int selectScenariosIndex = 0;
        ProcessType type;
        string confirmText;

        Dictionary<int, string[]> recordScenarios =new Dictionary<int, string[]>();

        bool haveScenariosChange = false;
        bool haveProcessChange = false;
        public ScenariosForm(FeatureRow featureRow,ProcessType processType)
        {
            InitializeComponent();
            this.featureRow = featureRow;
            type = processType;
        }

        private void ScenariosForm_Load(object sender, EventArgs e)
        {
            switch (type)
            {
                case ProcessType.DEL:
                    Updata(true);
                    InitialDELFrom();
                    break;
                case ProcessType.ADD:
                    InitialADDFrom();
                    break;
                case ProcessType.EDIT:
                    Updata(true);
                    InitialEDITFrom();
                    break;
                default:
                    break;
            }
        }
        private void UpdataScenarios()
        {
            haveScenariosChange = haveScenariosChange || featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].G == gTextBox.Text || featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].D == dTextBox.Text;
            if (featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].G == gTextBox.Text || featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].D == dTextBox.Text)
            {
                string[] record = { featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].G, featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].D };
                if (!recordScenarios.ContainsKey(selectScenariosIndex))
                {
                    recordScenarios.Add(selectScenariosIndex, record);
                }
            }
            featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].G = gTextBox.Text;
            featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].D = dTextBox.Text;
        }
        private void Updata(bool isProcess)
        {
            if (isProcess)
            {
                selectProcess = featureRow.Processes[selectProcessIndex];
                inputTextBox.Text = selectProcess.Input;
                outputTextBox.Text = selectProcess.Output;
                preRadioButton.Checked = selectProcess.Pre;
                nameTextBox.Text = selectProcess.ProcessName;
            }
            if (featureRow.Processes[selectProcessIndex].Scenarioses.Count == 0)
            {
                featureRow.Processes[selectProcessIndex].Scenarioses.Add(new Scenarios() { });
            }
            gTextBox.Text = featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].G;
            dTextBox.Text = featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].D;
 
        }
        private void InitialDELFrom()
        {
            inputTextBox.Enabled = false;
            outputTextBox.Enabled = false;
            preRadioButton.Enabled = false;
            nameTextBox.Enabled = false;
            gTextBox.Enabled = false;
            dTextBox.Enabled = false;
            Text = "Delete Process";
            operationButton.Text = "delete";
            confirmText = "Are you sure Delete this?";
        }

        private void InitialEDITFrom()
        {
            Text = "Edit Process";
            operationButton.Text = "Save";
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
            gTextBox.Enabled = false;
            dTextBox.Enabled = false;
            scenariosNextButton.Enabled = false;
            scenariosPreButton.Enabled = false;
            preRadioButton.Checked = true;
            addButton.Enabled = false;
            nameTextBox.Text = "";
            Text = "Add Process";
            operationButton.Text = "Save";
            confirmText = "Are you sure Add this?";
        }
        private void recoverEdit()
        {
            foreach (var item in recordScenarios)
            {
                selectProcess.Scenarioses[item.Key].G = item.Value[0];
                selectProcess.Scenarioses[item.Key].D = item.Value[1];
            }
            recordScenarios.Clear();
        }

        private void preButton_Click(object sender, EventArgs e)
        {
            ChangeProcess("Do you want to save the Scenarioses?");
            selectProcessIndex -= 1;
            if (selectProcessIndex <= 0)
            {
                selectProcessIndex = 0;
            }
            else
            {
                selectScenariosIndex = 0;
            }
            Updata(true);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            ChangeProcess("Do you want to save the Scenarioses?");
            selectProcessIndex += 1;
            if (selectProcessIndex >= featureRow.Processes.Count)
            {
                selectProcessIndex = featureRow.Processes.Count - 1;
            }
            else
            {
                selectScenariosIndex = 0;
            }
            Updata(true);
        }

        private void scenariosPreButton_Click(object sender, EventArgs e)
        {
            UpdataScenarios();
            selectScenariosIndex -= 1;
            if (selectScenariosIndex <= 0)
            {
                selectScenariosIndex = 0;
            }
            Updata(false);
        }

        private void scenariosNextButton_Click(object sender, EventArgs e)
        {
            UpdataScenarios();
            selectScenariosIndex += 1;
            if (selectScenariosIndex >= featureRow.Processes[selectProcessIndex].Scenarioses.Count)
            {
                selectScenariosIndex = featureRow.Processes[selectProcessIndex].Scenarioses.Count - 1;
            }
            Updata(false);
        }

        private void ChangeProcess(string warm,bool isForce=false)
        {
            if (haveScenariosChange|| isForce)
            {
                haveScenariosChange = false;
                if (MessageBox.Show(warm, "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    selectProcess.Input = inputTextBox.Text;
                    selectProcess.Output = outputTextBox.Text;
                    selectProcess.Pre = preRadioButton.Checked;
                    selectProcess.ProcessName = nameTextBox.Text;
                    if (featureRow.Processes[selectProcessIndex].Scenarioses.Count == 0)
                    {
                        featureRow.Processes[selectProcessIndex].Scenarioses.Add(new Scenarios() { });
                    }
                    featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].G = gTextBox.Text;
                    featureRow.Processes[selectProcessIndex].Scenarioses[selectScenariosIndex].D = dTextBox.Text;
                    operationAction(selectProcess);
                    recordScenarios.Clear();
                }
                else
                {
                    recoverEdit();
                }
            }
        }

        private void operationButton_Click(object sender, EventArgs e)
        {
            ChangeProcess(confirmText, true);
            Close();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            UpdataScenarios();
            haveScenariosChange = true;
            if (!string.IsNullOrEmpty(selectProcess.Scenarioses[selectProcess.Scenarioses.Count - 1].G + selectProcess.Scenarioses[selectProcess.Scenarioses.Count - 1].D))
            {
                selectProcess.Scenarioses.Add(new Scenarios() { });
            }
            selectScenariosIndex = selectProcess.Scenarioses.Count-1;
            Updata(false);
        }
    }
}
