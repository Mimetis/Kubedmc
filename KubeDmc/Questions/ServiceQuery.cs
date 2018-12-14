using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Questions
{
    public class ServiceQuery : Query
    {
        private readonly string ns;

        public Service Service{ get; }
        public override string Title => "Choose an action";

        public ServiceQuery(string ns, Service s)
        {
            this.ns = ns;
            this.Service = s;
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
                    return new ProcessQuery(this.Service.Name, $"describe svc {this.Service.Name} -n {this.ns}", this);
                case "yaml":
                    return new ProcessQuery(this.Service.Name, $"get svc {this.Service.Name} -n {this.ns} -o yaml", this);
                case "edit":
                    return new ProcessQuery(this.Service.Name, $"edit svc {this.Service.Name} -n {this.ns}", this);
                default:
                    return null;
            }

        }
    }
}
