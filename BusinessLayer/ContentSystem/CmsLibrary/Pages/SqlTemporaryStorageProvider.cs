using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Mediachase.Cms.Pages
{
	[Serializable]
	public class SqlTemporaryStorageProvider: IPageDocumentStorageProvider
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlTemporaryStorageProvider"/> class.
        /// </summary>
		public SqlTemporaryStorageProvider()
		{
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

		private int expire = 30;
		/// <summary>
		/// Gets or sets the expire time (in minutes) of the document in temporary storage.
		/// </summary>
		/// <value>Expire.</value>
		public int Expire
		{
			get { return expire; }
			set { expire = value; }
		}

		#region IPageDocumentStorageProvider Members

        /// <summary>
        /// Loads PageDocument from temporary database storage
        /// </summary>
        /// <param name="PageVersionId">The page version id.</param>
        /// <param name="UserUID">The user UID.</param>
        /// <returns></returns>

		public PageDocument Load(int PageVersionId, Guid UserUID)
		{
			PageDocument pd = new PageDocument();
			int StorageId = -1;
            using (IDataReader reader = Database.TemporaryDBStorage.GetByPageVersionId(PageVersionId, UserUID))
			{
                if (reader.Read())
                {
                    StorageId = (int)reader["StorageId"];
                    reader.Close();
                }
                else
                {
                    reader.Close();
                    return null;
                }
			}

            using (IDataReader reader = Database.TemporaryDBStorage.GetById(StorageId))
			{
				if (reader.Read())
				{
					pd = (PageDocument)Helper.Deserialize((byte[])reader["PageDocument"]);
                    reader.Close();
					return pd;
				}

                reader.Close();
			}
			return null;
		}
        /// <summary>
        /// Saves PageDocument to the temporary database storage
        /// </summary>
        /// <param name="pageDocument">The page document.</param>
        /// <param name="PageVersionId">The page version id.</param>
        /// <param name="UserUID">The user UID.</param>
		public void Save(PageDocument pageDocument, int PageVersionId, Guid UserUID)
		{
			if (pageDocument == null)
				return;
			int StorageId = -1;
            using (IDataReader reader = Database.TemporaryDBStorage.GetByPageVersionId(PageVersionId, UserUID))
			{
				if (reader.Read())
				{
					StorageId = (int)reader["StorageId"];
				}

                reader.Close();
			}
			if (StorageId > 0)//Update existing document
			{
				Database.TemporaryDBStorage.Update(StorageId,  PageVersionId, Expire, Helper.Serialize(pageDocument),UserUID);
			}
			else //Add new document
			{
				Database.TemporaryDBStorage.Add(PageVersionId, Expire, Helper.Serialize(pageDocument),UserUID);
			}
		}

        /// <summary>
        /// Deletes the PageDocument with UID from temporary database storage.
        /// </summary>
        /// <param name="PageVersionId">The page version id.</param>
        /// <param name="UserUID">The user UID.</param>

        public void Delete(int PageVersionId, Guid UserUID)
		{
			int StorageId = -1;
            DataTable dt = Database.TemporaryDBStorage.GetDTByPageVersionId(PageVersionId, UserUID);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    StorageId = (int)row["StorageId"];
                    Database.TemporaryDBStorage.Delete(StorageId);
                }
            }
		}
		#endregion
	}
}
