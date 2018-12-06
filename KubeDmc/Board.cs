using KubeDmc.Kub;
using KubeDmc.Questions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KubeDmc
{
    public class Board
    {
        // Contains all queries actually shown on screen
        private readonly Stack<Query> queriesStack = new Stack<Query>();
        private int topPosition = 0;

        public Board()
        {
            Console.CancelKeyPress += (s, e) => Exit();

        }

        private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {

        }

        public void Execute()
        {

            if (!KubService.Current.TryToConnect())
            {
                return;
            }

            Console.CursorVisible = false;

            // initialize position for cursor
            this.topPosition = Console.CursorTop;

            var firstQuery = new ClusterQueries();
            firstQuery.Refresh();
            firstQuery.TopPosition = this.topPosition;

            // first query
            this.queriesStack.Push(firstQuery);

            bool end = false;
            Console.CursorLeft = 0;


            while (!end)
            {
                if (this.queriesStack.Count == 0)
                    break;

                // replace cursor to top, to see all lines (can be hidden if we have scrolled to much )
                Console.CursorTop = this.topPosition;

                // pick the query to write without removing
                var query = this.queriesStack.Peek();

                // Write root question and choices
                this.WriteQuery(query);

                // Wait until a choice is been made
                Query next = null;

                // user wants to back in the stack
                var isBack = false;

                while (next == null && !end && !isBack)
                {
                    var keyInfo = this.ReadChoice(query);

                    // Handle response
                    // Get if we have to continue or if we go to next query (then go to next line)
                    (end, isBack, next) = this.HandleAnswer(query);
                }

                // clean current query, if it's a back, so clean the title too
                CleanQuery(query, isBack);

                // if isBack, remove the query from the stack
                if (isBack || next == null)
                {
                    this.queriesStack.Pop();
                }
                else
                {
                    // push the next query
                    next.Refresh();
                    // increment top position
                    next.TopPosition = query.TopPosition + 1;
                    // push in the stack
                    this.queriesStack.Push(next);
                }

            }

            Exit();

        }

        private void Exit()
        {
            while (this.queriesStack.TryPop(out var q) != false)
            {
                CleanQuery(q, true);
            }
            Console.CursorTop = this.topPosition;
            Console.CursorLeft = 0;
        }


        /// <summary>
        /// Let user navigrate through options, and wait an option is choosen
        /// </summary>
        public ConsoleKeyInfo ReadChoice(Query query)
        {
            // Current line index
            var keyInfo = new ConsoleKeyInfo();
            do
            {
                // get the current selected choice, correponding to the selected line
                Console.CursorTop = query.InProgressChoice.TopPosition;
                Console.CursorLeft = 0;
                var inProgressChoiceIndex = query.Choices.IndexOf(query.InProgressChoice);

                // Don't display the character on Console
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow && inProgressChoiceIndex > 0)
                {
                    this.WriteQueryLine(query.InProgressChoice, false, query.InProgressChoice.TopPosition);
                    inProgressChoiceIndex--;
                    query.InProgressChoice = query.Choices[inProgressChoiceIndex];
                    this.WriteQueryLine(query.InProgressChoice, true, query.InProgressChoice.TopPosition);
                }
                if (keyInfo.Key == ConsoleKey.DownArrow && inProgressChoiceIndex < query.Choices.Count - 1)
                {
                    this.WriteQueryLine(query.InProgressChoice, false, query.InProgressChoice.TopPosition);
                    inProgressChoiceIndex++;
                    query.InProgressChoice = query.Choices[inProgressChoiceIndex];
                    this.WriteQueryLine(query.InProgressChoice, true, query.InProgressChoice.TopPosition);
                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    query.SelectedChoice = query.InProgressChoice;
                }
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    query.SelectedChoice = query.Choices.FirstOrDefault(c => c.ChoiceType == QueryLineType.Back);
                    if (query.SelectedChoice == null)
                        query.SelectedChoice = query.Choices.FirstOrDefault(c => c.ChoiceType == QueryLineType.Exit);

                }

            } while (keyInfo.Key != ConsoleKey.Escape && keyInfo.Key != ConsoleKey.Enter);

            return keyInfo;
        }

        public (bool isEnd, bool isBack, Query nextStep) HandleAnswer(Query question)
        {
            switch (question.SelectedChoice.ChoiceType)
            {
                case QueryLineType.Choice:
                    this.WriteAnswer(question);
                    return (false, false, question.GetNextQuery());
                case QueryLineType.Back:
                    return (false, true, null);
                case QueryLineType.Exit:
                    return (true, true, null);
                case QueryLineType.Input:
                    return (false, true, null);
                case QueryLineType.None:
                case QueryLineType.Title:
                default:
                    return (false, true, null);
            }
        }

        /// <summary>
        /// Write the selected choice at the end of the question line
        /// </summary>
        public void WriteAnswer(Query question)
        {
            var initialForegroundColor = Console.ForegroundColor;

            Console.CursorLeft = question.AnswerCursorLeft;
            Console.CursorTop = question.TopPosition;

            ConsoleExt.DeleteEndOfLine();

            // replace to the correct position
            Console.CursorLeft = question.AnswerCursorLeft;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(question.SelectedChoice.Title);

            Console.ForegroundColor = initialForegroundColor;

        }

        /// <summary>
        /// Write a line, reposition the cursor to next line, and record the current position
        /// </summary>
        private void WriteLine()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// Write a full query
        /// </summary>
        public void WriteQuery(Query query)
        {
            var initialForegroundColor = Console.ForegroundColor;

            Console.CursorLeft = 0;
            Console.CursorTop = query.TopPosition;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("? ");
            Console.ForegroundColor = initialForegroundColor;
            Console.Write(query.Title);
            Console.Write(": ");

            query.TopPosition = Console.CursorTop;

            // save current position for answer
            query.AnswerCursorLeft = Console.CursorLeft;

            var titleChoices = query.Choices.Where(c => c.ChoiceType == QueryLineType.Title).FirstOrDefault();
            if (titleChoices != null)
            {
                this.WriteLine();
                this.WriteQueryLine(titleChoices, false);
            }

            var choices = query.Choices.Where(c => c.ChoiceType == QueryLineType.Choice).ToList();

            if (choices != null)
            {
                for (var i = 0; i < choices.Count; i++)
                {
                    this.WriteLine();
                    this.WriteQueryLine(choices[i], false);
                }

            }

            var texts = query.Choices.Where(c => c.ChoiceType == QueryLineType.PlainText).ToList();

            if (texts != null)
            {
                for (var i = 0; i < texts.Count; i++)
                {
                    this.WriteLine();
                    this.WriteQueryLine(texts[i], false);
                }

            }


            var back = query.Choices.Where(c => c.ChoiceType == QueryLineType.Back).FirstOrDefault();
            if (back != null)
            {
                this.WriteLine();
                this.WriteQueryLine(back, false);
            }

            var exit = query.Choices.Where(c => c.ChoiceType == QueryLineType.Exit).FirstOrDefault();
            if (exit != null)
            {
                this.WriteLine();
                this.WriteQueryLine(exit, false);
            }

            // saving last bottom position
            query.BottomPosition = Console.CursorTop;

            // Select first choice
            QueryLine inProgressChoice = query.InProgressChoice;

            if (inProgressChoice == null)
            {
                if (choices != null && choices.Count > 0)
                    inProgressChoice = choices[0];
                else if (back != null)
                    inProgressChoice = back;
                else
                    inProgressChoice = exit;
            }

            // replacing the selected line with same line, but selected (foreground varies)
            this.WriteQueryLine(inProgressChoice, true, inProgressChoice.TopPosition);
            query.InProgressChoice = inProgressChoice;
        }

        /// <summary>
        /// Write a query line, and select it, if necessary
        /// </summary>
        public void WriteQueryLine(QueryLine queryLine, bool isSelected, int? topPosition = null)
        {

            var initialForegroundColor = Console.ForegroundColor;

           Console.CursorLeft = 0;

            // if topPosition is set, we are in a "replacing" line mode
            if (topPosition.HasValue)
                Console.CursorTop = topPosition.Value;

            // store actual line top position
            queryLine.TopPosition = Console.CursorTop;

            // Delete current line if any characters are already there
            ConsoleExt.DeleteLine(replaceCursor: true);


            // getting line type
            var (choiceC, choiceCC) = this.GetChoiceType(queryLine);

            // for plain text let cursor visible to navigate, more easily
            // Console.CursorVisible = choice.ChoiceType == MenuChoiceType.PlainText;

            Console.ForegroundColor = choiceCC;
            Console.Write(choiceC);

            Console.ForegroundColor = isSelected ? choiceCC : initialForegroundColor;
            Console.Write(queryLine.Text);


            Console.ForegroundColor = initialForegroundColor;
            Console.CursorLeft = 0;
        }


        public (string c, ConsoleColor cc) GetChoiceType(QueryLine choice)
        {
            var initialForegroundColor = Console.ForegroundColor;
            switch (choice.ChoiceType)
            {
                case QueryLineType.Choice:
                    return ("> ", ConsoleColor.DarkBlue);
                case QueryLineType.Back:
                    return ("< ", ConsoleColor.Red);
                case QueryLineType.Exit:
                    return ("< ", ConsoleColor.Red);
                case QueryLineType.Input:
                    return ("* ", ConsoleColor.DarkBlue);
                case QueryLineType.Title:
                    return ("  ", initialForegroundColor);
                case QueryLineType.PlainText:
                case QueryLineType.None:
                default:
                    return ("", initialForegroundColor);
            }
        }

        /// <summary>
        /// Clean a full query, including or not, the first line
        /// </summary>
        private void CleanQuery(Query query, bool includeTitle)
        {
            // Remember cursor position before cleaning
            (int _top, int _left) = (Console.CursorTop, Console.CursorLeft);

            var fromTop = includeTitle ? query.TopPosition : query.TopPosition + 1;

            for (int i = fromTop; i <= query.BottomPosition; i++)
                ConsoleExt.DeleteLine(i);

            // Replace cursor position
            (Console.CursorTop, Console.CursorLeft) = (_top, _left);

        }

        

    }
}
