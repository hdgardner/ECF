using System;
using System.Workflow.Runtime;
using System.Collections.Generic;
using System.Workflow.ComponentModel;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Engine
{
    /// <summary>
    /// Implements operations for the work flow results.
    /// </summary>
    public class WorkflowResults
    {
        #region FactoryMethods

        /// <summary>
        /// Creates the completed workflow results.
        /// </summary>
        /// <param name="args">The <see cref="System.Workflow.Runtime.WorkflowCompletedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static WorkflowResults CreateCompletedWorkflowResults
                                        (WorkflowCompletedEventArgs args)
        {
            WorkflowResults results = new WorkflowResults(args);
            results._status = WorkflowStatus.Completed;
            return results;
        }

        /// <summary>
        /// Creates the terminated workflow results.
        /// </summary>
        /// <param name="args">The <see cref="System.Workflow.Runtime.WorkflowTerminatedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static WorkflowResults CreateTerminatedWorkflowResults
                                       (WorkflowTerminatedEventArgs args)
        {
            WorkflowResults results = new WorkflowResults(args);
            results._status = WorkflowStatus.Terminated;
            return results;
        }

        /// <summary>
        /// Creates the aborted workflow results.
        /// </summary>
        /// <param name="args">The <see cref="System.Workflow.Runtime.WorkflowEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static WorkflowResults CreateAbortedWorkflowResults(WorkflowEventArgs args)
        {
            WorkflowResults results = new WorkflowResults(args);
            results._status = WorkflowStatus.Aborted;
            return results;
        }

        /// <summary>
        /// Creates the running workflow results.
        /// </summary>
        /// <param name="args">The <see cref="System.Workflow.Runtime.WorkflowEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static WorkflowResults CreateRunningWorkflowResults(WorkflowEventArgs args)
        {
            WorkflowResults results = new WorkflowResults(args);
            results._status = WorkflowStatus.Running;
            return results;
        }

        #endregion

        #region Private Ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowResults"/> class.
        /// </summary>
        /// <param name="args">The <see cref="System.Workflow.Runtime.WorkflowCompletedEventArgs"/> instance containing the event data.</param>
        private WorkflowResults(WorkflowCompletedEventArgs args)
        {
            Check.ArgumentIsNotNull(args, "args");
            _outputs = args.OutputParameters;
            _instanceId = args.WorkflowInstance.InstanceId;

            // Getting definition is expensive and should be avoided
            //_definition = args.WorkflowDefinition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowResults"/> class.
        /// </summary>
        /// <param name="args">The <see cref="System.Workflow.Runtime.WorkflowTerminatedEventArgs"/> instance containing the event data.</param>
        private WorkflowResults(WorkflowTerminatedEventArgs args)
        {
            Check.ArgumentIsNotNull(args, "args");
            _instanceId = args.WorkflowInstance.InstanceId;
            _exception = args.Exception;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowResults"/> class.
        /// </summary>
        /// <param name="args">The <see cref="System.Workflow.Runtime.WorkflowEventArgs"/> instance containing the event data.</param>
        private WorkflowResults(WorkflowEventArgs args)
        {
            Check.ArgumentIsNotNull(args, "args");
            _instanceId = args.WorkflowInstance.InstanceId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowResults"/> class.
        /// </summary>
        private WorkflowResults()
        {
        }

        #endregion

        private WorkflowStatus _status;
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        public WorkflowStatus Status
        {
            get { return _status; }
        }

        private Dictionary<string, object> _outputs;
        /// <summary>
        /// Gets the output parameters.
        /// </summary>
        /// <value>The output parameters.</value>
        public Dictionary<string, object> OutputParameters
        {
            get { return _outputs; }
        }

        private Guid _instanceId;
        /// <summary>
        /// Gets the instance id.
        /// </summary>
        /// <value>The instance id.</value>
        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        /* too expensive for runtime
        private Activity _definition;
        /// <summary>
        /// Gets the workflow definition.
        /// </summary>
        /// <value>The workflow definition.</value>
        public Activity WorkflowDefinition
        {
            get { return _definition; }
        }
         * */

        private Exception _exception;
        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception
        {
            get { return _exception; }
        }
    } 
}