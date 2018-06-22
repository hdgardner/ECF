using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Ibn.Blob;
using System.IO;
using System.Diagnostics;
using LitS3;
using System.Collections.Specialized;

namespace Mediachase.Library.AmazonProviders
{
    public class S3StorageProvider : BlobStorageProvider
    {
        private S3Service _Service = null;
        private bool _useSSL = true;

        /// <summary>
        /// Sets a value indicating whether [use SSL].
        /// </summary>
        /// <value><c>true</c> if [use SSL]; otherwise, <c>false</c>.</value>
        public bool UseSSL
        {
            set { _useSSL = value; }
        }

        private bool _useSubDomains = true;

        /// <summary>
        /// Sets a value indicating whether [use sub domains].
        /// </summary>
        /// <value><c>true</c> if [use sub domains]; otherwise, <c>false</c>.</value>
        public bool UseSubDomains
        {
            set { _useSubDomains = value; }
        }

        private string _AccessKeyID = String.Empty;

        /// <summary>
        /// Gets or sets the access key ID.
        /// </summary>
        /// <value>The access key ID.</value>
        public string AccessKeyID
        {
            set { _AccessKeyID = value; }
        }

        private string _Host = String.Empty;

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host
        {
            set { _Host = value; }
            get { return _Host; }
        }

        private TimeSpan _ExpirationTimeSpan = new TimeSpan(1, 0, 0); // 1 hour by default

        /// <summary>
        /// Gets or sets the expiration time span.
        /// </summary>
        /// <value>The expiration time span.</value>
        public TimeSpan ExpirationTimeSpan
        {
            get { return _ExpirationTimeSpan; }
            set { _ExpirationTimeSpan = value; }
        }

        private string _SecretAccessKey = String.Empty;

        /// <summary>
        /// Gets or sets the secret access key.
        /// </summary>
        /// <value>The secret access key.</value>
        public string SecretAccessKey
        {
            set { _SecretAccessKey = value; }
        }

        private string _BucketName = String.Empty;

        /// <summary>
        /// Gets or sets the name of the bucket.
        /// </summary>
        /// <value>The name of the bucket.</value>
        public string BucketName
        {
            get { return _BucketName; }
            set { _BucketName = value; }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>The service.</value>
        public S3Service Service
        {
            get { return _Service; }
        }

        /// <summary>
        /// Creates the stream.
        /// </summary>
        /// <param name="blobInfo">The BLOB info.</param>
        /// <returns></returns>
        public override Stream CreateStream(BlobInfo blobInfo)
        {
            if (this.GetInfo(blobInfo.Uid) != null)
            {
                throw new Exception("Stream with such Guid already exist.");
            }
            Stream stream = null;
            try
            {
                BlobStorage.RaiseCreatingEvent(blobInfo);
                AddObjectRequest request = new AddObjectRequest(_Service, this.BucketName, blobInfo.Uid.ToString());
                request.ContentLength = blobInfo.ContentSize;
                request.ContentType = blobInfo.ContentType;
                request.ContentDisposition = String.Format("filename={0}", blobInfo.FileName);
                request.Metadata["FileName"] = blobInfo.FileName;
                stream = request.GetRequestStream();
                BlobStorage.RaiseCreatedEvent(blobInfo);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                throw;
            }
            return stream;
        }

        /// <summary>
        /// Gets the info.
        /// </summary>
        /// <param name="uidStream">The uid stream.</param>
        /// <returns></returns>
        public override BlobInfo GetInfo(Guid uidStream)
        {
            BlobInfo info;
            try
            {
                GetObjectRequest request = new GetObjectRequest(_Service, this.BucketName, uidStream.ToString(), true);
                using (GetObjectResponse response = request.GetResponse())
                {
                    info = this.CreateBlobInfo(response, uidStream);
                }
            }
            catch (S3Exception exception)
            {
                if (exception.ErrorCode == S3ErrorCode.NoSuchKey)
                    return null;

                throw;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                throw;
            }
            return info;
        }

        /// <summary>
        /// Reads the stream.
        /// </summary>
        /// <param name="blobInfo">The BLOB info.</param>
        /// <returns></returns>
        public override System.IO.Stream ReadStream(BlobInfo blobInfo)
        {
            Stream stream = null;
            try
            {
                BlobStorage.RaiseReadingEvent(blobInfo);
                GetObjectRequest request = new GetObjectRequest(_Service, this.BucketName, blobInfo.Uid.ToString(), false);
                GetObjectResponse response = request.GetResponse();
                stream = response.GetResponseStream();
                BlobStorage.RaiseReadedEvent(blobInfo);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                throw;
            }
            return stream;
        }

        /// <summary>
        /// Releases the stream.
        /// </summary>
        /// <param name="uidStream">The uid stream.</param>
        public override void ReleaseStream(Guid uidStream)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest(_Service, this.BucketName, uidStream.ToString());
                using (DeleteObjectResponse response = request.GetResponse())
                {
                }
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                throw;
            }
        }

