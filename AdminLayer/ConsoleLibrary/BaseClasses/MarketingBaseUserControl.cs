using System;
using System.Web.UI;
using Mediachase.Commerce.Marketing;

namespace Mediachase.Web.Console.BaseClasses
{
    public class MarketingBaseUserControl : BaseUserControl
    {
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            //MetaDataContext.Current = MarketingContext.MetaDataContext;
            base.OnInit(e);
        }
    }
}
