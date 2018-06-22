using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using System.Collections;
using log4net.Core;
using System.Data;
using log4net.Util;
using System.IO;
using Mediachase.Commerce.Core.Managers;

namespace Mediachase.Commerce.Core.Log.Appender
{
    /// <summary>
    /// Appender that logs to a database.
    /// </summary>
    public class AppDatabaseAppender : BufferingAppenderSkeleton
    {
        #region Public Instance Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDatabaseAppender"/> class.
        /// </summary>
        public AppDatabaseAppender()
        {
        }

        #endregion // Public Instance Constructors

        #region Public Instance Properties
        #endregion // Public Instance Properties

        #region Protected Instance Properties
        #endregion // Protected Instance Properties

        #region Implementation of IOptionHandler

        /// <summary>
        /// Initialize the appender based on the options set
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is part of the <see cref="IOptionHandler"/> delayed object
        /// activation scheme. The <see cref="ActivateOptions"/> method must 
        /// be called on this object after the configuration properties have
        /// been set. Until <see cref="ActivateOptions"/> is called this
        /// object is in an undefined state and must not be used. 
        /// </para>
        /// <para>
        /// If any of the configuration properties are modified then 
        /// <see cref="ActivateOptions"/> must be called again.
        /// </para>
        /// </remarks>
        override public void ActivateOptions()
        {
            base.ActivateOptions();
        }

        #endregion

        #region Override implementation of AppenderSkeleton

        /// <summary>
        /// Override the parent method to close the database
        /// </summary>
        /// <remarks>
        /// <para>
        /// Closes the database command and database connection.
        /// </para>
        /// </remarks>
        override protected void OnClose()
        {
            base.OnClose();
        }

        #endregion

        #region Override implementation of BufferingAppenderSkeleton

        /// <summary>
        /// Inserts the events into the database.
        /// </summary>
        /// <param name="events">The events to insert into the database.</param>
        /// <remarks>
        /// <para>
        /// Insert all the events specified in the <paramref name="events"/>
        /// array into the database.
        /// </para>
        /// </remarks>
        override protected void SendBuffer(LoggingEvent[] events)
        {
            foreach (LoggingEvent e in events)
            {
                LogManager.WriteLog(e.Level.DisplayName, e.Identity, e.LocationInformation.MethodName, e.LoggerName, "SYSTEM", e.ExceptionObject != null ? e.MessageObject.ToString() + "\n\r " + e.LocationInformation.FullInfo + "\n\r " + e.ExceptionObject.Message : e.MessageObject.ToString(), e.ExceptionObject == null ? true : false);
            }
        }

        #endregion // Override implementation of BufferingAppenderSkeleton

        #region Public Instance Methods
        #endregion // Public Instance Methods

        #region Protected Instance Methods
        #endregion // Protected Instance Methods

        #region Protected Instance Fields
        #endregion // Protected Instance Fields

        #region Private Instance Fields
        #endregion // Private Instance Fields
    }
}
