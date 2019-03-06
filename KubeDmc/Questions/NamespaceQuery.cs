using System.Collections.Generic;
using System.Diagnostics;

namespace KubeDmc.Questions
{
    public class NamespaceQuery : Query
    {
        public string Namespace { get; set; }

        public NamespaceQuery(string ns) : base("Choose a ressource")
        {
            this.Namespace = ns;
        }



        public override void CreateChoices()
        {
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Pods",
                Title = "Pods",
                Kind = "pod",
                HotkeyIndex = 4
            });
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Deployments",
                Title = "Deployments",
                Kind = "deployment",
                HotkeyIndex = 4
            });
            //this.Choices.Add(new QueryLine
            //{
            //    ChoiceType = QueryLineType.Choice,
            //    Question = this,
            //    Text = $"Get DaemonSets",
            //    Title = "DaemonSets",
            //    Kind = "daemonset",
            //});
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get StatefulSets",
                Title = "StatefulSets",
                Kind = "statefulset",
                HotkeyIndex = 5
            });
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Services",
                Title = "Services",
                Kind = "service",
                HotkeyIndex = 4
            });

            //this.Choices.Add(new QueryLine
            //{
            //    ChoiceType = QueryLineType.Choice,
            //    Question = this,
            //    Text = $"Get Ingress",
            //    Title = "Ingress",
            //    Kind = "ingress",
            //});
            //this.Choices.Add(new QueryLine
            //{
            //    ChoiceType = QueryLineType.Choice,
            //    Question = this,
            //    Text = $"Get ReplicaSets",
            //    Title = "ReplicaSets",
            //    Kind = "replicaset",
            //});
            //this.Choices.Add(new QueryLine
            //{
            //    ChoiceType = QueryLineType.Choice,
            //    Question = this,
            //    Text = $"Get HPA",
            //    Title = "Horizontal Pod Autoscaler",
            //    Kind = "hpa",
            //});
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
                case "deployment":
                    return new DeploymentsQuery(this.Namespace);
                case "statefulset":
                    return new StatefulSetsQuery(this.Namespace);
                case "pod":
                    return new PodsQuery(this.Namespace);
                case "service":
                    return new ServicesQuery(this.Namespace);
                case "describe":
                    return new ProcessQuery(this.Namespace, $"describe ns {this.Namespace}", this);
                case "edit":
                    return new ProcessQuery(this.Namespace, $"edit ns {this.Namespace}", this);
                case "yaml":
                    return new ProcessQuery(this.Namespace, $"get ns {this.Namespace} -o yaml", this);
                default:
                    return null;
            }

        }
    }
}
