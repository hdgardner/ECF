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

using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Pages;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.Web.UI.Controls;

public partial class Controls_Catalog_NodeView : BaseStoreUserControl, ICmsDataAdapter
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
                return Request["nc"];

            return _Code;
        }
        set
        {
            _Code = value;
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
            ctrl = CategoryInfoHolder.FindControl(id);
            if (ctrl == null)
            {
                CatalogNodeDto dto = CatalogContext.Current.GetCatalogNodeDto(Code);

                if (dto.CatalogNode.Count == 0)
                    throw new System.NullReferenceException(String.Format("CatalogNode \"{0}\" not found.", Code));

                string templateUrl = GetTemplateUrl(dto.CatalogNode[0].TemplateName);

                if(String.IsNullOrEmpty(templateUrl))
                    throw new System.NullReferenceException(String.Format("CatalogNode \"{0}\" does not have display template specified.", Code));

                try
                {
                    ctrl = this.LoadControl(templateUrl.ToString());

                    if (ctrl is IContextUserControl)
                    {
                        IDictionary dic = new ListDictionary();
                        dic.Add("Code", Code);
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

                this.CategoryInfoHolder.Controls.Add(ctrl);
                //Profile.LastCatalogPageUrl = CMSContext.Current.CurrentUrl;
                Session["LastCatalogPageUrl"] = CMSContext.Current.CurrentUrl;
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
        return DictionaryManager.GetTemplatePath(String.IsNullOrEmpty(Template) ? defaultTemplate : Template, "node");
    }

    /// <summary>
    /// Handles the PreRender event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (CMSContext.Current.IsDesignMode)
            divCatalogNodeViewer.InnerHtml = "Catalog Node Viewer Control";
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
			if (settings.Params["NodeCode"] != null)
				this.Code = settings.Params["NodeCode"].ToString();

			if (settings.Params["DisplayTemplate"] != null)
				this.Template = settings.Params["DisplayTemplate"].ToString();

			if (settings.Params["CatalogName"] != null)
				this.CatalogName = settings.Params["CatalogName"].ToString();
		}
	}

	#endregion
}