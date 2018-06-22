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

using Mediachase.Cms.Pages;
using Mediachase.Cms;
using Mediachase.Cms.DataAdapter;
using Mediachase.Cms.Web.UI.Controls;


#region PopUpEditor
public partial class PopUpEditor2 : System.Web.UI.UserControl, ICmsDataAdapter
{
    /// <summary>
    /// When implemented, gets or sets text between the opening and closing tags of a user control.
    /// </summary>
    /// <value></value>
    /// <returns>The text placed between the opening and closing tags of a user control.</returns>
    public string InnerText
    {
        get
        {
			if (CMSContext.Current.IsDesignMode)
			{
                DynamicNode dn = null;
				
				//this.EnsureID();
                if (this.Parent != null && this.Parent.Parent != null && this.Parent.Parent.ID != null)
                    dn = PageDocument.Current.DynamicNodes.LoadByUID(this.Parent.Parent.ID.Replace("_wControl", ""));

				if (dn != null)
				{
					ControlSettings cs = dn.GetSettings(dn.NodeUID);
					if (cs.Params != null && cs.Params["InnerText"] != null)
						return cs.Params["InnerText"].ToString();
				}
			}

            ControlSettings ctrlSettings = PageDocument.Current.StaticNode.Controls[this.ID];
            if (ctrlSettings != null && ctrlSettings.Params != null && ctrlSettings.Params["InnerText"] != null)
                return ctrlSettings.Params["InnerText"].ToString();

            return label.innerText;
        }
        set
        {
            label.innerText = value;

			//this.EnsureID();
            if (this.Parent != null && this.Parent.Parent != null && this.Parent.Parent.ID != null)
			{
				DynamicNode dn = PageDocument.Current.DynamicNodes.LoadByUID(this.Parent.Parent.ID.Replace("_wControl", ""));
				if (dn != null)
				{
					ControlSettings cs = dn.GetSettings(dn.NodeUID);
					if (cs.Params == null)
						cs.Params = new Param();

					cs.Params["InnerText"] = value;
					cs.IsModified = true;
					dn.IsModified = true;
				}
				else
				{
					ControlSettings cs = PageDocument.Current.StaticNode.Controls[this.ID];
					if (cs == null)
					{
						cs = new ControlSettings();
						PageDocument.Current.StaticNode.Controls.Add(this.ID, cs);
					}

					if (cs.Params == null)
						cs.Params = new Param();

					cs.Params["InnerText"] = value;
					cs.IsModified = true;
				}
			}
        }
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The key.</value>
    public string Key
    {
        get
        {
            return label.ID;
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {

        if (CMSContext.Current.IsDesignMode)
        {
            // Show controls that we need in design mode
            tmpContainerValue.Visible = true;
            hfEditInfo.Visible = true;

            //REGISTER SCRIPT
			/*loaded in CMS_Scripts
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "<script type='text/javascript' language='javascript' src='" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/Scripts/editableWrapper.js") + "'></script>");
			*/
 
            if ((this.Attributes["onclick"] != string.Empty) && (this.Attributes["onclick"] != null))
				//wrapperInline.Attributes.Add("ondblclick", string.Format("EditPopupWrapper('{0}');", hfEditInfo.ClientID) + this.Attributes["onclick"] + ";RecordEditedKey('" + label.ID + "');InitEditedContainer('" + label.ClientID + "');"/*+"EraseEditedTempValue();"*/);
				wrapperInline.Attributes.Add("ondblclick", string.Format("var _sh=$find('SnapHolder1_Snap'); if(_sh!=null){{_sh.EditPopupWrapper('{0}');", hfEditInfo.ClientID) + this.Attributes["onclick"] + ";_sh.RecordEditedKey('" + label.ID + "');_sh.InitEditedContainer('" + label.ClientID + "');}");
            else
                //wrapperInline.Attributes.Add("ondblclick", string.Format("EditPopupWrapper('{0}');", hfEditInfo.ClientID) + "RecordStaticEditedInfo('editStatic','" + this.ID + "','" + label.ID + "');InitEditedContainer('" + label.ClientID + "');"/*+"EraseEditedTempValue();"*/);
				wrapperInline.Attributes.Add("ondblclick", string.Format("var _sh=$find('SnapHolder1_Snap'); if(_sh!=null){{_sh.EditPopupWrapper('{0}');", hfEditInfo.ClientID) + "_sh.RecordStaticEditedInfo('editStatic','" + this.ID + "','" + label.ID + "');_sh.InitEditedContainer('" + label.ClientID + "');}");

            label.AllowEdit = "True";

			if (Request.Form[hfEditInfo.UniqueID] != InnerText && Request.Form[hfEditInfo.UniqueID] != string.Empty && Request.Form[hfEditInfo.UniqueID] != null)
			{
				InnerText = Request.Form[hfEditInfo.UniqueID];
				hfEditInfo.Value = "";
			}
        }
        else
        {
            label.AllowEdit = "False";
            wrapperInline.Attributes.Add("ondblclick", "");
            
            // Hide controls that we don't need
            tmpContainerValue.Visible = false;
            hfEditInfo.Visible = false;
        }

        label.innerText = InnerText;
    }

	#region ICmsDataAdapter Members

    /// <summary>
    /// Sets the param info.
    /// </summary>
    /// <param name="param">The param.</param>
	public void SetParamInfo(object param)
	{
		ControlSettings cs = (ControlSettings)param;

		if (cs.Params != null && cs.Params["InnerText"] != null)
			label.innerText = cs.Params["InnerText"].ToString();		
	}
	#endregion
}
#endregion