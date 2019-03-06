using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Questions
{
    public class ContextQuery : Query
    {
        public Kub.Context CurrentContext { get; set; }

        public ContextQuery(Kub.Context currentContext) : base($"Current context: {currentContext.Cluster}")
        {
            this.CurrentContext = currentContext;
        }

    
        public override void CreateChoices()
        {
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Root,
                Question = this,
                Text = "Root",
                HotkeyIndex = 0
            });
        }



        public override Query GetNextQuery()
        {
            return null;
        }

    }
}
