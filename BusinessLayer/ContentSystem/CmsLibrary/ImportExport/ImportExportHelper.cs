using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Security;
using System.Xml;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Cms.Pages;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Commerce.Core.Dto;
using System.Threading;

namespace Mediachase.Cms.ImportExport
{
	[Serializable]
	public class SiteImportExportException : Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteImportExportException"/> class.
        /// </summary>
		public SiteImportExportException()
			: base()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteImportExportException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
		public SiteImportExportException(string message)
			: base(message)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteImportExportException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
		public SiteImportExportException(string message, Exception inner)
			: base(message, inner)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteImportExportException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected SiteImportExportException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class SiteImportExportEventArgs : EventArgs
	{
		private string _Message = String.Empty;

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }

		private double _CompletedPercentage = 0;

		/// <summary>
		/// Gets or sets the completed percentage.
		/// </summary>
		/// <value>The completed percentage.</value>
		public double CompletedPercentage
		{
			get { return _CompletedPercentage; }
			set { _CompletedPercentage = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SiteImportExportEventArgs"/> class.
		/// </summary>
		public SiteImportExportEventArgs()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SiteImportExportEventArgs"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="percentage">The percentage.</param>
		public SiteImportExportEventArgs(string message, double percentage)
			: base()
		{
			this.Message = message;
			this.CompletedPercentage = percentage;
		}
	}

	/// <summary>
	/// The ImportExportMessageType enumeration defines the import/export message type.
	/// </summary>
	[Serializable]
	public enum SiteImportExportMessageType
	{
		/// <summary>
		/// Represents the info message type.
		/// </summary>
		Info = 0x00,
		/// <summary>
		/// Represents the warning message type.
		/// </summary>
		Warning = 0x01
	}

    /// <summary>
    /// 
    /// </summary>
	public delegate void SiteImportExportProgressMessageHandler(object source, SiteImportExportEventArgs args);
	
    /// <summary>
    /// Class used to import export elements of the ECF.
    /// </summary>
    public class ImportExportHelper
	{
		#region ImportExport Steps
		internal enum ExportSteps
		{
			Init = 0x00,
			StartExportSites = 0x01,
			EndExportSites = 0x02,
			Finish = 0x03
		}

		internal enum SiteExportSteps
		{
			StartExportSite = 0x01,
			StartExportBasicInfo = 0x02,
			EndExportBasicInfo = 0x03,
			StartExportMenus = 0x04,
			EndExportMenus = 0x05,
			StartExportPages = 0x06,
			EndExportPages = 0x07,
			EndExportSite = 0x08
		}

		internal enum ImportSteps
		{
			Init = 0x00,
			StartImportSite = 0x01,
			StartImportDomainInfo = 0x02,
			EndImportDomainInfo = 0x03,
			StartImportLanguages = 0x04,
			EndImportLanguages = 0x05,
			StartImportTemplates = 0x06,
			EndImportTemplates = 0x07,
			StartImportAttributes = 0x08,
			EndImportAttributes = 0x09,
			StartImportMenus = 0x0A,
			EndImportMenus = 0x0B,
			StartImportFolders = 0x0C,
			EndImportFolders = 0x0D,
			EndImportSite = 0x0E,
			Finish = 0x0F
		}

		/// <summary>
		/// Gets the total export steps.
		/// </summary>
		/// <returns></returns>
		internal static int GetTotalExportStepsCount()
		{
			return Enum.GetValues(typeof(ExportSteps)).Length;
		}

		/// <summary>
		/// Gets the site export steps.
		/// </summary>
		/// <returns></returns>
		internal static int GetSiteExportStepsCount()
		{
			return Enum.GetValues(typeof(SiteExportSteps)).Length;
		}

		/// <summary>
		/// Gets the total import steps.
		/// </summary>
		/// <returns></returns>
		internal static int GetTotalImportStepsCount()
		{
			return Enum.GetValues(typeof(ImportSteps)).Length;
		}
		#endregion

		//private static ImportExportHelper _Instance;
        bool IsFirstCall = true; // Used in AddMenuItems

		//public static ImportExportHelper Current
		//{
		//    get
		//    {
		//        if (_Instance == null)
		//            _Instance = new ImportExportHelper();

		//        return _Instance;
		//    }
		//}

		//private ImportExportHelper()
		//{
		//}

		public event SiteImportExportProgressMessageHandler ImportExportProgressMessage;

        /// <summary>
        /// Called when [import export progress message].
        /// </summary>
        /// <param name="source">The source.</param>
		/// <param name="args">The args.</param>
		protected virtual void OnSiteImportExportProgressMessage(object source, SiteImportExportEventArgs args)
		{
			if (this.ImportExportProgressMessage != null)
				this.ImportExportProgressMessage(source, args);
		}

		/// <summary>
		/// Called when [event].
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="percentage">The percentage.</param>
		private void OnEvent(string message, double percentage)
		{
			OnSiteImportExportProgressMessage(this, new SiteImportExportEventArgs(message, percentage));
		}

		/// <summary>
		/// Called when [event].
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="msgType">The message type.</param>
		private void OnEvent(string message, double percentage, SiteImportExportMessageType msgType)
		{
			if (msgType == SiteImportExportMessageType.Warning)
				message = "Warning - " + message;

			OnSiteImportExportProgressMessage(this, new SiteImportExportEventArgs(message, percentage));
		}

		private static double GetProgressPercent(int currentStep, int totalSteps)
		{
			double percentCompleted = 0d;
			if (totalSteps > 0)
				percentCompleted = Convert.ToDouble(currentStep) * 100 / Convert.ToDouble(totalSteps);

			return percentCompleted;
		}

		private static double GetProgressPercent2(double currentStep, int totalSteps)
		{
			double percentCompleted = 0d;
			if (totalSteps > 0)
				percentCompleted = currentStep * 100 / Convert.ToDouble(totalSteps);

			return percentCompleted;
		}

        #region Export Site Function
        /// <summary>
        /// Exports the site.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="output">The output.</param>
        public void ExportSite(Guid siteId, Stream output)
        {
			OnEvent("Starting export...", 0);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;

            XmlWriter writer = XmlWriter.Create(output, settings);

            //Use automatic indentation for readability.
            //writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();

			OnEvent("Retrieving site data from the database...", 0);

            // Export site info
            SiteDto siteDto = CMSContext.Current.GetSiteDto(siteId, true);

			OnEvent("Site data retrieved successfully.", 0);

            if (siteDto.Site.Count == 0)
                throw new SiteImportExportException("Site not found");

            writer.WriteStartElement("Sites", String.Empty); // Start Sites
            writer.WriteAttributeString("version", "1.0");

			// calculate total steps
			int totalSiteExportSteps = GetSiteExportStepsCount();
			int currentStep = 0;

			int totalSteps = GetTotalExportStepsCount() + totalSiteExportSteps * siteDto.Site.Count;

			// set current step
			currentStep = (int)ExportSteps.StartExportSites;

			OnEvent("Start exporting sites", GetProgressPercent(currentStep, totalSteps));

			int siteCount = -1; // counter for amount of exported sites

			// start exporting sites
            foreach (SiteDto.SiteRow row in siteDto.Site)
            {
				siteCount++;

                writer.WriteStartElement("Site", String.Empty); // start Site export

				OnEvent("Start exporting site " + row.Name, GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.StartExportSite, totalSteps));

				OnEvent("Start exporting basic site information", GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.StartExportBasicInfo, totalSteps));
                #region Write Basic Info
                writer.WriteAttributeString("id", row.SiteId.ToString());
                writer.WriteAttributeString("isActive", row.IsActive.ToString());
                writer.WriteAttributeString("isDefault", row.IsDefault.ToString());
                writer.WriteAttributeString("Folder", row.Folder);
                writer.WriteElementString("Name", row.Name);
                writer.WriteElementString("Description", row.Description);
                #endregion

				#region Parse domains
                writer.WriteStartElement("Domains", ""); // Start Domains

                string rowDomain = row.Domain.Replace("\r\n", "\n");
                string[] domains = rowDomain.Split(new char[] { '\n' });

                foreach (string domain in domains)
                    writer.WriteElementString("Domain", domain);

                writer.WriteEndElement();  // end Domains
                #endregion

				#region Write Languages
				writer.WriteStartElement("Languages", ""); // start Languages

				foreach (SiteDto.SiteLanguageRow langRow in row.GetSiteLanguageRows())
					writer.WriteElementString("Language", langRow.LanguageCode);

				writer.WriteEndElement();  // end Languages
				#endregion

                #region Write Display Templates
				OnEvent("Start exporting display templates", GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.StartExportBasicInfo, totalSteps));
                writer.WriteStartElement("DisplayTemplates", ""); // start templates
                TemplateDto templates = DictionaryManager.GetTemplateDto();
                foreach (TemplateDto.main_TemplatesRow templateRow in templates.main_Templates)
                {
                    writer.WriteStartElement("Template", ""); // Start template
                    writer.WriteAttributeString("name", templateRow.Name);
                    writer.WriteAttributeString("friendlyname", templateRow.FriendlyName);
                    writer.WriteAttributeString("path", templateRow.Path);
                    writer.WriteAttributeString("type", templateRow.TemplateType);
                    writer.WriteAttributeString("lang", templateRow.LanguageCode);
                    writer.WriteEndElement();  // End template
                }
                writer.WriteEndElement();  // end templates
				OnEvent("Finished exporting display templates", GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.StartExportBasicInfo, totalSteps));
                #endregion

				#region Write attributes
				writer.WriteStartElement("Attributes", ""); // Start Attributes
                foreach (SiteDto.main_GlobalVariablesRow attrRow in siteDto.main_GlobalVariables)
                {
                    writer.WriteStartElement("Attribute", ""); // Start Attribute
                    writer.WriteAttributeString("name", attrRow.KEY);
                    writer.WriteString(attrRow.VALUE);
                    writer.WriteEndElement();  // End Attribute
                }