        /// <summary>
        /// Searches the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        public override BlobInfo[] Search(string keyword)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Streams the commited.
        /// </summary>
        /// <param name="newBlobInfo">The new BLOB info.</param>
        /// <param name="oldBlobInfo">The old BLOB info.</param>
        public override void StreamCommited(BlobInfo newBlobInfo, BlobInfo oldBlobInfo)
        {
            if (newBlobInfo == null)
            {
                throw new NullReferenceException("Null exception");
            }
            if (newBlobInfo.ContentType == null)
            {
                throw new NullReferenceException("ContentType must be no null ");
            }
            if (newBlobInfo.FileName == null)
            {
                throw new NullReferenceException("FileName must be no null ");
            }
            try
            {
                BlobStorage.RaiseCommitingEvent(newBlobInfo, oldBlobInfo);
                /*

                // copy an object to itself with an updated meta data
                CopyObjectRequest request = new CopyObjectRequest(_Service, this.BucketName, newBlobInfo.Uid.ToString());
                request.Metadata["FileName"] = newBlobInfo.FileName;
                request.ContentType = newBlobInfo.ContentType;
                request.ContentDisposition = String.Format("filename={0}", newBlobInfo.FileName);


                //request.Metadata["ContentType"] = newBlobInfo.ContentType;
                //request.Metadata["OwnerKey"] = newBlobInfo.OwnerKey;
                //request.Metadata["OwnerType"] = newBlobInfo.OwnerType;
                //request.Metadata["DataSize"] = newBlobInfo.ContentSize.ToString();
                //request.Metadata["ContentType"] = newBlobInfo.ContentType;
                //request.Metadata["FileName"] = newBlobInfo.FileName;
                //request.Metadata["Created"] = newBlobInfo.Created.ToUniversalTime().ToString("u");
                //request.Metadata["AllowSearch"] = newBlobInfo.AllowSearch.ToString();

                using (CopyObjectResponse response = request.GetResponse())
                {
                }
                 * */

                BlobStorage.RaiseCommitedEvent(newBlobInfo, oldBlobInfo);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Initializes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="config">The config.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (!((config["accessKeyID"] == null) || string.IsNullOrEmpty(config["accessKeyID"])))
            {
                this.AccessKeyID = config["accessKeyID"];
            }
            if (!((config["secretAccessKey"] == null) || string.IsNullOrEmpty(config["secretAccessKey"])))
            {
                this.SecretAccessKey = config["secretAccessKey"];
            }
            if (!((config["bucketName"] == null) || string.IsNullOrEmpty(config["bucketName"])))
            {
                this.BucketName = config["bucketName"];
            }

            if (!((config["host"] == null) || string.IsNullOrEmpty(config["host"])))
            {
                this.Host = config["host"];
            }

            if (!((config["useSSL"] == null) || string.IsNullOrEmpty(config["useSSL"])))
            {
                this.UseSSL = Boolean.Parse(config["useSSL"]);
            }

            if (!((config["useSubDomains"] == null) || string.IsNullOrEmpty(config["useSubDomains"])))
            {
                this._useSubDomains = Boolean.Parse(config["useSubDomains"]);
            }

            if (!((config["expirationTimeSpan"] == null) || string.IsNullOrEmpty(config["expirationTimeSpan"])))
            {
                this.ExpirationTimeSpan = TimeSpan.Parse(config["expirationTimeSpan"]);
            }

            base.Initialize(name, config);

            _Service = new S3Service();
            _Service.AccessKeyID = this._AccessKeyID;
            _Service.SecretAccessKey = this._SecretAccessKey;
            _Service.UseSsl = _useSSL;
            if (!String.IsNullOrEmpty(Host))
                _Service.Host = Host;

            _Service.UseSubdomains = _useSubDomains;
        }

        #region Helper Functions
        private BlobInfo CreateBlobInfo(GetObjectResponse response, Guid uidStream)
        {
            NameValueCollection reader = response.Metadata;
            BlobInfo info = this.CreateBlobInfo(uidStream);

            info.ContentType = response.ContentType;
            info.ContentSize = response.ContentLength;
            info.FileName = uidStream.ToString();
            if (reader["OwnerKey"] != null)
            {
                info.OwnerKey = reader["OwnerKey"];
            }
            if (reader["OwnerType"] != null)
            {
                info.OwnerType = reader["OwnerType"];
            }
            if (reader["FileName"] != null)
            {
                info.FileName = reader["FileName"];
            }

            info.Created = response.LastModified;
            info.AllowSearch = false;
            return info;
        }

        private BlobInfo CreateBlobInfo(Guid uidStream)
        {
            BlobInfo info = new BlobInfo(uidStream);
            info.AllowSearch = false;
            info.Provider = this.Name;
            return info;
        }
        #endregion
    }
}
