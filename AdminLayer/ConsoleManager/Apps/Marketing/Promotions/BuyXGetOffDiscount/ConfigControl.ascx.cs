using System;
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
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Marketing.Objects;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Marketing.Dto;
using ComponentArt.Web.UI;
using Mediachase.Commerce.Manager;
using System.Collections.Generic;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Marketing;
using Mediachase.Commerce.Marketing.Managers;
using Mediachase.Commerce.Catalog;
using System.Globalization;

public partial class Apps_Marketing_Promotions_BuyXGetOffDiscount : PromotionBaseUserControl
{
    public const string SkuEntrysParamName = "EntryName";

    /// <summary>
    /// Gets or sets the persist sku entrys.
    /// </summary>
    /// <value>The persist sku entrys.</value>
    public string PersistSkuEntrys
    {
        get
        {
            return (string)SelectedEntries.Value;
        }
        set
        {
            SelectedEntries.Value = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private class SkuEntryHelper : List<string>
    {
        public static char SkuEntryDelim = ';';
        public static int MaxNumEntry = 0x08;

        /// <summary>
        /// Initializes a new instance of the <see cref="SkuEntryHelper"/> class.
        /// </summary>
        /// <param name="srcString">The SRC string.</param>
        public SkuEntryHelper(string srcString)
        {
            InitFromString(srcString);
        }

        /// <summary>
        /// Adds the sku entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        public void AddSkuEntry(string newEntry)
        {
            if (!this.Contains(newEntry) && this.Count < MaxNumEntry)
            {
                this.Add(newEntry);
            }
        }

        /// <summary>
        /// Removes the sku entry.
        /// </summary>
        /// <param name="removeEntry">The remove entry.</param>
        public void RemoveSkuEntry(string removeEntry)
        {
            this.Remove(removeEntry);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            string retVal = string.Empty;
            foreach (string str in this)
            {
                retVal += string.Format("{0}{1}", str, SkuEntryDelim);
            }

            return retVal;
        }

        /// <summary>
        /// Inits from string.
        /// </summary>
        /// <param name="skuEntrys">The sku entrys.</param>
        private void InitFromString(string skuEntrys)
        {
            if (string.IsNullOrEmpty(skuEntrys) == false)
            {
                foreach (string entry in skuEntrys.Split(new char[] { SkuEntryDelim }, StringSplitOptions.RemoveEmptyEntries))
                {
                    this.Add(entry);
                    if (this.Count == MaxNumEntry)
                        break;
                }
            }
        }

    }
    [Serializable]
    public class Settings
    {
        public string RuleSkuSet = String.Empty;
        public char SkuDelimiter = SkuEntryHelper.SkuEntryDelim;
        public decimal MinQuantity = 1;
        public string RewardType = PromotionRewardType.EachAffectedEntry;
        public decimal AmountOff = 0;
        public string AmountType = PromotionRewardAmountType.Value;

    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        AddEntry.Click += new EventHandler(AddEntry_Click);
        if (!IsPostBack)
            BindForm();

        // ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(DeleteButton);
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

        OfferAmount.Text = "0";
        MinQuantity.Text = "0";

        Settings settings = GetSettings();
        if (settings != null)
        {
            PersistSkuEntrys = settings.RuleSkuSet;
            MinQuantity.Text = settings.MinQuantity.ToString();
            EntryList.DataSource = new SkuEntryHelper(settings.RuleSkuSet);
            EntryList.DataBind();
        }
        if (PromotionDto != null && PromotionDto.Promotion.Count != 0)
        {
            OfferAmount.Text = PromotionDto.Promotion[0].OfferAmount.ToString("N2");
            ManagementHelper.SelectListItem(OfferType, PromotionDto.Promotion[0].OfferType);
        }
    }

    /// <summary>
    /// Handles the Click event of the AddEntry control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void AddEntry_Click(object sender, EventArgs e)
    {
        SkuEntryHelper skuEntrys = new SkuEntryHelper(PersistSkuEntrys);
        skuEntrys.AddSkuEntry(SkuEntryFilter.SelectedEntryCode);
        EntryList.DataSource = skuEntrys;
        EntryList.DataBind();
        this.PersistSkuEntrys = skuEntrys.ToString();

    }

    /// <summary>
    /// Handles the Command event of the DeleteButton control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
    protected void DeleteButton_Command(object sender, CommandEventArgs e)
    {
        SkuEntryHelper skuEntrys = new SkuEntryHelper(PersistSkuEntrys);
        skuEntrys.RemoveSkuEntry(e.CommandArgument.ToString());
        EntryList.DataSource = skuEntrys;
        EntryList.DataBind();
        this.PersistSkuEntrys = skuEntrys.ToString();
    }

    /// <summary>
    /// Gets the settings.
    /// </summary>
    /// <returns></returns>
    private Settings GetSettings()
    {
        Settings retVal = null;
        if (PromotionDto != null && PromotionDto.Promotion.Count != 0)
        {
            object settingsObj = DeseralizeSettings(typeof(Settings));
            if (settingsObj != null)
            {
                retVal = settingsObj as Settings;
            }
        }

        return retVal;
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

        settings.RuleSkuSet = this.PersistSkuEntrys;
        settings.MinQuantity = Decimal.Parse(MinQuantity.Text);
        settings.AmountOff = Decimal.Parse(OfferAmount.Text);
        settings.RewardType = PromotionRewardType.EachAffectedEntry;
        int offerType = Int32.Parse(OfferType.SelectedValue);
        settings.AmountType = offerType == 1 ? PromotionRewardAmountType.Percentage : PromotionRewardAmountType.Value;

        // Create custom expression from template
        string expr = Replace(Config.Expression, settings);

        // save properties for promotion
        PromotionDto.PromotionRow promotion = PromotionDto.Promotion[0];
        promotion.OfferAmount = Decimal.Parse(OfferAmount.Text);
        promotion.OfferType = Int32.Parse(OfferType.SelectedValue);
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
