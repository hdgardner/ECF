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
			Description.Add(".GIF","Изображение GIF");
			Description.Add(".JPG","Изображение JPG");
			Description.Add(".JPEG","Изображение JPG");
			Description.Add(".BMP","Изображение BMP");
			Description.Add(".TIFF","Изображение TIFF");
			Description.Add(".ICO","Изображение ICO");
			Description.Add(".PNG","Изображение PNG");
			//Documents
			Description.Add(".TXT","Текстовый файл");
			Description.Add(".PDF","Документ Adobe Acrobat Reader");
			Description.Add(".DOC","Документ Microsoft Word");
			Description.Add(".XLS","Документ Microsoft Excel");
			//Archives
			Description.Add(".RAR","Архив RAR");
			Description.Add(".ZIP","Архив ZIP");
			//Media
			Description.Add(".FLA","Объект flash");
			Description.Add(".SWF","Объект flash");
			Description.Add(".WAV","Аудиозапись");
			Description.Add(".MP3","Аудиозапись");
			Description.Add(".MPG","Видеозапись");
			Description.Add(".AVI","Видеозапись");
			Description.Add(".MPEG","Видеозапись");
			//EXE
			Description.Add(".EXE","Приложение");
			Description.Add(".MSI","Приложение");
		}
	}
}
