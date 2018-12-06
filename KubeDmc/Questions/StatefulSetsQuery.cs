using System;
using System.Collections.Generic;
using System.Linq;
using KubeDmc.Kub;

namespace KubeDmc.Questions
{
    public class StatefulSetsQuery : Query
    {

        public string Namespace { get; set; }
        public override string Title => "StatefulSets";

        public StatefulSetsQuery(string ns)
        {
            this.Namespace = ns;
            this.Items = KubService.Current.GetStatefulSets(this.Namespace);
        }


        public override Query GetNextQuery()
        {
            return new StatefulSetQuery(this.Namespace, (StatefulSet)this.SelectedChoice.Item);
        }

    }
}
