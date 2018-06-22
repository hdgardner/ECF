using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.UI;
using System.Collections.Specialized;
using System.Web;
using Mediachase.Cms.WebUtility.UI.Controls;

namespace Mediachase.Cms.Web.UI.Controls
{
    /// <summary>
    /// Extends standard NumericPagerField with a support for URL Rewritten pages.
    /// </summary>
    public class CmsNumericPagerField : NumericPagerField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CmsNumericPagerField"/> class.
        /// </summary>
        public CmsNumericPagerField()
        {
        }

        /// <summary>
        /// Creates and returns a new instance of the <see cref="T:System.Web.UI.WebControls.NumericPagerField"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of the <see cref="T:System.Web.UI.WebControls.NumericPagerField"/> class.
        /// </returns>
        protected override DataPagerField CreateField()
        {
            return new CmsNumericPagerField();
        }

        /// <summary>
        /// Gets the query string navigate url2.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <returns></returns>
        private string GetQueryStringNavigateUrl2(int pageNumber)
        {
            //string q = CMSContext.Current.CurrentUrl.Contains("?") ? "&" : "?";

            string queryField = this.DataPager.QueryStringField;

            StringBuilder builder = new StringBuilder();

            string path = String.Empty;
            if(CMSContext.Current.CurrentUrl.Contains("?"))
            {
                builder.Append(CMSContext.Current.CurrentUrl.Substring(0, CMSContext.Current.CurrentUrl.IndexOf("?")));
                builder.Append("?");

                NameValueCollection query = new NameValueCollection();
                string queryString = CMSContext.Current.CurrentUrl.Substring(CMSContext.Current.CurrentUrl.IndexOf("?")+1);
                string[] queryParamsArray = queryString.Split(new char[] {'&'});
                foreach(string queryParam in queryParamsArray)
                {
                    string[] queryParamArray = queryParam.Split(new char[] {'='});
                    if(!queryParamArray[0].Equals(queryField, StringComparison.OrdinalIgnoreCase))
                    {
                        builder.Append(HttpUtility.UrlEncode(queryParamArray[0]));
                        builder.Append("=");
                        if(queryParamArray.Length > 1)
                            builder.Append(HttpUtility.UrlEncode(queryParamArray[1]));
                        builder.Append("&");
                    }
                }
            }
            else
            {
                builder.Append(CMSContext.Current.CurrentUrl);
                builder.Append("?");
            }

            if (pageNumber > 1) // do not add p=1 for a first page to eliminate duplicate urls and improve SEO page ranks
            {
                builder.Append(queryField);
                builder.Append("=");
                builder.Append(pageNumber.ToString());
                return builder.ToString();
            }
            else
            {
                string returnString = builder.ToString();
                return returnString.Substring(0, returnString.Length - 1);
            }            
        }

        /// <summary>
        /// Creates the numeric button.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        private HyperLink CreateNumericButton(int pageIndex)
        {
            int pageNumber = pageIndex + 1;
            HyperLink link = new HyperLink();
            link.Text = pageNumber.ToString(CultureInfo.InvariantCulture);
            link.NavigateUrl = GetQueryStringNavigateUrl2(pageNumber);

            if (!String.IsNullOrEmpty(NumericButtonCssClass))
            {
                link.CssClass = NumericButtonCssClass;
            }

            return link;
        }

        /// <summary>
        /// Creates the next prev button.
        /// </summary>
        /// <param name="buttonText">The button text.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="imageUrl">The image URL.</param>
        /// <returns></returns>
        private HyperLink CreateNextPrevButton(string buttonText, int pageIndex, string imageUrl)
        {
            int pageNumber = pageIndex + 1;
            CmsHyperLink link = new CmsHyperLink();

            link.Text = buttonText;
            link.NavigateUrl = GetQueryStringNavigateUrl2(pageNumber);
            link.ImageUrl = imageUrl;

            WebControl webControl = link as WebControl;
            if (!String.IsNullOrEmpty(NextPreviousButtonCssClass))
            {
                link.CssClass = NextPreviousButtonCssClass;
            }

            return link;
        }

