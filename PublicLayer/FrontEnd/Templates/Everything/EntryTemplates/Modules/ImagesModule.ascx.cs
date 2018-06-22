using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Cms.WebUtility;
using Mediachase.Commerce.Catalog.Objects;
using System.Collections.Generic;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Blob.BlobProfileDownload;
using Mediachase.Ibn.Blob;

public partial class Templates_Everything_Entry_Modules_ImagesModule : BaseStoreUserControl
{
    private ItemAsset[] _DataSource = null;

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    /// <value>The data source.</value>
    public ItemAsset[] DataSource
    {
        set
        {
            _DataSource = value;
        }
        get
        {
            return _DataSource;
        }
    }

    string _GroupName = String.Empty;
    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    /// <value>The name of the group.</value>
    public string GroupName
    {
        get
        {
            return _GroupName;
        }
        set
        {
            _GroupName = value;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
    /// </summary>
    protected override void CreateChildControls()
    {
        BindData();
        base.CreateChildControls();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        if (DataSource == null)
        {
            this.Visible = false;
            return;
        }

        List<ItemAsset> assets = new List<ItemAsset>();
        if (!String.IsNullOrEmpty(GroupName))
        {   
            foreach (ItemAsset asset in DataSource)
            {
                if (asset.GroupName.Equals(GroupName))
                    assets.Add(asset);
            }
        }
        else
        {
            foreach (ItemAsset asset in DataSource)
            {
                assets.Add(asset);
            }
        }

        this.Visible = true;

        List<FolderElement> elements = new List<FolderElement>();
        
        foreach (ItemAsset asset in assets)
        {
            if (asset.AssetType.Equals("file"))
            {
                FolderElement[] myElements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("FolderElementId", FilterElementType.Equal, asset.AssetKey) });
                if (myElements.Length > 0)
                    elements.Add(myElements[0]);
            }
            else
            {
                FolderElement[] myElements = FolderElement.List<FolderElement>(FolderElement.GetAssignedMetaClass(), new FilterElement[] { new FilterElement("ParentId", FilterElementType.Equal, asset.AssetKey) });
                if (myElements.Length > 0)
                {
                    foreach (FolderElement myElement in myElements)
                        elements.Add(myElement);
                }
            }           
        }

        AltList.DataSource = elements;
        AltList.DataBind();
    }

    /// <summary>
    /// Gets the URL.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public string GetUrl(FolderElement element)
    {
        return String.Format("~{0}", element.GetUrl());
    }
}
