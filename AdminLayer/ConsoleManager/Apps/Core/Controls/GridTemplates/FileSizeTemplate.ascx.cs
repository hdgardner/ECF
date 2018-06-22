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
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Core.Controls.GridTemplates
{
    public partial class FileSizeTemplate : BaseUserControl
    {
		private string _FileSizeArgument = String.Empty;

        /// <summary>
        /// Gets or sets the file size argument.
        /// </summary>
        /// <value>The file size argument.</value>
		public string FileSizeArgument
        {
			get { return _FileSizeArgument; }
			set { _FileSizeArgument = value; }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(FileSizeArgument) || String.IsNullOrEmpty(FileSizeArgument))
                return;

			GridServerTemplateContainer container = this.Parent as GridServerTemplateContainer;
			if (container != null)
			{
				object fileSize = container.DataItem[FileSizeArgument];
				if (fileSize != null)
				{
					long size = 0;
					if (long.TryParse(fileSize.ToString(), out size))
						SizeText.Text = CommerceHelper.ByteSizeToStr(size);
				}
			}
		}
    }
}