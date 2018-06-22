using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Catalog.Objects
{
    /// <summary>
    /// Represents the catalog inventory objects.
    /// </summary>
    [DataContract]
    public partial class Inventory
    {
        /// <summary>
        /// Represents the inventory's in-stock quantity.
        /// </summary>
        private decimal _InStockQuantity;

        public decimal InStockQuantity
        {
            get { return _InStockQuantity; }
            set { _InStockQuantity = value; }
        }
        /// <summary>
        /// Represents the inventory's reserved quantity.
        /// </summary>
        private decimal _ReservedQuantity;

        public decimal ReservedQuantity
        {
            get { return _ReservedQuantity; }
            set { _ReservedQuantity = value; }
        }
        /// <summary>
        /// Represents the inventory's minimum re-order quantity.
        /// </summary>
        private decimal _ReorderMinQuantity;

        public decimal ReorderMinQuantity
        {
            get { return _ReorderMinQuantity; }
            set { _ReorderMinQuantity = value; }
        }
        /// <summary>
        /// Represents the inventory's pre-order quantity.
        /// </summary>
        private decimal _PreorderQuantity;

        public decimal PreorderQuantity
        {
            get { return _PreorderQuantity; }
            set { _PreorderQuantity = value; }
        }
        /// <summary>
        /// Represents the inventory's backorder quantity.
        /// </summary>
        private decimal _BackorderQuantity;

        public decimal BackorderQuantity
        {
            get { return _BackorderQuantity; }
            set { _BackorderQuantity = value; }
        }
        /// <summary>
        /// Represents whether to allow backorders.
        /// </summary>
        private bool _AllowBackorder;

        public bool AllowBackorder
        {
            get { return _AllowBackorder; }
            set { _AllowBackorder = value; }
        }
        /// <summary>
        /// Represents whether to allow pre-orders.
        /// </summary>
        private bool _AllowPreorder;

        public bool AllowPreorder
        {
            get { return _AllowPreorder; }
            set { _AllowPreorder = value; }
        }
        /// <summary>
        /// Represents the inventory's status.
        /// </summary>
        private string _InventoryStatus;

        public string InventoryStatus
        {
            get { return _InventoryStatus; }
            set { _InventoryStatus = value; }
        }
        /// <summary>
        /// Represents the availability date for the inventory's pre-orders.
        /// </summary>
        private DateTime _PreorderAvailabilityDate;

        public DateTime PreorderAvailabilityDate
        {
            get { return _PreorderAvailabilityDate; }
            set { _PreorderAvailabilityDate = value; }
        }
        /// <summary>
        /// Represents the availability date for the inventory's backorders.
        /// </summary>
        private DateTime _BackorderAvailabilityDate;

        public DateTime BackorderAvailabilityDate
        {
            get { return _BackorderAvailabilityDate; }
            set { _BackorderAvailabilityDate = value; }
        }
    }
}
