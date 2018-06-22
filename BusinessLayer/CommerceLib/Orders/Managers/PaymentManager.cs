using System;
using System.Data;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders.Managers
{
    /// <summary>
    /// Payment manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class PaymentManager
	{
		#region Get Payment Functions
        /// <summary>
        /// Gets the payment methods.
        /// </summary>
        /// <param name="languageid">The languageid.</param>
        /// <param name="returnInactive">if set to <c>true</c> [return inactive].</param>
        /// <returns></returns>
		public static PaymentMethodDto GetPaymentMethods(string languageid, bool returnInactive)
		{
            // Assign new cache key, specific for site guid and response groups requested
            string cacheKey = OrderCache.CreateCacheKey("payment-methods", languageid, returnInactive.ToString());

            PaymentMethodDto dto = null;

            // check cache first
            object cachedObject = OrderCache.Get(cacheKey);

            if (cachedObject != null)
                dto = (PaymentMethodDto)cachedObject;

            // Load the object
            if (dto == null)
            {
                DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
                cmd.CommandText = "[ecf_PaymentMethod_Language]";
                cmd.Parameters = new DataParameters();
                cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
                cmd.Parameters.Add(new DataParameter("LanguageId", languageid, DataParameterType.NVarChar, 128));
                cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
                cmd.DataSet = new PaymentMethodDto();
                cmd.TableMapping = DataHelper.MapTables("PaymentMethod", "PaymentMethodParameter", "ShippingPaymentRestriction");

                DataResult results = DataService.LoadDataSet(cmd);

                dto = (PaymentMethodDto)results.DataSet;

                // Insert to the cache collection
                OrderCache.Insert(cacheKey, dto, OrderConfiguration.Instance.Cache.PaymentCollectionTimeout);
            }

			return dto;
		}

        /// <summary>
        /// Returns list of active payment methods.
        /// </summary>
        /// <param name="languageid">The languageid.</param>
        /// <returns></returns>
		public static PaymentMethodDto GetPaymentMethods(string languageid)
		{
			return GetPaymentMethods(languageid, false);
		}

        /// <summary>
        /// Returns payment method by id.
        /// </summary>
        /// <param name="paymentMethodId">PaymentMethodId</param>
        /// <param name="returnInactive">if set to <c>true</c> [return inactive].</param>
        /// <returns></returns>
		public static PaymentMethodDto GetPaymentMethod(Guid paymentMethodId, bool returnInactive)
		{
			//TODO: cache results
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_PaymentMethod_PaymentMethodId]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("PaymentMethodId", paymentMethodId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
			cmd.DataSet = new PaymentMethodDto();
			cmd.TableMapping = DataHelper.MapTables("PaymentMethod", "PaymentMethodParameter", "ShippingPaymentRestriction");

			DataResult results = DataService.LoadDataSet(cmd);

			return (PaymentMethodDto)results.DataSet;
		}

        /// <summary>
        /// Returns active payment method by id.
        /// </summary>
        /// <param name="paymentMethodId">The payment method id.</param>
        /// <returns></returns>
		public static PaymentMethodDto GetPaymentMethod(Guid paymentMethodId)
		{
			return GetPaymentMethod(paymentMethodId, false);
		}

		/// <summary>
		/// Returns payment method by system name.
		/// </summary>
		/// <param name="name">PaymentMethod SystemKeyword</param>
		/// <param name="languageid">The languageid.</param>
		/// <param name="returnInactive">if set to <c>true</c> [return inactive].</param>
		/// <returns></returns>
		public static PaymentMethodDto GetPaymentMethodBySystemName(string name, string languageid, bool returnInactive)
		{
			//TODO: cache results
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = "[ecf_PaymentMethod_SystemKeyword]";
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("SystemKeyword", name, DataParameterType.NVarChar, 30));
			cmd.Parameters.Add(new DataParameter("LanguageId", languageid, DataParameterType.NVarChar, 128));
			cmd.Parameters.Add(new DataParameter("ReturnInactive", returnInactive, DataParameterType.Bit));
			cmd.DataSet = new PaymentMethodDto();
			cmd.TableMapping = DataHelper.MapTables("PaymentMethod", "PaymentMethodParameter", "ShippingPaymentRestriction");

			DataResult results = DataService.LoadDataSet(cmd);

			return (PaymentMethodDto)results.DataSet;
		}

		/// <summary>
		/// Returns active payment method by system name.
		/// </summary>
		/// <param name="paymentMethodId">The payment method system name.</param>
		/// <param name="languageid">The languageid.</param>
		/// <returns></returns>
		public static PaymentMethodDto GetPaymentMethodBySystemName(string name, string languageid)
		{
			return GetPaymentMethodBySystemName(name, languageid, false);
		}
        #endregion

        #region Edit Payment Functions
        /// <summary>
        /// Saves the payment.
        /// </summary>
        /// <param name="dto">The dto.</param>
        public static void SavePayment(PaymentMethodDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("PaymentMethodDto can not be null"));

			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(OrderContext.MetaDataContext, cmd, dto, "PaymentMethod", "PaymentMethodParameter", "ShippingPaymentRestriction");
				scope.Complete();
			}
        }
        #endregion
    }
}
