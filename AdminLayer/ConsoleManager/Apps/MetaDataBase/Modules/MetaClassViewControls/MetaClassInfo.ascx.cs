using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Modules.MetaClassViewControls
{
	public partial class MetaClassInfo : MCDataBoundControl
	{
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		#region DataBind
        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
		public override void DataBind()
		{
			if (DataItem != null)
			{
				MetaClass mc = DataItem as MetaClass;
				if (mc != null)
					BindData(mc);
			}
			base.DataBind();
		}
		#endregion

		#region BindData
        /// <summary>
        /// Binds the data.
        /// </summary>
        /// <param name="mc">The mc.</param>
		private void BindData(MetaClass mc)
		{
			trMoreInfo.Visible = false;

			// Labels
			lblClassName.Text = mc.Name;

			lblFriendlyName.Text = CHelper.GetResFileString(mc.FriendlyName);
			lblPluralName.Text = CHelper.GetResFileString(mc.PluralName);
			if (mc.Attributes.ContainsKey(MetaClassAttribute.IsBridge))
			{
				lblType.Text = GetGlobalResourceObject("GlobalMetaInfo", "Bridge").ToString();
				imClassType.ImageUrl = "../../images/metainfo/bridge.gif";
				if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					imClassType.ImageUrl = "../../images/metainfo/bridge_sys.gif";
			}
			else if (mc.Attributes.ContainsKey(MetaClassAttribute.IsCard))
			{
				lblType.Text = GetGlobalResourceObject("GlobalMetaInfo", "BusinessObjectExtension").ToString();
				imClassType.ImageUrl = "../../images/metainfo/card.gif";
				if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					imClassType.ImageUrl = "../../images/metainfo/card_sys.gif";
			}
			else
			{
				lblType.Text = GetGlobalResourceObject("GlobalMetaInfo", "Info").ToString();
				imClassType.ImageUrl = "../../images/metainfo/metaclass.gif";
				if (mc.Attributes.ContainsKey(MetaClassAttribute.IsSystem))
					imClassType.ImageUrl = "../../images/metainfo/metaclass_sys.gif";
			}

			// Public or Private (Department or User)
			if (mc.Attributes.ContainsKey(MetaDataWrapper.OwnerTypeAttr))
				lblType.Text += String.Format(" ({0})", mc.Attributes[MetaDataWrapper.OwnerTypeAttr].ToString());

			// Owner class for Card
			MetaClass ownerClass = MetaDataWrapper.GetOwnerClass(mc);
			if (ownerClass != null)
			{
				lblMoreInfo.Text = String.Format("<a href='MetaClassView.aspx?class={0}'>{1}</a>", ownerClass.Name, ownerClass.Name);

				trMoreInfo.Visible = true;
				lblMoreInfoLabel.Text = GetGlobalResourceObject("GlobalMetaInfo", "TableOwner").ToString() + ":";
			}

			// Cards for owner class
			if (mc.SupportsCards)
			{
				MetaClass[] cards = mc.GetCards();
				if (cards.Length > 0)
				{
					trMoreInfo.Visible = true;
					lblMoreInfoLabel.Text = GetGlobalResourceObject("GlobalMetaInfo", "Cards").ToString() + ":";

					string sCards = "";
					foreach (MetaClass card in cards)
					{
						if (sCards != "")
							sCards += ", ";

						sCards += String.Format("<a href='MetaClassView.aspx?class={0}'>{1}</a>", CHelper.GetResFileString(card.FriendlyName), card.Name);
					}
					lblMoreInfo.Text = sCards;
				}
			}
		}
		#endregion
	}
}