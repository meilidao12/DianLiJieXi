using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfServers.Attributes
{
    public class ControllerAttribute : Attribute
    {
        int age;
        public int Age
        {
            get
            {
                return this.age;
            }
            set
            {
                this.age = value;
            }
        }

        string url;
        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }

        }
    }
}
