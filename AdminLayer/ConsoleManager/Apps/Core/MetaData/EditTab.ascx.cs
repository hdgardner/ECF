using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Data.Provider;
using Mediachase.MetaDataPlus;
using Mediachase.MetaDataPlus.Configurator;
using Mediachase.Web.Console.BaseClasses;
using Mediachase.Web.Console.Interfaces;
using Mediachase.Web.Console.Common;
using Mediachase.Commerce.Storage;
using System.IO;

namespace Mediachase.Commerce.Manager.Core.MetaData
{
    public partial class EditTab : CoreBaseUserControl, IAdminTabControl, IAdminContextControl
    {
		public readonly string _MetaObjectContextKey = "MetaObjectContext-";
		public readonly string _OrderMetaObjectContextKey = "OrderMetaObjectContext-";

        #region Properties
        private int _ObjectId = 0;
        /// <summary>
        /// Gets or sets the object id.
        /// </summary>
        /// <value>The object id.</value>
        public int ObjectId
        {
            get
            {
                return _ObjectId;
            }
            set
            {
                _ObjectId = value;
            }
        }

        private int _MetaClassId = 0;
        /// <summary>
        /// Gets or sets the meta class id.
        /// </summary>
        /// <value>The meta class id.</value>
        public int MetaClassId
        {
            get
            {
                return _MetaClassId;
            }
            set
            {
                _MetaClassId = value;
            }
        }

        private string[] _Languages = new string[] { "en-US" };
        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>The languages.</value>
        public string[] Languages
        {
            get
            {
                return _Languages;
            }
            set
            {
                _Languages = value;
            }
        }

        string _ValidationGroup = String.Empty;
        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        public string ValidationGroup
        {
            get
            {
                return _ValidationGroup;
            }
            set
            {
                _ValidationGroup = value;
            }
		}
		#endregion

		/// <summary>
		/// Raises the <see cref="E:Init"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		/// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //CreateChildControlsInternal();           
            //MetaControls.DataBind();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            // Create controls structure
            //CreateChildControlsInternal();
            base.OnPreRender(e);
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {            
            if (!this.ChildControlsCreated)
            {                
                base.CreateChildControls();
                ChildControlsCreated = true;
            }
        }

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        public override void DataBind()
        {            
            base.DataBind();
            CreateChildControlsInternal();
        }

        /// <summary>
        /// Populates the meta controls.
        /// </summary>
        public void PopulateMetaControls()
        {
            //CreateChildControlsInternal();
        }

