using System;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Mediachase.Cms;
using Mediachase.Cms.Data;
using Mediachase.Data.Provider;

namespace Mediachase.Cms.Pages
{
	/// <summary>
	/// Summary description for DBPageDocument
	/// </summary>
	public class Database
	{
		public static class PageDocument
		{
			#region PageDocumentAdd
            /// <summary>
            /// Adds the specified page version id.
            /// </summary>
            /// <param name="pageVersionId">The page version id.</param>
            /// <returns></returns>
			public static int Add( int pageVersionId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_PageDocument_Add]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageVersionId", pageVersionId, DataParameterType.Int));
				return DataService.RunReturnInteger(cmd);
			}
			#endregion

			#region PageDocumentUpdate
            /// <summary>
            /// Updates the specified page id.
            /// </summary>
            /// <param name="pageId">The page id.</param>
            /// <param name="uid">The uid.</param>
            /// <param name="pageVersionId">The page version id.</param>
			public static void Update(int pageId, string uid, int pageVersionId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_PageDocument_Update]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("PageVersionId", pageVersionId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("UID", uid, DataParameterType.NVarChar, 255));
				DataService.Run(cmd);
			}
			#endregion

			#region PageDocumentDelete
            /// <summary>
            /// Deletes the specified page id.
            /// </summary>
            /// <param name="pageId">The page id.</param>
			public static void Delete(int pageId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_PageDocument_Delete]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
				DataService.Run(cmd);
			}
			#endregion

			#region PageDocumentGetById
            /// <summary>
            /// Gets the by id.
            /// </summary>
            /// <param name="pageId">The page id.</param>
            /// <returns></returns>
			public static IDataReader GetById(int pageId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_PageDocument_GetById]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region PageDocumentGetByPageVersionId
            /// <summary>
            /// Gets the by page version id.
            /// </summary>
            /// <param name="pageVersionId">The page version id.</param>
            /// <returns></returns>
			public static IDataReader GetByPageVersionId(int pageVersionId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_PageDocument_GetByPageVersionId]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageVersionId", pageVersionId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion
		}

		public static class Node
		{
			#region NodeAdd
            /// <summary>
            /// Adds the specified page id.
            /// </summary>
            /// <param name="pageId">The page id.</param>
            /// <param name="nodeTypeId">The node type id.</param>
            /// <param name="nodeUID">The node UID.</param>
            /// <param name="factoryUID">The factory UID.</param>
            /// <param name="factoryControlUID">The factory control UID.</param>
            /// <param name="controlPlaceId">The control place id.</param>
            /// <param name="controlPlaceIndex">Index of the control place.</param>
            /// <returns></returns>
			public static int Add(int pageId, int nodeTypeId, string nodeUID, string factoryUID,
				string factoryControlUID, string controlPlaceId, int controlPlaceIndex)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Node_Add]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("NodeTypeId", nodeTypeId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("NodeUID", nodeUID, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("FactoryUID", factoryUID, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("FactoryControlUID", factoryControlUID, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("ControlPlaceId", controlPlaceId, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("ControlPlaceIndex", controlPlaceIndex, DataParameterType.Int));
				return DataService.RunReturnInteger(cmd);
			}

			#endregion

			#region NodeUpdate
            /// <summary>
            /// Updates the specified node id.
            /// </summary>
            /// <param name="nodeId">The node id.</param>
            /// <param name="pageId">The page id.</param>
            /// <param name="nodeTypeId">The node type id.</param>
            /// <param name="nodeUID">The node UID.</param>
            /// <param name="factoryUID">The factory UID.</param>
            /// <param name="factoryControlUID">The factory control UID.</param>
            /// <param name="controlPlaceId">The control place id.</param>
            /// <param name="controlPlaceIndex">Index of the control place.</param>
			public static void Update(int nodeId, int pageId, int nodeTypeId, string nodeUID, string factoryUID,
				string factoryControlUID, string controlPlaceId, int controlPlaceIndex)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Node_Update]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("NodeId", nodeId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("NodeTypeId", nodeTypeId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("NodeUID", nodeUID, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("FactoryUID", factoryUID, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("FactoryControlUID", factoryControlUID, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("ControlPlaceId", controlPlaceId, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("ControlPlaceIndex", controlPlaceIndex, DataParameterType.Int));
				DataService.Run(cmd);
			}
			#endregion

			#region NodeDelete
            /// <summary>
            /// Deletes the specified node id.
            /// </summary>
            /// <param name="nodeId">The node id.</param>
			public static void Delete(int nodeId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Node_Delete]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("NodeId", nodeId, DataParameterType.Int));
				DataService.Run(cmd);
			}
			#endregion

			#region NodeGetById
            /// <summary>
            /// Gets the by id.
            /// </summary>
            /// <param name="nodeId">The node id.</param>
            /// <returns></returns>
			public static IDataReader GetById(int nodeId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Node_GetById]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("NodeId", nodeId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region NodeGetByPageId
            /// <summary>
            /// Gets the by page id.
            /// </summary>
            /// <param name="pageId">The page id.</param>
            /// <returns></returns>
			public static IDataReader GetByPageId(int pageId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Node_GetByPageId]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region NodeGetByControlPlaceId
            /// <summary>
            /// Gets the by control place id.
            /// </summary>
            /// <param name="controlPlaceId">The control place id.</param>
            /// <returns></returns>
			public static IDataReader GetByControlPlaceId(string controlPlaceId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Node_GetByControlPlaceId]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlPlaceId", controlPlaceId, DataParameterType.NVarChar, 255));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region NodeGetByUID
            /// <summary>
            /// Gets the by UID.
            /// </summary>
            /// <param name="nodeUID">The node UID.</param>
            /// <param name="pageId">The page id.</param>
            /// <returns></returns>
			public static IDataReader GetByUID(string nodeUID, int pageId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Node_GetByUID]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("NodeUID", nodeUID, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("PageId", pageId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			} 
			#endregion
		}

		public static class NodeType
		{
            #region Type
            public enum Type
            {
                StaticNode = 1,
                DynamicNode = 2

            }
            #endregion

			#region NodeTypeAdd
            /// <summary>
            /// Adds the specified type name.
            /// </summary>
            /// <param name="typeName">Name of the type.</param>
            /// <returns></returns>
			public static int Add(string typeName)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_NodeType_Add]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("TypeName", typeName, DataParameterType.NVarChar, 255));
				return DataService.RunReturnInteger(cmd);
			}
			#endregion

			#region NodeTypeUpdate
            /// <summary>
            /// Updates the specified node type id.
            /// </summary>
            /// <param name="nodeTypeId">The node type id.</param>
            /// <param name="typeName">Name of the type.</param>
			public static void Update(int nodeTypeId, string typeName)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_NodeType_Update]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("TypeName", typeName, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("NodeTypeId", nodeTypeId, DataParameterType.NVarChar, 255));
				DataService.Run(cmd);
			}

			#endregion

			#region NodeTypeDelete
            /// <summary>
            /// Deletes the specified node type id.
            /// </summary>
            /// <param name="nodeTypeId">The node type id.</param>
			public static void Delete(int nodeTypeId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_NodeType_Delete]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("NodeTypeId", nodeTypeId, DataParameterType.NVarChar, 255));
				DataService.Run(cmd);
			}
			#endregion

			#region NodeTypeGetById
            /// <summary>
            /// Gets the by id.
            /// </summary>
            /// <param name="nodeTypeId">The node type id.</param>
            /// <returns></returns>
			public static IDataReader GetById(int nodeTypeId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_NodeType_GetById]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("NodeTypeId", nodeTypeId, DataParameterType.NVarChar, 255));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion
		}

		public static class Control
		{
			#region ControlAdd
            /// <summary>
            /// Adds the specified node id.
            /// </summary>
            /// <param name="nodeId">The node id.</param>
            /// <param name="controlUID">The control UID.</param>
            /// <returns></returns>
			public static int Add(int nodeId, string controlUID)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Control_Add]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("NodeId", nodeId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("ControlUID", controlUID, DataParameterType.NVarChar, 255));
				return DataService.RunReturnInteger(cmd);
			}
			#endregion

			#region ControlUpdate
            /// <summary>
            /// Updates the specified control id.
            /// </summary>
            /// <param name="controlId">The control id.</param>
            /// <param name="nodeId">The node id.</param>
            /// <param name="controlUID">The control UID.</param>
			public static void Update(int controlId, int nodeId, string controlUID)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Control_Update]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlID", controlId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("NodeId", nodeId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("ControlUID", controlUID, DataParameterType.NVarChar, 255));
				DataService.Run(cmd);
			}
			#endregion

			#region ControlDelete
            /// <summary>
            /// Deletes the specified control id.
            /// </summary>
            /// <param name="controlId">The control id.</param>
			public static void Delete(int controlId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Control_Delete]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlID", controlId, DataParameterType.Int));
				DataService.Run(cmd);
			}
			#endregion

			#region ControlGetById
            /// <summary>
            /// Gets the by id.
            /// </summary>
            /// <param name="controlId">The control id.</param>
            /// <returns></returns>
			public static IDataReader GetById(int controlId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Control_GetById]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlID", controlId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region ControlGetByNodeId
            /// <summary>
            /// Gets the by node id.
            /// </summary>
            /// <param name="nodeId">The node id.</param>
            /// <returns></returns>
			public static IDataReader GetByNodeId(int nodeId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Control_GetByNodeId]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("NodeId", nodeId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region ControlGetByUID
            /// <summary>
            /// Gets the by UID.
            /// </summary>
            /// <param name="controlUID">The control UID.</param>
            /// <returns></returns>
			public static IDataReader GetByUID(string controlUID)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_Control_GetByUID]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlUID", controlUID, DataParameterType.NVarChar, 255));
				return DataService.LoadReader(cmd).DataReader;
			} 
			#endregion
		}

		public static class ControlStorage
		{
			#region ControlStorageAdd
            /// <summary>
            /// Adds the specified control id.
            /// </summary>
            /// <param name="controlId">The control id.</param>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            /// <returns></returns>
			public static int Add(int controlId, string key, Byte[] value)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_ControlStorage_Add]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlID", controlId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("Key", key, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("Value", value != null ? value : (object)DBNull.Value, DataParameterType.Image));
				return DataService.RunReturnInteger(cmd);
			}
			#endregion

			#region ControlStorageUpdate
            /// <summary>
            /// Updates the specified control storage id.
            /// </summary>
            /// <param name="controlStorageId">The control storage id.</param>
            /// <param name="controlId">The control id.</param>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
			public static void Update(int controlStorageId, int controlId, string key, Byte[] value)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_ControlStorage_Update]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlStorageID", controlStorageId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("ControlID", controlId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("Key", key, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("Value", value != null ? value : (object)DBNull.Value, DataParameterType.Image));
				DataService.Run(cmd);
			}
			#endregion

			#region ControlStorageDelete
            /// <summary>
            /// Deletes the specified control storage id.
            /// </summary>
            /// <param name="controlStorageId">The control storage id.</param>
			public static void Delete(int controlStorageId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_ControlStorage_Delete]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlStorageID", controlStorageId, DataParameterType.Int));
				DataService.Run(cmd);
			}
			#endregion

			#region ControlStorageGetById
            /// <summary>
            /// Gets the by id.
            /// </summary>
            /// <param name="controlStorageId">The control storage id.</param>
            /// <returns></returns>
			public static IDataReader GetById(int controlStorageId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_ControlStorage_GetById]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlStorageID", controlStorageId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region ControlStorageGetBycontrolId
            /// <summary>
            /// Gets the by control id.
            /// </summary>
            /// <param name="controlId">The control id.</param>
            /// <returns></returns>
			public static IDataReader GetByControlId(int controlId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_ControlStorage_GetByControlId]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlID", controlId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion
		}

		public static class ControlSettings
		{
			#region ControlSettingsGetByControlId
            /// <summary>
            /// Gets the by control id.
            /// </summary>
            /// <param name="controlId">The control id.</param>
            /// <returns></returns>
			public static IDataReader GetByControlId(int controlId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_ControlSettings_GetBycontrolId]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("ControlID", controlId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region ControlSettingsGetByKeyAndcontrolId
            /// <summary>
            /// Gets the by key and control id.
            /// </summary>
            /// <param name="key">The key.</param>
            /// <param name="controlId">The control id.</param>
            /// <returns></returns>
			public static IDataReader GetByKeyAndControlId(string key, int controlId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_ControlSettings_GetByKeyAndcontrolId]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("Key", key, DataParameterType.NVarChar, 255));
				cmd.Parameters.Add(new DataParameter("ControlID", controlId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			} 
			#endregion
		}

		public static class TemporaryDBStorage
		{
			#region TemporaryDBStorage_Add
            /// <summary>
            /// Adds the specified page version id.
            /// </summary>
            /// <param name="pageVersionId">The page version id.</param>
            /// <param name="expire">The expire.</param>
            /// <param name="pageDocument">The page document.</param>
            /// <param name="userUID">The user UID.</param>
            /// <returns></returns>
            public static int Add(int pageVersionId, int expire, byte[] pageDocument, Guid userUID)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_TemporaryStorage_Add]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageVersionId", pageVersionId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("Expire", expire, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("PageDocument", pageDocument, DataParameterType.Image));
				cmd.Parameters.Add(new DataParameter("CreatorUID", userUID, DataParameterType.UniqueIdentifier));
				return DataService.RunReturnInteger(cmd);
			}
			#endregion

			#region TemporaryDBStorage_Update
            /// <summary>
            /// Updates the specified storage id.
            /// </summary>
            /// <param name="storageId">The storage id.</param>
            /// <param name="pageVersionId">The page version id.</param>
            /// <param name="expire">The expire.</param>
            /// <param name="pageDocument">The page document.</param>
            /// <param name="userUID">The user UID.</param>
			public static void Update(int storageId, int pageVersionId, int expire, byte[] pageDocument, Guid userUID)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_TemporaryStorage_Update]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("StorageId", storageId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("PageVersionId", pageVersionId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("Expire", expire, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("PageDocument", pageDocument, DataParameterType.Image));
				cmd.Parameters.Add(new DataParameter("CreatorUID", userUID, DataParameterType.UniqueIdentifier));
				DataService.Run(cmd);
			} 
			#endregion

			#region TemporaryDBStorage_Delete
            /// <summary>
            /// Deletes the specified storage id.
            /// </summary>
            /// <param name="storageId">The storage id.</param>
			public static void Delete(int storageId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_TemporaryStorage_Delete]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("StorageId", storageId, DataParameterType.Int));
				DataService.Run(cmd);
			}
			#endregion

			#region TemporaryDBStorage_GetById
            /// <summary>
            /// Gets the by id.
            /// </summary>
            /// <param name="storageId">The storage id.</param>
            /// <returns></returns>
			public static IDataReader GetById(int storageId)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_TemporaryStorage_GetById]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("StorageId", storageId, DataParameterType.Int));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

			#region TemporaryDBStorage_GetByPageVersionId
            /// <summary>
            /// Gets the by page version id.
            /// </summary>
            /// <param name="pageVersionId">The page version id.</param>
            /// <param name="userUID">The user UID.</param>
            /// <returns></returns>
            public static IDataReader GetByPageVersionId(int pageVersionId, Guid userUID)
			{
				DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_TemporaryStorage_GetByPageVersionId]");
				cmd.Parameters = new DataParameters();
				cmd.Parameters.Add(new DataParameter("PageVersionId", pageVersionId, DataParameterType.Int));
				cmd.Parameters.Add(new DataParameter("CreatorUID", userUID, DataParameterType.UniqueIdentifier));
				return DataService.LoadReader(cmd).DataReader;
			}
			#endregion

            #region TemporaryDBStorage_GetByPageVersionId
            /// <summary>
            /// Gets the by page version id.
            /// </summary>
            /// <param name="pageVersionId">The page version id.</param>
            /// <param name="userUID">The user UID.</param>
            /// <returns></returns>
            public static DataTable GetDTByPageVersionId(int pageVersionId, Guid userUID)
            {
                DataCommand cmd = ContentDataHelper.CreateDataCommand("[dps_TemporaryStorage_GetByPageVersionId]");
                cmd.Parameters = new DataParameters();
                cmd.Parameters.Add(new DataParameter("PageVersionId", pageVersionId, DataParameterType.Int));
                cmd.Parameters.Add(new DataParameter("CreatorUID", userUID, DataParameterType.UniqueIdentifier));
                return DataService.LoadTable(cmd);
            }
            #endregion
		}
	}
}