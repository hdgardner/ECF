using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Mediachase.Cms.WebUtility.UI
{
    public delegate void ErrorEventHandler(object sender, ErrorEventArgs e);

    public class ErrorEventArgs : System.EventArgs
    {
        private string _Message = "";
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get
            {
                return _Message;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ErrorEventArgs"/> class.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public ErrorEventArgs(string msg)
        {
            _Message = msg;
        }
    }

    /// <summary>
    /// Summary description for ErrorManager.
    /// </summary>
    public class ErrorManager
    {
        #region Event handler
        public event ErrorEventHandler Error;

        // Invoke the Changed event; called whenever list changes
        /// <summary>
        /// Raises the <see cref="E:Error"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Mediachase.Cms.WebUtility.UI.ErrorEventArgs"/> instance containing the event data.</param>
        protected virtual void OnError(ErrorEventArgs e)
        {
            if (Error != null)
                Error(this, e);
        }
        #endregion

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ErrorManager Instance
        {
            get
            {
                if (HttpContext.Current.Items["ErrorManager"] == null)
                {
                    HttpContext.Current.Items["ErrorManager"] = new ErrorManager();
                }

                return (ErrorManager)HttpContext.Current.Items["ErrorManager"];
            }
        }

        /// <summary>
        /// Generates the error.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        public static void GenerateError(string msg)
        {
            ErrorManager man = ErrorManager.Instance;
            ErrorEventArgs args = new ErrorEventArgs(msg);
            if (man.Error != null)
                man.Error(man, args);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorManager"/> class.
        /// </summary>
        ErrorManager()
        {
        }
    }
}
