using System;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Mediachase.Cms.Util;

namespace Mediachase.Web.UI
{
    /// <summary>
    /// Summary description for DataGridExtended.
    /// </summary>
    public class DataGridExtended : DataGrid
    {
        // Constructor that sets some styles and graphical properties

        private readonly static object EventPageSizeChanged = new Object();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridExtended"/> class.
        /// </summary>
        public DataGridExtended()
        {
            PagerStyle.Mode = PagerMode.NumericPages;
            PagerStyle.HorizontalAlign = HorizontalAlign.Right;
            AllowPaging = true;
            ItemCreated += new DataGridItemEventHandler(OnItemCreated);

        }

        // PROPERTY: PagerCurrentPageCssClass
        String m_PagerCurrentPageCssClass = "";
        /// <summary>
        /// Gets or sets the pager current page CSS class.
        /// </summary>
        /// <value>The pager current page CSS class.</value>
        public String PagerCurrentPageCssClass
        {
            get { return m_PagerCurrentPageCssClass; }
            set { m_PagerCurrentPageCssClass = value; }
        }

        // PROPERTY: PagerOtherPageCssClass
        String m_PagerOtherPageCssClass = "";
        /// <summary>
        /// Gets or sets the pager other page CSS class.
        /// </summary>
        /// <value>The pager other page CSS class.</value>
        public String PagerOtherPageCssClass
        {
            get { return m_PagerOtherPageCssClass; }
            set { m_PagerOtherPageCssClass = value; }
        }

        bool m_Fixed = true;
        /// <summary>
        /// Gets or sets a value indicating whether [layout fixed].
        /// </summary>
        /// <value><c>true</c> if [layout fixed]; otherwise, <c>false</c>.</value>
        public bool LayoutFixed
        {
            get { return m_Fixed; }
            set { m_Fixed = value; }
        }

        /// <summary>
        /// Called when [change page size].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void OnChangePageSize(Object sender, System.EventArgs args)
        {
            int _PageSize = 10;
            DropDownList ddl = (DropDownList)sender;
            if (ddl != null)
                _PageSize = int.Parse(ddl.SelectedItem.Value);

            DataGridPageSizeChangedEventArgs dataGridPageSizeChangedEventArgs = new DataGridPageSizeChangedEventArgs(sender, _PageSize);
            OnPageSizeChanged(dataGridPageSizeChangedEventArgs);
        }


        // ...		
        /// <summary>
        /// Raises the <see cref="E:PageSizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Mediachase.Web.UI.DataGridPageSizeChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPageSizeChanged(DataGridPageSizeChangedEventArgs e)
        {
            DataGridPageSizeChangedEventHandler dataGridPageSizeChangedEventHandler = (DataGridPageSizeChangedEventHandler)base.Events[EventPageSizeChanged];
            if (dataGridPageSizeChangedEventHandler != null)
            {
                dataGridPageSizeChangedEventHandler(this, e);
            }
        }

        /// <summary>
        /// Called when [item created].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridItemEventArgs"/> instance containing the event data.</param>
        public void OnItemCreated(Object sender, DataGridItemEventArgs e)
        {
        }

