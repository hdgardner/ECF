using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Implements operations for the promotion entries set.
    /// </summary>
    public class PromotionEntriesSet
    {
        private IList<PromotionEntry> _Entries = new List<PromotionEntry>();

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public IList<PromotionEntry> Entries
        {
            get { return _Entries; }
            set { _Entries = value; }
        }

        private string _OwnerId = String.Empty;

        /// <summary>
        /// Gets or sets the owner id. This can be used to store the reference to the object that list belongs to. Like shipment.
        /// </summary>
        /// <value>The owner id.</value>
        public string OwnerId
        {
            get { return _OwnerId; }
            set { _OwnerId = value; }
        }

        /// <summary>
        /// Finds the entry by code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public PromotionEntry FindEntryByCode(string code)
        {
            foreach (PromotionEntry entry in Entries)
            {
                if (entry.CatalogEntryCode == code)
                    return entry;
            }
            return null;
        }

        /// <summary>
        /// Finds the entry by node code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public PromotionEntry FindEntryByNodeCode(string code)
        {
            foreach (PromotionEntry entry in Entries)
            {
                if (entry.CatalogEntryCode == code)
                    return entry;
            }
            return null;
        }

        /// <summary>
        /// Determines whether the specified code contains entry.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        /// 	<c>true</c> if the specified code contains entry; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsEntry(string code)
        {
            if (FindEntryByCode(code) != null)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the specified code contains entry with a specified qauntity or more.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>
        /// 	<c>true</c> if the specified code contains entry; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsEntry(string code, decimal quantity)
        {
            PromotionEntry entry = FindEntryByCode(code);
            if (entry != null && entry.Quantity >= quantity)
                return true;

            return false;
        }

        /// <summary>
        /// Determines whether the specified code contains node.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns>
        /// 	<c>true</c> if the specified code contains node; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsNode(string code)
        {
            if (FindEntryByNodeCode(code) != null)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the total cost.
        /// </summary>
        /// <value>The total cost.</value>
        public decimal TotalCost
        {
            get
            {
                decimal runningTotal = 0;
                foreach (PromotionEntry entry in Entries)
                {
                    runningTotal += entry.CostPerEntry * entry.Quantity;
                }

                return runningTotal;
            }
        }



        /// <summary>
        /// Gets the total quantity.
        /// </summary>
        /// <value>The total quantity.</value>
        public decimal TotalQuantity
        {
            get
            {
                decimal runningTotal = 0;
                foreach (PromotionEntry entry in Entries)
                {
                    runningTotal += entry.Quantity;
                }

                return runningTotal;
            }
        }

        /// <summary>
        /// Gets the quantity range.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public PromotionEntriesSet GetQuantityRange(decimal quantity)
        {
            decimal totalQuantity = 0;
            PromotionEntriesSet retVal = new PromotionEntriesSet();
            foreach (PromotionEntry entry in Entries)
            {
                if (totalQuantity == quantity)
                    break;

                PromotionEntry newEntry = (PromotionEntry)entry.Clone();
                totalQuantity += entry.Quantity;
                if(totalQuantity > quantity)
                {
                    newEntry.Quantity = quantity - (totalQuantity - entry.Quantity);
                    totalQuantity = quantity;
                }

                if(newEntry.Quantity != 0)
                    retVal.Entries.Add(newEntry);
            }

            // Copy owner id
            retVal.OwnerId = this.OwnerId;

            return retVal;
        }

        /// <summary>
        /// Creates a copy of current entries Set with specified items only.
        /// </summary>
        /// <param name="codes">The codes.</param>
        /// <returns></returns>
        public PromotionEntriesSet MakeCopy(string[] codes)
        {
            return MakeCopy(codes, false);
        }

        /// <summary>
        /// Makes the copy.
        /// </summary>
        /// <param name="codes">The codes.</param>
        /// <param name="exclude">if set to <c>true</c> [exclude].</param>
        /// <returns></returns>
        public PromotionEntriesSet MakeCopy(string[] codes, bool exclude)
        {
            PromotionEntriesSet newSet = new PromotionEntriesSet();
            Func<PromotionEntry, PromotionEntriesSet> addEntry = x => { newSet.Entries.Add((PromotionEntry)x.Clone()); return newSet; };
            Entries.Where(x => { bool found = codes.Contains(x.CatalogEntryCode); return exclude ? !found : found; }).Select(x => addEntry(x)).ToArray();
            //foreach (string code in codes)
            //{
            //    PromotionEntry entry = this.FindEntryByCode(code);
            //    if (entry != null)
            //    {
            //        newSet.Entries.Add(entry);
            //    }
            //}

            // Copy owner id
            newSet.OwnerId = this.OwnerId;

            return newSet;
        }
    }
}