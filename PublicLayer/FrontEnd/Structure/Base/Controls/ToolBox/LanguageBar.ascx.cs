namespace Mediachase.Cms.Web
{
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

    using mc = Mediachase.Cms;

    public partial class ToolBox_LanguageBar : System.Web.UI.UserControl
    {
        #region CurrentLanguageId Property
        private int currentLanguageId = 1;
        /// <summary>
        /// Gets or sets the current language id.
        /// </summary>
        /// <value>The current language id.</value>
        public int CurrentLanguageId
        {
            get
            {
                return currentLanguageId;
            }
            set
            {
                currentLanguageId = value;
            }
        }
        #endregion

        #region AvaliableLanguages Property
        private ArrayList avalLang = null;
        /// <summary>
        /// Gets or sets the avaliable language.
        /// </summary>
        /// <value>The avaliable language.</value>
        public ArrayList AvaliableLanguage
        {
            get
            {
                return avalLang;
            }
            set
            {
                avalLang = value;
            }
        }
        #endregion

        #region DisableUnavaliable Property
        private bool disableUnavaliable = false;
        /// <summary>
        /// Gets or sets a value indicating whether [disable unavaliable].
        /// </summary>
        /// <value><c>true</c> if [disable unavaliable]; otherwise, <c>false</c>.</value>
        public bool DisableUnavaliable
        {
            get { return disableUnavaliable; }
            set { disableUnavaliable = value; }
        }
        #endregion

        #region HighlightCurrentLanguage()
        /// <summary>
        /// Highlights the current language.
        /// </summary>
        protected void HighlightCurrentLanguage()
        {
            for (int i = 0; i < LanguageBar.Rows[0].Cells.Count; i++)
            {
                LanguageBar.Rows[1].Cells[i].BorderWidth = 0;
            }
            for (int i = 0; i < LanguageBar.Rows[0].Cells.Count; i++)
            {
                if (LanguageBar.Rows[0].Cells[i].Text == CurrentLanguageId.ToString())
                {
                    LanguageBar.Rows[1].Cells[i].BorderWidth = 2;
                    LanguageBar.Rows[1].Cells[i].BorderColor = System.Drawing.Color.Gold;
                }
            }
        }
        #endregion

        #region BindToolBar()
        /// <summary>
        /// Binds the toolbar.
        /// </summary>
        public void BindToolbar()
        {
            LanguageBar.Rows.Clear();
            TableRow newRow1 = new TableRow();
            TableRow newRow2 = new TableRow();
            using (IDataReader reader = mc.Language.GetAllLanguages())
            {
                while (reader.Read())
                {
                    TableCell newCell1 = new TableCell();
                    TableCell newCell2 = new TableCell();
                    newCell1.Text = reader["LangId"].ToString();
                    newCell1.Visible = false;
                    newRow1.Cells.Add(newCell1);
                    //newCell.Text = reader["LangName"].ToString();
                    ImageButton imgBtn = new ImageButton();
                    if (DisableUnavaliable)
                    {
                        if (AvaliableLanguage != null)
                            if (AvaliableLanguage.Contains(reader["LangId"]))
                                imgBtn.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsoluteThemedPath("/images/" + reader["LangName"].ToString() + ".gif", Page.Theme);
                            else
                                imgBtn.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsoluteThemedPath("/images/" + reader["LangName"].ToString() + "_gray.gif", Page.Theme);
                        else
                            imgBtn.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsoluteThemedPath("/images/" + reader["LangName"].ToString() + "_gray.gif", Page.Theme);
                    }
                    else
                    {
                        imgBtn.ImageUrl = Mediachase.Commerce.Shared.CommerceHelper.GetAbsoluteThemedPath("/images/" + reader["LangName"].ToString() + ".gif", Page.Theme);
                    }

                    imgBtn.Height = 12;
                    imgBtn.Width = 18;
                    imgBtn.BorderWidth = 1;
                    imgBtn.CommandArgument = reader["LangId"].ToString();
                    imgBtn.Click += new ImageClickEventHandler(imgBtn_Click);
                    imgBtn.CausesValidation = false;
                    newCell2.Controls.Add(imgBtn);
                    newRow2.Cells.Add(newCell2);
                }

                reader.Close();
            }
            LanguageBar.Rows.Add(newRow1);
            LanguageBar.Rows.Add(newRow2);
            HighlightCurrentLanguage();
        }

        #endregion

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            BindToolbar();
        }

        #region Events Handlers
        /// <summary>
        /// Handles the Click event of the imgBtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.ImageClickEventArgs"/> instance containing the event data.</param>
        void imgBtn_Click(object sender, ImageClickEventArgs e)
        {
            CurrentLanguageId = int.Parse(((ImageButton)sender).CommandArgument);
            HighlightCurrentLanguage();
        }
        #endregion
    }
}