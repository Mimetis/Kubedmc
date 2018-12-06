using System;

namespace KubeDmc.Kub
{
    public class Item
    {

        [Item("NAME", 1)]
        public string Name { get; set; }

        [Item("AGE", 100)]
        public string Age
        {
            get
            {
                if (this.StartDateTime.HasValue)
                {
                    var et = DateTime.Now.ToUniversalTime().Subtract(this.StartDateTime.Value);

                    if (et.Days >= 1)
                        return $"{et.Days}d";

                    if (et.Hours >= 1)
                        return $"{et.Hours}h";

                    if (et.TotalMinutes >= 1)
                        return $"{et.Minutes}m";

                    if (et.TotalSeconds >= 1)
                        return $"{et.Seconds}s";
                }

                return "";
            }
        }


        public DateTime? StartDateTime { get; set; }

    }
}