        /// <summary>
        /// Creates the child controls internal.
        /// </summary>
        private void CreateChildControlsInternal()
        {
            //if (this.ObjectId > 0)
            {
                MetaControls.EnableViewState = false;
                if (MetaControls.Controls.Count > 0)
                    return;

                MetaControls.Controls.Clear();

                MetaClass mc = MetaClass.Load(this.MDContext, MetaClassId);

                if (mc == null)
                    return;

                Dictionary<string, MetaObject> metaObjects = new Dictionary<string,MetaObject>();

                if (_metaObjects != null)
                    metaObjects = _metaObjects;
                else
                {
                    // cycle through each language and get meta objects
                    MDContext.UseCurrentUICulture = false;
                    foreach (string language in Languages)
                    {
                        MDContext.UseCurrentUICulture = false;
                        MDContext.Language = language;

                        MetaObject metaObj = null;
                        if (ObjectId > 0)
                        {
                            metaObj = MetaObject.Load(MDContext, ObjectId, mc);
                            if (metaObj == null)
                            {
                                metaObj = MetaObject.NewObject(MDContext, ObjectId, MetaClassId, FrameworkContext.Current.Profile.UserName);
                                metaObj.AcceptChanges(MDContext);
                            }
                        }

						metaObjects[language] = metaObj;
                    }
                    MDContext.UseCurrentUICulture = true;
                }

                MetaFieldCollection metaFieldsColl = MetaField.GetList(MDContext, MetaClassId);
                foreach (MetaField mf in metaFieldsColl)
                {
                    if (mf.IsUser)
                    {
                        int index = 0;
                        foreach (string language in Languages)
                        {
                            string controlName = ResolveMetaControl(mc, mf);
                            Control ctrl = MetaControls.FindControl(mf.Name);

                            if (ctrl == null)
                            {
                                ctrl = Page.LoadControl(controlName);
                                MetaControls.Controls.Add(ctrl);
                            }


							CoreBaseUserControl coreCtrl = ctrl as CoreBaseUserControl;
							if (coreCtrl != null)
								coreCtrl.MDContext = this.MDContext;

                            //ctrl.ID = String.Format("{0}-{1}", mf.Name, index.ToString());

                            ((IMetaControl)ctrl).MetaField = mf;
                            if(metaObjects[language] != null && metaObjects[language][mf]!=null)
                                ((IMetaControl)ctrl).MetaObject = metaObjects[language];

                            ((IMetaControl)ctrl).LanguageCode = language;
                            ((IMetaControl)ctrl).ValidationGroup = ValidationGroup;

                            ctrl.DataBind();

                            if (!mf.MultiLanguageValue)
                                break;

                            index++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resolves the meta control.
        /// </summary>
        /// <param name="metaClass">The meta class.</param>
        /// <param name="metaField">The meta field.</param>
        /// <returns></returns>
        private string ResolveMetaControl(MetaClass metaClass, MetaField metaField)
        {
            string basePath = "~/Apps/Core/MetaData/Controls/";
            string controlName = GetControlNameForMetaType(metaField.DataType);

            string fullPath = String.Format("{0}{1}.{2}.{3}.ascx", basePath, metaClass.Name, metaField.Name, controlName);

            if (File.Exists(Server.MapPath(fullPath)))
                return fullPath;

            fullPath = String.Format("{0}{1}.{2}.ascx", basePath, metaField.Name, controlName);

            if (File.Exists(Server.MapPath(fullPath)))
                return fullPath;

            
            fullPath = String.Format("{0}{1}.{2}.ascx", basePath, metaClass.Name, controlName);

            if (File.Exists(Server.MapPath(fullPath)))
                return fullPath;

            return String.Format("{0}{1}.ascx", basePath, controlName);
        }

        /// <summary>
        /// Returns path to the control for specific meta data type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public string GetControlNameForMetaType(MetaDataType type)
        {
            switch (type)
            {
                case MetaDataType.File:
                    return "FileControl";
                case MetaDataType.ImageFile:
                    return "ImageFileControl";
                case MetaDataType.DateTime:
                    return "DateTimeMetaControl";
                case MetaDataType.Money:
                    return "MoneyControl";
                case MetaDataType.Float:
                    return "FloatControl";
                case MetaDataType.Decimal:
                    return "DecimalControl";
                case MetaDataType.Int:
                case MetaDataType.Integer:
                    return "IntegerControl";
                case MetaDataType.Boolean:
                    return "BooleanControl";
                case MetaDataType.Date:
                    return "DateTimeMetaControl";
                case MetaDataType.Email:
                    return "EmailControl";
                case MetaDataType.URL:
                    return "URLControl";
                case MetaDataType.ShortString:
                    return "ShortStringControl";
                case MetaDataType.LongString:
                    return "LongStringControl";
                case MetaDataType.LongHtmlString:
                    return "LongHTMLStringControl";
                case MetaDataType.DictionarySingleValue:
                case MetaDataType.EnumSingleValue:
                    return "DicSingleValueControl";
                case MetaDataType.DictionaryMultiValue:
                case MetaDataType.EnumMultiValue:
                    return "MultiValueControl";
                case MetaDataType.StringDictionary:
                    return "StringDictionaryControl";
                default:
                    return "";
            }
        }

        #region IAdminTabControl Members
        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <param name="context">The context.</param>
        public void SaveChanges(IDictionary context)
        {
            if (ObjectId != 0)
			{
				// set username here, because calling FrameworkContext.Current.Profile causes MeteDataContext.Current to change (it's bug in ProfileContext class).
				string userName = FrameworkContext.Current.Profile.UserName;

				MDContext.UseCurrentUICulture = false;

				// make sure all the values are saved using transaction
				//using (TransactionScope scope = new TransactionScope())
				{
					MetaObjectSerialized serialized = new MetaObjectSerialized();
					//bool saveSerialized = false;
					//string 
					foreach (string language in Languages)
					{
						MDContext.Language = language;

						MetaObject metaObj = null;
						bool saveChanges = true;

						// Check if meta object contect exists
						if (context != null && context.Contains(_MetaObjectContextKey + language))
						{
							saveChanges = false;
							metaObj = (MetaObject)context[_MetaObjectContextKey + language];
						}
						//if (context != null)
						//{
						//    if (context.Contains(_MetaObjectContextKey + language))
						//    {
						//        saveChanges = false;
						//        saveSerialized = true;
						//        metaObj = (MetaObject)context[_MetaObjectContextKey + language];
						//    }
						//    else if (context.Contains(_OrderMetaObjectContextKey + language))
						//    {
						//        metaObj = (MetaObject)context[_OrderMetaObjectContextKey + language];
						//    }
						//}

						if (metaObj == null)
							metaObj = MetaObject.Load(MDContext, ObjectId, MetaClassId);

						if (metaObj == null)
						{
							metaObj = MetaObject.NewObject(MDContext, ObjectId, MetaClassId, userName);
							//DataBind(); return;
						}
						else
						{
							metaObj.ModifierId = userName;
							metaObj.Modified = DateTime.UtcNow;
						}

						foreach (Control ctrl in MetaControls.Controls)
						{
							// Only update controls that belong to current language
							if (String.Compare(((IMetaControl)ctrl).LanguageCode, language, true) == 0)
							{
								((IMetaControl)ctrl).MetaObject = metaObj;
								//((IMetaControl)ctrl).MetaField = metaObj;
								((IMetaControl)ctrl).Update();
							}
						}

						// Only save changes when new object has been created
						if (saveChanges)
							metaObj.AcceptChanges(MDContext);
						//else
						if (context != null)
							serialized.AddMetaObject(language, metaObj);
					}

					if (context != null) //&& saveSerialized)
						context.Add("MetaObjectSerialized", serialized);
					//scope.Complete();
				}

				MDContext.UseCurrentUICulture = true;
			}

            //DataBind();
        }
        #endregion

        #region IAdminContextControl Members

        Dictionary<string, MetaObject> _metaObjects = null;
        /// <summary>
        /// Loads the context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void LoadContext(IDictionary context)
        {
            if (context.Contains("MetaObjectsContext"))
                _metaObjects = (Dictionary<string, MetaObject>)context["MetaObjectsContext"];
        }

        #endregion
    }
}