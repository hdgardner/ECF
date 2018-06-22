using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Storage;
using Mediachase.Commerce.Core;
using Mediachase.Data.Provider;
using System.Threading;

namespace Mediachase.Commerce.Catalog.ImportExport
{
    /// <summary>
    /// Handles exceptions for the catalog import and export operations and inherits the <see cref="Exception"/> class.
    /// </summary>
    [Serializable]
    public class ImportExportException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExportException"/> class.
        /// </summary>
        public ImportExportException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExportException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ImportExportException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExportException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public ImportExportException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExportException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
        /// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
        protected ImportExportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    /// <summary>
    /// The ImportExportMessageType enumeration defines the import/export message type.
    /// </summary>
	[Serializable]
	public enum ImportExportMessageType
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
    /// Represents the arguments for the import and export events. (Inherits <see cref="EventArgs"/>.)
    /// </summary>
	[Serializable]
	public class ImportExportEventArgs : EventArgs
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
		/// Initializes a new instance of the <see cref="ImportExportEventArgs"/> class.
		/// </summary>
		public ImportExportEventArgs()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportExportEventArgs"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="percentage">The percentage.</param>
		public ImportExportEventArgs(string message, double percentage)
			: base()
		{
			this.Message = message;
			this.CompletedPercentage = percentage;
		}
	}

    /// <summary>
    /// Handles the import/export progress message.
    /// </summary>
    public delegate void ImportExportProgressMessageHandler(object source, ImportExportEventArgs args);

    /// <summary>
    /// Implements operations for catalog import and export.
    /// </summary>
    public class CatalogImportExport
    {
        // Following private member apparently never used
        // private static CatalogImportExport _Instance;
        private static readonly object _lockObject = new object();

        //public static CatalogImportExport Instance
        //{
        //    get
        //    {
        //        if (_Instance == null)
        //        {
        //            lock (_lockObject)
        //            {
        //                if (_Instance == null)
        //                    _Instance = new CatalogImportExport();
        //            }
        //        }
        //        return _Instance;
        //    }
        //}

        //private CatalogImportExport() { }

		#region ImportExport Steps
		internal enum ExportSteps
		{
			Init = 0x00,
			StartExportDictionaries = 0x01,
			EndExportDictionaries = 0x02,
			StartExportCatalogNodes = 0x03,
			EndExportCatalogNodes = 0x04,
			StartExportCatalogEntries = 0x05,
			EndExportCatalogEntres = 0x06,
			StartExportCatalogRelations = 0x07,
			EndExportCatalogRelations = 0x08,
			Finish = 0x09
		}

		internal enum ImportSteps
		{
			Init = 0x00,
			StartImportMetaDataSchema = 0x01,
			EndImportMetaDataSchema = 0x02,
			StartImportDictionaries = 0x03,
			EndImportDictionaries = 0x04,
			StartImportCatalogProperties = 0x05,
			EndImportCatalogProperties = 0x06,
			StartImportNodes = 0x07,
			EndImportNodes = 0x08,
			StartImportEntries = 0x09,
			EndImportEntries = 0x0A,
			StartImportRelations = 0x0B,
			EndImportRelations = 0x0C,
			StartImportAssociations = 0x0D,
			EndImportAssociations = 0x0E,
			Finish = 0x0F
		}

		/// <summary>
		/// Gets the total export steps.
		/// </summary>
		/// <returns></returns>
		public static int GetTotalExportSteps()
		{
			return Enum.GetValues(typeof(ExportSteps)).Length;
		}

		/// <summary>
		/// Gets the total import steps.
		/// </summary>
		/// <returns></returns>
		public static int GetTotalImportSteps()
		{
			return Enum.GetValues(typeof(ImportSteps)).Length;
		}
		#endregion

        /// <summary>
        /// Occurs when [import export progress message].
        /// </summary>
        public event ImportExportProgressMessageHandler ImportExportProgressMessage;

        /// <summary>
        /// Raises the <see cref="E:ImportExportProgressMessage"/> event.
        /// </summary>
		/// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.ImportExport.ImportExportEventArgs"/> instance containing the event data.</param>
		protected virtual void OnImportExportProgressMessage(object source, ImportExportEventArgs args)
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
			OnImportExportProgressMessage(this, new ImportExportEventArgs(message, percentage));
		}

		/// <summary>
		/// Called when [event].
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="msgType">The message type.</param>
		private void OnEvent(string message, double percentage, ImportExportMessageType msgType)
		{
			if (msgType == ImportExportMessageType.Warning)
				message = "Warning - " + message;

			OnImportExportProgressMessage(this, new ImportExportEventArgs(message, percentage));
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

		/// <summary>
		/// Returns culture used for importing/exporting (essential when parsing and writing decimal, and floating point values).
		/// </summary>
		/// <returns></returns>
		private static CultureInfo GetImportExportCulture()
		{
			CultureInfo ci = System.Globalization.CultureInfo.InvariantCulture;
			return ci;
		}

		#region Export
		/// <summary>
        /// Exports the specified catalog name.
        /// </summary>
        /// <param name="catalogName">Name of the catalog.</param>
        /// <param name="output">The output.</param>
        /// <param name="baseFilePath">The base file path.</param>
        public void Export(string catalogName, Stream output, string baseFilePath)
        {
			CultureInfo currentCultureInfo = GetImportExportCulture();

			int totalExportSteps = GetTotalExportSteps();

            Catalog.Dto.CatalogDto catalogDto = new Catalog.Dto.CatalogDto();
            Catalog.Dto.CatalogNodeDto catalogNodeDto = new Catalog.Dto.CatalogNodeDto();
            Catalog.Dto.CatalogEntryDto catalogEntryDto = new Catalog.Dto.CatalogEntryDto();
            Catalog.Dto.CatalogRelationDto catalogRelationDto = new Catalog.Dto.CatalogRelationDto();
			Catalog.Dto.CatalogAssociationDto catalogAssociationDto = new Catalog.Dto.CatalogAssociationDto();
            
            OnEvent("Starting export...", GetProgressPercent((int)ExportSteps.Init, totalExportSteps));

            #region XML File Settings

            XmlWriterSettings xmlFileSettings = new XmlWriterSettings();
            xmlFileSettings.Indent = true;

            #endregion

            #region XML Writer
            XmlWriter exportWriter = XmlWriter.Create(output, xmlFileSettings);

            // Start the Xml Document
            exportWriter.WriteStartDocument();

            #endregion

            // Get the Catalog
            catalogDto = FrameworkContext.Current.CatalogSystem.GetCatalogDto();

            if (catalogDto.Catalog.Count == 0)
                throw new ImportExportException("There are no Catalogs");

            exportWriter.WriteStartElement("Catalogs", "");  // Root
            exportWriter.WriteAttributeString("Version", "1.0");

            #region MetaDataSchema

            // meta data
            MetaClassCollection metaClasses = MetaClass.GetList(CatalogContext.MetaDataContext, "Mediachase.Commerce.Catalog", true);

            List<int> metaClassIds = new List<int>();
            foreach (MetaClass mc in metaClasses)
            {
                metaClassIds.Add(mc.Id);
            }

            // MetaData scheme
            exportWriter.WriteStartElement("MetaDataScheme", "");

            if (metaClassIds.Count > 0)
            {
                // Get MetaDataSchema Here
                string scheme = MetaInstaller.BackupMetaClasses(CatalogContext.MetaDataContext, false, metaClassIds.ToArray());
                exportWriter.WriteRaw(scheme);
            }

            exportWriter.WriteEndElement(); //End of MetaDataScheme

            exportWriter.Flush();

            #endregion

			#region Dictionaries
			OnEvent("Start exporting dictionaries", GetProgressPercent((int)ExportSteps.StartExportDictionaries, totalExportSteps));
			exportWriter.WriteStartElement("Dictionaries", "");
			
			WriteMerchants(exportWriter);
			WritePackages(exportWriter);
			WriteTaxCategories(exportWriter);
			WriteWarehouses(exportWriter);
			WriteAssociationTypes(exportWriter);

			exportWriter.WriteEndElement(); // End of Dictionaries
			exportWriter.Flush();
			OnEvent("Finished exporting dictionaries", GetProgressPercent((int)ExportSteps.EndExportDictionaries, totalExportSteps));
			#endregion

			#region Catalog Nodes
			// get the dictionary dtos. They will be used during variation export.
			CatalogEntryDto merchants = CatalogEntryManager.GetMerchants();
			CatalogTaxDto taxes = CatalogTaxManager.GetTaxCategories();

			// export all catalog nodes
			for (int i = 0; i < catalogDto.Catalog.Count; i++)
            {
                // Check if this Catalog Name matches the desired name passed
                if (!string.Equals(catalogDto.Catalog[i].Name, catalogName, StringComparison.InvariantCulture))
                    continue;

                Catalog.Dto.CatalogDto.CatalogRow catalogRow = catalogDto.Catalog[i];

                exportWriter.WriteStartElement("Catalog", "");

                #region Catalog Object Information

                exportWriter.WriteAttributeString("name", catalogRow.Name);
                exportWriter.WriteAttributeString("lastmodified", catalogRow.Modified.ToString("u"));
                exportWriter.WriteAttributeString("startDate", catalogRow.StartDate.ToString("u"));
                exportWriter.WriteAttributeString("endDate", catalogRow.EndDate.ToString("u"));
                exportWriter.WriteAttributeString("defaultCurrency", catalogRow.DefaultCurrency);
                exportWriter.WriteAttributeString("weightBase", catalogRow.WeightBase);
                exportWriter.WriteAttributeString("defaultLanguage", catalogRow.DefaultLanguage);
				exportWriter.WriteAttributeString("sortOrder", catalogRow.SortOrder.ToString(currentCultureInfo));
				exportWriter.WriteAttributeString("isActive", catalogRow.IsActive.ToString(currentCultureInfo));

                #endregion

                #region CatalogLanguages

                List<string> catalogLanguages = new List<string>();
                string languages = catalogRow.DefaultLanguage;
                catalogLanguages.Add(catalogRow.DefaultLanguage);

                foreach(Catalog.Dto.CatalogDto.CatalogLanguageRow catalogLanguageRow in catalogRow.GetCatalogLanguageRows())
                {
                    catalogLanguages.Add(catalogLanguageRow.LanguageCode);
                    languages += String.Concat(",", catalogLanguageRow.LanguageCode);
                }
                exportWriter.WriteAttributeString("languages", languages);
                
                #endregion

                #region CatalogSites

                exportWriter.WriteStartElement("Sites", ""); // Start Sitess
                foreach (Catalog.Dto.CatalogDto.SiteCatalogRow siteCatalogRow in catalogRow.GetSiteCatalogRows())
                {
                    exportWriter.WriteElementString("Site", siteCatalogRow.SiteId.ToString());
                }
                exportWriter.WriteEndElement();  // end Sites

                #endregion

                OnEvent("Start exporting catalog nodes", GetProgressPercent((int)ExportSteps.StartExportCatalogNodes, totalExportSteps));
                #region Catalog Nodes

                exportWriter.WriteStartElement("Nodes", "");

                catalogNodeDto = FrameworkContext.Current.CatalogSystem.GetCatalogNodesDto(catalogRow.CatalogId);
                catalogRelationDto = FrameworkContext.Current.CatalogSystem.GetCatalogRelationDto(catalogRow.CatalogId, 0, 0, String.Empty, new CatalogRelationResponseGroup());

                DataView catalogNodeDataView = catalogNodeDto.CatalogNode.DefaultView;
                DataView catalogNodeRelationDataView = catalogRelationDto.CatalogNodeRelation.DefaultView;
                DataView catalogEntryRelationDataView = catalogRelationDto.CatalogEntryRelation.DefaultView;
                DataView nodeEntryRelationDataView = catalogRelationDto.NodeEntryRelation.DefaultView;

				int currentNodeNumber = 0;
				int totalNodesCount = catalogNodeDto.CatalogNode.Count;

				exportWriter.WriteAttributeString("totalCount", totalNodesCount.ToString());

                foreach (Catalog.Dto.CatalogNodeDto.CatalogNodeRow nodeItem in catalogNodeDto.CatalogNode)
                {
					currentNodeNumber++;

                    exportWriter.WriteStartElement("Node", "");
                    exportWriter.WriteElementString("Name", nodeItem.Name);
                    exportWriter.WriteElementString("StartDate", nodeItem.StartDate.ToString("u"));
                    exportWriter.WriteElementString("EndDate", nodeItem.EndDate.ToString("u"));
					exportWriter.WriteElementString("IsActive", nodeItem.IsActive.ToString(currentCultureInfo));
					exportWriter.WriteElementString("SortOrder", nodeItem.SortOrder.ToString(currentCultureInfo));
                    exportWriter.WriteElementString("DisplayTemplate", nodeItem.IsTemplateNameNull() ? String.Empty : nodeItem.TemplateName);
                    exportWriter.WriteElementString("Code", nodeItem.Code);

                    // MetaData Information
                    WriteMetaData(exportWriter, nodeItem.MetaClassId, nodeItem.CatalogNodeId, catalogLanguages.ToArray(), baseFilePath);

                    // Get the Parent Code if exists
                    Catalog.Dto.CatalogNodeDto.CatalogNodeRow parentRow = catalogNodeDto.CatalogNode.FindByCatalogNodeId(nodeItem.ParentNodeId);

                    if (parentRow != null)
                    {
                        catalogNodeDataView.RowFilter = String.Format("Code='{0}'", parentRow.Code);
                        if (catalogNodeDataView.Count > 0)
                            exportWriter.WriteElementString("ParentNode", catalogNodeDataView[0]["Code"].ToString());
                        else
                            exportWriter.WriteElementString("ParentNode", null);
                    }
                    else
                        exportWriter.WriteElementString("ParentNode", null);

                    #region Seo Information

                    exportWriter.WriteStartElement("SeoInfo", "");

                    foreach (Catalog.Dto.CatalogNodeDto.CatalogItemSeoRow itemSeoRow in nodeItem.GetCatalogItemSeoRows())
                    {
                        exportWriter.WriteStartElement("Seo", "");

                        exportWriter.WriteElementString("LanguageCode", itemSeoRow.LanguageCode);
                        exportWriter.WriteElementString("Uri", itemSeoRow.Uri);
						exportWriter.WriteElementString("Title", itemSeoRow.IsTitleNull() ? String.Empty : itemSeoRow.Title);
						exportWriter.WriteElementString("Description", itemSeoRow.IsDescriptionNull() ? String.Empty : itemSeoRow.Description);
						exportWriter.WriteElementString("Keywords", itemSeoRow.IsKeywordsNull() ? String.Empty : itemSeoRow.Keywords);

                        exportWriter.WriteEndElement(); // End of Seo
                    }

                    exportWriter.WriteEndElement(); // End of SeoInfo
                    #endregion

                    exportWriter.WriteEndElement(); // End of Node
                    exportWriter.Flush(); // Flush buffer to output stream

					if ((currentNodeNumber % 20 == 0) || (currentNodeNumber == totalNodesCount))
						OnEvent(String.Format("Exported {0} of {1} nodes", currentNodeNumber, totalNodesCount), GetProgressPercent2((int)ExportSteps.StartExportCatalogNodes + (double)currentNodeNumber / totalNodesCount, totalExportSteps));
                }

                exportWriter.WriteEndElement(); // End of Nodes
                #endregion

                // Needed for testing long-running process with UI. Please do not remove!
				//for (int iTemp = 1; iTemp < 5000; iTemp++)
				//{
				//    Catalog.Dto.CatalogDto catalogDto2 = FrameworkContext.Current.CatalogSystem.GetCatalogDto(1);
				//}

                OnEvent("Finished exporting catalog nodes", GetProgressPercent((int)ExportSteps.EndExportCatalogNodes, totalExportSteps));

				// Needed for testing long-running process with UI. Please do not remove!
				//for (int iTemp = 1; iTemp < 3500; iTemp++)
				//{
				//    Catalog.Dto.CatalogDto catalogDto2 = FrameworkContext.Current.CatalogSystem.GetCatalogDto(1);
				//}

                OnEvent("Start exporting catalog entries", GetProgressPercent((int)ExportSteps.StartExportCatalogEntries, totalExportSteps));

                #region Entries

                exportWriter.WriteStartElement("Entries", "");

                // Should interrupt if we are on the last dataset
                int currentRecord = 0;
				bool firstLoop = true;

                while (true)
                {
                    CatalogSearchParameters pars = new CatalogSearchParameters();
                    CatalogSearchOptions options = new CatalogSearchOptions();

                    options.Namespace = String.Empty;
                    options.RecordsToRetrieve = 20;
                    options.StartingRecord = currentRecord;
                    currentRecord += options.RecordsToRetrieve;
                    pars.CatalogNames.Add(catalogName);

                    int totalEntriesCount = 0;
					catalogEntryDto = CatalogContext.Current.FindItemsDto(pars, options, ref totalEntriesCount, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                    //catalogEntryDto = FrameworkContext.Current.CatalogSystem.GetCatalogEntriesDto(catalogName);

					if (firstLoop)
					{
						// we just entered the loop; write total entries count
						exportWriter.WriteAttributeString("totalCount", totalEntriesCount.ToString());
						firstLoop = false;
					}

                    DataView catalogEntryDataView = catalogEntryDto.CatalogEntry.DefaultView;

                    foreach (Catalog.Dto.CatalogEntryDto.CatalogEntryRow itemEntry in catalogEntryDto.CatalogEntry)
                    {
                        exportWriter.WriteStartElement("Entry", "");
                        exportWriter.WriteElementString("Name", itemEntry.Name);
                        exportWriter.WriteElementString("StartDate", itemEntry.StartDate.ToString("u"));
                        exportWriter.WriteElementString("EndDate", itemEntry.EndDate.ToString("u"));
						exportWriter.WriteElementString("IsActive", itemEntry.IsActive.ToString(currentCultureInfo));
                        exportWriter.WriteElementString("DisplayTemplate", itemEntry.IsTemplateNameNull() ? String.Empty : itemEntry.TemplateName);
                        exportWriter.WriteElementString("Code", itemEntry.Code);
                        exportWriter.WriteElementString("EntryType", itemEntry.ClassTypeId);

                        // MetaData Information
                        WriteMetaData(exportWriter, itemEntry.MetaClassId, itemEntry.CatalogEntryId, catalogLanguages.ToArray(), baseFilePath);

                        #region VariationInfo
                        // Write variation information

						if (itemEntry.GetVariationRows().Length > 0)
						{
							exportWriter.WriteStartElement("VariationInfo", "");
							foreach (Catalog.Dto.CatalogEntryDto.VariationRow variationRow in itemEntry.GetVariationRows())
							{
								exportWriter.WriteStartElement("Variation", "");

								exportWriter.WriteElementString("ListPrice", variationRow.IsListPriceNull() ? String.Empty : variationRow.ListPrice.ToString(currentCultureInfo));
								exportWriter.WriteElementString("MaxQuantity", variationRow.IsMaxQuantityNull() ? String.Empty : variationRow.MaxQuantity.ToString(currentCultureInfo));
								exportWriter.WriteElementString("MinQuantity", variationRow.IsMinQuantityNull() ? String.Empty : variationRow.MinQuantity.ToString(currentCultureInfo));

								if (!variationRow.IsMerchantIdNull() && merchants != null && merchants.Merchant.Count > 0)
								{
									CatalogEntryDto.MerchantRow[] merchantRows = (CatalogEntryDto.MerchantRow[])merchants.Merchant.Select(String.Format("MerchantId='{0}'", variationRow.MerchantId));
									if (merchantRows != null && merchantRows.Length > 0)
										exportWriter.WriteElementString("MerchantName", merchantRows[0].Name);
								}

								ShippingMethodDto shPackage = ShippingManager.GetShippingPackage(variationRow.PackageId);
								if (shPackage != null && shPackage.Package.Count > 0)
									exportWriter.WriteElementString("PackageName", shPackage.Package[0].Name);

								if (taxes != null && taxes.TaxCategory.Count > 0)
								{
									CatalogTaxDto.TaxCategoryRow[] tcRows = (CatalogTaxDto.TaxCategoryRow[])taxes.TaxCategory.Select(String.Format("TaxCategoryId={0}", variationRow.TaxCategoryId));
									if (tcRows != null && tcRows.Length > 0)
										exportWriter.WriteElementString("TaxCategoryName", tcRows[0].Name);
								}

								exportWriter.WriteElementString("TrackInventory", variationRow.TrackInventory.ToString(currentCultureInfo));

								WarehouseDto warehouse = WarehouseManager.GetWarehouseByWarehouseId(variationRow.WarehouseId);
								if (warehouse != null && warehouse.Warehouse.Count > 0)
									exportWriter.WriteElementString("WarehouseName", warehouse.Warehouse[0].Name);

								exportWriter.WriteElementString("Weight", variationRow.Weight.ToString(currentCultureInfo));

								exportWriter.WriteEndElement(); // End of Variation

							}
							exportWriter.WriteEndElement(); // End of VariationInfo
						}
                        #endregion

                        #region InventoryInfo
                        // Write Inventory information                        
                        Catalog.Dto.CatalogEntryDto.InventoryRow inventoryRow = itemEntry.InventoryRow;

                        if (inventoryRow != null)
                        {
                            exportWriter.WriteStartElement("Inventory", "");

							exportWriter.WriteElementString("AllowBackorder", inventoryRow.AllowBackorder.ToString(currentCultureInfo));
							exportWriter.WriteElementString("AllowPreorder", inventoryRow.AllowPreorder.ToString(currentCultureInfo));
                            exportWriter.WriteElementString("BackorderAvailabilityDate", inventoryRow.BackorderAvailabilityDate.ToString("u"));
							exportWriter.WriteElementString("BackorderQuantity", inventoryRow.BackorderQuantity.ToString(currentCultureInfo));
							exportWriter.WriteElementString("InStockQuantity", inventoryRow.InStockQuantity.ToString(currentCultureInfo));
							exportWriter.WriteElementString("InventoryStatus", inventoryRow.InventoryStatus.ToString(currentCultureInfo));
                            exportWriter.WriteElementString("PreorderAvailabilityDate", inventoryRow.PreorderAvailabilityDate.ToString("u"));
							exportWriter.WriteElementString("PreorderQuantity", inventoryRow.PreorderQuantity.ToString(currentCultureInfo));
							exportWriter.WriteElementString("ReorderMinQuantity", inventoryRow.ReorderMinQuantity.ToString(currentCultureInfo));
							exportWriter.WriteElementString("ReservedQuantity", inventoryRow.ReservedQuantity.ToString(currentCultureInfo));

                            exportWriter.WriteEndElement(); // End of VariationInfo
                        }
                        #endregion

                        #region SalePrices
                        // Write Sale Prices information                        
                        if (itemEntry.GetSalePriceRows().Length > 0)
                        {
                            exportWriter.WriteStartElement("SalePrices", "");
                            foreach (Catalog.Dto.CatalogEntryDto.SalePriceRow priceRow in itemEntry.GetSalePriceRows())
                            {
                                exportWriter.WriteStartElement("SalePrice", "");

                                exportWriter.WriteElementString("Currency", priceRow.Currency.ToString());
                                exportWriter.WriteElementString("EndDate", priceRow.IsEndDateNull() ? String.Empty : priceRow.EndDate.ToString("u"));
                                exportWriter.WriteElementString("StartDate", priceRow.StartDate.ToString("u"));
                                //exportWriter.WriteElementString("ItemCode", priceRow.ItemCode.ToString());
								exportWriter.WriteElementString("MinQuantity", priceRow.MinQuantity.ToString(currentCultureInfo));
								exportWriter.WriteElementString("SaleCode", priceRow.IsSaleCodeNull() ? String.Empty : priceRow.SaleCode.ToString(currentCultureInfo));
								exportWriter.WriteElementString("SaleType", priceRow.SaleType.ToString(currentCultureInfo));
								exportWriter.WriteElementString("UnitPrice", priceRow.UnitPrice.ToString(currentCultureInfo));

                                exportWriter.WriteEndElement(); // End of Variation

                            }
                            exportWriter.WriteEndElement(); // End of VariationInfo
                        }
                        #endregion

                        #region SeoInfo

                        exportWriter.WriteStartElement("SeoInfo", "");

                        //Get GetCatalogItemSeoRows
                        foreach (Catalog.Dto.CatalogEntryDto.CatalogItemSeoRow itemEntrySeo in itemEntry.GetCatalogItemSeoRows())
                        {
                            exportWriter.WriteStartElement("Seo", "");

                            exportWriter.WriteElementString("LanguageCode", itemEntrySeo.LanguageCode);
                            exportWriter.WriteElementString("Uri", itemEntrySeo.Uri);
                            exportWriter.WriteElementString("Title", itemEntrySeo.IsTitleNull() ? String.Empty : itemEntrySeo.Title);
                            exportWriter.WriteElementString("Description", itemEntrySeo.IsDescriptionNull() ? String.Empty : itemEntrySeo.Description);
                            exportWriter.WriteElementString("Keywords", itemEntrySeo.IsKeywordsNull() ? String.Empty : itemEntrySeo.Keywords);

                            exportWriter.WriteEndElement(); // End of Seo
                        }
                        exportWriter.WriteEndElement(); // End of SeoInfo
                        #endregion

                        exportWriter.WriteEndElement(); // End of Entry
                        exportWriter.Flush(); // Flush buffer to output stream
                    }

                    // Break the loop if we retrieved all the records
					if (currentRecord > totalEntriesCount)
					{
						if (totalEntriesCount > 0)
							OnEvent(String.Format("Exported {0} of {1} entries", totalEntriesCount, totalEntriesCount), GetProgressPercent2((int)ExportSteps.StartExportCatalogEntries + 1f, totalExportSteps));
						break;
					}

					OnEvent(String.Format("Exported {0} of {1} entries", currentRecord, totalEntriesCount), GetProgressPercent2((int)ExportSteps.StartExportCatalogEntries + (double)currentRecord / totalEntriesCount, totalExportSteps));
                }

                exportWriter.WriteEndElement(); // End of Entries

				#endregion Entries
                
                exportWriter.Flush(); // Flush buffer to output stream

				OnEvent("Finished exporting catalog entries", GetProgressPercent((int)ExportSteps.EndExportCatalogEntres, totalExportSteps));

				OnEvent("Exporting relations...", GetProgressPercent((int)ExportSteps.StartExportCatalogRelations, totalExportSteps));

                #region Relations
                exportWriter.WriteStartElement("Relations", "");

                //CatalogNodeRelation export
                foreach (DataRowView row in catalogNodeRelationDataView)
                {
                    string childNodeCode = String.Empty;
                    string parentNodeCode = String.Empty;

                    //get node code
                    catalogNodeDataView.RowFilter = String.Format("CatalogNodeId = {0}", row["ChildNodeId"]);
                    if(catalogNodeDataView.Count > 0)
                        childNodeCode = catalogNodeDataView[0]["Code"].ToString();

                    //get node code
                    catalogNodeDataView.RowFilter = String.Format("CatalogNodeId = {0}", row["ParentNodeId"]);
                    if(catalogNodeDataView.Count > 0)
                        parentNodeCode = catalogNodeDataView[0]["Code"].ToString();

                    if (!String.IsNullOrEmpty(childNodeCode) && !String.IsNullOrEmpty(parentNodeCode))
                    {
                        exportWriter.WriteStartElement("NodeRelation", "");
                        
                        exportWriter.WriteElementString("ChildNodeCode", childNodeCode);
                        exportWriter.WriteElementString("ParentNodeCode", parentNodeCode);
						exportWriter.WriteElementString("SortOrder", row["SortOrder"].ToString());

                        exportWriter.WriteEndElement(); // End of NodeRelation
                    }
                }

                //NodeEntryRelation export
                foreach (DataRowView row in nodeEntryRelationDataView)
                {
                    string catalogEntryCode = String.Empty;
                    string catalogNodeCode = String.Empty;

                    //get parent entry code
                    catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto((int)row["CatalogEntryId"], new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                    if (catalogEntryDto.CatalogEntry.Count > 0)
                    {
                        //ignore entry relation whith other catalog 
                        if (catalogEntryDto.CatalogEntry[0].CatalogId != catalogRow.CatalogId)
                            continue;

                        catalogEntryCode = catalogEntryDto.CatalogEntry[0].Code;
                    }

                    //get node code
                    catalogNodeDataView.RowFilter = String.Format("CatalogNodeId = {0}", row["CatalogNodeId"]);
                    if(catalogNodeDataView.Count > 0)
                        catalogNodeCode = catalogNodeDataView[0]["Code"].ToString();

                    if (!String.IsNullOrEmpty(catalogEntryCode) && !String.IsNullOrEmpty(catalogNodeCode))
                    {
                        exportWriter.WriteStartElement("NodeEntryRelation", "");

                        exportWriter.WriteElementString("EntryCode", catalogEntryCode);
                        exportWriter.WriteElementString("NodeCode", catalogNodeCode);
						exportWriter.WriteElementString("SortOrder", row["SortOrder"].ToString());

                        exportWriter.WriteEndElement(); // End of NodeEntryRelation
                    }
                }

                //CatalogEntryRelation export
                foreach (DataRowView row in catalogEntryRelationDataView)
                {
                    string parentEntryCode = String.Empty;
                    string childEntryCode = String.Empty;

                    //get parent entry code
                    catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto((int)row["ParentEntryId"], new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                    if (catalogEntryDto.CatalogEntry.Count > 0)
                    {
                        //ignore entry relation whith other catalog 
                        if (catalogEntryDto.CatalogEntry[0].CatalogId != catalogRow.CatalogId)
                            continue;

                        parentEntryCode = catalogEntryDto.CatalogEntry[0].Code;
                    }

                    //get child entry code
                    catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto((int)row["ChildEntryId"], new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                    if (catalogEntryDto.CatalogEntry.Count > 0)
                    {
                        //ignore entry relation whith other catalog 
                        if (catalogEntryDto.CatalogEntry[0].CatalogId != catalogRow.CatalogId)
                            continue;

                        childEntryCode = catalogEntryDto.CatalogEntry[0].Code;
                    }

                    if (!String.IsNullOrEmpty(parentEntryCode) && !String.IsNullOrEmpty(childEntryCode))
                    {
                        exportWriter.WriteStartElement("EntryRelation", "");

                        exportWriter.WriteElementString("ParentEntryCode", parentEntryCode);
                        exportWriter.WriteElementString("ChildEntryCode", childEntryCode);
						exportWriter.WriteElementString("RelationType", row["RelationTypeId"].ToString());
						exportWriter.WriteElementString("Quantity", row["Quantity"].ToString());
                        exportWriter.WriteElementString("GroupName", row["GroupName"].ToString());
						exportWriter.WriteElementString("SortOrder", row["SortOrder"].ToString());

                        exportWriter.WriteEndElement(); // End of EntryRelation
                    }
                }

                exportWriter.WriteEndElement(); // End of Relations
                #endregion Relations

                exportWriter.Flush(); // Flush buffer to output stream

				OnEvent("Exporting associations...", GetProgressPercent((int)ExportSteps.StartExportCatalogRelations, totalExportSteps));

				#region Associations
				exportWriter.WriteStartElement("Associations", "");

				catalogAssociationDto = CatalogAssociationManager.GetCatalogAssociationDtoByCatalogId(catalogRow.CatalogId);

				foreach (CatalogAssociationDto.CatalogAssociationRow catalogAssociationRow in catalogAssociationDto.CatalogAssociation.Rows)
				{
					//Get entry code
					Catalog.Dto.CatalogEntryDto entryDto = FrameworkContext.Current.CatalogSystem.GetCatalogEntryDto(catalogAssociationRow.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));

					if (entryDto.CatalogEntry.Count > 0)
					{
						//ignore entry relation whith other catalog 
						if (entryDto.CatalogEntry[0].CatalogId != catalogRow.CatalogId)
							continue;

						exportWriter.WriteStartElement("CatalogAssociation", "");

						string entryCode = entryDto.CatalogEntry[0].Code;

						exportWriter.WriteElementString("Name", catalogAssociationRow.AssociationName);
						exportWriter.WriteElementString("Description", catalogAssociationRow.IsAssociationDescriptionNull() ? String.Empty : catalogAssociationRow.AssociationDescription);
						exportWriter.WriteElementString("SortOrder", catalogAssociationRow.SortOrder.ToString());
						exportWriter.WriteElementString("EntryCode", entryCode);

						foreach (CatalogAssociationDto.CatalogEntryAssociationRow itemCatalogEntryAssociation in catalogAssociationRow.GetCatalogEntryAssociationRows())
						{
							//Get entry code
							entryDto = FrameworkContext.Current.CatalogSystem.GetCatalogEntryDto(itemCatalogEntryAssociation.CatalogEntryId, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));

							if (entryDto.CatalogEntry.Count > 0)
							{
								//ignore entry relation with other catalog 
								if (entryDto.CatalogEntry[0].CatalogId != catalogRow.CatalogId)
									continue;

								entryCode = entryDto.CatalogEntry[0].Code;
							
								exportWriter.WriteStartElement("Association", "");

								exportWriter.WriteElementString("EntryCode", entryCode);
								exportWriter.WriteElementString("SortOrder", itemCatalogEntryAssociation.SortOrder.ToString());
								exportWriter.WriteElementString("Type", itemCatalogEntryAssociation.AssociationTypeId);

								exportWriter.WriteEndElement(); // End of Association
							}
						}

						exportWriter.WriteEndElement(); // End of CatalogAssociation
					}
				}

				exportWriter.WriteEndElement(); // End of Associations
				#endregion Associations

                //for (int iTemp = 1; iTemp < 2000; iTemp++)
                //{
                //    Catalog.Dto.CatalogDto catalogDto2 = FrameworkContext.Current.CatalogSystem.GetCatalogDto(1);
                //}

                OnEvent("Finished exporting catalog relations and associations", GetProgressPercent((int)ExportSteps.EndExportCatalogRelations, totalExportSteps));

                exportWriter.WriteEndElement(); // End of Catalog
                exportWriter.Flush(); // Flush buffer to output stream

                // For now since this method only supports one CatalogName go ahead and break out of here
                break;
			}
			#endregion

			exportWriter.WriteEndElement(); // End of Catalogs
            exportWriter.Close(); // Close the XmlWriter Stream

            OnEvent("Export successfully finished.", GetProgressPercent((int)ExportSteps.Finish, totalExportSteps));
        }

        /// <summary>
        /// Writes the meta data.
        /// </summary>
        /// <param name="wr">The wr.</param>
        /// <param name="metaClassId">The meta class id.</param>
        /// <param name="metaObjectId">The meta object id.</param>
        /// <param name="languages">The languages.</param>
        /// <param name="baseFilePath">The base file path.</param>
        private static void WriteMetaData(XmlWriter wr, int metaClassId, int metaObjectId, string[] languages, string baseFilePath)
        {
			CultureInfo currentCultureInfo = GetImportExportCulture();

            MetaClass mc = MetaClass.Load(CatalogContext.MetaDataContext, metaClassId);

            // If there is no meta class in the system that corresponds to the id, then obviously there are not meta
            // data information to write here
            if (mc == null)
                return;

            wr.WriteStartElement("MetaData");

            
            wr.WriteStartElement("MetaClass");
            wr.WriteElementString("Name", null, mc.Name);
            wr.WriteEndElement(); // MetaClass

            #region Meta Fields
            wr.WriteStartElement("MetaFields");

            Dictionary<string, MetaObject> metaObjects = new Dictionary<string, MetaObject>();

            // cycle through each language and get meta objects
            CatalogContext.MetaDataContext.UseCurrentUICulture = false;
            foreach (string language in languages)
            {
                CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                CatalogContext.MetaDataContext.Language = language;

                MetaObject metaObj = null;
                if (metaObjectId > 0)
                {
                    metaObj = MetaObject.Load(CatalogContext.MetaDataContext, metaObjectId, mc);
                    if (metaObj == null)
                    {
                        metaObj = MetaObject.NewObject(CatalogContext.MetaDataContext, metaObjectId, metaClassId, FrameworkContext.Current.Profile.UserName);
                        metaObj.AcceptChanges(CatalogContext.MetaDataContext);
                    }
                }

                metaObjects[language] = metaObj;
            }
            CatalogContext.MetaDataContext.UseCurrentUICulture = true;

            MetaFieldCollection metaFieldsColl = mc.MetaFields;
            if (metaFieldsColl != null)
            {
                foreach (MetaField mf in metaFieldsColl)
                {
                    if (!mf.IsUser)
                        continue;

                    wr.WriteStartElement("MetaField");
                    wr.WriteElementString("Name", null, mf.Name);
                    wr.WriteElementString("Type", null, mf.DataType.ToString());

                    foreach (string language in languages)
                    {
                        switch (mf.DataType)
                        {
                            case MetaDataType.File:
                            case MetaDataType.Image:
                            case MetaDataType.ImageFile:
                                MetaFile metaFile = (MetaFile)metaObjects[language][mf];
                                if (metaFile != null)
                                {
                                    wr.WriteStartElement("FileData");
                                    wr.WriteAttributeString("language", null, language);
                                    WriteBinaryData(baseFilePath, metaFile.Buffer, wr);
                                    wr.WriteElementString("ContentType", null, metaFile.ContentType);
                                    wr.WriteElementString("Name", null, metaFile.Name);
                                    wr.WriteElementString("CreationTime", null, metaFile.CreationTime.ToString("u"));
                                    wr.WriteEndElement(); // FileData
                                }
                                else
                                    wr.WriteElementString("FileData", null, null);
                                break;
							case MetaDataType.Decimal:
								wr.WriteStartElement("Data");
								wr.WriteAttributeString("language", null, language);
								if (metaObjects[language][mf] != null)
								{
                                    wr.WriteAttributeString("value", String.Format(currentCultureInfo, "{0:G}", metaObjects[language][mf]));
                                }
								else
								{
									wr.WriteAttributeString("value", "");
								}
								wr.WriteEndElement();
								break;
							case MetaDataType.Real:
							case MetaDataType.Float:
								wr.WriteStartElement("Data");
								wr.WriteAttributeString("language", null, language);
								if (metaObjects[language][mf] != null)
								{
									wr.WriteAttributeString("value", String.Format(currentCultureInfo, "{0:F}", metaObjects[language][mf]));
								}
								else
								{
									wr.WriteAttributeString("value", "");
								}
								wr.WriteEndElement();
								break;
							case MetaDataType.SmallMoney:
                            case MetaDataType.Money:
                                wr.WriteStartElement("Data");
                                wr.WriteAttributeString("language", null, language);
                                if (metaObjects[language][mf] != null)
                                {
									wr.WriteAttributeString("value", String.Format(currentCultureInfo, "{0}", Math.Round((decimal)metaObjects[language][mf], 2)));
                                }
                                else
                                {
                                    wr.WriteAttributeString("value", "");
                                }
                                wr.WriteEndElement();
                                break;
                            case MetaDataType.Date:
                            case MetaDataType.DateTime:
                                wr.WriteStartElement("Data");
                                wr.WriteAttributeString("language", null, language);
                                if (metaObjects[language][mf] != null)
                                {
                                    wr.WriteAttributeString("value", ((DateTime)metaObjects[language][mf]).ToString("u"));
                                }
                                else
                                {
                                    wr.WriteAttributeString("value", "");
                                }
                                wr.WriteEndElement();
                                break;
							case MetaDataType.BigInt:
							case MetaDataType.Bit:
                            case MetaDataType.Boolean:
							case MetaDataType.Char:
                            case MetaDataType.Email:
							case MetaDataType.Int:
							case MetaDataType.Integer:
							case MetaDataType.NChar:
							case MetaDataType.Numeric:
                            case MetaDataType.ShortString:
							case MetaDataType.SmallInt:
                            case MetaDataType.LongString:
                            case MetaDataType.LongHtmlString:
							case MetaDataType.NText:
							case MetaDataType.Text:
							case MetaDataType.Timestamp:
							case MetaDataType.TinyInt:
							case MetaDataType.UniqueIdentifier:
							case MetaDataType.URL:
							case MetaDataType.VarChar:
                            case MetaDataType.DictionarySingleValue:
                            case MetaDataType.EnumSingleValue:
                                wr.WriteStartElement("Data");
                                wr.WriteAttributeString("language", null, language);
                                if (metaObjects[language][mf] != null)
                                {
                                    wr.WriteAttributeString("value", metaObjects[language][mf].ToString());
                                }
                                else
                                {
                                    wr.WriteAttributeString("value", "");
                                }
                                wr.WriteEndElement();
                                break;
                            case MetaDataType.DictionaryMultiValue:
                            case MetaDataType.EnumMultiValue:
                                wr.WriteStartElement("Data");
                                wr.WriteAttributeString("language", null, language);
                                MetaDictionaryItem[] dictionaryItems = (MetaDictionaryItem[])metaObjects[language][mf];
								if (dictionaryItems != null)
								{
									foreach (MetaDictionaryItem item in dictionaryItems)
									{
										wr.WriteStartElement("Item");
										wr.WriteAttributeString("value", null, item.Value);
										wr.WriteEndElement();
									}
								}
                                wr.WriteEndElement();
                                break;
                            case MetaDataType.StringDictionary:
                                wr.WriteStartElement("Data");
                                wr.WriteAttributeString("language", null, language);
                                MetaStringDictionary dic = (MetaStringDictionary)metaObjects[language][mf];
                                if (dic != null)
                                {
                                    foreach (string key in dic.Keys)
                                    {
                                        wr.WriteStartElement("Item");
                                        wr.WriteAttributeString("key", null, key);
                                        wr.WriteAttributeString("value", null, dic[key]);
                                        wr.WriteEndElement();
                                    }
                                }
                                wr.WriteEndElement();
                                break;
                            default:
								wr.WriteStartElement("Data");
                                wr.WriteAttributeString("language", null, language);
								wr.WriteEndElement();
                                break;
                        }

                        if (!mf.MultiLanguageValue)
                            break;
                    }
                    wr.WriteEndElement();
                }
            }
            wr.WriteEndElement(); // MetaFields
            #endregion

            wr.WriteEndElement(); // MetaData
        }

        /// <summary>
        /// Writes the binary data.
        /// </summary>
        /// <param name="baseFilePath">The base file path.</param>
        /// <param name="data">The data.</param>
        /// <param name="wr">The wr.</param>
        private static void WriteBinaryData(string baseFilePath, byte[] data, XmlWriter wr)
        {
            if (data != null)
            {
                Guid gd = Guid.NewGuid();
                FileStream fs = new FileStream(baseFilePath + "\\" + gd.ToString("D"), FileMode.Create, FileAccess.Write, FileShare.Read);
                fs.Write(data, 0, data.Length);
                fs.Close();
                wr.WriteElementString("DiskFileName", null, gd.ToString("D"));
            }
            else
                wr.WriteElementString("DiskFileName", null, null);
		}

		#region Export Dictionaries
		/// <summary>
		/// Writes Merchants dictionary.
		/// </summary>
		/// <param name="wr">The wr.</param>
		private static void WriteMerchants(XmlWriter wr)
		{
			wr.WriteStartElement("Merchants");

			CatalogEntryDto dto = CatalogContext.Current.GetMerchantsDto();

			// if there are any merchants, export them
			if (dto != null && dto.Merchant.Count > 0)
			{
				// write the merchants
				foreach (CatalogEntryDto.MerchantRow merchant in dto.Merchant.Rows)
				{
					wr.WriteStartElement("Merchant");
					wr.WriteElementString("Name", null, merchant.Name);
					wr.WriteEndElement(); // Merchant
				}
			}

			wr.WriteEndElement(); // Merchants
		}

		/// <summary>
		/// Writes Packages dictionary.
		/// </summary>
		/// <param name="wr">The wr.</param>
		private static void WritePackages(XmlWriter wr)
		{
			CultureInfo currentCultureInfo = GetImportExportCulture();

			wr.WriteStartElement("Packages");

			ShippingMethodDto dto = ShippingManager.GetShippingPackages();

			// if there are any packages, export them
			if (dto != null && dto.Package.Count > 0)
			{
				// write the packages
				foreach (ShippingMethodDto.PackageRow package in dto.Package.Rows)
				{
					wr.WriteStartElement("Package");
					wr.WriteElementString("Name", null, package.Name);
					wr.WriteElementString("Description", null, package.IsDescriptionNull() ? String.Empty : package.Description);
					wr.WriteElementString("Width", null, package.IsWidthNull() ? String.Empty : package.Width.ToString(currentCultureInfo));
					wr.WriteElementString("Length", null, package.IsLengthNull() ? String.Empty : package.Length.ToString(currentCultureInfo));
					wr.WriteElementString("Height", null, package.IsHeightNull() ? String.Empty : package.Height.ToString(currentCultureInfo));
					wr.WriteEndElement(); // Package
				}
			}

			wr.WriteEndElement(); // Packages
		}

		/// <summary>
		/// Writes TaxCategories dictionary.
		/// </summary>
		/// <param name="wr">The wr.</param>
		private static void WriteTaxCategories(XmlWriter wr)
		{
			wr.WriteStartElement("TaxCategories");

			CatalogTaxDto dto = CatalogTaxManager.GetTaxCategories();

			// if there are any tax categories, export them
			if (dto != null && dto.TaxCategory.Count > 0)
			{
				// write the tax categories
				foreach (CatalogTaxDto.TaxCategoryRow tc in dto.TaxCategory.Rows)
				{
					wr.WriteStartElement("TaxCategory");
					wr.WriteElementString("Name", null, tc.Name);
					wr.WriteEndElement(); // TaxCategory
				}
			}

			wr.WriteEndElement(); // TaxCategories
		}

		/// <summary>
		/// Writes Warehouses dictionary.
		/// </summary>
		/// <param name="wr">The wr.</param>
		private static void WriteWarehouses(XmlWriter wr)
		{
			CultureInfo currentCultureInfo = GetImportExportCulture();

			wr.WriteStartElement("Warehouses");

			WarehouseDto dto = WarehouseManager.GetWarehouseDto();

			// if there are any warehouses, export them
			if (dto != null && dto.Warehouse.Count > 0)
			{
				// write the warehouse
				foreach (WarehouseDto.WarehouseRow wh in dto.Warehouse.Rows)
				{
					wr.WriteStartElement("Warehouse");
					wr.WriteElementString("Name", null, wh.Name);
					wr.WriteElementString("IsActive", null, wh.IsActive.ToString(currentCultureInfo));
					wr.WriteElementString("IsPrimary", null, wh.IsPrimary.ToString(currentCultureInfo));
					wr.WriteElementString("SortOrder", null, wh.SortOrder.ToString(currentCultureInfo));
					wr.WriteElementString("Code", null, wh.Code);
					wr.WriteElementString("FirstName", null, wh.IsFirstNameNull() ? String.Empty : wh.FirstName);
					wr.WriteElementString("LastName", null, wh.IsLastNameNull() ? String.Empty : wh.LastName);
					wr.WriteElementString("Organization", null, wh.IsOrganizationNull() ? String.Empty : wh.Organization);
					wr.WriteElementString("Line1", null, wh.IsLine1Null() ? String.Empty : wh.Line1);
					wr.WriteElementString("Line2", null, wh.IsLine2Null() ? String.Empty : wh.Line2);
					wr.WriteElementString("City", null, wh.IsCityNull() ? String.Empty : wh.City);
					wr.WriteElementString("State", null, wh.IsStateNull() ? String.Empty : wh.State);
					wr.WriteElementString("CountryCode", null, wh.IsCountryCodeNull() ? String.Empty : wh.CountryCode);
					wr.WriteElementString("CountryName", null, wh.IsCountryNameNull() ? String.Empty : wh.CountryName);
					wr.WriteElementString("PostalCode", null, wh.IsPostalCodeNull() ? String.Empty : wh.PostalCode);
					wr.WriteElementString("RegionCode", null, wh.IsRegionCodeNull() ? String.Empty : wh.RegionCode);
					wr.WriteElementString("RegionName", null, wh.IsRegionNameNull() ? String.Empty : wh.RegionName);
					wr.WriteElementString("DaytimePhoneNumber", null, wh.IsDaytimePhoneNumberNull() ? String.Empty : wh.DaytimePhoneNumber);
					wr.WriteElementString("EveningPhoneNumber", null, wh.IsEveningPhoneNumberNull() ? String.Empty : wh.EveningPhoneNumber);
					wr.WriteElementString("FaxNumber", null, wh.IsFaxNumberNull() ? String.Empty : wh.FaxNumber);
					wr.WriteElementString("Email", null, wh.IsEmailNull() ? String.Empty : wh.Email);
					wr.WriteEndElement(); // Warehouses
				}
			}

			wr.WriteEndElement(); // Warehouses
		}

		/// <summary>
		/// Writes AssociationType dictionary.
		/// </summary>
		/// <param name="wr">The wr.</param>
		private static void WriteAssociationTypes(XmlWriter wr)
		{
			wr.WriteStartElement("AssociationTypes");

			CatalogAssociationDto dto = CatalogAssociationManager.GetCatalogAssociationDto(0);

			// if there are any association types, export them
			if (dto != null && dto.AssociationType.Count > 0)
			{
				// write the association types
				foreach (CatalogAssociationDto.AssociationTypeRow at in dto.AssociationType.Rows)
				{
					wr.WriteStartElement("AssociationType");
					wr.WriteElementString("TypeId", null, at.AssociationTypeId);
					wr.WriteElementString("Description", null, at.IsDescriptionNull() ? String.Empty : at.Description);
					wr.WriteEndElement(); // AssociationType
				}
			}

			wr.WriteEndElement(); // AssociationTypes
		}
		#endregion
		#endregion

		#region Import
		/// <summary>
        /// Imports the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="applicationId">The application id.</param>
        /// <param name="baseFilePath">The base file path.</param>
        public void Import(Stream input, Guid applicationId, string baseFilePath)
        {
			CultureInfo currentCultureInfo = GetImportExportCulture();

			int totalImportSteps = GetTotalImportSteps();

            OnEvent("Starting import...", 0);

            XmlReaderSettings xmlReadSettings = new XmlReaderSettings();
            xmlReadSettings.IgnoreComments = true;
            xmlReadSettings.IgnoreWhitespace = true;

            XmlReader importReader = XmlReader.Create(input, xmlReadSettings);

            importReader.Read(); // skip over declaration

            importReader.Read(); // read catalogs

            // Get the Version
            string version = importReader.GetAttribute("version");

            //TODO: Read Schema node

            //Catalog.Dto.CatalogDto catalogDto = new Catalog.Dto.CatalogDto();
            Catalog.Dto.CatalogDto.CatalogRow catalogRow = null;
            Catalog.Data.CatalogAdmin catalogAdmin = new Mediachase.Commerce.Catalog.Data.CatalogAdmin();
            Catalog.Data.CatalogNodeAdmin nodeAdmin = new Mediachase.Commerce.Catalog.Data.CatalogNodeAdmin();

            // Holder variables
            DateTime parsedDate = DateTime.MinValue;
            int parsedInt = 0;
            bool parsedBool = false;

            while (importReader.Read() && !importReader.EOF)
            {
				if (importReader.IsStartElement() && !importReader.IsEmptyElement)
				{
					#region MetaDataPlusBackup
					if (importReader.LocalName.Equals("MetaDataScheme"))
					{
						OnEvent("Start importing MetaData scheme", GetProgressPercent((int)ImportSteps.StartImportMetaDataSchema, totalImportSteps));
						string mdpBackupXML = importReader.ReadInnerXml();
						MetaInstaller.Restore(CatalogContext.MetaDataContext, mdpBackupXML);
						OnEvent("Finished importing MetaData scheme", GetProgressPercent((int)ImportSteps.EndImportMetaDataSchema, totalImportSteps));
					}
					#endregion

					#region Dictionaries
					if (importReader.LocalName.Equals("Dictionaries"))
					{
						OnEvent("Start importing Dictionaries", GetProgressPercent((int)ImportSteps.StartImportDictionaries, totalImportSteps));
						XmlReader dictionariesReader = importReader.ReadSubtree();
						while (dictionariesReader.Read() && !dictionariesReader.EOF)
						{
							if (dictionariesReader.LocalName.Equals("Merchants"))
								ReadMerchants(dictionariesReader);
							if (dictionariesReader.LocalName.Equals("Packages"))
								ReadPackages(dictionariesReader);
							if (dictionariesReader.LocalName.Equals("TaxCategories"))
								ReadTaxCategories(dictionariesReader);
							if (dictionariesReader.LocalName.Equals("Warehouses"))
								ReadWarehouses(dictionariesReader);
							if (dictionariesReader.LocalName.Equals("AssociationTypes"))
								ReadAssociationTypes(dictionariesReader);
						}
						OnEvent("Finished importing Dictionaries", GetProgressPercent((int)ImportSteps.EndImportDictionaries, totalImportSteps));
					}
					#endregion

					OnEvent("Start importing catalog properties", GetProgressPercent((int)ImportSteps.StartImportCatalogProperties, totalImportSteps));
					#region Catalog
					// Get the Catalog row
					if (importReader.LocalName.Equals("Catalog"))
					{
						string catalogName = importReader.GetAttribute("name");

						// Get the Catalog
						CatalogDto catalogDto = FrameworkContext.Current.CatalogSystem.GetCatalogDto();
						DataRow[] rows = catalogDto.Catalog.Select(String.Format("Name LIKE '{0}'", catalogName.Replace("'", "''")));
						if (rows.Length > 0)
						{
							//throw new ImportExportException(String.Format("Catalog '{0}' already exists", catalogName));
							OnEvent(String.Format("Catalog '{0}' already exists", catalogName), GetProgressPercent((int)ImportSteps.StartImportCatalogProperties, totalImportSteps), ImportExportMessageType.Warning);
							catalogAdmin.CurrentDto.Catalog.ImportRow(catalogDto.Catalog[0]);
							catalogRow = catalogAdmin.CurrentDto.Catalog[0];
						}
						else
						{
							catalogRow = catalogAdmin.CurrentDto.Catalog.NewCatalogRow(); // Create a Row

							//set non-nullable properties of catalog row before inserting
							catalogRow.Name = catalogName;
							catalogRow.IsActive = true;
							catalogRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
							catalogRow.Created = DateTime.UtcNow;
							catalogRow.Modified = DateTime.UtcNow;
							catalogRow.SortOrder = 0;
							catalogAdmin.CurrentDto.Catalog.AddCatalogRow(catalogRow);
						}

						if (DateTime.TryParse(importReader.GetAttribute("lastmodified"), out parsedDate))
							catalogRow.Modified = parsedDate.ToUniversalTime();

						if (DateTime.TryParse(importReader.GetAttribute("startDate"), out parsedDate))
							catalogRow.StartDate = parsedDate.ToUniversalTime();

						if (DateTime.TryParse(importReader.GetAttribute("endDate"), out parsedDate))
							catalogRow.EndDate = parsedDate.ToUniversalTime();

						catalogRow.DefaultCurrency = importReader.GetAttribute("defaultCurrency");
						catalogRow.WeightBase = importReader.GetAttribute("weightBase");
						catalogRow.DefaultLanguage = importReader.GetAttribute("defaultLanguage");

						Int32.TryParse(importReader.GetAttribute("sortOrder"), NumberStyles.Integer, currentCultureInfo, out parsedInt);
						catalogRow.SortOrder = parsedInt;
						Boolean.TryParse(importReader.GetAttribute("isActive"), out parsedBool);
						catalogRow.IsActive = parsedBool;

						#region Default Values Set

						// Default values set that are not present in Export
						catalogRow.Created = DateTime.UtcNow;
						catalogRow.ApplicationId = applicationId;
						catalogRow.IsPrimary = false;
						#endregion

						// Save row
						catalogAdmin.Save();

                        #region CatalogLanguage

                        //Add Languages, they are comma delimited
						string languages = importReader.GetAttribute("languages");
						foreach (string language in languages.Split(','))
						{
							if (String.Equals(language, catalogRow.DefaultLanguage))
								continue;

							if (catalogDto.CatalogLanguage.Select(String.Format("LanguageCode LIKE '{0}'", language.Replace("'", "''"))).Length > 0)
								continue;

							Catalog.Dto.CatalogDto.CatalogLanguageRow catalogLanguageRow = catalogAdmin.CurrentDto.CatalogLanguage.NewCatalogLanguageRow();
							catalogLanguageRow.LanguageCode = language;
							catalogLanguageRow.CatalogId = catalogRow.CatalogId;
							catalogAdmin.CurrentDto.CatalogLanguage.AddCatalogLanguageRow(catalogLanguageRow);
                        }
                        #endregion

                        if (catalogAdmin.CurrentDto.HasChanges())
							catalogAdmin.Save();

                        
                    }
					#endregion
					OnEvent("Finished importing catalog properties", GetProgressPercent((int)ImportSteps.EndImportCatalogProperties, totalImportSteps));

					int totalCount = 0; // used for storing total nodes/entries count

					switch (importReader.LocalName)
					{
                        case "Sites":
                            ReadSites(catalogRow.CatalogId, importReader, FrameworkContext.Current.CatalogSystem.GetCatalogDto(), catalogAdmin);
                            break;
						case "Nodes":
							OnEvent("Start importing catalog nodes", GetProgressPercent((int)ImportSteps.StartImportNodes, totalImportSteps));
							Int32.TryParse(importReader.GetAttribute("totalCount"), out totalCount);
							nodeAdmin = ReadNodes(catalogRow.CatalogId, importReader, baseFilePath, totalCount);
							OnEvent("Finished importing catalog nodes", GetProgressPercent((int)ImportSteps.EndImportNodes, totalImportSteps));
							break;
						case "Entries":
							OnEvent("Start importing catalog entries", GetProgressPercent((int)ImportSteps.StartImportEntries, totalImportSteps));
							Int32.TryParse(importReader.GetAttribute("totalCount"), out totalCount);
							ReadEntries(catalogRow.CatalogId, importReader, baseFilePath, totalCount);
							OnEvent("Finished importing catalog entries", GetProgressPercent((int)ImportSteps.EndImportEntries, totalImportSteps));
							break;
						case "Relations":
							OnEvent("Start importing relations", GetProgressPercent((int)ImportSteps.StartImportRelations, totalImportSteps));
							ReadRelations(catalogRow.CatalogId, importReader, nodeAdmin);
							OnEvent("Finished importing relations", GetProgressPercent((int)ImportSteps.EndImportRelations, totalImportSteps));
							break;
						case "Associations":
							OnEvent("Start importing associations", GetProgressPercent((int)ImportSteps.StartImportAssociations, totalImportSteps));
							ReadAssociations(catalogRow.CatalogId, importReader);
							OnEvent("Finished importing associations", GetProgressPercent((int)ImportSteps.EndImportAssociations, totalImportSteps));
							break;
					}
				}
            }

			OnEvent("Import successfully finished.", 100);
        }

        #region ReadSites
        /// <summary>
        /// Reads the site id's.
        /// </summary>
        /// <param name="CatalogId">The catalog id.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="catalogDto">The catalog Dto.</param>
        /// <param name="catalogAdmin">The node admin.</param>
        /// <returns></returns>
        private void ReadSites(int CatalogId, XmlReader reader, CatalogDto catalogDto, Catalog.Data.CatalogAdmin catalogAdmin)
        {
            // Get the Subtree
            XmlReader readSubtree = reader.ReadSubtree();

            while (readSubtree.Read() && !readSubtree.EOF)
            {
                if (!readSubtree.IsEmptyElement)
                {
                    if (readSubtree.LocalName.Equals("Site"))
                    {
                        Guid SiteId = Guid.Empty;

                        string id = readSubtree.ReadString();
                        if (!String.IsNullOrEmpty(id))
                        {
                            try
                            {
                                SiteId = new Guid(id);
                            }
                            catch { }
                        }

                        if (SiteId != Guid.Empty)
                        {
                            if (catalogDto.SiteCatalog.FindByCatalogIdSiteId(CatalogId, SiteId) != null)
                                continue;

                            // Now make sure site belongs to the current application id
                            Catalog.Dto.CatalogDto.SiteCatalogRow siteCatalogRow = catalogAdmin.CurrentDto.SiteCatalog.NewSiteCatalogRow();
                            siteCatalogRow.SiteId = SiteId;
                            siteCatalogRow.CatalogId = CatalogId;
                            catalogAdmin.CurrentDto.SiteCatalog.AddSiteCatalogRow(siteCatalogRow);
                        }
                    }
                }
            }

            if (catalogAdmin.CurrentDto.HasChanges())
                catalogAdmin.Save();
        }
        #endregion

        /// <summary>
        /// Reads the nodes.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="baseFilePath">The base file path.</param>
		/// <param name="totalCount">Total nodes count. Used for more precise calculating of import progress.</param>
        /// <returns></returns>
        private Catalog.Data.CatalogNodeAdmin ReadNodes(int catalogId, XmlReader reader, string baseFilePath, int totalCount)
        {
			CultureInfo currentCultureInfo = GetImportExportCulture();

            int totalImportSteps = GetTotalImportSteps();

            // Get the Subtree
            XmlReader readSubtree = reader.ReadSubtree();

            // Variables for Parsing
            DateTime parsedDateTime;
            bool parsedBool;
            int parsedInt;

            bool hasError = false;

            // Place holder for Parent Nodes
            // Storage is Key = CatalogNodeId, Value = ParentNodeCode
            // Then once we have added the nodes we will iterrate through
            // the table and assign the ParentNode id based on ParentNodeCode record Id
            Hashtable parentNodes = new Hashtable();
            Hashtable parentNodeRelations = new Hashtable();
            string curParentNode = string.Empty;
            Dictionary<string, MetaObject> metaObjects = null;

            // Row identifiers
            CatalogNodeDto catalogNodeDto = new CatalogNodeDto();
			CatalogNodeDto.CatalogNodeRow nodeRow = null;
            CatalogNodeDto.CatalogItemSeoRow seoRow = null;

            Catalog.Data.CatalogNodeAdmin nodeAdmin = new Mediachase.Commerce.Catalog.Data.CatalogNodeAdmin();

			int nodeCount = 0;
			double currentPercent = 0;

            while (readSubtree.Read() && !readSubtree.EOF)
            {
                // Check if we have reached the end of Node Element
                if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Node") && !hasError)
                {
					nodeCount++;

					if (nodeCount % 20 == 0)
					{
						if (totalCount > 0)
							OnEvent(String.Format("Imported catalog nodes: {0} of {1}.", nodeCount, totalCount), currentPercent);
						else
							OnEvent(String.Format("Imported catalog nodes: {0}.", nodeCount), currentPercent);
					}

                    // Check if Detached, if not then this row was already saved
                    if (nodeRow.RowState == DataRowState.Detached)
                    {
                        nodeAdmin.CurrentDto.CatalogNode.AddCatalogNodeRow(nodeRow);
                        nodeAdmin.Save(); // Save our node

                        // Add the new Id and Parent Code if exists to our table
                        if (!String.IsNullOrEmpty(curParentNode))
                        {
                            parentNodes.Add(nodeRow.CatalogNodeId, curParentNode);
                            curParentNode = string.Empty; // Reset so another node doesnt get this parent
                        }
                    }
                }

                // Check if we are beginning a Node Element, create a row if so
                if (readSubtree.IsStartElement("Node"))
                {
                    hasError = false;
                    catalogNodeDto = new CatalogNodeDto();
					nodeRow = nodeAdmin.CurrentDto.CatalogNode.NewCatalogNodeRow();
                    nodeRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                    nodeRow.CatalogId = catalogId;

                    // Default the ParentNodeId to 0
                    // The actual Id will be assigned after the nodes have been added
                    nodeRow.ParentNodeId = 0;

                    // Default the Code to empty string
                    nodeRow.Code = String.Empty;

					// calculate purrent progress percent as soon as we read start Node node
					if (totalCount > 0)
						currentPercent = GetProgressPercent2((int)ImportSteps.StartImportNodes + (double)nodeCount / totalCount, totalImportSteps);
					else
						currentPercent = GetProgressPercent((int)ImportSteps.StartImportNodes, totalImportSteps);
                }

                // Ignore Empty Elements
                if (!readSubtree.IsEmptyElement && nodeRow != null && !hasError)
                {
                    // Name
                    if (readSubtree.LocalName.Equals("Name"))
                        nodeRow.Name = readSubtree.ReadString();

                    // Start Date
                    if (readSubtree.LocalName.Equals("StartDate"))
                    {
                        DateTime.TryParse(readSubtree.ReadString(), out parsedDateTime);
                        if (parsedDateTime != DateTime.MinValue)
							nodeRow.StartDate = parsedDateTime.ToUniversalTime();
                    }

                    // End Date
                    if (readSubtree.LocalName.Equals("EndDate"))
                    {
                        DateTime.TryParse(readSubtree.ReadString(), out parsedDateTime);
                        if (parsedDateTime != DateTime.MinValue)
							nodeRow.EndDate = parsedDateTime.ToUniversalTime();
                    }

                    // IsActive
                    if (readSubtree.LocalName.Equals("IsActive"))
                    {
                        Boolean.TryParse(readSubtree.ReadString(), out parsedBool);
                        nodeRow.IsActive = parsedBool;
                    }

                    // SortOrder
                    if (readSubtree.LocalName.Equals("SortOrder"))
                    {
                        Int32.TryParse(readSubtree.ReadString(), NumberStyles.Integer, currentCultureInfo, out parsedInt);
                        nodeRow.SortOrder = parsedInt;
                    }

                    // Template
                    if (readSubtree.LocalName.Equals("DisplayTemplate"))
                        nodeRow.TemplateName = readSubtree.ReadString();

                    // Code
                    if (readSubtree.LocalName.Equals("Code"))
                        nodeRow.Code = readSubtree.ReadString();

                    #region Process MetaData

                    // MetaData
                    if (readSubtree.LocalName.Equals("MetaData") && !hasError)
                    {
                        if (String.IsNullOrEmpty(nodeRow.Code))
                        {
							OnEvent(String.Format("Node '{0}' has empty code.", nodeRow.Name), currentPercent, ImportExportMessageType.Warning);
                            hasError = true;
                            continue;
                        }
                        else
                        {
                            catalogNodeDto = FrameworkContext.Current.CatalogSystem.GetCatalogNodeDto(nodeRow.Code, new CatalogNodeResponseGroup(CatalogNodeResponseGroup.ResponseGroup.CatalogNodeInfo));
                            if (catalogNodeDto.CatalogNode.Count > 0)
								OnEvent(String.Format("Node with code '{0}' already exists", nodeRow.Code), currentPercent, ImportExportMessageType.Warning);
                        }

                        MetaClass catalogNodeMetaClass = null;

                        while (readSubtree.Read() && !readSubtree.EOF)
                        {
                            if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("MetaData"))
                            {
                                break; // We are at the end of MetaData so break out of loop
                            }

                            // Check if beginning of MetaClass
                            if (readSubtree.NodeType == XmlNodeType.Element && readSubtree.LocalName.Equals("MetaClass"))
                            {
                                while (readSubtree.Read() && !readSubtree.EOF)
                                {
                                    if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("MetaClass"))
                                    {
                                        // Check if Detached, if not then this row was already saved
                                        if (nodeRow.RowState == DataRowState.Detached && catalogNodeMetaClass != null)
                                        {
                                            nodeRow.MetaClassId = catalogNodeMetaClass.Id;

											if (catalogNodeDto.CatalogNode.Count > 0)
											{
                                                nodeAdmin.CurrentDto.CatalogNode.ImportRow(catalogNodeDto.CatalogNode[0]);
                                                nodeRow = nodeAdmin.CurrentDto.CatalogNode[nodeAdmin.CurrentDto.CatalogNode.Count - 1];
                                                /* Sasha: is this needed? we should reindex the catalog when the new one is imported
												nodeRow.CatalogNodeId = catalogNodeDto.CatalogNode[0].CatalogNodeId;
												nodeAdmin.CurrentDto.CatalogNode.Rows.Add(nodeRow);                                                
												nodeRow.AcceptChanges();
												nodeRow.SetModified();
												nodeAdmin.Save(); // Save our node
                                                * */
											}
											else
											{
												nodeAdmin.CurrentDto.CatalogNode.Rows.Add(nodeRow);
												nodeAdmin.Save(); // Save our node prior to adding constrained records
											}
                                        }

                                        break; // We are at the end of MetaClass so break out of loop
                                    } 

                                    if (readSubtree.LocalName.Equals("Name"))
                                    {
                                        string metaClassName = readSubtree.ReadString();
                                        catalogNodeMetaClass = MetaClass.Load(CatalogContext.MetaDataContext, metaClassName);
                                    }
                                }
                            }

                            // Check if beginning of MetaFields
                            if (readSubtree.IsStartElement("MetaFields"))
                            {
                                metaObjects = new Dictionary<string, MetaObject>();

                                ReadMetaFields(metaObjects, readSubtree, nodeRow.CatalogNodeId, catalogNodeMetaClass, baseFilePath);

                                foreach (string language in metaObjects.Keys)
                                {
                                    CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                                    CatalogContext.MetaDataContext.Language = language;

                                    MetaObject metaObject = metaObjects[language];
                                    //MetaObject metaObject = MetaObject.Load(CatalogContext.MetaDataContext, nodeRow.CatalogNodeId, nodeRow.MetaClassId);
                                    foreach (MetaField mf in metaObject.MetaClass.MetaFields)
                                    {
                                        if (mf.IsSystem)
                                            continue;

                                        // Now do some processing for non multi language values
                                        if (!mf.MultiLanguageValue)
                                        {
                                            object foundValue = null;
                                            // find not null values and copy it to the rest of the meta fields
                                            foreach (string ml in metaObjects.Keys)
                                            {
                                                if (metaObjects[language][mf] != null)
                                                {
                                                    foundValue = metaObjects[language][mf];
                                                    break;
                                                }
                                            }

                                            // Now distribute it, unless it is null
                                            if (foundValue != null)
                                            {
                                                foreach (string ml in metaObjects.Keys)
                                                {
                                                    if (metaObjects[language][mf] == null)
                                                        metaObjects[language][mf] = foundValue;
                                                }
                                            }
                                        }
                                        else // do normal logic
                                        {
                                            if (metaObjects[language][mf] != null)
                                                metaObject[mf] = metaObjects[language][mf];
                                        }
                                    }

                                    metaObject.AcceptChanges(CatalogContext.MetaDataContext);

                                    CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                                }

                            }
                        }
                    }
                    #endregion

                    // Parent Node Code
                    if (readSubtree.LocalName.Equals("ParentNode") && ! hasError)
                    {
                        curParentNode = readSubtree.ReadString();

                        // Add the new Id and Parent Code if exists to our table
                        if (!String.IsNullOrEmpty(curParentNode))
                        {
                            parentNodes.Add(nodeRow.CatalogNodeId, curParentNode);
                            curParentNode = string.Empty; // Reset so another node doesnt get this parent
                        }
                    }

                    #region Process Seo Items

                    // SEO info
                    if (readSubtree.LocalName.Equals("SeoInfo") && !hasError)
                    {
                        while (readSubtree.Read() && !readSubtree.EOF)
                        {
                            if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("SeoInfo"))
                                break; // We are at the end of SeoInfo so break out of loop

                            // Check if beginning of Seo
                            if (readSubtree.IsStartElement("Seo"))
                            {
                                seoRow = nodeAdmin.CurrentDto.CatalogItemSeo.NewCatalogItemSeoRow();
                                seoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                                seoRow.CatalogNodeId = nodeRow.CatalogNodeId;
                                seoRow.Title = String.Empty;
                                seoRow.Keywords = String.Empty;
                                seoRow.Description = String.Empty;
                            }

                            // Check if at the end of Seo
                            if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Seo"))
                            {
                                DataRow[] rows = catalogNodeDto.CatalogItemSeo.Select(String.Format("Uri = '{0}' AND LanguageCode ='{1}'", seoRow.Uri, seoRow.LanguageCode));
                                if (rows.Length == 0)
                                {
                                    nodeAdmin.CurrentDto.CatalogItemSeo.Rows.Add(seoRow);
                                    nodeAdmin.Save();
                                }
                                else
                                {
                                    //update Seo info
                                    nodeAdmin.CurrentDto.CatalogItemSeo.ImportRow(rows[0]);

                                    CatalogNodeDto.CatalogItemSeoRow itemSeoRow = nodeAdmin.CurrentDto.CatalogItemSeo[nodeAdmin.CurrentDto.CatalogItemSeo.Count - 1];
                                    itemSeoRow.Description = seoRow.Description;
                                    itemSeoRow.Keywords = seoRow.Keywords;
                                    itemSeoRow.Title = seoRow.Title;

                                    if (nodeAdmin.CurrentDto.HasChanges())
                                        nodeAdmin.Save();
                                }
                            }

                            if (!readSubtree.IsEmptyElement && seoRow != null)
                            {
                                if (readSubtree.LocalName.Equals("LanguageCode"))
                                    seoRow.LanguageCode = readSubtree.ReadString();

                                if (readSubtree.LocalName.Equals("Uri"))
                                    seoRow.Uri = readSubtree.ReadString();

                                if (readSubtree.LocalName.Equals("Title"))
                                {
                                    string title = readSubtree.ReadString();
                                    if (!String.IsNullOrEmpty(title))
                                        seoRow.Title = title;
                                }

                                if (readSubtree.LocalName.Equals("Keywords"))
                                {
                                    string keywords = readSubtree.ReadString();
                                    if (!String.IsNullOrEmpty(keywords))
                                        seoRow.Keywords = keywords;
                                }

                                if (readSubtree.LocalName.Equals("Description"))
                                {
                                    string description = readSubtree.ReadString();
                                    if (!String.IsNullOrEmpty(description))
                                        seoRow.Description = description;
                                }
                            }

                        }
                    }
                    #endregion
                }
            }

            #region Assign the Parent Nodes

            // Now add our Parent Nodes accordingly
            if (parentNodes.Count > 0)
            {
                IDictionaryEnumerator hashValues = parentNodes.GetEnumerator();
                while (hashValues.MoveNext())
                {
                    // Get the Node we need to set the Parent to
                    Catalog.Dto.CatalogNodeDto.CatalogNodeRow nodeSetParent = nodeAdmin.CurrentDto.CatalogNode.FindByCatalogNodeId((int)hashValues.Key);

                    if (nodeSetParent != null)
                    {
                        // Now get the Parent based on the code
                        foreach (Catalog.Dto.CatalogNodeDto.CatalogNodeRow parentItem in nodeAdmin.CurrentDto.CatalogNode.Rows)
                        {
                            if (parentItem.Code == hashValues.Value.ToString())
                            {
                                // Set the parent
                                nodeSetParent.ParentNodeId = parentItem.CatalogNodeId;
                                break;
                            }
                        }
                    }
                }

                // Update the database with the changes
                nodeAdmin.Save();

                parentNodes.Clear();
                parentNodes = null;
            }
            #endregion

            return nodeAdmin;
        }

        /// <summary>
        /// Reads the entries.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="baseFilePath">The base file path.</param>
		/// <param name="totalCount">Total entries count. Used for more precise calculating of import progress.</param>
        private void ReadEntries(int catalogId, XmlReader reader, string baseFilePath, int totalCount)
        {
			CultureInfo currentCultureInfo = GetImportExportCulture();

			int totalImportSteps = GetTotalImportSteps();

            // Variables for Parsing
            DateTime parsedDateTime;
            bool parsedBool;
			decimal parsedDecimal;
			int parsedInt;

            bool hasError = false;

            // Get the Subtree
            XmlReader readSubtree = reader.ReadSubtree();

            // Row identifiers
            CatalogEntryDto catalogEntryDto = new CatalogEntryDto();
            CatalogEntryDto.CatalogEntryRow entryRow = null;
            CatalogEntryDto.CatalogItemSeoRow seoRow = null;
            CatalogEntryDto.VariationRow variationRow = null;
			CatalogEntryDto.InventoryRow inventoryRow = null;
			CatalogEntryDto.SalePriceRow salePriceRow = null;

            // this will contain list of meta information for multiple entries
            Dictionary<string, Dictionary<string, MetaObject>> metaObjectsList = new Dictionary<string, Dictionary<string, MetaObject>>();

            Catalog.Data.CatalogEntryAdmin entryAdmin = new Mediachase.Commerce.Catalog.Data.CatalogEntryAdmin();

            int entryCount = 0;
			bool salePriceRowsDeleted = false;
			double currentPercent = 0;

            while (readSubtree.Read() && !readSubtree.EOF)
            {
                // Check if we have reached the end of Node Element
                if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Entry") && !hasError)
                {
					entryCount++;

					if (entryCount % 100 == 0)
					{
                        // Save records
                        SaveEntryDto(ref entryAdmin, metaObjectsList);

						if (totalCount > 0)
							OnEvent(String.Format("Imported catalog entries: {0} of {1}.", entryCount, totalCount), currentPercent);
						else
							OnEvent(String.Format("Imported catalog entries: {0}.", entryCount), currentPercent);
					}

                    // Check if Detached, if not then this row was already saved
                    if (entryRow.RowState == DataRowState.Detached)
                    {
                        entryAdmin.CurrentDto.CatalogEntry.AddCatalogEntryRow(entryRow);
                        // Sasha: entryAdmin.Save(); // Save our node
                    }
                }

                // Check if we are beginning a Node Element, create a row if so
                if (readSubtree.IsStartElement("Entry"))
                {
                    //entryAdmin = new Mediachase.Commerce.Catalog.Data.CatalogEntryAdmin();
                    hasError = false;
					salePriceRowsDeleted = false; // reset salePriceRowsDeleted flag
                    //catalogEntryDto = new CatalogEntryDto();
                    entryRow = entryAdmin.CurrentDto.CatalogEntry.NewCatalogEntryRow();
                    entryRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                    entryRow.CatalogId = catalogId;
                    entryRow.Code = String.Empty;

					// calculate current progress percent as soon as we read start Entry node
					if (totalCount > 0)
						currentPercent = GetProgressPercent2((int)ImportSteps.StartImportEntries + (double)entryCount / totalCount, totalImportSteps);
					else
						currentPercent = GetProgressPercent((int)ImportSteps.StartImportEntries, totalImportSteps);
                }

                // Ignore Empty Elements
                if (!readSubtree.IsEmptyElement && entryRow != null && !hasError)
                {
                    // Name
                    if (readSubtree.LocalName.Equals("Name"))
                        entryRow.Name = readSubtree.ReadString();

                    // Start Date
                    if (readSubtree.LocalName.Equals("StartDate"))
                    {
                        DateTime.TryParse(readSubtree.ReadString(), out parsedDateTime);
                        if (parsedDateTime != DateTime.MinValue)
							entryRow.StartDate = parsedDateTime.ToUniversalTime();
                    }

                    // End Date
                    if (readSubtree.LocalName.Equals("EndDate"))
                    {
                        DateTime.TryParse(readSubtree.ReadString(), out parsedDateTime);
                        if (parsedDateTime != DateTime.MinValue)
							entryRow.EndDate = parsedDateTime.ToUniversalTime();
                    }

                    // isActive
                    if (readSubtree.LocalName.Equals("IsActive"))
                    {
                        Boolean.TryParse(readSubtree.ReadString(), out parsedBool);
                        entryRow.IsActive = parsedBool;
                    }

                    // Template
                    if (readSubtree.LocalName.Equals("DisplayTemplate"))
                        entryRow.TemplateName = readSubtree.ReadString();

                    // Code
                    if (readSubtree.LocalName.Equals("Code"))
                        entryRow.Code = readSubtree.ReadString();

                    // EntryType
                    if (readSubtree.LocalName.Equals("EntryType"))
                        entryRow.ClassTypeId = readSubtree.ReadString();

                    #region Process MetaData
                    // MetaData
                    if (readSubtree.LocalName.Equals("MetaData") && !hasError)
                    {
                        if (String.IsNullOrEmpty(entryRow.Code))
                        {
							OnEvent(String.Format("Entry '{0}' has empty code.", entryRow.Name), currentPercent, ImportExportMessageType.Warning);
                            hasError = true;
                            continue;
                        }
                        else
                        {
                            catalogEntryDto = FrameworkContext.Current.CatalogSystem.GetCatalogEntryDto(entryRow.Code, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryFull));
                            if (catalogEntryDto.CatalogEntry.Count > 0)
                            {
                                //throw new ImportExportException(String.Format("Entry with code '{0}' already exists", entryRow.Code));
								OnEvent(String.Format("Entry with code '{0}' already exists", entryRow.Code), currentPercent, ImportExportMessageType.Warning);
                                continue;
                            }
                        }

                        MetaClass catalogEntryMetaClass = null;

                        while (readSubtree.Read() && !readSubtree.EOF)
                        {
                            if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("MetaData"))
                            {
                                break; // We are at the end of MetaData so break out of loop
                            }

                            // Check if beginning of MetaClass
                            if (readSubtree.NodeType == XmlNodeType.Element && readSubtree.LocalName.Equals("MetaClass"))
                            {
                                while (readSubtree.Read() && !readSubtree.EOF)
                                {
                                    if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("MetaClass"))
                                    {
                                        // Check if Detached, if not then this row was already saved
                                        if (entryRow.RowState == DataRowState.Detached && catalogEntryMetaClass != null)
                                        {
                                            entryRow.MetaClassId = catalogEntryMetaClass.Id;

                                            if (catalogEntryDto.CatalogEntry.Count > 0)
                                            {
                                                entryAdmin.CurrentDto.CatalogEntry.ImportRow(catalogEntryDto.CatalogEntry[0]);
                                                entryRow = entryAdmin.CurrentDto.CatalogEntry[entryAdmin.CurrentDto.CatalogEntry.Count - 1];
                                                /* Sasha: changed it back to preserve speed
												entryRow.CatalogEntryId = catalogEntryDto.CatalogEntry[0].CatalogEntryId;
												entryAdmin.CurrentDto.CatalogEntry.Rows.Add(entryRow);
												entryRow.AcceptChanges();
												entryRow.SetModified();
												entryAdmin.Save(); // Save our node
                                                 * */
                                            }
                                            else
                                            {
                                                entryAdmin.CurrentDto.CatalogEntry.Rows.Add(entryRow);
                                                // Sasha: do not save the record just yet, we will try to optimize the import process
                                                // entryAdmin.Save(); // Save our node prior to adding constrained records
                                            }
                                        }

                                        break; // We are at the end of MetaClass so break out of loop
                                    }

                                    if (readSubtree.LocalName.Equals("Name"))
                                    {
                                        string metaClassName = readSubtree.ReadString();
                                        catalogEntryMetaClass = MetaHelper.LoadMetaClassCached(CatalogContext.MetaDataContext, metaClassName);
                                    }
                                }
                            }

                            // Check if beginning of MetaFields
                            if (readSubtree.IsStartElement("MetaFields"))
                            {
                                Dictionary<string, MetaObject> metaObjects = new Dictionary<string, MetaObject>();

                                ReadMetaFields(metaObjects, readSubtree, entryRow.CatalogEntryId, catalogEntryMetaClass, baseFilePath);
                                int rowIndex = entryAdmin.CurrentDto.CatalogEntry.Rows.IndexOf(entryRow);
                                metaObjectsList.Add(rowIndex.ToString(), metaObjects);

                                MetaObjectSerialized serialized = new MetaObjectSerialized();
                                foreach (string language in metaObjects.Keys)
                                {
                                    CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                                    CatalogContext.MetaDataContext.Language = language;

                                    MetaObject metaObject = metaObjects[language];
                                    //MetaObject metaObject = MetaObject.Load(CatalogContext.MetaDataContext, entryRow.CatalogEntryId, entryRow.MetaClassId);
                                    foreach(MetaField mf in metaObject.MetaClass.MetaFields)
                                    {
                                        if (mf.IsSystem)
                                            continue;

                                        // Now do some processing for non multi language values
                                        if (!mf.MultiLanguageValue)
                                        {
                                            object foundValue = null;
                                            // find not null values and copy it to the rest of the meta fields
                                            foreach (string ml in metaObjects.Keys)
                                            {
                                                if (metaObjects[ml][mf] != null)
                                                {
                                                    foundValue = metaObjects[ml][mf];
                                                    break;
                                                }
                                            }

                                            // Now distribute it, unless it is null
                                            if (foundValue != null)
                                            {
                                                foreach (string ml in metaObjects.Keys)
                                                {
                                                    if (metaObjects[ml][mf] == null)
                                                        metaObjects[ml][mf] = foundValue;
                                                }
                                            }
                                        }
                                        else // do normal logic
                                        {
                                            if (metaObjects[language][mf] != null)
                                                metaObject[mf] = metaObjects[language][mf];
                                        }
                                    }


                                    //metaObject.AcceptChanges(CatalogContext.MetaDataContext);

                                    // Add serialized data
                                    serialized.AddMetaObject(language, metaObject);

                                    CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                                }

                                // Save value to the serialized data field
                                entryRow.SerializedData = serialized.BinaryValue;
                            }
                        }
                    }
                    #endregion

                    //OnEvent("Start importing SEO items for catalog entry");
                    #region Process Seo Items

                    // SEO info
                    if (readSubtree.LocalName.Equals("SeoInfo") && !hasError)
                    {
                        while (readSubtree.Read() && !readSubtree.EOF)
                        {
                            if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("SeoInfo"))
                                break; // We are at the end of SeoInfo so break out of loop

                            // Check if beginning of Seo
                            if (readSubtree.IsStartElement("Seo"))
                            {
                                seoRow = entryAdmin.CurrentDto.CatalogItemSeo.NewCatalogItemSeoRow();
                                seoRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
                                seoRow.CatalogEntryId = entryRow.CatalogEntryId;
                                seoRow.Title = String.Empty;
                                seoRow.Keywords = String.Empty;
                                seoRow.Description = String.Empty;
                            }

                            // Check if at the end of Seo
                            if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Seo"))
                            {
                                DataRow[] rows = catalogEntryDto.CatalogItemSeo.Select(String.Format("Uri = '{0}' AND LanguageCode ='{1}'", seoRow.Uri, seoRow.LanguageCode));
                                if (rows.Length == 0)
                                {
                                    entryAdmin.CurrentDto.CatalogItemSeo.Rows.Add(seoRow);
                                    //entryAdmin.Save();
                                }
                                else
                                {
                                    //update Seo info
                                    entryAdmin.CurrentDto.CatalogItemSeo.ImportRow(rows[0]);

                                    CatalogEntryDto.CatalogItemSeoRow itemSeoRow = entryAdmin.CurrentDto.CatalogItemSeo[entryAdmin.CurrentDto.CatalogItemSeo.Count - 1];
                                    itemSeoRow.Description = seoRow.Description;
                                    itemSeoRow.Keywords = seoRow.Keywords;
                                    itemSeoRow.Title = seoRow.Title;

                                    /*
                                    if(entryAdmin.CurrentDto.HasChanges())
                                        entryAdmin.Save();
                                     * */
                                }
                            }

                            if (!readSubtree.IsEmptyElement && seoRow != null)
                            {
                                if (readSubtree.LocalName.Equals("LanguageCode"))
                                    seoRow.LanguageCode = readSubtree.ReadString();

                                if (readSubtree.LocalName.Equals("Uri"))
                                    seoRow.Uri = readSubtree.ReadString();

                                if (readSubtree.LocalName.Equals("Title"))
                                {
                                    string title = readSubtree.ReadString();
                                    if(!String.IsNullOrEmpty(title))
                                        seoRow.Title = title;
                                }

                                if (readSubtree.LocalName.Equals("Keywords"))
                                {
                                    string keywords = readSubtree.ReadString();
                                    if (!String.IsNullOrEmpty(keywords))
                                        seoRow.Keywords = keywords;
                                }

                                if (readSubtree.LocalName.Equals("Description"))
                                {
                                    string description = readSubtree.ReadString();
                                    if(!String.IsNullOrEmpty(description))
                                        seoRow.Description = description;
                                }
                            }

                        }
                    }
                    #endregion
                    //OnEvent("Finished importing SEO items for catalog entry");
					
                    #region Process Variation Items

                    // Variation info
                    if (readSubtree.LocalName.Equals("VariationInfo") && !hasError)
                    {
                        while (readSubtree.Read() && !readSubtree.EOF)
                        {
                            if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("VariationInfo"))
                                break; // We are at the end of VariationInfo so break out of loop

                            // Check if beginning of Variation
                            if (readSubtree.IsStartElement("Variation"))
                            {
								if (catalogEntryDto.Variation.Count > 0)
								{
									entryAdmin.CurrentDto.Variation.ImportRow(catalogEntryDto.Variation[0]);
									variationRow = entryAdmin.CurrentDto.Variation[entryAdmin.CurrentDto.Variation.Count - 1];
								}
								else
								{
									variationRow = entryAdmin.CurrentDto.Variation.NewVariationRow();

									// set initial values for the fields that don't allow null values
									variationRow.TaxCategoryId = 0;
									variationRow.WarehouseId = 0;
									variationRow.TrackInventory = false;
									variationRow.Weight = 0;
									variationRow.PackageId = 0;
								}

                                variationRow.CatalogEntryId = entryRow.CatalogEntryId;
                            }

                            // Check if at the end of Variation
                            if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Variation"))
                            {
								if (variationRow.RowState == DataRowState.Detached)
									entryAdmin.CurrentDto.Variation.Rows.Add(variationRow);

                                /*
                                if (entryAdmin.CurrentDto.HasChanges())
                                    entryAdmin.Save();
                                 * */
                            }

							// process Variation properties
                            if (!readSubtree.IsEmptyElement && variationRow != null)
                            {
                                if (readSubtree.LocalName.Equals("ListPrice"))
									variationRow.ListPrice = Decimal.Parse(readSubtree.ReadString(), currentCultureInfo);
                                if (readSubtree.LocalName.Equals("MaxQuantity"))
									variationRow.MaxQuantity = Decimal.Parse(readSubtree.ReadString(), currentCultureInfo);
                                if (readSubtree.LocalName.Equals("MinQuantity"))
									variationRow.MinQuantity = Decimal.Parse(readSubtree.ReadString(), currentCultureInfo);

                                if (readSubtree.LocalName.Equals("MerchantName"))
                                {
                                    string merchantName = readSubtree.ReadString();
									if (!String.IsNullOrEmpty(merchantName))
									{
										Guid merchantId = CatalogEntryManager.GetMerchantIdByName(merchantName);
										if (merchantId != Guid.Empty)
											variationRow.MerchantId = merchantId;
									}
                                }

								if (readSubtree.LocalName.Equals("PackageName"))
								{
									string packageName = readSubtree.ReadString();
									if (!String.IsNullOrEmpty(packageName))
									{
										int packageId = ShippingManager.GetShippingPackageIdByName(packageName);
										if (packageId > 0)
											variationRow.PackageId = packageId;
									}
								}

								if (readSubtree.LocalName.Equals("TaxCategoryName"))
								{
									string tcName = readSubtree.ReadString();
									if (!String.IsNullOrEmpty(tcName))
									{
										int tcId = CatalogTaxManager.GetTaxCategoryIdByName(tcName);
										if (tcId > 0)
											variationRow.TaxCategoryId = tcId;
									}
								}

                                if (readSubtree.LocalName.Equals("TrackInventory"))
                                    variationRow.TrackInventory = Boolean.Parse(readSubtree.ReadString());

								if (readSubtree.LocalName.Equals("WarehouseName"))
								{
									string whName = readSubtree.ReadString();
									if (!String.IsNullOrEmpty(whName))
									{
										int whId = WarehouseManager.GetWarehouseIdByName(whName);
										if (whId > 0)
											variationRow.WarehouseId = whId;
									}
								}

                                if (readSubtree.LocalName.Equals("Weight"))
									variationRow.Weight = Double.Parse(readSubtree.ReadString(), currentCultureInfo);
                            }
                        }
                    }
                    #endregion

					#region Process Inventory Items

					// Inventory
					if (readSubtree.LocalName.Equals("Inventory") && !hasError)
					{
						// initialize inventory row
						if (catalogEntryDto.Inventory.Count > 0)
						{
							entryAdmin.CurrentDto.Inventory.ImportRow(catalogEntryDto.Inventory[0]);
							inventoryRow = entryAdmin.CurrentDto.Inventory[entryAdmin.CurrentDto.Inventory.Count - 1];
						}
						else
						{
							inventoryRow = entryAdmin.CurrentDto.Inventory.NewInventoryRow();

							// set initial values for the non-nullable fields
                            inventoryRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
							inventoryRow.InStockQuantity = 0;
							inventoryRow.ReservedQuantity = 0;
							inventoryRow.ReorderMinQuantity = 0;
							inventoryRow.PreorderQuantity = 0;
							inventoryRow.BackorderQuantity = 0;
							inventoryRow.AllowBackorder = false;
							inventoryRow.AllowPreorder = false;
							inventoryRow.InventoryStatus = 0;
							inventoryRow.PreorderAvailabilityDate = DateTime.MinValue;
							inventoryRow.BackorderAvailabilityDate = DateTime.MinValue;
						}

						inventoryRow.SkuId = entryRow.Code;

						// process inventory properies
						while (readSubtree.Read() && !readSubtree.EOF)
						{
							if (!readSubtree.IsEmptyElement && inventoryRow != null)
							{
								if (readSubtree.LocalName.Equals("InStockQuantity"))
								{
									Decimal.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDecimal);
									inventoryRow.InStockQuantity = parsedDecimal;
								}

								if (readSubtree.LocalName.Equals("ReservedQuantity"))
								{
									Decimal.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDecimal);
									inventoryRow.ReservedQuantity = parsedDecimal;
								}

								if (readSubtree.LocalName.Equals("ReorderMinQuantity"))
								{
									Decimal.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDecimal);
									inventoryRow.ReorderMinQuantity = parsedDecimal;
								}

								if (readSubtree.LocalName.Equals("PreorderQuantity"))
								{
									Decimal.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDecimal);
									inventoryRow.PreorderQuantity = parsedDecimal;
								}

								if (readSubtree.LocalName.Equals("BackorderQuantity"))
								{
									Decimal.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDecimal);
									inventoryRow.BackorderQuantity = parsedDecimal;
								}

								if (readSubtree.LocalName.Equals("AllowBackorder"))
								{
									Boolean.TryParse(readSubtree.ReadString(), out parsedBool);
									inventoryRow.AllowBackorder = parsedBool;
								}

								if (readSubtree.LocalName.Equals("AllowPreorder"))
								{
									Boolean.TryParse(readSubtree.ReadString(), out parsedBool);
									inventoryRow.AllowPreorder = parsedBool;
								}

								if (readSubtree.LocalName.Equals("InventoryStatus"))
								{
									Int32.TryParse(readSubtree.ReadString(), NumberStyles.Integer, currentCultureInfo, out parsedInt);
									inventoryRow.InventoryStatus = parsedInt;
								}

								if (readSubtree.LocalName.Equals("PreorderAvailabilityDate"))
								{
									DateTime.TryParse(readSubtree.ReadString(), out parsedDateTime);
									inventoryRow.PreorderAvailabilityDate = parsedDateTime.ToUniversalTime();
								}

								if (readSubtree.LocalName.Equals("BackorderAvailabilityDate"))
								{
									DateTime.TryParse(readSubtree.ReadString(), out parsedDateTime);
									inventoryRow.BackorderAvailabilityDate = parsedDateTime.ToUniversalTime();
								}
							}

							// Check if at the end of Inventory
							if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Inventory"))
							{
								if (inventoryRow.RowState == DataRowState.Detached)
									entryAdmin.CurrentDto.Inventory.Rows.Add(inventoryRow);

                                /*
								if (entryAdmin.CurrentDto.HasChanges())
									entryAdmin.Save();
                                 * */

								break;
							}
						}
					}
					#endregion

					#region Process SalePrice Items

					if (!salePriceRowsDeleted && catalogEntryDto.SalePrice.Count > 0 &&
						entryAdmin.CurrentDto.CatalogEntry.Count > 0)
					{
						// delete old SalePrice rows if salePriceRowsDeleted is false, so that rows are deleted only once
						foreach (CatalogEntryDto.SalePriceRow spRow in catalogEntryDto.SalePrice.Rows)
						{
							entryAdmin.CurrentDto.SalePrice.ImportRow(spRow);
							entryAdmin.CurrentDto.SalePrice.Rows[entryAdmin.CurrentDto.SalePrice.Count - 1].Delete();
						}

                        /*
						if (entryAdmin.CurrentDto.HasChanges())
							entryAdmin.Save();
                         * */

						salePriceRowsDeleted = true;
					}

					// SalePrices info
					if (readSubtree.LocalName.Equals("SalePrices") && !hasError)
					{
						// read SalePrice rows
						while (readSubtree.Read() && !readSubtree.EOF)
						{
							if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("SalePrices"))
								break; // We are at the end of SalePrices so break out of loop

							// Check if beginning of SalePrice
							if (readSubtree.IsStartElement("SalePrice"))
							{
								// add new row
								salePriceRow = entryAdmin.CurrentDto.SalePrice.NewSalePriceRow();

								// set initial values for the fields that don't allow null values
								salePriceRow.SaleType = 0;
								salePriceRow.StartDate = DateTime.MinValue;
								salePriceRow.Currency = "USD";
								salePriceRow.MinQuantity = 0;
                                salePriceRow.SaleCode = String.Empty;
								salePriceRow.UnitPrice = 0;

								salePriceRow.ItemCode = entryRow.Code;
							}

							// Check if at the end of SalePrice
							if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("SalePrice"))
							{
								if (salePriceRow.RowState == DataRowState.Detached)
									entryAdmin.CurrentDto.SalePrice.Rows.Add(salePriceRow);

                                /*
								if (entryAdmin.CurrentDto.HasChanges())
									entryAdmin.Save();
                                 * */
							}

							// process SalePrice properties
							if (!readSubtree.IsEmptyElement && salePriceRow != null)
							{
								if (readSubtree.LocalName.Equals("SaleType"))
								{
									Int32.TryParse(readSubtree.ReadString(), out parsedInt);
									salePriceRow.SaleType = parsedInt;
								}

								if (readSubtree.LocalName.Equals("SaleCode"))
								{
									string saleCode = readSubtree.ReadString();
									if (!String.IsNullOrEmpty(saleCode))
										salePriceRow.SaleCode = saleCode;
                                    else
                                        salePriceRow.SaleCode = String.Empty;
								}

								if (readSubtree.LocalName.Equals("StartDate"))
								{
									DateTime.TryParse(readSubtree.ReadString(), out parsedDateTime);
									salePriceRow.StartDate = parsedDateTime.ToUniversalTime();
								}

								if (readSubtree.LocalName.Equals("Currency"))
								{
									string currency = readSubtree.ReadString();
									if (!String.IsNullOrEmpty(currency))
										salePriceRow.Currency = currency;
                                    else
                                        salePriceRow.Currency = String.Empty;
								}

								if (readSubtree.LocalName.Equals("MinQuantity"))
								{
									Decimal.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDecimal);
									salePriceRow.MinQuantity = parsedDecimal;
								}

								if (readSubtree.LocalName.Equals("UnitPrice"))
								{
									Decimal.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDecimal);
									salePriceRow.UnitPrice = parsedDecimal;
								}

								if (readSubtree.LocalName.Equals("EndDate"))
								{
									DateTime.TryParse(readSubtree.ReadString(), out parsedDateTime);
									salePriceRow.EndDate = parsedDateTime.ToUniversalTime();
								}
							}
						}
					}
					#endregion

                    //OnEvent("Finished importing catalog entry " + nodeCount.ToString());
                }
            }

            SaveEntryDto(ref entryAdmin, metaObjectsList);
        }

        /// <summary>
        /// Saves the cached entry dto together with meta data.
        /// </summary>
        /// <param name="admin">The admin.</param>
        /// <param name="metaObjectsList">The meta objects list.</param>
        void SaveEntryDto(ref Mediachase.Commerce.Catalog.Data.CatalogEntryAdmin admin, Dictionary<string, Dictionary<string, MetaObject>> metaObjectsList)
        {
            if (admin.CurrentDto.HasChanges())
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    admin.CurrentDto.CatalogEntry.ExtendedProperties["MetaObjects"] = metaObjectsList;
                    DataRowChangeEventHandler handler = new DataRowChangeEventHandler(CatalogEntry_RowChanged);
                    admin.CurrentDto.CatalogEntry.RowChanged += handler;
                    admin.Save();

                    // now go through and save all meta objects
                    foreach (CatalogEntryDto.CatalogEntryRow row in admin.CurrentDto.CatalogEntry)
                    {
                        if (!metaObjectsList.ContainsKey(row.CatalogEntryId.ToString()))
                            continue;

                        Dictionary<string, MetaObject> metaObjects = metaObjectsList[row.CatalogEntryId.ToString()];

                        foreach (string language in metaObjects.Keys)
                        {
                            CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                            CatalogContext.MetaDataContext.Language = language;

                            if (!metaObjects.ContainsKey(language))
                                continue;

                            MetaObject metaObject = metaObjects[language];

                            if (metaObject == null)
                                continue;

                            metaObject.AcceptChanges(CatalogContext.MetaDataContext);
                            CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                        }
                    }

                    scope.Complete();
                }
            }

            // reset the admin
            admin = new Mediachase.Commerce.Catalog.Data.CatalogEntryAdmin();
        }

        /// <summary>
        /// Handles the RowChanged event of the CatalogEntry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Data.DataRowChangeEventArgs"/> instance containing the event data.</param>
        void CatalogEntry_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            CatalogEntryDto.CatalogEntryDataTable table = (CatalogEntryDto.CatalogEntryDataTable)sender;
            CatalogEntryDto.CatalogEntryRow row = (CatalogEntryDto.CatalogEntryRow)e.Row;
            //table.Rows.IndexOf
            if (row.CatalogEntryId > 0)
            {
                Dictionary<string, Dictionary<string, MetaObject>> metaObjectList = (Dictionary<string, Dictionary<string, MetaObject>>)table.ExtendedProperties["MetaObjects"];

                int rowIndex = table.Rows.IndexOf(row);

                if (!metaObjectList.ContainsKey(rowIndex.ToString()))
                    return;

                Dictionary<string, MetaObject> metaObjects = metaObjectList[rowIndex.ToString()];
                Dictionary<string, MetaObject> metaObjectsUpdated = new Dictionary<string, MetaObject>();
                foreach (string language in metaObjects.Keys)
                {
                    CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                    CatalogContext.MetaDataContext.Language = language;

                    if (!metaObjects.ContainsKey(language))
                        continue;

                    MetaObject tempMetaObject = metaObjects[language];

                    if (tempMetaObject == null)
                        continue;

                    // Create new object specifying correct catalog entry id now
                    MetaObject metaObject = MetaObject.NewObject(row.CatalogEntryId, tempMetaObject.MetaClass, tempMetaObject.CreatorId);

                    //MetaObject metaObject = MetaObject.Load(CatalogContext.MetaDataContext, entryRow.CatalogEntryId, entryRow.MetaClassId);
                    foreach (MetaField mf in metaObject.MetaClass.MetaFields)
                    {
                        if (tempMetaObject[mf] != null)
                            metaObject[mf] = tempMetaObject[mf];
                    }

                    // Add updated meta object
                    metaObjectsUpdated.Add(language, metaObject);

                    // we can't save here, because we in the middle of the other transaction
                    // metaObject.AcceptChanges(CatalogContext.MetaDataContext);
                    CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                }

                // remove old object
                metaObjectList.Remove(rowIndex.ToString());

                // insert updated
                metaObjectList.Add(row.CatalogEntryId.ToString(), metaObjectsUpdated);
            }
        }


        /// <summary>
        /// Reads the relations.
        /// </summary>
        /// <param name="CatalogId">The catalog id.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="nodeAdmin">The node admin.</param>
        private void ReadRelations(int CatalogId, XmlReader reader, Catalog.Data.CatalogNodeAdmin nodeAdmin)
        {
			CultureInfo currentCultureInfo = GetImportExportCulture();

			int parsedInt;
            decimal parsedDecimal;

            int counter = 0;
            Catalog.Dto.CatalogRelationDto.CatalogNodeRelationRow nodeRelationRow = null;
            Catalog.Dto.CatalogRelationDto.NodeEntryRelationRow nodeEntryRelationRow = null;
            Catalog.Dto.CatalogRelationDto.CatalogEntryRelationRow catalogEntryRelationRow = null;

            CatalogRelationDto catalogRelationDto = FrameworkContext.Current.CatalogSystem.GetCatalogRelationDto(CatalogId, 0, 0, String.Empty, new CatalogRelationResponseGroup());

            Catalog.Data.CatalogRelationAdmin relationAdmin = new Mediachase.Commerce.Catalog.Data.CatalogRelationAdmin();
            DataView catalogNodeDataView = nodeAdmin.CurrentDto.CatalogNode.DefaultView;

            // Get the Subtree
            XmlReader readSubtree = reader.ReadSubtree();

            while (readSubtree.Read() && !readSubtree.EOF)
            {
                #region CatalogNodeRelation

                // Check if beginning of NodeRelation
                if (readSubtree.IsStartElement("NodeRelation"))
                {
                    nodeRelationRow = relationAdmin.CurrentDto.CatalogNodeRelation.NewCatalogNodeRelationRow();
                    nodeRelationRow.CatalogId = CatalogId;
                    nodeRelationRow.ChildNodeId = -1;
                    nodeRelationRow.ParentNodeId = -1;

                    while (readSubtree.Read() && !readSubtree.EOF)
                    {
                        if (!readSubtree.IsEmptyElement && nodeRelationRow != null)
                        {
                            if (readSubtree.LocalName.Equals("ChildNodeCode"))
                            {
                                string childNodeCode = readSubtree.ReadString();

                                //get child node id
                                catalogNodeDataView.RowFilter = String.Format("Code = '{0}'", childNodeCode.Replace("'", "''"));
                                if (catalogNodeDataView.Count > 0)
                                {
                                    nodeRelationRow.ChildNodeId = (int)catalogNodeDataView[0]["CatalogNodeId"];
                                }
                            }

                            if (readSubtree.LocalName.Equals("ParentNodeCode"))
                            {
                                string parentNodeCode = readSubtree.ReadString();

                                //get parent node id
                                catalogNodeDataView.RowFilter = String.Format("Code = '{0}'", parentNodeCode.Replace("'", "''"));
                                if (catalogNodeDataView.Count > 0)
                                {
                                    nodeRelationRow.ParentNodeId = (int)catalogNodeDataView[0]["CatalogNodeId"];
                                }
                            }

                            if (readSubtree.LocalName.Equals("SortOrder"))
                            {
                                Int32.TryParse(readSubtree.ReadString(), NumberStyles.Integer, currentCultureInfo, out parsedInt);
                                nodeRelationRow.SortOrder = parsedInt;
                            }
                        }

                        // Check if at the end of NodeRelation
                        if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("NodeRelation"))
                        {
                            if (nodeRelationRow.ChildNodeId == -1 || nodeRelationRow.ParentNodeId == -1)
                                break;

                            DataRow[] rows = catalogRelationDto.CatalogNodeRelation.Select(String.Format("ChildNodeId = {0} AND ParentNodeId = {1}", nodeRelationRow.ChildNodeId, nodeRelationRow.ParentNodeId));
                            if (rows.Length == 0)
                            {
                                relationAdmin.CurrentDto.CatalogNodeRelation.Rows.Add(nodeRelationRow);
                                counter++;
                            }

                            if (counter >= 20)
                            {
                                relationAdmin.Save();
                                relationAdmin = new Mediachase.Commerce.Catalog.Data.CatalogRelationAdmin();
                                counter = 0;
                            }
                            break;
                        }
                    }
                }
                #endregion CatalogNodeRelation

                #region NodeEntryRelation

                // Check if beginning of NodeEntryRelation
                if (readSubtree.IsStartElement("NodeEntryRelation"))
                {
                    nodeEntryRelationRow = relationAdmin.CurrentDto.NodeEntryRelation.NewNodeEntryRelationRow();
                    nodeEntryRelationRow.CatalogId = CatalogId;
                    nodeEntryRelationRow.CatalogEntryId = -1;
                    nodeEntryRelationRow.CatalogNodeId = -1;

                    while (readSubtree.Read() && !readSubtree.EOF)
                    {
                        if (!readSubtree.IsEmptyElement && nodeEntryRelationRow != null)
                        {
                            if (readSubtree.LocalName.Equals("EntryCode"))
                            {
                                string catalogEntryCode = readSubtree.ReadString();

                                //get parent entry id
                                Catalog.Dto.CatalogEntryDto catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(catalogEntryCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                                if (catalogEntryDto.CatalogEntry.Count > 0)
                                {
                                    nodeEntryRelationRow.CatalogEntryId = catalogEntryDto.CatalogEntry[0].CatalogEntryId;
                                }
                            }

                            if (readSubtree.LocalName.Equals("NodeCode"))
                            {
                                string parentNodeCode = readSubtree.ReadString();

                                //get node id
                                catalogNodeDataView.RowFilter = String.Format("Code = '{0}'", parentNodeCode.Replace("'", "''"));
                                if (catalogNodeDataView.Count > 0)
                                {
                                    nodeEntryRelationRow.CatalogNodeId = (int)catalogNodeDataView[0]["CatalogNodeId"];
                                }
                            }

                            if (readSubtree.LocalName.Equals("SortOrder"))
                            {
                                Int32.TryParse(readSubtree.ReadString(), NumberStyles.Integer, currentCultureInfo, out parsedInt);
                                nodeEntryRelationRow.SortOrder = parsedInt;
                            }
                        }

                        // Check if at the end of NodeRelation
                        if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("NodeEntryRelation"))
                        {
                            if (nodeEntryRelationRow.CatalogEntryId == -1 || nodeEntryRelationRow.CatalogNodeId == -1)
                                break;

                            DataRow[] rows = catalogRelationDto.NodeEntryRelation.Select(String.Format("CatalogEntryId = {0} AND CatalogNodeId = {1}", nodeEntryRelationRow.CatalogEntryId, nodeEntryRelationRow.CatalogNodeId));
                            if (rows.Length == 0)
                            {
                                relationAdmin.CurrentDto.NodeEntryRelation.Rows.Add(nodeEntryRelationRow);
                                counter++;
                            }

                            if (counter >= 100)
                            {
                                relationAdmin.Save();
                                relationAdmin = new Mediachase.Commerce.Catalog.Data.CatalogRelationAdmin();
                                counter = 0;
                            }
                            break;
                        }
                    }
                }
                #endregion NodeEntryRelation

                #region CatalogEntryRelation

                // Check if beginning of EntryRelation
                if (readSubtree.IsStartElement("EntryRelation"))
                {
                    catalogEntryRelationRow = relationAdmin.CurrentDto.CatalogEntryRelation.NewCatalogEntryRelationRow();

                    //Column 'ParentEntryId, ChildEntryId, RelationTypeId' is constrained to be unique.
                    catalogEntryRelationRow.ParentEntryId = -1;
                    catalogEntryRelationRow.ChildEntryId = -1;
                    catalogEntryRelationRow.RelationTypeId = String.Empty;

                    while (readSubtree.Read() && !readSubtree.EOF)
                    {
                        if (!readSubtree.IsEmptyElement && catalogEntryRelationRow != null)
                        {
                            if (readSubtree.LocalName.Equals("ChildEntryCode"))
                            {
                                string childEntryCode = readSubtree.ReadString();

                                //get child entry id
                                Catalog.Dto.CatalogEntryDto catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(childEntryCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                                if (catalogEntryDto.CatalogEntry.Count > 0)
                                {
                                    catalogEntryRelationRow.ChildEntryId = catalogEntryDto.CatalogEntry[0].CatalogEntryId;
                                }
                            }

                            if (readSubtree.LocalName.Equals("ParentEntryCode"))
                            {
                                string parentEntryCode = readSubtree.ReadString();

                                //get parent entry id
                                Catalog.Dto.CatalogEntryDto catalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(parentEntryCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                                if (catalogEntryDto.CatalogEntry.Count > 0)
                                {
                                    catalogEntryRelationRow.ParentEntryId = catalogEntryDto.CatalogEntry[0].CatalogEntryId;
                                }
                            }

                            if (readSubtree.LocalName.Equals("RelationType"))
                                catalogEntryRelationRow.RelationTypeId = readSubtree.ReadString();

                            if (readSubtree.LocalName.Equals("GroupName"))
                                catalogEntryRelationRow.GroupName = readSubtree.ReadString();

                            if (readSubtree.LocalName.Equals("Quantity"))
                            {
                                Decimal.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDecimal);
                                catalogEntryRelationRow.Quantity = parsedDecimal;
                            }

                            if (readSubtree.LocalName.Equals("SortOrder"))
                            {
                                Int32.TryParse(readSubtree.ReadString(), NumberStyles.Integer, currentCultureInfo, out parsedInt);
                                catalogEntryRelationRow.SortOrder = parsedInt;
                            }
                        }

                        // Check if at the end of NodeRelation
                        if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("EntryRelation"))
                        {
                            if (catalogEntryRelationRow.ParentEntryId == -1 || catalogEntryRelationRow.ChildEntryId == -1)
                                break;

                            DataRow[] rows = catalogRelationDto.CatalogEntryRelation.Select(String.Format("ParentEntryId = {0} AND ChildEntryId = {1}", catalogEntryRelationRow.ParentEntryId, catalogEntryRelationRow.ChildEntryId));
                            if (rows.Length == 0)
                            {
                                relationAdmin.CurrentDto.CatalogEntryRelation.Rows.Add(catalogEntryRelationRow);
                                counter++;
                            }

                            if (counter >= 20)
                            {
                                relationAdmin.Save();
                                relationAdmin = new Mediachase.Commerce.Catalog.Data.CatalogRelationAdmin();
                                counter = 0;
                            }

                            break;
                        }
                    }
                }
                #endregion CatalogEntryRelation
            }

            if (relationAdmin.CurrentDto.HasChanges())
                relationAdmin.Save();
        }

        /// <summary>
        /// Reads the associations.
        /// </summary>
        /// <param name="catalogId">The catalog id.</param>
        /// <param name="reader">The reader.</param>
        private void ReadAssociations(int catalogId, XmlReader reader)
        {
			CultureInfo currentCultureInfo = GetImportExportCulture();

			int totalImportSteps = GetTotalImportSteps();

            int parsedInt;

            int counter = 0;
            Catalog.Dto.CatalogAssociationDto.CatalogAssociationRow catalogAssociationRow = null;
            Catalog.Dto.CatalogAssociationDto.CatalogEntryAssociationRow catalogEntryAssociationRow = null;

            Catalog.Data.CatalogAssociationAdmin associationAdmin = new Mediachase.Commerce.Catalog.Data.CatalogAssociationAdmin();
            CatalogAssociationDto catalogAssociationDto = CatalogAssociationManager.GetCatalogAssociationDtoByCatalogId(catalogId);

            // Get the Subtree
            XmlReader readSubtree = reader.ReadSubtree();

            while (readSubtree.Read() && !readSubtree.EOF)
            {
                // Check if beginning of CatalogAssociation
                if (readSubtree.IsStartElement("CatalogAssociation"))
                {
                    catalogAssociationRow = associationAdmin.CurrentDto.CatalogAssociation.NewCatalogAssociationRow();
                    catalogAssociationRow.CatalogEntryId = -1;
                    catalogAssociationRow.AssociationName = String.Empty;

                    while (readSubtree.Read() && !readSubtree.EOF)
                    {
                        // Check if at the end of CatalogAssociation
                        if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("CatalogAssociation"))
                        {
                            if (counter >= 100)
                            {
                                associationAdmin.Save();
                                associationAdmin = new Mediachase.Commerce.Catalog.Data.CatalogAssociationAdmin();
                                counter = 0;
                            }
                            break;
                        }

                        if (!readSubtree.IsEmptyElement)
                        {
                            if (readSubtree.LocalName.Equals("Name"))
                                catalogAssociationRow.AssociationName = readSubtree.ReadString();

                            if (readSubtree.LocalName.Equals("Description"))
                            {
                                string associationDescription = readSubtree.ReadString();
                                if (!String.IsNullOrEmpty(associationDescription))
                                    catalogAssociationRow.AssociationDescription = associationDescription;
                            }

                            if (readSubtree.LocalName.Equals("SortOrder"))
                            {
                                Int32.TryParse(readSubtree.ReadString(), NumberStyles.Integer, currentCultureInfo, out parsedInt);
                                catalogAssociationRow.SortOrder = parsedInt;
                            }

                            if (readSubtree.LocalName.Equals("EntryCode"))
                            {
                                string catalogEntryCode = readSubtree.ReadString();
                                
                                //get parent entry id
                                CatalogEntryDto tmpCatalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(catalogEntryCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                                if (tmpCatalogEntryDto.CatalogEntry.Count > 0)
                                {
                                    catalogAssociationRow.CatalogEntryId = tmpCatalogEntryDto.CatalogEntry[0].CatalogEntryId;
                                }

                                DataRow[] rows = catalogAssociationDto.CatalogAssociation.Select(String.Format("CatalogEntryId = {0} AND AssociationName = '{1}'", catalogAssociationRow.CatalogEntryId, catalogAssociationRow.AssociationName.Replace("'", "''")));
                                if (rows.Length == 0)
                                {
                                    associationAdmin.CurrentDto.CatalogAssociation.Rows.Add(catalogAssociationRow);
                                    counter++;
                                }
                                else
                                {
                                    associationAdmin.CurrentDto.CatalogAssociation.ImportRow(rows[0]);
                                    catalogAssociationRow = associationAdmin.CurrentDto.CatalogAssociation[associationAdmin.CurrentDto.CatalogAssociation.Count - 1];
                                }
                            }
                        }

                        // Check if beginning of Association
                        if (readSubtree.IsStartElement("Association"))
                        {
                            catalogEntryAssociationRow = associationAdmin.CurrentDto.CatalogEntryAssociation.NewCatalogEntryAssociationRow();
                            catalogEntryAssociationRow.CatalogAssociationId = catalogAssociationRow.CatalogAssociationId;
                            catalogEntryAssociationRow.CatalogEntryId = -1;

                            while (readSubtree.Read() && !readSubtree.EOF)
                            {
                                if (!readSubtree.IsEmptyElement)
                                {
                                    if (readSubtree.LocalName.Equals("EntryCode"))
                                    {
                                        string catalogEntryCode = readSubtree.ReadString();

                                        //get parent entry id
                                        CatalogEntryDto tmpCatalogEntryDto = CatalogContext.Current.GetCatalogEntryDto(catalogEntryCode, new CatalogEntryResponseGroup(CatalogEntryResponseGroup.ResponseGroup.CatalogEntryInfo));
                                        if (tmpCatalogEntryDto.CatalogEntry.Count > 0)
                                        {
                                            catalogEntryAssociationRow.CatalogEntryId = tmpCatalogEntryDto.CatalogEntry[0].CatalogEntryId;
                                        }
                                    }

                                    if (readSubtree.LocalName.Equals("Type"))
                                        catalogEntryAssociationRow.AssociationTypeId = readSubtree.ReadString();

                                    if (readSubtree.LocalName.Equals("SortOrder"))
                                    {
                                        Int32.TryParse(readSubtree.ReadString(), NumberStyles.Integer, currentCultureInfo, out parsedInt);
                                        catalogEntryAssociationRow.SortOrder = parsedInt;
                                    }
                                }

                                // Check if at the end of Association
                                if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Association"))
                                {
									DataRow[] rows = catalogAssociationDto.CatalogEntryAssociation.Select(String.Format("CatalogEntryId = {0} AND CatalogAssociationId = '{1}'", catalogEntryAssociationRow.CatalogEntryId, catalogAssociationRow.CatalogAssociationId));
                                    if (rows.Length == 0)
                                        associationAdmin.CurrentDto.CatalogEntryAssociation.Rows.Add(catalogEntryAssociationRow);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (associationAdmin.CurrentDto.HasChanges())
                associationAdmin.Save();
        }

        /// <summary>
        /// Reads the meta fields and creates a list of meta objects.
        /// </summary>
        /// <param name="metaObjects">The meta objects.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="objectId">The object id.</param>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="baseFilePath">The base file path.</param>
        private void ReadMetaFields(Dictionary<string, MetaObject> metaObjects, XmlReader reader, int objectId, MetaClass metaClass, string baseFilePath)
        {
            string metaFieldName = String.Empty;
            string metaFieldType = String.Empty;

            XmlReader readSubtree = reader.ReadSubtree();

            while (readSubtree.Read() && !readSubtree.EOF)
            {
                if (readSubtree.NodeType == XmlNodeType.Element && readSubtree.LocalName.Equals("MetaField"))
                {
                    metaFieldName = String.Empty;
                    metaFieldType = String.Empty;
                }

                if (readSubtree.LocalName.Equals("Name"))
                {
                    metaFieldName = readSubtree.ReadString();
                }

                if (readSubtree.LocalName.Equals("Type"))
                {
                    metaFieldType = readSubtree.ReadString();
                }

                if (readSubtree.LocalName.Equals("Data") || readSubtree.LocalName.Equals("FileData"))
                {
                    string languageCode = readSubtree.GetAttribute("language");

                    if (languageCode == null)
                        continue;

                    CatalogContext.MetaDataContext.UseCurrentUICulture = false;
                    CatalogContext.MetaDataContext.Language = languageCode;

                    if (!metaObjects.ContainsKey(languageCode))
                    {
                        MetaObject metaObj = null;
                        // Sasha: for now assume we always have new meta object
                        //MetaObject metaObj = MetaObject.Load(CatalogContext.MetaDataContext, objectId, metaClass);
                        //if (metaObj == null)
                        //{
                        string userName = String.Empty;

                        if (Thread.CurrentPrincipal.Identity != null)
                            userName = Thread.CurrentPrincipal.Identity.Name;

                        metaObj = MetaObject.NewObject(objectId, metaClass, userName);
                        //    metaObj.AcceptChanges(CatalogContext.MetaDataContext);
                        //}
                        metaObjects[languageCode] = metaObj;
                    }

                    metaObjects[languageCode] = AssignMetaFieldValue(metaObjects[languageCode], readSubtree, metaFieldName, metaFieldType, languageCode, baseFilePath);

                    CatalogContext.MetaDataContext.UseCurrentUICulture = true;
                }
            }
        }

        /// <summary>
        /// Assigns the meta field value.
        /// </summary>
        /// <param name="metaObj">The meta obj.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="language">The language.</param>
        /// <param name="baseFilePath">The base file path.</param>
        /// <returns></returns>
        private MetaObject AssignMetaFieldValue(MetaObject metaObj, XmlReader reader, string fieldName, string fieldType, string language, string baseFilePath)
        {
			CultureInfo currentCultureInfo = GetImportExportCulture();

            if (reader.LocalName.Equals("Data") || reader.LocalName.Equals("FileData"))
            {
                MetaField field = null; // load only when we really need it
                try
                {
                    MetaFile metaFile = null;
                    MetaDictionaryItem item = null;

                    string sData = reader.GetAttribute("value");

                    MetaDataType mtype = (MetaDataType)Enum.Parse(typeof(MetaDataType), fieldType);

                    switch (mtype)
                    {
                        case MetaDataType.File:
                        case MetaDataType.ImageFile:
                            string contentType = String.Empty;
                            string diskFileName = String.Empty;
                            string fileName = String.Empty;

                            while (reader.Read() && !reader.EOF)
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (reader.LocalName.Equals("DiskFileName"))
                                        diskFileName = reader.ReadString();

                                    if (reader.LocalName.Equals("ContentType"))
                                        contentType = reader.ReadString();

                                    if (reader.LocalName.Equals("Name"))
                                        fileName = reader.ReadString();
                                }

                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (reader.LocalName.Equals("FileData"))
                                    {
                                        metaFile = new MetaFile();
                                        
                                        FileStream fs = File.Open(Path.Combine(baseFilePath, diskFileName), FileMode.Open, FileAccess.Read);
                                        byte[] buffer = new byte[fs.Length];
			                            fs.Read(buffer, 0, buffer.Length);
                                        fs.Close();

                                        metaFile.Buffer = buffer;
                                        if (!(metaFile.Buffer == null || metaFile.Buffer.Length == 0))
                                        {
                                            metaFile.ContentType = contentType;
                                            metaFile.Name = fileName;
                                            metaObj[fieldName] = metaFile;
                                        }

                                        break; // We are at the end of FileData so break out of loop
                                    }
                                }
                            }

                            break;
                        case MetaDataType.DateTime:
                            if (!String.IsNullOrEmpty(sData))
                                metaObj[fieldName] = DateTime.Parse(sData).ToUniversalTime();
                            break;
                        case MetaDataType.Money:
                            if (!String.IsNullOrEmpty(sData))
								metaObj[fieldName] = Decimal.Parse(sData, currentCultureInfo);
                            break;
                        case MetaDataType.Integer:
                            if (!String.IsNullOrEmpty(sData))
								metaObj[fieldName] = Int32.Parse(sData, currentCultureInfo);
                            break;
                        case MetaDataType.Boolean:
                            if (!String.IsNullOrEmpty(sData))
                                metaObj[fieldName] = Boolean.Parse(sData);
                            break;
                        case MetaDataType.Date:
                            if (!String.IsNullOrEmpty(sData))
                                metaObj[fieldName] = DateTime.Parse(sData);
                            break;
                        case MetaDataType.Email:
                        case MetaDataType.URL:
                        case MetaDataType.ShortString:
                        case MetaDataType.LongString:
                        case MetaDataType.LongHtmlString:
                            if (!String.IsNullOrEmpty(sData))
                                metaObj[fieldName] = sData;
                            break;
                        case MetaDataType.DictionarySingleValue:
                        case MetaDataType.EnumSingleValue:
                            item = null;
                            field = MetaField.Load(CatalogContext.MetaDataContext, fieldName);
                            foreach (MetaDictionaryItem i in field.Dictionary)
                            {
                                if (String.Compare(i.Value, sData, true) == 0)
                                {
                                    item = i;
                                    break;
                                }
                            }
                            if (item != null)
                                metaObj[fieldName] = item;
                            else
                            {
                                int id = metaObj.MetaClass.MetaFields[fieldName].Dictionary.Add(sData);
                                item = metaObj.MetaClass.MetaFields[fieldName].Dictionary[id] as MetaDictionaryItem;
                                metaObj[fieldName] = item;
                            }
                            break;
                        case MetaDataType.DictionaryMultiValue:
                        case MetaDataType.EnumMultiValue:
                            field = MetaField.Load(CatalogContext.MetaDataContext, fieldName);
							List<MetaDictionaryItem> dicFieldValues = new List<MetaDictionaryItem>();
                            while (reader.Read() && !reader.EOF)
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (reader.LocalName.Equals("Item"))
                                    {
                                        string val1 = reader.GetAttribute("value");

                                        item = null;
                                        foreach (MetaDictionaryItem i1 in field.Dictionary)
                                        {
                                            if (String.Compare(i1.Value, val1, true) == 0)
                                            {
                                                item = i1;
                                                break;
                                            }
                                        }
										if (item != null)
											dicFieldValues.Add(item);
										else
										{
											int newId = metaObj.MetaClass.MetaFields[fieldName].Dictionary.Add(val1);
											item = metaObj.MetaClass.MetaFields[fieldName].Dictionary[newId] as MetaDictionaryItem;
											dicFieldValues.Add(item);
										}
                                    }
                                }

                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (reader.LocalName.Equals("Data"))
                                        break;
                                }
                            }
							if (dicFieldValues.Count > 0)
								metaObj[fieldName] = dicFieldValues.ToArray();
                            break;
                        case MetaDataType.StringDictionary:
                            MetaStringDictionary dic = new MetaStringDictionary();
                            while (reader.Read() && !reader.EOF)
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    if (reader.LocalName.Equals("Item"))
                                    {
                                        string keyTemp = reader.GetAttribute("key");
                                        string valTemp = reader.GetAttribute("value");
                                        dic.Add(keyTemp, valTemp);
                                    }
                                }

                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    if (reader.LocalName.Equals("Data"))
                                    {
                                        if (dic.Count > 0)
                                            metaObj[fieldName] = dic;

                                        break;
                                    }
                                }
                            }
                            break;
                        default:
                            if (!String.IsNullOrEmpty(sData))
                                metaObj[fieldName] = sData;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return metaObj;
		}

		#region Import Dictionaries
		/// <summary>
		/// Reads the merchants.
		/// </summary>
		/// <param name="reader">The reader.</param>
		private void ReadMerchants(XmlReader reader)
		{
			int totalImportSteps = GetTotalImportSteps();

			int counter = 0;
			CatalogEntryDto.MerchantRow merchantRow = null;

			CatalogEntryDto merchantsDto = CatalogContext.Current.GetMerchantsDto();

			Catalog.Data.CatalogEntryAdmin admin = new Mediachase.Commerce.Catalog.Data.CatalogEntryAdmin();

			// Get the Subtree
			XmlReader readSubtree = reader.ReadSubtree();

			while (readSubtree.Read() && !readSubtree.EOF)
			{
				// Check if beginning of Merchant
				if (readSubtree.IsStartElement("Merchant"))
				{
					merchantRow = admin.CurrentDto.Merchant.NewMerchantRow();
					merchantRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
					merchantRow.MerchantId = Guid.NewGuid();
					merchantRow.Name = String.Empty;

					// read merchant properties
					while (readSubtree.Read() && !readSubtree.EOF)
					{
						if (!readSubtree.IsEmptyElement)
						{
							if (readSubtree.LocalName.Equals("Name"))
							{
								string name = readSubtree.ReadString();
								if (!String.IsNullOrEmpty(name))
									merchantRow.Name = name;
							}
						}

						// read to the end of merchant
						if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Merchant"))
						{
							if (String.IsNullOrEmpty(merchantRow.Name))
								break;

							DataRow[] rows = merchantsDto.Merchant.Select(String.Format("Name = '{0}'", merchantRow.Name.Replace("'", "''")));
							/*IEnumerable<CatalogEntryDto.MerchantRow> query = (from merchants in merchantsDto.Merchant
																				  select merchants).Where(String.Compare(m => m.Field<string>("Name"), "", true) == 0);*/

							if (rows.Length == 0)
                            {
								if (merchantRow.RowState == DataRowState.Detached)
									admin.CurrentDto.Merchant.Rows.Add(merchantRow);
                                counter++;
                            }

							// save merchant
							if (counter >= 20)
							{
								// save merchants
								if (admin.CurrentDto.HasChanges())
									admin.SaveMerchants();
								// clear the admin object
								admin = new Mediachase.Commerce.Catalog.Data.CatalogEntryAdmin();
								// reset the counter
								counter = 0;
							}
							break;
						}
					}
				}
			}

			if (admin.CurrentDto.HasChanges())
				admin.SaveMerchants();
		}

		/// <summary>
		/// Reads the packages.
		/// </summary>
		/// <param name="reader">The reader.</param>
		private void ReadPackages(XmlReader reader)
		{
			CultureInfo currentCultureInfo = GetImportExportCulture();

			double parsedDouble;

			int counter = 0;
			ShippingMethodDto.PackageRow packageRow = null;

			ShippingMethodDto smDto = ShippingManager.GetShippingPackages();

			// Get the Subtree
			XmlReader readSubtree = reader.ReadSubtree();

			while (readSubtree.Read() && !readSubtree.EOF)
			{
				// Check if beginning of Package
				if (readSubtree.IsStartElement("Package"))
				{
					packageRow = smDto.Package.NewPackageRow();
					packageRow.ApplicationId = OrderConfiguration.Instance.ApplicationId;
					packageRow.Name = String.Empty;

					// read package properties
					while (readSubtree.Read() && !readSubtree.EOF)
					{
						if (!readSubtree.IsEmptyElement)
						{
							if (readSubtree.LocalName.Equals("Name"))
							{
								string name = readSubtree.ReadString();
								if (!String.IsNullOrEmpty(name))
									packageRow.Name = name;
							}

							if (readSubtree.LocalName.Equals("Description"))
								packageRow.Description = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("Width"))
							{
								Double.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDouble);
								packageRow.Width = parsedDouble;
							}

							if (readSubtree.LocalName.Equals("Height"))
							{
								Double.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDouble);
								packageRow.Height = parsedDouble;
							}

							if (readSubtree.LocalName.Equals("Length"))
							{
								Double.TryParse(readSubtree.ReadString(), NumberStyles.Float, currentCultureInfo, out parsedDouble);
								packageRow.Length = parsedDouble;
							}
						}

						// read to the end of package
						if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Package"))
						{
							if (String.IsNullOrEmpty(packageRow.Name))
								break;

							DataRow[] rows = smDto.Package.Select(String.Format("Name = '{0}'", packageRow.Name.Replace("'", "''")));

							if (rows.Length == 0)
							{
								if (packageRow.RowState == DataRowState.Detached)
									smDto.Package.Rows.Add(packageRow);
								counter++;
							}
							else
							{
								// update the package
								ShippingMethodDto.PackageRow row = (ShippingMethodDto.PackageRow)rows[0];
								row.Description = packageRow.IsDescriptionNull() ? null : packageRow.Description;
								if (packageRow.IsWidthNull())
									row.SetWidthNull();
								else
									row.Width = packageRow.Width;
								if (packageRow.IsHeightNull())
									row.SetHeightNull();
								else
									row.Height = packageRow.Height;
								if (packageRow.IsLengthNull())
									packageRow.SetLengthNull();
								else
									row.Length = packageRow.Length;
							}

							// save package
							if (counter >= 20)
							{
								// save packages
								if (smDto.HasChanges())
									ShippingManager.SavePackage(smDto);

								// reset the counter
								counter = 0;
							}
							break;
						}
					}
				}
			}

			if (smDto.HasChanges())
				ShippingManager.SavePackage(smDto);
		}

		/// <summary>
		/// Reads the TaxCategories.
		/// </summary>
		/// <param name="reader">The reader.</param>
		private void ReadTaxCategories(XmlReader reader)
		{
			int counter = 0;
			CatalogTaxDto.TaxCategoryRow tcRow = null;

			CatalogTaxDto catalogTaxDto = CatalogTaxManager.GetTaxCategories();

			// Get the Subtree
			XmlReader readSubtree = reader.ReadSubtree();

			while (readSubtree.Read() && !readSubtree.EOF)
			{
				// Check if beginning of TaxCategory
				if (readSubtree.IsStartElement("TaxCategory"))
				{
					tcRow = catalogTaxDto.TaxCategory.NewTaxCategoryRow();
					tcRow.ApplicationId = OrderConfiguration.Instance.ApplicationId;
					tcRow.Name = String.Empty;

					// read tax category properties
					while (readSubtree.Read() && !readSubtree.EOF)
					{
						if (!readSubtree.IsEmptyElement)
						{
							if (readSubtree.LocalName.Equals("Name"))
							{
								string name = readSubtree.ReadString();
								if (!String.IsNullOrEmpty(name))
									tcRow.Name = name;
							}
						}

						// read to the end of TaxCategory
						if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("TaxCategory"))
						{
							if (String.IsNullOrEmpty(tcRow.Name))
								break;

							DataRow[] rows = catalogTaxDto.TaxCategory.Select(String.Format("Name = '{0}'", tcRow.Name.Replace("'", "''")));

							if (rows.Length == 0)
							{
								if (tcRow.RowState == DataRowState.Detached)
									catalogTaxDto.TaxCategory.Rows.Add(tcRow);
								counter++;
							}

							// save tax category
							if (counter >= 20)
							{
								// save tax categories
								if (catalogTaxDto.HasChanges())
									CatalogTaxManager.SaveTaxCategory(catalogTaxDto);

								// reset the counter
								counter = 0;
							}
							break;
						}
					}
				}
			}

			if (catalogTaxDto.HasChanges())
				CatalogTaxManager.SaveTaxCategory(catalogTaxDto);
		}

		/// <summary>
		/// Reads the Warehouses.
		/// </summary>
		/// <param name="reader">The reader.</param>
		private void ReadWarehouses(XmlReader reader)
		{
			CultureInfo currentCultureInfo = GetImportExportCulture();

			int parsedInt;
			bool parsedBool;

			int counter = 0;
			WarehouseDto.WarehouseRow whRow = null;

			WarehouseDto whDto = WarehouseManager.GetWarehouseDto();

			// Get the Subtree
			XmlReader readSubtree = reader.ReadSubtree();

			while (readSubtree.Read() && !readSubtree.EOF)
			{
				// Check if beginning of Warehouse
				if (readSubtree.IsStartElement("Warehouse"))
				{
					whRow = whDto.Warehouse.NewWarehouseRow();
					whRow.ApplicationId = CatalogConfiguration.Instance.ApplicationId;
					whRow.Name = String.Empty;
					whRow.Created = DateTime.UtcNow;
					whRow.Modified = DateTime.UtcNow;
					string userName = FrameworkContext.Current.Profile != null ? FrameworkContext.Current.Profile.UserName : String.Empty;
					whRow.CreatorId = userName;
					whRow.ModifierId = userName;
					whRow.IsPrimary = false;

					// read warehouse properties
					while (readSubtree.Read() && !readSubtree.EOF)
					{
						if (!readSubtree.IsEmptyElement)
						{
							if (readSubtree.LocalName.Equals("Name"))
							{
								string name = readSubtree.ReadString();
								if (!String.IsNullOrEmpty(name))
									whRow.Name = name;
							}

							if (readSubtree.LocalName.Equals("IsActive"))
							{
								Boolean.TryParse(readSubtree.ReadString(), out parsedBool);
								whRow.IsActive = parsedBool;
							}

							if (readSubtree.LocalName.Equals("IsPrimary"))
							{
								Boolean.TryParse(readSubtree.ReadString(), out parsedBool);
								whRow.IsPrimary = parsedBool;
							}

							if (readSubtree.LocalName.Equals("SortOrder"))
							{
								Int32.TryParse(readSubtree.ReadString(), NumberStyles.Integer, currentCultureInfo, out parsedInt);
								whRow.SortOrder = parsedInt;
							}

							if (readSubtree.LocalName.Equals("Code"))
								whRow.Code = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("FirstName"))
								whRow.FirstName = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("LastName"))
								whRow.LastName = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("Organization"))
								whRow.Organization = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("Line1"))
								whRow.Line1 = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("Line2"))
								whRow.Line2 = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("City"))
								whRow.City = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("State"))
								whRow.State = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("CountryCode"))
								whRow.CountryCode = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("CountryName"))
								whRow.CountryName = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("PostalCode"))
								whRow.PostalCode = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("RegionCode"))
								whRow.RegionCode = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("RegionName"))
								whRow.RegionName = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("DaytimePhoneNumber"))
								whRow.DaytimePhoneNumber = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("EveningPhoneNumber"))
								whRow.EveningPhoneNumber = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("FaxNumber"))
								whRow.FaxNumber = readSubtree.ReadString();

							if (readSubtree.LocalName.Equals("Email"))
								whRow.Email = readSubtree.ReadString();
						}

						// read to the end of warehouse
						if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("Warehouse"))
						{
							if (String.IsNullOrEmpty(whRow.Name))
								break;

							DataRow[] rows = whDto.Warehouse.Select(String.Format("Name = '{0}'", whRow.Name.Replace("'", "''")));

							if (rows.Length == 0)
							{
								if (whRow.RowState == DataRowState.Detached)
									whDto.Warehouse.Rows.Add(whRow);
								counter++;
							}
							else
							{
								// update the warehouse
								WarehouseDto.WarehouseRow row = (WarehouseDto.WarehouseRow)rows[0];
								row.ModifierId = whRow.ModifierId;
								row.Modified = whRow.Modified;
								row.IsActive = whRow.IsActive;
								row.IsPrimary = whRow.IsPrimary;
								row.SortOrder = whRow.SortOrder;
								row.Code = whRow.Code;
								row.FirstName = whRow.IsFirstNameNull() ? null : whRow.FirstName;
								row.LastName = whRow.IsLastNameNull() ? null : whRow.LastName;
								row.Organization = whRow.IsOrganizationNull() ? null : whRow.Organization;
								row.Line1 = whRow.IsLine1Null() ? null : whRow.Line1;
								row.Line2 = whRow.IsLine2Null() ? null : whRow.Line2;
								row.City = whRow.IsCityNull() ? null : whRow.City;
								row.State = whRow.IsStateNull() ? null : whRow.State;
								row.CountryCode = whRow.IsCountryCodeNull() ? null : whRow.CountryCode;
								row.CountryName = whRow.IsCountryNameNull() ? null : whRow.CountryName;
								row.PostalCode = whRow.IsPostalCodeNull() ? null : whRow.PostalCode;
								row.RegionCode = whRow.IsRegionCodeNull() ? null : whRow.RegionCode;
								row.RegionName = whRow.IsRegionNameNull() ? null : whRow.RegionName;
								row.DaytimePhoneNumber = whRow.IsDaytimePhoneNumberNull() ? null : whRow.DaytimePhoneNumber;
								row.EveningPhoneNumber = whRow.IsEveningPhoneNumberNull() ? null : whRow.EveningPhoneNumber;
								row.FaxNumber = whRow.IsFaxNumberNull() ? null : whRow.FaxNumber;
								row.Email = whRow.IsEmailNull() ? null : whRow.Email;
							}

							// save warehouse
							if (counter >= 20)
							{
								// save warehouses
								if (whDto.HasChanges())
									WarehouseManager.SaveWarehouse(whDto);

								// reset the counter
								counter = 0;
							}
							break;
						}
					}
				}
			}

			if (whDto.HasChanges())
				WarehouseManager.SaveWarehouse(whDto);
		}

		/// <summary>
		/// Reads the AssociationTypes.
		/// </summary>
		/// <param name="reader">The reader.</param>
		private void ReadAssociationTypes(XmlReader reader)
		{
			int counter = 0;
			CatalogAssociationDto.AssociationTypeRow associationTypeRow = null;

			CatalogAssociationDto associationDto = CatalogAssociationManager.GetCatalogAssociationDto(0);

			// Get the Subtree
			XmlReader readSubtree = reader.ReadSubtree();

			while (readSubtree.Read() && !readSubtree.EOF)
			{
				// Check if beginning of AssociationType
				if (readSubtree.IsStartElement("AssociationType"))
				{
					associationTypeRow = associationDto.AssociationType.NewAssociationTypeRow();
					associationTypeRow.AssociationTypeId = String.Empty;

					// read association type properties
					while (readSubtree.Read() && !readSubtree.EOF)
					{
						if (!readSubtree.IsEmptyElement)
						{
							if (readSubtree.LocalName.Equals("TypeId"))
							{
								string typeId = readSubtree.ReadString();
								if (!String.IsNullOrEmpty(typeId))
									associationTypeRow.AssociationTypeId = typeId;
							}
						}

						if (readSubtree.LocalName.Equals("Description"))
							associationTypeRow.Description = readSubtree.ReadString();

						// read to the end of AssociationType
						if (readSubtree.NodeType == XmlNodeType.EndElement && readSubtree.LocalName.Equals("AssociationType"))
						{
							if (String.IsNullOrEmpty(associationTypeRow.AssociationTypeId))
								break;

							DataRow[] rows = associationDto.AssociationType.Select(String.Format("AssociationTypeId = '{0}'", associationTypeRow.AssociationTypeId.Replace("'", "''")));

							if (rows.Length == 0)
							{
								if (associationTypeRow.RowState == DataRowState.Detached)
									associationDto.AssociationType.Rows.Add(associationTypeRow);
								counter++;
							}

							// save association types
							if (counter >= 20)
							{
								if (associationDto.HasChanges())
									CatalogAssociationManager.SaveAssociationType(associationDto);

								// reset the counter
								counter = 0;
							}
							break;
						}
					}
				}
			}

			if (associationDto.HasChanges())
				CatalogAssociationManager.SaveAssociationType(associationDto);
		}
		#endregion
		#endregion
	}
}
