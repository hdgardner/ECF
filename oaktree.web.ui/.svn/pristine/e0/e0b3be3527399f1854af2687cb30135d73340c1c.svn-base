using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.HtmlControls;

namespace OakTree.Web.UI.WebControls {
	public abstract class FormFieldComposite : CompositeControl {

		private Label _label;
		private HtmlGenericControl _legend;
		protected virtual Label Label {
			get {
				if (this._label == null) {
					this._label = new Label();
				}
				return this._label;
			}
		}
		protected HtmlGenericControl Legend {
			get {
				if (this._legend == null) this._legend = new HtmlGenericControl("legend");
				return this._legend;
			}
		}
		//this allows us to control the tag the control is rendered in
		protected override HtmlTextWriterTag TagKey {
			get {
				switch (this.ContainerType) {
					case FormFieldContainerType.DIV:
						return HtmlTextWriterTag.Div;
					case FormFieldContainerType.SPAN:
						return HtmlTextWriterTag.Span;
					case FormFieldContainerType.FIELDSET:
						return HtmlTextWriterTag.Fieldset;
					default:
						return HtmlTextWriterTag.Span;
				}
			}
		}

		public enum FormFieldLabelPosition { LEFT, RIGHT };
		public enum FormFieldContainerType { DIV, SPAN, FIELDSET }

		[Bindable(true)]
		[Category("Behaviour")]
		[Description("They type of HTML element the contrl will be rendered in.")]
		public FormFieldContainerType ContainerType { get; set; }
		[Bindable(true)]
		[Category("Behaviour")]
		[Description("The position of the form field's label.")]
		public FormFieldLabelPosition LabelPosition { get; set; }
		[Bindable(true)]
		[Category("Appearance")]
		[Description("The text of the form field's label.")]
		public string LabelText { get; set; }
		[Bindable(true)]
		[Category("Appearance")]
		[Description("The text of the form field's legend. This is only applicable if the form field is enclosed in a fieldset.")]
		public string LegendText { get; set; }

	}
}
