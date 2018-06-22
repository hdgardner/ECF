using System;
using System.Collections;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Profile;
using Mediachase.Commerce.Shared;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Core.MetaData.Admin
{
	/// <summary>
	/// MetaClassesControl.
	/// </summary>
	public partial class MetaClassesControl : CoreBaseUserControl
	{
		private const string _CatalogSystemName = "Catalog";
		private const string _ProfileSystemName = "Profile";
		private const string _OrderSystemName = "Order";
		private const string _CurrentViewName = "MetaClass-List";

		private int _MetaClassId = 0;

		#region Public Properties
		/// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
		public string Namespace
		{
			get
			{
				return ManagementHelper.GetStringValue(Request.QueryString["Namespace"], String.Empty);
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
				return ManagementHelper.GetStringValue(Request.QueryString["FieldNamespace"], String.Empty);
			}
		}

        /// <summary>
        /// Gets the MF view.
        /// </summary>
        /// <value>The MF view.</value>
		public string MFView
		{
			get
			{
				return ManagementHelper.GetStringValue(Request.QueryString["mfview"], String.Empty);
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
        /// Gets the view id.
        /// </summary>
        /// <value>The view id.</value>
		public string ViewId
		{
			get
			{
				return ManagementHelper.GetViewIdFromQueryString();
			}
		}

        /// <summary>
        /// Gets the meta class id.
        /// </summary>
        /// <value>The meta class id.</value>
		public int MetaClassId
		{
			get
			{
				return ManagementHelper.GetIntegerValue(Request.QueryString["id"], _MetaClassId);
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
			if (String.Compare(AppId, _CatalogSystemName, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(ViewId, _CurrentViewName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (permissionsEnabled && !isAdmin && !ProfileContext.Current.CheckPermission("catalog:admin:meta:cls:mng:delete"))
					EditSaveControl.PermissionOverrideHideDeleteButton();
			}
			else if (String.Compare(AppId, _ProfileSystemName, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(ViewId, _CurrentViewName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (permissionsEnabled && !isAdmin && !ProfileContext.Current.CheckPermission("profile:admin:meta:cls:mng:delete"))
					EditSaveControl.PermissionOverrideHideDeleteButton();
			}
			else if (String.Compare(AppId, _OrderSystemName, StringComparison.OrdinalIgnoreCase) == 0 && String.Compare(ViewId, _CurrentViewName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				if (permissionsEnabled && !isAdmin && !ProfileContext.Current.CheckPermission("order:admin:meta:cls:mng:delete"))
					EditSaveControl.PermissionOverrideHideDeleteButton();
			}

			ApplyLocalization();

			// bind top toolbar
			MetaToolbar1.ViewName = this.AppId;
			MetaToolbar1.PlaceName = this.ViewId;
			MetaToolbar1.DataBind();

			tblMetaClass.Visible = false;
			if (!IsPostBack || String.Compare(Request.Form["__EVENTTARGET"], CommandManager.GetCurrent(this.Page).ID, false) == 0)
				BindElement();
		}

        /// <summary>
        /// Applies the localization.
        /// </summary>
		private void ApplyLocalization()
		{
			LblElement.Text = RM.GetString("ATTRIBUTECLASSES_ELEMENT");
			LblType.Text = RM.GetString("ATTRIBUTECLASSES_TYPE");

			if (tblMetaClass != null && tblMetaClass.Visible)
			{
				RequiredFieldValidatorFriendlyName.ErrorMessage = RM.GetString("ATTRIBUTECLASSEDIT_ERROR_EMPTY_FRIENDLYNAME");

				if (ItemsGrid != null)
				{
					ItemsGrid.Columns[0].HeaderText = RM.GetString("ATTRIBUTECLASSES_SELECT");
					ItemsGrid.Columns[1].HeaderText = RM.GetString("ATTRIBUTECLASSES_SORT");
					ItemsGrid.Columns[2].HeaderText = RM.GetString("ATTRIBUTECLASSES_HDR_NAME");
				}
			}
		}

        /// <summary>
        /// Binds the element.
        /// </summary>
		private void BindElement()
		{
			ddlElement.Items.Clear();
			MetaClassCollection coll = MetaClass.GetList(MDContext, Namespace, true);
			foreach (MetaClass mc in coll)
			{
				if (mc.Parent == null)
					if (String.Compare(mc.TableName, "customeraccount", true) != 0 &&
						String.Compare(mc.TableName, "address", true) != 0)
						ddlElement.Items.Add(new ListItem(mc.FriendlyName, mc.Id.ToString()));
			}
			if (ddlElement.Items.Count > 0)
			{
				MetaClass mc = null;
				if (MetaClassId > 0 && !IsPostBack)
					mc = MetaClass.Load(MDContext, MetaClassId);
				if (mc != null)
				{
					ManagementHelper.SelectListItem(ddlElement, mc.Parent.Id.ToString());
				}
				else
					ManagementHelper.SelectListItem(ddlElement, ddlElement.Items[0].Value);
			}

			BindType();
		}

        /// <summary>
        /// Binds the type.
        /// </summary>
		private void BindType()
		{
			LblType.Visible = false;
			ddlType.Visible = false;

			if (ddlElement.Items.Count > 0)
			{
				int SelectedClassId = int.Parse(ddlElement.SelectedValue);
				MetaClass mclass = MetaClass.Load(MDContext, SelectedClassId);
				if (mclass.ChildClasses != null && mclass.ChildClasses.Count > 0)
				{
					LblType.Visible = true;
					ddlType.Visible = true;

					ddlType.Items.Clear();

					MetaClassCollection children = mclass.ChildClasses;
					foreach (MetaClass child in children)
						ddlType.Items.Add(new ListItem(child.FriendlyName, child.Id.ToString()));
				}
				if (ddlType.Items.Count > 0)
				{
					MetaClass mc = null;
					if (MetaClassId > 0 && !IsPostBack)
						mc = MetaClass.Load(MDContext, MetaClassId);
					if (mc != null)
					{
						ManagementHelper.SelectListItem(ddlType, MetaClassId.ToString());
						BindFields(mc);
					}
					else
					{
						ManagementHelper.SelectListItem(ddlType, ddlType.Items[0].Value);
						BindFields(MetaClass.Load(MDContext, int.Parse(ddlType.SelectedValue)));
					}
				}
			}
		}

        /// <summary>
        /// Binds the fields.
        /// </summary>
        /// <param name="mc">The mc.</param>
		private void BindFields(MetaClass mc)
		{
			tblMetaClass.Visible = false;

			if (mc == null)
				return;

			Name.Text = mc.Name;
			Name.Enabled = false;
			FriendlyName.Text = mc.FriendlyName;
			Description.Text = mc.Description;
			BindItemsGrid(mc.Id);

			// Do not allow deleting system meta classes
			if (mc.IsSystem || mc.Namespace.Contains("System"))
				EditSaveControl.ShowDeleteButton = false;
			else
				EditSaveControl.ShowDeleteButton = true;

			if (ddlType.Visible && ddlType.Items.Count > 0)
				tblMetaClass.Visible = true;
		}

        /// <summary>
        /// Binds the items grid.
        /// </summary>
        /// <param name="id">The id.</param>
		private void BindItemsGrid(int id)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("MetaFieldId", typeof(int));
			dt.Columns.Add("FriendlyName", typeof(string));

			// Fields
			MetaFieldCollection mfc = MetaField.GetList(MDContext/*Namespace, true*/); // allow sharing meta fields

			foreach (MetaField field in mfc)
			{
				if (field.IsUser)
				{
					DataRow row = dt.NewRow();
					row["MetaFieldId"] = field.Id;
					row["FriendlyName"] = field.FriendlyName;
					dt.Rows.Add(row);
				}
			}

			ItemsGrid.DataSource = new DataView(dt);
			ItemsGrid.DataBind();

			if (id > 0)
			{
				ArrayList list = new ArrayList();

				MetaFieldCollection mfc2 = MetaField.GetList(MDContext, id);
				foreach (MetaField mf in mfc2)
				{
					if (mf.IsUser)
					{
						for (int i = 0; i < ItemsGrid.Items.Count; i++)
						{
							if ((int)ItemsGrid.DataKeys[i] == mf.Id)
							{
								list.Add(i);
								TextBox box = (TextBox)ItemsGrid.Items[i].FindControl("Weight");
								box.Text = mf.Weight.ToString();
							}
						}
					}
				}

				((Mediachase.Web.Console.Controls.RowSelectorColumn)ItemsGrid.Columns[0]).SelectedIndexes = (int[])list.ToArray(typeof(int));
			}
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(SaveButton_Click);
            EditSaveControl.Delete += new EventHandler<SaveControl.SaveEventArgs>(DeleteButton_Click);
            EditSaveControl.Cancel += new EventHandler<SaveControl.SaveEventArgs>(CancelButton_Click);
		}

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void CancelButton_Click(object sender, System.EventArgs e)
		{
			BindElement();
		}

        /// <summary>
        /// Handles the Click event of the DeleteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void DeleteButton_Click(object sender, System.EventArgs e)
		{
			MetaClass.Delete(MDContext, int.Parse(ddlType.SelectedValue));
			BindElement();
		}

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddlElement_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindType();
		}

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
		{
			MetaClass metaclass = MetaClass.Load(MDContext, int.Parse(ddlType.SelectedValue));
			BindFields(metaclass);
			// TODO: make delete button invisible if class cannot be deleted
		}

        /// <summary>
        /// Handles the Click event of the SaveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void SaveButton_Click(object sender, System.EventArgs e)
		{
			MetaClass mc = MetaClass.Load(MDContext, int.Parse(ddlType.SelectedValue));
			if (mc == null)
				return;

			UpdateAttributes(mc);
			try
			{
				mc.FriendlyName = FriendlyName.Text;
				mc.Description = Description.Text;
			}
			catch (Exception)
			{
			}

			BindElement();
		}

        /// <summary>
        /// Updates the attributes.
        /// </summary>
        /// <param name="mc">The mc.</param>
		private void UpdateAttributes(MetaClass mc)
		{
			for (int i = 0; i < ItemsGrid.Items.Count; i++)
			{
				// Obtain references to row's controls
				HtmlInputCheckBox active = (System.Web.UI.HtmlControls.HtmlInputCheckBox)ItemsGrid.Items[i].Cells[0].Controls[0];
				TextBox weightText = (System.Web.UI.WebControls.TextBox)ItemsGrid.Items[i].FindControl("Weight");

				int weight = 0;
				// Wrap in try/catch block to catch errors in the event that someone types in
				// an invalid value for quantity
				try
				{
					weight = Int32.Parse(weightText.Text);
				}
				catch (System.FormatException)
				{
				}
				catch (System.OverflowException)
				{
				}

				try
				{
					// delete is unchecked
					int fieldId = (int)ItemsGrid.DataKeys[i];
					MetaField field = MetaField.Load(MDContext, fieldId);

					if (active.Checked)
					{
						if (mc.UserMetaFields[field.Name] != null)
							((MetaField)mc.UserMetaFields[field.Name]).Weight = weight;
						else
							mc.AddField(fieldId, weight);
					}
					else
					{
						if (mc.UserMetaFields[field.Name] != null)
							mc.DeleteField((int)ItemsGrid.DataKeys[i]);
					}
				}
				catch (System.FormatException)
				{
				}
				catch (System.OverflowException)
				{
				}
			}
		}
	}
}
