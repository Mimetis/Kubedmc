using KubeDmc.Kub;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace KubeDmc.Questions
{
    public abstract class Query
    {
        public string Answer { get; set; }
        public DataTable DataTable { get; set; }

        public IEnumerable<Item> Items { get; set; }

        /// <summary>
        /// Gets or Sets the initial first line (question) top position
        /// </summary>
        public int TopPosition { get; set; }

        /// <summary>
        /// Gets or Sets the initial first line (question) top position
        /// </summary>
        public int BottomPosition { get; set; }

        /// <summary>
        /// Gets or Sets the Answer cursor left position
        /// </summary>
        public int AnswerCursorLeft { get; set; }

        public List<QueryLine> QueryLines { get; set; }

        public QueryLine SelectedChoice { get; set; }

        public QueryLine InProgressChoice { get; set; }

        /// <summary>
        /// Get the Query Title
        /// </summary>
        public string Title { get; private set; }
        public virtual bool IsRefreshedEnabled { get; } = false;
    
        public Query(string title = null)
        {
            this.Title = title;
        }
        /// <summary>
        /// Handle an answer
        /// </summary>
        public abstract Query GetNextQuery();


        public virtual void RefreshItems()
        {

        }

        public void Initialize()
        {
            if (this.QueryLines != null)
                this.QueryLines.Clear();
            else
                this.QueryLines = new List<QueryLine>();

        }


        public void Refresh()
        {
            // Fill the query
            this.Initialize();

            this.RefreshItems();

            if (this.Items != null)
                this.DataTable = GetDataTable(this.Items);

            this.CreateChoicesTitle();
            this.CreateChoices();
            this.CreateBackOption();
            this.CreateExit();
            this.SetSelectedChoice();
            this.SetInProgressChoice();

        }


        /// <summary>
        /// Create the options we can have (exit, back) and return new top position
        /// </summary>
        public virtual void CreateBackOption()
        {
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Back,
                Question = this,
                Text = "Back",
                HotkeyIndex = 0
            });

        }

        public virtual void CreateExit()
        {
            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Exit,
                Question = this,
                Text = "Exit",
                HotkeyIndex = 0
            });
        }

        /// <summary>
        /// Create the choices and return new top position
        /// </summary>
        public virtual void CreateChoicesTitle()
        {
            if (this.Items == null)
                return;

            if (this.DataTable == null || this.DataTable.Columns.Count <= 0)
                return;

            string text = "";
            foreach (DataColumn c in this.DataTable.Columns)
                text += c.Caption;

            this.QueryLines.Add(new QueryLine
            {
                ChoiceType = QueryLineType.Title,
                Question = this,
                Text = text
            });

        }
        /// <summary>
        /// Create the choices and return new top position
        /// </summary>
        public virtual void CreateChoices()
        {
            if (this.Items == null)
                return;

            if (this.DataTable == null || this.DataTable.Rows.Count <= 0)
                return;

            foreach (DataRow row in this.DataTable.Rows)
            {
                var text = string.Concat(row.ItemArray);

                this.QueryLines.Add(new QueryLine
                {
                    ChoiceType = QueryLineType.Choice,
                    Question = this,
                    // Assuming first column is always row name property
                    Title = row[0].ToString(),
                    Text = text,
                    Item = this.Items.First(i => i.Name.ToLowerInvariant() == row["Name"].ToString().Trim().ToLowerInvariant())
                });
            }
        }

        /// <summary>
        /// Create one input if needed and return new top position
        /// </summary>
        public virtual void CreateInput()
        {

        }

        /// <summary>
        /// Set the correct choice. If null, set to the correct default one
        /// </summary>
        public virtual void SetSelectedChoice()
        {

        }

        /// <summary>
        /// Set the in progress choice. If null, set to the correct default one
        /// </summary>
        public virtual void SetInProgressChoice()
        {

        }


        public DataTable GetDataTable(IEnumerable<Item> items)
        {
            var dataTable = new DataTable();

            var space = 3;

            var lstItems = items.ToList();

            if (lstItems == null || lstItems.Count <= 0)
                return null;

            // Get all properties from my item
            var properties = lstItems[0].GetType().GetProperties();

            var cacheItems = new List<QueryCacheItem>();

            // Reflection to get custom attributes
            // TODO : May be cached later ?
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttributes(typeof(ItemAttribute), true).FirstOrDefault();

                if (attr != null)
                {
                    var itemAttribute = (ItemAttribute)attr;
                    cacheItems.Add(new QueryCacheItem { AttributeName = itemAttribute.Name, Order = itemAttribute.Order, PropertyName = prop.Name });
                }
            }

            foreach (var prop in cacheItems.OrderBy(tmp => tmp.Order))
            {
                // column name from the property, easy way to find when iterate trough lines
                var column = new DataColumn(prop.PropertyName)
                {
                    // calculate the length of the most long term in the list + needed space to separate columns
                    MaxLength = Math.Max(prop.AttributeName.Length, items.Max(item => item.GetType().GetProperty(prop.PropertyName).GetValue(item).ToString().Length)) + space
                };

                // The caption represents the name + all the spaces required
                column.Caption = $"{prop.AttributeName}{ new string(' ', column.MaxLength - prop.AttributeName.Length) }";

                dataTable.Columns.Add(column);

            }

            foreach (var item in items)
            {
                var newRow = dataTable.NewRow();
                foreach (var prop in properties)
                {
                    var column = dataTable.Columns[prop.Name];

                    if (column == null)
                        continue;

                    // Get value
                    var value = prop.GetValue(item).ToString();

                    // compute with correct spaces
                    value += new string(' ', column.MaxLength - value.Length);

                    newRow[prop.Name] = value;
                }


                dataTable.Rows.Add(newRow);
            }

            return dataTable;

        }

    }


}
