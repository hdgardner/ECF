using System;
using System.Data;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Commerce.Orders.Managers
{
    /// <summary>
    /// OrderStatus manager acts as proxy between methods that call data layer functions and the facade layer.
    /// The methods here check if the appropriate security is set and that the data is cached.
    /// </summary>
    public static class OrderStatusManager
    {
        #region OrderStatus Functions
        /// <summary>
        /// Gets the order status.
        /// </summary>
        /// <returns></returns>
		public static OrderStatusDto GetOrderStatus()
		{
			//TODO: cache results
			DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();
			cmd.CommandText = String.Format("select * from OrderStatus where ApplicationId = '{0}';", OrderConfiguration.Instance.ApplicationId);
			cmd.CommandType = CommandType.Text;
			cmd.Parameters = new DataParameters();
			cmd.DataSet = new OrderStatusDto();
			cmd.TableMapping = DataHelper.MapTables("OrderStatus");

			DataResult results = DataService.LoadDataSet(cmd);

			return (OrderStatusDto)results.DataSet;
		}
		#endregion

		#region Edit OrderStatus Functions
        /// <summary>
        /// Saves changes in OrderStatusDto. Not implemented!
        /// </summary>
        /// <param name="dto">The dto.</param>
		public static void SaveOrderStatus(OrderStatusDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException("dto", String.Format("OrderStatusDto can not be null"));

			// TODO: implement

			//// TODO: Check if user is allowed to perform this operation
			//DataCommand cmd = OrderDataHelper.CreateConfigDataCommand();

			//using (TransactionScope scope = new TransactionScope())
			//{
			//    DataHelper.SaveDataSetSimple(cmd, dto, "OrderStatus");
			//    scope.Complete();
			//}
        }
        #endregion
    }
}
