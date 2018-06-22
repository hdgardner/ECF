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
    using System.Threading;

	using Mediachase.Commerce.Shared;

    using mc = Mediachase.Cms;
    using Mediachase.Cms.Util;
    using System.Globalization;

    public partial class ToolBar_LanguageBar : System.Web.UI.UserControl
    {
        #region Event
        public event EventHandler OnLanguageChange;
        #endregion

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
                using (IDataReader reader = mc.Language.GetLangByName(Thread.CurrentThread.CurrentCulture.Name))
                {
                    if (reader.Read())
                    {
                        currentLanguageId = (int)reader["LangId"];
                    }

                    reader.Close();
                }
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
                LanguageBar.Rows[1].Cells[i].Style.Add("border", "0px none black");
            }
            for (int i = 0; i < LanguageBar.Rows[0].Cells.Count; i++)
            {
                if (LanguageBar.Rows[0].Cells[i].Text == CurrentLanguageId.ToString())
                {
                    LanguageBar.Rows[1].Cells[i].BorderWidth = 2;
                    LanguageBar.Rows[1].Cells[i].BorderColor = System.Drawing.Color.Gold;
                    LanguageBar.Rows[1].Cells[i].Style.Add("border", "2px solid gold");
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
                    ImageButton imgBtn = new ImageButton();
                    CultureInfo culture = CultureInfo.CreateSpecificCulture(reader["LangName"].ToString());
                    if (DisableUnavaliable)
                    {
                        if (AvaliableLanguage != null)
                            if (AvaliableLanguage.Contains(reader["LangId"]))
                                imgBtn.ImageUrl = CommonHelper.GetFlagIcon(culture);//CommerceHelper.GetAbsolutePath("~/images/flags/" + reader["LangName"].ToString().Substring(0, 2) + ".gif");
                            else
                                imgBtn.ImageUrl = CommerceHelper.GetAbsolutePath("~/images/flags/" + reader["LangName"].ToString().Substring(0, 2) + "_gray.gif");
                        else
                            imgBtn.ImageUrl = CommerceHelper.GetAbsolutePath("~/images/flags/" + reader["LangName"].ToString().Substring(0, 2) + "_gray.gif");

                        //imgBtn.ImageUrl = CommonHelper.GetFlagIcon(culture);
                    }
                    else
                    {
                        //imgBtn.ImageUrl = CommonHelper.GetFlagIcon(culture);
                        imgBtn.ImageUrl = CommonHelper.GetFlagIcon(CultureInfo.CreateSpecificCulture(reader["LangName"].ToString())); // CommerceHelper.GetAbsolutePath("~/images/flags/" + reader["LangName"].ToString().Substring(0, 2) + ".gif");
                    }

                    imgBtn.AlternateText = culture.DisplayName;

                    imgBtn.Height = 12;
                    imgBtn.Width = 18;

                    if ((int)reader["LangId"] == CurrentLanguageId)
                    {
                        imgBtn.BorderWidth = 2;
                        imgBtn.BorderColor = System.Drawing.Color.Gold;
                    }
                    else
                    {
                        imgBtn.BorderWidth = 1;
                        imgBtn.BorderColor = System.Drawing.Color.Black;
                    }

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
            //HighlightCurrentLanguage();

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

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            string LanguageName = String.Empty;
            for (int i = 0; i < LanguageBar.Rows[1].Cells.Count; i++)
            {
                using (IDataReader reader = mc.Language.LoadLanguage(int.Parse(LanguageBar.Rows[0].Cells[i].Text)))
                {
                    if (reader.Read())
                    {
                        LanguageName = reader["LangName"].ToString();
                    }

                    reader.Close();
                }
                ImageButton btn = (ImageButton)LanguageBar.Rows[1].Cells[i].Controls[0];
                if (btn != null)
                {
                    btn.Attributes.Add("onclick", "ChangeCurrentCulture('" + LanguageName + "');");
                }
            }
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

            //Raise Event Handler
            //mc.CMSContext.Current.LangChange = true;

            if (OnLanguageChange != null)
                OnLanguageChange(this, new EventArgs());
            Response.Redirect(Request.RawUrl);

        }
        #endregion
    }
}