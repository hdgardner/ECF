using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Commerce.Catalog.Events
{
    /// <summary>
    /// Provides the methods necessary for the catalog event listener.
    /// </summary>
    public interface ICatalogEventListener
    {
        #region Entry Events
        /// <summary>
        /// Entries updating.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.EntryEventArgs"/> instance containing the event data.</param>
        void EntryUpdating(object source, EntryEventArgs args);
        /// <summary>
        /// Entries updated.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.EntryEventArgs"/> instance containing the event data.</param>
        void EntryUpdated(object source, EntryEventArgs args);
        #endregion

        #region Node Events
        /// <summary>
        /// Nodes updating.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.NodeEventArgs"/> instance containing the event data.</param>
        void NodeUpdating(object source, NodeEventArgs args);
        /// <summary>
        /// Nodes updated.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.NodeEventArgs"/> instance containing the event data.</param>
        void NodeUpdated(object source, NodeEventArgs args);
        #endregion

        #region Catalog Events
        /// <summary>
        /// Catalogs updating.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.CatalogEventArgs"/> instance containing the event data.</param>
        void CatalogUpdating(object source, CatalogEventArgs args);
        /// <summary>
        /// Catalogs updated.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.CatalogEventArgs"/> instance containing the event data.</param>
        void CatalogUpdated(object source, CatalogEventArgs args);
        #endregion

        #region Association Events
        /// <summary>
        /// Associations updating.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.AssociationEventArgs"/> instance containing the event data.</param>
        void AssociationUpdating(object source, AssociationEventArgs args);
        /// <summary>
        /// Associations updated.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.AssociationEventArgs"/> instance containing the event data.</param>
        void AssociationUpdated(object source, AssociationEventArgs args);
        #endregion

        #region Relation Events
        /// <summary>
        /// Relations updating.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.RelationEventArgs"/> instance containing the event data.</param>
        void RelationUpdating(object source, RelationEventArgs args);
        /// <summary>
        /// Relations updated.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The <see cref="Mediachase.Commerce.Catalog.Events.RelationEventArgs"/> instance containing the event data.</param>
        void RelationUpdated(object source, RelationEventArgs args);
        #endregion
    }
}