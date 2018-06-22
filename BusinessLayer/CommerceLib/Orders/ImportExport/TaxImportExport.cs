using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Mediachase.MetaDataPlus.Import.Parser;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.Commerce.Catalog.Dto;

namespace Mediachase.Commerce.Orders.ImportExport
{
	/// <summary>
	/// The ImportExportMessageType enumeration defines the import/export message type.
	/// </summary>
	[Serializable]
	public enum TaxImportExportMessageType
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
	public class TaxImportExportEventArgs : EventArgs
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
		public TaxImportExportEventArgs()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportExportEventArgs"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="percentage">The percentage.</param>
		public TaxImportExportEventArgs(string message, double percentage)
			: base()
		{
			this.Message = message;
			this.CompletedPercentage = percentage;
		}
	}

	/// <summary>
	/// Handles the import/export progress message.
	/// </summary>
	public delegate void TaxImportExportProgressMessageHandler(object source, TaxImportExportEventArgs args);

	/// <summary>
	/// Implements operations for tax import.
	/// </summary>
	public class TaxImportExport
	{
		// Following private member apparently never used
		private static readonly object _lockObject = new object();

		#region ImportExport Steps
		
		internal enum ImportSteps
		{
			Init = 0x00,
			StartParseFile = 0x01,
			EndParseFile = 0x02,
			StartImport = 0x03,
			EndImport = 0x04,
			Finish = 0x05
		}

		/// <summary>
		/// Gets the total import steps.
		/// </summary>
		/// <returns></returns>
		internal static int GetTotalImportSteps()
		{
			return Enum.GetValues(typeof(ImportSteps)).Length;
		}
		#endregion

		/// <summary>
		/// Occurs when [import export progress message].
		/// </summary>
		public event TaxImportExportProgressMessageHandler ImportExportProgressMessage;

		/// <summary>
		/// Raises the <see cref="E:ImportExportProgressMessage"/> event.
		/// </summary>
		/// <param name="source">The source.</param>
		/// <param name="args">The <see cref="Mediachase.Commerce.Orders.ImportExport.TaxImportExportEventArgs"/> instance containing the event data.</param>
		protected virtual void OnImportExportProgressMessage(object source, TaxImportExportEventArgs args)
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
			OnImportExportProgressMessage(this, new TaxImportExportEventArgs(message, percentage));
		}

		/// <summary>
		/// Called when [event].
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="percentage">The percentage.</param>
		/// <param name="msgType">The message type.</param>
		private void OnEvent(string message, double percentage, TaxImportExportMessageType msgType)
		{
			if (msgType == TaxImportExportMessageType.Warning)
				message = "Warning - " + message;

			OnImportExportProgressMessage(this, new TaxImportExportEventArgs(message, percentage));
		}

		private static double GetProgressPercent(int currentStep, int totalSteps)
		{
			double percentCompleted = 0d;
			if (totalSteps > 0)
				percentCompleted = Convert.ToDouble(currentStep) * 100 / Convert.ToDouble(totalSteps);

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

		#region Import
		/// <summary>
		/// Imports the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="applicationId">The application id.</param>
		/// <param name="baseFilePath">The base file path.</param>
		public void Import(string pathToCsvFile, Guid applicationId, Guid? siteId, char delimiter)
		{
			CultureInfo currentCultureInfo = GetImportExportCulture();

			int totalImportSteps = GetTotalImportSteps();

			OnEvent("Starting import...", 0);

			string fileName = Path.GetFileName(pathToCsvFile);

			// 1. Parse csv file
			OnEvent("Start parsing csv file", GetProgressPercent((int)ImportSteps.StartParseFile, totalImportSteps));

			CsvIncomingDataParser parser = new CsvIncomingDataParser(Path.GetDirectoryName(pathToCsvFile), true, delimiter);

			DataSet taxDataSet = parser.Parse(fileName, null);

			OnEvent("Finished parsing csv file", GetProgressPercent((int)ImportSteps.EndParseFile, totalImportSteps));

			// 2. Create Dtos from parsed DataSet
			if (taxDataSet.Tables[fileName] == null)
				throw new OrdersImportExportException(String.Format("Unknown problem with parsing file {0}.", fileName));

			OnEvent("Start processing parsed rows", GetProgressPercent((int)ImportSteps.StartImport, totalImportSteps));

			int totalRowCount = taxDataSet.Tables[fileName].Rows.Count;

			JurisdictionDto currentJurisdictionDto = JurisdictionManager.GetJurisdictions(Mediachase.Commerce.Orders.Managers.JurisdictionManager.JurisdictionType.Tax);
			TaxDto currentTaxDto = TaxManager.GetTaxDto(TaxType.SalesTax);
			
			JurisdictionDto jurisdictionDto = new JurisdictionDto();
			TaxDto taxDto = new TaxDto();

			for (int i = 0; i <= totalRowCount - 1; i++)
			{
				DataRow currentRow = taxDataSet.Tables[fileName].Rows[i];

				#region Import Jurisdictions
				#region JurisdictionDto
				string jurisdictionCode = currentRow.ItemArray.GetValue(9).ToString();

				JurisdictionDto.JurisdictionRow jRow = null;

				JurisdictionDto.JurisdictionRow[] jRows = (JurisdictionDto.JurisdictionRow[])currentJurisdictionDto.Jurisdiction.Select(String.Format("Code='{0}'", jurisdictionCode));

				// check if row has already been imported
				// (to support multiple values for the same jurisdiction need to check if jurisdiction with the given name already exists in jurisdictionDto)
				JurisdictionDto.JurisdictionRow[] jRows2 = (JurisdictionDto.JurisdictionRow[])jurisdictionDto.Jurisdiction.Select(String.Format("Code='{0}'", jurisdictionCode));
				bool jurisdictionExists = jRows2 != null && jRows2.Length > 0;

				if (jurisdictionExists)
					jRow = jRows2[0];
				else
				{
					if (jRows != null && jRows.Length > 0)
					{
						jurisdictionDto.Jurisdiction.ImportRow(jRows[0]);
						jRow = jurisdictionDto.Jurisdiction[jurisdictionDto.Jurisdiction.Count - 1];
					}
					else
					{
						jRow = jurisdictionDto.Jurisdiction.NewJurisdictionRow();
						jRow.ApplicationId = applicationId;
						jRow.JurisdictionType = Mediachase.Commerce.Orders.Managers.JurisdictionManager.JurisdictionType.Tax.GetHashCode();
					}
				}

				jRow.DisplayName = currentRow.ItemArray.GetValue(0).ToString();
				jRow.StateProvinceCode = GetStringValue(currentRow.ItemArray.GetValue(1));
				jRow.CountryCode = currentRow.ItemArray.GetValue(2).ToString();
				jRow.ZipPostalCodeStart = GetStringValue(currentRow.ItemArray.GetValue(3));
				jRow.ZipPostalCodeEnd = GetStringValue(currentRow.ItemArray.GetValue(4));
				jRow.City = GetStringValue(currentRow.ItemArray.GetValue(5));
				jRow.District = GetStringValue(currentRow.ItemArray.GetValue(6));
				jRow.County = GetStringValue(currentRow.ItemArray.GetValue(7));
				jRow.GeoCode = GetStringValue(currentRow.ItemArray.GetValue(8));
				jRow.Code = jurisdictionCode;

				if (jRow.RowState == DataRowState.Detached)
					jurisdictionDto.Jurisdiction.Rows.Add(jRow);
				#endregion

				#region JurisdictionGroupDto

				string jurisdictionGroupCode = currentRow.ItemArray.GetValue(11).ToString();

				JurisdictionDto.JurisdictionGroupRow jGroupRow = null;

				JurisdictionDto.JurisdictionGroupRow[] jGroupRows = (JurisdictionDto.JurisdictionGroupRow[])currentJurisdictionDto.JurisdictionGroup.Select(String.Format("Code='{0}'", jurisdictionGroupCode));

				// check if row has already been imported
				// (to support multiple values for the same jurisdiction need to check if jurisdiction with the given name already exists in jurisdictionDto)
				JurisdictionDto.JurisdictionGroupRow[] jGroupRows2 = (JurisdictionDto.JurisdictionGroupRow[])jurisdictionDto.JurisdictionGroup.Select(String.Format("Code='{0}'", jurisdictionGroupCode));
				bool jurisdictionGroupExists = jGroupRows2 != null && jGroupRows2.Length > 0;

				if (jurisdictionGroupExists)
					jGroupRow = jGroupRows2[0];
				else
				{
					if (jGroupRows != null && jGroupRows.Length > 0)
					{
						jurisdictionDto.JurisdictionGroup.ImportRow(jGroupRows[0]);
						jGroupRow = jurisdictionDto.JurisdictionGroup[jurisdictionDto.JurisdictionGroup.Count - 1];
					}
					else
					{
						jGroupRow = jurisdictionDto.JurisdictionGroup.NewJurisdictionGroupRow();
						jGroupRow.ApplicationId = applicationId;
						jGroupRow.JurisdictionType = Mediachase.Commerce.Orders.Managers.JurisdictionManager.JurisdictionType.Tax.GetHashCode();
					}
				}

				jGroupRow.DisplayName = currentRow.ItemArray.GetValue(10).ToString();
				jGroupRow.Code = jurisdictionGroupCode;

				if (jGroupRow.RowState == DataRowState.Detached)
					jurisdictionDto.JurisdictionGroup.Rows.Add(jGroupRow);
				#endregion

				#region JurisdictionRelationDto
				JurisdictionDto.JurisdictionRelationRow jRelationRow = null;
				if (jRow.JurisdictionId > 0 && jGroupRow.JurisdictionGroupId > 0)
				{
					// check if relation already exists
					JurisdictionDto.JurisdictionRelationRow[] jRelationRows = (JurisdictionDto.JurisdictionRelationRow[])currentJurisdictionDto.JurisdictionRelation.Select(String.Format("JurisdictionId={0} AND JurisdictionGroupId={1}", jRow.JurisdictionId, jGroupRow.JurisdictionGroupId));
					if (jRelationRows != null && jRelationRows.Length > 0)
					{
						jurisdictionDto.JurisdictionRelation.ImportRow(jRelationRows[0]);
						jRelationRow = jurisdictionDto.JurisdictionRelation[jurisdictionDto.JurisdictionRelation.Count - 1];
					}
				}
				if (jRelationRow == null)
				{
					// create new relation
					jRelationRow = jurisdictionDto.JurisdictionRelation.NewJurisdictionRelationRow();
					jRelationRow.JurisdictionId = jRow.JurisdictionId;
					jRelationRow.JurisdictionGroupId = jGroupRow.JurisdictionGroupId;
					jurisdictionDto.JurisdictionRelation.Rows.Add(jRelationRow);
				}
				#endregion

				// save jurisdictionDto
				if (jurisdictionDto.HasChanges())
					JurisdictionManager.SaveJurisdiction(jurisdictionDto);
				#endregion

				#region Import Taxes
				#region TaxDto
				TaxDto.TaxRow taxRow = null;

				string taxName = currentRow.ItemArray.GetValue(13).ToString();

				// check if row already exists
				TaxDto.TaxRow[] taxRows = (TaxDto.TaxRow[])currentTaxDto.Tax.Select(String.Format("Name='{0}'", taxName));

				// check if row has already been imported
				TaxDto.TaxRow[] taxRows2 = (TaxDto.TaxRow[])taxDto.Tax.Select(String.Format("Name='{0}'", taxName));
				bool taxExists = taxRows2 != null && taxRows2.Length > 0;

				if (taxExists)
					taxRow = taxRows2[0];
				else
				{
					#region if tax is not in the destination dto
					if (taxRows != null && taxRows.Length > 0)
					{
						taxDto.Tax.ImportRow(taxRows[0]);
						taxRow = taxDto.Tax[taxDto.Tax.Count - 1];
					}
					else
					{
						taxRow = taxDto.Tax.NewTaxRow();
						taxRow.ApplicationId = applicationId;
						taxRow.TaxType = TaxType.SalesTax.GetHashCode();
						taxRow.Name = taxName;
					}
					#endregion
				}

				taxRow.SortOrder = Int32.Parse(currentRow.ItemArray.GetValue(14).ToString());

				if (taxRow.RowState == DataRowState.Detached)
					taxDto.Tax.Rows.Add(taxRow);
				#endregion

				#region TaxLanguageDto
				TaxDto.TaxLanguageRow taxLanguageRow = null;

				string langCode = currentRow.ItemArray.GetValue(15).ToString();

				// check if row already exists
				TaxDto.TaxLanguageRow[] taxLanguageRows = (TaxDto.TaxLanguageRow[])currentTaxDto.TaxLanguage.Select(String.Format("LanguageCode='{0}' and TaxId={1}", langCode, taxRow.TaxId));

				// check if row has already been imported
				TaxDto.TaxLanguageRow[] taxLanguageRows2 = (TaxDto.TaxLanguageRow[])taxDto.TaxLanguage.Select(String.Format("LanguageCode='{0}' and TaxId={1}", langCode, taxRow.TaxId));
				bool taxLanguageExists = taxLanguageRows2 != null && taxLanguageRows2.Length > 0;

				string displayName = currentRow.ItemArray.GetValue(12).ToString();

				if (taxLanguageExists)
					taxLanguageRow = taxLanguageRows2[0];
				else
				{
					#region if tax is not in the destination dto
					if (taxLanguageRows != null && taxLanguageRows.Length > 0)
					{
						taxDto.TaxLanguage.ImportRow(taxLanguageRows[0]);
						taxLanguageRow = taxDto.TaxLanguage[taxDto.TaxLanguage.Count - 1];
					}
					else
					{
						taxLanguageRow = taxDto.TaxLanguage.NewTaxLanguageRow();
						taxLanguageRow.LanguageCode = langCode;
						taxLanguageRow.TaxId = taxRow.TaxId;
					}
					#endregion
				}

				taxLanguageRow.DisplayName = displayName;

				if (taxLanguageRow.RowState == DataRowState.Detached)
					taxDto.TaxLanguage.Rows.Add(taxLanguageRow);
				#endregion

				#region TaxValueDto
				TaxDto.TaxValueRow taxValueRow = null;

				// check if row already exists
				TaxDto.TaxValueRow[] taxValueRows = null;
				if (siteId.HasValue)
                    taxValueRows = (TaxDto.TaxValueRow[])currentTaxDto.TaxValue.Select(String.Format("TaxId={0} and SiteId={1} and JurisdictionGroupId={2}", taxRow.TaxId, siteId.Value, jRelationRow.JurisdictionGroupId));
				else
                    taxValueRows = (TaxDto.TaxValueRow[])currentTaxDto.TaxValue.Select(String.Format("TaxId={0} and JurisdictionGroupId={1}", taxRow.TaxId, jRelationRow.JurisdictionGroupId));

				// check if row has already been imported
				TaxDto.TaxValueRow[] taxValueRows2 = null;
				if (siteId.HasValue)
                    taxValueRows2 = (TaxDto.TaxValueRow[])taxDto.TaxValue.Select(String.Format("TaxId={0} and SiteId={1} and JurisdictionGroupId={2}", taxRow.TaxId, siteId.Value, jRelationRow.JurisdictionGroupId));
				else
                    taxValueRows2 = (TaxDto.TaxValueRow[])taxDto.TaxValue.Select(String.Format("TaxId={0} and JurisdictionGroupId={1}", taxRow.TaxId, jRelationRow.JurisdictionGroupId));

				bool taxValueExists = taxValueRows2 != null && taxValueRows2.Length > 0;

				if (taxValueExists)
					taxValueRow = taxValueRows2[0];
				else
				{
					if (taxValueRows != null && taxValueRows.Length > 0)
					{
						taxDto.TaxValue.ImportRow(taxValueRows[0]);
						taxValueRow = taxDto.TaxValue[taxDto.TaxValue.Count - 1];
					}
					else
					{
						taxValueRow = taxDto.TaxValue.NewTaxValueRow();
						taxValueRow.TaxId = taxRow.TaxId;
					}
				}

				// assign tax values
				taxValueRow.JurisdictionGroupId = jGroupRow.JurisdictionGroupId;
				taxValueRow.TaxCategory = currentRow.ItemArray.GetValue(16).ToString();
				taxValueRow.Percentage = float.Parse(currentRow.ItemArray.GetValue(17).ToString(), currentCultureInfo);
				taxValueRow.AffectiveDate = DateTime.Parse(currentRow.ItemArray.GetValue(18).ToString(), currentCultureInfo);
				if (siteId.HasValue)
					taxValueRow.SiteId = siteId.Value;

				// add row to dto, if needed
				if (taxValueRow.RowState == DataRowState.Detached)
					taxDto.TaxValue.Rows.Add(taxValueRow);

				// create tax category, if it doesn't exist yet
				CatalogTaxManager.CreateTaxCategory(taxValueRow.TaxCategory, true);
				#endregion

				if (taxDto.HasChanges())
					TaxManager.SaveTax(taxDto);
				#endregion

				if ((i + 1) % 20 == 0)
					OnEvent(String.Format("Processed {0} of {1} rows", i + 1, totalRowCount), GetProgressPercent((int)ImportSteps.StartImport, totalImportSteps));
			}

			OnEvent(String.Format("Finished processing parsed rows. Total processed: {0}", totalRowCount), GetProgressPercent((int)ImportSteps.Finish, totalImportSteps));

			OnEvent("CSV file successfully imported.", 100);
		}

		/// <summary>
		/// Returns null if value equals to string "NULL", otherwise returns actual value.
		/// </summary>
		/// <param name="dsValue">Value from the DataSet; parsed by parser.</param>
		/// <remarks>Need to call thid function for string values that allow nulls.</remarks>
		private static string GetStringValue(object dsValue)
		{
			if (dsValue == null)
				return null;

			if (String.Compare(dsValue.ToString(), "NULL", StringComparison.OrdinalIgnoreCase) == 0)
				return null;

			return dsValue.ToString();
		}
		#endregion
	}
}