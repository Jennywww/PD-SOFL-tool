using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XMLFormsApp
{
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        Dictionary<string, int> values = new Dictionary<string, int>();


    //        values.Add("apple3", 3);
    //        values.Add("apple2", 2);
    //        values.Add("apple4", 4);
    //        values.Add("apple5", 5);
    //        values.Add("apple1", 1);
    //        values.Add("apple6", 6);
    //        values.Remove("apple1");
    //        var list = new List<int>(values.Values);

    //        for (int i = 0; i < list.Count; i++)
    //        {
    //            Console.WriteLine(list[i]);
    //        }

    //    }
    //}
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
