using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ComponentArt.Web.UI;
using System.ComponentModel;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Core.Controls.GridTemplates
{
    public partial class HyperlinkTemplate : BaseUserControl
    {
        private string _FormatString = String.Empty;

        /// <summary>
        /// Gets or sets the format string.
        /// </summary>
        /// <value>The format string.</value>
        public string FormatString
        {
            get { return _FormatString; }
            set { _FormatString = value; }
        }

        private string _LinkFormatString = String.Empty;

        /// <summary>
        /// Gets or sets the link format string.
        /// </summary>
        /// <value>The link format string.</value>
        public string LinkFormatString
        {
            get { return _LinkFormatString; }
            set { _LinkFormatString = value; }
        }

        private string _Argument = null;

        /// <summary>
        /// Gets or sets the data argument.
        /// </summary>
        /// <value>The data argument.</value>
        public string DataArgument
        {
            get { return _Argument; }
            set { _Argument = value; }
        }

        private string _LinkArgument = null;

        /// <summary>
        /// Gets or sets the link data argument.
        /// </summary>
        /// <value>The link data argument.</value>
        public string LinkDataArgument
        {
            get { return _LinkArgument; }
            set { _LinkArgument = value; }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(LinkDataArgument))
            {
                ArrayList list = new ArrayList();
                string[] argList = LinkDataArgument.Split(new char[] { ',' });
                foreach (string arg in argList)
                {
                    list.Add(((GridServerTemplateContainer)this.Parent).DataItem[arg]);
                }

                MyLink.NavigateUrl = String.Format(LinkFormatString, list.ToArray());
            }

            if (!String.IsNullOrEmpty(DataArgument))
            {
                ArrayList list = new ArrayList();
                string[] argList = DataArgument.Split(new char[] { ',' });
                foreach (string arg in argList)
                {
                    list.Add(((GridServerTemplateContainer)this.Parent).DataItem[arg]);
                }

                MyLink.Text = String.Format(FormatString, list.ToArray());
            }
        }
    }
}