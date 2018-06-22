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
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Storage;

namespace Mediachase.Commerce.Manager.Core.MetaData.MetaControls
{
	/// <summary>
	///		Summary description for IntegerControl.
	/// </summary>
    public partial class MultiValueControl : CoreBaseUserControl, IMetaControl
	{
        /// <summary>
        /// Gets or sets the dictionary items table.
        /// </summary>
        /// <value>The dictionary items table.</value>
        private DataTable DictionaryItemsTable
        {
            get
            {
                return Session["MultiValueDictionaryItems-" + this.ClientID] as DataTable;
            }
            set
            {
                Session["MultiValueDictionaryItems-" + this.ClientID] = value;
            }
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
		}

		public override void DataBind()
		{
            if (MetaField.DataType == MetaDataType.EnumMultiValue)
            {
                MetaValueCtrl.Visible = false;
                AddValue.Visible = false;
            }
            else
            {
                MetaValueCtrl.Visible = true;
                AddValue.Visible = true;
            }

			MetaLabelCtrl.Text = String.Format("{0} ({1})", MetaField.FriendlyName, LanguageCode);
            MetaDescriptionCtrl.Text = MetaField.Description;
            MetaMultiValueCtrl.SelectionMode = ListSelectionMode.Multiple;

			BindData();

            //base.DataBind();

		}

		private void ApplyLocalization()
		{
			AddValue.Text = RM.GetString("MULTIVALUECONTROL_ADD_VALUE");
		}

		private void BindData()
		{
            //foreach(string item in MetaField.DictionaryItems.Values)
            //{
            //    MetaMultiValueCtrl.Items.Add(new ListItem(item, item));
            //}

            if (this.IsPostBack && DictionaryItemsTable != null)
            {
                MetaMultiValueCtrl.DataSource = DictionaryItemsTable;
                MetaMultiValueCtrl.DataBind();
            }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] { new DataColumn("Id", typeof(int)), new DataColumn("Value", typeof(string)) });

                MetaDictionary dictionary = MetaField.Dictionary;
                if (dictionary != null)
                {
                    foreach (MetaDictionaryItem item in dictionary)
                    {
                        DataRow row = dt.NewRow();
                        row["Id"] = item.Id;
                        row["Value"] = item.Value;
                        dt.Rows.Add(row);
                    }
                }

                DictionaryItemsTable = dt;

                this.MetaMultiValueCtrl.DataSource = dt;
                this.MetaMultiValueCtrl.DataBind();
            }

            //if(MetaField.Value!=null)
            //foreach(string str in (string[])MetaField.Value)
            //{
            //    Util.CommonHelper.SelectListItem(MetaMultiValueCtrl, str, false);
            //}

            if (MetaObject != null)
            {
                MetaDictionaryItem[] items = MetaObject.GetDictionaryItems(MetaField);
                foreach (MetaDictionaryItem item in items)
                {
                    ManagementHelper.SelectListItem(MetaMultiValueCtrl, item.Id, false);
                }
            }
		}

        #region IMetaControl Members
        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        public string ValidationGroup
        {
            get
            {
                return "";
            }
            set{}
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

        public void Update()
        {

            // Add new values if any
            if (DictionaryItemsTable != null)
            {
                foreach (DataRow row in DictionaryItemsTable.Rows)
                {
                    if ((int)row["Id"] == -1)
                    {
                        int id = MetaField.Dictionary.Add((string)row["Value"]);
                        row["Id"] = id;
                        foreach (ListItem item in MetaMultiValueCtrl.Items)
                        {
                            if (Int32.Parse(item.Value) == -1 && String.Compare((string)row["Value"], item.Text) == 0)
                                item.Value = id.ToString();
                        }
                    }
                }
            }

            //// Add new values if any
            //foreach(ListItem item in MetaMultiValueCtrl.Items)
            //{
            //    if(item.Value.Length == 0)
            //        MetaField.DictionaryAdd(item.Text);
            //}

            // Save selected values
            ArrayList selected = new ArrayList();
            foreach(ListItem item in MetaMultiValueCtrl.Items)
            {
                if(item.Selected)
                    selected.Add(item.Value);
            }

            MetaHelper.SetMetaFieldValue(MetaField.Context, MetaObject, MetaField.Name, (string[])selected.ToArray(typeof(string)));

            //MetaField.Value = (string[])selected.ToArray(typeof(string));
		}
		#endregion

		protected void AddValue_Click(object sender, System.EventArgs e)
		{
            DataTable dt = DictionaryItemsTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] { new DataColumn("Id", typeof(int)), new DataColumn("Value", typeof(string)) });
            }

            if (MetaMultiValueCtrl.Items.FindByText(MetaValueCtrl.Text) == null)
            {
                DataRow row = dt.NewRow();
                row["Id"] = -1;
                row["Value"] = MetaValueCtrl.Text;
                dt.Rows.Add(row);

                DictionaryItemsTable = dt;
            }

            this.MetaMultiValueCtrl.DataSource = dt;
            this.MetaMultiValueCtrl.DataBind();

            MetaValueCtrl.Text = String.Empty;

            if (MetaObject != null)
            {
                MetaDictionaryItem[] items = MetaObject.GetDictionaryItems(MetaField);
                foreach (MetaDictionaryItem item in items)
                {
                    ManagementHelper.SelectListItem(MetaMultiValueCtrl, item.Id, false);
                }
            }
		}
	}
}
