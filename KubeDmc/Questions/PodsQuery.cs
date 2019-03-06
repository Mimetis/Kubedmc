using System;
using System.Collections.Generic;
using System.Linq;
using KubeDmc.Kub;

namespace KubeDmc.Questions
{
    public class PodsQuery : Query
    {

        public string Namespace { get; set; }

        public PodsQuery(string ns) : base("Pods")
        {
            this.Namespace = ns;
        }

        public override bool IsRefreshedEnabled => true;
        public override void RefreshItems()
        {
            this.Items = KubService.Current.GetPods(this.Namespace);
        }
        public override Query GetNextQuery()
        {
            return new PodQuery(this.Namespace, (Pod)this.SelectedChoice.Item);
        }

    }
}
