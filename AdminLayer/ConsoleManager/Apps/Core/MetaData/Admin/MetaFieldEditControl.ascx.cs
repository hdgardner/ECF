using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Manager.Core.MetaData.Admin
{
	/// <summary>
	///		Summary description.
	/// </summary>
	public partial class MetaFieldEditControl : CoreBaseUserControl
    {
		private const string _CatalogSystemName = "Catalog";
		private const string _ProfileSystemName = "Profile";
		private const string _OrderSystemName = "Order";

		MetaField _metaField = null;
		private MetaField CurrentMetaField
		{
			get
			{
				if (_metaField == null && AttributeId > 0)
					_metaField = MetaField.Load(MDContext, AttributeId);

				return _metaField;
			}
		}   

        #region Private Properties
		private int _AttributeId = 0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the attribute id.
        /// </summary>
        /// <value>The attribute id.</value>
        public int AttributeId
		{
			get
			{
				object id = Request.QueryString["id"];
				if (id == null)
					return _AttributeId;
				else
					return Int32.Parse(id.ToString());
			}
			set
			{
				_AttributeId = value;
			}
        }

        /// <summary>
        /// Gets the field namespace.
        /// </summary>
        /// <value>The field namespace.</value>
        public string FieldNamespace
        {
            get
            {
                object id = Request.QueryString["FieldNamespace"];
                if (id == null)
                    return String.Empty;
                else
                    return id.ToString();
            }
        }

		/// <summary>
		/// Gets the app id.
		/// </summary>
		/// <value>The app id.</value>
		public string AppId
		{
			get
			{
				return ManagementHelper.GetAppIdFromQueryString();
			}
		}

        /// <summary>
        /// Gets the save control.
        /// </summary>
        /// <value>The save control.</value>
        public SaveControl SaveControl
        {
            get
            {
                return EditSaveControl;
            }
        }
        #endregion

        #region Page Init methods
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, System.EventArgs e)
		{
            //if permissions are turned off, always provide permission
            bool permissionsEnabled = ProfileConfiguration.Instance.EnablePermissions;

            //If user is an admin, always provide permission
            bool isAdmin = SecurityManager.CheckPermission(new string[] { AppRoles.AdminRole }, false);

            //determine which section the control is being used for and then determine if permissions are present
			if (String.Compare(AppId, _CatalogSystemName, true) == 0)
            {
                if (permissionsEnabled && !isAdmin && !ProfileContext.Current.CheckPermission("catalog:admin:meta:fld:mng:delete"))
                    EditSaveControl.PermissionOverrideHideDeleteButton();
            }
			else if (String.Compare(AppId, _ProfileSystemName, true) == 0)
            {
                if (permissionsEnabled && !isAdmin && !ProfileContext.Current.CheckPermission("profile:admin:meta:fld:mng:delete"))
                    EditSaveControl.PermissionOverrideHideDeleteButton();
            }
			else if (String.Compare(AppId, _OrderSystemName, true) == 0)
            {
                if (permissionsEnabled && !isAdmin && !ProfileContext.Current.CheckPermission("order:admin:meta:fld:mng:delete"))
                    EditSaveControl.PermissionOverrideHideDeleteButton();
            }

			ApplyLocalization();
			if(!this.IsPostBack)
			{
				BindData();
				//DataBind();
			}
			//BindData();
			trError.Visible = false;
            InitUI();
		}

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
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(SaveButton_Click);
            EditSaveControl.Delete += new EventHandler<SaveControl.SaveEventArgs>(DeleteButton_Click);

        }
        #endregion

        /// <summary>
        /// Applies the localization.
        /// </summary>
		private void ApplyLocalization()
		{
            /*
			ToolBarItem1.Caption = RM.GetString("ATTRIBUTEEDIT_TOOLBAR_VIEW_CLASSES");
			ToolBarItem2.Caption = RM.GetString("ATTRIBUTEEDIT_TOOLBAR_NEW_ATTRIBUTE");
			ToolBarItem3.Caption = RM.GetString("ATTRIBUTEEDIT_TOOLBAR_VIEW_ATTRIBUTES");
             * */

			MetaLabelCtrl.Text = RM.GetString("ATTRIBUTEEDIT_NEW_VALUE");
			AllValuesLabel.Text = RM.GetString("ATTRIBUTEEDIT_ALL_VALUES");
			LinkButtonRemove.Text = RM.GetString("ATTRIBUTEEDIT_REMOVE");
			AddValue.Text = RM.GetString("ATTRIBUTEEDIT_ADD_VALUE");
			RequiredFieldValidator1.ErrorMessage = RM.GetString("ATTRIBUTEEDIT_ERROR_EMPTY_NAME");
			//DeleteButton.Text = RM.GetString("ATTRIBUTEEDIT_DELETE_ATTRIBUTE");
			//SaveButton.Text = RM.GetString("GENERAL_SAVEBUTTON");

			chkAllowNulls.Text = RM.GetString("ATTRIBUTEEDIT_FIELDALLOWNULLS");
			chkSaveHistory.Text = RM.GetString("ATTRIBUTEEDIT_FIELDSAVEHISTORY");
			chkAllowSearch.Text = RM.GetString("ATTRIBUTEEDIT_FIELDALLOWSEARCH");
            chkIsEncrypted.Text = RM.GetString("ATTRIBUTEEDIT_FIELDISENCRYPTED");
			chkEditable.Text = RM.GetString("ATTRIBUTEEDIT_EDITABLE");
			chkMultiline.Text = RM.GetString("ATTRIBUTEEDIT_MULTILINE");
			chkClientOption.Text = RM.GetString("ATTRIBUTEEDIT_CLIENTOPTION");
			chkUseInComparing.Text = RM.GetString("ATTRIBUTEEDIT_USE_IN_COMPARING");
            chkMultiLanguage.Text = "Supports Multiple Languages";

            //CustomValidatiorPrecision.ErrorMessage = RM.GetString("ATTRIBUTEEDIT_ERROR_PRECISION_SCALE") + "38";

            chkAutoResizeImage.Text = RM.GetString("ATTRIBUTEEDIT_AUTO_RESIZE_IMAGE");
            chkStretchImage.Text = RM.GetString("ATTRIBUTEEDIT_STRETCH_IMAGE");
            chkAutoGenerateThumbnail.Text = RM.GetString("ATTRIBUTEEDIT_AUTO_CREATE_THUMBNAIL_IMAGE");
            chkStretchThumbnail.Text = RM.GetString("ATTRIBUTEEDIT_STRETCH_THUMBNAIL");

            LinkButtonRemove.Text = RM.GetString("ATTRIBUTEEDIT_REMOVE_VALUE");
        }
        #endregion

        #region Binding Methods
        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
		{
			MetaField field = null;
			if (AttributeId > 0)
				field = MetaField.Load(MDContext, AttributeId);		

			BindTypes();

			if (field != null)
			{
				this.tbName.Text = field.Name;
				this.tbFriendlyName.Text = field.FriendlyName;
				this.tbDescription.Text = field.Description;
				chkAllowNulls.Checked = field.AllowNulls;
				chkAllowSearch.Checked = field.AllowSearch;
				chkIsEncrypted.Checked = field.IsEncrypted;
				chkSaveHistory.Checked = field.SaveHistory;
				chkMultiLanguage.Checked = field.MultiLanguageValue;

				if (CanBeCompared(field.DataType))
				{
					chkUseInComparing.Enabled = true;
					if (field.Attributes["UseInComparing"] != null)
						this.chkUseInComparing.Checked = Boolean.Parse(field.Attributes["UseInComparing"]);
					else
						this.chkUseInComparing.Checked = false;
				}
				else
					chkUseInComparing.Enabled = false;

				if (field.AllowSearch)
				{
					IndexStored.Enabled = true;
					if (field.Attributes["IndexStored"] != null)
						this.IndexStored.Checked = Boolean.Parse(field.Attributes["IndexStored"]);
					else
						this.IndexStored.Checked = false;

					if (field.Attributes["IndexField"] != null)
						ManagementHelper.SelectListItem(IndexOptionsList, field.Attributes["IndexField"]);
				}

				string clientOption = field.Attributes["ClientOption"];
				if (clientOption != null && clientOption.Length > 0)
					chkClientOption.Checked = Boolean.Parse(clientOption);

				ManagementHelper.SelectListItem(ddlType, GetTypeId(field.DataType).ToString());
				this.tbName.Enabled = false;
				this.ddlType.Enabled = false;
				chkAllowNulls.Enabled = false;
				chkMultiLanguage.Enabled = false;
				//chkAllowSearch.Enabled = false;
				chkSaveHistory.Enabled = AllowSaveHistory(field.DataType);
				chkMultiLanguage.Enabled = chkSaveHistory.Enabled;
				InitDataTypeUI(field.DataType);

				string confirm = "return confirm('" + RM.GetString("ATTRIBUTEEDIT_REMOVE_CONFIRMATION") + "')";
				//DeleteButton.Attributes.Add("onclick", confirm);
				// make delete button invisible if attribute cannot be deleted
				EditSaveControl.ShowDeleteButton = field.OwnerMetaClassIdList.Count > 0 ? false : true;

				if (field.IsSystem || field.Namespace.Contains("System"))
				{
					EditSaveControl.ShowDeleteButton = false;
				}

				// Decimal and Numeric properties
				if (field.DataType == MetaDataType.Decimal || field.DataType == MetaDataType.Numeric)
				{
					tbPrecision.Text = ManagementHelper.GetStringValue(field.Attributes["MdpPrecision"], "38");
					tbScale.Text = ManagementHelper.GetStringValue(field.Attributes["MdpScale"], "0");
					tbPrecision.ReadOnly = true;
					tbScale.ReadOnly = true;
				}

				// Image properties
				if (field.DataType == MetaDataType.ImageFile || field.DataType == MetaDataType.Image)
				{
					chkAutoResizeImage.Checked = ManagementHelper.GetBooleanValue(field.Attributes["AutoResize"], false);
					ImageWidth.Text = ManagementHelper.GetStringValue(field.Attributes["ImageWidth"], "");
					ImageHeight.Text = ManagementHelper.GetStringValue(field.Attributes["ImageHeight"], "");
					chkStretchImage.Checked = ManagementHelper.GetBooleanValue(field.Attributes["StretchImage"], false);

					chkAutoGenerateThumbnail.Checked = ManagementHelper.GetBooleanValue(field.Attributes["CreateThumbnail"], false);
					ThumbnailWidth.Text = ManagementHelper.GetStringValue(field.Attributes["ThumbnailWidth"], "");
					ThumbnailHeight.Text = ManagementHelper.GetStringValue(field.Attributes["ThumbnailHeight"], "");
					chkStretchThumbnail.Checked = ManagementHelper.GetBooleanValue(field.Attributes["StretchThumbnail"], false);
				}

				chkIsEncrypted.Enabled = IsEncryptionType(field.DataType);

				if (!IsDictionaryType(field.DataType))
					ManagementHelper.SelectListItem(ddlType, GetTypeId(field.DataType).ToString());
				else
				{
					ManagementHelper.SelectListItem(ddlType, 0);
					//ShowDictionaryRows(true);
					BindDictionary();
				}
			}
			else
			{
				EditSaveControl.ShowDeleteButton = false;
				//ShowDictionaryRows(false);
			}

			chkMultiline.Visible = false;
			chkEditable.Visible = false;
		}

        /// <summary>
        /// Binds the types.
        /// </summary>
		private void BindTypes()
		{
			ddlType.Items.Clear();
			MetaType[] coll = GetMetaTypeList();
			foreach (MetaType type in coll)
			{
				if (!IsDictionaryType(type.MetaDataType))
					ddlType.Items.Add(new ListItem(type.FriendlyName, type.Id.ToString()));
			}
			ddlType.Items.Add(new ListItem(RM.GetString("ATTRIBUTEEDIT_DICTIONARY"), "0"));
		}

        /// <summary>
        /// Binds the dictionary.
        /// </summary>
		private void BindDictionary()
		{
			/*IDataReader dr = Mediachase.Metadata01.DataLevel.MetaMultiValueSet.GetAllItems(AttributeId);
			while(dr.Read())
				DicSingleValueCtrl.Items.Add(new ListItem(dr["Value"].ToString(), dr["ItemId"].ToString()));
			dr.Close();*/
			MetaDictionary dictionary = MetaDictionary.Load(MDContext, AttributeId);
			if(dictionary!=null)
			{
				foreach(MetaDictionaryItem item in dictionary)
					DicSingleValueCtrl.Items.Add(new ListItem(item.Value.ToString(), item.Id.ToString()));
			}
        }
        #endregion

        /// <summary>
        /// Returns true if the type belongs to a dictionary
        /// </summary>
        /// <param name="type">type to be checked</param>
        /// <returns>bool</returns>
        public bool IsDictionaryType(MetaDataType type)
        {
            if (!(type == MetaDataType.DictionaryMultiValue
                || type == MetaDataType.DictionarySingleValue
                || type == MetaDataType.EnumMultiValue
                || type == MetaDataType.EnumSingleValue
                //|| type == MetaDataType.StringDictionary
                || type == MetaDataType.MetaObject))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Returns true if the type is encryption type
        /// </summary>
        /// <param name="type">type to be checked</param>
        /// <returns>bool</returns>
        public bool IsEncryptionType(MetaDataType type)
        {
            if (type == MetaDataType.NVarChar
                || type == MetaDataType.NText
                || type == MetaDataType.ShortString
                || type == MetaDataType.LongString
                || type == MetaDataType.LongHtmlString)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns the meta types supported by the framework
        /// </summary>
        /// <returns></returns>
        public MetaType[] GetMetaTypeList()
        {
            ArrayList retVal = new ArrayList();

            MetaTypeCollection fullList = MetaType.GetMetaTypes(MDContext);

            foreach (MetaType item in fullList)
            {
                // Skip dictionary types, although they are supported, they typically provide major performance hit and
                // are not recommended
                /*
                if (IsDictionaryType(item.MetaDataType))
                    continue;

                if (item.MetaDataType == MetaDataType.StringDictionary)
                    continue;

                if (item.Name.Contains("Dictionary"))
                    continue;
                 * */

                if (!item.IsSqlCommonType ||
                    String.Compare(item.Name, "Boolean", true) == 0 ||
                    String.Compare(item.Name, "Money", true) == 0 ||
                    String.Compare(item.Name, "DateTime", true) == 0 ||
                    String.Compare(item.Name, "Decimal", true) == 0 ||
                    String.Compare(item.Name, "Float", true) == 0)
                    retVal.Add(item);
            }

            return (MetaType[])retVal.ToArray(typeof(MetaType));
        }
		
        #region Private Help methods
        /// <summary>
        /// Gets the type id.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private int GetTypeId(MetaDataType type)
        {
            MetaType[] coll = GetMetaTypeList();
            foreach (MetaType t in coll)
            {
                if (t.MetaDataType == type)
                    return t.Id;
            }
            return -1;
        }

        /// <summary>
        /// Inits the data type UI.
        /// </summary>
        /// <param name="type">The type.</param>
        private void InitDataTypeUI(MetaDataType type)
        {
            DecimalProperties.Visible = type == MetaDataType.Decimal || type == MetaDataType.Numeric;
            ImageProperties.Visible = type == MetaDataType.Image || type == MetaDataType.ImageFile;
            DictionaryValues.Visible = IsDictionaryType(type);
            SearchProperties.Visible = AllowSearch(type);
            chkIsEncrypted.Visible = IsEncryptionType(type);
			chkMultiLanguage.Enabled = type != MetaDataType.File;
        }

        /// <summary>
        /// Inits the UI.
        /// </summary>
        private void InitUI()
        {
            AutoResizeImageProperties.Visible = chkAutoResizeImage.Checked;
            AutoGenerateThumbnailProperties.Visible = chkAutoGenerateThumbnail.Checked;
        }

        /// <summary>
        /// Allows the search.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private bool AllowSearch(MetaDataType type)
        {
            if (
                type == MetaDataType.BigInt ||
                type == MetaDataType.Decimal ||
                type == MetaDataType.Float ||
                type == MetaDataType.Int ||
                type == MetaDataType.Money ||
                type == MetaDataType.Numeric ||
                type == MetaDataType.SmallInt ||
                type == MetaDataType.SmallMoney ||
                type == MetaDataType.TinyInt ||
                type == MetaDataType.LongHtmlString ||
                type == MetaDataType.LongString ||
                type == MetaDataType.NChar ||
                type == MetaDataType.NText ||
                type == MetaDataType.NVarChar ||
                type == MetaDataType.ShortString ||
                type == MetaDataType.Text ||
                type == MetaDataType.VarChar
                )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Allows the save history.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private bool AllowSaveHistory(MetaDataType type)
        {
            switch (type)
            {
                case MetaDataType.Binary:
                case MetaDataType.DictionaryMultiValue:
                case MetaDataType.DictionarySingleValue:
                case MetaDataType.EnumMultiValue:
                case MetaDataType.EnumSingleValue:
                case MetaDataType.StringDictionary:
                case MetaDataType.File:
                case MetaDataType.Image:
                case MetaDataType.ImageFile:
                case MetaDataType.LongHtmlString:
                case MetaDataType.LongString:
                case MetaDataType.MetaObject:
                case MetaDataType.NText:
                case MetaDataType.VarBinary:
                    return false;
                default:
                    return true;
            }
        }
        #endregion

		#region Validation
		/// <summary>
		/// Checks if entered name is unique.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
		public void NameCheck(object sender, ServerValidateEventArgs args)
		{
			// get metafield by name
			MetaField field = MetaField.Load(MDContext, args.Value);

			bool found = false;
			if (field != null && field.Id != AttributeId)
				found = true;

			args.IsValid = !found;
		}
		#endregion

		#region Event Handlers
		/// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void SaveButton_Click(object sender, SaveControl.SaveEventArgs e)
		{
			Page.Validate("Precision");

			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

			MetaField field = CurrentMetaField;
			//if (AttributeId > 0)
			//    field = MetaField.Load(MDContext, AttributeId);

			if (field == null)
			{
				int DataTypeId = int.Parse(ddlType.SelectedItem.Value);
				MetaDataType type;
				if (DataTypeId != 0)
				{
					type = MetaType.Load(MDContext, DataTypeId).MetaDataType;
				}
				else
				{
					if (chkMultiline.Checked && chkEditable.Checked)
						type = MetaDataType.DictionaryMultiValue;
					else if (!chkMultiline.Checked && chkEditable.Checked)
						type = MetaDataType.DictionarySingleValue;
					else if (chkMultiline.Checked && !chkEditable.Checked)
						type = MetaDataType.EnumMultiValue;
					else
						type = MetaDataType.EnumSingleValue;
				}

				try
				{
					field = MetaField.Create(MDContext, FieldNamespace, MetaField.GetUniqueName(MDContext, tbName.Text), tbFriendlyName.Text, tbDescription.Text,
						type, 0, chkAllowNulls.Checked, chkSaveHistory.Checked, chkMultiLanguage.Checked, chkAllowSearch.Checked, chkIsEncrypted.Checked);
				}
				catch (System.Data.SqlClient.SqlException sqlException)
				{
					if (sqlException.Number == 2627 || sqlException.Number == 50000)
					{
						DisplayErrorMessage(RM.GetString("ATTRIBUTEEDIT_ERROR_FIELDNAMEDUPLICTAION"));
						return;
					}
				}
			}
			else
			{
				try
				{
					// MetaData bug:
					// setting value for AllowSearch works only in the beginning of transaction
                    
                    if (field.AllowSearch != chkAllowSearch.Checked)
					    field.AllowSearch = chkAllowSearch.Checked;

                    field.IsEncrypted = chkIsEncrypted.Checked;
					field.FriendlyName = tbFriendlyName.Text;
					field.Description = tbDescription.Text;
					//field.Namespace = field.Namespace;
					field.SaveHistory = chkSaveHistory.Checked;
					field.MultiLanguageValue = chkMultiLanguage.Checked;
				}
				catch (Exception)
				{
					throw;
				}
			}

            // Add dictionary elements if needed
            if (IsDictionaryType(field.DataType))
            {
                foreach (ListItem li in DicSingleValueCtrl.Items)
                {
                    if (li.Value == "-1")
                    {
                        // add new value to the dictionary
                        field.Dictionary.Add(li.Text);
                    }
                }
            }

            // Decimal and numeric properties
            if (field.DataType == MetaDataType.Decimal || field.DataType == MetaDataType.Numeric)
            {
                field.Attributes["MdpPrecision"] = tbPrecision.Text;
                field.Attributes["MdpScale"] = tbScale.Text;
            }

            // Image properties
            if (field.DataType == MetaDataType.ImageFile || field.DataType == MetaDataType.Image)
            {
                field.Attributes["AutoResize"] = chkAutoResizeImage.Checked.ToString();
                field.Attributes["ImageWidth"] = ImageWidth.Text;
                field.Attributes["ImageHeight"] = ImageHeight.Text;
                field.Attributes["StretchImage"] = chkStretchImage.Checked.ToString();

                field.Attributes["CreateThumbnail"] = chkAutoGenerateThumbnail.Checked.ToString();
                field.Attributes["ThumbnailWidth"] = ThumbnailWidth.Text;
                field.Attributes["ThumbnailHeight"] = ThumbnailHeight.Text;
                field.Attributes["StretchThumbnail"] = chkStretchThumbnail.Checked.ToString();
            }

			if (CanBeCompared(field.DataType))
				field.Attributes["UseInComparing"] = chkUseInComparing.Checked.ToString();
			else
				field.Attributes["UseInComparing"] = Boolean.FalseString;

            if (AllowSearch(field.DataType))
            {
                field.Attributes["IndexStored"] = IndexStored.Checked.ToString();
                field.Attributes["IndexField"] = IndexOptionsList.SelectedValue;
            }
        }

        /// <summary>
        /// Determines whether this instance [can be compared] the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can be compared] the specified type; otherwise, <c>false</c>.
        /// </returns>
		private bool CanBeCompared(MetaDataType type)
		{
			if (type != MetaDataType.ImageFile && type != MetaDataType.Image &&
					type != MetaDataType.File && type != MetaDataType.Binary &&
					type != MetaDataType.MetaObject)
				return true;
			else
				return false;
		}

        /// <summary>
        /// Handles the Click event of the DeleteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void DeleteButton_Click(object sender, System.EventArgs e)
        {
            MetaField.Delete(MDContext, AttributeId);
            //Response.Redirect("AttributeClassesHome.aspx");
        }

        /// <summary>
        /// Handles the Click event of the LinkButtonRemove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LinkButtonRemove_Click(object sender, System.EventArgs e)
        {
            System.Web.UI.WebControls.ListItem li = DicSingleValueCtrl.SelectedItem;
            if (li != null)
            {
                DicSingleValueCtrl.Items.Remove(li);
                //if (li.Value != "-1")
                {
                    MetaDictionary dictionary = MetaDictionary.Load(MDContext, AttributeId);
                    dictionary.Delete(int.Parse(li.Value));
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the AddValue control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AddValue_Click(object sender, System.EventArgs e)
		{
			if (String.IsNullOrEmpty(MetaValueCtrl.Text))
				return;

			// add new value to the dictionary
			if (CurrentMetaField != null)
			{
				int newId = CurrentMetaField.Dictionary.Add(MetaValueCtrl.Text);
				DicSingleValueCtrl.Items.Add(new ListItem(MetaValueCtrl.Text, newId.ToString()));
			}
			
			MetaValueCtrl.Text = "";
		}

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddlType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chkSaveHistory.Enabled = true;
			try
			{
				MetaDataType mdType = MetaType.Load(MDContext, int.Parse(ddlType.SelectedValue)).MetaDataType;
				chkSaveHistory.Enabled = AllowSaveHistory(mdType);
                InitDataTypeUI(mdType);

                if (!chkSaveHistory.Enabled)
                {
                    chkSaveHistory.Checked = false;
                    chkMultiLanguage.Checked = false;
                }
			}
			catch
			{
			}
			
			if (ddlType.SelectedItem.Value == "0")
			{
				chkMultiline.Visible = true;
				chkEditable.Visible = true;
				chkAllowNulls.Checked = true;
				chkAllowNulls.Enabled = false;
			}
			else
			{
				chkMultiline.Visible = false;
				chkEditable.Visible = false;
				chkAllowNulls.Enabled = true;
			}
        }

        /// <summary>
        /// Handles the Changed event of the AutoResizeImage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AutoResizeImage_Changed(object sender, System.EventArgs e)
        {
            InitUI();
        }

        /// <summary>
        /// Precisions the validate.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="System.Web.UI.WebControls.ServerValidateEventArgs"/> instance containing the event data.</param>
        protected void PrecisionValidate(object source, ServerValidateEventArgs args)
        {
            int precision = Int32.Parse(tbPrecision.Text);
            int scale = Int32.Parse(tbScale.Text);
            args.IsValid = precision + scale <= 38;
        }
        #endregion
	}
}
