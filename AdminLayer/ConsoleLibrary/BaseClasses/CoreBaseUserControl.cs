using System;
using System.Web.UI;
using Mediachase.MetaDataPlus;
using Mediachase.Commerce.Catalog;

namespace Mediachase.Web.Console.BaseClasses
{
    public class CoreBaseUserControl : BaseUserControl
    {
		private MetaDataContext _MDContext = null;

        /// <summary>
        /// Gets or sets the MD context.
        /// </summary>
        /// <value>The MD context.</value>
		public MetaDataContext MDContext
		{
			get
			{
				return _MDContext;
			}
			set
			{
				_MDContext = value;
				//SetMetaDataContext();
			}
		}

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
			//SetMetaDataContext();
            base.OnInit(e);
        }

        /// <summary>
        /// Sets the meta data context.
        /// </summary>
		private void SetMetaDataContext()
		{
			//MetaDataContext.Current = MDContext;
		}
    }
}
