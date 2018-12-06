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
    }
}
