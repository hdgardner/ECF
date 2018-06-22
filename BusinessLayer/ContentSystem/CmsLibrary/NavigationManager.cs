using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using Mediachase.Cms.Data;
using Mediachase.Data.Provider;

namespace Mediachase.Cms
{
	public static class NavigationManager
    {
        #region --- Add ---

        #region NewCommand
        /// <summary>
        /// News the command.
        /// </summary>
        /// <param name="urlUID">The URL UID.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="pars">The pars.</param>
        /// <param name="triggerParam">The trigger param.</param>
        /// <returns></returns>
        public static int NewCommand(string urlUID, int itemId, string pars, string triggerParam)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationCommandInsert]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("UrlUID", urlUID, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("ItemId", itemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Params", pars, DataParameterType.NVarChar, 1024));
			cmd.Parameters.Add(new DataParameter("TrigerParam", triggerParam, DataParameterType.NVarChar, 256));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion

        #region NewItem
        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        public static int NewItem(string itemName)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationItemsInsert]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ItemName", itemName, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion

        #region NewParam
        /// <summary>
        /// News the param.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="name">The name.</param>
        /// <param name="val">The val.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <returns></returns>
        public static int NewParam(int itemId, string name, string val, bool isRequired)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationParamsInsert]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ItemId", itemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("Value", val, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("IsRequired", isRequired, DataParameterType.Bit));
			return DataService.RunReturnInteger(cmd);
        }
        #endregion

        #endregion

        #region --- Get ---

        #region GetCommandById
        /// <summary>
        /// Gets the command by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static IDataReader GetCommandById(int id)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationCommandSelect]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// Gets the command by item id.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public static IDataReader GetCommandByItemId(int itemId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationCommandSelectByItemId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ItemId", itemId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        } 
        #endregion

        #region GetAllCommands
        /// <summary>
        /// Gets all commands.
        /// </summary>
        /// <returns></returns>
        public static IDataReader GetAllCommands()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationCommandAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// Gets all commands DT.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllCommandsDT()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationCommandAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
        }
        #endregion

        #region GetAllParams
        /// <summary>
        /// Gets all params.
        /// </summary>
        /// <returns></returns>
        public static IDataReader GetAllParams()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationParamsSelectAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }
        #endregion

        #region GetParamsByItemId
        /// <summary>
        /// Gets the params by item id.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public static IDataReader GetParamsByItemId(int itemId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationParamsSelectByItemId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ItemId", itemId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// Gets the params by item id DT.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public static DataTable GetParamsByItemIdDT(int itemId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationParamsSelectByItemId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ItemId", itemId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
        }

        #endregion

        #region Gets for Items
        /// <summary>
        /// Gets the item by id.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public static IDataReader GetItemById(int itemId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationItemsSelect]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ItemId", itemId, DataParameterType.Int));
			return DataService.LoadReader(cmd).DataReader;
        }

        #region GetItemNameById
        /// <summary>
        /// Gets the item name by id.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public static string GetItemNameById(int itemId)
        {
			string name = String.Empty;
            using (IDataReader reader = GetItemById(itemId))
            {
                if (reader.Read())
                    name = (string)reader["ItemName"];

                reader.Close();
            }
			return name;
        }
        #endregion

        #region GetItemNameByCommandId
        /// <summary>
        /// Gets the item name by command id.
        /// </summary>
        /// <param name="commandId">The command id.</param>
        /// <returns></returns>
        public static string GetItemNameByCommandId(int commandId)
        {
			string name = String.Empty;
            using (IDataReader reader = GetCommandById(commandId))
            {
                if (reader.Read())
                {
                    using (IDataReader reader2 = GetItemById((int)reader["ItemId"]))
                    {
                        if (reader2.Read())
							name = (string)reader2["ItemName"];

                        reader2.Close();
                    }
                }

                reader.Close();
            }
			return name;
        } 
        #endregion

        #region GetItemIdByCommandId
        /// <summary>
        /// Gets the item id by command id.
        /// </summary>
        /// <param name="commandId">The command id.</param>
        /// <returns></returns>
        public static int GetItemIdByCommandId(int commandId)
        {
            using (IDataReader reader = GetCommandById(commandId))
            {
                if (reader.Read())
                {
                    int itemId = (int)reader["ItemId"];                    
                    reader.Close();
                    return itemId;
                }
                else
                {
                    reader.Close();
                    return -1;
                }
            }
        } 
        #endregion

        #region GetAllItems
        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns></returns>
        public static IDataReader GetAllItems()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationItemsSelectAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadReader(cmd).DataReader;
        }

        /// <summary>
        /// Gets all items DT.
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllItemsDT()
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationItemsSelectAll]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			return DataService.LoadTable(cmd);
        }

        /// <summary>
        /// Gets the name of the item by.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        public static IDataReader GetItemByName(string itemName)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationItemsSelectByName]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
			cmd.Parameters.Add(new DataParameter("ItemName", itemName, DataParameterType.NVarChar, 256));
			return DataService.LoadReader(cmd).DataReader;
        } 
        #endregion

        #endregion 

        #region GetByPageId
        /// <summary>
        /// Gets the by page id.
        /// </summary>
        /// <param name="pageId">The page id.</param>
        /// <returns></returns>
        public static DataTable GetByPageId(int pageId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationCommandSelectByPageId]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("UrlUID", pageId, DataParameterType.Int));
			return DataService.LoadTable(cmd);
        } 
        #endregion

        #endregion

        #region --- Delete ---

        #region DeleteCommand
        /// <summary>
        /// Deletes the command.
        /// </summary>
        /// <param name="id">The id.</param>
        public static void DeleteCommand(int id)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationCommandDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			DataService.Run(cmd);
        } 
        #endregion

        #region DeleteParam
        /// <summary>
        /// Deletes the param.
        /// </summary>
        /// <param name="id">The id.</param>
        public static void DeleteParam(int id)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationParamsDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			DataService.Run(cmd);
        } 
        #endregion

        #region DeleteItem
        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="id">The id.</param>
        public static void DeleteItem(int id)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationItemsDelete]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			DataService.Run(cmd);
        } 
        #endregion

        #endregion

        #region --- Update ---

        #region UpdateCommand
        /// <summary>
        /// Updates the command.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="urlUID">The URL UID.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="pars">The pars.</param>
        /// <param name="triggerParam">The trigger param.</param>
        public static void UpdateCommand(int id, string urlUID, int itemId, string pars, string triggerParam)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationCommandUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("UrlUID", urlUID, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("ItemId", itemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Params", pars, DataParameterType.NVarChar, 1024));
			cmd.Parameters.Add(new DataParameter("TrigerParam", triggerParam, DataParameterType.NVarChar, 256));
			DataService.Run(cmd);
        } 
        #endregion

        #region UpdateItem
        /// <summary>
        /// Updates the item.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="itemName">Name of the item.</param>
        public static void UpdateItem(int id, string itemName)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationItemsUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("ItemName", itemName, DataParameterType.NVarChar, 256));
			DataService.Run(cmd);
        } 
        #endregion

        #region UpdateParam
        /// <summary>
        /// Updates the param.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="name">The name.</param>
        /// <param name="val">The val.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        public static void UpdateParam(int id, int itemId, string name, string val, bool isRequired)
        {
            DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_NavigationParamsUpdate]");
			cmd.Parameters = new DataParameters();
			cmd.Parameters.Add(new DataParameter("Id", id, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("ItemId", itemId, DataParameterType.Int));
			cmd.Parameters.Add(new DataParameter("Name", name, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("Value", val, DataParameterType.NVarChar, 256));
			cmd.Parameters.Add(new DataParameter("IsRequired", isRequired, DataParameterType.Bit));
			DataService.Run(cmd);

        } 
        #endregion

        #endregion

        #region GetUrlByVersionId
        /// <summary>
        /// Gets the URL by version id.
        /// </summary>
        /// <param name="versionId">The version id.</param>
        /// <param name="pars">The pars.</param>
        /// <returns></returns>
        public static string GetUrlByVersionId(int versionId, string pars)
        {
            using (IDataReader reader = PageVersion.GetVersionById(versionId))
            {
                if (reader.Read())
                {
                    using (IDataReader reader2 = FileTreeItem.GetItemById((int)reader["PageId"]))
                    {
                        if (reader2.Read())
                        {
							string url = "~"+(string)reader2["Outline"] + pars;
                            reader2.Close();
                            return url;
                        }
                    }
                }

                reader.Close();
            }
            return string.Empty;
        } 
        #endregion

        #region GetLanguageId
        /*private static int GetLanguageId()
        {
            int languageId = -1;

            using (IDataReader reader = Language.GetLangByName(Thread.CurrentThread.CurrentCulture.Name))
            {
                if (reader.Read())
                {
                    languageId = (int)reader["LangId"];
                }
                else
                {
                    using (IDataReader reader2 = Language.GetAllLanguages())
                    {
                        while (reader2.Read())
                        {
                            if ((bool)reader2["IsDefault"])
                            {
                                return (int)reader2["LangId"];
                            }
                        }
                    }
                }
            }
            return languageId;
        } */
        #endregion

        #region GetUrlByUID
        /// <summary>
        /// Gets the URL by UID.
        /// </summary>
        /// <param name="urlUID">The URL UID.</param>
        /// <param name="pars">The pars.</param>
        /// <returns></returns>
        public static string GetUrlByUID(string urlUID, string pars)
        {
			using (IDataReader reader = FileTreeItem.GetItemById(Convert.ToInt32(urlUID)))
			{
				if (reader.Read())
				{
                    string url = "~" + (string)reader["Outline"] + pars;
                    reader.Close();
					return url;
				}

                reader.Close();
			}
            return string.Empty;
        }

        #endregion

        #region GetUrl
        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <param name="cmdName">Name of the CMD.</param>
        /// <returns></returns>
        public static string GetUrl(string cmdName)
        {
            return GetUrl(cmdName, new List<String>(0), new List<String>(0));
        }

        /// <summary>
        /// Gets the URL. Results are cached using MenuTimeout setting.
        /// </summary>
        /// <param name="cmdName">Name of the CMD.</param>
        /// <param name="pars">The pars.</param>
        /// <param name="vals">The vals.</param>
        /// <returns></returns>
        public static string GetUrl(string cmdName, List<string> pars, List<string> vals)
        {
            string cacheKey = CmsCache.CreateCacheKey("commands", cmdName, MakeParams(pars, vals));

            // check cache first
            object cachedObject = CmsCache.Get(cacheKey);

            if (cachedObject != null)
                return (string)cachedObject;

			using (IDataReader reader = GetItemByName(cmdName))
			{
				if (reader.Read())
				{
                    if (!CheckParams((int)reader["ItemId"], pars))
                    {
                        reader.Close();
                        throw new Exception("Some parameters for NavigationCommand = " + cmdName + " are invalid");
                    }

					using (IDataReader reader2 = GetCommandByItemId((int)reader["ItemId"]))
					{
						if (reader2.Read())
						{
                            if (!String.IsNullOrEmpty(reader2["Params"] as string) && !String.IsNullOrEmpty(MakeParams(pars, vals)))
                            {
                                string url = GetUrlByUID((string)reader2["UrlUID"], MakeParams(pars, vals) + "&" + (string)reader2["Params"]);
                                reader2.Close();
                                reader.Close();
                                CmsCache.Insert(cacheKey, url, CmsConfiguration.Instance.Cache.MenuTimeout);
                                return url;
                            }
                            else
                            {
                                string url = GetUrlByUID((string)reader2["UrlUID"], MakeParams(pars, vals));
                                reader2.Close();
                                reader.Close();
                                CmsCache.Insert(cacheKey, url, CmsConfiguration.Instance.Cache.MenuTimeout);
                                return url;
                            }
						}

                        reader2.Close();
					}
				}

                reader.Close();
			}

            CmsCache.Insert(cacheKey, String.Empty, CmsConfiguration.Instance.Cache.MenuTimeout);
            return String.Empty;
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <param name="cmdName">Name of the CMD.</param>
        /// <param name="pars">The pars.</param>
        /// <param name="vals">The vals.</param>
        /// <param name="versionId">The version id.</param>
        /// <returns></returns>
        public static string GetUrl(string cmdName, List<string> pars, List<string> vals, int versionId)
        {
			using (IDataReader reader = GetItemByName(cmdName))
			{
				if (reader.Read())
				{
                    if (!CheckParams((int)reader["ItemId"], pars))
                    {
                        reader.Close();
                        throw new Exception("Some parameters for NavigationCommand = " + cmdName + " are invalid");
                    }

					using (IDataReader reader2 = GetCommandByItemId((int)reader["ItemId"]))
					{
						if (reader2.Read())
						{
							string url = GetUrlByVersionId(versionId, MakeParams(pars, vals) + "&" + (string)reader2["Params"]);
                            reader2.Close();
                            return url;
						}
					}
				}

                reader.Close();
			}

            return string.Empty;
        }

        /// <summary>
        /// Gets the URL.
        /// </summary>
        /// <param name="cmdName">Name of the CMD.</param>
        /// <param name="paramsValues">The params values.</param>
        /// <returns></returns>
		public static string GetUrl(string cmdName, params object[] paramsValues)
		{
			if (paramsValues == null)
				return GetUrl(cmdName);
			else
			{
				if (paramsValues.GetLength(0) % 2 != 0)
					throw new ArgumentException("Number of parameters and values must be identical");

				List<string> _params = new List<string>(paramsValues.GetLength(0) / 2);
                List<string> _value = new List<string>(paramsValues.GetLength(0) / 2);

				for (int i = 0; i < paramsValues.GetLength(0); i++)
				{
					if (i % 2 == 0)
					{
						_params.Add(paramsValues.GetValue(i).ToString());
					}
					else
					{
                        _value.Add(paramsValues.GetValue(i).ToString());
					}
				}

				return GetUrl(cmdName, _params, _value);
			}
		}


        #endregion

        #region CheckParams
        /// <summary>
        /// Checks all required the params.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="pars">The parameters.</param>
        /// <returns></returns>
        private static bool CheckParams(int itemId, List<string> pars)
        {
            //select * from [NavigationParams] where [ItemdId] = 'ItemId'
            DataView dv = GetParamsByItemIdDT(itemId).DefaultView;
			for (int i = 0; i < pars.Count; i++)
            {
				dv.RowFilter = "Name = '" + pars[i] + "'";
				if (dv.Count == 0) throw new Exception("Unknown parameter " + pars[i]);
            }

            //select * from [NavigationParams] where [IsRequred] = 1 and [ItemdId] = 'ItemId'
            dv.RowFilter = "IsRequired = 1";
            foreach (DataRowView row in dv)
            {
				if (!pars.Contains(row["Name"].ToString()))
                {
                    return false;
                }
            }
            return true;
        } 
        #endregion

        #region MakeParams
        /// <summary>
        /// Makes the params.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
		private static string MakeParams(List<string> parameters, List<string> values)
		{
			if (parameters.Count != values.Count)
				throw new Exception("NavigationManager: Number of \"Parameters\" doesn't match number of \"Values\". The navigation (menu) is not configured correctly.");

			string str = String.Empty;

			for (int i = 0; i < parameters.Count; i++)
			{
				if (!String.IsNullOrEmpty(str))
					str += "&";
				else
					str = "?";
				str += String.Format("{0}={1}", parameters[i], values[i]);
			}
			return str;
		}
        #endregion
    }
}
