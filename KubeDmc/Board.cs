using KubeDmc.Kub;
using KubeDmc.Questions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;

namespace KubeDmc
{
    public class Board
    {
        // Contains all queries actually shown on screen
        private readonly Stack<Query> queriesStack = new Stack<Query>();
        private int topPosition = 0;
        private bool cursorVisibility;
        private Query currentQuery;
        //private Timer refreshTimer = null;
        private System.Timers.Timer refresherTimer;
        private ElapsedEventHandler elapsedEventHandler = null;
        private bool isPaused = false;
        private bool isRefreshing = false;

        public Board()
        {
            Console.CancelKeyPress += (s, e) => Exit();
            this.refresherTimer = new System.Timers.Timer(1000);
        }

        public void DisableTimerForQuery()
        {
            if (this.elapsedEventHandler != null)
                this.refresherTimer.Elapsed -= elapsedEventHandler;

            if (this.refresherTimer.Enabled == true)
                this.refresherTimer.Stop();

            this.isPaused = false;
            this.isRefreshing = false;
        }

        public void PauseTimerForQuery()
        {
            if (this.refresherTimer.Enabled)
            {
                this.isPaused = true;
                this.refresherTimer.Stop();
            }

        }

        public void UnpauseTimerForQuery()
        {
            if (!this.refresherTimer.Enabled && isPaused)
            {
                this.isPaused = false;
                this.refresherTimer.Start();
            }

        }


        public void EnableTimeForQuery()
        {
            elapsedEventHandler = new ElapsedEventHandler((__, eea) =>
            {
                this.isRefreshing = true;
                Debug.WriteLine(this.currentQuery.Title + " is refreshed.");

                // Refresh
                this.currentQuery.Refresh();

                // Write root question and choices
                this.WriteQuery();

                this.isRefreshing = false;

            });

            this.refresherTimer.Elapsed += elapsedEventHandler;
            this.refresherTimer.Start();

        }


