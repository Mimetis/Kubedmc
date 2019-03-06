using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KubeDmc.Questions
{
    public class DeploymentsQuery : Query
    {
        public string Namespace { get; set; }
        public List<Deployment> Deployments { get; }

        public DeploymentsQuery(string ns) : base("Deployments")
        {
            this.Namespace = ns;
        }

        public override void RefreshItems()
        {
             this.Items = KubService.Current.GetDeployments(this.Namespace);
       }

        public override Query GetNextQuery()
        {
            return new DeploymentQuery(this.Namespace, (Deployment)this.SelectedChoice.Item);
        }

    }
}
