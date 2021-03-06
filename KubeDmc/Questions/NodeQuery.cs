﻿using KubeDmc.Kub;
using System.Collections.Generic;
using System.Diagnostics;

namespace KubeDmc.Questions
{
    public class NodeQuery : Query
    {
        public Node Node { get; }

        public NodeQuery(Node node) : base("Choose a ressource")
        {
            this.Node = node;
        }

        public override void CreateBackOption()
        {
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Back,
                Question = this,
                Text = "Back",
            });
        }

        public override void CreateChoices()
        {
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Choice,
                Question = this,
                Text = $"[Describe]",
                Title = "Decribe",
                Kind = "describe",
                HotkeyIndex = 1
            });
        }

 

      
        public override Query GetNextQuery()
        {
            switch (this.SelectedChoice.Kind)
            {
                case "describe":
                default:
                    return new ProcessQuery(this.Node.Name, $"describe node {this.Node.Name}",  this);
            }

        }
    }
}
