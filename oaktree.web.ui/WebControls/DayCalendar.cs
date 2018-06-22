using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Collections.ObjectModel;

namespace OakTree.Web.UI.WebControls {
	[ToolboxItem(false)]
	public class DayCalendar :CompositeControl {
		
		private DateTime _startDate = DateTime.Now.AddDays(-3);
		private DateTime _endDate = DateTime.Now.AddDays(3);
		private DayCalendarRowSets _rowSets;


		public DateTime StartDate { 
			get { return this._startDate; }
			set { 
				this._startDate = value;
				//this.Update();
			}
		}
		public DateTime EndDate {
			get { return this._endDate; }
			set {
				this._endDate = value;
				//this.Update();
			}
		}
		public Boolean ShowDayHeader { get; set; }
		public Boolean ShowRowLabel { get; set; }

		public DayCalendar():base() {
			this._rowSets = new DayCalendarRowSets(this);
			//this.Rows.Changed += new EventHandler<DayCalendarRowChangedEventArgs>(Rows_Changed);
		}

		[PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DayCalendarRowSets RowSets { get { return this._rowSets; } }

		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		public int NumberOfDays {
			get { return (this.EndDate - this.StartDate).Days; } 
		}

		protected override void CreateChildControls() {

			HtmlGenericControl table = new HtmlGenericControl("table");
			this.Controls.Add(table);
			
			if (this.ShowDayHeader) { 
				HtmlGenericControl thead = new HtmlGenericControl("thead");
			

				DayCalendarRow headerRow = new DayCalendarRow();
				headerRow.DayCalendar = this;
				headerRow.RowType = DayCalenderRowType.HeaderRow;
				headerRow.ShowHeading = true;

				thead.Controls.Add(headerRow);
				DateTime date = this.StartDate;
				table.Controls.Add(thead);
			}

			HtmlGenericControl tbody = new HtmlGenericControl("tbody");
			table.Controls.Add(tbody);

			foreach (DayCalendarRowSet row in this.RowSets) {
				tbody.Controls.Add(row);
			}
			
		}

	}

	public enum DayCalendarDayType { DataCell, HeaderCell, FooterCell }
	public enum DayCalenderRowType { DataRow, HeaderRow, FooterRow }
	
	[ToolboxItem(false)]
	public class DayCalendarDay :CompositeControl,ITextControl {

		public DayCalendarDayType DayType { get; set; }
		public DateTime Day { get; set; }
		public int RowSpan {
			get { return int.Parse( this.Attributes["rowspan"]) ; }
			set { this.Attributes.Add("rowspan", value.ToString()); }
		}

		protected override System.Web.UI.HtmlTextWriterTag TagKey {
			get {
				if (this.DayType == DayCalendarDayType.HeaderCell)
					return HtmlTextWriterTag.Th;
				return System.Web.UI.HtmlTextWriterTag.Td;
			}
		}


		protected override void CreateChildControls() {
			if (!string.IsNullOrEmpty(this.Text)) {
				Literal litText = new Literal();
				litText.Text = this.Text;
				this.Controls.Add(litText);
			}
		}

		#region ITextControl Members

		public string Text { get; set; }

		#endregion
	}
	[ToolboxItem(false)]
	public class DayCalendarRow : CompositeControl {
		private DayCalendar _dayCalendar;

		public DayCalendar DayCalendar {
			get { return this._dayCalendar; }
			set { 
				this._dayCalendar = value; 
			}
		}

		public DayCalenderRowType RowType { get; set; }
		public string HeadingText { get; set; }
		public bool ShowHeading { get; set; }
		public int HeaderRowspan { get; set; }

		private List<DayCalendarDay> _days;
		protected List<DayCalendarDay> Days { 
			get {
				if (this._days == null) this._days = new List<DayCalendarDay>();
				return this._days;
			} 
		}

		public void Update() {
			this.Days.Clear();
			DateTime date = this.DayCalendar.StartDate;
			do {
				DayCalendarDay Day = new DayCalendarDay();
				if (this.RowType == DayCalenderRowType.HeaderRow) {
					Day.DayType = DayCalendarDayType.HeaderCell;
					//Day.Text = date.DayOfWeek.ToString();
					Day.Text = date.Date.ToShortDateString();
				}
				this.Days.Add(Day);
				//this.Controls.Add(Day);
				date = date.AddDays(1);
			} while (date.Date.CompareTo(this.DayCalendar.EndDate.Date) < 1);
		}

