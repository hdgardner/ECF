using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OakTree.Web.UI {
	public struct PhoneNumber {

		public static string DefaultInternationalFormatString = "{0} ({1}) {2}-{3}";
		public static string DefaultLocalFormatString = "({0}) {1}-{2}";

		public string ToString(string formatString) {
			return string.Format(formatString, this.CountryCode, this.AreaCode, this.Prefix, this.Suffix);
		}
		public override string ToString() {
			return this.ToString(PhoneNumber.DefaultInternationalFormatString);
			
		}

		public static PhoneNumber Parse(string s) {
			return new PhoneNumber(s);
		}
		public static bool TryParse(string s, ref PhoneNumber PhoneNumber) {
			try {
				PhoneNumber = PhoneNumber.Parse(s);
				return true;
			} catch (Exception ex) { 
				return false; 
			}
		}

		public PhoneNumber(string CountryCode, string AreaCode, string Prefix, string Suffix) {
			_countryCode = CountryCode;
			_areaCode = AreaCode;
			_prefix = Prefix;
			_suffix = Suffix;
		
		}
		public PhoneNumber(string AreaCode, string Prefix, string Suffix) : this(null, AreaCode, Prefix, Suffix) { }
		public PhoneNumber(string Number) {
			string cleanNumber = Regex.Replace(Number, "[^0-9]", string.Empty);
			if (cleanNumber.Length < 10) throw new Exception("The phone number has too few digits");
			this._suffix = cleanNumber.Substring(cleanNumber.Length - 4, 4);
			this._prefix = cleanNumber.Substring(cleanNumber.Length - 7, 3);
			this._areaCode = cleanNumber.Substring(cleanNumber.Length - 10, 3);
			if (cleanNumber.Length > 10) this._countryCode = cleanNumber.Substring(0, cleanNumber.Length - 10);
			else { this._countryCode = null; }
		}

		private string _countryCode;
		private string _areaCode;
		private string _prefix;
		private string _suffix;


		public string CountryCode { get { return this._countryCode; } set { this._countryCode = value; } }
		public string AreaCode { get { return this._areaCode; } set { this._areaCode = value; } }
		public string Prefix { get { return this._prefix; } set { this._prefix = value; } }
		public string Suffix { get { return this._suffix; } set { this._suffix = value; } }
		public bool IsInternational{get{return this.CountryCode != null;}}
		public bool IsEmpty {
			get {
				return (string.IsNullOrEmpty(this.AreaCode) && string.IsNullOrEmpty(this.Prefix) && string.IsNullOrEmpty(this.Suffix));
			}
		}
		public bool IsValid { 
			get {
				if (string.IsNullOrEmpty(this.AreaCode) || string.IsNullOrEmpty(this.Prefix) || string.IsNullOrEmpty(this.Suffix)) return false;
				if(this.AreaCode.Length != 3) return false;
				if(this.Prefix.Length != 3) return false;
				if(this.Suffix.Length != 4) return false;
				return true;
			} 
		}
	}
}
