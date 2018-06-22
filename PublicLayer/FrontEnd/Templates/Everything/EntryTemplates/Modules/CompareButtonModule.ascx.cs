using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms;
using Mediachase.Cms.Util;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.WebUtility.UI;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Shared;
using Mediachase.MetaDataPlus.Configurator;

/// <summary>
/// Contains all the functionality for compare button module
/// </summary>
public partial class Templates_Everything_Entry_Modules_CompareButtonModule : BaseStoreUserControl, ICallbackEventHandler
{
	protected string _returnValue;

    private Entry _Product = null;

    /// <summary>
    /// Gets the product id.
    /// </summary>
    /// <value>The product id.</value>
    public string ProductId
    {
        get
        {
            if (Product != null)
                return Product.ID;
            else
                return "";
        }
    }

    /// <summary>
    /// Gets or sets the product.
    /// </summary>
    /// <value>The product.</value>
    public Entry Product
    {
        get
        {
            return _Product;
        }
        set
        {
            _Product = value;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, System.EventArgs e)
    {
        Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "CompareProducts_js", CommerceHelper.GetAbsolutePath("/Scripts/CompareProducts.js"));
        Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "CompareViewPageUrl", String.Format("CSCompareProducts.CompareViewPageUrl = \"{0}\";", CMSContext.Current.ResolveUrl("~/compare.aspx")), true);

