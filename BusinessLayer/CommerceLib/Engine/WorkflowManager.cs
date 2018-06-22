using System;
using System.Collections.Generic;
using System.Text;
using System.Workflow.Runtime;
using Mediachase.Commerce.Orders;
using System.Xml;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.Runtime.Hosting;
using Mediachase.Commerce.Shared;

namespace Mediachase.Commerce.Engine
{
    /// <summary>
    /// Implements operations for the work flow manager.
    /// </summary>
    public static class WorkflowManager
    {
        //
        /// <summary>
        /// Executes the workflow.
        /// </summary>
        /// <param name="workflowName">Name of the workflow.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="instanceId">The instance id.</param>
        public static void ExecuteWorkflow(string workflowName, Dictionary<string, object> parameters, Guid instanceId)
        {
            // Retrieve definition
            WorkflowDefinition workflowDefinition = WorkflowConfiguration.Instance.GetWorkflow(workflowName);
            if (workflowDefinition == null)
            {
                throw new InvalidOperationException(String.Format("Workflow \"{0}\" is not defined in the configuration file.", workflowName));
            }

            // Create workflow runtime
             WorkflowRuntime runtime = OrderContext.Current.WorkflowRuntime;

            //using (WorkflowRuntime runtime = new WorkflowRuntime())
            {
                //runtime.AddService(new SynchronizationContextSchedulerService(true));

                Type workflowType = null;
                if (!String.IsNullOrEmpty(workflowDefinition.ClassName))
                {
                    workflowType = Type.GetType(workflowDefinition.ClassName);
                    if (workflowType == null)
                    {
                        throw new TypeLoadException(workflowDefinition.ClassName);
                    }

                    /*
                    TypeProvider typeProvider = new TypeProvider(_Runtime);
                    typeProvider.AddAssembly(workflowType.Assembly);
                    _Runtime.AddService(typeProvider);
                     * */
                }


                XmlReader xomlReader = null;
                if (!String.IsNullOrEmpty(workflowDefinition.Path))
                    xomlReader = XmlReader.Create(workflowDefinition.Path);

                XmlReader rulesReader = null;
                if (!String.IsNullOrEmpty(workflowDefinition.Rules))
                    rulesReader = XmlReader.Create(workflowDefinition.Rules);

                WorkflowInstance workflowInstance = null;

                try
                {
                    if (xomlReader == null)
                        workflowInstance = runtime.CreateWorkflow(workflowType, parameters, instanceId);
                    else
                        workflowInstance = runtime.CreateWorkflow(xomlReader, rulesReader, parameters, instanceId);
                }
                catch (WorkflowValidationFailedException)
                {
                    /*
                    foreach (ValidationError error in ex.Errors)
                    {
                        Console.Write(error.ToString());
                    }
                     * */

                    throw;
                }

                // Start the Workflow
                workflowInstance.Start();

                // Start manual service
                bool result = runtime.GetService<ManualWorkflowSchedulerService>().RunWorkflow(instanceId);

                Check.IsTrue(result, "Could not run workflow " + instanceId);
            }
        }
    }
}
