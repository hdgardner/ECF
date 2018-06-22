using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace Mediachase.Web.Console.Controls
{
    public class JavaScriptHyperLinkField : HyperLinkField
    {
        /// <summary>
        /// Initializes a cell in a <see cref="T:System.Web.UI.WebControls.HyperLinkField"/> object.
        /// </summary>
        /// <param name="cell">A <see cref="T:System.Web.UI.WebControls.DataControlFieldCell"/> that contains the text or controls of the <see cref="T:System.Web.UI.WebControls.HyperLinkField"/>.</param>
        /// <param name="cellType">One of the <see cref="T:System.Web.UI.WebControls.DataControlCellType"/> values.</param>
        /// <param name="rowState">One of the <see cref="T:System.Web.UI.WebControls.DataControlRowState"/> values that specifies the state of the row containing the <see cref="T:System.Web.UI.WebControls.HyperLinkField"/>.</param>
        /// <param name="rowIndex">The index of the row in the table.</param>
        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            // Trick the thing
            if (cellType == DataControlCellType.DataCell)
                this.DataNavigateUrlFormatString = this.DataNavigateUrlFormatString.Replace("javascript:", "[javascript]");

            base.InitializeCell(cell, cellType, rowState, rowIndex);
            if (cellType == DataControlCellType.DataCell)
            {
                if(cell.Controls.Count == 0)
                    return;

                HyperLink link = (HyperLink)cell.Controls[0];
                link.DataBinding += new EventHandler(link_DataBinding);
            }
        }

        /// <summary>
        /// Handles the DataBinding event of the link control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void link_DataBinding(object sender, EventArgs e)
        {
            HyperLink link = (HyperLink)sender;
            link.NavigateUrl = link.NavigateUrl.Replace("[javascript]", "javascript:");
        }
    }
}
