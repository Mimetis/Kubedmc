using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace KubeDmc.Questions
{
    public class ContextQueries : Query
    {
        private Context selectedContext;

        public ContextQueries() : base("Contexts")
        {
        }

        public override void RefreshItems()
        {
            var contexts = KubService.Current.GetAllContexts();
            this.selectedContext = contexts.FirstOrDefault(c => c.Current == "*");
            this.Items = contexts;

        }


        public override void SetInProgressChoice()
        {
            if (this.QueryLines == null || this.QueryLines.Count == 0)
                return;

            this.InProgressChoice = this.QueryLines.FirstOrDefault(ql => ((Context)ql.Item) == this.selectedContext);

            if (this.InProgressChoice == null)
                this.InProgressChoice = this.QueryLines[0];
        }

        public override Query GetNextQuery()
        {
            if (this.SelectedChoice == null || this.SelectedChoice.Item == null)
                return null;

            var context = this.SelectedChoice.Item as Kub.Context;

            if (context.Current == "")
            {
                var process = new Process();
                process.StartInfo.FileName = "kubectl";
                process.StartInfo.Arguments = $"config use-context {context.Cluster}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();

                //var contexts = KubService.Current.GetAllContexts();
                //this.Items = contexts;
                //this.SelectedContext = contexts.FirstOrDefault(c => c.Current == "*");

            }
            return null;

        }
    }
}
