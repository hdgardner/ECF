using System;
using System.Threading;
using System.Workflow.Runtime.Hosting;
using System.Collections.Generic;

namespace Mediachase.Commerce.Engine
{
    /// <summary>
    /// Implements operations for the synchronization context scheduler service. (Inherits <see cref="WorkflowSchedulerService"/>.)
    /// </summary>
    public sealed class SynchronizationContextSchedulerService : WorkflowSchedulerService
    {
        bool synchronousDispatch = true;
        Dictionary<Guid, Timer> timers = new Dictionary<Guid, Timer>();
        SynchronizationContext originalContext = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextSchedulerService"/> class.
        /// </summary>
        public SynchronizationContextSchedulerService()
            : this(true)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizationContextSchedulerService"/> class.
        /// </summary>
        /// <param name="synchronousDispatch">if set to <c>true</c> [synchronous dispatch].</param>
        public SynchronizationContextSchedulerService(bool synchronousDispatch)
        {
            this.originalContext = SynchronizationContext.Current;
            this.synchronousDispatch = synchronousDispatch;
        }
        /// <summary>
        /// Gets a value indicating whether [synchronous dispatch].
        /// </summary>
        /// <value><c>true</c> if [synchronous dispatch]; otherwise, <c>false</c>.</value>
        public bool SynchronousDispatch
        {
            get { return this.synchronousDispatch; }
        }

        /// <summary>
        /// When overridden in a derived class, this method is called by the runtime to schedule a work item (callback) for a particular instance ID.
        /// </summary>
        /// <param name="callback">A <see cref="T:System.Threading.WaitCallback"/> multicast delegate that represents the method to run.</param>
        /// <param name="workflowInstanceId">A <see cref="T:System.Guid"/> that represents the workflow instance.</param>
        protected override void Schedule(WaitCallback callback, Guid workflowInstanceId)
        {
            //if the captured context on the thread that created the WF runtime
            //is null, try obtaining the Synch Context of the current thread
            SynchronizationContext ctx = this.originalContext != null ? this.originalContext : SynchronizationContext.Current;
            if (ctx != null)
            {
                if (this.SynchronousDispatch)
                    ctx.Send(delegate
                    {
                        callback(workflowInstanceId);
                    }, null);
                else
                    ctx.Post(delegate
                    {
                        callback(workflowInstanceId);
                    }, null);
            }
            else //oh well, run the scheduler's dispatch loop w/o a synch context
                callback(workflowInstanceId);
        }
        /// <summary>
        /// When overridden in a derived class, this method is called by the runtime to schedule a work item (callback) for a particular workflow instance to be done at the given time (<see cref="T:System.DateTime"/>).
        /// </summary>
        /// <param name="callback">A <see cref="T:System.Threading.WaitCallback"/> multicast delegate that represents the method to run.</param>
        /// <param name="workflowInstanceId">A <see cref="T:System.Guid"/> that represents the workflow instance to add.</param>
        /// <param name="whenUtc">The <see cref="T:System.DateTime"/> to begin running the workflow item.</param>
        /// <param name="timerId">A <see cref="T:System.Guid"/> that represents the scheduled timer.</param>
        protected override void Schedule(WaitCallback callback, Guid workflowInstanceId, DateTime whenUtc, Guid timerId)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan span = (whenUtc > now) ? whenUtc - now : TimeSpan.Zero;
            this.timers.Add(timerId, new Timer(delegate { this.Schedule(callback, workflowInstanceId); },
                timerId, span, new TimeSpan(Timeout.Infinite)));
        }

        /// <summary>
        /// Cancels the specified timer GUID.
        /// </summary>
        /// <param name="timerGuid">The timer GUID.</param>
        protected override void Cancel(Guid timerGuid)
        {
            ((IDisposable)this.timers[timerGuid]).Dispose();
            this.timers.Remove(timerGuid);
        }

        /// <summary>
        /// When overridden in a derived class, represents the method that will be called when the workflow runtime engine raises the <see cref="E:System.Workflow.Runtime.WorkflowRuntime.Stopped"/> event.
        /// </summary>
        protected override void OnStopped()
        {
            foreach (Timer timer in this.timers.Values)
                ((IDisposable)timer).Dispose();

            this.timers.Clear();
            base.OnStopped();
        }
    }
}
