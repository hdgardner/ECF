﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Commerce.Manager;
using Mediachase.Web.Console.Interfaces;

public partial class Apps_Asset_GridTemplates_NodeHyperlinkTemplate2 : System.Web.UI.UserControl, IEcfListViewTemplate
{
    private object _DataItem;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    #region IEcfListViewTemplate Members

    public object DataItem
    {
        get
        {
            return _DataItem;
        }
        set
        {
            _DataItem = value;
        }
    }

    #endregion
}