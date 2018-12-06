using KubeDmc.Kub;
using System.Collections.Generic;

namespace KubeDmc.Questions
{
    public class NamespacesQuery : Query
    {

        public override string Title => "Namespaces";

        public NamespacesQuery()
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
