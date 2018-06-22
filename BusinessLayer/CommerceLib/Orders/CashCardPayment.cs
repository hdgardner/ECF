using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Represents Cash Card type of payment.
    /// </summary>
    [Serializable]
    public class CashCardPayment : Payment
    {
        private static MetaClass _MetaClass;

        /// <summary>
        /// Gets the cash card payment meta class.
        /// </summary>
        /// <value>The cash card payment meta class.</value>
        public static MetaClass CashCardPaymentMetaClass
        {
            get
            {
                if(_MetaClass == null)
                {
                    _MetaClass = MetaClass.Load(OrderContext.MetaDataContext, "CashCardPayment");
                }

                return _MetaClass;
            }
        }

        /// <summary>
        /// Gets or sets the cash card number. The field is not encrypted by default. Encryption should be handled by 
        /// the layer calling the property.
        /// </summary>
        /// <value>The credit card number.</value>
        public string CashCardNumber
        {
            get
            {
                return GetString("CashCardNumber");
            }
            set
            {
                this["CashCardNumber"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the cash card security code.
        /// </summary>
        /// <value>The cash card security code.</value>
        public string CashCardSecurityCode
        {
            get
            {
                return GetString("CashCardSecurityCode");
            }
            set
            {
                this["CashCardSecurityCode"] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CashCardPayment"/> class.
        /// </summary>
        public CashCardPayment()
            : base(CashCardPaymentMetaClass) 
        {
            this.PaymentType = PaymentType.CashCard;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CashCardPayment"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected CashCardPayment(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
            this.PaymentType = PaymentType.CashCard;
        }
    }
}
