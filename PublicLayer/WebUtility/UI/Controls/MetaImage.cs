namespace Mediachase.Cms.Web.UI.Controls
{
	using System;
	using System.Data;
	using System.Web;
	using System.Web.UI.HtmlControls;
    using Mediachase.Commerce.Catalog.Objects;

	/// <summary>
	///	Used to load image from the image meta field
	/// </summary>
    public partial class MetaImage : System.Web.UI.WebControls.Image
	{
        private Images _DataSource = null;

        private string _PropertyName = String.Empty;

        /// <summary>
        /// The property name within meta fields, if not specified, the image won't be rendered
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            set
            {
                _PropertyName = value;
            }
            get
            {
                return _PropertyName;
            }
        }

        private bool _ShowThumbImage = false;
        /// <summary>
        /// If this property set, the thumbnail image will be displayed.
        /// </summary>
        /// <value><c>true</c> if [show full image]; otherwise, <c>false</c>.</value>
        public bool ShowThumbImage
        {
            get
            {
                return _ShowThumbImage;
            }
            set
            {
                _ShowThumbImage = value;
            }
        }

        private bool _OpenFullImage = false;
        /// <summary>
        /// If this property set, the clicking will result in opening a full image dialog.
        /// </summary>
        /// <value><c>true</c> if [show full image]; otherwise, <c>false</c>.</value>
        public bool OpenFullImage
        {
            get
            {
                return _OpenFullImage;
            }
            set
            {
                _OpenFullImage = value;
            }
        }

        /// <summary>
        /// Image property within ItemAttributes
        /// </summary>
        /// <value>The data source.</value>
        public Images DataSource
        {
            set
            {
                _DataSource = value;
            }
            get
            {
                return _DataSource;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.DataBinding"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            BindImage();
        }

        /// <summary>
        /// Binds the image.
        /// </summary>
        private void BindImage()
        {
            this.Visible = false;
            if (DataSource != null && PropertyName != String.Empty)
            {
                Image image = DataSource[PropertyName];
                if (image != null)
                {
                    if (String.IsNullOrEmpty(image.ThumbnailUrl))
                    {
                        this.ImageUrl = image.Url;
                        if (!String.IsNullOrEmpty(image.Height))
                        {
                            if (!IsDimensionSpecified())
                            {
                                this.Height = System.Web.UI.WebControls.Unit.Parse(image.Height);
                                this.Width = System.Web.UI.WebControls.Unit.Parse(image.Width);
                            }
                        }
                    }
                    else
                    {
                        if (ShowThumbImage)
                        {
                            if (OpenFullImage)
                            {
                                this.Style.Add("cursor", "hand");
                                this.Attributes.Add("onclick", String.Format("javascript:window.open('{0}', {1}, {2})", image.Url, 1, 1));
                            }
                            this.ImageUrl = image.ThumbnailUrl;
                            if (!String.IsNullOrEmpty(image.ThumbnailHeight))
                            {
                                if (!IsDimensionSpecified())
                                {
                                    this.Height = System.Web.UI.WebControls.Unit.Parse(image.ThumbnailHeight);
                                    this.Width = System.Web.UI.WebControls.Unit.Parse(image.ThumbnailWidth);
                                }
                            }
                        }
                        else
                        {
                            this.ImageUrl = image.Url;
                            if (!String.IsNullOrEmpty(image.Height))
                            {
                                if (!IsDimensionSpecified())
                                {
                                    this.Height = System.Web.UI.WebControls.Unit.Parse(image.Height);
                                    this.Width = System.Web.UI.WebControls.Unit.Parse(image.Width);
                                }
                            }
                        }
                    }
                    this.Visible = true;
                }
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (!this.Page.IsPostBack || !this.IsViewStateEnabled)
            {
            }

            base.OnPreRender(e);
        }

        /// <summary>
        /// Determines whether [is dimension specified].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is dimension specified]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDimensionSpecified()
        {
            if (this.Height != System.Web.UI.WebControls.Unit.Empty || this.Width != System.Web.UI.WebControls.Unit.Empty)
                return true;

            return false;
        }
	}
}