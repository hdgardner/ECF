using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Commerce.Storage;
using System.Runtime.Serialization;
using Mediachase.MetaDataPlus.Configurator;
using System.Data;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Implements operations for the order storage base. (Inherits <see cref="MetaStorageBase"/>, <see cref="ISerializable"/>.)
    /// </summary>
    [Serializable]
    public abstract class OrderStorageBase : MetaStorageBase, ISerializable
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaStorageBase"/> class.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="reader">The reader.</param>
        internal OrderStorageBase(MetaClass metaClass, IDataReader reader) : base(metaClass, reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaStorageBase"/> class.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        internal OrderStorageBase(MetaClass metaClass) : base(metaClass)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderStorageBase"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OrderStorageBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Gets the object data.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);
        }
        #endregion

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        /// <param name="saveSystem">if set to <c>true</c> [save system].</param>
        internal virtual void AcceptChanges(bool saveSystem)
        {
            base.AcceptChanges(OrderContext.MetaDataContext, saveSystem);
        }

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            base.AcceptChanges(OrderContext.MetaDataContext);
        }
    }
}
