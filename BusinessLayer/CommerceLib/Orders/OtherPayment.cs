using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.MetaDataPlus;
using System.Runtime.Serialization;

namespace Mediachase.Commerce.Orders
{
    /// <summary>
    /// Represents Other type of payment.
    /// </summary>
    [Serializable]
    public class OtherPayment : Payment
    {
        private static MetaClass _MetaClass;

        /// <summary>
        /// Gets the invoice payment meta class.
        /// </summary>
        /// <value>The invoice payment meta class.</value>
        public static MetaClass OtherPaymentMetaClass
        {
            get
            {
                if(_MetaClass == null)
                {
                    _MetaClass = MetaClass.Load(OrderContext.MetaDataContext, "OtherPayment");
                }

                return _MetaClass;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherPayment"/> class.
        /// </summary>
        public OtherPayment()
            : base(OtherPaymentMetaClass) 
        {
            this.PaymentType = PaymentType.Other;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OtherPayment"/> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected OtherPayment(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
            this.PaymentType = PaymentType.Other;
        }
    }
}
