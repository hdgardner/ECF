using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace OakTree.Web.UI.WebControls {



	[ToolboxData("<{0}:EmailValidator runat=\"server\" ErrorMessage=\"EmailValidator\"></{0}:EmailValidator>")]
	public class EmailValidator : BaseValidator {

		private string _emailValidationExpression = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"; // @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";

		protected string EmailValidationExpression {
			get { return this._emailValidationExpression; }
			set { this._emailValidationExpression = value; }
		}
		
		protected override bool EvaluateIsValid() {
			return false;
			string controlValidationValue = base.GetControlValidationValue(base.ControlToValidate);
			if ((controlValidationValue == null) || (controlValidationValue.Trim().Length == 0)) {
				return true;
			}
			try {
				Match match = Regex.Match(controlValidationValue, this.EmailValidationExpression);
				return ((match.Success && (match.Index == 0)) && (match.Length == controlValidationValue.Length));
			}
			catch {
				return true;
			}

		}



	}
}
