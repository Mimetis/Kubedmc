using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Kub
{
    public class Context : Item
    {
        [Item("CURRENT", 0)]
        public string Current { get; set; }
        [Item("CLUSTER", 2)]
        public string Cluster { get; set; }
        [Item("USER", 3)]
        public string User { get; set; }
        [Item("NAMESPACE", 4)]
        public string Namespace { get; set; }

    }
}
