using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Orders;

namespace Mediachase.Cms.WebUtility.Commerce
{
    /// <summary>
    /// Summary description for IPaymentOption
    /// </summary>
    /// <summary>
    /// IPaymentOption Interface that all payment gateways must implement.
    /// </summary>
    public interface IPaymentOption
    {
        /// <summary>
        /// Validates input data for the control. In case of Credit card pre authentication will be the best way to
        /// validate. The error message if any should be displayed within a control itself.
        /// </summary>
        /// <returns>Returns false if validation is failed.</returns>
        bool ValidateData();

        /// <summary>
        /// This method is called before the order is completed. This method should check all the parameters
        /// and validate the credit card or other parameters accepted.
        /// </summary>
        /// <param name="info">object of the OrderDetails class</param>
        /// <returns>bool</returns>
        Payment PreProcess(OrderForm form);

        /// <summary>
        /// This method is called after the order is placed. This method should be used by the gateways that want to
        /// redirect customer to their site.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        bool PostProcess(OrderForm orderForm);
    }

    /*
    /// <summary>
    /// Summary description for IPaymentButton
    /// </summary>
    public interface IPaymentButton
    {
        /// <summary>
        /// The Order will be automatically set by the engine.
        /// </summary>
        OrderInfo Order { get; set; }
    }
     * */
}