                writer.WriteEndElement();  // End Attributes
                #endregion
				OnEvent("Finished exporting basic site information", GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.EndExportBasicInfo, totalSteps));

				OnEvent("Start exporting menus for the site", GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.StartExportMenus, totalSteps));
                #region Write Menus
                writer.WriteStartElement("Menus", ""); // Start Menus
                MenuDto menuDto = MenuManager.GetMenuDto(siteId);
                foreach (MenuDto.MenuRow menuRow in menuDto.Menu)
                {
                    writer.WriteStartElement("Menu", ""); // Start Menu
                    writer.WriteAttributeString("name", menuRow.FriendlyName);

                    writer.WriteStartElement("Items", ""); // Start Items
                    DataView view = new DataView(menuDto.MenuItem);
                    view.RowFilter = String.Format("MenuId = {0} and IsRoot = 1", menuRow.MenuId);
                    foreach (DataRowView itemRow in view)
                    {
                        WriteMenuItem(writer, menuDto, (MenuDto.MenuItemRow)itemRow.Row);
                    }
                    writer.WriteEndElement();  // End Items

                    writer.WriteEndElement();  // End Menu
                }

                writer.WriteEndElement();  // End Menus
                #endregion
				OnEvent("Finished exporting menus for the site", GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.EndExportMenus, totalSteps));

				OnEvent("Start exporting site's folders and pages", GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.StartExportPages, totalSteps));
                #region Write Pages & Folders
                writer.WriteStartElement("Folders", ""); // Start Folders
                // Get very root folder
				DataTable rootItem = FileTreeItem.GetItemByIdDT(FileTreeItem.GetRoot(siteId));
				if (rootItem != null && rootItem.Rows.Count > 0)
					WriteFolderPage(writer, rootItem.Rows[0]);

                writer.WriteEndElement();  // End Folders
                #endregion
				OnEvent("Finished exporting site's folders and pages", GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.EndExportPages, totalSteps));

                writer.WriteEndElement();  // end Site

				OnEvent("Finished exporting site " + row.Name, GetProgressPercent(currentStep + siteCount * totalSiteExportSteps + (int)SiteExportSteps.EndExportSite, totalSteps));
            }

			currentStep = siteCount * totalSiteExportSteps + (int)ExportSteps.EndExportSites;

            writer.WriteEndElement();  // end Sites

			OnEvent("Finished exporting sites", GetProgressPercent(currentStep, totalSteps));

            writer.WriteEndDocument();

            writer.Close();

			OnEvent("Export successfully finished.", 100);
        }

        #region Helper Functions
        /// <summary>
        /// Writes the menu item.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="menuDto">The menu dto.</param>
        /// <param name="itemRow">The item row.</param>
        private void WriteMenuItem(XmlWriter writer, MenuDto menuDto, MenuDto.MenuItemRow itemRow)
        {
            writer.WriteStartElement("Item", ""); // Start Item
            writer.WriteElementString("Name", itemRow.Text);
            writer.WriteElementString("SortOrder", itemRow.Order.ToString());
            writer.WriteElementString("IsVisible", itemRow.IsVisible.ToString());
			writer.WriteStartElement("Command", ""); // Start Command
            if (itemRow["CommandType"] != DBNull.Value)
            {   
                writer.WriteAttributeString("type", GetCommandTypeName(ConvertDBValueToStringSafe(itemRow["CommandType"]))); // TODO: get name instead
                writer.WriteString(ConvertDBValueToStringSafe(itemRow["CommandText"]));
            }
			writer.WriteEndElement(); // End Command

            writer.WriteStartElement("Resources", ""); // Start Resources
            foreach (MenuDto.MenuItem_ResourcesRow resourceRow in itemRow.Getmain_MenuItem_ResourcesRows())
            {
                writer.WriteStartElement("Resource", ""); // Start Resource
                writer.WriteAttributeString("language", GetLanguageCode(Int32.Parse(resourceRow.LanguageId.ToString())));
                writer.WriteElementString("Title", resourceRow.Text);
                writer.WriteElementString("Tooltip", resourceRow.ToolTip);
                writer.WriteEndElement();  // End Resource
            }
            writer.WriteEndElement();  // End Resources

            // Write sub elements if any
            writer.WriteStartElement("Items", ""); // Start Items
            DataView view = new DataView(menuDto.MenuItem);
			view.RowFilter = String.Format("MenuId = {0} and Outline like '{1}{2}.%' and OutlineLevel={3}", itemRow.MenuId, itemRow.Outline, itemRow.MenuItemId.ToString(), itemRow.OutlineLevel + 1);
            foreach (DataRowView itemRow2 in view)
            {
                WriteMenuItem(writer, menuDto, (MenuDto.MenuItemRow)itemRow2.Row);
            }
            writer.WriteEndElement();  // End Items
			
            writer.WriteEndElement();  // End Item
        }

		/// <summary>
		/// Writes the folder page.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="reader">The reader.</param>
		private void WriteFolderPage(XmlWriter writer, DataRow row)
		{
			if ((bool)row["IsFolder"])
				WriteFolder(writer, row);
			else
				WritePage(writer, row);
		}

		/// <summary>
		/// Writes the folder.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="reader">The reader.</param>
		private void WriteFolder(XmlWriter writer, DataRow dataRow)
		{
			int pageId = Int32.Parse(dataRow["PageId"].ToString());
			writer.WriteStartElement("Folder", ""); // Start Folder
			WriteFolderPageCommon(writer, dataRow);

			// Add sub folders and pages
			DataTable dt = FileTreeItem.LoadItemByFolderIdDT(pageId);
			if (dt.Rows.Count > 0)
			{
				foreach (DataRow row in dt.Rows)
					WriteFolderPage(writer, row);
			}

			writer.WriteEndElement();  // End Folder
		}

		/// <summary>
		/// Writes the page.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="reader">The reader.</param>
		private void WritePage(XmlWriter writer, DataRow dataRow)
		{
			int pageId = Int32.Parse(dataRow["PageId"].ToString());
			writer.WriteStartElement("Page", ""); // Start Page
			writer.WriteAttributeString("isDefault", dataRow["IsDefault"].ToString());
			writer.WriteAttributeString("masterpage", dataRow[8/*"MasterPage"*/].ToString());
			WriteFolderPageCommon(writer, dataRow);
			WritePageNavigation(writer, pageId);
			WritePageVersions(writer, pageId);
			writer.WriteEndElement();  // End Page
		}

		/// <summary>
		/// Writes the folder page ContentDataHelper.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <param name="reader">The reader.</param>
		private void WriteFolderPageCommon(XmlWriter writer, DataRow dataRow)
		{
			int pageId = Int32.Parse(dataRow["PageId"].ToString());
			writer.WriteAttributeString("name", dataRow["name"].ToString());
			writer.WriteAttributeString("isPublic", dataRow["IsPublic"].ToString());
			writer.WriteElementString("SortOrder", dataRow["Order"].ToString());
			WriteAccessRoles(writer, pageId);

			using (IDataReader reader2 = PageAttributes.GetByPageId(pageId))
			{
				if (reader2.Read())
				{
					writer.WriteElementString("Title", (string)reader2["Title"]);
					writer.WriteElementString("Keywords", (string)reader2["MetaKeys"]);
					writer.WriteElementString("Description", (string)reader2["MetaDescriptions"]);
				}
                reader2.Close();
			}
		}

        /// <summary>
        /// Writes the access roles.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="pageId">The page id.</param>
        private void WriteAccessRoles(XmlWriter writer, int pageId)
        {
            using (IDataReader reader = FileTreeItem.PageAccessGetByPageId(pageId))
            {
                string roles = String.Empty;
                while (reader.Read())
                {
                    if (String.IsNullOrEmpty(roles))
                        roles = reader["RoleId"].ToString().Trim();
                    else
                        roles += String.Format(",{0}", reader["RoleId"].ToString().Trim());
                }
                writer.WriteElementString("Roles", roles);
                reader.Close();
            }
        }

		/// <summary>
		/// Writes the NavigationCommand elements.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="pageId"></param>
		private void WritePageNavigation(XmlWriter writer, int pageId)
		{
			writer.WriteStartElement("PageNavigation", String.Empty); // Start PageNavigation
			DataTable dt = NavigationManager.GetByPageId(pageId);
			if (dt != null && dt.Rows.Count > 0)
			{
				foreach (DataRow row in dt.Rows)
				{
					writer.WriteStartElement("NavigationCommand", ""); // Start NavigationCommand
					writer.WriteAttributeString("item", GetNavigationItemName(Int32.Parse(row["ItemId"].ToString())));
					writer.WriteAttributeString("params", row["Params"].ToString());
					writer.WriteAttributeString("trigerParam", row["TrigerParam"].ToString());

                    // Write parameters
                    DataTable parameters = NavigationManager.GetParamsByItemIdDT(Int32.Parse(row["ItemId"].ToString()));
                    writer.WriteStartElement("Parameters", ""); // Start params
                    foreach (DataRow paramRow in parameters.Rows)
                    {
                        writer.WriteStartElement("Parameter", ""); // Start param
                        writer.WriteAttributeString("name", paramRow["name"].ToString());
                        writer.WriteAttributeString("value", paramRow["value"].ToString());
                        writer.WriteAttributeString("required", paramRow["IsRequired"].ToString());
                        writer.WriteEndElement(); // end param
                    }
                    writer.WriteEndElement(); // end params

					writer.WriteEndElement();
				}
			}
			writer.WriteEndElement(); // PageNavigation
		}

        /// <summary>
        /// Writes the page versions.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="pageId">The page id.</param>
        private void WritePageVersions(XmlWriter writer, int pageId)
        {
            //int pageVersion = 0;
            //bool pageVersionSet = false;
            int versionId = 0;
            PageDocument pageDocument = null;
            int menuid;

            //GET PUBLISHED VERSION
            int statusId = WorkflowStatus.GetLast();
			DataTable versionsTable = PageVersion.GetVersionByStatusIdDT(pageId, statusId);

			writer.WriteStartElement("Versions", ""); // Start Versions

			if (versionsTable != null && versionsTable.Rows.Count > 0)
			{
				foreach (DataRow versionRow in versionsTable.Rows)
				{
					writer.WriteStartElement("Version", ""); // Start Version
					writer.WriteAttributeString("version", versionRow["VersionNum"].ToString());
					writer.WriteAttributeString("language", GetLanguageCode(Int32.Parse(versionRow["LangId"].ToString())));
					writer.WriteAttributeString("template", GetTemplateName(Int32.Parse(versionRow["templateid"].ToString())));
					writer.WriteAttributeString("status", GetStatusName(Int32.Parse(versionRow["statusid"].ToString())));
					writer.WriteAttributeString("state", GetStateName(Int32.Parse(versionRow["stateid"].ToString())));
                    writer.WriteAttributeString("created", DateTime.Parse(versionRow["created"].ToString()).ToString("u"));
					writer.WriteAttributeString("createdby", GetUserName(new Guid(versionRow["creatoruid"].ToString())));
                    writer.WriteAttributeString("modified", DateTime.Parse(versionRow["edited"].ToString()).ToString("u"));
					writer.WriteAttributeString("modifiedby", GetUserName(new Guid(versionRow["editoruid"].ToString())));
					writer.WriteElementString("Comments", ConvertDBValueToStringSafe(versionRow["Comment"]));

					//pageVersionSet = false;
					int.TryParse(versionRow["VersionId"].ToString(), out versionId);

                    /*
					using (IDataReader pdreader = Database.PageDocument.GetById(pageId))
					{
						if (pdreader.Read())
						{
							int.TryParse(pdreader["PageVersionId"].ToString(), out pageVersion);
							pageVersionSet = true;
						}
						pdreader.Close();
					}
                     * */

					//if (pageVersionSet)
					{
                        pageDocument = PageDocument.Open(versionId, OpenMode.View, Guid.Empty);
						//pageDocument = PageDocument.op.PersistentDocumentStorage.Load(pageVersion, Guid.Empty);
					}

                    MapMenuIdToFriendlyName(pageDocument);
                    
                    writer.WriteStartElement("PageDocument", ""); // Start Document
					if (pageDocument != null)
					{
						byte[] pageDocBytes = Helper.Serialize(pageDocument);
						writer.WriteBase64(pageDocBytes, 0, pageDocBytes.Length);
					}
					writer.WriteEndElement();  // End Document
					writer.WriteEndElement();  // End Version
				}
			}

            writer.WriteEndElement();  // End Versions
        }

        //Fix export/Import issue with menus by using friendly name rather than menuid (since menuId is a primary key that can't be
        //used when recreating menus
        public static void MapMenuIdToFriendlyName(PageDocument pageDocument)
        {
            //translate the menu id to friendly name so that it can be mapped correctly during import
            foreach (DynamicNode node in pageDocument.DynamicNodes)
            {
                if (node.FactoryControlUID.ToString() == "08046F65-1D6C-4f5e-B409-90DD9789B2DE" ||
                    node.FactoryControlUID.ToString() == "0606DB60-C62B-4FD7-9BF9-A4D836CE8623")
                {
                    Param settings = (Param)node.Controls[0].Params;
                    TranslateMenuIdToFriendlyName(settings);
                }
            }

            //translate the static menu id to friendly name so that it can be mapped correctly during import
            if (pageDocument.StaticNode != null)
            {
                foreach (ControlSettings ctl in pageDocument.StaticNode.Controls.AllValues)
                {
                    TranslateMenuIdToFriendlyName(ctl.Params);
                }
            }
        }

        private static void TranslateMenuIdToFriendlyName(Param settings)
        {
            int menuId;

            if (settings != null)
            {
                if (settings["DataMember"] != null && settings["DataMember"].ToString() != "")
                {
                    //found a menu header control. Now set the DataMember to the menuId, translating it from the friendly name
                    //that was exported

                    if (int.TryParse(settings["DataMember"].ToString(), out menuId))
                    {
                        //now find the id for the menu with that friendly name
                        using (MenuDto menuData = MenuManager.GetMenuDto(menuId))
                        {

                            if (menuData != null && menuData.Menu != null && menuData.Menu.Count > 0)
                            {
                                settings["DataMember"] = menuData.Menu[0].FriendlyName;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
		/// Gets the name of the navigation item.
		/// </summary>
		/// <param name="itemId">The template id.</param>
		/// <returns></returns>
		private string GetNavigationItemName(int itemId)
		{
			string name = String.Empty;
			using (IDataReader reader = NavigationManager.GetItemById(itemId))
			{
				if (reader.Read())
					name = (string)reader["ItemName"];

                reader.Close();
			}

			return name;
		}

        /// <summary>
        /// Gets the language code.
        /// </summary>
        /// <param name="languageId">The language id.</param>
        /// <returns></returns>
        private string GetLanguageCode(int languageId)
        {
			string lang = String.Empty;
			using (IDataReader reader = Language.LoadLanguage(languageId))
			{
				if (reader.Read())
					lang = reader["LangName"].ToString();

				reader.Close();
			}

            return lang;
        }

        /// <summary>
        /// Get the LangId based on the Language Name passed
        /// </summary>
        /// <param name="langName">Language Name (Example: en-US)</param>
        /// <param name="addIfNotExists">Add this code if it doesnt exist</param>
        /// <returns>Returns LangId if found/Added, else -1</returns>
        private int GetLanguageId(string langName, bool addIfNotExists)
        {
			int id = -1;
			bool found = false;
			using (IDataReader reader = Language.GetLangByName(langName))
			{
				if (reader.Read())
				{
					id = (int)reader["LangId"];
					found = true;
				}
                reader.Close();
			}

			if (found)
				return id;

            if (addIfNotExists)
                id = Language.AddLanguage(langName, langName, false);
            else
                id = -1;

			return id;
        }

        /// <summary>
        /// Gets the name of the template.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        /// <returns></returns>
        private string GetTemplateName(int templateId)
        {
            TemplateDto dto = DictionaryManager.GetTemplateDto(templateId);
            if (dto.main_Templates.Count > 0)
                return dto.main_Templates[0].Name;

            return String.Empty;
        }

        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        private string GetUserName(Guid userId)
        {
            return userId.ToString();
        }

        /// <summary>
        /// Gets the name of the status.
        /// </summary>
        /// <param name="statusId">The status id.</param>
        /// <returns></returns>
        private string GetStatusName(int statusId)
        {
			string status = String.Empty;
			using (IDataReader reader = WorkflowStatus.LoadById(statusId))
			{
				if (reader.Read())
					status = reader["FriendlyName"].ToString();

				reader.Close();
			}

            return status;
        }

        /// <summary>
        /// Gets the name of the state.
        /// </summary>
        /// <param name="stateId">The state id.</param>
        /// <returns></returns>
        private string GetStateName(int stateId)
        {
			string state = String.Empty;
			using (IDataReader reader = PageState.GetById(stateId))
			{
				if (reader.Read())
					state = reader["FriendlyName"].ToString();

                reader.Close();
			}

            return state;
        }

        /// <summary>
        /// Gets the name of the command type.
        /// </summary>
        /// <param name="commandType">Type of the command.</param>
        /// <returns></returns>
        private string GetCommandTypeName(string commandType)
        {
            if (commandType == "0")
                return "None";
            else if (commandType == "1")
                return "Link";
            else if (commandType == "2")
                return "Script";
            else if (commandType == "3")
                return "Navigation";

            return String.Empty;
        }

        /// <summary>
        /// Converts the DB value to string safe.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        private string ConvertDBValueToStringSafe(object val)
        {
            if (val != DBNull.Value)
                return val.ToString();

            return String.Empty;
        }
        #endregion

        #endregion

        #region Import Site Function

        /// <summary>
        /// Imports the site.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="appId">The app id.</param>
        /// <param name="siteId">The site id.</param>
        /// <param name="createFromTemplate">if set to <c>true</c>, then site will be created from xml template.</param>
        /// <returns>IDs of imported sites.</returns>
        public Guid[] ImportSite(Stream input, Guid appId, Guid siteId, bool createFromTemplate)
        {
			int totalSteps = GetTotalImportStepsCount();

			List<Guid> importedSiteIds = new List<Guid>();

			OnEvent("Starting import...", 0);

            XmlReader reader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;

            using (reader = XmlReader.Create(input, settings))
            {
				// move down to the root node we need to work with
                // skip declaration element
                reader.Read(); //this moves the reader to the first element, which may be a declaration element
                if (reader.LocalName != "Sites")
                    reader.ReadToFollowing("Sites");

                while (reader.Read() && reader.LocalName != "Sites" && reader.NodeType != XmlNodeType.EndElement)
                {
					OnEvent("Started importing site", GetProgressPercent((int)ImportSteps.StartImportSite, totalSteps));
                    // now we're at the site level - loop through the site elements
					importedSiteIds.Add(ProcessSite(reader, appId, siteId, createFromTemplate));
					OnEvent("Finished importing site", GetProgressPercent((int)ImportSteps.EndImportSite, totalSteps));
                }
            }

			OnEvent("Import successfully finished.", 100);

			return importedSiteIds.ToArray();
        }

        #region Helper Functions

        /// <summary>
        /// Gets the site IDs.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="appId">The app id.</param>
        /// <returns>IDs of the sites.</returns>
        public Guid GetSiteID(Stream input, Guid appId)
        {
            List<Guid> importedSiteIds = new List<Guid>();

            XmlReader reader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;

            using (reader = XmlReader.Create(input, settings))
            {
                // skip declaration element
                reader.Read(); //this moves the reader to the first element, which may be a declaration element
                if (reader.LocalName != "Sites")
                    reader.ReadToFollowing("Sites");

                while (reader.Read() && reader.LocalName != "Sites" && reader.NodeType != XmlNodeType.EndElement)
                {
                    return new Guid(ReadAttributeValue(reader, "id", String.Empty));
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Processes the site.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="appId">The app id.</param>
        /// <param name="targetSiteId">The target site Id.</param>
		/// <param name="createFromTemplate">if set to <c>true</c>, then site will be created from xml template.</param>
        private Guid ProcessSite(XmlReader reader, Guid appId, Guid targetSiteId, bool createFromTemplate)
        {
			int totalSteps = GetTotalImportStepsCount();

            SiteDto siteDto = null; //create dto references to map data to
            SiteDto.SiteRow row; //row to map site data to
            Guid siteId; //for siteId
            Guid sourceSiteId;
            bool siteBool;
            Mediachase.Cms.Data.SiteAdmin admin = null;
            IDataReader fileReader;
            IDataReader accessReader;
            int fileId;
            int accessId;
            int menuItemId;
            int menuId;

            //first process the site-level attributes
            sourceSiteId = new Guid(ReadAttributeValue(reader, "id", String.Empty));

            if (targetSiteId.Equals(Guid.Empty) || createFromTemplate)
                siteId = Guid.NewGuid(); //TODO: Add Default values
            else
                siteId = targetSiteId;

            // Check if site has been imported already, if not, use the exact same id
            if (sourceSiteId != Guid.Empty)
            {
                siteId = sourceSiteId;
                siteDto = CMSContext.Current.GetSiteDto(sourceSiteId, true);
                if (siteDto.Site.Count > 0)
                {
                    siteId = Guid.NewGuid();
                }
            }

            //create site row and start mapping data to it
            if (targetSiteId.Equals(Guid.Empty))
                siteDto = new SiteDto();
            else
            {
                siteDto = CMSContext.Current.GetSiteDto(targetSiteId, true);
                if (siteDto.Site.Count == 0)
                {
                    targetSiteId = Guid.Empty;
                    siteId = Guid.NewGuid();
                }
            }

            // One last chance to create non empty site id
            if(siteId == Guid.Empty)
                siteId = Guid.NewGuid();

            row = siteDto.Site.NewSiteRow();
            SetSiteItemRowDefaults(row); //Set site default values
            row.SiteId = siteId;
            row.ApplicationId = appId;

			OnEvent("Retrieved site id: " + row.SiteId, GetProgressPercent((int)ImportSteps.StartImportSite, totalSteps));

			#region Handle overwriteExisting=true
			if (!createFromTemplate)
			{
				// check if site already exists; delete if it is to be overwritten & otherwise throw error to prevent
				//two sites with the same site id to exist in the system
                if (!targetSiteId.Equals(Guid.Empty))
				{
					OnEvent("Overwriting existing site with id=" + row.SiteId, GetProgressPercent((int)ImportSteps.StartImportSite, totalSteps));
					if (siteDto.Site.Count > 0)
					{
						siteDto.Site.Rows[0].Delete();
						// no need to delete global variables since they are deleted by cascade deleting when site row is deleted
						//foreach (SiteDto.main_GlobalVariablesRow globalRow in siteDto.main_GlobalVariables)
						//    globalRow.Delete();

						admin = new Mediachase.Cms.Data.SiteAdmin(siteDto);

						//Save method handles above deletes to the Site and main_GlobalVariables tables
						admin.Save();
					}

					//delete the page attribute rows
                    using (fileReader = FileTreeItem.GetFileTreeItemAll(targetSiteId))
					{
						if (!fileReader.IsClosed)
						{
							while (fileReader.Read())
							{
								accessId = 0;

								if (fileReader["IsFolder"].ToString() != "True")
								{
									//try to delete the page attributes, if it has an attributes row
									int.TryParse(fileReader["PageId"].ToString(), out fileId);
									PageAttributes.DeleteByPageId(fileId);
								}

								//delete the page access rows
								int.TryParse(fileReader["PageId"].ToString(), out fileId);
								using (accessReader = FileTreeItem.PageAccessGetByPageId(fileId))
								{
									while (accessReader.Read())
									{
										int.TryParse(accessReader["PageAccessId"].ToString(), out accessId);
										FileTreeItem.DeletePageAccess(accessId);
									}

                                    accessReader.Close();
								}

								//delete the page version rows
								using (accessReader = PageVersion.GetVersionByPageId(fileId))
								{
									while (accessReader.Read())
									{
										int.TryParse(accessReader["VersionId"].ToString(), out accessId);
										PageVersion.DeletePageVersion(accessId);
									}
                                    accessReader.Close();
								}

								//finally, delete the folder hierarchy info
								FileTreeItem.DeleteFileItem(fileId);
							}
						}

                        fileReader.Close();
					}

					//delete all menu rows for this site
					List<int> menuIds = new List<int>();
                    using (DataTable dt = MenuItem.LoadAllDT(targetSiteId, 0))
					{
						for (int i = 0; i < dt.Rows.Count; i++)
						{

							int.TryParse(dt.Rows[i]["MenuId"].ToString(), out menuId);
							if (!menuIds.Exists(delegate(int containsId)
							{
								return containsId == menuId;
							}))
								menuIds.Add(menuId);

							int.TryParse(dt.Rows[i]["MenuItemId"].ToString(), out menuItemId);
							MenuItem.Delete(menuItemId);
						}
					}

					//also delete the top menu row
					for (int i = 0; i < menuIds.Count; i++)
						Menu.Delete(menuIds[i]);
				}
			}
			#endregion

			//Retrieve site-level values
            bool.TryParse(ReadAttributeValue(reader, "isDefault", "true"), out siteBool); //TODO: Add Default values
            row.IsDefault = siteBool;

            bool.TryParse(ReadAttributeValue(reader, "isActive", "false"), out siteBool); //TODO: Add Default values
            row.IsActive = siteBool;

            row.Folder = ReadAttributeValue(reader, "Folder", String.Empty);

            //move to first descendant node
            reader.Read();

            siteDto.EnforceConstraints = false; //prevent relationship errors if row order insert is incorrect
            siteDto.Site.AddSiteRow(row);

            //iterate through the site xml and populate the related objects
            while (ProcessNextSiteNode(reader, siteDto, row))
            {
            };//do nothing

            siteDto.EnforceConstraints = true;

            //now save the site information
            if (admin == null)
                admin = new Mediachase.Cms.Data.SiteAdmin(siteDto);
            admin.Save();

			return siteId;
        }

        //A generic location to hand off the reader to find out where to goto next; 
        //assumes no particular order in the xml
        /// <summary>
        /// Processes the next site node.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="dto">The dto.</param>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        private bool ProcessNextSiteNode(XmlReader reader, SiteDto dto, SiteDto.SiteRow row)
        {
			int totalSteps = GetTotalImportStepsCount();

            if (reader.EOF)
                return false;

            string nodeName = reader.LocalName;

            switch (nodeName)
            {
                case "Name":
                    string name = ReadElementValue(reader, String.Empty); //TODO: Add Default values
                    row.Name = name;
                    break;
                case "Description":
                    string description = ReadElementValue(reader, String.Empty); //TODO: Add Default values
                    row.Description = description;
                    break;
                case "Domains":
					OnEvent("Started adding domain info for the site", GetProgressPercent((int)ImportSteps.StartImportDomainInfo, totalSteps));
                    AddDomainInfo(reader, row);
					OnEvent("Finished adding domain info for the site", GetProgressPercent((int)ImportSteps.EndImportDomainInfo, totalSteps));
                    break;
				case "Languages":
					OnEvent("Started adding languages for the site", GetProgressPercent((int)ImportSteps.StartImportLanguages, totalSteps));
					AddLanguages(reader, dto, row.SiteId, row);
					OnEvent("Finished adding languages for the site", GetProgressPercent((int)ImportSteps.EndImportLanguages, totalSteps));
					break;
                case "DisplayTemplates":
					if (reader.NodeType != XmlNodeType.EndElement)
					{
						OnEvent("Started adding templates for the site", GetProgressPercent((int)ImportSteps.StartImportTemplates, totalSteps));
						AddDisplayTemplates(reader, row.SiteId);
						OnEvent("Finished adding templates for the site", GetProgressPercent((int)ImportSteps.EndImportTemplates, totalSteps));
					}
					else
						reader.Read();
                    break;
                case "Attributes":
					OnEvent("Started adding attributes for the site", GetProgressPercent((int)ImportSteps.StartImportAttributes, totalSteps));
                    AddAttributes(reader, dto, row.SiteId, row);
					OnEvent("Finished adding attributes for the site", GetProgressPercent((int)ImportSteps.EndImportAttributes, totalSteps));
                    break;
                case "Site":
                    //end of the site info reached
                    return false;
                case "Menus":
					if (reader.NodeType != XmlNodeType.EndElement)
					{
						OnEvent("Started adding menus for the site", GetProgressPercent((int)ImportSteps.StartImportMenus, totalSteps));
						AddMenus(reader, row.SiteId);
						OnEvent("Finished adding menus for the site", GetProgressPercent((int)ImportSteps.EndImportMenus, totalSteps));
					}
					else
						reader.Read();
                    break;
                case "Folders":
					OnEvent("Started adding folders for the site", GetProgressPercent((int)ImportSteps.StartImportFolders, totalSteps));
                    AddFolders(reader, row.SiteId);
					OnEvent("Finished adding folders for the site", GetProgressPercent((int)ImportSteps.EndImportFolders, totalSteps));
                    break;
                default:
                    //go through any unrecognized elements and ignore
                    reader.Read();
                    break;
            }

            return true;
        }

        /// <summary>
		/// Read through each domain and collect info to be stored in the Site.Domain db field as a comma-delimited list.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="row">The row.</param>
        private void AddDomainInfo(XmlReader reader, SiteDto.SiteRow row)
        {
			string domainName;
            string domainList = String.Empty;

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                domainName = ReadElementValue(reader, String.Empty);
                if (domainName != String.Empty)
                    if (domainList == String.Empty) //only add a column before the second and subsequent domains
                        domainList = domainName;
                    else
                        domainList += "," + domainName;
            }

            if (domainList != String.Empty)
                row.Domain = domainList;

            //read through end element
            reader.Read();
        }

		/// <summary>
		/// Adds the site languages.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="dto">The dto.</param>
		/// <param name="siteId">The site id.</param>
		/// <param name="siteRow">The site row.</param>
		private void AddLanguages(XmlReader reader, SiteDto dto, Guid siteId, SiteDto.SiteRow siteRow)
		{	
			SiteDto.SiteLanguageRow langRow;
			string languageCode;

			//move to the first language
			reader.Read();

			//read each element into separate rows; when end element reached (/Languages), exit
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				languageCode = ReadElementValue(reader, String.Empty);
				if (!String.IsNullOrEmpty(languageCode))
				{
					langRow = dto.SiteLanguage.NewSiteLanguageRow();
					langRow.SiteId = siteId;
					langRow.LanguageCode = languageCode;
					langRow.SetParentRow(siteRow);
					dto.SiteLanguage.AddSiteLanguageRow(langRow);
				}
			}

			//move past the Languages end element
			reader.Read();
		}

        /// <summary>
        /// Adds the display templates.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="siteId">The site id.</param>
        private void AddDisplayTemplates(XmlReader reader, Guid siteId)
		{
			if (reader.NodeType == XmlNodeType.Element)
            {	
                XmlReader subtreeReader = reader.ReadSubtree();
                TemplateDto templates = DictionaryManager.GetTemplateDto();
                while (subtreeReader.Read() && !subtreeReader.EOF)
                {
                    if (subtreeReader.IsStartElement("Template"))
                    {
                        string name = ReadAttributeValue(reader, "name", String.Empty);
                        string friendlyname = ReadAttributeValue(reader, "friendlyname", String.Empty);
                        string path = ReadAttributeValue(reader, "path", String.Empty);
                        string type = ReadAttributeValue(reader, "type", String.Empty);
                        string lang = ReadAttributeValue(reader, "lang", String.Empty);

                        // try finding one that already exists
                        DataView view = templates.main_Templates.DefaultView;
                        view.RowFilter = String.Format("name = '{0}' and TemplateType = '{1}'", name, type);

                        TemplateDto.main_TemplatesRow row = null;
                        if (view.Count > 0)
                        {
                            row = templates.main_Templates.FindByTemplateId((int)view[0]["TemplateId"]);
                        }
                        else
                        {
                            row = templates.main_Templates.Newmain_TemplatesRow();
                        }

                        row.Name = name;
                        row.FriendlyName = friendlyname;
                        row.ApplicationId = CmsConfiguration.Instance.ApplicationId;
                        row.Path = path;
                        row.TemplateType = type;
                        row.LanguageCode = lang;

                        if (row.RowState == DataRowState.Detached)
                        {
                            templates.main_Templates.Rows.Add(row);
                        }
                    }
                }

                DictionaryManager.SaveTemplateDto(templates);
            }
		}
        
        /// <summary>
        /// Adds the attributes.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="dto">The dto.</param>
        /// <param name="siteId">The site id.</param>
        /// <param name="siteRow">The site row.</param>
        private void AddAttributes(XmlReader reader, SiteDto dto, Guid siteId, SiteDto.SiteRow siteRow)
        {
            SiteDto.main_GlobalVariablesRow row = null;
            string attributeName;
            string attributeValue;

            //move to first attribute
            reader.Read();

            //read each element into separate rows; when end element reached (/Attributes), exit
            while (reader.NodeType != XmlNodeType.EndElement)
            {
				row = null;

                attributeName = ReadAttributeValue(reader, "name", String.Empty);
                if (attributeName != String.Empty)
                {
                    DataView view = dto.main_GlobalVariables.DefaultView;
                    view.RowFilter = String.Format("KEY = '{0}'", attributeName);
                    
                    if (view.Count > 0)
                    {
                        foreach(SiteDto.main_GlobalVariablesRow variableRow in dto.main_GlobalVariables)
                        {
                            if(variableRow.KEY == (string)view[0]["KEY"])
                                row = variableRow;
                        }
                    }

                    if(row == null)
                    {
                        row = dto.main_GlobalVariables.Newmain_GlobalVariablesRow();
                    }

                    attributeValue = ReadElementValue(reader, String.Empty);
                    row.KEY = attributeName;
                    row.VALUE = attributeValue;
                    row.SiteId = siteId;                    

                    if (row.RowState == DataRowState.Detached)
                    {
                        row.SetParentRow(siteRow);
                        dto.main_GlobalVariables.Addmain_GlobalVariablesRow(row);
                    }
                }
                else
                    reader.Read();
            }

            //move past the Attributes end element
            reader.Read();
        }

        /// <summary>
        /// Adds the menus.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="siteId">The site id.</param>
        private void AddMenus(XmlReader reader, Guid siteId)
        {
            //move to the first menu
            //reader.Read();

            //add each menu
			if (reader.NodeType == XmlNodeType.Element)
			{
				XmlReader subtreeReader = reader.ReadSubtree();
				while (subtreeReader.Read() && !subtreeReader.EOF)
				{
					if (subtreeReader.IsStartElement("Menu"))
					{
						IsFirstCall = true;
						AddMenu(subtreeReader, siteId);
					}
				}
			}
			//while (AddMenu(reader, siteId))
			//{
			//}; //Do nothing
        }

        /// <summary>
        /// Adds the menu.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="siteId">The site id.</param>
        /// <returns></returns>
        private bool AddMenu(XmlReader reader, Guid siteId)
        {
			//reader.Read();

            MenuDto menuDto = new MenuDto();
            MenuDto.MenuRow row = menuDto.Menu.NewMenuRow();
            int menuId;

            row.SiteId = siteId;
            row.FriendlyName = ReadAttributeValue(reader, "name", String.Empty);
            if (row.FriendlyName == "")
                return false; //if no name is found, its assumed that the menu is invalid

            //Add menu to the dto
            menuDto.Menu.AddMenuRow(row);

            //save the menu to the database so the menuId can be generated and used as the foreign key value
            //in the menu item rows
            menuId = Mediachase.Cms.Menu.Add(row.FriendlyName, row.SiteId);

            //default starting level is always 0
            AddMenuItems(reader, menuDto, menuId, 0, null, siteId);

            return false;
        }

        /// <summary>
        /// Adds the menu items.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="dto">The dto.</param>
        /// <param name="menuId">The menu id.</param>
        /// <param name="level">The level.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="siteId">The site id.</param>
        void AddMenuItems(XmlReader reader, MenuDto dto, int menuId, int level, int? parentId, Guid siteId)
        {
            // We're not interested in storing the menuitem into the dto since we are saving the menu item directly to the database
            MenuDto.MenuItemRow menuRow = null;

            // Take off Constraints
            // We are adding Resources with no MenuItem and need this to ignore that fact
            dto.EnforceConstraints = false;
			            
            int parsedInt;
            bool parsedBool;
            bool isSaved = false;

            while (reader.Read())
            {
                // Check if we are done
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals("Menu", StringComparison.InvariantCulture))
                    break;

                // Check if we are at the end of the Item so we can save it
                // if it has not already been saved
                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName == "Item")
                {
                    if (menuRow != null)
                        if (!isSaved) // hasn't been saved yet
                        {
                            menuRow.MenuId = menuId;
                            int rootId = -1;

                            if (!IsFirstCall)
                                rootId = Mediachase.Cms.MenuItem.Add(menuRow.MenuId, menuRow.Text, menuRow.CommandText, menuRow.CommandType, menuRow.IsVisible, menuRow.Order);
                            else
                            {
                                rootId = menuRow.MenuItemId;
                                Mediachase.Cms.MenuItem.Update(menuRow.MenuItemId, menuRow.Text, menuRow.CommandText, menuRow.CommandType, menuRow.IsVisible, menuRow.Order);
                                IsFirstCall = false;
                            }

                            // If has ParentId then move this menu item to its parent
                            if (parentId.HasValue)
                                Mediachase.Cms.MenuItem.MoveTo(rootId, (int)parentId.Value);

                            // Save the Resources
                            foreach (MenuDto.MenuItem_ResourcesRow resRow in dto.MenuItem_Resources.Rows)
                            {
                                Mediachase.Cms.MenuItem.AddResource(rootId, resRow.LanguageId, resRow.Text, resRow.ToolTip);
                            }

                            // Clear out the Resources
                            dto.MenuItem_Resources.Clear();

                            menuRow = null;
                            isSaved = true;
                        }
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals("Items", StringComparison.InvariantCulture))
                    break; // leave, we are finished with this level

                // Check if we are beginning a new Item
                if (reader.IsStartElement("Item", ""))
                {
                    if (!IsFirstCall)
                        menuRow = dto.MenuItem.NewMenuItemRow();
                    else
                    {
                        // We need to get the Root Row that was created in the stored procedure
                        int MainMenuId = GetRootItem(siteId, menuId);
                        menuRow = dto.MenuItem.NewMenuItemRow();

                        if (MainMenuId > -1)
                        {
                            IDataReader dataReader = Mediachase.Cms.MenuItem.LoadById(MainMenuId);

                            while (dataReader.Read())
                            {
                                menuRow.MenuItemId = MainMenuId;
                                menuRow.Text = (string)dataReader["Text"];
                                menuRow.IsRoot = true;
                                
                                break;
                            }

                            dataReader.Close();
                        }
                    }
                    SetMenuItemRowDefaults(level, menuRow);
                    isSaved = false;
                    reader.Read(); // move to next element
                }

                if (reader.IsStartElement("Name", ""))
                    menuRow.Text = reader.ReadElementString();

                if (reader.IsStartElement("SortOrder", ""))
                {
                    int.TryParse(reader.ReadElementString(), out parsedInt);
                    menuRow.Order = parsedInt;
                }

                if (reader.IsStartElement("IsVisible", ""))
                {
                    bool.TryParse(reader.ReadElementString(), out parsedBool);
                    menuRow.IsVisible = parsedBool;
                }

                if (reader.IsStartElement("Command", ""))
                {
                    menuRow.CommandType = GetCommandTypeNumber(reader);
					menuRow.CommandText = reader.ReadElementString(); // move reader to next element
                }

                // Resources
                if (reader.IsStartElement("Resources", "") && !reader.IsEmptyElement)
                {
                    MenuDto.MenuItem_ResourcesRow resources = null;
                    while (reader.Read())
                    {
                        if (reader.IsStartElement("Resource", ""))
                        {
                            resources = dto.MenuItem_Resources.NewMenuItem_ResourcesRow();
							resources.LanguageId = GetLanguageId(ReadAttributeValue(reader, "language", GetDefaultImportLanguage()), true);
                            resources.MenuItemId = menuRow.MenuItemId;
                            reader.Read(); // move to next element
                        }

                        if (reader.IsStartElement("Title", ""))
                        {
                            resources.Text = reader.ReadElementString();
                        }

                        if (reader.IsStartElement("Tooltip", ""))
                        {
                            resources.ToolTip = reader.ReadElementString();
                        }

                        // Check if we are at the end, need to add to dto
                        if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals("Resource", StringComparison.InvariantCulture))
                        {
                            dto.MenuItem_Resources.AddMenuItem_ResourcesRow(resources);
                        }

                        if (reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals("Resources", StringComparison.InvariantCulture))
                            break; // we are at the end of Resources
                    }
                }


                // Check if there are nested menu items in the current item
                if (reader.IsStartElement("Items", "") && !reader.IsEmptyElement && menuRow != null)
                {

                    menuRow.MenuId = menuId;
                    // Save this menu item
                    int rootId = -1;

                    if (!IsFirstCall)
                        rootId = Mediachase.Cms.MenuItem.Add(menuRow.MenuId, menuRow.Text, menuRow.CommandText, menuRow.CommandType, menuRow.IsVisible, menuRow.Order);
                    else
                    {
                        rootId = menuRow.MenuItemId;
                        Mediachase.Cms.MenuItem.Update(menuRow.MenuItemId, menuRow.Text, menuRow.CommandText, menuRow.CommandType, menuRow.IsVisible, menuRow.Order);
                        IsFirstCall = false;
                    }

                    // Save the Resources
                    foreach (MenuDto.MenuItem_ResourcesRow resRow in dto.MenuItem_Resources.Rows)
                        Mediachase.Cms.MenuItem.AddResource(rootId, resRow.LanguageId, resRow.Text, resRow.ToolTip);

                    // Clear out the Resources
                    dto.MenuItem_Resources.Clear();

                    // Set our flag that this node was saved
                    isSaved = true;

                    // If has ParentId then move this menu item to its parent
                    if (parentId.HasValue)
                        Mediachase.Cms.MenuItem.MoveTo(rootId, (int)parentId.Value);
					
                    // Drill into the next level
                    AddMenuItems(reader, new MenuDto(), menuId, level + 1, rootId, siteId);
                }
            }
        }

        /// <summary>
        /// Gets the root item.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="menuId">The menu id.</param>
        /// <returns></returns>
        int GetRootItem(Guid siteId, int menuId)
        {
			int retVal = -1;
			using (IDataReader reader = Mediachase.Cms.MenuItem.LoadAllRoot(siteId))
			{
				while (reader.Read())
				{
					if ((bool)reader["IsRoot"] == true)
						if (reader["CommandType"] == DBNull.Value /*&& String.Compare((string)reader["Text"], "TopMenu", true) == 0*/ && (int)reader["MenuId"] == menuId)
						{
							retVal = (int)reader["MenuItemId"];
							break;
						}
				}

                reader.Close();
			}
            return retVal;
        }
		        
        /// <summary>
        /// Sets the site item row defaults.
        /// </summary>
        /// <param name="row">The row.</param>
        private static void SetSiteItemRowDefaults(SiteDto.SiteRow row)
        {
			//TODO: Review default values
            row.Description = String.Empty;
            row.Domain = String.Empty;
            row.Folder = String.Empty;
        }

        /// <summary>
        /// Sets the menu item row defaults.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="row">The row.</param>
        private static void SetMenuItemRowDefaults(int level, MenuDto.MenuItemRow row)
        {
            //handle outline level and isroot properties. 0 level is root and 0 outline level
            //1 level and higher is not root and the same number outline level
            //Typically the root level is not visible
            if (level == 0)
            {
                row.IsRoot = true;
                row.OutlineLevel = level;
                row.Outline = ".";
                row.IsVisible = false;
            }
            else
            {
                row.IsRoot = false;
                row.OutlineLevel = level;
                row.Outline = String.Empty; //TODO: Get algorithm for populating this field
                row.IsVisible = true;
                row.IsInherits = false;
            }

            row.CommandType = 0;
            row.CommandText = "";
            row.LeftImageUrl = String.Empty;
            row.RightImageUrl = String.Empty;
			row.Order = 0;
        }

        /// <summary>
        /// The expected format for the root of the folders xml is :
        /// <Folders>
        ///    <Folder name="Root" isPublic="True">...
        /// There should only be one root folder
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="siteId"></param>
        private void AddFolders(XmlReader reader, Guid siteId)
        {
			int totalSteps = GetTotalImportStepsCount();

            Guid id = Guid.NewGuid();

            //move to the first menu
            reader.Read();

            //add the root folder
            if (reader.LocalName == "Folder")
            {
                //create generic to hold rows of page/folder data
                List<CMSCommon> rows = new List<CMSCommon>();

				//add each folder to List<CMSCommon> (read from xml)
                while (AddFolder(reader, siteId, rows, 0, id))
                {
                }; //Do nothing

                //first add them to the database
                //Add the root first
                FileTreeItem.GetRoot(siteId); //the sp associated with this method creates a root if one isn't found
                foreach (CMSCommon row in rows)
                    row.Save();

                //now move them to their hierarchical position
				OnEvent("Creating folder hierarchy", GetProgressPercent((int)ImportSteps.StartImportFolders, totalSteps));
                CreateFolderHierarchy(rows);

                //now add the page attributes
				OnEvent("Adding page attributes", GetProgressPercent((int)ImportSteps.StartImportFolders, totalSteps));
                AddPageAttributes(rows);

                //now add the access rights
				OnEvent("Adding access rights", GetProgressPercent((int)ImportSteps.StartImportFolders, totalSteps));
                AddAccess(rows);

				//add page commands
				OnEvent("Adding page commands", GetProgressPercent((int)ImportSteps.StartImportFolders, totalSteps));
				AddPageNavigation(rows);

                //add page version info
				OnEvent("Adding page versions", GetProgressPercent((int)ImportSteps.StartImportFolders, totalSteps));
                AddPageVersions(rows, siteId);
            }
            else
            {
                //skip adding folders. There are either no folders or incorrect structure. Read until end folders element
                if (reader.LocalName != "Folders")
                    reader.ReadToFollowing("Folders");
            }
        }

        /// <summary>
        /// Creates the folder hierarchy.
        /// </summary>
        /// <param name="rows">The rows.</param>
        private void CreateFolderHierarchy(List<CMSCommon> rows)
        {
            int maxOutlineLevel = 0;

            //get max outline level
            foreach (CMSCommon row in rows)
            {
                if (maxOutlineLevel < row._outlineLevel)
                    maxOutlineLevel = row._outlineLevel;
            }

            //By default, the outline level for roots is 0; all other nodes are defaulted to 1 when inserted. So the only outline
            //level that needs to be set is 2 and higher
            //the outline levels are added in order to prevent confustion in setting the sortOrder
            for (int i = 2; i <= maxOutlineLevel; i++)
            {
                foreach (CMSCommon rw in rows)
                {
                    if (rw._outlineLevel == i)
                    {
                        FileTreeItem.MoveTo(rw._id, GetParentId(rows, rw._parentId));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the parent id.
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        private int GetParentId(List<CMSCommon> rows, Guid parentId)
        {
            foreach (CMSCommon row in rows)
            {
                if (row._internalId == parentId)
                    return row._id;
            }

            return 0; //defaults to root
        }

        /// <summary>
        /// Adds the page attributes.
        /// </summary>
        /// <param name="rows">The rows.</param>
        private void AddPageAttributes(List<CMSCommon> rows)
        {
            CMSPage page;

            foreach (CMSCommon row in rows)
            {
                if (!row._isFolder)
                {
                    page = (CMSPage)row;

                    //only add a row if there is a title
                    if (page._title != null && page._title != "")
                    {
                        PageAttributes.Add(page._id, page._title, page._keywords, page._description);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the access.
        /// </summary>
        /// <param name="rows">The rows.</param>
        private void AddAccess(List<CMSCommon> rows)
        {
            foreach (CMSCommon row in rows)
            {
                if (row._roles != null)
                {
                    for (int i = 0; i < row._roles.Length; i++)
                    {
                        FileTreeItem.AddPageAccess(row._roles[i], row._id);
                    }
                }
            }
        }

		/// <summary>
		/// Adds the page commands.
		/// </summary>
		/// <param name="rows">The rows.</param>
		private void AddPageNavigation(List<CMSCommon> rows)
		{
			CMSPage page;

			foreach (CMSCommon row in rows)
			{
				if (!row._isFolder)
				{
					page = (CMSPage)row;

					if (page._commandList != null)
					{
						foreach (CMSNavigationCommand cmd in page._commandList)
						{
							cmd._id = NavigationManager.NewCommand(page._id.ToString(), cmd._itemId, cmd._params, cmd._trigerParam);

                            DataTable paramTable = NavigationManager.GetParamsByItemIdDT(cmd._itemId);
                            
                            foreach (CMSNavigationCommandParameter parameter in cmd._parameters)
                            {
                                DataRow[] paramRows = paramTable.Select(String.Format("Name = '{0}'", parameter._name));
                                if(paramRows.Length > 0)
                                {
                                    if(paramRows[0]["Id"] != DBNull.Value)
                                        parameter._id = (int)paramRows[0]["Id"];
                                }
                                else
                                {
                                    parameter._id = NavigationManager.NewParam(cmd._itemId, parameter._name, parameter._value, parameter._required);
                                    paramTable = NavigationManager.GetParamsByItemIdDT(cmd._itemId);
                                }
                            }
						}
					}
				}
			}
		}

        /// <summary>
        /// Adds the page versions.
        /// </summary>
        /// <param name="rows">The rows.</param>
        private void AddPageVersions(List<CMSCommon> rows, Guid SiteId)
        {
            CMSPage page;
            int versionId;

            foreach (CMSCommon row in rows)
            {
                if (!row._isFolder)
                {
                    page = (CMSPage)row;

                    if (page._versionList != null)
                    {
                        foreach (CMSVersion version in page._versionList)
                        {
                            versionId = PageVersion.AddPageVersion2(page._id, version._templateId, version._versionNum, version._languageId,
													   version._statusId, version._createdBy, version._createdDate, version._modifiedBy, version._modifiedDate, version._stateId, version._comments);

                            //translate the friendly menu names to ids
                            MapMenuFriendlyNameToMenuId(SiteId, version._doc);

                            try
                            {
                                PageDocument.PersistentDocumentStorage = new SqlPageDocumentStorageProvider();
                                PageDocument.PersistentDocumentStorage.Save(version._doc, versionId, Guid.NewGuid());
                            }
                            catch (Exception e)
                            {
                                Console.Write(e.StackTrace);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Map the values of menu using friendlyname and menuId. MenuIds can't be stored in export file because they are primary keys that
        /// can't be re-inserted; friendly names are used instead
        /// </summary>
        /// <param name="SiteId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static Guid MapMenuFriendlyNameToMenuId(Guid SiteId, PageDocument doc)
        {
            foreach (DynamicNode node in doc.DynamicNodes)
            {
                if (node.FactoryControlUID.ToString() == "08046F65-1D6C-4f5e-B409-90DD9789B2DE" ||
                    node.FactoryControlUID.ToString() == "0606DB60-C62B-4FD7-9BF9-A4D836CE8623")
                {
                    Param settings = (Param)node.Controls[0].Params;
                    TranslateMenuFriendlyNametoMenuId(settings, SiteId);
                }
            }

            foreach (ControlSettings ctl in doc.StaticNode.Controls.AllValues)
            {
                TranslateMenuFriendlyNametoMenuId(ctl.Params, SiteId);
            }
            return SiteId;
        }

        public static void TranslateMenuFriendlyNametoMenuId(Param settings, Guid SiteId)
        {
            bool found = false;
            if (settings != null)
            {
                if (settings["DataMember"] != null && settings["DataMember"].ToString() != "")
                {
                    //found a menu header control. Now set the DataMember to the menuId, translating it from the friendly name
                    //that was exported
                    string menuName = settings["DataMember"].ToString();

                    //now find the id for the menu with that friendly name
                    using (MenuDto menuData = MenuManager.GetMenuDto(SiteId))
                    {
                        if (menuData != null && menuData.Menu != null && menuData.Menu.Count > 0)
                        {
                            foreach (DataRow menuRow in menuData.Menu.Rows)
                            {
                                if (menuRow["FriendlyName"].ToString() == menuName)
                                {
                                    settings["DataMember"] = menuRow["MenuId"].ToString();
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                settings["DataMember"] = menuData.Menu[0]["MenuId"].ToString();
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Adds the folder page.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="siteId">The site id.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="outlineLevel">The outline level.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        private bool AddFolderPage(XmlReader reader, Guid siteId, List<CMSCommon> rows, int outlineLevel, Guid parentId)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
            {
                reader.Read();
                return false; //end of folders reached
            }
            else if (reader.LocalName == "Folders")
            {
                reader.Read(); //advance to the first item and reparse
                AddFolderPage(reader, siteId, rows, outlineLevel, parentId);
            }

            if (reader.LocalName == "Folder")
            {
                while (AddFolder(reader, siteId, rows, outlineLevel, parentId))
                {
                };
            }
            else if (reader.LocalName == "Page")
            {
                while (AddPage(reader, siteId, rows, outlineLevel, parentId))
                    ;
            }
            else
                return false; // error in algorithm or xml structure

            return true;
        }

        /// <summary>
        /// Returns whether if there are sibling elements will to process
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="siteId">The site id.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="outlineLevel">The outline level.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        private bool AddFolder(XmlReader reader, Guid siteId, List<CMSCommon> rows, int outlineLevel, Guid parentId)
        {
            XmlReader subReader;
            CMSFolder folder = new CMSFolder();

            //just get the subnodes related to this folder
            subReader = reader.ReadSubtree();
            subReader.Read();

            //handle the root attributes
            AddFolderPageCommon(subReader, folder, siteId, outlineLevel, parentId);

            //now add values from sub elements
            while (ProcessNextFolderPageProperty(subReader, folder, rows, outlineLevel))
            {
            };

            //now save the data to the generic
            rows.Add(folder);

            if (reader.NodeType == XmlNodeType.EndElement || reader.EOF)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Returns whether if there are sibling elements will to process
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="siteId">The site id.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="outlineLevel">The outline level.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        private bool AddPage(XmlReader reader, Guid siteId, List<CMSCommon> rows, int outlineLevel, Guid parentId)
        {
            XmlReader subReader;
            CMSPage page = new CMSPage();

            //just get the subnodes related to this page
            subReader = reader.ReadSubtree();
            subReader.Read();

            //handle the root attributes
			AddPageAttributes(reader, page);
            AddFolderPageCommon(subReader, page, siteId, outlineLevel, parentId);

            //now add values from sub elements
            while (ProcessNextFolderPageProperty(subReader, page, rows, outlineLevel))
            {
            };

            //now save the data to the generic list
            rows.Add(page);

            //default is to continue until </ Page> reached
            if (reader.NodeType == XmlNodeType.EndElement || reader.EOF)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Adds the folder page ContentDataHelper.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="folderPage">The folder page.</param>
        /// <param name="siteId">The site id.</param>
        /// <param name="outlineLevel">The outline level.</param>
        /// <param name="parentId">The parent id.</param>
        private void AddFolderPageCommon(XmlReader reader, CMSCommon folderPage, Guid siteId, int outlineLevel, Guid parentId)
        {
            folderPage._name = ReadAttributeValue(reader, "name", "");
            bool.TryParse(ReadAttributeValue(reader, "isPublic", "true"), out folderPage._isPublic);
            folderPage._outlineLevel = outlineLevel;
            folderPage._internalId = Guid.NewGuid();
            folderPage._siteId = siteId;
            folderPage._parentId = parentId;
            reader.Read(); //move to first subnode
        }

        /// <summary>
        /// Adds the page attributes.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="page">The page.</param>
        private void AddPageAttributes(XmlReader reader, CMSPage page)
        {
            bool.TryParse(ReadAttributeValue(reader, "isDefault", "false"), out page._isDefault);
            page._masterPage = ReadAttributeValue(reader, "masterpage", "");
        }

        /// <summary>
        /// Processes the next folder page property.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="folderPage">The folder page.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="outlineLevel">The outline level.</param>
        /// <returns></returns>
        private bool ProcessNextFolderPageProperty(XmlReader reader, CMSCommon folderPage, List<CMSCommon> rows, int outlineLevel)
        {
            if (reader.EOF)
                return false;

            string nodeName = reader.Name;
            switch (nodeName)
            {
                case "SortOrder":
                    int order;
                    int.TryParse(ReadElementValue(reader, "0"), out order);
                    folderPage._sortOrder = order;
                    break;
                case "Roles":
                    AddRoles(reader, folderPage);
                    break;
                case "Title":
                    ((CMSPage)folderPage)._title = ReadElementValue(reader, "");
                    break;
                case "Keywords":
                    ((CMSPage)folderPage)._keywords = ReadElementValue(reader, "");
                    break;
                case "Description":
                    ((CMSPage)folderPage)._description = ReadElementValue(reader, "");
                    break;
				case "PageNavigation":
					AddPageNavigation(reader, (CMSPage)folderPage);
					reader.Read(); // read EndElement
					break;
                case "Versions":
                    AddVersions(reader, (CMSPage)folderPage);
					reader.Read(); // read EndElement
                    break;
                case "Page":
                case "Folder":
                    if (reader.NodeType != XmlNodeType.EndElement)
                        AddFolderPage(reader, folderPage._siteId, rows, outlineLevel + 1, folderPage._internalId);
                    else
                    {
                        if (folderPage.GetType() == typeof(CMSPage) && reader.LocalName == "Page" ||
                            folderPage.GetType() == typeof(CMSFolder) && reader.LocalName == "Folder")
                        {
                            reader.Read();
                            return false;
                        }
                    }
                    reader.Read(); //read to the next element, which may be eof
                    break;
                default:
                    reader.Read(); //read and ignore unrecognizable elements
                    break;
            }

            return true;
        }

        /// <summary>
        /// Adds the roles.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="folderPage">The folder page.</param>
        private void AddRoles(XmlReader reader, CMSCommon folderPage)
        {
            string roles;

            if (!reader.IsEmptyElement)
            {
                roles = ReadElementValue(reader, "");
                folderPage._roles = roles.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
                reader.Read();
        }

		/// <summary>
		/// Adds the PageNavigation.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="page">The page.</param>
		private void AddPageNavigation(XmlReader reader, CMSPage page)
		{
            if (reader.NodeType == XmlNodeType.Element)
            {
                XmlReader subtreeReader = reader.ReadSubtree();

                if (page._commandList == null)
                    page._commandList = new List<CMSNavigationCommand>();

                while (subtreeReader.Read() && !subtreeReader.EOF)
                {
                    if (subtreeReader.IsStartElement("NavigationCommand"))
                    {
                        CMSNavigationCommand navCmd = new CMSNavigationCommand();
                        page._commandList.Add(navCmd);

                        navCmd._pageId = page._id;

                        navCmd._item = ReadAttributeValue(subtreeReader, "item", "");
                        navCmd._itemId = GetNavigationItemId(navCmd._item, false, true);

                        navCmd._params = ReadAttributeValue(subtreeReader, "params", "");
                        navCmd._trigerParam = ReadAttributeValue(subtreeReader, "trigerParam", "");

                        navCmd._parameters = new List<CMSNavigationCommandParameter>();

                        //read to first "Comment" node
                        XmlReader parametersTree = subtreeReader.ReadSubtree();
                        parametersTree.Read();
                        parametersTree.Read();

                        if (!parametersTree.EOF)
                        {
                            if (parametersTree.Name == "Parameters")
                            {
                                XmlReader parameterTree = parametersTree.ReadSubtree();
                                //parametersTree.Read();
                                while (parameterTree.Read() && !parameterTree.EOF)
                                {
                                    if (parameterTree.IsStartElement("Parameter"))
                                    {
                                        CMSNavigationCommandParameter parameter = new CMSNavigationCommandParameter();

                                        parameter._itemId = navCmd._itemId;
                                        parameter._name = ReadAttributeValue(parameterTree, "name", "");
                                        parameter._required = Boolean.Parse(ReadAttributeValue(parameterTree, "required", ""));
                                        parameter._value = ReadAttributeValue(parameterTree, "value", "");

                                        navCmd._parameters.Add(parameter);
                                    }
                                }
                            }
                        }
                    }
                }
            }
		}

        /// <summary>
        /// Adds the versions.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="page">The page.</param>
        private void AddVersions(XmlReader reader, CMSPage page)
        {
            //CMSVersion version;
            //XmlReader commentsTree;
            string tempValue;

            //read to the first version node
			//reader.Read();

			if (reader.NodeType == XmlNodeType.Element)
			{
				XmlReader subtreeReader = reader.ReadSubtree();

				if (page._versionList == null)
					page._versionList = new List<CMSVersion>();

				while (subtreeReader.Read() && !subtreeReader.EOF)
				{
					if (subtreeReader.IsStartElement("Version"))
					{
						CMSVersion version = new CMSVersion();
						page._versionList.Add(version);

						tempValue = ReadAttributeValue(subtreeReader, "createdby", "");
						if (tempValue == "")
							throw new SiteImportExportException("Creator id missing from version information");
						version._createdBy = new Guid(tempValue);

						DateTime.TryParse(ReadAttributeValue(subtreeReader, "created", DateTime.UtcNow.ToShortDateString()),
											out version._createdDate);
						version._language = ReadAttributeValue(subtreeReader, "language", "");
						version._languageId = GetLanguageId(version._language, true);

						tempValue = ReadAttributeValue(subtreeReader, "modifiedby", "");
						if (tempValue == "")
							throw new SiteImportExportException("Modifier id missing from version information");
						version._modifiedBy = new Guid(tempValue);

						DateTime.TryParse(ReadAttributeValue(subtreeReader, "modified", DateTime.UtcNow.ToShortDateString()),
										  out version._modifiedDate);
						version._status = ReadAttributeValue(subtreeReader, "status", "");
						version._statusId = GetStatusId(version._status, false);
						version._state = ReadAttributeValue(subtreeReader, "state", "");
						version._stateId = GetStateId(version._state, false);
						version._template = ReadAttributeValue(subtreeReader, "template", "");
						version._templateId = GetTemplateId(version._template, false);

						Int32.TryParse(ReadAttributeValue(subtreeReader, "version", "0"), out version._versionNum);

						//read to first "Comment" node
						XmlReader commentsTree = subtreeReader.ReadSubtree();
						commentsTree.Read();
						commentsTree.Read();

						while (ProcessNextVersionProperty(commentsTree, version))
						{
						}; //do nothing
					}
				}
			}
        }

        /// <summary>
        /// Processes the next version property.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="version">The version.</param>
        /// <returns></returns>
        private bool ProcessNextVersionProperty(XmlReader reader, CMSVersion version)
        {
            string pageDocString;

            if (reader.EOF)
                return false;

            string nodeName = reader.Name;

            switch (nodeName)
            {
                case "Comments":
                    version._comments = ReadElementValue(reader, "");
                    break;
                case "PageDocument":
                    pageDocString = ReadElementValue(reader, "");
                    if (pageDocString != "")
                        version._doc = AddPageDocument(pageDocString);
                    break;
                case "Version":
                case "Versions":
                    return false;
                default:
                    reader.Read();
                    break;
            }

            return true;
        }

        /// <summary>
        /// Adds the page document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        private PageDocument AddPageDocument(string document)
        {
            PageDocument doc = null;
            MemoryStream stream;

            stream = new MemoryStream(Convert.FromBase64String(document));
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            doc = (PageDocument)formatter.Deserialize(stream);

            return doc;
        }

        //Handles null and non-numeric values for the type
		
		/// <summary>
		/// Gets the NavigationItem id.
		/// </summary>
		/// <param name="itemName">Name of the NavigationItem.</param>
		/// <param name="assignDefaultIfDoesntExist">Returns the first item's id if item with specified name doesn't exist.</param>
		/// <param name="addIfDoesntExist">Adds new NavigationItem to the db if it doesn't exist.</param>
		/// <returns>Id of the found/created NavigationItem.</returns>
		private int GetNavigationItemId(string itemName, bool assignDefaultIfDoesntExist, bool addIfDoesntExist)
		{
			int result = -1;
			using (IDataReader reader = NavigationManager.GetItemByName(itemName))
			{
				if (reader.Read())
					result = (int)reader["ItemId"];

				if (result == -1)
				{
					if (assignDefaultIfDoesntExist)
					{
						// try to assign first navigation item. Throw exception if there are no any items in the db.
						bool exc = false;
						using (IDataReader allItemsReader = NavigationManager.GetAllItems())
						{
							if (allItemsReader.Read())
								result = (int)reader["ItemId"];
							else
								exc = true;
						}
                        if (exc)
                        {
                            reader.Close();
                            throw new Exception("At least one NavigationItem must be specified in the NavigationItems table");
                        }
					}
					else if (addIfDoesntExist)
					{
						try
						{
							result = NavigationManager.NewItem(itemName);
                            if (result <= 0)
                            {
                                reader.Close();
                                throw new Exception("Unknown error.");
                            }
						}
						catch(Exception ex)
						{
                            reader.Close();
							throw new Exception(String.Format("Could not add NavigationItem. Error: {0}", ex.Message));
						}
					}
				}

                reader.Close();
			}

			return result;
		}

        /// <summary>
        /// Gets the command type number.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        private int GetCommandTypeNumber(XmlReader reader)
        {
            switch (ReadAttributeValue(reader, "type", ""))
            {
                case "None":
                    return 0;
                case "Link":
                    return 1;
                case "Script":
                    return 2;
                case "Navigation":
                    return 3;
            }

			return -1; //TODO: Add default value
        }

        /// <summary>
        /// Gets the status id.
        /// </summary>
        /// <param name="statusName">Name of the status.</param>
        /// <param name="addIfDoesntExist">if set to <c>true</c> [add if doesnt exist].</param>
        /// <returns></returns>
        private int GetStatusId(string statusName, bool addIfDoesntExist)
        {
            int result = -1;//TODO: Check default value
            int defaultResult = -1;

            if (statusName != "")
            {
                using (IDataReader reader = WorkflowStatus.LoadAll())
                {
                    while (reader.Read())
                    {
                        if (defaultResult == -1)
                            int.TryParse(reader["StatusId"].ToString(), out defaultResult);

						if (String.Compare(reader["FriendlyName"].ToString(), statusName, true) == 0)
						{
							int.TryParse(reader["StatusId"].ToString(), out result);
							break;
						}
                    }

                    reader.Close();
                }
            }

            //if no match is found, try to use the first value returned; if no values returned, throw exception
            if (result == -1 && addIfDoesntExist)
            {
                if (defaultResult != -1)
                    result = defaultResult;
                else
                    throw new Exception("At least one row must exist in the [WorkFlowStatus] table");
            }
            return result;
        }

        /// <summary>
        /// Gets the state id.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <param name="addIfDoesntExist">if set to <c>true</c> [add if doesnt exist].</param>
        /// <returns></returns>
        private int GetStateId(string stateName, bool addIfDoesntExist)
        {
            int result = -1; //TODO: Check default value
            int defaultResult = -1;
            if (stateName != "")
            {
                using (IDataReader reader = PageState.GetAll())
                {
                    while (reader.Read())
                    {
                        if (defaultResult == -1)
                            int.TryParse(reader["StateId"].ToString(), out defaultResult);

						if (String.Compare(reader["FriendlyName"].ToString(), stateName, true) == 0)
						{
							int.TryParse(reader["StateId"].ToString(), out result);
							break;
						}
                    }

                    reader.Close();
                }
            }

            if (result == -1 && addIfDoesntExist)
            {
                //no matches were found in the reader - return the first value, if one exists
                if (defaultResult != -1)
                    result = defaultResult;
                else
                    throw new Exception("At least one state must exist in the main_PageState table");
            }

            return result;
        }

        /// <summary>
        /// Gets the template id.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="addIfDoesntExist">if set to <c>true</c> [add if doesnt exist].</param>
        /// <returns></returns>
        private int GetTemplateId(string templateName, bool addIfDoesntExist)
        {
            int result = -1;
            using (TemplateDto dto = DictionaryManager.GetTemplateDto())
            {
                if (dto.main_Templates.Count > 0)
                {
                    foreach (TemplateDto.main_TemplatesRow row in dto.main_Templates)
                    {
                        if (row.Name == templateName)
                            result = row.TemplateId;
                    }
                }

                if (result == -1 && addIfDoesntExist)
                {
                    if (dto.main_Templates.Count > 0)
                    {
                        result = ((TemplateDto.main_TemplatesRow)dto.main_Templates.Rows[0]).TemplateId;
                    }
                    else
                        throw new Exception("At least one template must be specified in the main_Templates table");
                }
            }

            return result;
        }

		/// <summary>
		/// Gets the language that will be used when language is needed but was not retrieved from xml.
		/// </summary>
		/// <returns>Language code.</returns>
		private string GetDefaultImportLanguage()
		{
			string langCode = String.Empty;
			SettingsDto dto = CommonSettingsManager.GetSettingByName(CommonSettingsManager.SettingsNames.DefaultLanguage);
			if (dto.CommonSettings.Count > 0)
				langCode = dto.CommonSettings[0].Value;
			else
			{
				DataTable dt = Language.GetAllLanguagesDT();
				if (dt.Rows.Count > 0)
				{
					// if exists language with the same culture as current thread's, get it; otherwise take the first language
					DataRow[] rows = dt.Select(String.Format("LangName='{0}'", Thread.CurrentThread.CurrentUICulture.Name));
					if (rows != null && rows.Length > 0)
						langCode = rows[0]["LangName"].ToString();
					else
						langCode = dt.Rows[0]["LangName"].ToString();
				}
			}

			return langCode;
		}

        /// <summary>
		/// Read a string element and return an empty string if the element value is null.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private string ReadElementValue(XmlReader reader, string defaultValue)
        {
            string result;
            try
            {
                if (reader.NodeType != XmlNodeType.EndElement)
                    result = reader.ReadElementString();
                else
                    result = "";
            }
            catch
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
		/// Read an attribute and return an empty string if the attribute value is null
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="name">The name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        private string ReadAttributeValue(XmlReader reader, string name, string defaultValue)
        {
            string result;
            try
            {
                result = reader.GetAttribute(name);
                if (result == null)
                    result = defaultValue;
            }
            catch
            {
                result = defaultValue;
            }

            return result;
        }

		#region Classes
        private class CMSCommon
        {
            public string _name;
            public bool _isPublic;
            public bool _isFolder;
            public bool _isDefault;
            public int _sortOrder;
            public int _outlineLevel;
            public Guid _siteId;
            public string[] _roles;
            public string _masterPage = "";
            public int _id;//primary key value once saved to db
            public Guid _internalId; //see comment below
            public Guid _parentId; //used internally to set the hierarchy later; assumes sort order isn't necessarily populated

            /// <summary>
            /// Saves this instance.
            /// </summary>
            public void Save()
            {
                //only save if its not the root
                if (_outlineLevel != 0)
                    _id = FileTreeItem.AddFileItem(_name, _isPublic, _isFolder, _isDefault, _masterPage, _siteId);
            }
        }

        private class CMSFolder : CMSCommon
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CMSFolder"/> class.
            /// </summary>
            public CMSFolder()
            {
                _isFolder = true;
                _masterPage = "";
                _isDefault = false;
            }
        }

        private class CMSPage : CMSCommon
        {
            public List<CMSVersion> _versionList;
			public List<CMSNavigationCommand> _commandList;
            public string _title;
            public string _keywords;
            public string _description;

            /// <summary>
            /// Initializes a new instance of the <see cref="CMSPage"/> class.
            /// </summary>
            public CMSPage()
            {
                _isFolder = false;
            }
        }

        private class CMSVersion
        {
            public int _id = 0;
            public string _language;
            public int _languageId;
            public string _template;
            public int _templateId;
            public string _status;
            public int _statusId;
            public string _state;
            public int _stateId;
            public DateTime _createdDate;
            public Guid _createdBy;
            public DateTime _modifiedDate;
            public Guid _modifiedBy;
            public string _comments;
			public int _versionNum;
            public PageDocument _doc;

            /// <summary>
            /// Initializes a new instance of the <see cref="CMSVersion"/> class.
            /// </summary>
            public CMSVersion()
            {
                _doc = null;
            }
        }

		private class CMSNavigationCommand
		{
			public int _id = 0;
			public int _pageId;
			public int _itemId;
			public string _item;
			public string _params;
			public string _trigerParam;
            public List<CMSNavigationCommandParameter> _parameters;
		}

        private class CMSNavigationCommandParameter
        {
            public int _id = 0;
            public int _itemId;
            public string _name;
            public string _value;
            public bool _required;
        }
		#endregion

        #endregion

        #endregion
    }

    /// <summary>
    /// Class used to copy elements of the ECF.
    /// </summary>
    public class CopyHelper
    {
        #region CopySiteContent
        /// <summary>
        /// Copy the site content.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="targetSiteId">The target site id.</param>
        public void CopySiteContent(Guid siteId, Guid targetSiteId)
        {
            int rootId = FileTreeItem.GetRoot(siteId);
            if (rootId > 0)
            {
                int newRootId = FileTreeItem.GetRoot(targetSiteId);
                DataTable items = FileTreeItem.LoadItemByFolderIdDT(rootId);
                if (items.Rows.Count > 0)
                {
                    foreach (DataRow dr in items.Rows)
                    {
                        CopyFoldersPages(dr, newRootId, targetSiteId);
                    }
                }
            }
        }

        /// <summary>
        /// Copy the folders and pages.
        /// </summary>
        /// <param name="newParentId">The new parent page id.</param>
        /// <param name="siteId">The site id.</param>
        private void CopyFoldersPages(DataRow row, int newParentId, Guid siteId)
        {
            if ((int)row["OutlineLevel"] > 0)
            {
                int pageId = (Int32)row["PageId"];

                int newPageId = FileTreeItem.AddFileItem((string)row["Name"], (bool)row["IsPublic"], (bool)row["IsFolder"], (bool)row["IsDefault"], (string)row["MasterPage"], siteId);

                FileTreeItem.MoveTo(newPageId, newParentId);

                //Copy page access
                using (IDataReader dr = FileTreeItem.PageAccessGetByPageId(pageId))
                {
                    while (dr.Read())
                    {
                        FileTreeItem.AddPageAccess((string)dr["RoleId"], newPageId);
                    }
                }

                if (!(bool)row["IsFolder"])
                {
                    //Copy page attributes
                    using (IDataReader dr = PageAttributes.GetByPageId(pageId))
                    {
                        while (dr.Read())
                        {
                            PageAttributes.Add(newPageId, (string)dr["Title"], (string)dr["MetaKeys"], (string)dr["MetaDescriptions"]);
                        }
                    }

                    //Copy navigation commands
                    using (DataTable dt = NavigationManager.GetByPageId(pageId))
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                NavigationManager.NewCommand(newPageId.ToString(), (Int32)dr["ItemId"], (string)dr["Params"], (string)dr["TrigerParam"]);
                            }
                        }
                    }

                    //Copy page versions
                    int statusId = WorkflowStatus.GetLast();
                    using (DataTable dt = PageVersion.GetVersionByStatusIdDT(pageId, statusId))
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            PageDocument.PersistentDocumentStorage = new SqlPageDocumentStorageProvider();

                            foreach (DataRow dr in dt.Rows)
                            {
                                PageDocument pageDocument = PageDocument.Open((Int32)dr["VersionId"], OpenMode.View, Guid.Empty);

                                //translate the menu ids for the menu controls to friendly name from the original site...
                                ImportExportHelper.MapMenuIdToFriendlyName(pageDocument);

                                //and then translate the friendlyname to the menu id in the new site
                                ImportExportHelper.MapMenuFriendlyNameToMenuId(siteId, pageDocument);

                                int newVersionId = PageVersion.AddPageVersion2(newPageId, (Int32)dr["TemplateId"], (Int32)dr["VersionNum"], (Int32)dr["LangId"], (Int32)dr["StatusId"], new Guid(dr["creatorUID"].ToString()), (DateTime)dr["Created"], new Guid((string)dr["editorUID"].ToString()), (DateTime)dr["Edited"], (Int32)dr["StateId"], (string)dr["Comment"]);

                                try
                                {
                                    PageDocument.PersistentDocumentStorage.Save(pageDocument, newVersionId, Guid.NewGuid());
                                }
                                catch { }
                            }
                        }
                    }
                }
                else
                    newParentId = newPageId;
            }

            // Add sub folders
            if ((bool)row["IsFolder"])
            {
                DataTable folders = FileTreeItem.LoadItemByFolderIdDT((int)row["PageId"]);
                if (folders.Rows.Count > 0)
                {
                    foreach (DataRow dr in folders.Rows)
                    {
                        CopyFoldersPages(dr, newParentId, siteId);
                    }
                }
            }
        }

        #endregion

        #region CopyMenu
        public void CopySiteMenu(Guid siteId, Guid targetSiteId)
        {
            MenuDto menuDto = MenuManager.GetMenuDto(siteId);
            foreach (MenuDto.MenuRow menuRow in menuDto.Menu)
            {
                int menuId = Mediachase.Cms.Menu.Add(menuRow.FriendlyName, targetSiteId);
                int rootMenuId = GetRootItem(targetSiteId, menuId);

                DataView view = new DataView(menuDto.MenuItem);
                view.RowFilter = String.Format("MenuId = {0} and IsRoot = 1", menuRow.MenuId);
                foreach (DataRowView itemRow in view)
                {
                    //fix 
                    AddMenuItem(menuId, menuDto, (MenuDto.MenuItemRow)itemRow.Row, 0, rootMenuId);
                }
            }
        }

        /// <summary>
        /// Adds the menu item.
        /// </summary>
        /// <param name="menuId">The menu id.</param>
        /// <param name="menuDto">The menu dto.</param>
        /// <param name="itemRow">The menu item row.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="rootMenuId">The root menu id.</param>
        private void AddMenuItem(int menuId, MenuDto menuDto, MenuDto.MenuItemRow itemRow, int parentId, int rootMenuId)
        {
            int menuItemId = -1;

            // Save this menu item
            if (rootMenuId > -1)
            {
                Mediachase.Cms.MenuItem.Update(rootMenuId, itemRow.Text, itemRow.CommandText, itemRow.CommandType, itemRow.IsVisible, itemRow.Order);
                menuItemId = rootMenuId;
            }
            else
                menuItemId = Mediachase.Cms.MenuItem.Add(menuId, itemRow.Text, itemRow.CommandText, itemRow.CommandType, itemRow.IsVisible, itemRow.Order);

            // Save the Resources
            foreach (MenuDto.MenuItem_ResourcesRow resourceRow in itemRow.Getmain_MenuItem_ResourcesRows())
                Mediachase.Cms.MenuItem.AddResource(menuItemId, resourceRow.LanguageId, resourceRow.Text, resourceRow.ToolTip);

            // If has ParentId then move this menu item to its parent
            if (parentId > 0)
                Mediachase.Cms.MenuItem.MoveTo(menuItemId, parentId);

            // Drill into the next level
            DataView view = new DataView(menuDto.MenuItem);
            view.RowFilter = String.Format("MenuId = {0} and Outline like '{1}{2}.%' and OutlineLevel={3}", itemRow.MenuId, itemRow.Outline, itemRow.MenuItemId.ToString(), itemRow.OutlineLevel + 1);
            foreach (DataRowView itemRow2 in view)
            {
                AddMenuItem(menuId, menuDto, (MenuDto.MenuItemRow)itemRow2.Row, menuItemId, -1);
            }
        }

        /// <summary>
        /// Gets the root item.
        /// </summary>
        /// <param name="siteId">The site id.</param>
        /// <param name="menuId">The menu id.</param>
        /// <returns></returns>
        private int GetRootItem(Guid siteId, int menuId)
        {
            int retVal = -1;
            using (IDataReader reader = Mediachase.Cms.MenuItem.LoadAllRoot(siteId))
            {
                while (reader.Read())
                {
                    if ((bool)reader["IsRoot"] == true && (int)reader["MenuId"] == menuId)
                    {
                        retVal = (int)reader["MenuItemId"];
                        break;
                    }
                }

                reader.Close();
            }
            return retVal;
        }
        #endregion
    }
}
