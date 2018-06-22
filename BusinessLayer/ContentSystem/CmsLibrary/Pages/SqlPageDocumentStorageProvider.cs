using System;
using System.Data;
using System.Collections;

namespace Mediachase.Cms.Pages
{
    /// <summary>
    /// Saves Page Document into the permanent SQL Storage.
    /// </summary>
    [Serializable]
    public class SqlPageDocumentStorageProvider : IPageDocumentStorageProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlPageDocumentStorageProvider"/> class.
        /// </summary>
        public SqlPageDocumentStorageProvider()
        {
            //
            // TODO: Add constructor logic here
            //

        }

        private string _connectionString = string.Empty;
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }


        #region IPageDocumentStorageProvider Members


        /// <summary>
        /// Loads PageDocument instance from the SQL Database
        /// </summary>
        /// <param name="PageVersionId">The page version id.</param>
        /// <param name="UserUID">The user UID.</param>
        /// <returns></returns>
        public PageDocument Load(int PageVersionId, Guid UserUID)
        {
            PageDocument pd = new PageDocument();
            int PageId = -1;
            int i, j;
            using (IDataReader reader = Database.PageDocument.GetByPageVersionId(PageVersionId))
            {
                if (reader.Read())
                {
                    PageId = (int)reader["PageId"];
                    pd.PageVersionId = (int)reader["PageVersionId"];
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    return null;
                }
            }
            //Reading page nodes
            int sNodeId = -1;
            ArrayList dNodesIDs = new ArrayList();
            dNodesIDs.Clear();
            using (IDataReader reader = Database.Node.GetByPageId(PageId))
            {
                while (reader.Read())
                {
                    if ((int)reader["NodeTypeId"] == 1) //Static node
                    {
                        sNodeId = (int)reader["NodeId"];
                    }
                    else //dynamic node
                    {
                        dNodesIDs.Add((int)reader["NodeId"]);
                    }
                }

                reader.Close();
            }
            ArrayList conIDs = null;
            Node StaticNode = null;
            DynamicNode DNode = null;
            if (sNodeId > 0)//Load controls and control settings for static node
            {
                using (IDataReader reader = Database.Node.GetById(sNodeId))
                {
                    if (reader.Read())
                    {
                        StaticNode = new Node();
                        StaticNode.NodeUID = reader["NodeUID"].ToString();
                        StaticNode.Controls = new NodeControlSettingsCollection();
                    }

                    reader.Close();
                }
                if (StaticNode != null)
                {
                    using (IDataReader reader = Database.Control.GetByNodeId(sNodeId))
                    {
                        conIDs = new ArrayList();
                        while (reader.Read())
                        {
                            conIDs.Add((int)reader["ControlId"]);
                        }

                        reader.Close();
                    }
                }
                if (conIDs != null && conIDs.Count > 0)
                {
                    for (i = 0; i < conIDs.Count; i++)
                    {
                        Param par = new Param();
                        ControlSettings cs = new ControlSettings();
                        using (IDataReader reader = Database.ControlSettings.GetByControlId((int)conIDs[i]))
                        {
                            while (reader.Read())
                            {
                                par.Add(reader["Key"].ToString(),
                                                    reader["Value"] != DBNull.Value ? Helper.Deserialize((byte[])reader["Value"]) : null);

                                cs.Params = par;
                            }

                            reader.Close();
                        }
                        string ControlUID = string.Empty;
                        using (IDataReader reader = Database.Control.GetById((int)conIDs[i]))
                        {
                            if (reader.Read())
                            {
                                ControlUID = reader["ControlUID"].ToString();
                            }

                            reader.Close();
                        }
                        if (StaticNode != null)
                        {
                            StaticNode.Controls.Add(ControlUID, cs);
                        }
                    }
                }
                pd.StaticNode = StaticNode;
                pd.CurrentNode = StaticNode;
            }//if (sNodeId > 0)
            //Dynamic nodes
            if (pd.DynamicNodes == null)
                pd.DynamicNodes = new DynamicNodeCollection(pd);
            if (dNodesIDs != null && dNodesIDs.Count > 0)
            {
                for (j = 0; j < dNodesIDs.Count; j++)
                {
                    DNode = null;
                    conIDs = null;
                    using (IDataReader reader = Database.Node.GetById((int)dNodesIDs[j]))
                    {
                        if (reader.Read())
                        {
                            DNode = new DynamicNode();
                            DNode.NodeUID = reader["NodeUID"].ToString();
                            DNode.FactoryUID = reader["FactoryUID"].ToString();
                            DNode.FactoryControlUID = reader["FactoryControlUID"].ToString();
                            DNode.ControlPlaceId = reader["ControlPlaceId"].ToString();
                            DNode.ControlPlaceIndex = Convert.ToInt32(reader["ControlPlaceIndex"].ToString());
                            DNode.Controls = new NodeControlSettingsCollection();
                        }

                        reader.Close();
                    }
                    if (DNode != null)
                    {
                        conIDs = new ArrayList();
                        using (IDataReader reader = Database.Control.GetByNodeId((int)dNodesIDs[j]))
                        {
                            while (reader.Read())
                            {
                                conIDs.Add((int)reader["ControlId"]);
                            }

                            reader.Close();
                        }
                    }
                    if (conIDs != null && conIDs.Count > 0)
                    {
                        for (i = 0; i < conIDs.Count; i++)
                        {
                            Param par = new Param();
                            ControlSettings cs = new ControlSettings();
                            using (IDataReader reader = Database.ControlSettings.GetByControlId((int)conIDs[i]))
                            {
                                while (reader.Read())
                                {
                                    par.Add(reader["Key"].ToString(),
                                                        reader["Value"] != DBNull.Value ? Helper.Deserialize((byte[])reader["Value"]) : null);

                                    cs.Params = par;
                                }

                                reader.Close();
                            }
                            string ControlUID = string.Empty;
                            using (IDataReader reader = Database.Control.GetById((int)conIDs[i]))
                            {
                                if (reader.Read())
                                {
                                    ControlUID = reader["ControlUID"].ToString();
                                }

                                reader.Close();
                            }
                            if (DNode != null)
                            {
                                DNode.Controls.Add(ControlUID, cs);
                            }
                        }
                    }
                    if (DNode != null)
                        pd.DynamicNodes.Add(DNode);
                }//for (j = 0; j < dNodesIDs.Count; j++)
            }//if (dNodesIDs != null && dNodesIDs.Count > 0)
            return pd;
        }

        /// <summary>
        /// Saves the specified page document to the SQL Database.
        /// </summary>
        /// <param name="pageDocument">The page document.</param>
        /// <param name="pageVersionId">The page version id.</param>
        /// <param name="userUID">The user UID.</param>
        public void Save(PageDocument pageDocument, int pageVersionId, Guid userUID)
        {
            if (pageDocument == null)
                throw new Exception("Page document is null!");

            bool existingDoc = false;
            int pageId = -1;
            int nodeId = -1;
            int controlId = -1;

            Hashtable controls = new Hashtable();
            

            //CHECK EXIST
            using (IDataReader reader = Database.PageDocument.GetByPageVersionId(pageVersionId))
            {
                if (reader.Read())
                {
                    existingDoc = true;
                    pageId = (int)reader["PageId"];
                }

                reader.Close();
            }

            //Page document already exists
            if (existingDoc)
            {
                //get static node
                Node sNode = pageDocument.StaticNode;

                if (sNode != null)
                {
                    //get static node id
                    using (IDataReader reader = Database.Node.GetByUID(sNode.NodeUID, pageId))
                    {
                        if (reader.Read())
                        {
                            nodeId = (int)reader["NodeId"];
                        }

                        reader.Close();
                    }

					//DV: 17-09-2007
					if (nodeId == -1)
					{
                        nodeId = Database.Node.Add(pageId, 1, sNode.NodeUID, string.Empty, string.Empty, string.Empty, 0);
					}

                    //Update controls settings
                    if (sNode.Controls.IsModified)
                    {
                        string[] sNodeContKeys = sNode.Controls.AllKeys;

                        //Check if some controls have been deleted
                        using (IDataReader reader = Database.Control.GetByNodeId(nodeId))
                        {
                            while (reader.Read())
                            {
                                int contId = (int)reader["ControlId"];
                                string contUID = reader["ControlUID"].ToString();
                                bool contains = false;
                                for (int i = 0; i < sNodeContKeys.Length; i++)
                                {
                                    if (contUID == sNodeContKeys[i])
                                        contains = true;
                                }
                                if (!contains)
                                    Database.Control.Delete(contId);
                                else
                                    controls.Add(contUID, contId);
                            }

                            reader.Close();
                        }

                        //add new controls
                        foreach (string key in sNodeContKeys)
                        {
                            if(!controls.Contains(key))
                            {
                                //add control
                                int newControlId = Database.Control.Add(nodeId, key);

                                //add params
                                if (sNode.Controls[key].Params != null)
                                {
                                    string[] sNodeParamKeys = sNode.Controls[key].Params.AllKeys;
                                    for (int j = 0; j < sNodeParamKeys.Length; j++)
                                    {
                                        Database.ControlStorage.Add(newControlId, sNodeParamKeys[j], Helper.Serialize(sNode.Controls[key].Params[sNodeParamKeys[j]]));
                                    }
                                }

                                //add to hash
                                controls.Add(key, newControlId);
                            }
                        }

                        //GA 23.05.2006
                        if (controls.Count > 0)
                        {
                            for (int l = 0; l < controls.Count; l++)
                            {
                                controlId = (int)controls[sNodeContKeys[l]];
                                if (controlId > 0 && sNode.Controls[sNodeContKeys[l]].IsModified)//Update control in static node
                                {
                                    Param param = sNode.Controls[sNodeContKeys[l]].Params;
                                    string[] sNodeParamKeys = param.AllKeys;
                                    using (IDataReader reader = Database.ControlSettings.GetByControlId(controlId))
                                    {
                                        while (reader.Read())
                                        {
                                            int contStorId = -1;
                                            if (reader["ControlStorageId"] != DBNull.Value)
                                                contStorId = (int)reader["ControlStorageId"];
                                            
                                            string key = reader["Key"].ToString();
                                            bool contains = false;
                                            for (int k = 0; k < sNodeParamKeys.Length; k++)
                                            {
                                                if (key == sNodeParamKeys[k])
                                                    contains = true;
                                            }
                                            if (!contains)
                                            {
                                                if (contStorId > 0)
                                                    Database.ControlStorage.Delete(contStorId);
                                            }
                                        }

                                        reader.Close();
                                    }
                                    for (int k = 0; k < sNodeParamKeys.Length; k++)
                                    {
                                        using (IDataReader reader = Database.ControlSettings.GetByKeyAndControlId(sNodeParamKeys[k], controlId))
                                        {
                                            if (reader.Read())
                                            {
                                                if (reader["ControlStorageId"] != DBNull.Value)
                                                {
                                                    Database.ControlStorage.Update((int)reader["ControlStorageId"],
                                                        controlId, sNodeParamKeys[k], Helper.Serialize(param[sNodeParamKeys[k]]));
                                                }
                                                else
                                                {
													Database.ControlStorage.Add(controlId, sNodeParamKeys[k], Helper.Serialize(param[sNodeParamKeys[k]]));
                                                }
                                            }
                                            else
                                            {
												Database.ControlStorage.Add(controlId, sNodeParamKeys[k], Helper.Serialize(param[sNodeParamKeys[k]]));
                                            }

                                            reader.Close();
                                        }
                                    }
                                }
                                if (controlId < 0)//Add new control to static node
                                {
                                    controlId = Database.Control.Add(nodeId, sNodeContKeys[l]);
                                    //Add control settings
                                    Param param = sNode.Controls[sNodeContKeys[l]].Params;
                                    string[] parKeys = param.AllKeys;
                                    for (int j = 0; j < parKeys.Length; j++)
                                    {
                                        Database.ControlStorage.Add(controlId, parKeys[j], Helper.Serialize(param[parKeys[j]]));
                                    }
                                }
                                sNode.Controls[l].IsModified = false;
                            }
                        }
                    }
                }
                //Check if some dynamic nodes have been deleted
                using (IDataReader reader = Database.Node.GetByPageId(pageId))
                {
                    while (reader.Read())
                    {
                        if ((int)reader["NodeTypeId"] == 1)//Static node
                            continue;
                        nodeId = (int)reader["NodeId"];
                        string nodeUID = reader["NodeUID"].ToString();
                        bool contains = false;
                        foreach (DynamicNode dn in pageDocument.DynamicNodes)
                        {
                            if (dn.NodeUID == nodeUID)
                                contains = true;
                        }
                        if (!contains)
                            Database.Node.Delete(nodeId);
                    }

                    reader.Close();
                }
                //Update dynamic nodes
                foreach (DynamicNode dn in pageDocument.DynamicNodes)
                {
                    if (dn.IsModified)
                    {
                        using (IDataReader reader = Database.Node.GetByUID(dn.NodeUID, pageId))
                        {
                            if (reader.Read())
                            {
                                nodeId = (int)reader["NodeId"];
                                Database.Node.Update((int)reader["NodeId"], (int)reader["PageId"],
                                    2, dn.NodeUID, dn.FactoryUID, dn.FactoryControlUID, dn.ControlPlaceId,
                                    dn.ControlPlaceIndex);
                            }
                            else
                            {
                                nodeId = Database.Node.Add(pageId, 2, dn.NodeUID,
                                    dn.FactoryUID, dn.FactoryControlUID, dn.ControlPlaceId,
                                    dn.ControlPlaceIndex);
                            }

                            reader.Close();
                        }//using (IDataReader reader = Database.Node.GetByUID(dn.NodeUID))
                        string[] contKeys = dn.Controls.AllKeys;

                        //Check if some controls have been deleted from dynamic node
                        using (IDataReader reader = Database.Control.GetByNodeId(nodeId))
                        {
                            while (reader.Read())
                            {
                                int contId = (int)reader["ControlId"];
                                string contUID = reader["ControlUID"].ToString();
                                bool contains = false;
                                for (int i = 0; i < contKeys.Length; i++)
                                {
                                    if (contUID == contKeys[i])
                                        contains = true;
                                }
                                if (!contains)
                                    Database.Control.Delete(contId);
                            }

                            reader.Close();
                        }
                        for (int i = 0; i < contKeys.Length; i++)
                        {
                            controlId = -1;
                            using (IDataReader reader = Database.Control.GetByUID(contKeys[i]))
                            {
                                while (reader.Read())
                                {
                                    //make sure that the control id is associated with the correct node
                                    //since there may be multiple instances of the control in the application
                                    if ((int)reader["NodeId"] == nodeId)
                                    {
                                        controlId = (int)reader["ControlId"];
                                        break;
                                    }
                                }

                                reader.Close();
                            }
                            if (controlId > 0 && dn.Controls[contKeys[i]].IsModified)//Update control
                            {
                                Database.Control.Update(controlId, nodeId, contKeys[i]);
                                //Update control settings
                                Param param = (dn.Controls[contKeys[i]]).Params;
                                string[] parKeys = param.AllKeys;
                                //Check if some settings have been deleted from control
                                using (IDataReader reader = Database.ControlSettings.GetByControlId(controlId))
                                {
                                    while (reader.Read())
                                    {
                                        int contStorId = -1;
                                        if (reader["ControlStorageId"] != DBNull.Value)
                                            contStorId = (int)reader["ControlStorageId"];
                                        
                                        string key = reader["Key"].ToString();
                                        bool contains = false;
                                        for (int k = 0; k < parKeys.Length; k++)
                                        {
                                            if (key == parKeys[k])
                                                contains = true;
                                        }
                                        if (!contains)
                                        {
                                            if (contStorId > 0)
                                                Database.ControlStorage.Delete(contStorId);
                                        }
                                    }

                                    reader.Close();
                                }
                                for (int k = 0; k < parKeys.Length; k++)
                                {
                                    using (IDataReader reader = Database.ControlSettings.GetByKeyAndControlId(parKeys[k], controlId))
                                    {
                                        if (reader.Read())//Update control settings
                                        {
                                            if (reader["ControlStorageId"] != DBNull.Value)//Update settings in control storage
                                            {
                                                Database.ControlStorage.Update((int)reader["ControlStorageId"],
                                                    controlId, parKeys[k], Helper.Serialize(param[parKeys[k]]));
                                            }
                                            else //Add settings to ControlStorage
                                            {
                                                Database.ControlStorage.Add(controlId, parKeys[k], Helper.Serialize(param[parKeys[k]]));
                                            }
                                        }
                                        else //Add new control settings
                                        {
                                            Database.ControlStorage.Add(controlId, parKeys[k], Helper.Serialize(param[parKeys[k]]));
                                        }

                                        reader.Close();
                                    }
                                }
                            }
                            if (controlId < 0)//Add new control
                            {
                                controlId = Database.Control.Add(nodeId, contKeys[i]);
                                //Add control settings
                                //foreach (Param param in dn.Controls[contKeys[i]].Params)
                                //{
                                Param param = dn.Controls[contKeys[i]].Params;
                                string[] parKeys = param.AllKeys;
                                for (int j = 0; j < parKeys.Length; j++)
                                {
                                    Database.ControlStorage.Add(controlId, parKeys[j], Helper.Serialize(param[parKeys[j]]));
                                }
                                //}
                            }

                        }//for (int i = 0; i < contKeys.Length; i++)
                    }//if(dn.IsModified)
                }//foreach (DynamicNode dn in pageDocument.DynamicNodes)

            }//if (ExistingDoc)
            else //New document 
            {
                //Add new document
                pageId = Database.PageDocument.Add(pageVersionId);
                //Add static node
                nodeId = Database.Node.Add(pageId, 1, pageDocument.StaticNode.NodeUID, "", "", "0", 0);
                //Add static node controls
                string[] sNodeContKeys = pageDocument.StaticNode.Controls.AllKeys;
                for (int i = 0; i < sNodeContKeys.Length; i++)
                {
                    controlId = Database.Control.Add(nodeId, sNodeContKeys[i]);
                    if (pageDocument.StaticNode.Controls[sNodeContKeys[i]].Params != null)
                    {
                        string[] sNodeParamKeys = pageDocument.StaticNode.Controls[sNodeContKeys[i]].Params.AllKeys;
                        for (int j = 0; j < sNodeParamKeys.Length; j++)
                        {
                            Database.ControlStorage.Add(controlId, sNodeParamKeys[j], Helper.Serialize(pageDocument.StaticNode.Controls[sNodeContKeys[i]].Params[sNodeParamKeys[j]]));
                        }
                    }
                }//for (int i = 0; i < sNodeContKeys.Length; i++)

                //Add dynamic node controls
                foreach (DynamicNode dn in pageDocument.DynamicNodes)
                {
                    nodeId = Database.Node.Add(pageId, 2, dn.NodeUID, dn.FactoryUID,
                        dn.FactoryControlUID, dn.ControlPlaceId, dn.ControlPlaceIndex);
                    string[] dNodeContKeys = dn.Controls.AllKeys;
                    for (int i = 0; i < dNodeContKeys.Length; i++)
                    {
                        controlId = Database.Control.Add(nodeId, dNodeContKeys[i]);
                        Param param = dn.Controls[dNodeContKeys[i]].Params;
                        if (param == null) 
							continue;
                        string[] dNodeParamKeys = param.AllKeys;
                        for (int j = 0; j < dNodeParamKeys.Length; j++)
                        {
                            Database.ControlStorage.Add(controlId, dNodeParamKeys[j], Helper.Serialize(param[dNodeParamKeys[j]]));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes PageDocument with the specified PageVersionId from persistent SQL storage.
        /// </summary>
        /// <param name="pageVersionId">PageDocument.PageVersionId</param>
        /// <param name="userUID">The user UID.</param>
        public void Delete(int pageVersionId, Guid userUID)
        {
            int pageId = -1;
            using (IDataReader reader = Database.PageDocument.GetByPageVersionId(pageVersionId))
            {
                while (reader.Read())
                {
                    pageId = (int)reader["PageId"];
                    Database.PageDocument.Delete(pageId);
                }

                reader.Close();
            }
        }

        #endregion
    }
}