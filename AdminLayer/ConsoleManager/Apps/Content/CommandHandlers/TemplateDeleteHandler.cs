using System;
using System.Data;
using System.Collections.Specialized;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Web.Console.Common;
using Mediachase.Web.Console.Controls;
using Mediachase.Cms;
using Mediachase.Cms.Dto;
using Mediachase.Cms.Managers;
using System.Data.SqlClient;

namespace Mediachase.Commerce.Manager.Content.CommandHandlers
{
	public class TemplateDeleteHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object sender, object element)
		{
			if (element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)element;

				string gridId = cp.CommandArguments["GridId"];
				string[] items = EcfListView.GetCheckedCollection(((Control)sender).Page, gridId);

				if (items != null)
				{
					int error = 0;
					string errorMessage = String.Empty;
					try
					{
						ProcessDeleteCommand(items);

						ManagementHelper.SetBindGridFlag(gridId);
					}
                    catch (SqlException sqlEx)
                    {
                        error++;
                        if (sqlEx.Number == 547)
                            errorMessage = Resources.ContentStrings.Site_Template_Delete_ReferenceException;
                        else
                            errorMessage = String.Concat(Resources.ContentStrings.Site_Template_Delete_TitleException, sqlEx.Message);
                    }
					catch (Exception ex)
					{
						error++;
						errorMessage = String.Concat(Resources.ContentStrings.Site_Template_Delete_TitleException, ex.Message);
					}

					if (error > 0)
					{
						errorMessage = errorMessage.Replace("'", "\\'").Replace(Environment.NewLine, "\\n");
						ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
							String.Format("alert('{0}');", errorMessage), true);
					}
				}
				else
					return;
			}
		}
		#endregion

		void ProcessDeleteCommand(string[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				string[] keys = EcfListView.GetPrimaryKeyIdStringItems(items[i]);
				if (keys != null)
				{
					int id = Int32.Parse(keys[0]);

					// delete selected items
					TemplateDto templateDto = DictionaryManager.GetTemplateDto(id);
					if (templateDto.main_Templates.Count > 0)
					{
						templateDto.main_Templates.Rows[0].Delete();
						DictionaryManager.SaveTemplateDto(templateDto);
					}
				}
			}
		}
	}
}