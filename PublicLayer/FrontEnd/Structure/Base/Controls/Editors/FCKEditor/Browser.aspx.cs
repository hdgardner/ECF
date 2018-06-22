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
using System.IO;

using Mediachase.Cms.Util;
using System.Text;

public enum ViewMode
{
    All,
    ImageOnly,
    FlashOnly
}

public partial class Browser : System.Web.UI.Page
{

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            BindStorage();
    }

    #region Propertys
    #region RepositoryPath
    /// <summary>
    /// Gets the repository path.
    /// </summary>
    /// <value>The repository path.</value>
    public string RepositoryPath
    {
        get
        {
            if (Request["RepositoryPath"] != null)
            {
                return Request["RepositoryPath"];
            }
            return "~/Repository/";
        }
    }
    #endregion

    #region ArchivePath
    /// <summary>
    /// Gets the archive path.
    /// </summary>
    /// <value>The archive path.</value>
    public string ArchivePath
    {
        get
        {
            if (Request.QueryString["ArchivePath"] != null)
            {
                return Request.QueryString["ArchivePath"];
            }
            return "~/Archive/";
        }
    }
    #endregion

    #region TempPath
    /// <summary>
    /// Gets the temp path.
    /// </summary>
    /// <value>The temp path.</value>
    public string TempPath
    {
        get
        {
            if (Request.QueryString["TempPath"] != null)
            {
                return Request.QueryString["TempPath"];
            }
            return "~/Temp/";
        }
    }
    #endregion

    #region VersionId
    /// <summary>
    /// Gets the version id.
    /// </summary>
    /// <value>The version id.</value>
    public int VersionId
    {
        get
        {
            if (Request.QueryString["PageVersionId"] != null)
            {
                return int.Parse(Request.QueryString["PageVersionId"]);
            }
            return -1;
        }
    }
    #endregion

    #region Mode
    /// <summary>
    /// Gets the mode.
    /// </summary>
    /// <value>The mode.</value>
    public ViewMode Mode
    {
        get
        {
            if (Request.QueryString["Mode"] != null)
            {
                if (Request.QueryString["Mode"].ToUpper() == "ALL")
                {
                    return ViewMode.All;
                }
                if (Request.QueryString["Mode"].ToUpper() == "IMAGEONLY")
                {
                    return ViewMode.ImageOnly;
                }
                if (Request.QueryString["Mode"].ToUpper() == "FLASHONLY")
                {
                    return ViewMode.FlashOnly;
                }
            }
            return ViewMode.All;
        }
    }
    #endregion

    #region CurrentFolder
    /// <summary>
    /// Gets the current folder.
    /// </summary>
    /// <value>The current folder.</value>
    public string CurrentFolder
    {
        get
        {
            if (Request.QueryString["CurrentFolder"] != null)
            {
                return Request.QueryString["CurrentFolder"];
            }
            return RepositoryPath;
        }
    }
    #endregion

    #region ParentFolder
    /// <summary>
    /// Gets the parent folder.
    /// </summary>
    /// <value>The parent folder.</value>
    public string ParentFolder
    {
        get
        {
            if (Request.QueryString["ParentFolder"] != null)
            {
                return Request.QueryString["ParentFolder"];
            }
            return string.Empty;
        }
    }
    #endregion
    #endregion

    #region BindStorage()
    /// <summary>
    /// Binds the storage.
    /// </summary>
    private void BindStorage()
    {
        //show current path
        if (VersionId != -2)
        {
            lbPath.Text = CurrentFolder.Substring(1).Replace(ArchivePath.Substring(1) + VersionId.ToString() + "/", "/Repository/Private/");
        }
        else
        {
            lbPath.Text = CurrentFolder.Substring(1).Replace(ArchivePath.Substring(1) + Membership.GetUser().UserName + "/", "/Repository/Private/");
        }

        DataTable dt = CreateDataTable();

        DirectoryInfo dir = SelectDirectory();

        DataRow dr;

        #region Create back/private folder icon
        //add folder icon
        StringBuilder url = new StringBuilder();
        url.Append("Browser.aspx");
        url.Append("?PageVersionId=" + VersionId.ToString());
        url.Append("&RepositoryPath=" + RepositoryPath);
        url.Append("&ArchivePath=" + ArchivePath);
        url.Append("&TempPath=" + TempPath);
        url.Append("&Mode=" + Request.QueryString["Mode"]);
        dr = dt.NewRow();
        dr["Weight"] = 0;
        dr["Icon"] = string.Empty;
        dr["sortName"] = string.Empty;
        dr["ActionEdit"] = string.Empty;
        dr["ActionVS"] = string.Empty;
        dr["sortSize"] = 0;
        dr["ModifiedDate"] = string.Empty;
        dr["sortModified"] = DateTime.Now;
        dr["Modifier"] = string.Empty;
        dr["sortModifier"] = string.Empty;
        dr["CanDelete"] = false;
        dr["Size"] = string.Empty;

        //add back icon
        if (CurrentFolder != RepositoryPath)
        {
            //get parent directory url
            DirectoryInfo currentDir = new DirectoryInfo(MapPath(CurrentFolder));

            string parentPath = currentDir.Parent.FullName.Replace(MapPath(Request.ApplicationPath), "").Replace("\\", "/");

            if (!parentPath.EndsWith("/"))
                parentPath += "/";

            if (!parentPath.StartsWith("/"))
                parentPath = "/" + parentPath;

            if (!parentPath.StartsWith("~"))
                parentPath = "~" + parentPath;

            //if parent folder is resource folder
            // then set current folder to RepositoryPath
            if (parentPath == TempPath || parentPath == ArchivePath)
            {
                url.Append("&CurrentFolder=" + RepositoryPath);
            }
            else
            {
                url.Append("&CurrentFolder=" + parentPath);
            }

            dr["Name"] = "<a id='divId__a' href='" + url.ToString() + "'>[..]</a>"; ;
            dr["BigIcon"] = "<a href='" + url.ToString() + "'><img src='" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/images/back.gif") + "' align='absmiddle' border='0' /></a>";
        }
        //add icon for user or version resource 
        else
        {
            if (VersionId == -2)
            {
                url.Append("&CurrentFolder=" + ArchivePath + Membership.GetUser().UserName + "/");
            }
            else
            {
                url.Append("&CurrentFolder=" + ArchivePath + VersionId.ToString() + "/");
            }

            dr["Name"] = "<a id='divId__a' href='" + url.ToString() + "'><b>Private</b></a>"; ;
            dr["BigIcon"] = "<a href='" + url.ToString() + "'><img src='" + Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/images/private.gif") + "' align='absmiddle' border='0' /></a>";
        }

        dt.Rows.Add(dr);
        #endregion

        #region Get Folders
        DirectoryInfo[] dirs = dir.GetDirectories();
        foreach (DirectoryInfo d in dirs)
        {
            url = new StringBuilder();
            url.Append("Browser.aspx");
            url.Append("?PageVersionId=" + VersionId.ToString());
            url.Append("&RepositoryPath=" + RepositoryPath);
            url.Append("&ArchivePath=" + ArchivePath);
            url.Append("&TempPath=" + TempPath);
            url.Append("&Mode=" + Request.QueryString["Mode"]);
            url.Append("&CurrentFolder=" + CurrentFolder + d.Name + "/");
            url.Append("&ParentFolder=" + CurrentFolder);

            dr = dt.NewRow();
            dr["Id"] = d.FullName.Replace("\\", "").Replace(".","").Replace(" ","").Replace(":","");
            dr["FullPath"] = d.FullName;
            dr["Weight"] = 1;
            dr["Icon"] = string.Empty;
            dr["BigIcon"] = String.Format("<a id={2} href=\"{0}\"><img src='{1}' align='absmiddle' border='0' /></a>",
					url.ToString(), Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/images/folder1.gif"), "divId_" + (string)dr["Id"] + "_a");

            dr["Name"] = String.Format("<a href=\"{0}\">{1}</a>", url.ToString(), d.Name);
            dr["sortName"] = string.Empty;
            dr["ActionVS"] = string.Empty;
            dr["ActionEdit"] = string.Empty;
            dr["sortSize"] = 0;
            dr["ModifiedDate"] = d.LastWriteTime.ToShortDateString();
            dr["sortModified"] = d.LastWriteTime;
            dr["Modifier"] = string.Empty;
            dr["sortModifier"] = string.Empty;
            dr["CanDelete"] = true;
            dr["Size"] = string.Empty;
            dt.Rows.Add(dr);
        }
        #endregion

        #region Get Files
        //fill datatable
        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            if (!IsImage(file.Extension) && Mode == ViewMode.ImageOnly)
            {
                continue;
            }
            if (!IsFlash(file.Extension) && Mode == ViewMode.FlashOnly)
            {
                continue;
            }

            string fileUrl = string.Empty;
            string resUrl = string.Empty;
            string appPath = MapPath(Request.ApplicationPath);

            fileUrl = Request.ApplicationPath + file.FullName.Replace(appPath, "").Replace("\\", "/");

            if (!CurrentFolder.StartsWith(RepositoryPath))
            {
                string patt = CurrentFolder.Substring(1);
                if (VersionId != -2)
                {
                    resUrl = fileUrl.Replace(ArchivePath.Substring(1) + VersionId.ToString() + "/", "/Repository/Private/");
                }
                else
                {
                    resUrl = fileUrl.Replace(ArchivePath.Substring(1) + Membership.GetUser().UserName + "/", "/Repository/Private/");
                }
            }
            else
            {
                resUrl = fileUrl;
            }

            dr = dt.NewRow();
            dr["Id"] = file.FullName.Replace("\\", "").Replace(".", "").Replace(" ", "").Replace(":", "");
            dr["FullPath"] = file.FullName;
            dr["Weight"] = 2;
            dr["Icon"] = string.Empty;

            //add preview or type icon
            if (IsImage(file.Extension))
            {
                //add preview
                dr["BigIcon"] = String.Format("<a href=\"{0}\"><img src='{1}' align='absmiddle' border='0' " + ImageResize(fileUrl, 90, 90) + "/></a>",
                    "javascript:window.opener.SetUrl('" + resUrl.ToLower() + "');window.close();", fileUrl);
            }
            else
            {
                //add type icon
                if (File.Exists(MapPath("~/images/filetype/") + file.Extension.Replace(".", "") + ".gif"))
                {
                    dr["BigIcon"] = String.Format("<a href=\"{0}\" style='height:48px;width:48px;overflow:hidden;'><img src='{1}' align='absmiddle' border='0'/></a>",
						"javascript:window.opener.SetUrl('" + resUrl.ToLower() + "');window.close();", Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/images/filetype/" + file.Extension.Replace(".", "") + ".gif"));
                }
                else
                {
                    dr["BigIcon"] = String.Format("<a href=\"{0}\" style='height:48px;width:48px;overflow:hidden;'><img src='{1}' align='absmiddle' border='0'/></a>",
						"javascript:window.opener.SetUrl('" + resUrl.ToLower() + "');window.close();", Mediachase.Commerce.Shared.CommerceHelper.GetAbsolutePath("/images/filetype/unknown.gif"));
                }
            }

            dr["Name"] = String.Format("<a id={2} href=\"{0}\">{1}</a>", "javascript:window.opener.SetUrl('" + resUrl.ToLower() + "');window.close();", file.Name, "divId_" + (string)dr["Id"] + "_aa");
            dr["sortName"] = string.Empty;
            dr["ActionVS"] = string.Empty;
            dr["ActionEdit"] = string.Empty;
            dr["sortSize"] = 0;
            dr["ModifiedDate"] = file.LastWriteTime.ToShortDateString();
            dr["sortModified"] = file.LastWriteTime;
            dr["Modifier"] = string.Empty;
            dr["sortModifier"] = string.Empty;
            dr["CanDelete"] = true;
            dr["Size"] = FormatBytes((long)file.Length);
            dt.Rows.Add(dr);
        }
        #endregion

        DataView dv = dt.DefaultView;
        repThumbs.DataSource = dv;
        repThumbs.DataBind();
    }


    #endregion

    #region Helpers
    /// <summary>
    /// Formats the bytes.
    /// </summary>
    /// <param name="bytes">The bytes.</param>
    /// <returns></returns>
    string FormatBytes(long bytes)
    {
        const double ONE_KB = 1024;
        const double ONE_MB = ONE_KB * 1024;
        const double ONE_GB = ONE_MB * 1024;
        const double ONE_TB = ONE_GB * 1024;
        const double ONE_PB = ONE_TB * 1024;
        const double ONE_EB = ONE_PB * 1024;
        const double ONE_ZB = ONE_EB * 1024;
        const double ONE_YB = ONE_ZB * 1024;

        if ((double)bytes <= 999)
            return bytes.ToString() + " bytes";
        else if ((double)bytes <= ONE_KB * 999)
            return ThreeNonZeroDigits((double)bytes / ONE_KB) + " KB";
        else if ((double)bytes <= ONE_MB * 999)
            return ThreeNonZeroDigits((double)bytes / ONE_MB) + " MB";
        else if ((double)bytes <= ONE_GB * 999)
            return ThreeNonZeroDigits((double)bytes / ONE_GB) + " GB";
        else if ((double)bytes <= ONE_TB * 999)
            return ThreeNonZeroDigits((double)bytes / ONE_TB) + " TB";
        else if ((double)bytes <= ONE_PB * 999)
            return ThreeNonZeroDigits((double)bytes / ONE_PB) + " PB";
        else if ((double)bytes <= ONE_EB * 999)
            return ThreeNonZeroDigits((double)bytes / ONE_EB) + " EB";
        else if ((double)bytes <= ONE_ZB * 999)
            return ThreeNonZeroDigits((double)bytes / ONE_ZB) + " ZB";
        else
            return ThreeNonZeroDigits((double)bytes / ONE_YB) + " YB";
    }

    /// <summary>
    /// Threes the non zero digits.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    string ThreeNonZeroDigits(double value)
    {
        if (value >= 100)
            return ((int)value).ToString();
        else if (value >= 10)
            return value.ToString("0.0");
        else
            return value.ToString("0.00");
    }

    /// <summary>
    /// Determines whether the specified ext is image.
    /// </summary>
    /// <param name="ext">The ext.</param>
    /// <returns>
    /// 	<c>true</c> if the specified ext is image; otherwise, <c>false</c>.
    /// </returns>
    bool IsImage(string ext)
    {
        ArrayList imgEXT = new ArrayList();
        imgEXT.Add(".GIF");
        imgEXT.Add(".JPG");
        imgEXT.Add(".JPEG");
        imgEXT.Add(".BMP");
        imgEXT.Add(".TIFF");
        imgEXT.Add(".ICO");
        imgEXT.Add(".PNG");

        return imgEXT.Contains((ext.ToUpper()));
    }

    /// <summary>
    /// Determines whether the specified ext is flash.
    /// </summary>
    /// <param name="ext">The ext.</param>
    /// <returns>
    /// 	<c>true</c> if the specified ext is flash; otherwise, <c>false</c>.
    /// </returns>
    private bool IsFlash(string ext)
    {
        ArrayList flashEXT = new ArrayList();
        flashEXT.Add(".SWF");

        return flashEXT.Contains(ext.ToUpper());
    }

    /// <summary>
    /// Images the resize.
    /// </summary>
    /// <param name="imageUrl">The image URL.</param>
    /// <param name="MaxWidth">Width of the max.</param>
    /// <param name="MaxHeight">Height of the max.</param>
    /// <returns></returns>
    string ImageResize(string imageUrl, int MaxWidth, int MaxHeight)
    {
        string imagePath = MapPath(imageUrl);
        System.Drawing.Bitmap img = new System.Drawing.Bitmap(imagePath);

        if (img.Width > MaxWidth || img.Height > MaxHeight)
        {
            double widthRatio = (double)img.Width / (double)MaxWidth;
            double heightRatio = (double)img.Height / (double)MaxHeight;
            double ratio = Math.Max(widthRatio, heightRatio);
            int newWidth = (int)(img.Width / ratio);
            int newHeight = (int)(img.Height / ratio);
            img.Dispose();
            return " width='" + newWidth.ToString() + "px'" + " height='" + newHeight.ToString() + "px' ";
        }
        else
        {
            img.Dispose();
            return "";
        }
    }

    /// <summary>
    /// Creates the data table.
    /// </summary>
    /// <returns></returns>
    private static DataTable CreateDataTable()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("Id", typeof(string)));
        dt.Columns.Add(new DataColumn("Weight", typeof(int)));	//0- Root, 1- Folder, 2-File
        dt.Columns.Add(new DataColumn("Icon", typeof(string)));
        dt.Columns.Add(new DataColumn("BigIcon", typeof(string)));
        dt.Columns.Add(new DataColumn("Name", typeof(string)));
        dt.Columns.Add(new DataColumn("sortName", typeof(string)));
        dt.Columns.Add(new DataColumn("ModifiedDate", typeof(string)));
        dt.Columns.Add(new DataColumn("sortModified", typeof(DateTime)));
        dt.Columns.Add(new DataColumn("Modifier", typeof(string)));
        dt.Columns.Add(new DataColumn("sortModifier", typeof(string)));
        dt.Columns.Add(new DataColumn("ActionVS", typeof(string)));
        dt.Columns.Add(new DataColumn("ActionEdit", typeof(string)));
        dt.Columns.Add(new DataColumn("CanDelete", typeof(bool)));
        dt.Columns.Add(new DataColumn("Size", typeof(string)));
        dt.Columns.Add(new DataColumn("FullPath", typeof(string)));
        dt.Columns.Add(new DataColumn("sortSize", typeof(int)));
        return dt;
    }

    /// <summary>
    /// Selects the directory.
    /// </summary>
    /// <returns></returns>
    private DirectoryInfo SelectDirectory()
    {
        DirectoryInfo dir = new DirectoryInfo(MapPath(CurrentFolder));

        if (!dir.Exists)
        {
            dir.Create();
        }

        return dir;
    }
    #endregion

    #region Event Handler
    /// <summary>
    /// Handles the ItemCommand event of the repThumbs control.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void repThumbs_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {

            if (File.Exists(e.CommandArgument.ToString()))
            {
                FileInfo file = new FileInfo(e.CommandArgument.ToString());
                file.Delete();
            }
            if (Directory.Exists(e.CommandArgument.ToString()))
            {
                Directory.Delete(e.CommandArgument.ToString(), true);
            }
            BindStorage();
        }
    }

    /// <summary>
    /// Handles the Click event of the Button1 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (FileUpload1.PostedFile.FileName != string.Empty)
        {
            //GA: 05.07.2006
            //IE fix. Get filename without full path
            string fileName = string.Empty;
            if (FileUpload1.PostedFile.FileName.LastIndexOf("\\") > 0)
            {
                fileName = FileUpload1.PostedFile.FileName.Substring(FileUpload1.PostedFile.FileName.LastIndexOf("\\") + 1);
            }
            else
            {
                fileName = FileUpload1.PostedFile.FileName;
            }

            FileUpload1.PostedFile.SaveAs(MapPath(CurrentFolder) + fileName);

            BindStorage();
        }
    }

    /// <summary>
    /// Handles the Click event of the Button2 control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Button2_Click(object sender, EventArgs e)
    {
        if (txtFolderName.Text != string.Empty)
        {
            string parentPath = MapPath(CurrentFolder);

            try
            {
                Directory.CreateDirectory(parentPath + txtFolderName.Text);
                BindStorage();
            }
            catch
            {
                throw new Exception("Error! Folder path " + parentPath + txtFolderName.Text);
            }
        }
    }

    #endregion
}