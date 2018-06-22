using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Managers;
using System.Collections.Generic;
using Mediachase.Cms.Util;
using Mediachase.Ibn.Library;
using Mediachase.Ibn.Data;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml;
using System.IO;

public partial class Templates_Everything_Entry_Modules_ImagesXml : System.Web.UI.Page
{
    public class ImageNode
    {
        public string Title;
        public string Description;
        public string Url;
        public string ImageUrl;
    }


    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    /// <value>The data source.</value>
    public string EntryCode
    {
        get
        {
            return Request.QueryString["code"];
        }
    }

    /// <summary>
    /// Gets the XSL.
    /// </summary>
    /// <value>The XSL.</value>
    public string Xsl
    {
        get
        {
            return Request.QueryString["xsl"];
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
        BindData();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        Entry entry = CatalogContext.Current.GetCatalogEntry(EntryCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));

        if (entry == null || entry.Assets == null)
        {
            this.Visible = false;
            return;
        }

        List<ItemAsset> assets = new List<ItemAsset>();
        if (!String.IsNullOrEmpty(GroupName))
        {
            foreach (ItemAsset asset in entry.Assets)
            {
                if (asset.GroupName.Equals(GroupName))
                    assets.Add(asset);
            }
        }
        else
        {
            foreach (ItemAsset asset in entry.Assets)
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

        List<ImageNode> images = new List<ImageNode>();

        // Get Images
        foreach (Image image in entry.ItemAttributes.Images.Image)
        {
            ImageNode node = new ImageNode();
            node.Title = entry.ItemAttributes["DisplayName"].ToString();
            //node.Url = String.Format("javascript:window.open('{0}', '1', 'height=200,width=400,status=yes,toolbar=no,menubar=no,location=no');", Page.ResolveUrl(String.Format("FlashGallery.aspx?code={0}", this.EntryCode)));
            node.Url = String.Format("{0}", Page.ResolveUrl(String.Format("FlashGallery.aspx?code={0}", this.EntryCode)));
            node.ImageUrl = Page.ResolveUrl(image.Url);
            node.Description = entry.ItemAttributes["Description"].ToString();
            images.Add(node);
        }

        foreach (FolderElement element in elements)
        {
            ImageNode node = new ImageNode();
            node.Title = element.Properties["Title"].Value != null ? element.Properties["Title"].Value.ToString() : element.Name;
            node.Url = String.Format("{0}", Page.ResolveUrl(String.Format("FlashGallery.aspx?code={0}", this.EntryCode)));
            node.ImageUrl = Page.ResolveUrl(String.Format("~{0}", element.GetUrl()));
            node.Description = element.Properties["Description"].Value != null ? element.Properties["Description"].Value.ToString() : String.Empty;
            images.Add(node);
        }

        // Write XML
        WriteXml(images);
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

    /// <summary>
    /// Writes the XML.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    private void WriteXml(List<ImageNode> nodes)
    {
        Response.CacheControl = "no-cache";
        Response.AddHeader("Pragma", "no-cache");
        Response.Expires = -1;
        XmlSerializer serializer = new XmlSerializer(nodes.GetType());
        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load(Server.MapPath(this.Xsl));
        MemoryStream stream = new MemoryStream();
        
        
        //serializer.Serialize(Response.OutputStream, nodes);
        serializer.Serialize(stream, nodes);

        stream.Position = 0;
        XPathDocument pathDoc = new XPathDocument(stream);
        xslt.Transform(pathDoc, null, Response.OutputStream);

        //string json = new JavaScriptSerializer().Serialize(nodes);
        //Response.Write(json);
    }
}
