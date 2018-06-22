using System;
using System.Data;
using Mediachase.Cms.Dto;
using Mediachase.Commerce.Storage;
using Mediachase.Data.Provider;

namespace Mediachase.Cms.Data
{
    public class TemplateAdmin
    {
        private TemplateDto _DataSet;

        /// <summary>
        /// Gets the current dto.
        /// </summary>
        /// <value>The current dto.</value>
        public TemplateDto CurrentDto
        {
            get
            {
                return _DataSet;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateAdmin"/> class.
        /// </summary>
        internal TemplateAdmin()
        {
            _DataSet = new TemplateDto();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateAdmin"/> class.
        /// </summary>
        /// <param name="dto">The dto.</param>
        internal TemplateAdmin(TemplateDto dto)
        {
            _DataSet = dto;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        internal void Save()
        {
            if (CurrentDto.main_Templates == null)
                return;

            if (CurrentDto.main_Templates.Count == 0)
                return;

            DataCommand cmd = ContentDataHelper.CreateDataCommand();

            using (TransactionScope scope = new TransactionScope())
            {
				DataHelper.SaveDataSetSimple(cmd, CurrentDto, "main_Templates");
                scope.Complete();
            }
        }

        /// <summary>
        /// Loads the specified template id.
        /// </summary>
        /// <param name="templateId">The template id.</param>
        internal void Load(int templateId)
        {
			DataCommand cmd = ContentDataHelper.CreateDataCommand("[cms_TemplatesLoadById]");
            cmd.Parameters = new DataParameters();
            cmd.Parameters.Add(new DataParameter("ApplicationId", CmsConfiguration.Instance.ApplicationId, DataParameterType.UniqueIdentifier));
            cmd.Parameters.Add(new DataParameter("TemplateId", templateId, DataParameterType.Int));

            cmd.DataSet = this.CurrentDto;
			cmd.TableMapping = DataHelper.MapTables("main_Templates");

            /*DataResult results = */DataService.LoadDataSet(cmd);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        internal void Load()
        {
            Load(0);
        }
	}


    /*
    /// <summary>
    /// Template class.
    /// </summary>
    public class Template
    {
        #region Add()
        /// <summary>
        /// Adds the specified template.
        /// </summary>
        /// <param name="TemplateFriendlyName">Template friendly name.</param>
        /// <param name="Path">The path.</param>
        /// <param name="PreviewImage">The preview image.</param>
        /// <returns></returns>
        public static int Add(string TemplateFriendlyName, string Path, byte[] PreviewImage)
        {
            int TemplateId = DBTemplate.Add(TemplateFriendlyName,Path);
            if(PreviewImage != null)
            {
                DBTemplate.UpdateImage(TemplateId, PreviewImage);
            }
            return TemplateId;
        } 
        #endregion

        #region Delete()
        /// <summary>
        /// Deletes the specified template.
        /// </summary>
        /// <param name="TemplateId">The template id.</param>
        public static void Delete(int TemplateId)
        {
            DBTemplate.Delete(TemplateId);
        }
        #endregion

        #region Update()
        /// <summary>
        /// Updates the specified template.
        /// </summary>
        /// <param name="TemplateId">The template id.</param>
        /// <param name="TemplateFriendlyName">Name of the template friendly.</param>
        /// <param name="Path">The path.</param>
        public static void Update(int TemplateId, string TemplateFriendlyName, string Path)
        {
            DBTemplate.Update(TemplateId,TemplateFriendlyName,Path);
        }
        #endregion

        #region Update()
        /// <summary>
        /// Updates the specified template.
        /// </summary>
        /// <param name="TemplateId">The template id.</param>
        /// <param name="TemplateFriendlyName">Name of the template friendly.</param>
        /// <param name="Path">The path.</param>
        /// <param name="PreviewImage">The preview image.</param>
        public static void Update(int TemplateId, string TemplateFriendlyName, string Path, byte[] PreviewImage)
        {
            DBTemplate.Update(TemplateId, TemplateFriendlyName, Path);
            DBTemplate.UpdateImage(TemplateId,PreviewImage);
        }
        #endregion

        #region LoadById()
        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <param name="TemplateId">The template id.</param>
        /// <returns>[TemplateId], [FriendlyName], [Path], [Preview]</returns>
        public static IDataReader LoadById(int TemplateId)
        {
            return DBTemplate.GetById(TemplateId);
        } 
        #endregion

        #region LoadAll()
        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <returns>
        ///IDataReader [TemplateId], [FriendlyName], [Path], [Preview]
        /// </returns>
        public static IDataReader LoadAll()
        {
            return DBTemplate.GetById(0);
        } 
        #endregion

        #region LoadAllDT()
        /// <summary>
        /// Gets the by id.
        /// </summary>
        /// <returns>
        ///DataTable [TemplateId], [FriendlyName], [Path], [Preview]
        /// </returns>
        public static DataTable LoadAllDT()
        {
            return DBTemplate.GetByIdDT(0);
        }
        #endregion
    }
    */
}