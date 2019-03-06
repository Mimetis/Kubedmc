using KubeDmc.Kub;
using System.Collections.Generic;

namespace KubeDmc.Questions
{
    public class NamespacesQuery : Query
    {
        public NamespacesQuery() : base("Namespaces")
        {
        }


        public override void RefreshItems()
        {
            this.Items = KubService.Current.GetNamespaces();
        }

        public override Query GetNextQuery()
        {
            if (this.SelectedChoice == null || this.SelectedChoice.Item == null)
                return null;

            return new NamespaceQuery(((Namespace)this.SelectedChoice.Item).Name);
        }

    }
}
