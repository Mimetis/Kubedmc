using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KubeDmc.Questions
{
    public class NodesQuery : Query
    {
        public override string Title => "Nodes";

        public NodesQuery()
        {
            this.Items = KubService.Current.GetNodes();
        }


        public override Query GetNextQuery()
        {
            return new NodeQuery((Node)this.SelectedChoice.Item);
        }

    }
}
