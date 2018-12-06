using KubeDmc.Kub;

namespace KubeDmc.Questions
{
    public class DeploymentQuery : Query
    {
        public override string Title => "Choose a ressource";

        public string Namespace { get; set; }
        public Deployment Deployment { get; }

        public DeploymentQuery(string ns, Deployment deployment)
        {
            this.Namespace = ns;
            this.Deployment = deployment;
        }

        public override void CreateBackOption()
        {
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Back,
                Question = this,
                Text = "Back",
            });
        }



        public override void CreateChoices()
        {
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Edit]",
                Title = "Edit",
                Kind = "edit",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Describe]",
                Kind = "describe",
            });
            this.Choices.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Yaml output]",
                Title = "Yaml output",
                Kind = "yaml",
            });

        }


        public override Query GetNextQuery()
        {
            switch (this.SelectedChoice.Kind)
            {
                case "describe":
                    return new ProcessQuery(this.Deployment.Name, $"describe deployment {this.Deployment.Name} -n {this.Namespace}", this);
                case "edit":
                    return new ProcessQuery(this.Deployment.Name, $"edit deployment {this.Deployment.Name} -n {this.Namespace}", this);
                case "yaml":
                    return new ProcessQuery(this.Deployment.Name, $"get deployment  {this.Deployment.Name} -n {this.Namespace} -o yaml", this);
                default:
                    return null;
            }
        }


    }
}
