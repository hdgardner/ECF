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
using Mediachase.Commerce.Manager;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using System.IO;
using Mediachase.Commerce.Catalog.Data;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;

public partial class Apps_Catalog_GridTemplates_ItemControlTemplate : System.Web.UI.UserControl, IEcfListViewTemplate, IBatchUpdateControl
{
	private object _DataItem;
    private bool _IsMetaField = false;
    private string _FieldName = String.Empty;
    private string _LanguageCode = "en-us";
    private int _CatalogNodeId = 0;

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
    }

    public override void DataBind()
	{
        if (!String.IsNullOrEmpty(FieldName))
		    BindData();

		base.DataBind();
	}

    /// <summary>
    /// Binds the batch control.
    /// </summary>
	private void BindData()
    {
        CatalogEntryDto.VariationRow[] variationRows = null;

        CatalogEntryDto.CatalogEntryRow item = (CatalogEntryDto.CatalogEntryRow)DataItem;

        if (IsMetaField)
        {
            int MetaClassId = item.MetaClassId;
            int ObjectId = item.CatalogEntryId;
            MetaDataContext MDContext = CatalogContext.MetaDataContext;

            MetaControls.EnableViewState = false;
            if (MetaControls.Controls.Count > 0)
                return;

            MetaControls.Controls.Clear();

            MetaClass mc = MetaClass.Load(MDContext, MetaClassId);

            if (mc == null)
                return;

            MDContext.UseCurrentUICulture = false;
            MDContext.Language = LanguageCode;

            MetaObject metaObj = null;
            if (ObjectId > 0)
            {
                metaObj = MetaObject.Load(MDContext, ObjectId, mc);
                if (metaObj == null)
                {
                    metaObj = MetaObject.NewObject(MDContext, ObjectId, MetaClassId, FrameworkContext.Current.Profile.UserName);
                    metaObj.AcceptChanges(MDContext);
                }
            }
            MDContext.UseCurrentUICulture = true;


            MetaField mf = MetaField.Load(MDContext, FieldName);
            if (mf.IsUser)
            {
                string controlName = ResolveMetaControl(mc, mf);
                Control ctrl = MetaControls.FindControl(mf.Name);

                if (ctrl == null)
                {
                    ctrl = Page.LoadControl(controlName);
                    MetaControls.Controls.Add(ctrl);
                }


                CoreBaseUserControl coreCtrl = ctrl as CoreBaseUserControl;
                if (coreCtrl != null)
                    coreCtrl.MDContext = MDContext;

                //ctrl.ID = String.Format("{0}-{1}", mf.Name, index.ToString());

                ((IMetaControl)ctrl).MetaField = mf;
                if (metaObj != null && metaObj[mf.Name] != null)
                    ((IMetaControl)ctrl).MetaObject = metaObj;

                ((IMetaControl)ctrl).LanguageCode = LanguageCode;
                ((IMetaControl)ctrl).ValidationGroup = String.Empty;

                ctrl.DataBind();

            }
        }
        else
        {
            switch (FieldName)
            {
                case "Name":
                case "Code":
                    tbItem.Visible = true;
                    tbItem.Text = (string)item[FieldName];
                    break;
                case "StartDate":
                case "EndDate":
                    cdpItem.Visible = true;
                    cdpItem.Value = (DateTime)item[FieldName];
                    break;
                case "TemplateName":
                    ddlItem.Visible = true;
                    TemplateDto templates = DictionaryManager.GetTemplateDto();
                    if (templates.main_Templates.Count > 0)
                    {
                        DataView view = templates.main_Templates.DefaultView;
                        view.RowFilter = "TemplateType = 'entry'";
                        ddlItem.DataTextField = "FriendlyName";
                        ddlItem.DataValueField = "Name";
                        ddlItem.DataSource = view;
                        ddlItem.DataBind();
                    }
                    ManagementHelper.SelectListItem2(ddlItem, item.TemplateName);
                    break;
                case "SortOrder":
                    tbItem.Visible = true;
                    CatalogRelationDto relationDto = CatalogContext.Current.GetCatalogRelationDto(item.CatalogId, CatalogNodeId, item.CatalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));
                    if(relationDto.NodeEntryRelation.Count > 0)
                        tbItem.Text = relationDto.NodeEntryRelation[0].SortOrder.ToString();
                    break;
                case "IsActive":
                    becItem.Visible = true;
                    becItem.IsSelected = item.IsActive;
                    break;
                case "ListPrice":
                    tbItem.Visible = true;
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        tbItem.Text = variationRows[0].ListPrice.ToString("#0.00");
                    }
                    break;
                case "TaxCategoryId":
                    ddlItem.Visible = true;
                    ddlItem.Items.Add(new ListItem(Resources.CatalogStrings.Entry_Select_Tax_Category, "0"));
                    CatalogTaxDto taxes = CatalogTaxManager.GetTaxCategories();
                    if (taxes.TaxCategory != null)
                    {
                        foreach (CatalogTaxDto.TaxCategoryRow row in taxes.TaxCategory.Rows)
                            ddlItem.Items.Add(new ListItem(row.Name, row.TaxCategoryId.ToString()));
                    }
                    ddlItem.DataBind();
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        ManagementHelper.SelectListItem2(ddlItem, variationRows[0].TaxCategoryId);
                    }
                    break;
                case "TrackInventory":
                    becItem.Visible = true;
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        becItem.IsSelected = variationRows[0].TrackInventory;
                    }
                    break;
                case "MerchantId":
                    ddlItem.Visible = true;
                    ddlItem.Items.Insert(0, new ListItem(Resources.CatalogStrings.Entry_Select_Merchant, "")); 
                    CatalogEntryDto merchants = CatalogContext.Current.GetMerchantsDto();
                    if (merchants.Merchant != null)
                    {
                        foreach (CatalogEntryDto.MerchantRow row in merchants.Merchant.Rows)
                            ddlItem.Items.Add(new ListItem(row.Name, row.MerchantId.ToString()));
                    }
                    ddlItem.DataBind();
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0 && !variationRows[0].IsMerchantIdNull())
                    {
                        ManagementHelper.SelectListItem2(ddlItem, variationRows[0].MerchantId);
                    }
                    break;
                case "WarehouseId":
                    ddlItem.Visible = true;
                    ddlItem.Items.Insert(0, new ListItem(Resources.CatalogStrings.Entry_Select_Warehouse, "0"));
                    WarehouseDto warehouses = WarehouseManager.GetWarehouseDto();
                    if (warehouses.Warehouse != null)
                    {
                        foreach (WarehouseDto.WarehouseRow row in warehouses.Warehouse.Rows)
                            ddlItem.Items.Add(new ListItem(row.Name, row.WarehouseId.ToString()));
                    }
                    ddlItem.DataBind();
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        ManagementHelper.SelectListItem2(ddlItem, variationRows[0].WarehouseId);
                    }
                    break;
                case "PackageId":
                    ddlItem.Visible = true;
                    ddlItem.Items.Insert(0, new ListItem(Resources.CatalogStrings.Entry_Select_Package, "0")); 
                    ShippingMethodDto shippingDto = ShippingManager.GetShippingPackages();
                    if (shippingDto.Package != null)
                    {
                        foreach (ShippingMethodDto.PackageRow row in shippingDto.Package.Rows)
                            ddlItem.Items.Add(new ListItem(row.Name, row.PackageId.ToString()));
                    }
                    ddlItem.DataBind();
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        ManagementHelper.SelectListItem2(ddlItem, variationRows[0].PackageId);
                    }
                    break;
                case "Weight":
                case "MinQuantity":
                case "MaxQuantity":
                    tbItem.Visible = true;
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0 && variationRows[0][FieldName] != DBNull.Value)
                    {
                        tbItem.Text = variationRows[0][FieldName].ToString();
                    }
                    break;
                case "InStockQuantity":
                case "ReservedQuantity":
                case "ReorderMinQuantity":
                case "PreorderQuantity":
                case "BackorderQuantity":
                    tbItem.Visible = true;
                    if (item.InventoryRow != null && item.InventoryRow[FieldName] != DBNull.Value)
                    {
                        tbItem.Text = item.InventoryRow[FieldName].ToString();
                    }
                    break;
                case "AllowBackorder":
                case "AllowPreorder":
                    becItem.Visible = true;
                    if (item.InventoryRow != null)
                    {
                        becItem.IsSelected = (bool)item.InventoryRow[FieldName];
                    }
                    break;
                case "InventoryStatus":
                    ddlItem.Visible = true;
                    ddlItem.Items.Insert(0, new ListItem(Resources.SharedStrings.Disabled, "0"));
                    ddlItem.Items.Insert(0, new ListItem(Resources.SharedStrings.Enabled, "1"));
                    ddlItem.Items.Insert(0, new ListItem(Resources.SharedStrings.Ignored, "2"));
                    ddlItem.DataBind();
                    if (item.InventoryRow != null)
                    {
                        ManagementHelper.SelectListItem2(ddlItem, item.InventoryRow.InventoryStatus);
                    }
                    break;
                case "PreorderAvailabilityDate":
                case "BackorderAvailabilityDate":
                    cdpItem.Visible = true;
                    if (item.InventoryRow != null)
                    {
                        cdpItem.Value = (DateTime)item.InventoryRow[FieldName];
                    }
                    break;
            }
        }
	}

    /// <summary>
    /// Resolves the meta control.
    /// </summary>
    /// <param name="metaClass">The meta class.</param>
    /// <param name="metaField">The meta field.</param>
    /// <returns></returns>
    private string ResolveMetaControl(MetaClass metaClass, MetaField metaField)
    {
        string basePath = "~/Apps/Core/MetaData/Controls/";
        string controlName = GetControlNameForMetaType(metaField.DataType);

        string fullPath = String.Format("{0}{1}.{2}.{3}.ascx", basePath, metaClass.Name, metaField.Name, controlName);

        if (File.Exists(Server.MapPath(fullPath)))
            return fullPath;

        fullPath = String.Format("{0}{1}.{2}.ascx", basePath, metaField.Name, controlName);

        if (File.Exists(Server.MapPath(fullPath)))
            return fullPath;


        fullPath = String.Format("{0}{1}.{2}.ascx", basePath, metaClass.Name, controlName);

        if (File.Exists(Server.MapPath(fullPath)))
            return fullPath;

        return String.Format("{0}{1}.ascx", basePath, controlName);
    }

    /// <summary>
    /// Returns path to the control for specific meta data type
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public string GetControlNameForMetaType(MetaDataType type)
    {
        switch (type)
        {
            case MetaDataType.File:
                return "FileControl";
            case MetaDataType.ImageFile:
                return "ImageFileControl";
            case MetaDataType.DateTime:
                return "DateTimeMetaControl";
            case MetaDataType.Money:
                return "MoneyControl";
            case MetaDataType.Float:
                return "FloatControl";
            case MetaDataType.Decimal:
                return "DecimalControl";
            case MetaDataType.Int:
            case MetaDataType.Integer:
                return "IntegerControl";
            case MetaDataType.Boolean:
                return "BooleanControl";
            case MetaDataType.Date:
                return "DateTimeMetaControl";
            case MetaDataType.Email:
                return "EmailControl";
            case MetaDataType.URL:
                return "URLControl";
            case MetaDataType.ShortString:
                return "ShortStringControl";
            case MetaDataType.LongString:
                return "LongStringControl";
            case MetaDataType.LongHtmlString:
                return "LongHTMLStringControl";
            case MetaDataType.DictionarySingleValue:
            case MetaDataType.EnumSingleValue:
                return "DicSingleValueControl";
            case MetaDataType.DictionaryMultiValue:
            case MetaDataType.EnumMultiValue:
                return "MultiValueControl";
            case MetaDataType.StringDictionary:
                return "StringDictionaryControl";
            default:
                return "";
        }
    }

    /// <summary>
    /// Updates static field
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <returns></returns>
    private void UpdateStaticField(CatalogEntryDto.CatalogEntryRow item)
    {
        CatalogEntryDto dto = new CatalogEntryDto();

        CatalogEntryDto.VariationRow[] variationRows = null;

        decimal decimalValue = 0;
        int intValue = 0;
        double doubleValue = 0;

        switch (FieldName)
        {
            case "Name":
                item.Name = tbItem.Text;
                dto.CatalogEntry.ImportRow(item);
                break;
            case "StartDate":
                item.StartDate = cdpItem.Value;
                dto.CatalogEntry.ImportRow(item);
                break;
            case "EndDate":
                item.EndDate = cdpItem.Value;
                dto.CatalogEntry.ImportRow(item);
                break;
            case "TemplateName":
                item.TemplateName = ddlItem.SelectedValue;
                dto.CatalogEntry.ImportRow(item);
                break;
            case "Code":
                if (item.InventoryRow != null)
                {
                    CatalogEntryDto.InventoryRow inventoryRow = item.InventoryRow;
                    inventoryRow.SkuId = tbItem.Text;
                    dto.Inventory.ImportRow(inventoryRow);
                }
                item.Code = tbItem.Text;
                dto.CatalogEntry.ImportRow(item);
                break;
            case "SortOrder":
                intValue = -1;
                if(Int32.TryParse(tbItem.Text, out intValue))
                {
                    CatalogRelationDto relationDto = CatalogContext.Current.GetCatalogRelationDto(item.CatalogId, CatalogNodeId, item.CatalogEntryId, String.Empty, new CatalogRelationResponseGroup(CatalogRelationResponseGroup.ResponseGroup.NodeEntry));
                    // Update relations
                    foreach (CatalogRelationDto.NodeEntryRelationRow row in relationDto.NodeEntryRelation)
                    {
                        row.SortOrder = intValue;
                    }
                    if(relationDto.HasChanges())
                        CatalogContext.Current.SaveCatalogRelationDto(relationDto);
                }
                break;
            case "IsActive":
                item.IsActive = becItem.IsSelected;
                dto.CatalogEntry.ImportRow(item);
                break;
            case "ListPrice":
                decimalValue = 0;
                if (Decimal.TryParse(tbItem.Text, out decimalValue))
                {
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        variationRows[0].ListPrice = decimalValue;
                        dto.CatalogEntry.ImportRow(item);
                        dto.Variation.ImportRow(variationRows[0]);
                    }
                }
                break;
            case "TrackInventory":
                variationRows = item.GetVariationRows();
                if (variationRows.Length > 0)
                {
                    variationRows[0].TrackInventory = becItem.IsSelected;
                    dto.CatalogEntry.ImportRow(item);
                    dto.Variation.ImportRow(variationRows[0]);
                }
                break;
            case "MerchantId":
                if (!String.IsNullOrEmpty(ddlItem.SelectedValue))
                {
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        variationRows[0].MerchantId = new Guid(ddlItem.SelectedValue);
                        dto.CatalogEntry.ImportRow(item);
                        dto.Variation.ImportRow(variationRows[0]);
                    }
                }
                break;
            case "Weight":
                doubleValue = 0;
                if (Double.TryParse(tbItem.Text, out doubleValue))
                {
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        variationRows[0].Weight = doubleValue;
                        dto.CatalogEntry.ImportRow(item);
                        dto.Variation.ImportRow(variationRows[0]);
                    }
                }
                break;
            case "TaxCategoryId":
            case "WarehouseId":
            case "PackageId":
                if (!String.IsNullOrEmpty(ddlItem.SelectedValue))
                {
                    intValue = 0;
                    if (Int32.TryParse(ddlItem.SelectedValue, out intValue))
                    {
                        variationRows = item.GetVariationRows();
                        if (variationRows.Length > 0)
                        {
                            variationRows[0][FieldName] = intValue;
                            dto.CatalogEntry.ImportRow(item);
                            dto.Variation.ImportRow(variationRows[0]);
                        }
                    }
                }
                break;
            case "MinQuantity":
            case "MaxQuantity":
                decimalValue = 1;
                if (Decimal.TryParse(tbItem.Text, out decimalValue))
                {
                    variationRows = item.GetVariationRows();
                    if (variationRows.Length > 0)
                    {
                        variationRows[0][FieldName] = decimalValue;
                        dto.CatalogEntry.ImportRow(item);
                        dto.Variation.ImportRow(variationRows[0]);
                    }
                }
                break;
            case "InStockQuantity":
            case "ReservedQuantity":
            case "ReorderMinQuantity":
            case "PreorderQuantity":
            case "BackorderQuantity":
                decimalValue = 0;
                if (Decimal.TryParse(tbItem.Text, out decimalValue))
                {
                    if (item.InventoryRow != null)
                    {
                        item.InventoryRow[FieldName] = decimalValue;
                        dto.CatalogEntry.ImportRow(item);
                        dto.Inventory.ImportRow(item.InventoryRow);
                    }
                }
                break;
            case "AllowBackorder":
            case "AllowPreorder":
                if (item.InventoryRow != null)
                {
                    item.InventoryRow[FieldName] = becItem.IsSelected;
                    dto.CatalogEntry.ImportRow(item);
                    dto.Inventory.ImportRow(item.InventoryRow);
                }
                break;
            case "InventoryStatus":
                if (!String.IsNullOrEmpty(ddlItem.SelectedValue))
                {
                    intValue = 0;
                    if (Int32.TryParse(ddlItem.SelectedValue, out intValue))
                    {
                        if (item.InventoryRow != null)
                        {
                            item.InventoryRow[FieldName] = intValue;
                            dto.CatalogEntry.ImportRow(item);
                            dto.Inventory.ImportRow(item.InventoryRow);
                        }
                    }
                }
                break;
            case "PreorderAvailabilityDate":
            case "BackorderAvailabilityDate":
                if (item.InventoryRow != null)
                {
                    item.InventoryRow[FieldName] = cdpItem.Value;
                    dto.CatalogEntry.ImportRow(item);
                    dto.Inventory.ImportRow(item.InventoryRow);
                }
                break;
        }   

        if(dto.HasChanges())
            CatalogContext.Current.SaveCatalogEntry(dto);
    }

    /// <summary>
    /// Updates meta field
    /// </summary>
    /// <param name="item">The data item.</param>
    /// <returns></returns>
    private void UpdateMetaField(CatalogEntryDto.CatalogEntryRow item)
    {
        int MetaClassId = item.MetaClassId;
        int ObjectId = item.CatalogEntryId;
        MetaDataContext MDContext = CatalogContext.MetaDataContext;

        if (ObjectId != 0)
        {
            // set username here, because calling FrameworkContext.Current.Profile causes MeteDataContext.Current to change (it's bug in ProfileContext class).
            string userName = FrameworkContext.Current.Profile.UserName;

            MDContext.UseCurrentUICulture = false;

            MDContext.Language = LanguageCode;

            MetaObject metaObj = null;
            bool saveChanges = true;

            metaObj = MetaObject.Load(MDContext, ObjectId, MetaClassId);

            if (metaObj == null)
            {
                metaObj = MetaObject.NewObject(MDContext, ObjectId, MetaClassId, userName);
                //DataBind(); return;
            }
            else
            {
                metaObj.ModifierId = userName;
                metaObj.Modified = DateTime.UtcNow;
            }

            foreach (Control ctrl in MetaControls.Controls)
            {
                // Only update controls that belong to current language
                if (String.Compare(((IMetaControl)ctrl).LanguageCode, LanguageCode, true) == 0)
                {
                    ((IMetaControl)ctrl).MetaObject = metaObj;
                    //((IMetaControl)ctrl).MetaField = metaObj;
                    ((IMetaControl)ctrl).Update();
                }
            }

            // Only save changes when new object has been created
            if (saveChanges)
            {
                metaObj.AcceptChanges(MDContext);
            }

            MDContext.UseCurrentUICulture = true;
        }
    }

	#region IEcfListViewTemplate Members

    /// <summary>
    /// Gets or sets the data item.
    /// </summary>
    /// <value>The data item.</value>
	public object DataItem
	{
		get
		{
			return _DataItem;
		}
		set
		{
			_DataItem = value;
		}
	}

	#endregion


    #region IBatchUpdateControl Members

    public bool IsMetaField
    {
        get
        {
            return _IsMetaField;
        }
        set
        {
            _IsMetaField = value;
        }
    }

    public string FieldName
    {
        get
        {
            return _FieldName;
        }
        set
        {
            _FieldName = value;
        }
    }

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

    public int CatalogNodeId
    {
        get
        {
            return _CatalogNodeId;
        }
        set
        {
            _CatalogNodeId = value;
        }
    }

    public void Update()
    {
        CatalogEntryDto.CatalogEntryRow item = (CatalogEntryDto.CatalogEntryRow)DataItem;
        if (item != null)
        {
            if (IsMetaField)
                UpdateMetaField(item);
            else
                UpdateStaticField(item);
        }
    }

    #endregion
}