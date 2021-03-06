﻿using KubeDmc.Kub;

namespace KubeDmc.Questions
{
    public class PodQuery : Query
    {
        private readonly string ns;

        public Pod pod { get; }

        public PodQuery(string ns, Pod pod) : base("Choose an action")
        {
            this.ns = ns;
            this.pod = pod;
        }


        public override void CreateChoices()
        {
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Logs]",
                Title = "Logs",
                Kind = "log",
                HotkeyIndex = 1
            }
            );
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Bash]",
                Title = "Bash",
                Kind = "bash",
                HotkeyIndex = 1
            }
          );
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Edit]",
                Title = "Edit",
                Kind = "edit",
                HotkeyIndex = 1
            }
        );
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
                case "yaml":
                    return new ProcessQuery(this.pod.Name, $"get pod {this.pod.Name} -n {this.ns} -o yaml", this);
                case "log":
                    return new ProcessQuery(this.pod.Name, $"logs {this.pod.Name} -n {this.ns}", this);
                case "bash":
                    return new ProcessQuery(this.pod.Name, $"exec -it {this.pod.Name} -n {this.ns} sh", this, false);
                case "edit":
                    return new ProcessQuery(this.pod.Name, $"edit pods {this.pod.Name} -n {this.ns}", this);
                case "describe":
                default:
                    return new ProcessQuery(this.pod.Name, $"describe pod {this.pod.Name} -n {this.ns}", this);
            }

        }
    }
}
