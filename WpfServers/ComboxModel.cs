using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfServers
{
    public class ComboxModel
    {
        private string name;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        private string value;
    }
}