        public void Execute()
        {

            if (!KubService.Current.TryToConnect())
            {
                return;
            }
            // since we change the default value, just remember when application ends
            this.cursorVisibility = Console.CursorVisible;

            Console.CursorVisible = false;

            // initialize position for cursor
            this.topPosition = Console.CursorTop;

            var firstQuery = new ClusterQueries();
            firstQuery.Refresh();
            firstQuery.TopPosition = this.topPosition;

            // first query
            this.queriesStack.Push(firstQuery);

            bool isEnd = false;
            Console.CursorLeft = 0;

            while (!isEnd)
            {
                if (this.queriesStack.Count == 0)
                    break;

                // replace cursor to top, to see all lines (can be hidden if we have scrolled to much )
                Console.CursorTop = this.topPosition;

                // pick the query to write without removing
                this.currentQuery = this.queriesStack.Peek();

                // Write root question and choices
                this.WriteQuery();

                if (this.isPaused && this.currentQuery.IsRefreshedEnabled)
                    this.UnpauseTimerForQuery();
                else if (this.currentQuery.IsRefreshedEnabled)
                    this.EnableTimeForQuery();
                else
                    this.DisableTimerForQuery();

                // Wait until a choice is been made
                Query next = null;

                // user wants to back in the stack
                var isBack = false;
                var isRoot = false;
                var isRefresh = false;

                while (next == null && !isEnd && !isBack && !isRefresh)
                {
                    this.ReadChoice();

                    // Handle response
                    // Get if we have to continue or if we go to next query (then go to next line)
                    (isEnd, isBack, isRoot, isRefresh, next) = this.HandleAnswer();
                }

                // clean current query, if it's a back, so clean the title too
                this.CleanQuery(isBack);

                // if refresh, we stay on the same query
                if (isRefresh)
                {
                    next = this.currentQuery;
                    // push the next query
                    next.Refresh();
                }
                // if isBack, remove the query from the stack
                else if (isBack)
                {
                    this.queriesStack.Pop();
                }
                else if (isRoot)
                {
                    while (this.queriesStack.Count > 1)
                        this.queriesStack.Pop();

                }
                else
                {
                    // push the next query
                    next.Refresh();
                    // increment top position
                    next.TopPosition = this.currentQuery.TopPosition + 1;
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
                this.currentQuery = q;
                CleanQuery(true);
            }
            Console.CursorTop = this.topPosition;
            Console.CursorLeft = 0;
            // since we change the default value, just remember when application ends
            Console.CursorVisible = this.cursorVisibility;


        }


        /// <summary>
        /// Let user navigrate through options, and wait an option is choosen
        /// </summary>
        public ConsoleKeyInfo ReadChoice()
        {
            // Current line index
            var keyInfo = new ConsoleKeyInfo();
            bool isSelected = false;
            do
            {
                // get the current selected choice, correponding to the selected line
                Console.CursorTop = this.currentQuery.InProgressChoice.TopPosition;
                Console.CursorLeft = 0;
                var inProgressChoiceIndex = this.currentQuery.QueryLines.IndexOf(this.currentQuery.InProgressChoice);

                // Don't display the character on Console
                keyInfo = Console.ReadKey(true);

                // do nothing until refreshing is ended
                if (this.isRefreshing)
                    continue;

                // disable timer during this phase
                this.PauseTimerForQuery();

                if (keyInfo.Key == ConsoleKey.UpArrow && inProgressChoiceIndex > 0)
                {
                    this.WriteQueryLine(this.currentQuery.InProgressChoice, false, this.currentQuery.InProgressChoice.TopPosition);
                    inProgressChoiceIndex--;
                    this.currentQuery.InProgressChoice = this.currentQuery.QueryLines[inProgressChoiceIndex];
                    this.WriteQueryLine(this.currentQuery.InProgressChoice, true, this.currentQuery.InProgressChoice.TopPosition);
                    this.UnpauseTimerForQuery();
                }
                if (keyInfo.Key == ConsoleKey.DownArrow && inProgressChoiceIndex < this.currentQuery.QueryLines.Count - 1)
                {
                    this.WriteQueryLine(this.currentQuery.InProgressChoice, false, this.currentQuery.InProgressChoice.TopPosition);
                    inProgressChoiceIndex++;
                    this.currentQuery.InProgressChoice = this.currentQuery.QueryLines[inProgressChoiceIndex];
                    this.WriteQueryLine(this.currentQuery.InProgressChoice, true, this.currentQuery.InProgressChoice.TopPosition);
                    this.UnpauseTimerForQuery();
                }
                if ((int)keyInfo.Key >= 65 && (int)keyInfo.Key <= 90)
                {
                    var selectedQueryLine = this.currentQuery.QueryLines.FirstOrDefault(q => q.GetConsoleKey() == keyInfo.Key);
                    if (selectedQueryLine != null)
                    {
                        this.WriteQueryLine(this.currentQuery.InProgressChoice, false, this.currentQuery.InProgressChoice.TopPosition);
                        this.currentQuery.InProgressChoice = selectedQueryLine;
                        this.WriteQueryLine(this.currentQuery.InProgressChoice, true, this.currentQuery.InProgressChoice.TopPosition);
                        this.currentQuery.SelectedChoice = this.currentQuery.InProgressChoice;
                        isSelected = true;
                        this.DisableTimerForQuery();
                    }

                }
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    this.currentQuery.SelectedChoice = this.currentQuery.InProgressChoice;
                    this.DisableTimerForQuery();
                    isSelected = true;
                }
                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    this.currentQuery.SelectedChoice = this.currentQuery.QueryLines.FirstOrDefault(c => c.ChoiceType == QueryLineType.Back);
                    if (this.currentQuery.SelectedChoice == null)
                        this.currentQuery.SelectedChoice = this.currentQuery.QueryLines.FirstOrDefault(c => c.ChoiceType == QueryLineType.Exit);
                    this.DisableTimerForQuery();

                }

            } while (keyInfo.Key != ConsoleKey.Escape && !isSelected);

            return keyInfo;
        }

        public (bool isEnd, bool isBack, bool isRoot, bool isRefresh, Query nextStep) HandleAnswer()
        {
            switch (this.currentQuery.SelectedChoice.ChoiceType)
            {
                case QueryLineType.Choice:
                    this.WriteAnswer();
                    var nextQuery = this.currentQuery.GetNextQuery();
                    bool isRefresh = nextQuery == null;
                    return (false, false, false, isRefresh, nextQuery);
                case QueryLineType.Back:
                    return (false, true, false, false, null);
                case QueryLineType.Exit:
                    return (true, true, false, false, null);
                case QueryLineType.Root:
                    return (false, false, true, false, null);
                case QueryLineType.Input:
                    return (false, true, false, false, null);
                case QueryLineType.None:
                case QueryLineType.Title:
                default:
                    return (false, true, false, false, null);
            }
        }

