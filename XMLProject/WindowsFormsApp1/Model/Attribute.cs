using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLFormsApp.Model
{
    public class Attribute
    {
        public string Name { get { return _name; } set { _name = value; } }
        public string Value { get { return _value; } set { _value = value; } }
        private string _name;
        private string _value;
    }
}
