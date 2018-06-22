using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Mediachase.Ibn.Blob.BlobProfileDownload;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Mediachase.Ibn.Library
{
    public class DownloadFilterThumbnail : DownloadFilterBase
    {
        private int width;
        private int height;

        /// <summary>
        /// Initializes the specified ?.
        /// </summary>
        /// <param name="parameters"></param>
        public override void Initialize(NameValueCollection parameters)
        {
            if(!String.IsNullOrEmpty(parameters["width"]))
            {
                width = Convert.ToInt32(parameters["width"]);
            }

            if (!String.IsNullOrEmpty(parameters["height"]))
            {
                height = Convert.ToInt32(parameters["height"]);
            }
        }

        /// <summary>
        /// Processes the filter.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        public override void ProcessFilter(object sender, DownloadFilterArgs args)
        {
              
            Image img = Image.FromStream(args.FilteredStream);

            if(img.RawFormat.Guid == ImageFormat.Jpeg.Guid
                || img.RawFormat.Guid == ImageFormat.MemoryBmp.Guid)
            {
                MemoryStream stream = new MemoryStream();
                Image thumbnail = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
                thumbnail.Save(stream, ImageFormat.Jpeg);

                args.FilteredStream = stream;
                args.BlobInfo.ContentSize = stream.Length;
            }
           
        }

        
    }
}
