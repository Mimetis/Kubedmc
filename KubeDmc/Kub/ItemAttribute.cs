using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Kub
{
    public class ItemAttribute : Attribute
    {
        public ItemAttribute(string name, int order= 99)
        {
            this.Name = name;
            this.Order = order;
        }

        public string Name { get; set; }
        public int Order { get; internal set; }
    }
}
