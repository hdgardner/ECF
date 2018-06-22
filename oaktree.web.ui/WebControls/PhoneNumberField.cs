using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text.RegularExpressions;

using OakTree.Web.UI;

namespace OakTree.Web.UI.WebControls {

	public enum AreaCodeFormatType  {PARENTHESES, NONE};
	public enum PrefixFormatType  {HYPHEN, NONE };

	/// <summary>
	/// A field for collecting phone number data
	/// </summary>
	[ToolboxData("<{0}:PhoneNumberField runat=\"server\" />"), DefaultProperty("LabelText"), ValidationProperty("Text")]
	public class PhoneNumberField:FormFieldComposite {

		private TextBox _countryCode;
		private TextBox _areaCode;
		private TextBox _prefix;
		private TextBox _suffix;
		private Label _label;
		private string _internationalNumberFormatString = "{0} ({1}) {2}-{3}";
		private string _localNumberFormatString = "({0}) {1}-{2}";

		protected TextBox CountryCodeBox {
			get {
				if (this._countryCode == null) {
					this._countryCode = new TextBox();
					this._countryCode.Columns = 1;
				}
				return this._countryCode;
			}
		}
		protected TextBox AreaCodeBox {
			get { 
				if(this._areaCode == null){
					this._areaCode = new TextBox();
					this._areaCode.ID = new Guid().ToString();
					this._areaCode.Columns = 2;
					this._areaCode.MaxLength = 3;
				}
				return this._areaCode;
			}
		}
		protected TextBox PrefixBox {
			get {
				if (this._prefix == null) {
					this._prefix = new TextBox();
					this._prefix.Columns = 2;
					this._prefix.MaxLength = 3;
				}
				return this._prefix;
			}
		}
		protected TextBox SuffixBox {
			get {
				if (this._suffix == null) {
					this._suffix = new TextBox();
					this._suffix.Columns = 3;
					this._suffix.MaxLength = 4;
				}
				return this._suffix;
			}
		}
		protected override Label Label {
			get {
				if (this._label == null) {
					this._label = new Label();
					this._label.AssociatedControlID = this.AreaCodeBox.ID;
				}
				return this._label;
			}
		}

		public PhoneNumber PhoneNumber {
			get {
				this.EnsureChildControls();
				return new PhoneNumber(!string.IsNullOrEmpty(this.CountryCodeBox.Text)?this.CountryCodeBox.Text:null,this.AreaCodeBox.Text,this.PrefixBox.Text,this.SuffixBox.Text);
			}
			set {
				this.EnsureChildControls();
				if (value.IsInternational) CountryCodeBox.Text = value.CountryCode.ToString();
				this.AreaCodeBox.Text = value.AreaCode.ToString();
				this.PrefixBox.Text = value.Prefix.ToString();
				this.SuffixBox.Text = value.Suffix.ToString();
			}
		}

		[Category("Appearance")]
		[Description("Determines how the area code text box will appear in the form")]
		public AreaCodeFormatType AreaCodeFormat {get;set;}
		[Category("Appearance")]
		[Description("Determines how the prefix text box will appear in the form")]
		public PrefixFormatType PrefixFormat { get; set; }

		[Category("Behaviour")]
		[Description("The way international phone numbers will render when getting the Phone Number")]
		public string InternationalNumberFormatString {
			get { return this._internationalNumberFormatString; }
			set { this._internationalNumberFormatString = value; }
		}
		[Category("Behaviour")]
		[Description("The way local phone numbers will render when getting the Phone Number")]
		public string LocalNumberFormatString {
			get { return this._localNumberFormatString; }
			set { this._localNumberFormatString = value; }
		}
		[Category("Behaviour")]
		public bool IsInternational { get; set; }

