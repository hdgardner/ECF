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

using Mediachase.Cms;
using Mediachase.Web.UI;
//using Mediachase.Cms;

public partial class Controls_VersionList : PageBase
{
    #region PageId
    /// <summary>
    /// Gets the page id.
    /// </summary>
    /// <value>The page id.</value>
    public int PageId
    {
        get
        {
            if (Request["PageId"] != null)
            {
                return int.Parse(Request["PageId"]);
            }
            return -1;
        }
    }
    #endregion

    #region GetPageUrl
    /// <summary>
    /// Gets the page URL.
    /// </summary>
    /// <param name="VersionId">The version id.</param>
    /// <returns></returns>
    protected string GetPageUrl(int VersionId)
    {
        string path = String.Empty;
        using (IDataReader reader = FileTreeItem.GetItemById(PageId))
        {
            if (reader.Read())
            {
                path = "~" + (string)reader["Outline"];
            }

            reader.Close();
        }
        path += "?VersionId=" + VersionId.ToString();
        return path.Trim();
    }
    #endregion

    #region GetStatusName()
    /// <summary>
    /// Gets the name of the status.
    /// </summary>
    /// <param name="StatusId">The status id.</param>
    /// <returns></returns>
    protected string GetStatusName(int StatusId)
    {
        using (IDataReader reader = WorkflowStatus.LoadById(StatusId))
        {
            if (reader.Read())
            {
                string friendlyName = (string)reader["FriendlyName"];
                reader.Close();
                return friendlyName;
            }

            reader.Close();
        }
        return "Draft";
    } 
    #endregion

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        DataView dvVersions = PageVersion.GetVersionByLangIdDT(PageId, LanguageId).DefaultView;
        grdMain.DataSource = dvVersions;
        grdMain.DataBind();
    }
}
