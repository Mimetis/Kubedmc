using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Kub
{
    public class StatefulSet : Item
    {
        [Item("CURRENT")]
        public int Current { get; set; }
        [Item("DESIRED")]
        public int Desired { get; internal set; }

    }
}
