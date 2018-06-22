using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Text;

namespace Mediachase.Commerce.Manager.Apps.Core.Controls
{
	public delegate void ProgressCompletedHandler(object data);

    public partial class ProgressControl : System.Web.UI.UserControl
    {
		public event ProgressCompletedHandler ProgressCompleted;

        private string _ProgressStatus = String.Empty;
        private string _ProgressMessage = String.Empty;
        private string _LatestMessage = String.Empty;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                return TitleLabel.Text;
            }
            set
            {
                TitleLabel.Text = value;
            }
        }

		/// <summary>
		/// Raises the <see cref="E:ProgressCompleted"/> event.
		/// </summary>
		/// <param name="data"></param>
		protected virtual void OnImportExportProgressMessage(object data)
		{
			if (this.ProgressCompleted != null)
				this.ProgressCompleted(data);
		}

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            _ProgressStatus = this.ClientID + "_ProgressStatus";
            _ProgressMessage = this.ClientID + "_ProgressMessage";
            _LatestMessage = this.ClientID + "_LatestMessage";
        }

        /// <summary>
        /// Starts the operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public void StartOperation(ThreadStart operation)
        {
            _ProgressStatus = this.ClientID + "_ProgressStatus";
            _ProgressMessage = this.ClientID + "_ProgressMessage";
            _LatestMessage = this.ClientID + "_LatestMessage";

            // reset session variables
            Session[_ProgressStatus] = 0;
            Session[_ProgressMessage] = String.Empty;
            Session[_LatestMessage] = String.Empty;

            this.ModalPopupExtender.Show();

            System.Threading.Thread thread = new System.Threading.Thread(operation);
            thread.IsBackground = true;
            thread.Start();

            // set initial controls state
            lblProgress.InnerHtml = String.Empty;
            ProgressStatusMessage.Text = String.Empty;
            pnlTimer.Enabled = true;
        }

        /// <summary>
        /// Handles the Tick event of the pnlTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnTimer(object sender, EventArgs e)
        {
            this.ModalPopupExtender.Show();
            object stat = Session[_ProgressStatus];
            int progressValue = 0;
            if (stat != null && Int32.TryParse(stat.ToString(), out progressValue))
            {
                int statusWidth = Convert.ToInt32(398 * progressValue / 100);
                if (statusWidth < 0)
                    statusWidth = 0;

                if (progressValue >= 0)
                    amountComplete.InnerHtml = progressValue.ToString() + "% complete";
                else
                    amountComplete.InnerHtml = "Failed";

                statusBar.Style.Add(HtmlTextWriterStyle.Width, statusWidth.ToString() + "px");
                statusBar.Visible = true;
                // 100 - export succeeded, -1 - export failed
                if (progressValue == 100 || progressValue == -1)
                {
                    pnlTimer.Enabled = false;

					OnImportExportProgressMessage(progressValue);
                }

                // update message from export method
                lblProgress.InnerHtml = (string)Session[_ProgressMessage];
                ProgressStatusMessage.Text = (string)Session[_LatestMessage];
            }
        }

        /// <summary>
        /// Adds the progress message text.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddProgressMessageText(string message, int percent)
        {
            AddProgressMessageText(message, false, percent);
        }

        /// <summary>
        /// Adds the progress message text.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="error">if set to <c>true</c> [error].</param>
        public void AddProgressMessageText(string message, bool error, int percent)
        {
            Session[_ProgressStatus] = percent;
            if (!error)
            {
                string msg = String.Format("{0}: {1}", DateTime.Now, message);
                Session[_LatestMessage] = msg;
                Session[_ProgressMessage] = msg + "<br />" + Session[_ProgressMessage];
            }
            else
            {
                string msg = String.Format("<font color=\"red\">{0}: {1}</font>", DateTime.Now, message);
                Session[_LatestMessage] = msg;
                StringBuilder sb = new StringBuilder((string)Session[_ProgressMessage]);
                sb.Insert(0, msg + "<br />");
                sb.AppendLine();
                Session[_ProgressMessage] = sb.ToString();
            }
        }
    }
}