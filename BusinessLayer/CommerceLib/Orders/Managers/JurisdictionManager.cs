using System;
using System.Data;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders.Managers
{
	/// <summary>
	/// Jurisdiction manager acts as proxy between methods that call data layer functions and the facade layer.
	/// The methods here check if the appropriate security is set and that the data is cached.
	/// </summary>
	public static class JurisdictionManager
	{
		/// <summary>
		/// JurisdictionType enumeration.
		/// </summary>
		public enum JurisdictionType
		{
			/// <summary>
			/// 
			/// </summary>
			Tax = 1,
			/// <summary>
			/// 
			/// </summary>
			Shipping = 2
		}

		#region Get Jurisdiction Functions
		/// <summary>
		/// Gets the jurisdictions and jurisdiction groups for the specified jurisdiction type.
		/// </summary>
		/// <param name="jurisdictionType">JurisdictionType. 1 - tax, 2 - shipping. null - all jurisdictions.</param>
		/// <returns></returns>
		public static JurisdictionDto GetJurisdictions(JurisdictionType? jurisdictionType)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = OrderCache.CreateCacheKey("jurisdictions", jurisdictionType.HasValue ? jurisdictionType.Value.ToString() : String.Empty);

			JurisdictionDto dto = null;

			// check cache first
			object cachedObject = OrderCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (JurisdictionDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
				cmd.CommandText = "[ecf_Jurisdiction]";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				if (jurisdictionType.HasValue)
					cmd.Parameters.Add(new DataParameter("JurisdictionType", jurisdictionType.Value.GetHashCode(), DataParameterType.Int));
				cmd.DataSet = new JurisdictionDto();
				cmd.TableMapping = DataHelper.MapTables("Jurisdiction", "JurisdictionGroup", "JurisdictionRelation");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (JurisdictionDto)results.DataSet;

				// Insert to the cache collection
				OrderCache.Insert(cacheKey, dto, OrderConfiguration.Instance.Cache.PaymentCollectionTimeout);
			}

			return dto;
		}

		/// <summary>
		/// Returns jurisdiction groups.
		/// </summary>
		/// <param name="jurisdictionType">JurisdictionType. 1 - tax, 2 - shipping. null - all jurisdictions.</param>
		/// <returns></returns>
		public static JurisdictionDto GetJurisdictionGroups(JurisdictionType? jurisdictionType)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = OrderCache.CreateCacheKey("jurisdiction-groups", jurisdictionType.HasValue ? jurisdictionType.Value.ToString() : String.Empty);

			JurisdictionDto dto = null;

			// check cache first
			object cachedObject = OrderCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (JurisdictionDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
				cmd.CommandText = "[ecf_Jurisdiction_JurisdictionGroups]";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				if (jurisdictionType.HasValue)
					cmd.Parameters.Add(new DataParameter("JurisdictionType", jurisdictionType.Value.GetHashCode(), DataParameterType.Int));
				cmd.DataSet = new JurisdictionDto();
				cmd.TableMapping = DataHelper.MapTables("JurisdictionGroup");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (JurisdictionDto)results.DataSet;
			}

			return dto;
		}

		/// <summary>
		/// Returns JurisdictionGroup by id.
		/// </summary>
		/// <param name="jurisdictionGroupId">The JurisdictionGroupId.</param>
		/// <returns></returns>
		public static JurisdictionDto GetJurisdictionGroup(int jurisdictionGroupId)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = OrderCache.CreateCacheKey("jurisdiction-group", jurisdictionGroupId.ToString());

			JurisdictionDto dto = null;

			// check cache first
			object cachedObject = OrderCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (JurisdictionDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
				cmd.CommandText = "[ecf_Jurisdiction_JurisdictionGroupId]";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.Parameters.Add(new DataParameter("JurisdictionGroupId", jurisdictionGroupId, DataParameterType.Int));
				cmd.DataSet = new JurisdictionDto();
                cmd.TableMapping = DataHelper.MapTables("JurisdictionGroup", "JurisdictionRelation");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (JurisdictionDto)results.DataSet;
			}

			return dto;
		}

		/// <summary>
		/// Returns JurisdictionGroup by code.
		/// </summary>
		/// <param name="code">The JurisdictionGroup Code.</param>
		/// <returns></returns>
		public static JurisdictionDto GetJurisdictionGroup(string code)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = OrderCache.CreateCacheKey("jurisdiction-group-code", code);

			JurisdictionDto dto = null;

			// check cache first
			object cachedObject = OrderCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (JurisdictionDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
				cmd.CommandText = "[ecf_Jurisdiction_JurisdictionGroupCode]";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.Parameters.Add(new DataParameter("JurisdictionGroupCode", code, DataParameterType.NVarChar, 50));
				cmd.DataSet = new JurisdictionDto();
				cmd.TableMapping = DataHelper.MapTables("JurisdictionGroup", "JurisdictionRelation");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (JurisdictionDto)results.DataSet;
			}

			return dto;
		}

		/// <summary>
		/// Returns Jurisdiction by id.
		/// </summary>
		/// <param name="jurisdictionId">The jurisdictionId.</param>
		/// <param name="returnAllGroups">True, to return all jurisdiction groups.</param>
		/// <returns></returns>
		public static JurisdictionDto GetJurisdiction(int jurisdictionId, bool returnAllGroups)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = OrderCache.CreateCacheKey("jurisdiction", jurisdictionId.ToString(), returnAllGroups.ToString());

			JurisdictionDto dto = null;

			// check cache first
			object cachedObject = OrderCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (JurisdictionDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
				cmd.CommandText = "[ecf_Jurisdiction_JurisdictionId]";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.Parameters.Add(new DataParameter("JurisdictionId", jurisdictionId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("ReturnAllGroups", returnAllGroups, DataParameterType.Bit));
				cmd.DataSet = new JurisdictionDto();
				cmd.TableMapping = DataHelper.MapTables("Jurisdiction", "JurisdictionGroup", "JurisdictionRelation");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (JurisdictionDto)results.DataSet;
			}

			return dto;
		}

		/// <summary>
		/// Calls <see cref="GetJurisdiction(int, bool)"/> with returnAllGroups = false
		/// </summary>
		/// <param name="jurisdictionId">The jurisdictionId.</param>
		/// <returns></returns>
		public static JurisdictionDto GetJurisdiction(int jurisdictionId)
		{
			return GetJurisdiction(jurisdictionId, false);
		}

		/// <summary>
		/// Returns Jurisdiction by code.
		/// </summary>
		/// <param name="code">The jurisdiction code.</param>
		/// <param name="returnAllGroups">True, to return all jurisdiction groups.</param>
		/// <returns></returns>
		public static JurisdictionDto GetJurisdiction(string code, bool returnAllGroups)
		{
			// Assign new cache key, specific for site guid and response groups requested
			string cacheKey = OrderCache.CreateCacheKey("jurisdiction-code", code, returnAllGroups.ToString());

			JurisdictionDto dto = null;

			// check cache first
			object cachedObject = OrderCache.Get(cacheKey);

			if (cachedObject != null)
				dto = (JurisdictionDto)cachedObject;

			// Load the object
			if (dto == null)
			{
				DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
				cmd.CommandText = "[ecf_Jurisdiction_JurisdictionCode]";
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ApplicationId", OrderConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
				cmd.Parameters.Add(new DataParameter("JurisdictionCode", code, DataParameterType.NVarChar, 50));
				cmd.Parameters.Add(new DataParameter("ReturnAllGroups", returnAllGroups, DataParameterType.Bit));
				cmd.DataSet = new JurisdictionDto();
				cmd.TableMapping = DataHelper.MapTables("Jurisdiction", "JurisdictionGroup", "JurisdictionRelation");

				DataResult results = DataService.LoadDataSet(cmd);

				dto = (JurisdictionDto)results.DataSet;
			}

			return dto;
		}

		/// <summary>
		/// Calls <see cref="GetJurisdiction(string, bool)"/> with returnAllGroups = false
		/// </summary>
		/// <param name="code">The jurisdiction code.</param>
		/// <returns></returns>
		public static JurisdictionDto GetJurisdiction(string code)
		{
			return GetJurisdiction(code, false);
		}
		#endregion

		#region Edit Jurisdiction Functions
		/// <summary>
		/// Saves the jurisdiction.
		/// </summary>
		/// <param name="dto">The dto.</param>
		public static void SaveJurisdiction(JurisdictionDto dto)
		{
			if (dto == null)
				throw new ArgumentNullException("dto", String.Format("JurisdictionDto can not be null"));

			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();

			using (TransactionScope scope = new TransactionScope())
			{
				DataHelper.SaveDataSetSimple(OrderContext.MetaDataContext, cmd, dto, "Jurisdiction", "JurisdictionGroup", "JurisdictionRelation");
				scope.Complete();
			}
		}
		#endregion
	}
}