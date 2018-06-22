using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Represents Invoice type of payment.
    /// </summary>
    [Serializable]
    public class InvoicePayment : Payment
    {
        private static MetaClass _MetaClass;

        /// <summary>
        /// Gets the invoice payment meta class.
        /// </summary>
        /// <value>The invoice payment meta class.</value>
        public static MetaClass InvoicePaymentMetaClass
        {
            get
            {
                if(_MetaClass == null)
                {
                    _MetaClass = MetaClass.Load(OrderContext.MetaDataContext, "InvoicePayment");
                }

                return _MetaClass;
            }
        }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        /// <value>The invoice number.</value>
        public string InvoiceNumber
        {
            get
            {
                return GetString("InvoiceNumber");
            }
            set
            {
                this["InvoiceNumber"] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicePayment"/> class.
        /// </summary>
        public InvoicePayment()
            : base(InvoicePaymentMetaClass) 
        {
            this.PaymentType = PaymentType.Invoice;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicePayment"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected InvoicePayment(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
            this.PaymentType = PaymentType.Invoice;
        }
    }
}
