using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KubeDmc.Questions
{
    public class NodesQuery : Query
    {

        public NodesQuery() : base("Nodes")
        {
        }

        public override void RefreshItems()
        {
            this.Items = KubService.Current.GetNodes();
        }

        public override Query GetNextQuery()
        {
            return new NodeQuery((Node)this.SelectedChoice.Item);
        }

    }
}
