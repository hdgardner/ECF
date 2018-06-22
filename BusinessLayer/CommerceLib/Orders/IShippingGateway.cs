using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Orders;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Provdies the methods necessary for the shipping gateway.
    /// </summary>
    public interface IShippingGateway
    {
        /// <summary>
        /// Returns the package option array when method id and package that needs to be send is passed.
        /// Use passed message string to pass errors back to the application if any occured.
        /// </summary>
        /// <param name="methodId">The method id.</param>
        /// <param name="items">The items.</param>
        /// <param name="message">The message.</param>
        /// <returns>empty array if no results found</returns>
        ShippingRate GetRate(Guid methodId, LineItem[] items, ref string message);
    }

    /// <summary>
    /// Contains information about shipping rate
    /// </summary>
    [Serializable]
    public class ShippingRate
    {
        /// <summary>
        /// Represents the shipping rate ID.
        /// </summary>
        public Guid Id;
        /// <summary>
        /// Represents the shipping rate price.
        /// </summary>
        public decimal Price;
        /// <summary>
        /// Represents the shipping rate name.
        /// </summary>
        public string Name;
        /// <summary>
        /// Represents the shipping rate's currency code.
        /// </summary>
        public string CurrencyCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingRate"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="price">The price.</param>
        /// <param name="currencyCode">The currency code.</param>
        public ShippingRate(Guid id, string name, decimal price, string currencyCode)
        {
            Id = id;
            Price = price;
            Name = name;
            CurrencyCode = currencyCode;
        }
    }
}
