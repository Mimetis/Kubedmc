using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KubeDmc.Questions
{
    public class ServicesQuery : Query
    {
        public string Namespace { get; set; }

        public ServicesQuery(string ns)
        {
            this.Namespace = ns;
            this.Items = KubService.Current.GetServices(this.Namespace);

        }
        public override string Title => "Choose a ressource";

        public override Query GetNextQuery()
        {
            return new ServiceQuery(this.Namespace, (Service)this.SelectedChoice.Item);
        }

    }
}
