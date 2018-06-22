using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using Mediachase.Web.Console.BaseClasses;

namespace Mediachase.Commerce.Manager.Core
{
    public partial class SaveControl : BaseUserControl
    {
        public class SaveEventArgs : EventArgs
        {
            public bool RunScript = true;
        }

        public event EventHandler<SaveEventArgs> SaveChanges;
        public event EventHandler<SaveEventArgs> Cancel;
        public event EventHandler<SaveEventArgs> Delete;

        /// <summary>
        /// Saveds the notify.
        /// </summary>
        /// <param name="message">The message.</param>
        private void SavedNotify(string message)
        {
            //if (!Page.ClientScript.IsClientScriptBlockRegistered("ecf-SaveControl-SaveScript"))
            {
                if (!String.IsNullOrEmpty(SavedClientScript))
                    ScriptManager.RegisterStartupScript(this, typeof(SaveControl), "ecf-SaveControl-SaveScript", String.Format(SavedClientScript), true);
            }
            

            //if (!Page.ClientScript.IsStartupScriptRegistered("ecf-SaveControl"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(SaveControl), "ecf-SaveControl", String.Format("CSManagementClient.SetSaveStatus('{0}');", message), true);
            }
        }

        /// <summary>
        /// Cancels the notify.
        /// </summary>
        /// <param name="message">The message.</param>
        private void CancelNotify(string message)
        {
            //if (!ScriptManager.IsClientScriptBlockRegistered("ecf-SaveControl-CancelScript"))
            {
                if (!String.IsNullOrEmpty(CancelClientScript))
                    ScriptManager.RegisterStartupScript(this, typeof(SaveControl), "ecf-SaveControl-CancelScript", String.Format(CancelClientScript), true);
            }


            //if (!Page.ClientScript.IsStartupScriptRegistered("ecf-SaveControl"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(SaveControl), "ecf-SaveControl", String.Format("CSManagementClient.SetSaveStatus('{0}');", message), true);
            }
        }

        /// <summary>
        /// Deletes the notify.
        /// </summary>
        /// <param name="message">The message.</param>
        private void DeleteNotify(string message)
        {
            //if (!ScriptManager.IsClientScriptBlockRegistered("ecf-SaveControl-CancelScript"))
            {
                if (!String.IsNullOrEmpty(DeleteClientScript))
                    ScriptManager.RegisterStartupScript(this, typeof(SaveControl), "ecf-SaveControl-DeleteScript", String.Format(DeleteClientScript), true);
            }


            //if (!Page.ClientScript.IsStartupScriptRegistered("ecf-SaveControl"))
            {
                ScriptManager.RegisterStartupScript(this, typeof(SaveControl), "ecf-SaveControl", String.Format("CSManagementClient.SetSaveStatus('{0}');", message), true);
            }
        }

		private string _SavedMessage = String.Empty;
        /// <summary>
        /// Gets or sets the saved message.
        /// </summary>
        /// <value>The saved message.</value>
        public string SavedMessage
        {
            get
            {
                return _SavedMessage;
            }
            set
            {
                _SavedMessage = value;
            }
        }

		private string _CancelMessage = String.Empty;
        /// <summary>
        /// Gets or sets the cancel message.
        /// </summary>
        /// <value>The cancel message.</value>
        public string CancelMessage
        {
            get
            {
                return _CancelMessage;
            }
            set
            {
                _CancelMessage = value;
            }
        }

		private string _DeleteMessage = String.Empty;
        /// <summary>
        /// Gets or sets the delete message.
        /// </summary>
        /// <value>The delete message.</value>
        public string DeleteMessage
        {
            get
            {
                return _DeleteMessage;
            }
            set
            {
                _DeleteMessage = value;
            }
        }

        private bool _ShowDeleteButton = false;

        /// <summary>
        /// Gets or sets a value indicating whether [show delete button].
        /// </summary>
        /// <value><c>true</c> if [show delete button]; otherwise, <c>false</c>.</value>
        public bool ShowDeleteButton
        {
            get { return _ShowDeleteButton; }
            set 
            { 
                if (!_PermissionOverrideHideDeleteButton)
                    _ShowDeleteButton = value; 
            }
        }
		
        private bool _PermissionOverrideHideDeleteButton = false;

        /// <summary>
        /// Use permissions setting to determine whether delete button shows. Overrides other attempts to set visibility.
        /// </summary>
        /// <value>Overrides ShowDeleteButton <c>true</c> if [show delete button]; otherwise, <c>false</c>.</value>
        public void PermissionOverrideHideDeleteButton()
        {
                //only to be used to hide button
                _ShowDeleteButton = false;
                _PermissionOverrideHideDeleteButton = true; 
        }

        private string _SavedClientScript = String.Empty;
        /// <summary>
        /// Gets or sets the saved client script.
        /// </summary>
        /// <value>The saved client script.</value>
        public string SavedClientScript
        {
            get
            {
                return _SavedClientScript;
            }
            set
            {
                _SavedClientScript = value;
            }
        }

        private string _CancelClientScript = String.Empty;
        /// <summary>
        /// Gets or sets the cancel client script.
        /// </summary>
        /// <value>The cancel client script.</value>
        public string CancelClientScript
        {
            get
            {
                return _CancelClientScript;
            }
            set
            {
                _CancelClientScript = value;
            }
        }

        private string _DeleteClientScript = String.Empty;
        /// <summary>
        /// Gets or sets the delete client script.
        /// </summary>
        /// <value>The delete client script.</value>
        public string DeleteClientScript
        {
            get
            {
                return _DeleteClientScript;
            }
            set
            {
                _DeleteClientScript = value;
            }
        }

        /// <summary>
        /// Called when [save changes].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnSaveChanges(object sender, EventArgs e)
        {
            EventHandler<SaveEventArgs> handler = SaveChanges;
            SaveEventArgs args = new SaveEventArgs();
            if (handler != null)
            {
                handler(sender, args);
            }

            if(args.RunScript)
                SavedNotify(SavedMessage);
        }

        /// <summary>
        /// Called when [cancel].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnCancel(object sender, EventArgs e)
        {
            EventHandler<SaveEventArgs> handler = Cancel;
            SaveEventArgs args = new SaveEventArgs();
            if (handler != null)
            {
                handler(sender, args);
            }

            if (args.RunScript)
                CancelNotify(CancelMessage);
        }

        /// <summary>
        /// Called when [delete].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDelete(object sender, EventArgs e)
        {
            EventHandler<SaveEventArgs> handler = Delete;
            SaveEventArgs args = new SaveEventArgs();
            if (handler != null)
            {
                handler(sender, args);
            }

            if (args.RunScript)
                DeleteNotify(DeleteMessage);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            SaveChangesButton.Click += new EventHandler(OnSaveChanges);
            CancelButton.Click += new EventHandler(OnCancel);
            DeleteButton.Click += new EventHandler(OnDelete);
            DeleteButton.Visible = ShowDeleteButton;

            if (!this.IsPostBack)
            {
				string confirm = "return confirm('" + RM.GetString("DeleteSelectedItemsConfirmation") + "')";
                DeleteButton.Attributes.Add("onclick", confirm);                
            }
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			/*Type csType = this.GetType();
			string csName = "SaveControl_DisableButtons";

			string csText = "alert('Here'); " +
					"if (document.all || document.getElementById) {" +
						"alert('Here2'); " +
						"for (i = 0; i < document.forms[0].length; i++) {" +
							"var tempobj = document.forms[0].elements[i];" +
							"if (tempobj.type.toLowerCase() == \"submit\")" +
							"tempobj.disabled = true;}" +
						"return true;" +
					"}";

			if (!this.Page.ClientScript.IsOnSubmitStatementRegistered(csType, csName))
				ScriptManager.RegisterOnSubmitStatement(this, csType, csName, csText);*/

			/*string validationtext = "if (typeof(Page_ClientValidate) == 'function') { " +
				"if (Page_ClientValidate('" + SaveChangesButton.ValidationGroup + "') == false) { return false; }} ";*/

			/*string scriptString = "javascript:" +
				  SaveChangesButton.ClientID + ".disabled=true;" +
				  CancelButton.ClientID + ".disabled=true;" +
				  "if(document.getElementById('" + DeleteButton.ClientID + "')!=null) " + DeleteButton.ClientID + ".disabled=true;";

			SaveChangesButton.Attributes.Add("onclick", scriptString + this.Page.ClientScript.GetPostBackEventReference(SaveChangesButton, ""));
			CancelButton.Attributes.Add("onclick", scriptString + this.Page.ClientScript.GetPostBackEventReference(CancelButton, ""));
			if (DeleteButton.Visible)
				DeleteButton.Attributes.Add("onclick", scriptString + this.Page.ClientScript.GetPostBackEventReference(DeleteButton, ""));
			*/

            if (String.IsNullOrEmpty(SavedMessage))
			    SavedMessage = RM.GetString("ChangesSavedMsg");
            if (String.IsNullOrEmpty(CancelMessage))
			    CancelMessage = RM.GetString("ChangesDiscardedMsg");
            if (String.IsNullOrEmpty(DeleteMessage))
			    DeleteMessage = RM.GetString("ItemDeletedMsg");

			base.OnInit(e);
		}
    }
}