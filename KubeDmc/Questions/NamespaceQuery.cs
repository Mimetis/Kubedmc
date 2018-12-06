using System.Collections.Generic;
using System.Diagnostics;

namespace KubeDmc.Questions
{
    public class NamespaceQuery : Query
    {
        public string Namespace { get; set; }

        public override string Title => "Choose a ressource";


        public NamespaceQuery(string ns)
        {
            this.Namespace = ns;
        }

        public override void CreateBackOption()
        {
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Back,
                Question = this,
                Text = "Back",
            });
        }

        public override void CreateChoices()
        {
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Pods",
                Title = "Pods",
                Kind = "pod",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Deployments",
                Title = "Deployments",
                Kind = "deployment",
            });
            //this.Choices.Add(new QueryLine
            //{
            //    ChoiceType = QueryLineType.Choice,
            //    Question = this,
            //    Text = $"Get DaemonSets",
            //    Title = "DaemonSets",
            //    Kind = "daemonset",
            //});
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get StatefulSets",
                Title = "StatefulSets",
                Kind = "statefulset",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Services",
                Title = "Services",
                Kind = "service",
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
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Edit]",
                Title = "Edit",
                Kind = "edit",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Describe]",
                Title = "Describe",
                Kind = "describe",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Yaml output]",
                Title = "Yaml output",
                Kind = "yaml",
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
