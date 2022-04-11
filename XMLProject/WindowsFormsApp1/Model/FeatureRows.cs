using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLFormsApp.Model
{
    public class FeatureRow
    {
        public Feature Feature { 
            get 
            { 
                return feature; 
            } 
            set 
            { 
                feature = value;
                if (feature!=null&& feature.Processes != null)
                {
                    foreach (var item in feature.Processes)
                    {
                        AddProcess(item.Value);
                    }
                }
            } 
        }
        public List<ProcessRow> ProcessRows { get { if (processRows == null) processRows = new List<ProcessRow>(); return processRows; } }
        public List<Process> Processes { get { if (processes == null) processes = new List<Process>(); return processes; } }

        public string Name { get { return name; } set { name = value; } }

        public void AddProcess(Process process)
        {
            if (processes == null)
            {
                processes = new List<Process>();
            }
            Process p = processes.Find(a => { return process.ProcessName == a.ProcessName; });
            if (p == null&&process.FeaturnName.Equals(name))
            {
                processes.Add(process);
            }
        }
        public void AddProcessRow(ProcessRow processRow)
        {
            if (processRows == null)
            {
                processRows = new List<ProcessRow>();
            }
            if (processes == null)
            {
                processes = new List<Process>();
            }
            processRows.Add(processRow);
            foreach (var item in processRow.ProcessNodes)
            {
                AddProcess(item.Process);
            }
            processRow.FeatureRow = this;
        }
        private List<ProcessRow> processRows;
        private Feature feature;
        private string name;
        private List<Process> processes;

    }
}
