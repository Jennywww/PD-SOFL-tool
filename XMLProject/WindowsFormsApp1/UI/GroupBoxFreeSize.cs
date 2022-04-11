using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XMLFormsApp.UI
{
    class GroupBoxFreeSize:GroupBox
    {
        public GroupBoxFreeSize()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            //UpdateStatus.Continue;
            UpdateStyles();
            this.MouseMove += Form1_MouseMove;
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
        }
        //代码比较简单，就不多解析了。       
        #region 移动窗体保存数据

        Point mouseOff;//鼠标移动位置变量
        bool leftFlag; //标志是否为左键
        bool largeFlag; //标志是否同时改变宽度及高度
        bool widthFlag; //标志是否改变宽度
        bool heightFlag;//标志是否改变高度

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseSet = Control.MousePosition;
            if (leftFlag)
            {
                //改变宽高
                if (largeFlag)
                {
                    int dw = mouseSet.X - this.Location.X;
                    int dh = mouseSet.Y - this.Location.Y;
                    this.Width = dw;
                    this.Height = dh;
                }
                //改变宽度
                else if (widthFlag)
                {
                    int dw = mouseSet.X - this.Location.X;
                    this.Width = dw;
                }
                //改变高度
                else if (heightFlag)
                {
                    int dh = mouseSet.Y - this.Location.Y;
                    this.Height = dh;
                }
                //移动位置
                else
                {
                    mouseSet.Offset(mouseOff.X, mouseOff.Y); //设置移动后的位置
                    this.Location = mouseSet;
                }
            }
            else
            {
                //设置改变窗体宽高的标志
                if ((this.Location.X + this.Width - mouseSet.X) < 10 && (this.Location.Y + this.Height - mouseSet.Y) < 10)
                {
                    this.Cursor = Cursors.SizeNWSE;
                    largeFlag = true;
                }
                //设置改变窗体宽度的标志
                else if ((this.Location.X + this.Width - mouseSet.X) < 10)
                {
                    this.Cursor = Cursors.SizeWE;
                    widthFlag = true;
                }
                //设置改变窗体高度的标志
                else if ((this.Location.Y + this.Height - mouseSet.Y) < 10)
                {
                    this.Cursor = Cursors.SizeNS;
                    heightFlag = true;
                }
                //设置移动位置的标志
                else
                {
                    this.Cursor = Cursors.Default;
                    largeFlag = false;
                    widthFlag = false;
                    heightFlag = false;
                }
            }
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }
        /// <summary>
        /// 鼠标弹起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {

                leftFlag = false;//释放鼠标后标注为false;
            }
        }
    #endregion
    }
}
