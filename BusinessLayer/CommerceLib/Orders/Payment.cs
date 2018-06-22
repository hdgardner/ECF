using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus.Configurator;
using System.Data;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Storage;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Base class for Payments, every payment accepted by the order system should be inherited from this class.
    /// </summary>
    [XmlInclude(typeof(CreditCardPayment))]
    [XmlInclude(typeof(OtherPayment))]
    [XmlInclude(typeof(CashCardPayment))]
    [XmlInclude(typeof(GiftCardPayment))]
    [XmlInclude(typeof(InvoicePayment))]
    public abstract class Payment : OrderStorageBase, ISerializable
    {
        private OrderForm _Parent;

        /// <summary>
        /// Gets the parent OrderForm payment belongs to.
        /// </summary>
        /// <value>The parent.</value>
        public OrderForm Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payment"/> class.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        public Payment(MetaClass metaClass) : base(metaClass) 
        {
            ImplementationClass = this.GetType().FullName;
            base["PaymentId"] = 0;
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="Parent">The parent.</param>
        public override void SetParent(object Parent)
        {
            _Parent = (OrderForm)Parent;
        }

        #region Public Properties
        /// <summary>
        /// Gets the payment id.
        /// </summary>
        /// <value>The payment id.</value>
        public int PaymentId
        {
            get
            {
                return base.GetInt("PaymentId");
            }
        }

        /// <summary>
        /// Gets or sets the order form id.
        /// </summary>
        /// <value>The order form id.</value>
        public int OrderFormId
        {
            get
            {
                return base.GetInt("OrderFormId");
            }
            set
            {
                base["OrderFormId"] = value;
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
                return base.GetInt("OrderGroupId");
            }
            set
            {
                base["OrderGroupId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the billing address id.
        /// </summary>
        /// <value>The billing address id.</value>
        public string BillingAddressId
        {
            get
            {
                return base.GetString("BillingAddressId");
            }
            set
            {
                base["BillingAddressId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the payment method id.
        /// </summary>
        /// <value>The payment method id.</value>
        public Guid PaymentMethodId
        {
            get
            {
                return base.GetGuid("PaymentMethodId");
            }
            set
            {
                base["PaymentMethodId"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the payment method.
        /// </summary>
        /// <value>The name of the payment method.</value>
        public string PaymentMethodName
        {
            get
            {
                return base.GetString("PaymentMethodName");
            }
            set
            {
                base["PaymentMethodName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        /// <value>The name of the customer.</value>
        public string CustomerName
        {
            get
            {
                return base.GetString("CustomerName");
            }
            set
            {
                base["CustomerName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>The amount.</value>
        public decimal Amount
        {
            get
            {
                return base.GetDecimal("Amount");
            }
            set
            {
                base["Amount"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of the payment. Types are CreditCard, CashCard, Invoice, GiftCard, Other
        /// </summary>
        /// <value>The type of the payment.</value>
        public PaymentType PaymentType
        {
            get
            {
                return (PaymentType)base.GetInt("PaymentType");
            }
            set
            {
                base["PaymentType"] = value.GetHashCode();
            }
        }

        /// <summary>
        /// Gets or sets the validation code.
        /// </summary>
        /// <value>The validation code.</value>
        public string ValidationCode
        {
            get
            {
                return base.GetString("ValidationCode");
            }
            set
            {
                base["ValidationCode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the authorization code.
        /// </summary>
        /// <value>The authorization code.</value>
        public string AuthorizationCode
        {
            get
            {
                return base.GetString("AuthorizationCode");
            }
            set
            {
                base["AuthorizationCode"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status
        {
            get
            {
                return base.GetString("Status");
            }
            set
            {
                base["Status"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the implementation class.
        /// </summary>
        /// <value>The implementation class.</value>
        private string ImplementationClass
        {
            get
            {
                return base.GetString("ImplementationClass");
            }
            set
            {
                base["ImplementationClass"] = value;
            }
        }

        #endregion

        /// <summary>
        /// Accepts the changes.
        /// </summary>
        public override void AcceptChanges()
        {
            if (_Parent == null)
                throw new NoNullAllowedException("Parent");

            if (_Parent.ObjectState == MetaObjectState.Added)
            {
                throw new MetaException("Must save parent object");
            }

            if (ObjectState != MetaObjectState.Deleted)
            {
                this.OrderGroupId = _Parent.OrderGroupId;
                this.OrderFormId = _Parent.OrderFormId;
            }
            base.AcceptChanges();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Payment"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected Payment(SerializationInfo info, StreamingContext context)
            : base(info, context) 
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
