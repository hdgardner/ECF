﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.Util;
using Mediachase.Cms.WebUtility;

namespace Mediachase.Cms.Website.Templates.NWTD.PageTemplates
{
    public partial class MSSDCatalogTemplate : BaseStoreUserControl, IPublicTemplate {
		protected void Page_Load(object sender, EventArgs e) {

		}

		#region IPublicTemplate Members

		public string ControlPlaces {
			get { return "MainContentArea, RightContentArea"; }
		}

		#endregion
	}
}