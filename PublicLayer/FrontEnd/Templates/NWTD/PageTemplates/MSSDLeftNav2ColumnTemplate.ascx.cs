using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Cms.WebUtility;
using Mediachase.Cms.Util;
namespace Mediachase.Cms.Website.Templates.NWTD.PageTemplates
{
    public partial class MSSDLeftNav2ColumnTemplate : BaseStoreUserControl, IPublicTemplate
    {
        public string ControlPlaces
        {
            get { return "MainContentArea, RightContentArea"; }
        }

    }
}