using System;

namespace Mediachase.Web.Console.Interfaces
{
	public sealed class SelectorEventArgs : EventArgs
	{
		#region Properties
		private string serverStorageValue;

        /// <summary>
        /// Gets or sets the server storage value.
        /// </summary>
        /// <value>The server storage value.</value>
		public string ServerStorageValue
		{
			get { return serverStorageValue; }
			set { serverStorageValue = value; }
		}

		private int selectCount;
        /// <summary>
        /// Gets or sets the select count.
        /// </summary>
        /// <value>The select count.</value>
		public int SelectCount
		{
			get { return selectCount; }
			set { selectCount = value; }
		}
		#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorEventArgs"/> class.
        /// </summary>
        /// <param name="serverStorageValue">The server storage value.</param>
        /// <param name="selectCount">The select count.</param>
		public SelectorEventArgs(string serverStorageValue, int selectCount)
		{
			this.serverStorageValue = serverStorageValue;
			this.selectCount = selectCount;
		}
	}

    /// <summary>
    /// 
    /// </summary>
	public delegate void ValueChangedEventHandler(Object sender, SelectorEventArgs e);

    public interface ISelectorControl
    {
		string ContainerId { get;}
		string GridId { get; set;}
		bool IsAutoPostBack { get; set;}
		int PostDelayTime { get; set;	}
		bool AllowMultiSelect { get; set;}
		string ServerValuesId { get;}
		string ClientValuesId { get;}
		string UpdateElementId { get;}
		string ServerValues { get;}
		string ClientValues { get;}
		int SelectedCount { get;}

		event ValueChangedEventHandler ValueChange;
    }
}
