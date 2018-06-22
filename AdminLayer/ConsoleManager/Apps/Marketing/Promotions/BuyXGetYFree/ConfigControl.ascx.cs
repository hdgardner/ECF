using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Marketing.Dto;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Managers;
using System.Xml.Serialization;
using System.IO;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Marketing.Objects;
using System.Reflection;
using Mediachase.Web.Console.BaseClasses;

public partial class Apps_Marketing_Promotions_BuyXGetYFree_ConfigControl : PromotionBaseUserControl
{
    [Serializable]
    public class Settings
    {
        public string EntryXFilter = String.Empty;
        public string EntryYFilter = String.Empty;
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!this.IsPostBack)
            BindForm();
    }

    /// <summary>
    /// Binds a data source to the invoked server control and all its child controls.
    /// </summary>
    public override void DataBind()
    {
        base.DataBind();
        BindForm();
    }

    /// <summary>
    /// Binds the form.
    /// </summary>
    private void BindForm()
    {
        if (Config != null)
        {
            Description.Text = Config.Description;
        }

        if (PromotionDto != null && PromotionDto.Promotion.Count != 0)
        {
            PromotionDto.PromotionRow row = PromotionDto.Promotion[0];
            object settingsObj = DeseralizeSettings(typeof(Settings));
            if (settingsObj != null)
            {
                Settings settings = (Settings)settingsObj;
                EntryXFilter.SelectedEntryCode = settings.EntryXFilter;
                EntryYFilter.SelectedEntryCode = settings.EntryYFilter;
            }

        }
    }

    /// <summary>
    /// Saves the changes.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void SaveChanges(IDictionary context)
    {
        base.SaveChanges(context);

        // Populate setting
        Settings settings = new Settings();
        settings.EntryXFilter = EntryXFilter.SelectedEntryCode;
        settings.EntryYFilter = EntryYFilter.SelectedEntryCode;
        // Create custom expression from template
        string expr = Replace(Config.Expression, settings);

        // save properties for promotion
        PromotionDto.PromotionRow promotion = PromotionDto.Promotion[0];
        promotion.OfferAmount = 100;
        promotion.OfferType = 0; //undef not use
        promotion.Params = SerializeSettings(settings);

        // Create or modify expression
        ExpressionDto expressionDto = new ExpressionDto();
        //PromotionDto.PromotionConditionRow row = null;

        ExpressionDto.ExpressionRow expressionRow = CreateExpressionRow(ref expressionDto);
        expressionRow.Category = ExpressionCategory.CategoryKey.Promotion.ToString();
        expressionRow.ExpressionXml = expr;
        if (expressionRow.RowState == DataRowState.Detached)
            expressionDto.Expression.Rows.Add(expressionRow);

        // Save expression
        ExpressionManager.SaveExpression(expressionDto);

        // Assign expression id to our new condition
        PromotionDto.PromotionCondition[0].ExpressionId = expressionRow.ExpressionId;

        //if (row.RowState == DataRowState.Detached)
            //PromotionDto.PromotionCondition.Rows.Add(row);
    }
}
