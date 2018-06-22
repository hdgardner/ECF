using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Mediachase.Cms.Util;
using Mediachase.Cms.WebUtility;

namespace Mediachase.Cms.Website.Templates.NWTD.PageTemplates {
	
    public partial class NWTDHomeTemplate : BaseStoreUserControl, IPublicTemplate {
		
		protected void Page_Load(object sender, EventArgs e) {

        // On 10/10/17, Heath implemented the following XML and SlideshowImages logic as part of my javascript/html/css slideshow
        //  solution to replace the original outdated flash control that wasn't compatible with most browsers
        // NOTE: The various components to my slideshow solution are in: NWTDHomeTemplate.ascx, NWTD -> home_v2.css, and here in this .ascx.cs
            string StaticImage_Path = string.Empty;
            string Image1_Path = string.Empty;
            string Image2_Path = string.Empty;
            string Image3_Path = string.Empty;
            string Image4_Path = string.Empty;
            string Image5_Path = string.Empty;
            string Image6_Path = string.Empty;
            string Image7_Path = string.Empty;

            // Use LINQ to access xml file that holds the image paths (hdg 10/10/17)
            string xmlPath = Server.MapPath("~/Repository/HomePage_Slideshow/SlideshowImagePaths_NWTD.xml");
            XDocument doc = XDocument.Load(xmlPath);

            // Loop through the xml file and get image paths (hdg 10/10/17)
            foreach (XElement el in doc.Root.Elements())
            {
                if (el.Attribute("id").Value == "StaticHomePageImage")
                    StaticImage_Path = el.Attribute("path").Value;
                else if (el.Attribute("id").Value == "SlideshowImage1")
                    Image1_Path = el.Attribute("path").Value;
                else if (el.Attribute("id").Value == "SlideshowImage2")
                    Image2_Path = el.Attribute("path").Value;
                else if (el.Attribute("id").Value == "SlideshowImage3")
                    Image3_Path = el.Attribute("path").Value;
                else if (el.Attribute("id").Value == "SlideshowImage4")
                    Image4_Path = el.Attribute("path").Value;
                else if (el.Attribute("id").Value == "SlideshowImage5")
                    Image5_Path = el.Attribute("path").Value;
                else if (el.Attribute("id").Value == "SlideshowImage6")
                    Image6_Path = el.Attribute("path").Value;
                else if (el.Attribute("id").Value == "SlideshowImage7")
                    Image7_Path = el.Attribute("path").Value;
            }

            // Set the frontend img paths for the slideshow (hdg 10/10/17) 
            this.StaticHomePageImage.Attributes["src"] = ResolveUrl(StaticImage_Path);
            this.SlideshowImage1.Attributes["src"] = ResolveUrl(Image1_Path);
            this.SlideshowImage2.Attributes["src"] = ResolveUrl(Image2_Path);
            this.SlideshowImage3.Attributes["src"] = ResolveUrl(Image3_Path);
            this.SlideshowImage4.Attributes["src"] = ResolveUrl(Image4_Path);
            this.SlideshowImage5.Attributes["src"] = ResolveUrl(Image5_Path);
            this.SlideshowImage6.Attributes["src"] = ResolveUrl(Image6_Path);
            this.SlideshowImage7.Attributes["src"] = ResolveUrl(Image7_Path);            

            
            //if we're not in design mode we'll hide certain content areas based on who we are (anonymous, level a, etc).
			//we'll also hide the handy labels that are only helpful in design mode
			if (!Mediachase.Cms.CMSContext.Current.IsDesignMode) {

				this.litAnon1.Visible = false;
				this.litAnon2.Visible = false;
				this.litLevelA1.Visible = false;
				this.litLevelA2.Visible = false;
				this.litLevelB1.Visible = false;
				this.litLevelB2.Visible = false;

				if (Profile.IsAnonymous) {
					this.pnlLevelAFeature1.Visible = false;
					this.pnlLevelAFeature2.Visible = false;
					this.pnlLevelBFeature1.Visible = false;
					this.pnlLevelBFeature2.Visible = false;
				}
				else {
                // On 10/10/17, Heath commented out the following as part of replacing the old flash slideshow 
				//	this.soHeader.FlashVars[0].Value = global::NWTD.Profile.BusinessPartnerState;
					this.pnlAnonymousMainContent.Visible = false;
					this.pnlAnonymousFeature1.Visible = false;
					this.pnlAnonymousFeature2.Visible = false;


					if (global::NWTD.Profile.CurrentUserLevel.Equals(global::NWTD.UserLevel.A)) {
						this.pnlLevelAFeature1.Visible = true;
						this.pnlLevelAFeature2.Visible = true;
						this.pnlLevelBFeature1.Visible = false;
						this.pnlLevelBFeature2.Visible = false;
					}
					if (global::NWTD.Profile.CurrentUserLevel.Equals(global::NWTD.UserLevel.B)) {
						this.pnlLevelAFeature1.Visible = false;
						this.pnlLevelAFeature2.Visible = false;
						this.pnlLevelBFeature1.Visible = true;
						this.pnlLevelBFeature2.Visible = true;
					}
				}
			}
		}


		#region IPublicTemplate Members

		/// <summary>
		/// Gets the control places.
		/// </summary>
		/// <value>The control places.</value>
		public string ControlPlaces {
			get {
				return "MainContentArea,FeatureContent1,FeatureContent2,AnonymousFeatureContent1,AnonymousFeatureContent2,LevelBFeatureContent1,LevelBFeatureContent2";
			}
		}

		#endregion
	}
}