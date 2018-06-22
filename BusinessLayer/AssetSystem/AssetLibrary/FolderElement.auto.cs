
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// An auto generated class. Don't modify it manually.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;



namespace Mediachase.Ibn.Library
{
    public partial class FolderElement: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("FolderElement");
        }
        #endregion
        
        #region .Ctor
        public FolderElement()
             : base(FolderElement.GetAssignedMetaClass())
        {
        }

        public FolderElement(MetaObjectOptions options)
             : base(FolderElement.GetAssignedMetaClass(), options)
        {
        }
        
        public FolderElement(PrimaryKeyId primaryKeyId)
             : base(FolderElement.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public FolderElement(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(FolderElement.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public FolderElement(CustomTableRow row)
             : base(FolderElement.GetAssignedMetaClass(), row)
        {
        }
        
        public FolderElement(CustomTableRow row, MetaObjectOptions options)
             : base(FolderElement.GetAssignedMetaClass(), row, options)
        {
        }

        public FolderElement(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public FolderElement(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
            : base(metaType, row, options)
        {
        }
        #endregion

        #region Extended Properties
        public MetaObjectProperty[] ExtendedProperties
        {
            get
            {
                if(_exProperies==null)
                {
                    List<MetaObjectProperty> retVal = new List<MetaObjectProperty>();
                    
                    foreach(MetaObjectProperty property in base.Properties)
                    {
                        switch(property.Name)
                        {
                            case "FolderElementId": 
                            case "Created": 
                            case "CreatorId": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Card": 
                            case "Name": 
                            case "ParentId": 
                            case "Parent": 
                            case "BlobUid": 
                            case "BlobStorageProvider": 
                            case "Description": 
                            case "ContentType": 
                            case "ContentSize": 
                            
                                 break;
                            default:
                                 retVal.Add(property);    
                                 break;
                        }
                    }
                    _exProperies = retVal.ToArray();
                }
                
                return _exProperies;
            }
        }
        #endregion
        
        #region Static Methods (List + GetTotalCount)
        public static FolderElement[] List()
        {
            return MetaObject.List<FolderElement>(FolderElement.GetAssignedMetaClass());
        }
        
        public static FolderElement[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<FolderElement>(FolderElement.GetAssignedMetaClass(),filters);
        }

        public static FolderElement[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<FolderElement>(FolderElement.GetAssignedMetaClass(),sorting);
        }

        public static FolderElement[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<FolderElement>(FolderElement.GetAssignedMetaClass(),filters, sorting);
        }

        public static FolderElement[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<FolderElement>(FolderElement.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(FolderElement.GetAssignedMetaClass(), filters);
        }
        #endregion
        
        #region Named Properties
        
        public System.DateTime Created
        {
            get
            {
                return (System.DateTime)base.Properties["Created"].Value;
            }
            
            set
            {
                base.Properties["Created"].Value = value;
            }
            
        }
        
        public System.Int32 CreatorId
        {
            get
            {
                return (System.Int32)base.Properties["CreatorId"].Value;
            }
            
            set
            {
                base.Properties["CreatorId"].Value = value;
            }
            
        }
        
        public System.DateTime Modified
        {
            get
            {
                return (System.DateTime)base.Properties["Modified"].Value;
            }
            
            set
            {
                base.Properties["Modified"].Value = value;
            }
            
        }
        
        public System.Int32 ModifierId
        {
            get
            {
                return (System.Int32)base.Properties["ModifierId"].Value;
            }
            
            set
            {
                base.Properties["ModifierId"].Value = value;
            }
            
        }
        
        public System.String Card
        {
            get
            {
                return (System.String)base.Properties["Card"].Value;
            }
            
            set
            {
                base.Properties["Card"].Value = value;
            }
            
        }
        
        public System.String Name
        {
            get
            {
                return (System.String)base.Properties["Name"].Value;
            }
            
            set
            {
                base.Properties["Name"].Value = value;
            }
            
        }
        
        public Nullable<Mediachase.Ibn.Data.PrimaryKeyId> ParentId
        {
            get
            {
                return (Nullable<Mediachase.Ibn.Data.PrimaryKeyId>)base.Properties["ParentId"].Value;
            }
            
            set
            {
                base.Properties["ParentId"].Value = value;
            }
            
        }
        
        public System.String Parent
        {
            get
            {
                return (System.String)base.Properties["Parent"].Value;
            }
            
        }
        
        public Nullable<System.Guid> BlobUid
        {
            get
            {
                return (Nullable<System.Guid>)base.Properties["BlobUid"].Value;
            }
            
            set
            {
                base.Properties["BlobUid"].Value = value;
            }
            
        }
        
        public System.String BlobStorageProvider
        {
            get
            {
                return (System.String)base.Properties["BlobStorageProvider"].Value;
            }
            
            set
            {
                base.Properties["BlobStorageProvider"].Value = value;
            }
            
        }
        
        public System.String Description
        {
            get
            {
                return (System.String)base.Properties["Description"].Value;
            }
            
            set
            {
                base.Properties["Description"].Value = value;
            }
            
        }
        
        public System.String ContentType
        {
            get
            {
                return (System.String)base.Properties["ContentType"].Value;
            }
            
            set
            {
                base.Properties["ContentType"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> ContentSize
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ContentSize"].Value;
            }
            
            set
            {
                base.Properties["ContentSize"].Value = value;
            }
            
        }
        
        #endregion
        
        
    }
}
