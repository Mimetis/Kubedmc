using System;
using System.Collections.Generic;
using System.Linq;
using KubeDmc.Kub;

namespace KubeDmc.Questions
{
    public class StatefulSetsQuery : Query
    {

        public string Namespace { get; set; }

        public StatefulSetsQuery(string ns)  : base("StatefulSets")
        {
            this.Namespace = ns;
        }
        public override void RefreshItems()
        {
            this.Items = KubService.Current.GetStatefulSets(this.Namespace);
        }

        public override Query GetNextQuery()
        {
            return new StatefulSetQuery(this.Namespace, (StatefulSet)this.SelectedChoice.Item);
        }

    }
}
