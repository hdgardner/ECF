using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog.Exceptions;

namespace Mediachase.Commerce.Catalog.Data
{
    /// <summary>
    /// Implements administrative functions for the currency
    /// </summary>
    public class CurrencyAdmin
    {
        private CurrencyDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public CurrencyDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyAdmin"/> class.
        /// </summary>
        internal CurrencyAdmin()
        {
            _DataSet = new CurrencyDto();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal CurrencyAdmin(CurrencyDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Loads the currencies.
        /// </summary>
        internal void Load()
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();
            cmd.CommandText = "ecf_Currency";
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.DataSet = CurrentDto;

            cmd.TableMapping = DataHelper.MapTables("Currency", "CurrencyRate");

            DataService.LoadDataSet(cmd);
        }

		/// <summary>
		/// Loads the currency by specified currency id.
		/// </summary>
		internal void LoadByCurrencyId(int currencyId)
		{
			DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = "ecf_Currency_CurrencyId";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("CurrencyId", currencyId, DataParameterType.Int));
			cmd.DataSet = CurrentDto;

			cmd.TableMapping = DataHelper.MapTables("Currency", "CurrencyRate");

			CurrentDto.EnforceConstraints = false;
			DataService.LoadDataSet(cmd);
		}

		/// <summary>
		/// Loads the currency by specified currency code.
		/// </summary>
		internal void LoadByCurrencyCode(string currencyCode)
		{
			DataCommand cmd = CatalogDataHelper.CreateDataCommand();
			cmd.CommandText = "ecf_Currency_Code";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CatalogConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("CurrencyCode", currencyCode, DataParameterType.NChar, 3));
			cmd.DataSet = CurrentDto;

			cmd.TableMapping = DataHelper.MapTables("Currency", "CurrencyRate");

			CurrentDto.EnforceConstraints = false;
			DataService.LoadDataSet(cmd);
		}

        /// <summary>
        /// Updates the currency.
        /// </summary>
        internal void Save()
        {
            DataCommand cmd = CatalogDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
                DataHelper.SaveDataSetSimple(CatalogContext.MetaDataContext, cmd, CurrentDto, "Currency", "CurrencyRate");
                scope.Complete();
            }
        }
    }
}