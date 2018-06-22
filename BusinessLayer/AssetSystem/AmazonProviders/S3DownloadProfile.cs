using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mediachase.Ibn.Blob.BlobProfileDownload;
using Mediachase.Ibn.Blob.BlobProfileDownload.Configuration;
using Mediachase.Ibn.Blob;
using System.Web;
using LitS3;

namespace Mediachase.Library.AmazonProviders
{
    public class S3DownloadProfile : BaseBlobProfile
    {
        // Fields
        private BlobInfo _blobInfo;
        private BlobStorageProvider _provider;
        private bool _AuthEnable = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3DownloadProfile"/> class.
        /// </summary>
        /// <param name="profile">The profile.</param>
        public S3DownloadProfile(BlobProfileDownloadElement profile)
            : base(profile)
        {
            _AuthEnable = profile.AuthEnable;
        }

        /// <summary>
        /// Gets the profile ticket.
        /// </summary>
        /// <param name="blobInfo">The BLOB info.</param>
        /// <returns></returns>
        public override string GetProfileTicket(Mediachase.Ibn.Blob.BlobInfo blobInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes the by ticket.
        /// </summary>
        /// <param name="ticket">The ticket.</param>
        protected override void InitializeByTicket(string ticket)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void ProcessRequest(System.Web.HttpContext context)
        {
            if (this._blobInfo == null)
            {
                throw new BlobDownloadException(400, "Not found");
            }
            if (this._provider == null)
            {
                throw new BlobDownloadException(400, "Not found");
            }
            if (!(this._provider is S3StorageProvider))
            {
                throw new BlobDownloadException(400, "Not Found");
            }
            S3StorageProvider provider = (S3StorageProvider)_provider;
            S3Service service = provider.Service;
            string url = String.Empty;

            if(_AuthEnable)
                url = service.GetAuthorizedUrl(provider.BucketName, this._blobInfo.Uid.ToString(), DateTime.UtcNow.Add(provider.ExpirationTimeSpan));
            else
                url = service.GetUrl(provider.BucketName, this._blobInfo.Uid.ToString());

            context.Response.Redirect(url);
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="blobInfo">The BLOB info.</param>
        public override void ProcessRequest(HttpContext context, BlobInfo blobInfo)
        {
            this._blobInfo = blobInfo;
            this._provider = BlobStorage.Providers[blobInfo.Provider];
            this.ProcessRequest(context);
        }
    }
}
