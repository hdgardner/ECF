using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Marketing
{
    /// <summary>
    /// Promotion Filer. Defines filtering options that will be applied when promotion engine executes.
    /// Use this configuration file to control which parameters are checked.
    /// </summary>
    public sealed class PromotionFilter
    {
        private bool _IgnorePolicy = true;
        private bool _IgnoreSegment = true;
        private bool _IgnoreConditions = true;
        private bool _IncludeCoupons = true;
        private bool _IncludeInactive;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionFilter"/> class.
        /// </summary>
        public PromotionFilter()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore policy].
        /// </summary>
        /// <value><c>true</c> if [ignore policy]; otherwise, <c>false</c>.</value>
        public bool IgnorePolicy 
        {
            get
            {
                return _IgnorePolicy;
            }
            set
            {
                _IgnorePolicy = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore segments].
        /// </summary>
        /// <value><c>true</c> if [ignore segments]; otherwise, <c>false</c>.</value>
        public bool IgnoreSegments
        {
            get
            {
                return _IgnoreSegment;
            }
            set
            {
                _IgnoreSegment = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore conditions].
        /// </summary>
        /// <value><c>true</c> if [ignore conditions]; otherwise, <c>false</c>.</value>
        public bool IgnoreConditions 
        {
            get
            {
                return _IgnoreConditions;
            }
            set
            {
                _IgnoreConditions = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include coupons].
        /// </summary>
        /// <value><c>true</c> if [include coupons]; otherwise, <c>false</c>.</value>
        public bool IncludeCoupons
        {
            get
            {
                return _IncludeCoupons;
            }
            set
            {
                _IncludeCoupons = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [include inactive].
        /// </summary>
        /// <value><c>true</c> if [include inactive]; otherwise, <c>false</c>.</value>
        public bool IncludeInactive 
        {
            get
            {
                return _IncludeInactive;
            }
            set
            {
                _IncludeInactive = value;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            PromotionFilter filter = (PromotionFilter)obj;

            if(filter.IgnoreConditions == this.IgnoreConditions && filter.IgnorePolicy == this.IgnorePolicy
                && filter.IgnoreSegments == this.IgnoreSegments && filter.IncludeCoupons == this.IncludeCoupons
                && filter.IncludeInactive == this.IncludeInactive)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <remarks>
        /// Compiler complains that there is no override of the GetHashCode() method. 
        /// Maybe a separate one can be implemented in the future? 
        /// For now, just the default definition .NET provides.
        /// </remarks>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
