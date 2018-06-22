using System;
using System.Collections.Generic;
using System.Text;
using Mediachase.Commerce.Catalog;
using Mediachase.Commerce.Catalog.Dto;
using Mediachase.Commerce.Catalog.Search;
using Mediachase.Commerce.Catalog.Managers;
using Mediachase.MetaDataPlus;
using Lucene.Net.Documents;
using Mediachase.MetaDataPlus.Configurator;
using Lucene.Net.Store;
using Lucene.Net.Index;
using System.IO;
using System.Data;
using Mediachase.Commerce.Core;
using System.Globalization;
using Mediachase.Commerce;
using Mediachase.Commerce.Core.Dto;
using Mediachase.Commerce.Core.Managers;
using Mediachase.Search.Extensions.Indexers;
using Mediachase.Search;
using Mediachase.Commerce.Storage;
using System.Collections;

namespace NWTD.Search.Extensions.Indexers {
	/// <summary>
	/// A custom indexer that accounts for Grade ranges. 
	/// It provides the ability to index grades in such a way that grade ranges (e.g. k-8) can be indexed so that they would be returned in searches for grades within the range (e.g. grade 6).
	/// Use this indexer for NWTD search indexing rather than the built-in one.
	/// </summary>
	public class CatalogIndexBuilder : Mediachase.Search.Extensions.Indexers.CatalogIndexBuilder {

		/// <summary>
		/// Adds a field to the search index
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="metaField"></param>
		/// <param name="metaObject"></param>
		protected override void AddField(Document doc, MetaField metaField, System.Collections.Hashtable metaObject) {
			if (metaField.Name == "Grade") {
				this.AddGradeFields(doc, metaField, metaObject);
			}
			base.AddField(doc, metaField, metaObject);
		}


		/// <summary>
		/// Adds a "grade" field to the search index
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="metaField"></param>
		/// <param name="metaObject"></param>
		private void AddGradeFields(Document doc, MetaField metaField, System.Collections.Hashtable metaObject) {
			object val = MetaHelper.GetMetaFieldValue(metaField, metaObject[metaField.Name]);
			string[] grades = parseGradeRange(val.ToString());
			if (grades.Length > 0) { 
				//Add the smallest grade to the index and make it a number for sorting
				int gradeNumber = parseGradeName( grades[0])  + 1;//no negative numbers in the index
				doc.Add(new Field("GradeNumber", ConvertStringToSortable(gradeNumber.ToString()), Field.Store.YES, Field.Index.UN_TOKENIZED));
			}
			foreach (string grade in grades) {
				doc.Add(new Field("Grade", grade, Field.Store.YES, Field.Index.UN_TOKENIZED));
			}
		}

		/// <summary>
		/// parses a grade range from a string
		/// </summary>
		/// <param name="gradeRange">The grade range string. E.g. "K-6" or "PS"</param>
		/// <returns></returns>
		private string[] parseGradeRange(string gradeRange) {
			gradeRange = gradeRange.ToLower();
			
			string[] bounds = gradeRange.Split('-');
			if (bounds.Length == 1) return bounds;
			//this.OnEvent(string.Format("Parsing {0}", bounds[1]), 0);

			List<string> grades = new List<string>();
			for (int i = parseGradeName(bounds[0]); i <= parseGradeName(bounds[1]); i++) {
				grades.Add(parseGradeNumber(i));
			}

			return grades.ToArray();
			//return new string[0];
		}

		/// <summary>
		/// Converts a grade number to a string.
		/// </summary>
		/// <param name="gradeNumber"></param>
		/// <returns></returns>
		private string parseGradeNumber(int gradeNumber) {
			switch (gradeNumber) {
				case -2:
					return "na";
				case -1:
					return "ps";
				case 0:
					return "k";
				case 13:
					return "ap";
				default:
					return gradeNumber.ToString();
			}
		}

		/// <summary>
		/// converts a grade name to an integer
		/// </summary>
		/// <param name="gradeName"></param>
		/// <returns></returns>
		private int parseGradeName(string gradeName) {
			gradeName = gradeName.ToLower();
			switch (gradeName) {
				case "na":
					return -2;
				case "ps":
					return -1;
				case "k":
					return 0;
				case "ap":
					return 13;
				default:
					int gradenumber = -3;
					int.TryParse(gradeName, out gradenumber);
					return gradenumber;
			}
		}

	}
}
