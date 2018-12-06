using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Kub
{
    public class Pod : Item
    {
        [Item("STATUS")]
        public string Status { get; set; }

        public int ContainersReadyCount { get; set; }
        public int ContainersCount { get; set; }
        public int ContainerRestarts { get; set; }

        [Item("READY")]
        public string Ready
        {
            get
            {
                return $"{this.ContainersReadyCount}/{this.ContainersCount}";
            }
        }

    }
}
