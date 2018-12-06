using System;
using System.Diagnostics;
using System.Text;

namespace KubeDmc.Questions
{
    public class ProcessQuery : Query
    {
        public ProcessQuery(string ressource, string arguments, Query backQuery, bool redirectStandardOutput = true) 
        {
            this.ressource = ressource;
            this.Arguments = arguments;
            this.BackQuery = backQuery;
            this.redirectStandardOutput = redirectStandardOutput;
        }

        public override string Title => $"Describe {this.ressource}";

        private string ressource;
        private readonly bool redirectStandardOutput;

        public string Arguments { get; }
        public Query BackQuery { get; }


        public override void CreateChoices()
        {
            Console.WriteLine();
            var process = new Process();
            process.StartInfo.FileName = "kubectl";
            process.StartInfo.Arguments = this.Arguments;
            
            if (redirectStandardOutput)
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;

            }
            process.Start();

            if (redirectStandardOutput)
            {
                // Synchronously read the standard output of the spawned process. 
                using (var reader = process.StandardOutput)
                {

                    string text = "";
                    try
                    {
                        text = reader.ReadToEnd();

                    }
                    catch (Exception ex)
                    {
                        text = ex.Message;
                    }

                    this.Choices.Add(new QueryLine
                    {
                        ChoiceType = QueryLineType.PlainText,
                        Question = this,
                        Text = text,
                    });

                    reader.Close();
                }

            }

            process.WaitForExit();

        }


        //public override void CreateChoices()
        //{
        //    var process = new Process();
        //    process.StartInfo.FileName = "kubectl";
        //    process.StartInfo.Arguments = this.Arguments;
        //    //process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.Start();

        //    // Synchronously read the standard output of the spawned process. 
        //    using (var reader = process.StandardOutput)
        //    {

        //        string text = "";
        //        try
        //        {
        //            text = reader.ReadToEnd();

        //        }
        //        catch (Exception ex)
        //        {
        //            text = ex.Message;
        //        }

        //        this.Choices.Add(new QueryLine
        //        {
        //            ChoiceType = QueryLineType.PlainText,
        //            Question = this,
        //            Text = text,
        //        });

        //        reader.Close();
        //    }

        //    process.WaitForExit();

        //}

        public override Query GetNextQuery()
        {
            return null;
        }

    }
}
