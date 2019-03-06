using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Questions
{
    public class StatefulSetQuery : Query
    {
        private readonly string ns;

        public StatefulSet StatefulSet{ get; }

        public StatefulSetQuery(string ns, StatefulSet sts) : base("Choose an action")
        {
            this.ns = ns;
            this.StatefulSet = sts;
        }


        public override void CreateChoices()
        {
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Edit]",
                Title = "Edit",
                Kind = "edit",
                HotkeyIndex = 1
            });
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Describe]",
                Title = "Decribe",
                Kind = "describe",
                HotkeyIndex = 1
            }
            );
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Yaml output]",
                Title = "Yaml output",
                Kind = "yaml",
                HotkeyIndex = 1
            });
        }




        public override Query GetNextQuery()
        {
            switch (this.SelectedChoice.Kind)
            {
                case "describe":
                    return new ProcessQuery(this.StatefulSet.Name, $"describe sts {this.StatefulSet.Name} -n {this.ns}", this);
                case "yaml":
                    return new ProcessQuery(this.StatefulSet.Name, $"get sts {this.StatefulSet.Name} -n {this.ns} -o yaml", this);
                case "edit":
                    return new ProcessQuery(this.StatefulSet.Name, $"edit sts {this.StatefulSet.Name} -n {this.ns}", this);
                default:
                    return null;
            }

        }
    }
}
