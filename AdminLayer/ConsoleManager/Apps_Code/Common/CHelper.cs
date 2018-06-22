using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;

namespace Mediachase.Ibn.Web.UI
{
	public class CHelper
	{
		protected readonly static int timeOutInterval = 1000;
		public readonly static string NeedToDataBindKey = "NeedToDataBind";
		public readonly static string NeedToBindGridKey = "GridBindFlag";

		#region GetUserName
        /// <summary>
        /// Gets the name of the user.
        /// </summary>
        /// <param name="UserID">The user ID.</param>
        /// <returns></returns>
		public static string GetUserName(int UserID)
		{
			if (UserID <= 0)
				return "Unknown User";

			try
			{
				MetaObject pc = MetaObjectActivator.CreateInstance("Principal", UserID);
				return pc.Properties[MetaDataWrapper.GetMetaClassByName("Principal").TitleFieldName].Value.ToString();
				//Principal pc = new Principal(UserID);
				//return pc.Name;
			}
			catch { }

			return "Unknown User";
		}
		#endregion

		#region GetObjectTitle
        /// <summary>
        /// Gets the object title.
        /// </summary>
        /// <param name="ObjectTypeId">The object type id.</param>
        /// <param name="ObjectId">The object id.</param>
        /// <returns></returns>
		public static string GetObjectTitle(int ObjectTypeId, int ObjectId)
		{
			string retval = "";
			switch (ObjectTypeId)
			{
				case 1:
					retval = GetUserName(ObjectId);
					break;
				default:
					break;
			}

			return retval;
		}
		#endregion

		#region GetObjectHTMLTitle
        /// <summary>
        /// Gets the object HTML title.
        /// </summary>
        /// <param name="ObjectTypeId">The object type id.</param>
        /// <param name="ObjectId">The object id.</param>
        /// <returns></returns>
		public static string GetObjectHTMLTitle(int ObjectTypeId, int ObjectId)
		{
			string retval = "";
			switch (ObjectTypeId)
			{
				case 1:
					//retval = GetUserStatusUL(ObjectId);
					break;
				default:
					break;
			}

			return retval;
		}
		#endregion

		#region GetAbsolutePath
        /// <summary>
        /// Gets the absolute path.
        /// </summary>
        /// <param name="xs_Path">The XS_ path.</param>
        /// <returns></returns>
		public static string GetAbsolutePath(string xs_Path)
		{
			string UrlScheme = System.Configuration.ConfigurationManager.AppSettings["UrlScheme"];

			StringBuilder builder = new StringBuilder();
			if (UrlScheme != null)
				builder.Append(UrlScheme);
			else
				builder.Append(HttpContext.Current.Request.Url.Scheme);
			builder.Append("://");

			// Oleg Rylin: Fixing the problem with non-default port [6/20/2006]
			builder.Append(HttpContext.Current.Request.Url.Authority);

			builder.Append(HttpContext.Current.Request.ApplicationPath);
            
            if (builder[builder.Length - 1] != '/')
                    builder.Append("/");

			if (xs_Path != string.Empty)
			{
				if (xs_Path[0] == '/')
					xs_Path = xs_Path.Substring(1, xs_Path.Length - 1);
				builder.Append(xs_Path);
			}
			return builder.ToString();
		}
		#endregion

		#region GetResFileString
        /// <summary>
        /// Gets the res file string.
        /// </summary>
        /// <param name="ResFileKey">The res file key.</param>
        /// <returns></returns>
		public static string GetResFileString(string ResFileKey)//for {file:key} values
		{
			string sTemp = ResFileKey;
			if (sTemp.StartsWith("{") && sTemp.EndsWith("}"))
			{
				sTemp = sTemp.Substring(1, sTemp.Length - 2);
				string fileName = "Global";
				if (sTemp.IndexOf(":") >= 0)
				{
					fileName = sTemp.Substring(0, sTemp.IndexOf(":"));
					sTemp = sTemp.Substring(sTemp.IndexOf(":") + 1);
				}
				try
				{
					return HttpContext.GetGlobalResourceObject(fileName, sTemp).ToString();
				}
				catch
				{
					return ResFileKey;
				}
			}
			else
				return ResFileKey;
		}
		#endregion