		//[Category("Behaviour")]
		//public string CountryCode { 
		//    get {
		//        this.EnsureChildControls();
		//        int number = -1;
		//        if (string.IsNullOrEmpty(this.CountryCodeBox.Text)) this.PhoneNumber.CountryCode = null;
		//        if (!int.TryParse(this.CountryCodeBox.Text, out number)) throw new Exception(string.Format("Invalid Country Code:{0}", this.CountryCodeBox.Text));
		//        return this.CountryCodeBox.Text;
		//    }
		//    set {
		//        this.EnsureChildControls();
		//        this.CountryCodeBox.Text = value.ToString();
		//    }
		//}
		//[Category("Behaviour")]
		//public int AreaCode {
		//    get {
		//        this.EnsureChildControls();
		//        int number = -1;
		//        if (string.IsNullOrEmpty(this.AreaCodeBox.Text)) return number;
		//        if(! int.TryParse(this.AreaCodeBox.Text,out number)) throw new Exception(string.Format("Invalid Area Code:{0}",this.AreaCodeBox.Text));
		//        return number;
		//    }
		//    set {
		//        this.EnsureChildControls();
		//        this.AreaCodeBox.Text = value.ToString();
		//    }
		//}
		//[Category("Behaviour")]
		//public int Prefix {
		//    get {
		//        this.EnsureChildControls();
		//        int number = -1;
		//        if (string.IsNullOrEmpty(this.PrefixBox.Text)) return number;
		//        if (!int.TryParse(this.PrefixBox.Text, out number)) throw new Exception(string.Format("Invalid Prefix:{0}", this.PrefixBox.Text));
		//        return number;
		//    }
		//    set {
		//        this.EnsureChildControls();
		//        this.PrefixBox.Text = value.ToString();
		//    }
		//}
		//[Category("Behaviour")]
		//public int Suffix {
		//    get {
		//        this.EnsureChildControls();
		//        int number = -1;
		//        if (string.IsNullOrEmpty(this.SuffixBox.Text)) return number;
		//        if (!int.TryParse(this.SuffixBox.Text, out number)) throw new Exception(string.Format("Invalid Suffix:{0}", this.SuffixBox.Text));
		//        return number;
		//    }
		//    set {
		//        this.EnsureChildControls();
		//        this.SuffixBox.Text = value.ToString();
		//    }
		//}
		
		public string Text {
			get {
				if (!this.PhoneNumber.IsValid) return null;
				return this.PhoneNumber.ToString();			
			}
			set {
				PhoneNumber phoneNumber = new PhoneNumber();
				if(PhoneNumber.TryParse(value, ref phoneNumber)){
					this.PhoneNumber = phoneNumber;
				}
			}			
		}

		protected void CreateAddressChildControls() {
			if (this.IsInternational) this.Controls.Add(this.CountryCodeBox);
			if (this.AreaCodeFormat.Equals(AreaCodeFormatType.PARENTHESES)) this.Controls.Add(new Literal() { Text = "(" });
			this.Controls.Add(this.AreaCodeBox);
			if (this.AreaCodeFormat.Equals(AreaCodeFormatType.PARENTHESES)) this.Controls.Add(new Literal() { Text = ")" });
			this.Controls.Add(this.PrefixBox);
			if (this.PrefixFormat.Equals(PrefixFormatType.HYPHEN)) this.Controls.Add(new Literal() { Text = "-" });
			this.Controls.Add(this.SuffixBox);
		}

		protected override void CreateChildControls() {
			if (!string.IsNullOrEmpty(this.LegendText) && this.ContainerType == FormFieldContainerType.FIELDSET) {
				this.Legend.InnerText = this.LegendText;
				this.Controls.Add(this.Legend);
			}
			if (!string.IsNullOrEmpty(this.LabelText)) {
				this.Label.Text = this.LabelText;
				if (this.LabelPosition == FormFieldLabelPosition.LEFT) {
					this.Controls.Add(this.Label);
					this.CreateAddressChildControls();
				} else {
					this.CreateAddressChildControls();
					this.Controls.Add(this.Label);
				}
			} else {
				this.CreateAddressChildControls();
			}

		}

	}
}