        /// <summary>
        /// Creates the user interface (UI) controls for the pager field object and adds them to the specified container.
        /// </summary>
        /// <param name="container">The container that is used to store the controls.</param>
        /// <param name="startRowIndex">The index of the first record on the page.</param>
        /// <param name="maximumRows">The maximum number of items on a single page.</param>
        /// <param name="totalRowCount">The total number of items.</param>
        /// <param name="fieldIndex">The index of the data pager field in the <see cref="P:System.Web.UI.WebControls.DataPager.Fields"/> collection.</param>
        public override void CreateDataPagers(DataPagerFieldItem container, int startRowIndex, int maximumRows, int totalRowCount, int fieldIndex)
        {
            int currentPageIndex = startRowIndex / DataPager.PageSize;
            object currentPageObj = DataPager.Page.Request.QueryString[this.DataPager.QueryStringField];
            short currentQSPageIndex;
            bool resetProperties = false;
            if (currentPageObj != null)
            {
                bool parsed = Int16.TryParse(currentPageObj.ToString(), out currentQSPageIndex);
                if (parsed)
                {
                    currentQSPageIndex--;
                    int highestPageIndex = (totalRowCount - 1) / maximumRows;
                    if (currentPageIndex != currentQSPageIndex && currentQSPageIndex <= highestPageIndex)
                    {
                        currentPageIndex = currentQSPageIndex;
                        startRowIndex = (currentPageIndex * DataPager.PageSize);
                        resetProperties = true;
                    }
                }
            }

            int firstButtonIndex = (startRowIndex / (ButtonCount * DataPager.PageSize)) * ButtonCount;
            int lastButtonIndex = firstButtonIndex + ButtonCount - 1;
            int lastRecordIndex = ((lastButtonIndex + 1) * DataPager.PageSize) - 1;

            if (firstButtonIndex != 0)
            {
                container.Controls.Add(CreateNextPrevButton(PreviousPageText, firstButtonIndex - 1, PreviousPageImageUrl));
                if (RenderNonBreakingSpacesBetweenControls)
                {
                    container.Controls.Add(new LiteralControl("&nbsp;"));
                }
            }

            for (int i = 0; i < ButtonCount && totalRowCount > ((i + firstButtonIndex) * DataPager.PageSize); i++)
            {
                if (i + firstButtonIndex == currentPageIndex)
                {
                    Label pageNumber = new Label();
                    pageNumber.Text = (i + firstButtonIndex + 1).ToString(CultureInfo.InvariantCulture);
                    if (!String.IsNullOrEmpty(CurrentPageLabelCssClass))
                    {
                        pageNumber.CssClass = CurrentPageLabelCssClass;
                    }
                    container.Controls.Add(pageNumber);
                }
                else
                {
                    container.Controls.Add(CreateNumericButton(i + firstButtonIndex));
                }
                if (RenderNonBreakingSpacesBetweenControls)
                {
                    container.Controls.Add(new LiteralControl("&nbsp;"));
                }
            }

            if (lastRecordIndex < totalRowCount - 1)
            {
                if (RenderNonBreakingSpacesBetweenControls)
                {
                    container.Controls.Add(new LiteralControl("&nbsp;"));
                }
                container.Controls.Add(CreateNextPrevButton(NextPageText, firstButtonIndex + ButtonCount, NextPageImageUrl));
                if (RenderNonBreakingSpacesBetweenControls)
                {
                    container.Controls.Add(new LiteralControl("&nbsp;"));
                }
            }

            if (resetProperties)
            {
                DataPager.SetPageProperties(startRowIndex, maximumRows, true);
            }
        }

        /// <summary>
        /// Handles events that occur in the <see cref="T:System.Web.UI.WebControls.NumericPagerField"/> object and performs the appropriate action.
        /// </summary>
        /// <param name="e">The event data.</param>
        public override void HandleEvent(CommandEventArgs e)
        {
            // Not needed because Hyperlinks don't generate bubbled events.
        }

        // Required for design-time support (DesignerPagerStyle)
        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="T:System.Web.UI.WebControls.NumericPagerField"/> object.
        /// </summary>
        /// <param name="o">The object to compare with the current <see cref="T:System.Web.UI.WebControls.NumericPagerField"/> instance.</param>
        /// <returns>
        /// true if the specified object is equal to the current <see cref="T:System.Web.UI.WebControls.NumericPagerField"/> object; otherwise, false.
        /// </returns>
        public override bool Equals(object o)
        {
            CmsNumericPagerField field = o as CmsNumericPagerField;
            if (field != null)
            {
                return base.Equals(o);
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:System.Web.UI.WebControls.NumericPagerField"/> class.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Web.UI.WebControls.NumericPagerField"/> object. For more information, see the <see cref="M:System.Object.GetHashCode"/> class.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
