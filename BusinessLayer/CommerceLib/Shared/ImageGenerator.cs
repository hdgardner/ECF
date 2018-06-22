using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Mediachase.Commerce.Catalog;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements operations for the image generator.
    /// </summary>
    public static class ImageGenerator
    {
        /// <summary>
        /// Thumbnails the callback.
        /// </summary>
        /// <returns></returns>
        public static bool ThumbnailCallback() { return false; }

        /// <summary>
        /// Creates the image thumbnail.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="stretch">if set to <c>true</c> [stretch].</param>
        /// <returns></returns>
        public static byte[] CreateImageThumbnail(byte[] data, string contentType, int height, int width, bool stretch)
        {
            MemoryStream stream = new MemoryStream(data);
            return CreateImageThumbnail(stream, contentType, height, width, stretch);
        }

        /// <summary>
        /// Creates the image thumbnail.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="stretch">if set to <c>true</c> [stretch].</param>
        /// <returns></returns>
        public static byte[] CreateImageThumbnail(Stream stream, string contentType, int height, int width, bool stretch)
        {
            stream.Position = 0;

            Bitmap image = (Bitmap)Bitmap.FromStream(stream, true);

            //Bitmap image = new Bitmap(data);
            int widthTh, heightTh; float widthOrig, heightOrig;
            float fx, fy, f;

            if (!stretch)
            {
                // retain aspect ratio
                widthOrig = image.Width;
                heightOrig = image.Height;
                fx = widthOrig / width;
                fy = heightOrig / height; // subsample factors

                // must fit in thumbnail size
                f = Math.Max(fx, fy); if (f < 1) f = 1;
                widthTh = (int)(widthOrig / f); heightTh = (int)(heightOrig / f);
            }
            else
            {
                widthTh = width; heightTh = height;
            }

            // We got here, so we do want to scale it.
            Graphics graph;
            Bitmap bitmap = new Bitmap(widthTh, heightTh);
            graph = Graphics.FromImage(bitmap);
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // pre paint white to the background of transparent images
            graph.Clear(Color.White);
            graph.DrawImage(image, 0, 0, widthTh, heightTh);

            if (contentType.Contains("pjpeg"))
                contentType = "image/jpeg";

            ImageCodecInfo codec = GetEncoderInfo(contentType);

            if (codec == null)
                codec = FindEncoder(CatalogConfiguration.Instance.EncodingSettings.DefaultFormat);

            // set image quality
            EncoderParameters eps = new EncoderParameters(1);
            eps = new EncoderParameters();
            eps.Param[0] = new EncoderParameter(Encoder.Quality, CatalogConfiguration.Instance.EncodingSettings.ImageQualityPercentage);

            // Save to the cache
            MemoryStream data = new MemoryStream();
            bitmap.Save(data, codec, eps);

            bitmap.Dispose();
            graph.Dispose();
            eps.Dispose();
            image.Dispose();

            return data.ToArray();
        }

        /// <summary>
        /// Gets the encoder information for the specified mimetype.  Used in imagescaling
        /// </summary>
        /// <param name="mimeType">The mimetype of the picture.</param>
        /// <returns>System.Drawing.Imaging.ImageCodecInfo</returns>
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] myEncoders =
                ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo myEncoder in myEncoders)
                if (myEncoder.MimeType == mimeType)
                    return myEncoder;

            return null;
        }

        /// <summary>
        /// Finds the encoder.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        internal static ImageCodecInfo FindEncoder(ImageFormat format)
        {
            ImageCodecInfo[] infoArray1 = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo[] infoArray2 = infoArray1;
            for (int num1 = 0; num1 < infoArray2.Length; num1++)
            {
                ImageCodecInfo info1 = infoArray2[num1];
                if (info1.FormatID.Equals(format.Guid))
                {
                    return info1;
                }
            }
            return null;
        }
    }
}
