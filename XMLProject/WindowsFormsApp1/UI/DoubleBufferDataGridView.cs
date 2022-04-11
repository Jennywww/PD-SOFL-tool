using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XMLFormsApp.UI
{
    class DoubleBufferDataGridView : DataGridView
    {
        public DoubleBufferDataGridView()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            //UpdateStatus.Continue;
            UpdateStyles();
        }
    }
}
