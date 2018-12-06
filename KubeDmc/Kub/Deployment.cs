using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Kub
{
    public class Deployment : Item
    {

        [Item("CURRENT")]
        public int Current { get; set; }
        [Item("DESIRED")]
        public int Desired { get; internal set; }
        [Item("AVAILABLE")]
        public int Available { get; internal set; }
        [Item("UP-TO-DATE")]
        public int UpToDate { get; internal set; }
    }
}
