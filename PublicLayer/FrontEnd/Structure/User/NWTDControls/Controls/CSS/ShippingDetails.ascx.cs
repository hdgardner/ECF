using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NWTD.InfoManager;

namespace Mediachase.Cms.Website.Structure.User.NWTDControls.Controls.CSS {

	/// <summary>
	/// This control displays shipping details for an Invoice retrieved from NWTD's InfoManager Service
	/// </summary>
	public partial class ShippingDetails : NWTD.Web.UI.UserControls.InfoManagerUserControl {

		private Invoice _invoice;
		
		/// <summary>
		/// The ID of the invoice for which to show the details. Retrieved from the query string.
		/// </summary>
		public string InvoiceId {
			get {
				return this.Request["InvoiceId"];
			}
		
		}

		/// <summary>
		/// The invoice for which to show the details
		/// </summary>
		public Invoice Invoice {
			get { 
				if(this._invoice == null && !string.IsNullOrEmpty(this.InvoiceId)){
					this._invoice = this.Client.GetInvoiceDetailByInvoiceId(int.Parse(this.InvoiceId));
				}
				return this._invoice;
			}
		
		}
		
		/// <summary>
		/// During page load, the businessparnter ID is retrieved from the current user and compared with the Invoice/Shipment 
		/// If it matches, we show the shipment. Otehrwise an acces denied panel is shown. - hg 09/16/13
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {
            //Make sure not postback per asp.net best practices for retrieving DB data (Heath Gardner 09/16/13)
            if (!IsPostBack)
            {
                //Make sure Invoice exists (Heath Gardner 09/16/13)
                if (this.Invoice != null)
                {
                    //Show Access Denied message if this Invoice doesn't belong to this BP (Heath Gardner 09/16/13)
                    if (this.Invoice.Header.BusinessPartnerId != NWTD.Profile.BusinessPartnerID)
                    {
                        this.pnlAccessIsDenied.Visible = true;
                        this.pnlInvoiceDetails.Visible = false;
                        this.gvInvoice.Visible = false;
                        return;
                    }

                    //Bind invoice data if correct BP (Heath Gardner 09/16/13)
                    BindInvoiceLines();
                    //Bind Repeaters and Hyperlinks for Tracking numbers (Heath Gardner on 09/16/13)
                    BindTrackNumsRepeater();
                }
            }
		}

        private void BindInvoiceLines()
        {
            this.gvInvoice.DataSource = this.Invoice.Lines;
            this.gvInvoice.DataBind();
        }

/////// Tracking Number Hyperlink functionality (Heath Gardner 09/16/13)////////////////////////////////////////////////////////////
        private void BindTrackNumsRepeater() //Bind TrackNums to relevant repeater
        {
            //Make sure some Track Nums exist
            if (!string.IsNullOrEmpty(this.Invoice.Header.TrackingNums))
            {
                //Get tracking numbers CSV string and remove spaces 
                string sTrackNums = this.Invoice.Header.TrackingNums.Replace(" ", "");

                //Get the CarrierID (e.g. UPS, MAIL)
                string sCarrierID = this.Invoice.Header.TrackingHyperLinks;

                //Configure UPS tracking data
                if (sCarrierID == "UPS")
                {
                    UPS_Data(sTrackNums);
                }

                //Configure Mail tracking data
                else if (sCarrierID == "MAIL")
                {
                    Mail_Data(sTrackNums);
                }

                //Configure all other non-hyperlink tracking data (usually blank or null)
                else
                {
                    Generic_Data(sTrackNums);
                }
            }
        }

        //Bind UPS Data 
        private void UPS_Data(string TrackNumsCsvString)
        {
            //Break tracking number CSV string into list 
            List<string> UPSTrackNums = new List<string>(TrackNumsCsvString.Split(',').ToList());

            //Total number of records in list
            int iUPSListCount = UPSTrackNums.Count;

            //See if last track num is a partial and/or if any track nums are missing
            if (PartialOrMissingTrackNums(UPSTrackNums, iUPSListCount))
            {
                //Remove partial track num (0 based so listcount - 1)
                UPSTrackNums.RemoveAt(iUPSListCount - 1);
            }

            //Loop through the remaining list and put line break on the end
            for (int i = 0; i < iUPSListCount; i++)
            {
                //Add line break to end if not last record 
                if (i != iUPSListCount - 1)
                {
                    //Line break required as seperator for bulk tracking (e.g. copy/paste) on UPS.com
                    UPSTrackNums[i] = UPSTrackNums[i] + "<br />";
                }
            }

            //Bind the list of Track Nums to repeater (this repeater holds a list of UPS hyperlinks)
            UPSTrackNumsRepeater.DataSource = UPSTrackNums;
            UPSTrackNumsRepeater.DataBind();

            //Make UPSTrackNumsRepeater visible
            UPSTrackNumsRepeater.Visible = true;
        }

