using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;
using System.Web;
using Mediachase.Commerce.Catalog.Events;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Engine.Events;

namespace Mediachase.Commerce.Catalog
{
    /// <summary>
    /// This class is reponsible for the following:
    /// 1. Loading all the plugins.
    /// 2. Binding those plugins to appropriate handlers
    /// 3. Providing methods to generate events
    /// </summary>
    public class EventContext
    {
        /// <summary>
        /// Handles the entry event.
        /// </summary>
        public delegate void EntryEventHandler(object sender, EntryEventArgs e);
        /// <summary>
        /// Handles the node event.
        /// </summary>
        public delegate void NodeEventHandler(object sender, NodeEventArgs e);
        /// <summary>
        /// Handles the catalog event.
        /// </summary>
        public delegate void CatalogEventHandler(object sender, CatalogEventArgs e);
        /// <summary>
        /// Handles the association event.
        /// </summary>
        public delegate void AssociationEventHandler(object sender, AssociationEventArgs e);
        /// <summary>
        /// Handles the relation event.
        /// </summary>
        public delegate void RelationEventHandler(object sender, RelationEventArgs e);

        #region Entry Events
        /// <summary>
        /// Occurs when [entry updating].
        /// </summary>
        public event EntryEventHandler EntryUpdating;
        /// <summary>
        /// Occurs when [entry updated].
        /// </summary>
        public event EntryEventHandler EntryUpdated;
        #endregion

        #region Node Events
        /// <summary>
        /// Occurs when [node updating].
        /// </summary>
        public event NodeEventHandler NodeUpdating;
        /// <summary>
        /// Occurs when [node updated].
        /// </summary>
        public event NodeEventHandler NodeUpdated;
        #endregion

        #region Catalog Events
        /// <summary>
        /// Occurs when [catalog updating].
        /// </summary>
        public event CatalogEventHandler CatalogUpdating;
        /// <summary>
        /// Occurs when [catalog updated].
        /// </summary>
        public event CatalogEventHandler CatalogUpdated;
        #endregion

        #region Association Events
        /// <summary>
        /// Occurs when [association updating].
        /// </summary>
        public event AssociationEventHandler AssociationUpdating;
        /// <summary>
        /// Occurs when [association updated].
        /// </summary>
        public event AssociationEventHandler AssociationUpdated;
        #endregion

        #region Relation Events
        /// <summary>
        /// Occurs when [relation updating].
        /// </summary>
        public event RelationEventHandler RelationUpdating;
        /// <summary>
        /// Occurs when [relation updated].
        /// </summary>
        public event RelationEventHandler RelationUpdated;
        #endregion

        // Keep handles to created instance so garbage collection does not remove them
        private ArrayList _Handles = new ArrayList();

        /// <summary>
        /// Returns an instance of event context class
        /// </summary>
        /// <value>The instance.</value>
        public static EventContext Instance
        {
            get
            {
                // Persist in thread
                if (HttpContext.Current == null)
                {
                    object ctxThread = Thread.GetData(Thread.GetNamedDataSlot("Mediachase-CatalogSystem-EventContext"));
                    if (ctxThread != null)
                        return (EventContext)ctxThread;

                    EventContext ctx = new EventContext();
                    Thread.SetData(Thread.GetNamedDataSlot("Mediachase-CatalogSystem-EventContext"), ctx);
                    return ctx;
                }

                // Persist in HttpContext
                if (HttpContext.Current.Items["Mediachase-CatalogSystem-EventContext"] == null)
                {
                    EventContext ctx = new EventContext();
                    HttpContext.Current.Items.Add("Mediachase-CatalogSystem-EventContext", ctx);
                    return ctx;
                }
                else
                    return (EventContext)HttpContext.Current.Items["Mediachase-CatalogSystem-EventContext"];
            }
        }

        #region Entry Event Methods
        /// <summary>
        /// Raises the entry updated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.EntryEventArgs"/> instance containing the event data.</param>
        public void RaiseEntryUpdatedEvent(CatalogEntryDto sender, EntryEventArgs args)
        {
            if (EntryUpdated != null)
                EntryUpdated(sender, args);
        }

        /// <summary>
        /// Raises the entry updating event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.EntryEventArgs"/> instance containing the event data.</param>
        public void RaiseEntryUpdatingEvent(CatalogEntryDto sender, EntryEventArgs args)
        {
            if (EntryUpdating != null)
                EntryUpdating(sender, args);
        }
        #endregion

