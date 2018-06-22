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
using Mediachase.Ibn.Web.UI.WebControls;

public partial class Apps_Marketing_Promotions_EntryCustomDiscount : PromotionBaseUserControl
{
	[Serializable]
	public class Settings
	{
		public FilterExpressionNodeCollection ConditionExpression = new FilterExpressionNodeCollection();
		public FilterExpressionNodeCollection TargetExpression = new FilterExpressionNodeCollection();
	}

	/// <summary>
	/// Handles the Load event of the Page control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
	protected void Page_Load(object sender, EventArgs e)
	{
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


		Settings settings = GetSettings();
		if (settings != null)
		{
			//MinQuantity.Text = settings.MinQuantity.ToString();
		}
		BindExpressionEditor(0, settings.ConditionExpression);
		BindApplyExpressionEditor(0, settings.TargetExpression);
	}

	private void BindExpressionEditor(int expressionId, object dataSource)
	{ 
		ConditionFilter.ProviderName = "PromotionDataProvider";
		ConditionFilter.ExpressionKey = expressionId.ToString();
		//Place Promotion Target Group to ExpressionPlace
		ConditionFilter.ExpressionPlace = string.Join(":", new string[] { base.Config.Group, "PromotionCondition" }); 
		ConditionFilter.DataSource = dataSource;
		ConditionFilter.DataBind();
	}

	private void BindApplyExpressionEditor(int expressionId, object dataSource)
	{
		ApplyConditionFilter.ProviderName = "PromotionActionProvider";
		ApplyConditionFilter.ExpressionKey = expressionId.ToString();
		ApplyConditionFilter.ExpressionPlace = string.Join(":", new string[] { base.Config.Group, "PromotionAction" });
		ApplyConditionFilter.DataSource = dataSource;
		ApplyConditionFilter.DataBind();
	}


	/// <summary>
	/// Gets the settings.
	/// </summary>
	/// <returns></returns>
	private Settings GetSettings()
	{
		Settings retVal = new Settings();
		if (PromotionDto != null && PromotionDto.Promotion.Count != 0)
		{
			object settingsObj = DeseralizeSettingsBinary(typeof(Settings));
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

		settings.TargetExpression = ApplyConditionFilter.NodeCollection[0].ChildNodes;
		settings.ConditionExpression = ConditionFilter.NodeCollection[0].ChildNodes;

	
		// save properties for promotion
		PromotionDto.PromotionRow promotion = PromotionDto.Promotion[0];
		promotion.Params = SerializeSettingsBinary(settings);

		//// Create custom expression from template
		//string expr = Replace(Config.Expression, settings);

		//// Create or modify expression
		ExpressionDto expressionDto = new ExpressionDto();
		ExpressionDto.ExpressionRow expressionRow = CreateExpressionRow(ref expressionDto);
		expressionRow.Category = ExpressionCategory.CategoryKey.Promotion.ToString();
		expressionRow.ExpressionXml = string.Empty;
		if (expressionRow.RowState == DataRowState.Detached)
			expressionDto.Expression.Rows.Add(expressionRow);
		//Change DataSource from FilterNodeCollection  to ExpressionDto
		ConditionFilter.Provider.DataSource = expressionDto;
		ConditionFilter.Provider.SaveFilters("PromotionCondition", expressionRow.ExpressionId.ToString(),
											settings.ConditionExpression);

		//// Save expression
		ExpressionManager.SaveExpression(expressionDto);

		ApplyConditionFilter.DataSource = expressionDto;
		ApplyConditionFilter.Provider.SaveFilters("PromotionAction", expressionRow.ExpressionId.ToString(), 
												 settings.TargetExpression);


		//// Save expression
		ExpressionManager.SaveExpression(expressionDto);

		//// Assign expression id to our new condition
		PromotionDto.PromotionCondition[0].ExpressionId = expressionRow.ExpressionId;
	}

	/*
	private RuleSet CreateRuleSetFromSettings(Settings settings)
	{

	}
	 * */
}
