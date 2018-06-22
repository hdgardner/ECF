using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Commerce.Marketing;
using System.Reflection;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Commerce.Marketing.Dto;
using System.Collections;
using Mediachase.Commerce.Marketing.Objects;
using System.Xml.Serialization;
using System.IO;
using Mediachase.Commerce.Marketing.Managers;
using System.Data;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog;
using System.Threading;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mediachase.Web.Console.BaseClasses
{
    public class PromotionBaseUserControl : MarketingBaseUserControl, IAdminTabControl, IAdminContextControl
    {
        /// <summary>
        /// Replaces the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public string Replace(string expression, object settings)
        {
            foreach (FieldInfo field in settings.GetType().GetFields())
            {
                object val = field.GetValue(settings);
                string valValue = String.Empty;

				// Author Fix: et 28.10.2008  
				// In call rulset in workflow current culture not equals 
				// culture in this thread, and some string values not correctly
				// parsed.
				//tmporary change current thread culture to invariant
				//some types decimal, DateTime is culture specific 
				CultureInfo oldCult = Thread.CurrentThread.CurrentCulture;
				Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                if (val != null)
                    valValue = val.ToString();
				//Restore back culture
				Thread.CurrentThread.CurrentCulture = oldCult;


                expression = Replace(expression, "$" + field.Name, valValue);
            }

            return expression;
        }

        /// <summary>
        /// Replaces the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="token">The token.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public string Replace(string expression, string token, string value)
        {
            return expression.Replace(token, value);
        }

        /// <summary>
        /// Serializes the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public byte[] SerializeSettings(object settings)
        {
            XmlSerializer serializer = new XmlSerializer(settings.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, settings);
            return stream.GetBuffer();
        }

        /// <summary>
        /// Deseralizes the settings.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public object DeseralizeSettings(Type type)
        {
            if (_PromotionDto != null && _PromotionDto.Promotion.Count != 0)
            {
                PromotionDto.PromotionRow row = _PromotionDto.Promotion[0];
                if (!row.IsParamsNull())
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    MemoryStream stream = new MemoryStream(row.Params);
                    return serializer.Deserialize(stream);
                }
            }

            return null;
        }

        /// <summary>
        /// Serializes the settings binary.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public byte[] SerializeSettingsBinary(object settings)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, settings);
            return stream.GetBuffer();
        }

        /// <summary>
        /// Deseralizes the settings binary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public object DeseralizeSettingsBinary(Type type)
        {
            if (_PromotionDto != null && _PromotionDto.Promotion.Count != 0)
            {
                PromotionDto.PromotionRow row = _PromotionDto.Promotion[0];
                if (!row.IsParamsNull())
                {
                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream(row.Params);
                    return formatter.Deserialize(stream);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the expression row.
        /// </summary>
        /// <param name="expressionDto">The expression dto.</param>
        /// <returns></returns>
        public ExpressionDto.ExpressionRow CreateExpressionRow(ref ExpressionDto expressionDto)
        {
            PromotionDto.PromotionConditionRow row = null;
            ExpressionDto.ExpressionRow expressionRow = null;

            if (_PromotionDto.PromotionCondition.Count == 0)
            {
                row = _PromotionDto.PromotionCondition.NewPromotionConditionRow();
                row.PromotionId = _PromotionDto.Promotion[0].PromotionId;
                expressionRow = expressionDto.Expression.NewExpressionRow();
                expressionRow.ApplicationId = MarketingConfiguration.Instance.ApplicationId;
                expressionRow.Category = ExpressionCategory.CategoryKey.Promotion.ToString();
                expressionRow.Created = DateTime.UtcNow;
            }
            else
            {
                row = _PromotionDto.PromotionCondition[0];
                expressionDto = ExpressionManager.GetExpressionDto(row.ExpressionId);
                expressionRow = expressionDto.Expression[0];
            }

            expressionRow.ModifiedBy = Page.User.Identity.Name;
            expressionRow.Description = _Config.Description;
            expressionRow.Name = _Config.Type;

            row.ExpressionId = expressionRow.ExpressionId;

            if (row.RowState == DataRowState.Detached)
                PromotionDto.PromotionCondition.Rows.Add(row);

            return expressionRow;
        }

        PromotionDto _PromotionDto = null;

        /// <summary>
        /// Gets the promotion dto.
        /// </summary>
        /// <value>The promotion dto.</value>
        protected PromotionDto PromotionDto
        {
            get { return _PromotionDto; }
        }

        PromotionConfig _Config = null;

        /// <summary>
        /// Gets the config.
        /// </summary>
        /// <value>The config.</value>
        protected PromotionConfig Config
        {
            get { return _Config; }
        }

        /// <summary>
        /// Gets the name of the catalog entry.
        /// </summary>
        /// <param name="catalogEntryId">The catalog entry id.</param>
        /// <returns></returns>
        protected string GetCatalogEntryName(object catalogEntryId)
        {
            if (catalogEntryId == null)
                return String.Empty;

            CatalogEntryDto entry = CatalogContext.Current.GetCatalogEntryDto(catalogEntryId.ToString());

            if (entry.CatalogEntry.Count > 0)
                return String.Format("{0} [{1}]", entry.CatalogEntry[0].Name, entry.CatalogEntry[0].Code);
            else
                return String.Format("Entry Code:[{0}]", catalogEntryId.ToString());
        }

        #region IAdminTabControl Members

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void SaveChanges(IDictionary context)
        {
            _PromotionDto = (PromotionDto)context["PromotionDto"];
            _Config = (PromotionConfig)context["Config"];

            _PromotionDto.Promotion[0].PromotionGroup = _Config.Group;
            _PromotionDto.Promotion[0].PromotionType = _Config.Type;
        }

        #endregion

        #region IAdminContextControl Members
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void LoadContext(IDictionary context)
        {
            _PromotionDto = (PromotionDto)context["PromotionDto"];
            _Config = (PromotionConfig)context["Config"];
        }
        #endregion
    }
}