        #region Node Event Methods
        /// <summary>
        /// Raises the node updated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.NodeEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeUpdatedEvent(CatalogNodeDto sender, NodeEventArgs args)
        {
            if (NodeUpdated != null)
                NodeUpdated(sender, args);
        }

        /// <summary>
        /// Raises the node updating event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.NodeEventArgs"/> instance containing the event data.</param>
        public void RaiseNodeUpdatingEvent(CatalogNodeDto sender, NodeEventArgs args)
        {
            if (NodeUpdating != null)
                NodeUpdating(sender, args);
        }
        #endregion

        #region Catalog Event Methods
        /// <summary>
        /// Raises the catalog updated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.CatalogEventArgs"/> instance containing the event data.</param>
        public void RaiseCatalogUpdatedEvent(CatalogDto sender, CatalogEventArgs args)
        {
            if (CatalogUpdated != null)
                CatalogUpdated(sender, args);
        }

        /// <summary>
        /// Raises the catalog updating event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.CatalogEventArgs"/> instance containing the event data.</param>
        public void RaiseCatalogUpdatingEvent(CatalogDto sender, CatalogEventArgs args)
        {
            if (CatalogUpdating != null)
                CatalogUpdating(sender, args);
        }
        #endregion

        #region Association Event Methods
        /// <summary>
        /// Raises the association updated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.AssociationEventArgs"/> instance containing the event data.</param>
        public void RaiseAssociationUpdatedEvent(CatalogAssociationDto sender, AssociationEventArgs args)
        {
            if (AssociationUpdated != null)
                AssociationUpdated(sender, args);
        }

        /// <summary>
        /// Raises the association updating event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.AssociationEventArgs"/> instance containing the event data.</param>
        public void RaiseAssociationUpdatingEvent(CatalogAssociationDto sender, AssociationEventArgs args)
        {
            if (AssociationUpdating != null)
                AssociationUpdating(sender, args);
        }
        #endregion

        #region Relation Event Methods
        /// <summary>
        /// Raises the relation updated event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.RelationEventArgs"/> instance containing the event data.</param>
        public void RaiseRelationUpdatedEvent(CatalogRelationDto sender, RelationEventArgs args)
        {
            if (RelationUpdated != null)
                RelationUpdated(sender, args);
        }

        /// <summary>
        /// Raises the relation updating event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.RelationEventArgs"/> instance containing the event data.</param>
        public void RaiseRelationUpdatingEvent(CatalogRelationDto sender, RelationEventArgs args)
        {
            if (RelationUpdating != null)
                RelationUpdating(sender, args);
        }
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="T:EventContext"/> class.
        /// </summary>
        private EventContext()
        {
            LoadInterfaces();
        }

        /// <summary>
        /// Loads interfaces and binds them to appropriate delegates
        /// </summary>
        private void LoadInterfaces()
        {
            // Find assembly that contains the needed class
            EventCollection events = CatalogConfiguration.Instance.Events;
            if (events != null)
            {
                foreach (EventDefinition eventDef in events)
                {
                    Type type = Type.GetType(eventDef.ClassName);

                    if (type == null)
                    {
                        // Log Exception here
                        continue;
                    }

                    if (type.IsAbstract)
                        continue;

                    foreach (Type intf in type.GetInterfaces())
                    {
                        if (intf == typeof(ICatalogEventListener))
                        {
                            ICatalogEventListener listener = (ICatalogEventListener)Activator.CreateInstance(type);

                            EntryUpdating += new EntryEventHandler(listener.EntryUpdating);
                            EntryUpdated += new EntryEventHandler(listener.EntryUpdated);

                            NodeUpdating += new NodeEventHandler(listener.NodeUpdating);
                            NodeUpdated += new NodeEventHandler(listener.NodeUpdated);

                            CatalogUpdating += new CatalogEventHandler(listener.CatalogUpdating);
                            CatalogUpdated += new CatalogEventHandler(listener.CatalogUpdated);

                            AssociationUpdating += new AssociationEventHandler(listener.AssociationUpdating);
                            AssociationUpdated += new AssociationEventHandler(listener.AssociationUpdated);

                            RelationUpdating += new RelationEventHandler(listener.RelationUpdating);
                            RelationUpdated += new RelationEventHandler(listener.RelationUpdated);

                            // Save a handle
                            _Handles.Add(new ObjectHandle(listener));
                        }
                    }
                }
            }
        }
    }
}
