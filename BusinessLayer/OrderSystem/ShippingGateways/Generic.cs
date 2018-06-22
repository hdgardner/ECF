using System;
using System.Text;
using System.Net;
using System.Collections;
using System.Globalization;
using Mediachase.Commerce.Orders;
using System.Threading;
using Mediachase.Commerce.Orders.Dto;
using Mediachase.Commerce.Orders.Managers;

namespace Mediachase.Commerce.Plugins.Shipping.Generic
{
	/// <summary>
	/// Summary description for Generic.
	/// </summary>
	public class GenericGateway : IShippingGateway
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericGateway"/> class.
        /// </summary>
		public GenericGateway()
		{
		}

		#region IShippingGateway Members

        /// <summary>
        /// Returns the package option array when method id and package that needs to be send is passed.
        /// Use passed message string to pass errors back to the application if any occured.
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="items">The items.</param>
        /// <param name="message">The message.</param>
        /// <returns>empty array if no results found</returns>
        public ShippingRate GetRate(Guid methodId, LineItem[] items, ref string message)
		{
            if (items == null || items.Length == 0)
                return null;

            ShippingMethodDto methods = ShippingManager.GetShippingMethods(Thread.CurrentThread.CurrentCulture.Name);

            ShippingMethodDto.ShippingMethodRow row = methods.ShippingMethod.FindByShippingMethodId(methodId);
            if (row != null)
            {
                return new ShippingRate(methodId, row.DisplayName, row.BasePrice, row.Currency);
            }
			else
			{
                message = "The shipping method could not be loaded.";
				return null;
			}
		}

		#endregion
    }
}
