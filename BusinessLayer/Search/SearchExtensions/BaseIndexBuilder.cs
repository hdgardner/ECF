using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.MetaDataPlus;
using Lucene.Net.Documents;
using Mediachase.MetaDataPlus.Configurator;
using Lucene.Net.Store;
using Lucene.Net.Index;
using System.IO;
using System.Data;
using Mediachase.Commerce.Core;
using System.Globalization;
using Mediachase.Commerce;
using Mediachase.Commerce.Storage;
using System.Collections;


namespace Mediachase.Search.Extensions.Indexers
{
    public abstract class BaseIndexBuilder : ISearchIndexBuilder
    {
        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="percentage">The percentage.</param>
        protected virtual void OnEvent(string message, double percentage)
        {
            this.Manager.RaiseSearchIndexEvent(this, new SearchIndexEventArgs(message, percentage));
        }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="metaField">The meta field.</param>
        /// <param name="metaObject">The meta object.</param>
        protected virtual void AddField(Document doc, MetaField metaField, Hashtable metaObject)
        {
            if (metaField.AllowSearch)
            {
                Field.Store store = null;
                Field.Index index = null;
                if (metaField.Attributes["IndexStored"] != null)
                {
                    if (metaField.Attributes["IndexStored"].Equals("true", StringComparison.OrdinalIgnoreCase))
                        store = Field.Store.YES;
                    else
                        store = Field.Store.NO;
                }

                if (metaField.Attributes["IndexField"] != null)
                {
                    if (metaField.Attributes["IndexField"].Equals("tokenized", StringComparison.OrdinalIgnoreCase))
                        index = Field.Index.TOKENIZED;
                    else
                        index = Field.Index.UN_TOKENIZED;
                }

                object val = MetaHelper.GetMetaFieldValue(metaField, metaObject[metaField.Name]);
                //object val = metaObject[metaField];
                string valString = String.Empty;
                if (
                    metaField.DataType == MetaDataType.BigInt ||
                    metaField.DataType == MetaDataType.Decimal ||
                    metaField.DataType == MetaDataType.Float ||
                    metaField.DataType == MetaDataType.Int ||
                    metaField.DataType == MetaDataType.Money ||
                    metaField.DataType == MetaDataType.Numeric ||
                    metaField.DataType == MetaDataType.SmallInt ||
                    metaField.DataType == MetaDataType.SmallMoney ||
                    metaField.DataType == MetaDataType.TinyInt
                    )
                {
                    if (val != null)
                    {
                        valString = val.ToString();
                        if (!String.IsNullOrEmpty(valString))
                        {
                            if (metaField.DataType == MetaDataType.Decimal)
                                doc.Add(new Field(metaField.Name, ConvertStringToSortable(Decimal.Parse(valString)), store == null ? Field.Store.NO : store, index == null ? Field.Index.UN_TOKENIZED : index));
                            else
                                doc.Add(new Field(metaField.Name, ConvertStringToSortable(valString), store == null ? Field.Store.NO : store, index == null ? Field.Index.UN_TOKENIZED : index));
                        }
                    }
                }
                else if (val != null)
                {
                    if (metaField.DataType == MetaDataType.DictionaryMultiValue)
                    {
                        foreach (string s in (string[])val)
                        {
                            doc.Add(new Field(metaField.Name, s == null ? String.Empty : s.ToLower(), store == null ? Field.Store.NO : store, index == null ? Field.Index.TOKENIZED : index));
                            doc.Add(new Field("_content", s == null ? String.Empty : s.ToString().ToLower(), store == null ? Field.Store.NO : store, index == null ? Field.Index.TOKENIZED : index));
                        }
                    }
                    else if (metaField.DataType == MetaDataType.StringDictionary)
                    {
                        MetaStringDictionary stringDictionary = val as MetaStringDictionary;
                        ArrayList strvals = new ArrayList();
                        if (stringDictionary != null)
                        {
                            foreach (string key in stringDictionary.Keys)
                            {
                                string s = stringDictionary[key];
                                doc.Add(new Field(metaField.Name, s == null ? String.Empty : s.ToLower(), store == null ? Field.Store.NO : store, index == null ? Field.Index.TOKENIZED : index));
                                doc.Add(new Field("_content", s == null ? String.Empty : s.ToString().ToLower(), store == null ? Field.Store.NO : store, index == null ? Field.Index.TOKENIZED : index));
                            }
                        }
                    }
                    else
                    {
                        doc.Add(new Field(metaField.Name, val == null ? String.Empty : val.ToString().ToLower(), store == null ? Field.Store.NO : store, index == null ? Field.Index.TOKENIZED : index));
                        doc.Add(new Field("_content", val == null ? String.Empty : val.ToString().ToLower(), store == null ? Field.Store.NO : store, index == null ? Field.Index.TOKENIZED : index));
                    }
                }
            }
        }

        /// <summary>
        /// Converts the string to sortable.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        protected string ConvertStringToSortable(string input)
        {
            string valString = String.Empty;
            input = input.Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator, "");
            input = input.Replace(".", "");
            valString = NumberTools.LongToString(long.Parse(input));
            return valString;
        }

        /// <summary>
        /// Converts the string to sortable.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        protected string ConvertStringToSortable(decimal input)
        {
            string valString = String.Empty;
            valString = input.ToString("####.0000", System.Globalization.CultureInfo.InvariantCulture);
            valString = NumberTools.LongToString(long.Parse(valString.Replace(".", "")));
            return valString;
        }

        SearchManager _Manager;
        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public SearchManager Manager
        {
            get
            {
                return _Manager;
            }
            set
            {
                _Manager = value;
            }
        }

        IndexBuilder _BuildIndexer;
        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        /// <value>The manager.</value>
        public IndexBuilder Indexer
        {
            get
            {
                return _BuildIndexer;
            }
            set
            {
                _BuildIndexer = value;
            }
        }

        #region ISearchIndexBuilder Members
        public abstract void BuildIndex(bool rebuild);
        #endregion
    }
}
