using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Web.Console.Common;

namespace Mediachase.Commerce.Manager.Order.Shipping.Tabs
{
	public partial class ShippingOptionEditPackages : OrderBaseUserControl, IAdminTabControl, IAdminContextControl
	{
		private const string _ShippingMethodDtoString = "ShippingMethodDto";

		private ShippingMethodDto _ShippingMethodDto = null;
        private ShippingMethodDto _PackagesDto = null;

        private ShippingMethodDto GetPackages()
        {
            if (_PackagesDto == null)
                _PackagesDto = ShippingManager.GetShippingPackages();

            return _PackagesDto;
        }

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
				BindForm();
		}

		/// <summary>
		/// Raises the <see cref="E:Init"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			lvMain.ItemCommand += new EventHandler<ListViewCommandEventArgs>(lvMain_ItemCommand);
            lvMain.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvMain_ItemDataBound);
            lvMain.ItemEditing += new EventHandler<ListViewEditEventArgs>(lvMain_ItemEditing);
            lvMain.ItemUpdating += new EventHandler<ListViewUpdateEventArgs>(lvMain_ItemUpdating);
            lvMain.ItemCanceling += new EventHandler<ListViewCancelEventArgs>(lvMain_ItemCanceling);
            lvMain.ItemDeleting += new EventHandler<ListViewDeleteEventArgs>(lvMain_ItemDeleting);
            lvMain.ItemInserting += new EventHandler<ListViewInsertEventArgs>(lvMain_ItemInserting);
            lvMain.ItemCreated += new EventHandler<ListViewItemEventArgs>(lvMain_ItemCreated);

			base.OnInit(e);
		}

        protected void lbAdd_Click(object sender, EventArgs e)
        {
            lvMain.EditIndex = -1;
            lvMain.InsertItemPosition = InsertItemPosition.LastItem;

            ((LinkButton)sender).Visible = false;
			BindForm();
        }

        #region Grid Events
        void lvMain_ItemInserting(object sender, ListViewInsertEventArgs e)
        {
            //lvMain.InsertItemPosition = InsertItemPosition.None;
        }

        void lvMain_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            // get Id being updated
            int id = Int32.Parse(lvMain.DataKeys[lvMain.EditIndex].Value.ToString());

            // get the current item being edited
            ListViewItem item = lvMain.Items[lvMain.EditIndex];

            if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingPackage.Count > 0)
            {
                ShippingMethodDto.ShippingPackageRow row = _ShippingMethodDto.ShippingPackage.FindByShippingPackageId(id);

                DropDownList ddl = (DropDownList)item.FindControl("PackagesList");
                if (ddl != null && ddl.Items.Count > 0)
                    row.PackageId = Int32.Parse(ddl.SelectedValue);

                TextBox tbName = item.FindControl("tbPackageName") as TextBox;
                if (tbName != null)
                    row.PackageName = tbName.Text;
            }

            // exit the edit mode
            lvMain.EditIndex = -1;

            // bind the listview
            BindForm();
        }

        void lvMain_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            // get the current item being deleted
			int id = Int32.Parse(lvMain.DataKeys[e.ItemIndex].Value.ToString());

            // delete shippingpackage with ShippingPackageId==id
			ShippingMethodDto.ShippingPackageRow shPackageRow = _ShippingMethodDto.ShippingPackage.FindByShippingPackageId(id);
			if (shPackageRow != null)
				shPackageRow.Delete();
            
            BindForm();
        }

        void lvMain_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            if (e.CancelMode == ListViewCancelMode.CancelingEdit)
            {
                lvMain.EditIndex = -1;
                BindForm();
            }
			else if (e.CancelMode == ListViewCancelMode.CancelingInsert)
			{
				CancelInsertMode();
			}
        }

        void lvMain_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            // set the ListView to Edit mode
            lvMain.EditIndex = e.NewEditIndex;

            int id = Int32.Parse(lvMain.DataKeys[lvMain.EditIndex].Value.ToString());

            ShippingMethodDto.ShippingPackageRow shPackageRow = _ShippingMethodDto.ShippingPackage.FindByShippingPackageId(id);
            if (shPackageRow != null)
            {
                // select package in package dropdown
                DropDownList ddl = (DropDownList)lvMain.Items[e.NewEditIndex].FindControl("PackageList");
                if (ddl != null)
                    ManagementHelper.SelectListItem(ddl, shPackageRow.PackageId);
            }

            // bind the ListView
            BindForm();
        }

        void lvMain_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (String.Compare(e.CommandName, "Save", true) == 0)
                AddPackageItem(e.Item);
        }        

		void lvMain_ItemCreated(object sender, ListViewItemEventArgs e)
		{
			if ((e.Item.ItemType == ListViewItemType.DataItem && lvMain.EditIndex != -1) ||
				e.Item.ItemType == ListViewItemType.InsertItem)
			{
				DropDownList list = e.Item.FindControl("PackagesList") as DropDownList;
				if (list != null)
				{
					list.DataSource = GetPackagesList();
					list.DataBind();
				}
			}
		}

		void lvMain_ItemDataBound(object sender, ListViewItemEventArgs e)
		{
            if (e.Item.ItemType == ListViewItemType.DataItem && lvMain.EditIndex != -1)
            {
                int id = Int32.Parse(lvMain.DataKeys[lvMain.EditIndex].Value.ToString());

                ShippingMethodDto.ShippingPackageRow shPackageRow = _ShippingMethodDto.ShippingPackage.FindByShippingPackageId(id);
                if (shPackageRow != null)
                {
                    // select package in packages DropDownList
                    DropDownList ddl = (DropDownList)e.Item.FindControl("PackagesList");
                    if (ddl != null)
                        ManagementHelper.SelectListItem(ddl, shPackageRow.PackageId.ToString());
                }
            }
        }
        #endregion

		protected void CancelEditMode()
		{
			lvMain.EditIndex = -1;
			BindForm();
			lbAdd.Visible = true;
			updatePanel.Update();
		}

        protected void CancelInsertMode()
        {
            lvMain.InsertItemPosition = InsertItemPosition.None;
			lbAdd.Visible = true;
            BindForm();
            updatePanel.Update();
        }

		/// <summary>
		/// Binds the form.
		/// </summary>
		private void BindForm()
		{
			if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingOption.Count > 0)
			{
				try
				{
					ShippingMethodDto.ShippingOptionRow shippingRow = _ShippingMethodDto.ShippingOption[0];

					lvMain.DataSource = _ShippingMethodDto.ShippingPackage.DefaultView;
					lvMain.DataBind();
				}
				catch (Exception ex)
				{
					DisplayErrorMessage("Error during binding form: " + ex.Message);
				}
			}
			else
			{
				// set default form values
			}
		}

		protected DataView GetPackagesList()
		{
			ShippingMethodDto dto = GetPackages();
			return dto.Package.DefaultView;
		}

        protected string GetPackageNameById(int packageId)
        {
            string name = String.Empty;

            ShippingMethodDto dto = GetPackages();

            if (dto != null)
            {
                ShippingMethodDto.PackageRow packageRow = dto.Package.FindByPackageId(packageId);
                name = packageRow.Name;
            }

            return name;
        }

        void AddPackageItem(ListViewItem item)
        {
            DropDownList ddl = (DropDownList)item.FindControl("PackagesList");
            if (ddl != null && ddl.Items.Count > 0)
            {
                if (_ShippingMethodDto != null && _ShippingMethodDto.ShippingOption.Count > 0)
                {
                    ShippingMethodDto.ShippingPackageRow row = _ShippingMethodDto.ShippingPackage.NewShippingPackageRow();
                    row.ShippingOptionId = _ShippingMethodDto.ShippingOption[0].ShippingOptionId;
                    row.PackageId = Int32.Parse(ddl.SelectedValue);

                    TextBox tbName = item.FindControl("tbPackageName") as TextBox;
                    if (tbName != null)
                        row.PackageName = tbName.Text;

                    if (row.RowState == DataRowState.Detached)
                        _ShippingMethodDto.ShippingPackage.Rows.Add(row);
                }
            }

            // hide the text boxes
            CancelInsertMode();

            // bind the ListView
            BindForm();
        }

		#region IAdminContextControl Members
		public void LoadContext(IDictionary context)
		{
			_ShippingMethodDto = (ShippingMethodDto)context[_ShippingMethodDtoString];
		}
		#endregion

		#region IAdminTabControl Members
		public void SaveChanges(IDictionary context)
		{
			ShippingMethodDto dto = (ShippingMethodDto)context[_ShippingMethodDtoString];

            if (dto == null || dto.ShippingOption == null || dto.ShippingOption.Count == 0)
                // dto must be created in base Shipping control that holds tabs
                return;

			//ShippingMethodDto.ShippingOptionRow row = dto.ShippingOption[0];

			//// save grid updates
			//foreach (ListViewDataItem item in lvMain.Items)
			//{
			//    //lvMain.UpdateItem(i.DataItemIndex, true);
			//}
		}
		#endregion
	}
}