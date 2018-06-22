using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Mediachase.Commerce.Shared
{
    /// <summary>
    /// Implements helper methods for the zip class.
    /// </summary>
    public static class ZipHelper
    {
        /// <summary>
        /// Creates the zip.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="outputFile">The output file.</param>
		public static void CreateZip(string folder, string outputFile)
		{
			FastZip fastZip = new FastZip();
			fastZip.CreateEmptyDirectories = true;
			fastZip.RestoreAttributesOnExtract = true;
			fastZip.RestoreDateTimeOnExtract = true;

			fastZip.CreateZip(outputFile, folder, true, String.Empty);
		}

        /// <summary>
        /// Extracts the zip.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="destDir">The dest dir.</param>
		public static void ExtractZip(string zipFile, string destDir)
		{
			FastZip fastZip = new FastZip();
			fastZip.CreateEmptyDirectories = true;
			fastZip.RestoreAttributesOnExtract = true;
			fastZip.RestoreDateTimeOnExtract = true;

			fastZip.ExtractZip(zipFile, destDir, FastZip.Overwrite.Always, null, String.Empty, String.Empty, true);
		}

        /// <summary>
        /// Creates the zip_old.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="outputFile">The output file.</param>
        public static void CreateZip_old(string folder, string outputFile)
        {
			string[] fileNames = Directory.GetFiles(folder);
			using (ZipOutputStream s = new ZipOutputStream(File.Create(outputFile)))
			{
				s.SetLevel(9);
				s.IsStreamOwner = false;

				// buffer for reading files' contents
				byte[] buffer = new byte[4096];
				
				foreach (string file in fileNames)
				{
					ZipEntry entry = new ZipEntry(Path.GetFileName(file));

					entry.DateTime = DateTime.Now;
					s.PutNextEntry(entry);

					using (FileStream fs = File.OpenRead(file))
					{
						// Using a fixed size buffer here makes no noticeable difference for output
						// but keeps a lid on memory usage.
						int sourceBytes;
						do
						{
							sourceBytes = fs.Read(buffer, 0, buffer.Length);
							s.Write(buffer, 0, sourceBytes);
						} 
						while (sourceBytes > 0);
					}
				}

				s.Finish();
				s.Close();
			}
        }

        /// <summary>
        /// Extracts the zip_old.
        /// </summary>
        /// <param name="zipFile">The zip file.</param>
        /// <param name="destDir">The dest dir.</param>
        public static void ExtractZip_old(string zipFile, string destDir)
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFile)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fileName = Path.Combine(destDir, theEntry.Name);

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(fileName))
                        {
                            int bufSize = 512000;
                            byte[] bt = new byte[bufSize];
                            int size = 0;
                            while ((size = s.Read(bt, 0, bufSize)) > 0)
                                streamWriter.Write(bt, 0, size);

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the file from archive.
        /// </summary>
        /// <param name="sArchivePath">The s archive path.</param>
        /// <param name="sFileName">Name of the s file.</param>
        /// <returns></returns>
        public static byte[] GetFileFromArchive(string sArchivePath, string sFileName)
        {
            ZipFile zipFile = new ZipFile(File.Open(sArchivePath, FileMode.Open, FileAccess.Read));
            ZipEntry entry = zipFile.GetEntry(sFileName);
            if (entry != null)
            {
                MemoryStream ms = new MemoryStream();
                Stream sIn = zipFile.GetInputStream(entry);
                int bufSize = 512000;
                byte[] bt = new byte[bufSize];
                int size = 0;
                while ((size = sIn.Read(bt, 0, bufSize)) > 0)
                    ms.Write(bt, 0, size);

                zipFile.Close();

                return ms.GetBuffer();
            }
            else
                return null;
        }		
    }
}
