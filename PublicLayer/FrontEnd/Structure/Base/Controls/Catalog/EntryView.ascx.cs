using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.Web.UI.Controls;
using Mediachase.Cms.WebUtility.Commerce;

public partial class Controls_Catalog_EntryView : BaseStoreUserControl, ICmsDataAdapter
{
    string _Code = String.Empty;
    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>The code.</value>
    public string Code
    {
        get
        {
            if (String.IsNullOrEmpty(_Code))
                return Request["ec"];

            return _Code;
        }
        set
        {
            _Code = value;
        }
    }

    string _NodeCode = String.Empty;
    /// <summary>
    /// Gets or sets the node code.
    /// </summary>
    /// <value>The node code.</value>
    public string NodeCode
    {
        get
        {
            if (String.IsNullOrEmpty(_NodeCode))
                return Request["nc"];

            return _NodeCode;
        }
        set
        {
            _NodeCode = value;
        }
    }

    string _CatalogName = String.Empty;
    /// <summary>
    /// Gets or sets the name of the catalog.
    /// </summary>
    /// <value>The name of the catalog.</value>
    public string CatalogName
    {
        get
        {
            if (String.IsNullOrEmpty(_CatalogName) && Request["c"] != null)
                return Request["c"];

            return _CatalogName;
        }
        set
        {
            _CatalogName = value;
        }
    }

    string _Template = String.Empty;
    /// <summary>
    /// Gets or sets the template.
    /// </summary>
    /// <value>The template.</value>
    public string Template
    {
        get
        {
            return _Template;
        }
        set
        {
            _Template = value;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Page_Load(object sender, System.EventArgs e)
    {
        CreateChildControlsTree();
    }

    /// <summary>
    /// Creates the child controls tree.
    /// </summary>
    private void CreateChildControlsTree()
    {
        if (!String.IsNullOrEmpty(Code))
        {
            System.Web.UI.Control ctrl = null;

            string id = "Template" + this.ID;
            ctrl = ProductInfoHolder.FindControl(id);
            if (ctrl == null)
            {
                CatalogEntryDto dto = CatalogContext.Current.GetCatalogEntryDto(Code);
                string templateUrl = GetTemplateUrl(dto.CatalogEntry[0].TemplateName);

                if (String.IsNullOrEmpty(templateUrl))
                    throw new System.NullReferenceException(String.Format("CatalogEntry \"{0}\" does not have display template specified.", Code));

                try
                {
                    ctrl = this.LoadControl(templateUrl.ToString());
                    
                    if (ctrl is IContextUserControl)
                    {
                        IDictionary dic = new ListDictionary();
                        dic.Add("Code", Code);
                        dic.Add("NodeCode", NodeCode);
                        dic.Add("CatalogName", CatalogName);
                        ((IContextUserControl)ctrl).LoadContext(dic);
                    }
                }
                catch (HttpException ex)
                {
                    if (ex.GetHttpCode() == 404)
                        throw new System.IO.FileNotFoundException("Template not found", ex);
                    else
                        throw;
                }

                this.ProductInfoHolder.Controls.Add(ctrl);

                Session["LastCatalogPageUrl"] = CMSContext.Current.CurrentUrl;

                StoreHelper.AddBrowseHistory("Entries", Code);

                //Profile.LastCatalogPageUrl = CMSContext.Current.CurrentUrl;

                /*
                // Record history
                StringCollection historyDic = (StringCollection)Profile["EntryHistory"];

                // Check if the code already exists
                if (historyDic.Contains(Code))
                {
                    historyDic.RemoveAt(historyDic.IndexOf(Code));
                }

                // Only keep history of last 5 items visited
                if (historyDic.Count >= 5)
                    historyDic.RemoveAt(0);

                historyDic.Add(Code);

                // set value
                Profile["EntryHistory"] = historyDic;
                 * */
            }
        }
    }

    /// <summary>
    /// Gets the template URL.
    /// </summary>
    /// <param name="defaultTemplate">The default template.</param>
    /// <returns></returns>
    private string GetTemplateUrl(string defaultTemplate)
    {
        return DictionaryManager.GetTemplatePath(String.IsNullOrEmpty(Template) ? defaultTemplate : Template, "entry");
    }

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (CMSContext.Current.IsDesignMode)
            divProductViewer.InnerHtml = "Catalog Entry Viewer Control";
    }


	#region ICmsDataAdapter Members

    /// <summary>
    /// Sets the param info.
    /// </summary>
    /// <param name="param">The param.</param>
	public void SetParamInfo(object param)
	{
		ControlSettings settings = (ControlSettings)param;

		if (settings != null && settings.Params != null)
		{
			if (settings.Params["EntryCode"] != null)
				this.Code = settings.Params["EntryCode"].ToString();

			if (settings.Params["NodeCode"] != null)
				this.NodeCode = settings.Params["NodeCode"].ToString();

			if (settings.Params["CatalogName"] != null)
				this.CatalogName = settings.Params["CatalogName"].ToString();

			if (settings.Params["DisplayTemplate"] != null)
				this.Template = settings.Params["DisplayTemplate"].ToString();

			this.DataBind();
		}
	}

	#endregion
}