		protected override System.Web.UI.HtmlTextWriterTag TagKey {
			get {
				return System.Web.UI.HtmlTextWriterTag.Tr;
			}
		}
		
		protected override void CreateChildControls() {

			//TODO: Add a way to set the rowspan attribute to be the number of divisions, Then not render the rest of the rows with that cell.
			if (this.ShowHeading) {
				DayCalendarDay HeadingDay = new DayCalendarDay();
				HeadingDay.DayType = DayCalendarDayType.HeaderCell;
				HeadingDay.Text = this.HeadingText;
				HeadingDay.RowSpan = this.HeaderRowspan;
				this.Controls.Add(HeadingDay);
			}

			for (int i = 0; i < this.DayCalendar.NumberOfDays; i++) {
				DayCalendarDay Day = new DayCalendarDay();
				Day.Day = this.DayCalendar.StartDate.AddDays(i);

				if (this.RowType == DayCalenderRowType.HeaderRow) {		
					Day.DayType = DayCalendarDayType.HeaderCell;
					Day.Text = Day.Day.DayOfWeek.ToString();
				}
				this.Controls.Add(Day);
				
			}
		}
	}
	[ToolboxItem(false)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DayCalendarRowSet : CompositeControl {
		private int _divisions = 1;

		public String Title { get; set; }
		public DayCalendar DayCalendar { get; set; }
		public int Divisions { 
			get { return this._divisions; }
			set { this._divisions = value; }
		}

		public override void RenderBeginTag(HtmlTextWriter writer) {
			//base.RenderBeginTag(writer);
		}
		public override void RenderEndTag(HtmlTextWriter writer) {
			//base.RenderEndTag(writer);
		}
		protected override void CreateChildControls() {
			for (int i = 0; i < this.Divisions; i++) {
				DayCalendarRow row = new DayCalendarRow();
				row.RowType = DayCalenderRowType.DataRow;
				row.DayCalendar = this.DayCalendar;
				if (i == 0) {
					row.HeadingText = this.Title;
					row.ShowHeading = true;
					row.HeaderRowspan = this.Divisions;
				} else {
					row.ShowHeading = false;
				}
				this.Controls.Add(row);
			}
		}

	}

	public class DayCalendarRowSets : Collection<DayCalendarRowSet> {
		public event EventHandler<DayCalendarRowSetChangedEventArgs> Changed;

		public DayCalendar _parent = null;

		public DayCalendarRowSets(DayCalendar Parent) {
			this._parent = Parent;
		}

		protected override void InsertItem(int index, DayCalendarRowSet newItem) {
			base.InsertItem(index, newItem);

			newItem.DayCalendar = this._parent;
			newItem.Page = this._parent.Page;

			EventHandler<DayCalendarRowSetChangedEventArgs> temp = Changed;
			if (temp != null) {
				temp(this, new DayCalendarRowSetChangedEventArgs(
					ChangeType.Added, newItem, null));
			}
		}

		protected override void SetItem(int index, DayCalendarRowSet newItem) {
			DayCalendarRowSet replaced = Items[index];
			base.SetItem(index, newItem);

			EventHandler<DayCalendarRowSetChangedEventArgs> temp = Changed;
			if (temp != null) {
				temp(this, new DayCalendarRowSetChangedEventArgs(
					ChangeType.Replaced, replaced, newItem));
			}
		}

		protected override void RemoveItem(int index) {
			DayCalendarRowSet removedItem = Items[index];
			base.RemoveItem(index);

			EventHandler<DayCalendarRowSetChangedEventArgs> temp = Changed;
			if (temp != null) {
				temp(this, new DayCalendarRowSetChangedEventArgs(
					ChangeType.Removed, removedItem, null));
			}
		}

		protected override void ClearItems() {
			base.ClearItems();

			EventHandler<DayCalendarRowSetChangedEventArgs> temp = Changed;
			if (temp != null) {
				temp(this, new DayCalendarRowSetChangedEventArgs(
					ChangeType.Cleared, null, null));
			}
		}

	}

	public class DayCalendarRowSetChangedEventArgs : EventArgs {
		public readonly DayCalendarRowSet ChangedItem;
		public readonly ChangeType ChangeType;
		public readonly DayCalendarRowSet ReplacedWith;

		public DayCalendarRowSetChangedEventArgs(ChangeType change, DayCalendarRowSet item,
			DayCalendarRowSet replacement) {
			ChangeType = change;
			ChangedItem = item;
			ReplacedWith = replacement;
		}
	}

	public enum ChangeType {
		Added,
		Removed,
		Replaced,
		Cleared
	};


}
