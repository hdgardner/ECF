
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
    public partial class Folder: BusinessObject
    {
        private MetaObjectProperty[] _exProperies = null;
        
        #region Util
        public static MetaClass GetAssignedMetaClass()
        {
             return DataContext.Current.GetMetaClass("Folder");
        }
        #endregion
        
        #region .Ctor
        public Folder()
             : base(Folder.GetAssignedMetaClass())
        {
        }

        public Folder(MetaObjectOptions options)
             : base(Folder.GetAssignedMetaClass(), options)
        {
        }
        
        public Folder(PrimaryKeyId primaryKeyId)
             : base(Folder.GetAssignedMetaClass(), primaryKeyId)
        {
        }
        
        public Folder(PrimaryKeyId primaryKeyId, MetaObjectOptions options)
             : base(Folder.GetAssignedMetaClass(), primaryKeyId, options)
        {
        }

        public Folder(CustomTableRow row)
             : base(Folder.GetAssignedMetaClass(), row)
        {
        }
        
        public Folder(CustomTableRow row, MetaObjectOptions options)
             : base(Folder.GetAssignedMetaClass(), row, options)
        {
        }

        public Folder(MetaClass metaType, PrimaryKeyId primaryKeyId, MetaObjectOptions options)
            : base(metaType, primaryKeyId, options)
        {
        }

        public Folder(MetaClass metaType, CustomTableRow row, MetaObjectOptions options)
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
                            case "FolderId": 
                            case "Created": 
                            case "CreatorId": 
                            case "Modified": 
                            case "ModifierId": 
                            case "Name": 
                            case "OutlineLevel": 
                            case "OutlineNumber": 
                            case "HasChildren": 
                            case "ParentId": 
                            case "ScopeIndex": 
                            case "Description": 
                            
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
        public static Folder[] List()
        {
            return MetaObject.List<Folder>(Folder.GetAssignedMetaClass());
        }
        
        public static Folder[] List(params Mediachase.Ibn.Data.FilterElement[] filters)
        {
            return MetaObject.List<Folder>(Folder.GetAssignedMetaClass(),filters);
        }

        public static Folder[] List(params Mediachase.Ibn.Data.SortingElement[] sorting)
        {
            return MetaObject.List<Folder>(Folder.GetAssignedMetaClass(),sorting);
        }

        public static Folder[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting)
        {
            return MetaObject.List<Folder>(Folder.GetAssignedMetaClass(),filters, sorting);
        }

        public static Folder[] List(Mediachase.Ibn.Data.FilterElementCollection filters, Mediachase.Ibn.Data.SortingElementCollection sorting, int start, int count)
        {
            return MetaObject.List<Folder>(Folder.GetAssignedMetaClass(), filters, sorting, start, count);
        }

        public static int GetTotalCount(params FilterElement[] filters)
        {
            return MetaObject.GetTotalCount(Folder.GetAssignedMetaClass(), filters);
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
        
        public System.Int32 OutlineLevel
        {
            get
            {
                return (System.Int32)base.Properties["OutlineLevel"].Value;
            }
            
            set
            {
                base.Properties["OutlineLevel"].Value = value;
            }
            
        }
        
        public System.String OutlineNumber
        {
            get
            {
                return (System.String)base.Properties["OutlineNumber"].Value;
            }
            
            set
            {
                base.Properties["OutlineNumber"].Value = value;
            }
            
        }
        
        public System.Boolean HasChildren
        {
            get
            {
                return (System.Boolean)base.Properties["HasChildren"].Value;
            }
            
            set
            {
                base.Properties["HasChildren"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> ParentId
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ParentId"].Value;
            }
            
            set
            {
                base.Properties["ParentId"].Value = value;
            }
            
        }
        
        public Nullable<System.Int32> ScopeIndex
        {
            get
            {
                return (Nullable<System.Int32>)base.Properties["ScopeIndex"].Value;
            }
            
            set
            {
                base.Properties["ScopeIndex"].Value = value;
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
        
        #endregion
        
        
    }
}