		#region SafeSelect
        /// <summary>
        /// Safes the select.
        /// </summary>
        /// <param name="ddl">The DDL.</param>
        /// <param name="val">The val.</param>
		public static void SafeSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
			{
				ddl.ClearSelection();
				li.Selected = true;
			}
		}
		#endregion

		#region SafeMultipleSelect
        /// <summary>
        /// Safes the multiple select.
        /// </summary>
        /// <param name="ddl">The DDL.</param>
        /// <param name="val">The val.</param>
		public static void SafeMultipleSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
				li.Selected = true;
		}
		#endregion

		#region GetFullPageTitle
        /// <summary>
        /// Gets the full page title.
        /// </summary>
        /// <param name="Title">The title.</param>
        /// <returns></returns>
		public static string GetFullPageTitle(string Title)
		{
			if (Title.Length > 0)
				return Title + " | Instant Business Network 5.0";
			else
				return "Instant Business Network 5.0";
		}
		#endregion

		#region CloseItAndRefresh
        /// <summary>
        /// Closes it and refresh.
        /// </summary>
        /// <param name="response">The response.</param>
		public static void CloseItAndRefresh(HttpResponse response)
		{
			response.Clear();
			response.Write("<script language=\"javascript\" type=\"text/javascript\">");
			response.Write("try {window.opener.document.forms[0].submit();} catch (e) {}");
			response.Write("window.close()");
			response.Write("</script>");
			response.End();
		}
		#endregion

		#region CloseItAndRefresh
        /// <summary>
        /// Closes it and refresh.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="RefreshButton">The refresh button.</param>
		public static void CloseItAndRefresh(HttpResponse response, string RefreshButton)
		{
			response.Clear();
			response.Write("<script language=\"javascript\" type=\"text/javascript\">");
			response.Write(String.Format(CultureInfo.InvariantCulture, "try {{window.opener.{0};}} catch (e) {{}}", RefreshButton));
			response.Write(String.Format(CultureInfo.InvariantCulture, "setTimeout('window.close()', {0});", timeOutInterval));
			response.Write("</script>");
			response.End();
		}
		#endregion

		#region GetCloseRefreshString
        /// <summary>
        /// Gets the close refresh string.
        /// </summary>
        /// <param name="RefreshButton">The refresh button.</param>
        /// <returns></returns>
		public static string GetCloseRefreshString(string RefreshButton)
		{
			string retVal = String.Format(CultureInfo.InvariantCulture, "try {{window.opener.{0};}} catch (e) {{}}", RefreshButton);
			retVal += String.Format(CultureInfo.InvariantCulture, "setTimeout('window.close()', {0}); return false;", timeOutInterval);
			return retVal;
		}
		#endregion

		#region AddToContext
        /// <summary>
        /// Adds to context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
		public static void AddToContext(string key, object value)
		{
			if (HttpContext.Current.Items.Contains(key))
				HttpContext.Current.Items[key] = value;
			else
				HttpContext.Current.Items.Add(key, value);
		}
		#endregion

		#region GetFromContext
        /// <summary>
        /// Gets from context.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public static object GetFromContext(string key)
		{
			if (HttpContext.Current.Items.Contains(key))
				return HttpContext.Current.Items[key];
			else
				return null;
		}
		#endregion

		#region RemoveFromContext
        /// <summary>
        /// Removes from context.
        /// </summary>
        /// <param name="key">The key.</param>
		public static void RemoveFromContext(string key)
		{
			if (HttpContext.Current.Items.Contains(key))
				HttpContext.Current.Items.Remove(key);
		}
		#endregion

		#region GetMetaTypeName
        /// <summary>
        /// Gets the name of the meta type.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
		public static string GetMetaTypeName(MetaField field)
		{
			string typeName = "";
			if (field.IsMultivalueEnum)
				typeName = "EnumMultiValue";
			else if (field.IsEnum)
				typeName = "Enum";
			else if (field.GetMetaType().McDataType == McDataType.MultiReference)
				typeName = "MultiReference";
			else
				typeName = field.GetMetaType().Name;
			return typeName;
		}
		#endregion

		#region UpdateParentPanel
        /// <summary>
        /// Updates the parent panel.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
		public static void UpdateParentPanel(Control startPoint)
		{
			Control c = startPoint;
			do
			{
				c = c.Parent;
				if (c is UpdatePanel)
				{
					((UpdatePanel)c).Update();
					break;
				}
			} while (c != startPoint.Page);
		}
		#endregion

		#region UpdateAllParentPanels
        /// <summary>
        /// Updates all parent panels.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
		public static void UpdateAllParentPanels(Control startPoint)
		{
			Control c = startPoint;
			do
			{
				c = c.Parent;
				if (c is UpdatePanel)
				{
					((UpdatePanel)c).Update();
				}
			} while (c != startPoint.Page);
		}
		#endregion

		#region CheckCardField
        /// <summary>
        /// Checks the card field.
        /// </summary>
        /// <param name="_class">The _class.</param>
        /// <param name="cardField">The card field.</param>
        /// <returns></returns>
		public static bool CheckCardField(MetaClass _class, MetaField cardField)
		{
			string CardPKeyName = string.Format(CultureInfo.InvariantCulture, "{0}Id", cardField.Owner.Name);
			string CardRefKeyName = string.Format(CultureInfo.InvariantCulture, "{0}Id", _class.Name);
			return (cardField.Name != CardRefKeyName &&
					cardField.Name != CardPKeyName &&
					!(cardField.GetOriginalMetaType().McDataType == McDataType.ReferencedField &&
					cardField.Attributes.GetValue<string>(McDataTypeAttribute.ReferencedFieldMetaClassName) == _class.Name)
					);
		}
		#endregion

		#region GetAllMetaFields
        /// <summary>
        /// Gets all meta fields.
        /// </summary>
        /// <param name="View">The view.</param>
        /// <returns></returns>
		public static List<MetaField> GetAllMetaFields(MetaView View)
		{
			List<MetaField> retVal = new List<MetaField>();
			foreach (MetaField field in View.MetaClass.Fields)
			{
				retVal.Add(field);
			}

			if (View.Card != null)
			{
				MetaClass mcCard = MetaDataWrapper.GetMetaClassByName(View.Card.Name);
				foreach (MetaField field in mcCard.Fields)
				{
					if (CHelper.CheckCardField(View.MetaClass, field))
						retVal.Add(field);
				}
			}

			return retVal;
		}
		#endregion

		#region CreateDefaultUserPreference
        /// <summary>
        /// Creates the default user preference.
        /// </summary>
        /// <param name="View">The view.</param>
		public static void CreateDefaultUserPreference(Mediachase.Ibn.Data.Meta.Management.MetaView View)
		{
			McMetaViewPreference pref = new McMetaViewPreference();
			pref.MetaView = View;

			foreach (Mediachase.Ibn.Data.Meta.Management.MetaField field in View.MetaClass.Fields)
			{
				pref.SetAttribute(field.Name, McMetaViewPreference.AttrWidth, 150);
			}

			pref.Attributes.Set("MarginTop", 10);
			pref.Attributes.Set("MarginLeft", 10);
			pref.Attributes.Set("MarginRight", 10);
			pref.Attributes.Set("MarginBottom", 10);
			Mediachase.Ibn.Core.UserMetaViewPreference.Save((int)DataContext.Current.CurrentUserId, pref);
		}
		#endregion

		#region CreateDefaultPreference
        /// <summary>
        /// Creates the default preference.
        /// </summary>
        /// <param name="View">The view.</param>
		public static void CreateDefaultPreference(MetaView View)
		{
			McMetaViewPreference pref = new McMetaViewPreference();
			pref.MetaView = View;

			foreach (MetaField field in View.MetaClass.Fields)
			{
				pref.SetAttribute(field.Name, McMetaViewPreference.AttrWidth, 150);
			}

			pref.Attributes.Set("MarginTop", 10);
			pref.Attributes.Set("MarginLeft", 10);
			pref.Attributes.Set("MarginRight", 10);
			pref.Attributes.Set("MarginBottom", 10);
			Mediachase.Ibn.Core.UserMetaViewPreference.SaveDefault(pref);
		}
		#endregion

		#region GetLinkObjectView
        /// <summary>
        /// Gets the link object view.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="objectId">The object id.</param>
        /// <returns></returns>
		public static string GetLinkObjectView(string className, string objectId)
		{
			return String.Format(CultureInfo.InvariantCulture, "~/TestFolder/ObjectView.aspx?ClassName={0}&ObjectId={1}", className, objectId);
		}
		#endregion

		#region GetLinkMetaViewShow
        /// <summary>
        /// Gets the link meta view show.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <returns></returns>
		public static string GetLinkMetaViewShow(string viewName)
		{
			return String.Format(CultureInfo.InvariantCulture, "~/Apps/MetaUI/Pages/Admin/MetaViewShow.aspx?ViewName={0}", viewName);
		}
		#endregion

		#region GetPermissionIconPath
        /// <summary>
        /// Gets the permission icon path.
        /// </summary>
        /// <param name="rightValue">The right value.</param>
        /// <returns></returns>
		public static string GetPermissionIconPath(int rightValue)
		{
			return GetPermissionIconPath(rightValue, false);
		}

        /// <summary>
        /// Gets the permission icon path.
        /// </summary>
        /// <param name="rightValue">The right value.</param>
        /// <param name="isInhereted">if set to <c>true</c> [is inhereted].</param>
        /// <returns></returns>
		public static string GetPermissionIconPath(int rightValue, bool isInhereted)
		{
			string path;
			path = GetAbsolutePath("/Images/Blank.gif");
			if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Allow)
				path = GetAbsolutePath("/Images/Shield-Green-Tick.png");
			else if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Forbid)
				path = GetAbsolutePath("/Images/Shield-Red-Cross.png");
			return path;
		}
		#endregion

		#region GetPermissionImage
        /// <summary>
        /// Gets the permission image.
        /// </summary>
        /// <param name="rightValue">The right value.</param>
        /// <returns></returns>
		public static string GetPermissionImage(int rightValue)
		{
			return GetPermissionImage(rightValue, false);
		}

        /// <summary>
        /// Gets the permission image.
        /// </summary>
        /// <param name="rightValue">The right value.</param>
        /// <param name="isInhereted">if set to <c>true</c> [is inhereted].</param>
        /// <returns></returns>
		public static string GetPermissionImage(int rightValue, bool isInhereted)
		{
			string toolTip;
			if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Allow)
				toolTip = HttpContext.GetGlobalResourceObject("Security", "PermissionAllow").ToString();
			else if (rightValue == (int)Mediachase.Ibn.Data.Services.Security.Rights.Forbid)
				toolTip = HttpContext.GetGlobalResourceObject("Security", "PermissionForbid").ToString();
			else
				toolTip = HttpContext.GetGlobalResourceObject("Security", "PermissionNone").ToString();

			return String.Format(CultureInfo.InvariantCulture,
				"<img src='{0}' width='16' height='16' alt='{1}' align='absmiddle'/>",
				GetPermissionIconPath(rightValue, isInhereted),
				toolTip);
		}
		#endregion

		#region GetEventResourceString
        /// <summary>
        /// Gets the event resource string.
        /// </summary>
        /// <param name="eventObject">The event object.</param>
        /// <returns></returns>
		public static string GetEventResourceString(MetaObject eventObject)
		{
			string retVal = GetResFileString(eventObject.Properties["EventTitle"].Value.ToString());
			//{event:...}
			MatchCollection coll = Regex.Matches(retVal, "{event:(?<EventProp>[^}]*)}");
			foreach (Match match in coll)
			{
				string sArg = match.Groups["EventProp"].Value;
				retVal = retVal.Replace(match.ToString(), GetResFileString(eventObject.Properties[sArg].Value.ToString()));
			}
			//{args:...}
			if (eventObject.Properties["ArgumentType"].Value != null &&
				eventObject.Properties["ArgumentData"].Value != null)
			{
				string argumentType = eventObject.Properties["ArgumentType"].Value.ToString();
				string argumentData = eventObject.Properties["ArgumentData"].Value.ToString();
				MatchCollection argscoll = Regex.Matches(retVal, "{args:(?<EventArg>[^}]*)}");
				if (argscoll.Count > 0)
				{
					Type objType = Mediachase.Ibn.Data.AssemblyUtil.LoadType(argumentType);
					object obj = McXmlSerializer.GetObject(objType, argumentData);
					if (obj != null)
					{
						foreach (Match match in argscoll)
						{
							string p_name = match.Groups["EventArg"].Value;
							PropertyInfo pinfo = objType.GetProperty(p_name);
							if (pinfo != null)
							{
								string sTemp = pinfo.GetValue(obj, null).ToString();

								retVal = retVal.Replace(match.ToString(), GetResFileString(sTemp));
							}
						}
					}
				}
			}
			return retVal;
		}
		#endregion

		#region GetWeekStartByDate
        /// <summary>
        /// Gets the week start by date.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <returns></returns>
		public static DateTime GetWeekStartByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = (int)FirstDayOfWeek.Monday;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);

			if (result.Year < start.Year)
				result = new DateTime(start.Year, 1, 1);

			return result;
		}
		#endregion

		#region GetRealWeekStartByDate
		public static DateTime GetRealWeekStartByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = (int)FirstDayOfWeek.Monday;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);

			return result;
		}
		#endregion

		#region GetWeekEndByDate
		public static DateTime GetWeekEndByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = (int)FirstDayOfWeek.Monday;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);
			result = result.AddDays(6);
			if (result.Year > start.Year)
				return new DateTime(start.Year, 12, 31);
			else
				return result;
		}
		#endregion

		#region GetRealWeekEndByDate
		public static DateTime GetRealWeekEndByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = (int)FirstDayOfWeek.Monday;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);
			result = result.AddDays(6);
			return result;
		}
		#endregion

		#region ParseText
		/// <summary>
		/// Parses the text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static string ParseText(string text)
		{
			return ParseText(text, false, false, false);
		}

		/// <summary>
		/// Parses the text.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="preserveWhiteSpace">if set to <c>true</c> [preserve white space].</param>
		/// <param name="preserveLineBreaks">if set to <c>true</c> [preserve line breaks].</param>
		/// <param name="preserveHtmlTags">if set to <c>true</c> [preserve HTML tags].</param>
		/// <returns></returns>
		public static string ParseText(string text, bool preserveWhiteSpace, bool preserveLineBreaks, bool preserveHtmlTags)
		{
			StringBuilder sb = new StringBuilder(text);
			if (preserveWhiteSpace)
				sb.Replace("  ", " &nbsp;");
			if (!preserveHtmlTags)
			{
				sb.Replace("<", "&lt;");
				sb.Replace(">", "&gt;");
				sb.Replace("\"", "&quot;");
			}
			string resultString = sb.ToString();
			if (preserveLineBreaks)
			{
				StringReader sr = new StringReader(resultString);
				StringWriter sw = new StringWriter();
				while (sr.Peek() > -1)
				{
					string temp = sr.ReadLine();
					sw.Write(temp + "<br>");
				}
				resultString = sw.GetStringBuilder().ToString();
			}
			return resultString;
		}
		#endregion

		#region GetParentControl
		public static Control GetParentControl(Control startPoint, Type type, string clientId)
		{
			Control retVal = null;
			Control c = startPoint;
			do
			{
				c = c.Parent;
				if (c.GetType() == type && c.ClientID == clientId)
				{
					retVal = c;
					break;
				}
			} while (c != startPoint.Page);
			return retVal;
		}
		#endregion		

		#region GetMetaFieldName
		public static string GetMetaFieldName(MetaField field)
		{
			string name = GetResFileString(field.FriendlyName);
			if (field.IsReference)
				name += " (ref)";
			return name;
		}
		#endregion

		#region GetMcDataTypeName
		public static string GetMcDataTypeName(McDataType mcDataType)
		{
			return GetMcDataTypeName(mcDataType.ToString());
		}

		public static string GetMcDataTypeName(string mcDataType)
		{
			string key = String.Format(CultureInfo.InvariantCulture, "McDataType_{0}", mcDataType);
			return HttpContext.GetGlobalResourceObject("GlobalMetaInfo", key).ToString();
		}
		#endregion

		#region NeedToDataBind
		public static bool NeedToDataBind()
		{
			bool retval = false;
			object needtodatabind = GetFromContext(NeedToDataBindKey);
			if (needtodatabind != null && needtodatabind.ToString() == "true")
			{
				retval = true;
			}
			return retval;
		}
		#endregion

		#region RequireDataBind
		public static void RequireDataBind()
		{
			AddToContext(NeedToDataBindKey, "true");
		}

		public static void RequireDataBind(bool value)
		{
			if (value)
			{
				AddToContext(NeedToDataBindKey, "true");
			}
			else if (NeedToDataBind())
			{
				RemoveFromContext(NeedToDataBindKey);
			}
		}
		#endregion

		#region NeedToBindGrid
		public static bool NeedToBindGrid()
		{
			bool retval = false;
			object needtobindgrid = GetFromContext(NeedToBindGridKey);
			if (needtobindgrid != null && (needtobindgrid.ToString() == "1" || needtobindgrid.ToString() == "true"))
			{
				retval = true;
			}
			return retval;
		}
		#endregion

		#region RequireBindGrid
		public static void RequireBindGrid()
		{
			AddToContext(NeedToBindGridKey, 1);
		}

		public static void RequireBindGrid(bool value)
		{
			if (value)
			{
				AddToContext(NeedToBindGridKey, 1);
			}
			else if (NeedToBindGrid())
			{
				RemoveFromContext(NeedToBindGridKey);
			}
		}
		#endregion

		#region GetFormName
		public static string GetFormName(string systemFormName)
		{
			string retval = systemFormName;
			switch (systemFormName)
			{
				case FormController.BaseFormType:
					retval = Resources.MetaForm.Form_Edit;
					break;
				case FormController.CreateFormType:
					retval = Resources.MetaForm.Form_Create;
					break;
				case FormController.GeneralViewFormType:
					retval = Resources.MetaForm.Form_View;
					break;
				case FormController.ShortViewFormType:
					retval = Resources.MetaForm.Form_ShortInfo;
					break;
				case FormController.PublicEditFormType:
					retval = Resources.MetaForm.Form_PublicInfo;
					break;
				case FormController.CustomFormType:
					retval = Resources.MetaForm.CustomForm;
					break;
				default:
					break;
			}

			return retval;
		}
		#endregion

		#region GetDateDiffInSeconds
		/// <summary>
		/// Gets the date diff in seconds.
		/// </summary>
		/// <param name="dt1">The date1.</param>
		/// <param name="dt2">The date2.</param>
		/// <returns></returns>
		public static int GetDateDiffInSeconds(DateTime dt1, DateTime dt2)
		{
			TimeSpan ts;
			if (dt1 > dt2)
				ts = dt1.Subtract(dt2);
			else
				ts = dt2.Subtract(dt1);

			return (int)ts.TotalSeconds;
		}
		#endregion

		#region LoadExtJsGridScripts
		/// <summary>
		/// Loads the ext js grid scripts.
		/// </summary>
		/// <param name="p">The p.</param>
		public static void LoadExtJsGridScripts(Page p)
		{
			//ScriptManager.GetCurrent(p).Scripts.Add(new ScriptReference("~/Scripts/IbnFramework/ext-base.js"));
			ScriptManager.GetCurrent(p).Scripts.Add(new ScriptReference("~/Apps/MetaDataBase/Scripts/ext-all.js"));

			p.ClientScript.RegisterClientScriptBlock(p.GetType(), Guid.NewGuid().ToString(),
				String.Format("<link type='text/css' rel='stylesheet' href='{0}' />", p.ResolveClientUrl("~/Apps/MetaDataBase/styles/ext-all2.css")));

		}
		#endregion

        /// <summary>
        /// Gets the link object view_ edit.
        /// </summary>
        /// <param name="sReferencedClass">The s referenced class.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static string GetLinkObjectView_Edit(string sReferencedClass, string p)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the icon.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string GetIcon(string fileName)
        {
            string extension = String.Empty;
            if (fileName.IndexOf(".") > 0)
                extension = fileName.Substring(fileName.LastIndexOf("."), fileName.Length - fileName.LastIndexOf(".")).ToLower();
            else
                extension = fileName.ToLower();

			string baseImagesFolder = "~/Apps/Asset/Images/FileType/";

            switch (extension)
            {
                case ".ai":
					return String.Concat(baseImagesFolder, "ai.gif");
                case ".avi":
                case ".wmv":
					return String.Concat(baseImagesFolder, "avi.gif");
                case ".bmp":
					return String.Concat(baseImagesFolder, "bmp.gif");
                case ".cs":
					return String.Concat(baseImagesFolder, "cs.gif");
                case ".dll":
					return String.Concat(baseImagesFolder, "dll.gif");
                case ".doc":
					return String.Concat(baseImagesFolder, "doc.gif");
                case ".exe":
					return String.Concat(baseImagesFolder, "exe.gif");
                case ".fla":
					return String.Concat(baseImagesFolder, "fla.gif");
                case ".htm":
					return String.Concat(baseImagesFolder, "htm.gif");
                case ".html":
					return String.Concat(baseImagesFolder, "html.gif");
                case ".jpg":
                case ".jpeg":
					return String.Concat(baseImagesFolder, "jpg.gif");
                case ".js":
					return String.Concat(baseImagesFolder, "js.gif");
                case ".mdb":
					return String.Concat(baseImagesFolder, "mdb.gif");
                case ".mp3":
					return String.Concat(baseImagesFolder, "mp3.gif");
                case ".pdf":
					return String.Concat(baseImagesFolder, "pdf.gif");
                case ".ppt":
					return String.Concat(baseImagesFolder, "ppt.gif");
                case ".rdp":
					return String.Concat(baseImagesFolder, "rdp.gif");
                case ".swf":
					return String.Concat(baseImagesFolder, "swf.gif");
                case ".swt":
					return String.Concat(baseImagesFolder, "swt.gif");
                case ".txt":
					return String.Concat(baseImagesFolder, "txt.gif");
                case ".vsd":
					return String.Concat(baseImagesFolder, "vsd.gif");
                case ".xls":
					return String.Concat(baseImagesFolder, "xls.gif");
                case ".xml":
					return String.Concat(baseImagesFolder, "xml.gif");
                case ".rar":
                case ".zip":
					return String.Concat(baseImagesFolder, "zip.gif");
                default:
					return String.Concat(baseImagesFolder, "default.icon.gif");
            }
        }

		/// <summary>
		/// Creates the authentication cookie.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="domain">The domain.</param>
		/// <param name="remember">if set to <c>true</c> [remember].</param>
		public static void CreateAuthenticationCookie(string username, string domain, bool remember)
		{
			// this line is needed for cookieless authentication
			FormsAuthentication.SetAuthCookie(username, remember);
			DateTime expirationDate = FormsAuthentication.GetAuthCookie(username, remember).Expires;

			// the code below does not work for cookieless authentication

			// we need to handle ticket ourselves since we need to save session paremeters as well
			FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2,
					username,
					DateTime.Now,
				/*expirationDate, - doesn't work when it's DateTime.MinValue. The date needs to be convertible to FileTime, i.e. >=01/01/1601 */
					expirationDate == DateTime.MinValue ? DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout) : expirationDate,
					remember,
					domain,
					FormsAuthentication.FormsCookiePath);

			// Encrypt the ticket.
			string encTicket = FormsAuthentication.Encrypt(ticket);

			// remove the cookie, if one already exists with the same cookie name
			if (HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName] != null)
				HttpContext.Current.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);

			HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
			cookie.HttpOnly = true;

			cookie.Path = FormsAuthentication.FormsCookiePath;
			cookie.Secure = FormsAuthentication.RequireSSL;
			if (FormsAuthentication.CookieDomain != null)
				cookie.Domain = FormsAuthentication.CookieDomain;

			if (ticket.IsPersistent)
				cookie.Expires = ticket.Expiration;

			// Create the cookie.
			HttpContext.Current.Response.Cookies.Set(cookie);
		}
	}
}
