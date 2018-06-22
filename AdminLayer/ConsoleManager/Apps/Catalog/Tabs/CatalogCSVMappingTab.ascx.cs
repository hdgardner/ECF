using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.ImportExport;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Shared;
using Mediachase.FileUploader.Web;
using Mediachase.MetaDataPlus;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using System.Data;
using Mediachase.MetaDataPlus.Import;
using Mediachase.Cms;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Commerce.Catalog.CSVImport;
using System.Globalization;
using Mediachase.MetaDataPlus.Import.Parser;
using Mediachase.Commerce.Catalog.Objects;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Catalog.Tabs
{
	public partial class CatalogCSVMappingTab : CatalogBaseUserControl
	{
		protected System.Web.UI.WebControls.RadioButtonList rbObjects;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdObject;

		private const string _SelectedFilePathString = "Import_SelectedFilePath_D61CCF81-EBF7-4d04-93FE-11C6084036B7";
		private const string _SelectedMappingFilePathString = "Import_SelectedMappingFilePath_D61CCF81-EBF7-4d04-93FE-11C6084036B7";

		private string _sourcePath = "";
		private string _rulesPath = "";

		private string SourcePath
		{
			get
			{
				if (_sourcePath == String.Empty)
					_sourcePath = HttpContext.Current.Server.MapPath(ManagementHelper.GetImportExportFolderPath("csv\\data"));

				return _sourcePath;
			}
		}

		private string RulesPath
		{
			get
			{
				if (_rulesPath == String.Empty)
					_rulesPath = HttpContext.Current.Server.MapPath(ManagementHelper.GetImportExportFolderPath("csv\\rule"));

				return _rulesPath;
			}
		}


		private string LastPostFile
		{
			get
			{
				if (Session[_SelectedFilePathString] != null)
					return Session[_SelectedFilePathString].ToString();
				else return "";
			}
			set
			{
				Session[_SelectedFilePathString] = value;
			}
		}

		private string LastMapFile
		{
			get
			{
				if (Session[_SelectedMappingFilePathString] != null)
					return Session[_SelectedMappingFilePathString].ToString();
				else return "";
			}
			set
			{
				Session[_SelectedMappingFilePathString] = value;
			}
		}

		private MetaDataPlus.Import.Rule ClassRule
		{
			get
			{
				return (MetaDataPlus.Import.Rule)ViewState["ClassRule"];
			}
			set
			{
				ViewState["ClassRule"] = value;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindMappingType();
				BindLanguages();
				BindDataFiles(this.RulesPath, this.LastMapFile, ddlMappingFiles);
				BindDataFiles(this.SourcePath, this.LastPostFile, ddlDataFiles);
				BindDataType();
			}
		}

		/// <summary>
		/// Binds the languages.
		/// </summary>
		private void BindLanguages()
		{
			ddlLanguage.DataValueField = "LangId";
			ddlLanguage.DataTextField = "LangName";

			DataTable languages = Language.GetAllLanguagesDT();
			foreach (DataRow row in languages.Rows)
			{
				if (String.IsNullOrEmpty(row["LangName"].ToString()))
					continue;

				CultureInfo culture = CultureInfo.CreateSpecificCulture(row["LangName"].ToString());
				ListItem item = new ListItem(culture.DisplayName, culture.Name.ToLower());
				ddlLanguage.Items.Add(item);
			}
		}

		/// <summary>
		/// Binds the mapping types.
		/// </summary>
		private void BindMappingType()
		{
			ddlTypeData.Items.Clear();
			ddlTypeData.Items.Add(new ListItem("Category w/ Meta Data", "Category"));
			ddlTypeData.Items.Add(new ListItem("Entry w/ Meta Data", "Entry"));
			ddlTypeData.Items.Add(new ListItem("Entry Relation", "EntryRelation"));
			ddlTypeData.Items.Add(new ListItem("Entry Association", "EntryAssociation"));
			ddlTypeData.Items.Add(new ListItem("Variation w/ Inventory", "Variation"));
			ddlTypeData.Items.Add(new ListItem("Sale Price", "SalePrice"));

			BindMetaclass("CatalogNode");
		}

		/// <summary>
		/// Binds the meta classes.
		/// </summary>
		private void BindMetaclass(string metaClassName)
		{
			// Bind Meta classes
			MetaClass catalogEntry = MetaClass.Load(CatalogContext.MetaDataContext, metaClassName);
			ddlMetaClass.Items.Clear();
			if (catalogEntry != null)
			{
				ddlMetaClass.Items.Add(new ListItem("", ""));
				MetaClassCollection metaClasses = catalogEntry.ChildClasses;
				foreach (MetaClass metaClass in metaClasses)
				{
					ddlMetaClass.Items.Add(new ListItem(metaClass.FriendlyName, metaClass.Name));
				}
				ddlMetaClass.DataBind();
			}
		}

		private Encoding GetEncoding(string sEncoding)
		{
			if (sEncoding == String.Empty || sEncoding == "Default" || sEncoding == null)
				return Encoding.Default;
			else
				return Encoding.GetEncoding(sEncoding);
		}

		#region BindDataFiles
		private void BindDataFiles(string _path, string _chosenFile, object _ddl)
		{
			DropDownList ddl = (DropDownList)_ddl;
			if (!Directory.Exists(_path))
				Directory.CreateDirectory(_path);
			else
			{
				ddl.DataSource = Directory.GetFiles(_path);
				ddl.DataBind();
				for (int i = 0; i < ddl.Items.Count; i++)
				{
					string[] val = ddl.Items[i].Value.Split('\\');
					ddl.Items[i].Text = val[val.Length - 1];
				}
			}
			ddl.Items.Insert(0, new ListItem("", ""));

			if (_chosenFile != "")
			{
				for (int i = 0; i < ddl.Items.Count; i++)
					if (ddl.Items[i].Value.Equals(_chosenFile))
					{
						ddl.SelectedIndex = i;
						break;
					}
			}
		}
		#endregion

		#region BindDataType
		private void BindDataType()
		{
			DataRow dr;
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("key", typeof(string)));
			dt.Columns.Add(new DataColumn("name", typeof(string)));
			dt.Columns.Add(new DataColumn("Type", typeof(string)));
			dt.Columns.Add(new DataColumn("IsSystemDictionary", typeof(bool)));
			dt.Columns.Add(new DataColumn("AllowNulls", typeof(bool)));
			dt.Columns.Add(new DataColumn("IsConstant", typeof(bool)));

			MappingMetaClass mmc = null;
			MetaDataPlus.Import.Rule mapping = null;
			string MetaClassName = ddlMetaClass.SelectedValue;
			string language = ddlLanguage.SelectedValue;

			switch (ddlTypeData.SelectedValue)
			{
				case "Category":
					CatalogContext.MetaDataContext.UseCurrentUICulture = false;
					CatalogContext.MetaDataContext.Language = language;

					if (!String.IsNullOrEmpty(MetaClassName))
						mmc = new CategoryMappingMetaClass(CatalogContext.MetaDataContext, MetaClassName, -1);
					else
						mmc = new CategoryMappingMetaClass(CatalogContext.MetaDataContext, -1);

					CatalogContext.MetaDataContext.UseCurrentUICulture = true;

					mapping = mmc.CreateClassRule();
					mapping.Attribute.Add("TypeName", "Category");

					if (!String.IsNullOrEmpty(language)) mapping.Attribute.Add("Language", language);
					break;

				case "Entry":
					CatalogContext.MetaDataContext.UseCurrentUICulture = false;
					CatalogContext.MetaDataContext.Language = language;

					if (!String.IsNullOrEmpty(MetaClassName))
						mmc = new EntryMappingMetaClass(CatalogContext.MetaDataContext, MetaClassName, -1);
					else
						mmc = new EntryMappingMetaClass(CatalogContext.MetaDataContext, -1);

					CatalogContext.MetaDataContext.UseCurrentUICulture = true;

					mapping = mmc.CreateClassRule();
					mapping.Attribute.Add("TypeName", "Entry");

					if (!String.IsNullOrEmpty(language)) mapping.Attribute.Add("Language", language);
					break;

				case "EntryRelation":
					mmc = new EntryRelationMappingMetaClass(CatalogContext.MetaDataContext, -1);
					mapping = mmc.CreateClassRule();
					mapping.Attribute.Add("TypeName", "EntryRelation");
					break;

				case "EntryAssociation":
					mmc = new EntryAssociationMappingMetaClass(CatalogContext.MetaDataContext, -1);
					mapping = mmc.CreateClassRule();
					mapping.Attribute.Add("TypeName", "EntryAssociation");
					break;

				case "Variation":
					mmc = new VariationMappingMetaClass(CatalogContext.MetaDataContext, -1);
					mapping = mmc.CreateClassRule();
					mapping.Attribute.Add("TypeName", "Variation");
					break;

				case "SalePrice":
					mmc = new PricingMappingMetaClass(CatalogContext.MetaDataContext, -1);
					mapping = mmc.CreateClassRule();
					mapping.Attribute.Add("TypeName", "SalePrice");
					break;

				default:
					return;
			}
			mapping.Attribute.Add("Delimiter", this.ddlDelimiter.SelectedValue);
			mapping.Attribute.Add("TextQualifier", this.ddlTextQualifier.SelectedValue);
			foreach (ColumnInfo ci in mmc.ColumnInfos)
			{
				dr = dt.NewRow();
				if (ci.Field.IsSystem)
				{
					dr["key"] = ci.FieldName;
					dr["name"] = (ci.FieldFriendlyName != null) ? ci.FieldFriendlyName : ci.FieldName;
				}
				else
				{
					dr["key"] = ci.FieldName;
					dr["name"] = ci.FieldFriendlyName;
				}

				if (ci.Field.MultiLanguageValue && !String.IsNullOrEmpty(language))
					dr["name"] += String.Format(" ({0})", language);

				dr["Type"] = ci.Field.DataType.ToString();
				dr["IsSystemDictionary"] = ci.IsSystemDictionary;
				dr["AllowNulls"] = ci.Field.AllowNulls;
				dr["IsConstant"] = false;
				dt.Rows.Add(dr);
				mapping.Add(new RuleItem(ci.Field, MetaDataPlus.Import.FillType.NotUse));
			}
			grdFields.Columns[0].HeaderText = RM.GetString("IMPORT_MAPPING_TITLE_FIELDS");
			grdFields.Columns[1].HeaderText = RM.GetString("IMPORT_MAPPING_TITLE_COLUMN_HEADERS");
			grdFields.Columns[2].HeaderText = "Custom values";
			grdFields.DataSource = dt;
			grdFields.DataBind();
			this.ClassRule = mapping;

			if (ddlDataFiles.SelectedIndex > 0)
			{
				IIncomingDataParser parser = null;
				DataSet rawData = null;
				try
				{
					char chTextQualifier = (this.ddlTextQualifier.SelectedValue == "") ? '\0' : char.Parse(this.ddlTextQualifier.SelectedValue);
					parser = new CsvIncomingDataParser(SourcePath, true, char.Parse(this.ddlDelimiter.SelectedValue), chTextQualifier, true, GetEncoding(this.ddlEncoding.SelectedValue));
					rawData = parser.Parse(ddlDataFiles.SelectedItem.Text, null);
				}
				catch (Exception ex)
				{
					DisplayErrorMessage(ex.Message);
					return;
				}
				DataTable dtSource = rawData.Tables[0];

				DataTable dtColumns = new DataTable();
				dtColumns.Columns.Add(new DataColumn("Text", typeof(string)));
				dtColumns.Columns.Add(new DataColumn("Value", typeof(string)));

				foreach (DataColumn dc in dtSource.Columns)
				{
					dr = dtColumns.NewRow();
					dr["Text"] = "Column " + (dc.Ordinal + 1) + " - " + dc.ColumnName;
					dr["Value"] = dc.ColumnName;
					dtColumns.Rows.Add(dr);
				}

				foreach (DataGridItem dgi in grdFields.Items)
				{
					DropDownList ddl = (DropDownList)dgi.FindControl("ddlFields");
					TextBox tbCustomValue = (TextBox)dgi.FindControl("tbCustomValue");
					DropDownList ddlValues = (DropDownList)dgi.FindControl("ddlValues");

					string sKey = dgi.Cells[3].Text;
					string sType = dgi.Cells[4].Text;
					bool sIsSystemDictionary = bool.Parse(dgi.Cells[5].Text);
					bool allowNulls = bool.Parse(dgi.Cells[6].Text);
					bool IsConstant = bool.Parse(dgi.Cells[7].Text);

					bool useDictionaryControl = GetUseDictionaryFlag(dgi);
					
					if (!IsConstant)
					{
						if (ddl != null)
						{
							ddl.DataSource = dtColumns;
							ddl.DataTextField = "Text";
							ddl.DataValueField = "Value";
							ddl.DataBind();

							if (sIsSystemDictionary)
								ddl.Items.Insert(0, new ListItem("<" + RM.GetString("IMPORT_TYPE_VALUES_DICTIONARY_VALUE") + ">", "CustomValue"));
							else
								ddl.Items.Insert(0, new ListItem("<" + RM.GetString("IMPORT_TYPE_VALUES_CUSTOM_VALUE") + ">", "CustomValue"));

							if (allowNulls)
								ddl.Items.Insert(0, new ListItem("", ""));
							else
								ddl.Items.Insert(0, new ListItem("<" + RM.GetString("IMPORT_TYPE_VALUES_NOT_SET") + ">", "NotSet"));

							string customControlID = useDictionaryControl ? ddlValues.ClientID : tbCustomValue.ClientID;

							string jsDllOnChange = String.Format("ddlOnChange(this, 'CustomValue', '{0}')", customControlID);
							ddl.Attributes.Add("OnChange", jsDllOnChange);
						}
					}

					//fill custom or dictionary controls
					ddlValues.Visible = useDictionaryControl;
					tbCustomValue.Visible = !ddlValues.Visible;

					if (sType.Equals(MetaDataType.Boolean.ToString()) || sType.Equals(MetaDataType.Bit.ToString()))
					{
						ddlValues.Items.Clear();
						ddlValues.Items.Add("True");
						ddlValues.Items.Add("False");
					}

					if (sKey.Equals("sys_RowAction"))
					{
						ddlValues.Items.Clear();
						ddlValues.Items.Add(RowAction.Default.ToString());
						ddlValues.Items.Add(RowAction.Insert.ToString());
						ddlValues.Items.Add(RowAction.Update.ToString());
						ddlValues.Items.Add(RowAction.Delete.ToString());
					}

					switch (ddlTypeData.SelectedValue)
					{
						case "Category":
							if (sKey == "TemplateName")
							{
								TemplateDto templates = DictionaryManager.GetTemplateDto();
								if (templates.main_Templates.Count > 0)
								{
									DataView view = templates.main_Templates.DefaultView;
									view.RowFilter = "TemplateType = 'node'";
									ddlValues.DataTextField = "FriendlyName";
									ddlValues.DataValueField = "Name";
									ddlValues.DataSource = view;
									ddlValues.DataBind();
								}
							}
							break;
						case "Entry":
							if (sKey == "ClassTypeId")
							{
								ddlValues.Items.Clear();
								ddlValues.Items.Add(new ListItem("Product", EntryType.Product));
								ddlValues.Items.Add(new ListItem("Variation/Sku", EntryType.Variation));
								ddlValues.Items.Add(new ListItem("Package", EntryType.Package));
								ddlValues.Items.Add(new ListItem("Bundle", EntryType.Bundle));
								ddlValues.Items.Add(new ListItem("Dynamic Package", EntryType.DynamicPackage));
							}

							if (sKey == "TemplateName")
							{
								TemplateDto templates = DictionaryManager.GetTemplateDto();
								if (templates.main_Templates.Count > 0)
								{
									DataView view = templates.main_Templates.DefaultView;
									view.RowFilter = "TemplateType = 'entry'";
									ddlValues.DataTextField = "FriendlyName";
									ddlValues.DataValueField = "Name";
									ddlValues.DataSource = view;
									ddlValues.DataBind();
								}
							}
							break;
						case "EntryAssociation":
							if (sKey == "AssociationType")
							{
								ddlValues.Items.Clear();
								CatalogAssociationDto dto = CatalogContext.Current.GetCatalogAssociationDto(0);
								if (dto.AssociationType.Count > 0)
								{
									ddlValues.DataTextField = "Description";
									ddlValues.DataValueField = "AssociationTypeId";
									ddlValues.DataSource = dto.AssociationType;
									ddlValues.DataBind();
								}
							}
							break;
						case "Variation":
							if (sKey == "TaxCategoryId")
							{
								CatalogTaxDto taxes = CatalogTaxManager.GetTaxCategories();
								if (taxes.TaxCategory != null)
								{
									ddlValues.DataTextField = "Name";
									ddlValues.DataValueField = "TaxCategoryId";
									ddlValues.DataSource = taxes.TaxCategory.Rows;
									ddlValues.DataBind();
								}
							}
							if (sKey == "MerchantId")
							{
								CatalogEntryDto merchants = CatalogContext.Current.GetMerchantsDto();
								if (merchants.Merchant != null)
								{
									ddlValues.DataTextField = "Name";
									ddlValues.DataValueField = "MerchantId";
									ddlValues.DataSource = merchants.Merchant.Rows;
									ddlValues.DataBind();
								}
							}
							if (sKey == "WarehouseId")
							{
								WarehouseDto warehouses = WarehouseManager.GetWarehouseDto();
								if (warehouses.Warehouse != null)
								{
									ddlValues.DataTextField = "Name";
									ddlValues.DataValueField = "WarehouseId";
									ddlValues.DataSource = warehouses.Warehouse.Rows;
									ddlValues.DataBind();
								}
							}
							if (sKey == "PackageId")
							{
								ShippingMethodDto shippingDto = ShippingManager.GetShippingPackages();
								if (shippingDto.Package != null)
								{
									ddlValues.DataTextField = "Name";
									ddlValues.DataValueField = "PackageId";
									ddlValues.DataSource = shippingDto.Package.Rows;
									ddlValues.DataBind();
								}
							}

							break;
						case "SalePrice":
							if (sKey == "SaleType")
							{
								ddlValues.Items.Clear();
								foreach (SalePriceTypeDefinition element in CatalogConfiguration.Instance.SalePriceTypes)
								{
									ListItem li = new ListItem(UtilHelper.GetResFileString(element.Description), element.Value.ToString());
									ddlValues.Items.Add(li);
								}
							}
							if (sKey == "Currency")
							{
								CurrencyDto dto = CatalogContext.Current.GetCurrencyDto();
								ddlValues.DataTextField = "Name";
								ddlValues.DataValueField = "CurrencyCode";
								ddlValues.DataSource = dto.Currency;
								ddlValues.DataBind();
							}
							break;

						default:
							break;
					}
				}
			}
		}
		#endregion

		#region GetUseDictionaryFlag
		private bool GetUseDictionaryFlag(DataGridItem item)
		{
			string sKey = item.Cells[3].Text;
			string sType = item.Cells[4].Text;
			bool sIsSystemDictionary = bool.Parse(item.Cells[5].Text);

			bool retVal = sIsSystemDictionary;
			retVal = retVal || sType.Equals(MetaDataType.Boolean.ToString()) || sType.Equals(MetaDataType.Bit.ToString());
			retVal = retVal || sKey.Equals("sys_RowAction");

			return retVal;
		}
		#endregion

		#region RestoreValues
		private void RestoreValues(MetaDataPlus.Import.Rule mapping)
		{
			foreach (DataGridItem item in grdFields.Items)
			{
				DropDownList ddl = (DropDownList)item.FindControl("ddlFields");
				TextBox tbCustomValue = (TextBox)item.FindControl("tbCustomValue");
				DropDownList ddlValues = (DropDownList)item.FindControl("ddlValues");

				string sKey = item.Cells[3].Text;
				//string sType = item.Cells[4].Text;
				//bool sIsSystemDictionary = bool.Parse(item.Cells[5].Text);
				bool IsConstant = bool.Parse(item.Cells[7].Text);

				bool useDictionaryControl = GetUseDictionaryFlag(item);

				if (IsConstant)
				{
					if (mapping.Attribute[sKey] != null)
					{
						string val = mapping.Attribute[sKey];
						for (int i = 0; i < ddl.Items.Count; i++)
							if (ddl.Items[i].Value == val)
							{
								ddl.SelectedIndex = i;
								break;
							}
					}
					continue;
				}

				if (mapping[sKey] != null)
				{
					MetaDataPlus.Import.RuleItem ruleItem = (MetaDataPlus.Import.RuleItem)mapping[sKey];
					if (ruleItem.SrcColumnName != "")
					{
						for (int i = 0; i < ddl.Items.Count; i++)
							if (ddl.Items[i].Value == ruleItem.SrcColumnName)
							{
								ddl.SelectedIndex = i;
								break;
							}
					}
					else
					{
						if (ruleItem.CustomValue != "")
						{
							if (useDictionaryControl)
							{
								for (int i = 0; i < ddl.Items.Count; i++)
									if (ddl.Items[i].Value == "CustomValue")
									{
										ddl.SelectedIndex = i;
										break;
									}
								if (ddl.SelectedIndex > 0)
								{
									ddlValues.Style.Add("display", "block");
									for (int i = 0; i < ddlValues.Items.Count; i++)
										if (ddlValues.Items[i].Value == ruleItem.CustomValue)
										{
											ddlValues.SelectedIndex = i;
											break;
										}
								}
							}
							else
							{
								for (int i = 0; i < ddl.Items.Count; i++)
									if (ddl.Items[i].Value == "CustomValue")
									{
										ddl.SelectedIndex = i;
										tbCustomValue.Style.Add("display", "block");
										tbCustomValue.Text = ruleItem.CustomValue;
										break;
									}
							}
						}
					}
				}
			}
		}
		#endregion

		#region MappingFill
		private void MappingFill()
		{
			MetaDataPlus.Import.Rule mapping = this.ClassRule;

			foreach (DataGridItem item in grdFields.Items)
			{
				DropDownList ddl = (DropDownList)item.FindControl("ddlFields");
				TextBox tbCustomValue = (TextBox)item.FindControl("tbCustomValue");
				DropDownList ddlValues = (DropDownList)item.FindControl("ddlValues");

				string sKey = item.Cells[3].Text;
				bool IsConstant = bool.Parse(item.Cells[7].Text);

				bool useDictionaryControl = GetUseDictionaryFlag(item);

				MetaDataPlus.Import.RuleItem ruleItem = (MetaDataPlus.Import.RuleItem)mapping[sKey];

				if (IsConstant)
				{
					if (mapping.Attribute[sKey] != null)
						mapping.Attribute[sKey] = ddl.SelectedValue;
					else
						mapping.Attribute.Add(sKey, ddl.SelectedValue);
				}
				else
				{
					switch (ddl.SelectedValue)
					{
						case "CustomValue":
							mapping[sKey].FillType = MetaDataPlus.Import.FillType.Custom;
							if (useDictionaryControl)
								ruleItem.CustomValue = ddlValues.SelectedValue;
							else
								ruleItem.CustomValue = tbCustomValue.Text;
							ruleItem.SrcColumnName = "";
							break;
						case "NotSet":
							ruleItem.FillType = MetaDataPlus.Import.FillType.NotUse;
							ruleItem.SrcColumnName = "";
							ruleItem.CustomValue = "";
							break;
						case "":
							ruleItem.FillType = MetaDataPlus.Import.FillType.Default;
							ruleItem.SrcColumnName = "";
							ruleItem.CustomValue = "";
							break;
						default:
							ruleItem.FillType = MetaDataPlus.Import.FillType.CopyValue;
							ruleItem.SrcColumnName = ddl.SelectedValue;
							ruleItem.SrcColumnType = typeof(string);
							ruleItem.CustomValue = "";
							break;

					}
				}
			}
			this.ClassRule = mapping;
		}
		#endregion

		public void ddlMappingType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ddlTypeData.SelectedValue.Equals("Category"))
			{
				BindMetaclass("CatalogNode");
				this.ddlMetaClass.Enabled = true;
			}
			else if (ddlTypeData.SelectedValue.Equals("Entry"))
			{
				BindMetaclass("CatalogEntry");
				this.ddlMetaClass.Enabled = true;
			}
			else if (ddlTypeData.SelectedValue.Equals("EntryRelation") ||
				ddlTypeData.SelectedValue.Equals("EntryAssociation") ||
				ddlTypeData.SelectedValue.Equals("Variation") ||
				ddlTypeData.SelectedValue.Equals("SalePrice"))
			{
				this.ddlMetaClass.SelectedIndex = 0;
				this.ddlMetaClass.Enabled = false;
			}
			BindDataType();
		}

		public void ddlMetaClassList_SelectedIndexChanged(object sender, EventArgs e)
		{
			MappingFill();
			MetaDataPlus.Import.Rule _mapping = this.ClassRule;
			BindDataType();
			if (_mapping != null)
				RestoreValues(_mapping);

		}
		public void ddlFields_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindDataType();
		}

		public void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			MappingFill();
			MetaDataPlus.Import.Rule _mapping = this.ClassRule;
			BindDataType();
			if (_mapping != null)
				RestoreValues(_mapping);
		}

		private void ddlDataFiles_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.LastPostFile = ddlDataFiles.SelectedValue;
			BindDataType();
		}

		public void SaveMapping_Click(object sender, EventArgs e)
		{
			if (this.tbMappingFileName.Text != "")
			{
				if (tbMappingFileName.Text.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) != -1)
				{
					DisplayErrorMessage("The mapping filename is invalid.");
					return;
				}

				string path = RulesPath;
				if (!Directory.Exists(path)) Directory.CreateDirectory(path);

				string _FilePath = Path.Combine(path, tbMappingFileName.Text + ".xml");

				MappingFill();

				MetaDataPlus.Import.Rule mapping = this.ClassRule;
				this.LastPostFile = this.ddlDataFiles.SelectedValue;

				if (!mapping.Attribute.Contains("DataFile"))
					mapping.Attribute.Add("DataFile", this.LastPostFile);

				if (!mapping.Attribute.Contains("Encoding"))
					mapping.Attribute.Add("Encoding", this.ddlEncoding.SelectedValue);

				Mediachase.MetaDataPlus.Import.Rule.XmlSerialize(mapping, _FilePath);
				this.LastMapFile = _FilePath;

				Response.Redirect(Request.RawUrl);
			}

		}

		public void btnLoadMapFile_Click(object sender, EventArgs e)
		{
			if (this.ddlMappingFiles.SelectedIndex > 0)
			{
				if (!File.Exists(this.ddlMappingFiles.SelectedValue))
				{
					//DisplayErrorMessage(String.Format("The file \"{0}\" does not exist.", this.ddlExMappingFiles.SelectedValue));
					return;
				}

				MetaDataPlus.Import.Rule mapping = MetaDataPlus.Import.Rule.XmlDeserialize(CatalogContext.MetaDataContext, ddlMappingFiles.SelectedValue);
				this.ClassRule = mapping;

				string sTypeName = mapping.Attribute["TypeName"];
				if (sTypeName != null)
				{
					for (int i = 0; i < this.ddlTypeData.Items.Count; i++)
						if (this.ddlTypeData.Items[i].Value == sTypeName)
						{
							this.ddlTypeData.SelectedIndex = i;
							break;
						}
				}

				if (ddlTypeData.SelectedValue.Equals("Category"))
				{
					BindMetaclass("CatalogNode");
					this.ddlMetaClass.Enabled = true;
				}
				else if (ddlTypeData.SelectedValue.Equals("Entry"))
				{
					BindMetaclass("CatalogEntry");
					this.ddlMetaClass.Enabled = true;
				}
				else if (ddlTypeData.SelectedValue.Equals("EntryRelation") ||
					ddlTypeData.SelectedValue.Equals("EntryAssociation") ||
					ddlTypeData.SelectedValue.Equals("Variation") ||
					ddlTypeData.SelectedValue.Equals("SalePrice"))
				{
					this.ddlMetaClass.SelectedIndex = 0;
					this.ddlMetaClass.Enabled = false;
				}

				for (int i = 0; i < this.ddlMetaClass.Items.Count; i++)
					if (mapping.ClassName.Equals(this.ddlMetaClass.Items[i].Value, StringComparison.OrdinalIgnoreCase))
					{
						this.ddlMetaClass.SelectedIndex = i;
						break;
					}

				if (this.ddlMetaClass.SelectedIndex == 0)
				{
					MetaClass _metaClass = MetaClass.Load(CatalogContext.MetaDataContext, mapping.ClassName);
					if (_metaClass == null)
					{
						DisplayErrorMessage(String.Format("The metaclass '{0}' does not exists.", mapping.ClassName));
					}
				}

				if (mapping.Attribute["Language"] != null)
				{
					string language = mapping.Attribute["Language"];
					if (!String.IsNullOrEmpty(language))
					{
						for (int i = 0; i < this.ddlLanguage.Items.Count; i++)
							if (this.ddlLanguage.Items[i].Value.Equals(language, StringComparison.OrdinalIgnoreCase))
							{
								this.ddlLanguage.SelectedIndex = i;
								break;
							}
					}
				}

				this.ddlDataFiles.SelectedIndex = 0;
				string sDataFile = Path.Combine(SourcePath, Path.GetFileName(mapping.Attribute["DataFile"]));
				if (sDataFile != null)
				{
					for (int i = 0; i < this.ddlDataFiles.Items.Count; i++)
						if (Path.Equals(this.ddlDataFiles.Items[i].Value, sDataFile))
						{
							this.ddlDataFiles.SelectedIndex = i;
							break;
						}
					if (this.ddlDataFiles.SelectedIndex == 0)
					{
						DisplayErrorMessage(String.Format("The file \"{0}\" does not exist in a folder \"{1}\".", Path.GetFileName(sDataFile), SourcePath));
						return;
					}
				}
				else
					DisplayErrorMessage("The wrong version of a mapping file. Required attribute is missed.");


				string sDelimiter = mapping.Attribute["Delimiter"];
				if (sDelimiter != null)
				{
					for (int i = 0; i < this.ddlDelimiter.Items.Count; i++)
						if (this.ddlDelimiter.Items[i].Value == sDelimiter)
						{
							this.ddlDelimiter.SelectedIndex = i;
							break;
						}
				}

				string sTextQualifier = mapping.Attribute["TextQualifier"];
				if (sTextQualifier != null)
				{
					for (int i = 0; i < this.ddlTextQualifier.Items.Count; i++)
						if (this.ddlTextQualifier.Items[i].Value == sTextQualifier)
						{
							this.ddlTextQualifier.SelectedIndex = i;
							break;
						}
				}

				string sEncoding = "Default";
				try
				{
					sEncoding = mapping.Attribute["Encoding"];
				}
				catch { }
				if (sEncoding != null)
				{
					for (int i = 0; i < this.ddlEncoding.Items.Count; i++)
						if (this.ddlDelimiter.Items[i].Value == sEncoding)
						{
							this.ddlEncoding.SelectedIndex = i;
							break;
						}
				}

				if (this.ddlDataFiles.SelectedIndex > 0)
				{
					this.tbMappingFileName.Text = Path.GetFileNameWithoutExtension(this.ddlMappingFiles.SelectedValue);
					this.ddlMappingFiles.SelectedIndex = 0;
				}
				BindDataType();
				RestoreValues(mapping);
				MappingFill();
			}
		}

	}
}