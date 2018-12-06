using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Questions
{
    public class ClusterQueries : Query
    {

        public ClusterQueries()
        {
        }

        public override string Title => "Choose a ressource";

        public override void CreateChoices()
        {
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Namespaces",
                Title = "Namespaces",
                Kind = "namespace",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Nodes",
                Title = "Nodes",
                Kind = "node",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Cluster info",
                Title = "Cluster info",
                Kind = "clusterinfo",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Current config",
                Title = "Current config",
                Kind = "currentcontext",
            });

        }




        public override Query GetNextQuery()
        {
            switch (this.SelectedChoice.Kind)
            {
                case "namespace":
                    return new NamespacesQuery();
                case "node":
                    return new NodesQuery();
                case "clusterinfo":
                    return new ProcessQuery("Cluster info", $"cluster-info", this);
                case "currentcontext":
                    return new ProcessQuery("Current config", $"config current-context", this);
                default:
                    return null;
            }

        }
    }
}
