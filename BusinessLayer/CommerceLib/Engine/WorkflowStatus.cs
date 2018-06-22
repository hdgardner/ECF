using System;

namespace Mediachase.Commerce.Engine
{
    /// <summary>
    /// The WorkflowStatus enumeration defines the work flow status.
    /// </summary>
    public enum WorkflowStatus
    {
        /// <summary>
        /// Represents the completed status.
        /// </summary>
        Completed,
        /// <summary>
        /// Represents the terminated status.
        /// </summary>
        Terminated,
        /// <summary>
        /// Represents the aborted status.
        /// </summary>
        Aborted,
        /// <summary>
        /// Represents the running status.
        /// </summary>
        Running
    } 
}