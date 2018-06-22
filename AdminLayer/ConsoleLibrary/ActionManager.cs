using System;
using System.Collections.Generic;
using System.Text;
using System.Web;


namespace Mediachase.Web.Console
{
    public delegate void ActionEventHandler(object sender, ActionEventArgs e);

    public class ActionEventArgs : System.EventArgs
    {
        private string _Name = "";
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        private string[] _Args;
        /// <summary>
        /// Gets the args.
        /// </summary>
        /// <value>The args.</value>
        public string[] Args
        {
            get
            {
                return _Args;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionEventArgs"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        public ActionEventArgs(string name, string args)
        {
            _Name = name;
            if (!String.IsNullOrEmpty(args))
                _Args = args.Split(new char[] { ',' });
        }
    }

    /// <summary>
	/// Summary description for ActionManager.
    /// </summary>
    public class ActionManager
    {
        #region Action handler
        public event ActionEventHandler Action;

        // Invoke the Changed event; called whenever list changes
        /// <summary>
        /// Raises the <see cref="E:Action"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Mediachase.Web.Console.ActionEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAction(ActionEventArgs e)
        {
            if (Action != null)
                Action(this, e);
        }
        #endregion

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ActionManager Instance
        {
            get
            {
                if (HttpContext.Current.Items["ActionManager"] == null)
                {
                    HttpContext.Current.Items["ActionManager"] = new ActionManager();
                }

                return (ActionManager)HttpContext.Current.Items["ActionManager"];
            }
        }

        /// <summary>
        /// Generates the action.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="args">The args.</param>
        public static void GenerateAction(string name, string args)
        {
            ActionManager man = ActionManager.Instance;
            man.Action(man, new ActionEventArgs(name, args));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionManager"/> class.
        /// </summary>
        ActionManager()
        {
        }
    }

}

