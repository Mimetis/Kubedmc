using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Kub
{
    public class Node : Item
    {
        [Item("STATUS")]
        public string Status { get; set; }
        [Item("ROLES")]
        public string Roles { get; set; }
        [Item("VERSION")]
        public string Version { get; set; }

    }
}
