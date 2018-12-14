using KubeDmc.Kub;
using KubeDmc.Questions;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc
{
    public class QueryLine
    {
        

        /// <summary>
        /// Position of the Hotkey for the current line
        /// </summary>
        public int HotkeyIndex { get; set; } = -1;

        /// <summary>
        /// Gets or Sets the query title    
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Get the query text 
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Get the Menu choice
        /// </summary>
        public QueryLineType ChoiceType { get; set; }

        /// <summary>
        /// Get the Parent Menu item
        /// </summary>
        public Query Question { get; set; }

        /// <summary>
        /// Gets or Sets the position of the item in the console
        /// </summary>
        public int TopPosition { get; set; }

        /// <summary>
        /// Kind of choice
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// Tag ref
        /// </summary>
        public Object Item { get; set; }


        public ConsoleKey GetConsoleKey()
        {
            if (this.HotkeyIndex >= 0)
               return Enum.Parse<ConsoleKey>(this.Text.ToUpperInvariant().Substring(this.HotkeyIndex, 1));
            else
                return ConsoleKey.NoName;
        }
    }
}
