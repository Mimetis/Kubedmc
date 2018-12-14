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
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get NameSpaces",
                Title = "Namespaces",
                Kind = "namespace",
                HotkeyIndex = 8
            });
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Get Nodes",
                Title = "Nodes",
                Kind = "node",
                HotkeyIndex = 4
            });
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Cluster Info",
                Title = "Cluster info",
                Kind = "clusterinfo",
                HotkeyIndex = 8
            });
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"Current Config",
                Title = "Current config",
                Kind = "currentcontext",
                HotkeyIndex = 8
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
