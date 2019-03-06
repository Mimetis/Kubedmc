using System;
using System.Collections.Generic;
using System.Text;

namespace KubeDmc.Questions
{
    public class ClusterQueries : Query
    {

        public ClusterQueries() : base("Choose a ressource")
        {
        }

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
                Text = $"Get Cluster config",
                Title = "Cluster config",
                Kind = "clusterconfig",
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
                case "clusterconfig":
                    return new ContextQueries();
                default:
                    return null;
            }

        }
    }
}
