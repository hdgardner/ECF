using System;
using System.Collections;

namespace Mediachase.Cms
{
	/// <summary>
	/// File extension description
	/// </summary>
	public static class FileExtension
	{
		private static Hashtable _Description;

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
		public static Hashtable Description
		{
			get
			{
				return _Description;
			}
		}

        /// <summary>
        /// Initializes the <see cref="FileExtension"/> class.
        /// </summary>
		static FileExtension()
		{
			_Description = new Hashtable();
			//Images
			Description.Add(".GIF","����������� GIF");
			Description.Add(".JPG","����������� JPG");
			Description.Add(".JPEG","����������� JPG");
			Description.Add(".BMP","����������� BMP");
			Description.Add(".TIFF","����������� TIFF");
			Description.Add(".ICO","����������� ICO");
			Description.Add(".PNG","����������� PNG");
			//Documents
			Description.Add(".TXT","��������� ����");
			Description.Add(".PDF","�������� Adobe Acrobat Reader");
			Description.Add(".DOC","�������� Microsoft Word");
			Description.Add(".XLS","�������� Microsoft Excel");
			//Archives
			Description.Add(".RAR","����� RAR");
			Description.Add(".ZIP","����� ZIP");
			//Media
			Description.Add(".FLA","������ flash");
			Description.Add(".SWF","������ flash");
			Description.Add(".WAV","�����������");
			Description.Add(".MP3","�����������");
			Description.Add(".MPG","�����������");
			Description.Add(".AVI","�����������");
			Description.Add(".MPEG","�����������");
			//EXE
			Description.Add(".EXE","����������");
			Description.Add(".MSI","����������");
		}
	}
}
