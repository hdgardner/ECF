using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Data.Provider;
using Mediachase.Commerce.Storage;
using System.Data;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Base class for discounts in the system. All discounts should inherit this class.
    /// </summary>
    public abstract class Discount : SimpleObject, ISerializable
    {
        /// <summary>
        /// Gets or sets the discount id.
        /// </summary>
        /// <value>The discount id.</value>
        public int DiscountId
        {
            get
            {
                return base.GetInt("DiscountId");
            }
            set
            {
                base["DiscountId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the order group id.
        /// </summary>
        /// <value>The order group id.</value>
        public int OrderGroupId
        {
            get
            {
                return GetInt32("OrderGroupId");
            }
            set
            {
                this["OrderGroupId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the discount amount. The amount of discount that was applied. It will equal to DiscountValue if it is
        /// a fixed monetary value. For the percentage based discount this value will be different.
        /// </summary>
        /// <value>The discount amount.</value>
        public decimal DiscountAmount
        {
            get
            {
                return base.GetDecimal("DiscountAmount");
            }
            set
            {
                base["DiscountAmount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the discount code. Also known as a coupon code.
        /// </summary>
        /// <value>The discount code.</value>
        public string DiscountCode
        {
            get
            {
                return base.GetString("DiscountCode");
            }
            set
            {
                base["DiscountCode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the discount.
        /// </summary>
        /// <value>The name of the discount.</value>
        public string DiscountName
        {
            get
            {
                return base.GetString("DiscountName");
            }
            set
            {
                base["DiscountName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the display message.
        /// </summary>
        /// <value>The display message.</value>
        public string DisplayMessage
        {
            get
            {
                return base.GetString("DisplayMessage");
            }
            set
            {
                base["DisplayMessage"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the discount value. Either fixed money value or percentage.
        /// </summary>
        /// <value>The discount value.</value>
        public decimal DiscountValue
        {
            get
            {
                return base.GetDecimal("DiscountValue");
            }
            set
            {
                base["DiscountValue"] = value;
            }
        }

        /// <summary>
        /// Creates the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        protected virtual void CreateParameters(DataCommand command)
        {
            command.Parameters.Add(new DataParameter("DiscountId", DiscountId, DataParameterType.Int));
            command.Parameters.Add(new DataParameter("OrderGroupId", OrderGroupId, DataParameterType.Int));
            command.Parameters.Add(new DataParameter("DiscountAmount", DiscountAmount, DataParameterType.Decimal));
            command.Parameters.Add(new DataParameter("DiscountCode", DiscountCode, DataParameterType.NVarChar));
            command.Parameters.Add(new DataParameter("DiscountName", DiscountName, DataParameterType.NVarChar));
            command.Parameters.Add(new DataParameter("DisplayMessage", DisplayMessage, DataParameterType.NVarChar));
            command.Parameters.Add(new DataParameter("DiscountValue", DiscountValue, DataParameterType.Decimal));           
        }

        /// <summary>
        /// Creates the insert command.
        /// </summary>
        /// <returns></returns>
        protected override DataCommand CreateInsertCommand()
        {
            string tableName = TableName;
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = OrderContext.MetaDataContext.ConnectionString;
            cmd.CommandText = DataHelper.CreateInsertStoredProcedureName(tableName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters = new DataParameters();
            CreateParameters(cmd);
            //cmd.Parameters.Sort();

            // Assume first parameter is the key
            cmd.Parameters[0].Direction = ParameterDirection.Output;

            // we expect null value in the procedure
            if (this.ObjectState == MetaObjectState.Added && (int)cmd.Parameters[0].Value == 0)
                cmd.Parameters[0].Value = DBNull.Value;

            return cmd;
        }

        /// <summary>
        /// Creates the update command.
        /// </summary>
        /// <returns></returns>
        protected override DataCommand CreateUpdateCommand()
        {
            string tableName = TableName;
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = OrderContext.MetaDataContext.ConnectionString;
            cmd.CommandText = DataHelper.CreateUpdateStoredProcedureName(tableName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters = new DataParameters();
            CreateParameters(cmd);
            return cmd;
        }

        /// <summary>
        /// Creates the delete command.
        /// </summary>
        /// <returns></returns>
        protected override DataCommand CreateDeleteCommand()
        {
            string tableName = TableName;
            DataCommand cmd = new DataCommand();
            cmd.ConnectionString = OrderContext.MetaDataContext.ConnectionString;
            cmd.CommandText = DataHelper.CreateDeleteStoredProcedureName(tableName);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters = new DataParameters();
            CreateParameters(cmd);
            cmd.Parameters.RemoveRange(1, cmd.Parameters.Count - 1);
            return cmd;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Discount"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Discount(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Discount"/> class.
        /// </summary>
        public Discount() : base()
        {
        }

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

    }
}
