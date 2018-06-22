using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Mediachase.Commerce.Catalog.Dto;

namespace Mediachase.Commerce.Catalog.Security
{
    /// <summary>
    /// Implements operations for and represents the catalog permission record set.
    /// </summary>
    public class PermissionRecordSet
    {
        private Dictionary<string, PermissionRecord> Records;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionRecordSet"/> class.
        /// </summary>
        public PermissionRecordSet()
        {
            Records = new Dictionary<string, PermissionRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionRecordSet"/> class.
        /// </summary>
        /// <param name="records">The records.</param>
        public PermissionRecordSet(Dictionary<string, PermissionRecord> records)
        {
            Records = records;
        }

        /// <summary>
        /// Gets the <see cref="Mediachase.Commerce.Catalog.Security.PermissionRecord"/> with the specified key.
        /// </summary>
        /// <value></value>
        public PermissionRecord this[string key]
        {
            get
            {
                if (Records.ContainsKey(key))
                    return Records[key];
                else
                    return null;
            }
        }
    }

    /// <summary>
    /// Implements operations for and represents the permission record.
    /// </summary>
    public class PermissionRecord
    {
        #region Private Members
        string _sid;
        string _scope;
        Permission _allowMask;
        Permission _denyMask;
        #endregion

        #region Public Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionRecord"/> class.
        /// </summary>
        public PermissionRecord()
        {
            _allowMask = 0;
            _denyMask = (Permission)(long)-1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionRecord"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="sid">The sid.</param>
        /// <param name="allow">The allow.</param>
        /// <param name="deny">The deny.</param>
        public PermissionRecord(string scope, string sid, Permission allow, Permission deny)
        {
            _scope = scope;
            _sid = sid;
            _allowMask = allow;
            _denyMask = deny;
        }
        #endregion

        #region Core Public Properties
        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public string Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }

        /// <summary>
        /// Gets or sets the sid.
        /// </summary>
        /// <value>The sid.</value>
        public string Sid
        {
            get { return _sid; }
            set { _sid = value; }
        }

        /// <summary>
        /// Gets or sets the allow mask.
        /// </summary>
        /// <value>The allow mask.</value>
        public Permission AllowMask
        {
            get { return _allowMask; }
            set { _allowMask = value; }
        }

        /// <summary>
        /// Gets or sets the deny mask.
        /// </summary>
        /// <value>The deny mask.</value>
        public Permission DenyMask
        {
            get { return _denyMask; }
            set { _denyMask = value; }
        }

        #endregion

        #region GetBin
        /// <summary>
        /// Gets the bit.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <returns></returns>
        public bool GetBit(Permission mask)
        {
            bool bReturn = false;

            if ((_denyMask & mask) == mask)
                bReturn = false;

            if ((_allowMask & mask) == mask)
                bReturn = true;

            return bReturn;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the bit.
        /// </summary>
        /// <param name="mask">The mask.</param>
        /// <param name="accessControl">The access control.</param>
        public void SetBit(Permission mask, AccessControlEntry accessControl)
        {
            switch (accessControl)
            {
                case AccessControlEntry.Allow:
                    _allowMask |= (Permission)((long)mask & (long)-1);
                    _denyMask &= ~(Permission)((long)mask & (long)-1);
                    break;
                case AccessControlEntry.NotSet:
                    _allowMask &= ~(Permission)((long)mask & (long)-1);
                    _denyMask &= ~(Permission)((long)mask & (long)-1);
                    break;
                default:
                    _allowMask &= ~(Permission)((long)mask & (long)-1);
                    _denyMask |= (Permission)((long)mask & (long)-1);
                    break;
            }
        }
        /// <summary>
        /// This method merges the supplied permissions with the current permissions to come up with an
        /// updated permission set. The logic is that and Implied Allow overrides an Implied Deny, but 
        /// and Explicit Deny overrides an Implicit Allow, while an Explicit Allow overrides an Explicit
        /// Deny. This gives us a least restrictive security system.
        /// </summary>
        /// <param name="permissionRecord">The permission to merge with the current permission set</param>
        public void Merge(PermissionRecord permissionRecord)
        {
            this._allowMask |= permissionRecord.AllowMask;
            this._denyMask |= permissionRecord.DenyMask;
        }

        #endregion
    }
}