using System;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Manager.Core.MetaData.MetaControls
{
	/// <summary>
    ///		Summary description for StringDictionaryControl.
	/// </summary>
    public partial class StringDictionaryControl : CoreBaseUserControl, IMetaControl
	{
        /// <summary>
        /// Gets or sets the dictionary items table.
        /// </summary>
        /// <value>The dictionary items table.</value>
        private DataTable DictionaryItemsTable
        {
            get
            {
                return Session["StringDictionaryItems-" + this.ClientID] as DataTable;
            }
            set
            {
                Session["StringDictionaryItems-" + this.ClientID] = value;
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
 			ApplyLocalization();
		}

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			MetaLabelCtrl.Text = String.Format("{0} ({1})", MetaField.FriendlyName, LanguageCode);
            MetaDescriptionCtrl.Text = MetaField.Description;

			if(DictionaryItemsTable != null)
            {
                ItemsGrid.DataSource = DictionaryItemsTable;
                ItemsGrid.DataBind();
            }
            else
                BindData();

			base.DataBind();
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		private void ApplyLocalization()
		{
            AddStringDictionaryItemButton.Text = RM.GetString("STRINGDICTIONARYCONTROL_ADD_ITEM");

            ItemsGrid.Columns[0].HeaderText = RM.GetString("STRINGDICTIONARYCONTROL_HDR_KEY");
            ItemsGrid.Columns[1].HeaderText = RM.GetString("STRINGDICTIONARYCONTROL_HDR_VALUE");
            ItemsGrid.Columns[2].HeaderText = RM.GetString("GENERAL_OPTIONS");
		}

        /// <summary>
        /// Binds the data.
        /// </summary>
		private void BindData()
		{
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("Key", typeof(string)), new DataColumn("Value", typeof(string)) });

            if (MetaObject != null)
            {
                MetaStringDictionary dict = (MetaStringDictionary)MetaObject.GetStringDictionary(MetaField);
                if (dict != null)
                {
                    foreach (string key in dict.Keys)
                    {
                        DataRow row = dt.NewRow();
                        row["Key"] = key;
                        row["Value"] = dict[key];
                        dt.Rows.Add(row);
                    }
                }
            }

            DictionaryItemsTable = dt;

            this.ItemsGrid.DataSource = dt;
            this.ItemsGrid.DataBind();
		}

        /// <summary>
        /// Handles the Click event of the AddStringDictionaryItemButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddStringDictionaryItemButton_Click(object sender, System.EventArgs e)
        {
            if (!rfValidatorKey.IsValid || !rfValidatorValue.IsValid)
                return;

            DataTable dt = DictionaryItemsTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] { new DataColumn("Key", typeof(string)), new DataColumn("Value", typeof(string)) });
            }

            DataRow row = dt.NewRow();
            row["Key"] = Key.Text;
            row["Value"] = Value.Text;
            dt.Rows.Add(row);

            DictionaryItemsTable = dt;

            this.ItemsGrid.DataSource = dt;
            this.ItemsGrid.DataBind();

            Key.Text = String.Empty;
            Value.Text = String.Empty;
        }

        #region Standard DataGrid Functions
        /// <summary>
        /// Handles the Delete event of the ItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
        private void ItemsGrid_Delete(object sender, DataGridCommandEventArgs e)
        {
            DataTable dt = DictionaryItemsTable;
            if (dt != null)
                dt.Rows.RemoveAt(e.Item.ItemIndex);

            DictionaryItemsTable = dt;

            this.ItemsGrid.DataSource = dt;
            this.ItemsGrid.DataBind();
        }

        /// <summary>
        /// Handles the Cancel event of the ItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
        private void ItemsGrid_Cancel(object sender, DataGridCommandEventArgs e)
        {
            ItemsGrid.EditItemIndex = -1;

            this.ItemsGrid.DataSource = DictionaryItemsTable;
            this.ItemsGrid.DataBind();
        }

        /// <summary>
        /// Handles the Update event of the ItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
        private void ItemsGrid_Update(object sender, DataGridCommandEventArgs e)
        {
            DataTable dt = DictionaryItemsTable;
            TextBox tbValue = (TextBox)e.Item.FindControl("tbValue");
            if (tbValue != null)
                dt.Rows[ItemsGrid.EditItemIndex]["Value"] = tbValue.Text;

            ItemsGrid.EditItemIndex = -1;

            DictionaryItemsTable = dt;

            this.ItemsGrid.DataSource = dt;
            this.ItemsGrid.DataBind();
        }

        /// <summary>
        /// Handles the EditCommand event of the ItemsGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.DataGridCommandEventArgs"/> instance containing the event data.</param>
        private void ItemsGrid_EditCommand(object sender, DataGridCommandEventArgs e)
        {
            ItemsGrid.EditItemIndex = e.Item.ItemIndex;

            this.ItemsGrid.DataSource = DictionaryItemsTable;
            this.ItemsGrid.DataBind();
        }

        #endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            ItemsGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(ItemsGrid_EditCommand);
            ItemsGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(ItemsGrid_Cancel);
            ItemsGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(ItemsGrid_Update);
            ItemsGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(ItemsGrid_Delete);
		}
		#endregion

		#region IMetaControl Members
        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        public string ValidationGroup
        {
            get
            {
                return null; //rfValidatorKey.ValidationGroup;
            }
            set
            {
                //rfValidatorKey.ValidationGroup = value;
            }
        }

        private MetaField _MetaField;
        /// <summary>
        /// Gets or sets the meta field.
        /// </summary>
        /// <value>The meta field.</value>
        public MetaField MetaField
        {
            get
            {
                return _MetaField;
            }
            set
            {
                _MetaField = value;
            }
        }


        MetaObject _MetaObject;
        /// <summary>
        /// Gets or sets the meta object.
        /// </summary>
        /// <value>The meta object.</value>
        public MetaObject MetaObject
        {
            get
            {
                return _MetaObject;
            }
            set
            {
                _MetaObject = value;
            }
        }

        private string _LanguageCode;
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>The language code.</value>
        public string LanguageCode
        {
            get
            {
                return _LanguageCode;
            }
            set
            {
                _LanguageCode = value;
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
		public void Update()
		{
            if (DictionaryItemsTable != null)
            {
                MetaStringDictionary dict = new MetaStringDictionary();
                foreach (DataRow row in DictionaryItemsTable.Rows)
                    dict.Add((string)row["Key"], (string)row["Value"]);

                MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, new object[] { dict });
            }
            else
                MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, new object[] { null });
		}
		#endregion
	}
}