        /// <summary>
        /// Write the selected choice at the end of the question line
        /// </summary>
        public void WriteAnswer()
        {
            var initialForegroundColor = Console.ForegroundColor;

            Console.CursorLeft = this.currentQuery.AnswerCursorLeft;
            Console.CursorTop = this.currentQuery.TopPosition;

            ConsoleExt.DeleteEndOfLine();

            // replace to the correct position
            Console.CursorLeft = this.currentQuery.AnswerCursorLeft;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(this.currentQuery.SelectedChoice.Title);

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
        public void WriteQuery()
        {
            var initialForegroundColor = Console.ForegroundColor;

            Console.CursorLeft = 0;
            Console.CursorTop = this.currentQuery.TopPosition;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("? ");
            Console.ForegroundColor = initialForegroundColor;
            Console.Write(this.currentQuery.Title);
            Console.Write(": ");

            this.currentQuery.TopPosition = Console.CursorTop;

            // save current position for answer
            this.currentQuery.AnswerCursorLeft = Console.CursorLeft;

            var titleChoices = this.currentQuery.QueryLines.Where(c => c.ChoiceType == QueryLineType.Title).FirstOrDefault();
            if (titleChoices != null)
            {
                this.WriteLine();
                this.WriteQueryLine(titleChoices, false);
            }

            var choices = this.currentQuery.QueryLines.Where(c => c.ChoiceType == QueryLineType.Choice).ToList();

            if (choices != null)
            {
                for (var i = 0; i < choices.Count; i++)
                {
                    this.WriteLine();
                    this.WriteQueryLine(choices[i], false);
                }

            }

            var texts = this.currentQuery.QueryLines.Where(c => c.ChoiceType == QueryLineType.PlainText).ToList();

            if (texts != null)
            {
                for (var i = 0; i < texts.Count; i++)
                {
                    this.WriteLine();
                    this.WriteQueryLine(texts[i], false);
                }

            }

            var root = this.currentQuery.QueryLines.Where(c => c.ChoiceType == QueryLineType.Root).FirstOrDefault();
            if (root != null)
            {
                this.WriteLine();
                this.WriteQueryLine(root, false);
            }

            var back = this.currentQuery.QueryLines.Where(c => c.ChoiceType == QueryLineType.Back).FirstOrDefault();
            if (back != null)
            {
                this.WriteLine();
                this.WriteQueryLine(back, false);
            }

            var exit = this.currentQuery.QueryLines.Where(c => c.ChoiceType == QueryLineType.Exit).FirstOrDefault();
            if (exit != null)
            {
                this.WriteLine();
                this.WriteQueryLine(exit, false);
            }

            // saving last bottom position
            this.currentQuery.BottomPosition = Console.CursorTop;

            // Select first choice
            var inProgressChoice = this.currentQuery.InProgressChoice;

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
            this.currentQuery.InProgressChoice = inProgressChoice;
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

            // color hotkey if supported and not selected
            if (queryLine.HotkeyIndex >= 0 && !isSelected)
            {
                Console.ForegroundColor = choiceCC;
                var letter = queryLine.Text.Substring(queryLine.HotkeyIndex, 1);
                Console.CursorLeft = queryLine.HotkeyIndex + 2; // +2 for first letters "> "
                Console.Write(letter);
            }

            Console.ForegroundColor = initialForegroundColor;
            Console.CursorLeft = 0;
        }


        public (string c, ConsoleColor cc) GetChoiceType(QueryLine choice)
        {
            var initialForegroundColor = Console.ForegroundColor;
            switch (choice.ChoiceType)
            {
                case QueryLineType.Choice:
                    return ("> ", ConsoleColor.Green);
                case QueryLineType.Back:
                    return ("< ", ConsoleColor.Red);
                case QueryLineType.Exit:
                    return ("< ", ConsoleColor.Red);
                case QueryLineType.Input:
                    return ("* ", ConsoleColor.Green);
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
        private void CleanQuery(bool includeTitle)
        {
            // Remember cursor position before cleaning
            (int _top, int _left) = (Console.CursorTop, Console.CursorLeft);

            var fromTop = includeTitle ? this.currentQuery.TopPosition : this.currentQuery.TopPosition + 1;

            for (int i = fromTop; i <= this.currentQuery.BottomPosition; i++)
                ConsoleExt.DeleteLine(i);

            // Replace cursor position
            (Console.CursorTop, Console.CursorLeft) = (_top, _left);

        }



    }
}
