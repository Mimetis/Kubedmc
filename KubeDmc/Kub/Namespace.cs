using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Kub
{
    public class Namespace : Item
    {
        [Item("STATUS")]
        public string Status { get; set; }
    }
}