        /// <summary>
        /// Occurs when [page size changed].
        /// </summary>
        public event DataGridPageSizeChangedEventHandler PageSizeChanged
        {
            add
            {
                base.Events.AddHandler(EventPageSizeChanged, value);
            }

            remove
            {
                base.Events.RemoveHandler(EventPageSizeChanged, value);
            }
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.UI.WebControls.DataGridItem"/> object that contains the paging UI.
        /// </summary>
        /// <param name="item">The <see cref="T:System.Web.UI.WebControls.DataGridItem"/> that contains the pager.</param>
        /// <param name="columnSpan">The number of columns to span the pager.</param>
        /// <param name="pagedDataSource">A <see cref="T:System.Web.UI.WebControls.PagedDataSource"/> that contains the properties for the pager.</param>
        protected override void InitializePager(DataGridItem item, int columnSpan, PagedDataSource pagedDataSource)
        {
            int colspan = 0;

            if (LayoutFixed)
            {
                foreach (DataGridColumn dgc in this.Columns)
                    if (dgc.Visible) colspan++;
                this.Style.Add("table-layout", "fixed");
            }
            else
                colspan = columnSpan;

            Table tbl = new Table();
            tbl.CellPadding = 3;
            tbl.CellSpacing = 0;
            tbl.BorderWidth = Unit.Pixel(0);
            tbl.CssClass = "text";
            tbl.Width = Unit.Percentage(100);

            TableRow tr1 = new TableRow();

            TableCell tableCell = new TableCell();
            tableCell.HorizontalAlign = HorizontalAlign.Right;
            //tableCell.ColumnSpan = columnSpan;
            DataGridPagerStyle dataGridPagerStyle = PagerStyle;

            Label lbl = new Label();
            lbl.CssClass = PagerStyle.CssClass;
            lbl.Text = "Page" + " : ";

            tableCell.Controls.Add(lbl);

            TableCell tableCellL = new TableCell();
            Label lblPageSize = new Label();
            lblPageSize.CssClass = PagerStyle.CssClass;
            lblPageSize.Text = "Page Size" + " :&nbsp;";

            DropDownList ddl = new DropDownList();
            ddl.EnableViewState = true;
            ddl.ClearSelection();
            ddl.Items.Add(new ListItem("10 " + "Rows", "10"));
            ddl.Items.Add(new ListItem("25 " + "Rows", "25"));
            ddl.Items.Add(new ListItem("50 " + "Rows", "50"));
            ddl.Items.Add(new ListItem("100 " + "Rows", "100"));
            CommonHelper.SafeSelect(ddl, PageSize.ToString());
            ListItem li = ddl.Items.FindByValue(PageSize.ToString());
            if (li != null)
                li.Selected = true;



            // pager.BorderWidth = Unit.Pixel(1);
            //ddl.CssClass = "text";
            ddl.AutoPostBack = true;
            ddl.SelectedIndexChanged += new System.EventHandler(OnChangePageSize);

            tableCellL.Controls.Add(lblPageSize);
            tableCellL.Controls.Add(ddl);


            int i1 = pagedDataSource.PageCount;
            int j1 = pagedDataSource.CurrentPageIndex + 1;
            int k1 = dataGridPagerStyle.PageButtonCount;
            int i2 = k1;
            if (i1 < i2)
            {
                i2 = i1;
            }
            int j2 = 1;
            int k2 = i2;
            if (j1 > k2)
            {
                j2 = pagedDataSource.CurrentPageIndex / k1 * k1 + 1;
                k2 = j2 + k1 - 1;
                if (k2 > i1)
                {
                    k2 = i1;
                }
                if (k2 - j2 + 1 < k1)
                {
                    j2 = Math.Max(1, k2 - k1 + 1);
                }
            }
            if (j2 != 1)
            {
                LinkButton linkButton3 = new DataGridExtendedLinkButton();
                linkButton3.Text = "...";
                linkButton3.CommandName = "Page";
                int k3 = j2 - 1;
                linkButton3.CommandArgument = k3.ToString(NumberFormatInfo.InvariantInfo);
                linkButton3.CausesValidation = false;
                tableCell.Controls.Add(linkButton3);
                tableCell.Controls.Add(new LiteralControl("&nbsp;"));
            }
            for (int j3 = j2; j3 <= k2; j3++)
            {
                string str = j3.ToString(NumberFormatInfo.InvariantInfo);
                if (j3 == j1)
                {
                    Label label3 = new Label();
                    label3.Font.Bold = true;
                    label3.Text = str;
                    tableCell.Controls.Add(label3);
                }
                else
                {
                    LinkButton linkButton3 = new DataGridExtendedLinkButton();
                    linkButton3.Text = "[ " + str + " ]";
                    linkButton3.CommandName = "Page";
                    linkButton3.CommandArgument = str;
                    linkButton3.CausesValidation = false;
                    tableCell.Controls.Add(linkButton3);
                }
                if (j3 < k2)
                {
                    tableCell.Controls.Add(new LiteralControl("&nbsp;"));
                }
            }
            if (i1 > k2)
            {
                tableCell.Controls.Add(new LiteralControl("&nbsp;"));
                LinkButton linkButton3 = new DataGridExtendedLinkButton();
                linkButton3.Text = "[ ... ]";
                linkButton3.CommandName = "Page";
                int k3 = k2 + 1;
                linkButton3.CommandArgument = k3.ToString(NumberFormatInfo.InvariantInfo);
                linkButton3.CausesValidation = false;
                tableCell.Controls.Add(linkButton3);
            }

            tbl.Rows.Add(tr1);


            tr1.Cells.Add(tableCellL);
            tr1.Cells.Add(tableCell);

            TableCell tc = new TableCell();
            tc.ColumnSpan = colspan;

            tc.Controls.Add(tbl);
            item.Cells.Add(tc);
        }
    }



    public sealed class DataGridPageSizeChangedEventArgs : EventArgs
    {
        private object commandSource;
        private int newPageSize;

        /// <summary>
        /// Gets the command source.
        /// </summary>
        /// <value>The command source.</value>
        public object CommandSource
        {
            get
            {
                return commandSource;
            }
        }

        /// <summary>
        /// Gets the new size of the page.
        /// </summary>
        /// <value>The new size of the page.</value>
        public int NewPageSize
        {
            get
            {
                return newPageSize;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridPageSizeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="commandSource">The command source.</param>
        /// <param name="newPageSize">New size of the page.</param>
        public DataGridPageSizeChangedEventArgs(object commandSource, int newPageSize)
        {
            this.commandSource = commandSource;
            this.newPageSize = newPageSize;
        }
    }

    public delegate void DataGridPageSizeChangedEventHandler(object source, DataGridPageSizeChangedEventArgs e);

    sealed class DataGridExtendedLinkButton : LinkButton
    {

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            SetForeColor();
            base.Render(writer);
        }

        /// <summary>
        /// Sets the color of the fore.
        /// </summary>
        private void SetForeColor()
        {
            Control control = this;
            for (int i = 0; i < 3; i++)
            {
                control = control.Parent;
                Color color = ((WebControl)control).ForeColor;
                if (color != Color.Empty)
                {
                    base.ForeColor = color;
                    return;
                }
            }
        }
    }



}

