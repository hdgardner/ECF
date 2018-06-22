using System;
using System.Collections;
using System.Collections.Specialized;
using Mediachase.Commerce.Manager.Core;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Common;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Commerce.Manager.Marketing
{
    public partial class PromotionEdit : MarketingBaseUserControl
    {
		private const string _PromotionDtoEditSessionKey = "ECF.PromotionDto.Edit";
		private const string _PromotionIdString = "PromotionId";
		private const string _PromotionDtoString = "PromotionDto";

        /// <summary>
        /// Gets the promotion id.
        /// </summary>
        /// <value>The promotion id.</value>
        public int PromotionId
        {
            get
            {
                return ManagementHelper.GetIntFromQueryString(_PromotionIdString);
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadContext();
			CommandManager.GetCurrent(this.Page).AddCommand(string.Empty, "Marketing", "Promotion-Edit", "cmdShowEditorDialog_PromotionCatalogEntrySelect");
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditSaveControl.SaveChanges += new EventHandler<SaveControl.SaveEventArgs>(EditSaveControl_SaveChanges);
        }

        /// <summary>
        /// Loads the fresh.
        /// </summary>
        /// <returns></returns>
        private PromotionDto LoadFresh()
        {
            PromotionDto promo = PromotionManager.GetPromotionDto(PromotionId);

            // persist in session
            Session[_PromotionDtoEditSessionKey] = promo;

            return promo;
        }

        /// <summary>
        /// Loads the context.
        /// </summary>
        private void LoadContext()
        {
            if (PromotionId > 0)
            {
                PromotionDto promo = null;
                if (!this.IsPostBack && (!this.Request.QueryString.ToString().Contains("Callback=yes"))) // load fresh on initial load
                {
                    promo = LoadFresh();
                }
                else // load from session
                {
                    promo = (PromotionDto)Session[_PromotionDtoEditSessionKey];

                    if (promo == null)
                    {
                        promo = LoadFresh();
                    }
                }

                // Put a dictionary key that can be used by other tabs
                IDictionary dic = new ListDictionary();
                dic.Add(_PromotionDtoString, promo);

                // Call tabs load context
                ViewControl.LoadContext(dic);
            }
        }

        /// <summary>
        /// Handles the SaveChanges event of the EditSaveControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void EditSaveControl_SaveChanges(object sender, SaveControl.SaveEventArgs e)
        {
			// Validate form
			if (!this.Page.IsValid)
			{
				e.RunScript = false;
				return;
			}

            PromotionDto promo = (PromotionDto)Session[_PromotionDtoEditSessionKey];

            if (promo == null && PromotionId > 0)
                promo = PromotionManager.GetPromotionDto(PromotionId); //Int32.Parse(Parameters["PromotionId"].ToString()));

			if (PromotionId == 0)
				promo = new PromotionDto();

			/*
			// if we add a new promotion, remove all other segments from Dto that is passed to control that saves changes
			if (PromotionId == 0 && promo != null && promo.Promotion.Count > 0)
			{
				PromotionDto.PromotionRow[] rows2del = (PromotionDto.PromotionRow[])promo.Promotion.Select(String.Format("{0} <> {1}", _PromotionIdString, PromotionId));
				if (rows2del != null)
					foreach (PromotionDto.PromotionRow row in rows2del)
						promo.Promotion.RemovePromotionRow(row);
			}*/

            IDictionary context = new ListDictionary();
            context.Add(_PromotionDtoString, promo);

            ViewControl.SaveChanges(context);

            if (promo.HasChanges())
                PromotionManager.SavePromotion(promo);

			// we don't need to store Dto in session any more
			Session.Remove(_PromotionDtoEditSessionKey);
        }
    }
}