using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mediachase.Cms.WebActionSet
{
    public class Action
    {
        #region Allow
        private ArrayList allow;

        /// <summary>
        /// Gets or sets the allowed roles.
        /// </summary>
        /// <value>The allow.</value>
        public ArrayList Allow
        {
            get { return allow; }
            set { allow = value; }
        } 
        #endregion

        #region Deny
        private ArrayList deny;

        /// <summary>
        /// Gets or sets the denyed roles.
        /// </summary>
        /// <value>The deny.</value>
        public ArrayList Deny
        {
            get { return deny; }
            set { deny = value; }
        } 
        #endregion

        #region Event
        private string _event;

        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        /// <value>The event.</value>
        public string Event
        {
            get { return _event; }
            set { _event = value; }
        } 
        #endregion

        #region Code
        private string code;

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code
        {
            get { return code; }
            set { code = value; }
        } 
        #endregion

        #region Script
        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <value>The script.</value>
        public string Script
        {
            get
            {
                return "obj."+Event+" = function(){"+Code+ "};";
            }
        } 
        #endregion

        #region ctr:Action()
        /// <summary>
        /// Initializes a new instance of the <see cref="Action"/> class.
        /// </summary>
        public Action()
        {
            Allow = new ArrayList();
            Deny = new ArrayList();
            Event = String.Empty;
            Code = String.Empty;
        } 
        #endregion
    }
}