        if (Product != null)
        {
            string cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "CSCompareProducts.ReceiveServerData", "context");
            String callbackScript = "function CallServer" + Product.CatalogEntryId + "(arg, context) { " + cbReference + ";}";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), this.UniqueID, callbackScript, true);
        }
    }

    /// <summary>
    /// Gets the control from collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="coll">The coll.</param>
    /// <param name="controlId">The control id.</param>
    /// <returns></returns>
    public static T GetControlFromCollection<T>(ControlCollection coll, string controlId) where T : Control
    {
        T retVal = null;
        if (coll != null && coll.Count > 0 && !String.IsNullOrEmpty(controlId))
        {
            foreach (Control ctrl in coll)
            {
                if ((ctrl is T) && String.Compare(ctrl.ID, controlId, false) == 0)
                {
                    retVal = (T)ctrl;
                    break;
                }
                else
                {
                    retVal = GetControlFromCollection<T>(ctrl.Controls, controlId);
                    if (retVal != null)
                        break;

                }
            }
        }
        return retVal;
    }

    /// <summary>
    /// Updates the item for comparing.
    /// </summary>
    /// <param name="itemId">The item id.</param>
    /// <param name="mcId">Id of the product's metaclass.</param>
    /// <returns></returns>
    //[AjaxPro.AjaxMethod]
    public string[] ToggleCompare(string itemId, string mcId)
    {
        string[] results = new string[2];
        // mcId is needed to add only products from the same meta class
		//bool added2Compare = CommonHelper.GetCompareProductsIds().Contains(Int32.Parse(itemId));
		//if (added2Compare)
		//{
		//    int amount = CommonHelper.RemoveProductFromCookie(itemId);
		//    results[0] = amount.ToString();
		//    results[1] = "-1";
		//}
		//else
		//{
		//    int res = CommonHelper.SetCompareCookie(itemId, mcId);
		//    if (res <= -100)
		//        results[1] = res.ToString();
		//    else
		//        results[1] = "0";
		//    results[0] = res.ToString();
		//}

        return results;
    }

    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content.</param>
    protected override void Render(HtmlTextWriter writer)
    {
        bool render = Product != null;
        if (render)
        {
            RenderStartInLine(writer);
        }
        base.Render(writer);
        if (render)
        {
            RenderEndInLine(writer);
        }
    }

    /// <summary>
    /// Renders the start in line.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public void RenderStartInLine(HtmlTextWriter writer)
    {
		if (Product != null)
		{	
			MetaClass mc = Product.GetEntryMetaClass();
			if (mc == null)
			{
				ErrorManager.GenerateError("Could not load Product's MetaClass");
				return;
			}

			bool added2Compare = CommonHelper.GetCompareProductsIds(mc.Name).Contains(Product.CatalogEntryId);
            HtmlInputCheckBox chkbox = new HtmlInputCheckBox();
            chkbox.Attributes.Add("onclick", String.Format("javascript:CallServer{0}(this.checked);", Product.CatalogEntryId));
            chkbox.ID = GetCompareButtonId();
            if (added2Compare)
                chkbox.Checked = true;
            phCompareCheckbox.Controls.Add(chkbox);

            HtmlGenericControl span = new HtmlGenericControl("span");
            span.InnerHtml = "&nbsp;";
            phCompareCheckbox.Controls.Add(span);

            HtmlAnchor lnkCompare = new HtmlAnchor();
            lnkCompare.HRef = String.Format("javascript:CSCompareProducts.OpenCompareView('{0}');", mc.Name); 
            lnkCompare.InnerHtml = RM.GetString("COMPAREBUTTONMODULE_COMPARE_PRODUCTS");
            phCompareCheckbox.Controls.Add(lnkCompare);
		}
		else
		{
			this.Visible = false;
		}
    }


    /// <summary>
    /// Renders the end in line.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public void RenderEndInLine(HtmlTextWriter writer)
    {
    }

	#region ICallbackEventHandler Members
    /// <summary>
    /// Returns the results of a callback event that targets a control.
    /// </summary>
    /// <returns>The result of the callback.</returns>
	public string GetCallbackResult()
	{
		return _returnValue;
	}

    /// <summary>
    /// Processes a callback event that targets a control.
    /// </summary>
    /// <param name="eventArgument">A string that represents an event argument to pass to the event handler.</param>
	public void RaiseCallbackEvent(string eventArgument)
	{
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //List<string> args = serializer.Deserialize<List<string>>(eventArgument);

		_returnValue = String.Empty;
        
		CompareButtonClickResult cmpResult = new CompareButtonClickResult();
		cmpResult.chbId = GetCompareButtonId();
        string refId = GetRefreshObjectId();
		if (Product != null)
		{
			// add/remove product to/from cookie
			bool isChecked = false;
			if (bool.TryParse(eventArgument, out isChecked))
			{
				MetaClass mc = Product.GetEntryMetaClass();
				int retVal = 0;
				if (isChecked)
				{
                    retVal = CommonHelper.SetCompareCookie(Product.CatalogEntryId.ToString(), mc != null ? mc.Name : String.Empty);
					if (retVal >= 0)
					{
						cmpResult.resultMsg = "Product has been successfully added";
						cmpResult.check = true;
                        cmpResult.refId = refId;
					}
					else if (retVal == -100)
					{
						cmpResult.resultMsg = "Product is already added";
						cmpResult.check = true;
					}
					else if (retVal == -101)
					{
						cmpResult.isError = true;
						cmpResult.resultMsg = "Maximum number of products reached";
						cmpResult.check = isChecked;
					}
				}
				else
				{
                    retVal = CommonHelper.RemoveProductFromCookie(Product.CatalogEntryId.ToString(), mc != null ? mc.Name : String.Empty);
					cmpResult.check = false;
                    cmpResult.refId = refId;
					cmpResult.resultMsg = "Product has been successfully removed";
				}

				cmpResult.retCode = retVal;
			}
			else
			{
				cmpResult.resultMsg = "Invalid input parameter";
				cmpResult.isError = true;
			}
		}
		else
		{
			cmpResult.isError = true;
			cmpResult.resultMsg = "Product is null";
		}

        _returnValue = serializer.Serialize(cmpResult);
	}
	#endregion

    /// <summary>
    /// Gets the compare button id.
    /// </summary>
    /// <returns></returns>
	private string GetCompareButtonId()
	{
		return this.ClientID + "_chkCompare";
	}

    private string GetRefreshObjectId()
    {
        string id = String.Empty;

        HiddenField obj = GetControlFromCollection<HiddenField>(this.Page.Form.Controls, "hfRefreshCompareList");
        if (obj != null)
            id = obj.UniqueID;

        return id;
    }

	private class CompareButtonClickResult
	{
		public bool isError = false;
		public string resultMsg = String.Empty;
		public int retCode = 0;
		public string chbId = String.Empty;
        public string refId = String.Empty;
		public bool check = false;
	}
}