        //Bind Mail Data
        private void Mail_Data(string TrackNumsCsvString)
        {
            List<string> MailTrackNums = new List<string>(TrackNumsCsvString.Split(',').ToList());

            //Total number of records in list
            int iMailListCount = MailTrackNums.Count;

            //See if last track num is a partial and/or if any track nums are missing. 
            if (PartialOrMissingTrackNums(MailTrackNums, iMailListCount))
            {
                //Remove partial track num (0 based so listcount - 1)
                MailTrackNums.RemoveAt(iMailListCount - 1);
                //Reduce the list count by 1 since we just removed a record
                iMailListCount = iMailListCount - 1;
            }

            //Loop through the remaining list and make sure 94 is on the front and a comma is on the back
            for (int i = 0; i < iMailListCount; i++)
            {
                //Make sure at least two characters long (required for substring)
                if (MailTrackNums[i].Length >= 2)
                {
                    //Add "94" to the front of each number if missing
                    if (MailTrackNums[i].Substring(0, 2) != "94")
                    {
                        MailTrackNums[i] = "94" + MailTrackNums[i];
                    }
                }

                //Add comma if not last record 
                if (i != iMailListCount - 1)
                {
                    //Comma required as seperator for bulk tracking (e.g. copy/paste) on USPS.com
                    MailTrackNums[i] = MailTrackNums[i] + ",";
                }
            }

            //Bind the list of Track Nums to repeater (this repeater holds a list of Mail hyperlinks)
            MailTrackNumsRepeater.DataSource = MailTrackNums;
            MailTrackNumsRepeater.DataBind();

            //Make MailTrackNumsRepeater visible
            MailTrackNumsRepeater.Visible = true;
        }

        //Bind all other shipments
        private void Generic_Data(string TrackNumsCsvString)
        {
            //Break tracking number CSV string into list 
            List<string> GenericTrackNums = TrackNumsCsvString.Split(',').ToList();

            //Bind the list of Track Nums to repeater (this repeater holds a list of track nums with & NO hyperlinks)
            GenericTrackNumsRepeater.DataSource = GenericTrackNums;
            GenericTrackNumsRepeater.DataBind();
            GenericTrackNumsRepeater.Visible = true;
        }

        //Turns on missing track nums message if shipment has more tracknums than are displayed and returns "true" if final number is partial. This 
        // issue is caused from SBO not storing all track nums (e.g. currently 254 char or 13 UPS 12 Mail limit in SBO) -- Heath Gardner 09/16/13
        private bool PartialOrMissingTrackNums(List<string> TrackNumsList, int TrackNumsCount)
        {
            //Get final track num position
            int iFinalTrackNum = TrackNumsCount - 1;
            //Get first to last track num position
            int iPrevious = iFinalTrackNum - 1;
            //Get total carton count from Invoice
            int U_NumCartons = Convert.ToInt16(this.Invoice.Header.CartonCount);

            //Check if track nums are missing by comparing carton count to tracknum count
            if (U_NumCartons > TrackNumsCount)
            {
                //Display partial track num message if some are missing
                lblPartialTrackTitle.Visible = true;
                lblPartialTrackMessage.Visible = true;
            }

            //Check final tracking number to see if partial 
            if (iFinalTrackNum > 0)
            {
                //Compare final track num length to first to last track num length
                if (TrackNumsList[iFinalTrackNum].Length < TrackNumsList[iPrevious].Length)
                {
                    //Make sure partial track num message is on
                    lblPartialTrackTitle.Visible = true;
                    lblPartialTrackMessage.Visible = true;
                    //Return true if final track num record is shorter than previous (this means it is a partial)
                    return true;
                }
            }

            //Otherwise, return false
            return false;
        }

        //Removes line break and/or comma from tracking number string (hg 09/16/13)
        protected string RemoveSpecialChar(object TrackNum, string CharToRemove)
        {
            string CleanString = TrackNum.ToString();
            if (CharToRemove == "LineBreak")
                CleanString = CleanString.Replace("<br />", "");
            else if (CharToRemove == "Comma")
                CleanString = CleanString.Replace(",", "");
            return CleanString;

        }
//////// End of Tracking Number Hyperlink functionality (Heath Gardner 09/16/13)//////////////////////////////////////////////////////////// 
	}
}