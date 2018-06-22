using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.IO;

namespace TaxImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            ArgumentsUtility commandlineargs = new ArgumentsUtility(args);
            string applicationid;
            string siteid;
            string pathtocsv;
            DataSet TaxDataSet;

            Console.WriteLine(String.Format("Mediachase (R) Tax Import Utility Version {0}.", Assembly.GetExecutingAssembly().GetName().Version));
            Console.WriteLine(String.Format("Copyright (C) Mediachase LLC 2008. All rights reserved."));
            Console.WriteLine("");

            // Check the number of arguments.
            if (args.Length < 3 || args.Length > 3)
            {
                Console.WriteLine("Incorrect number of parameters supplied.");
                Console.WriteLine("Usage: taximporter.exe [-,--,/][applicationid:] \"applicationid_value\" [-,--,/][siteid:] \"siteid_value\" [-,--,/][pathtocsv:] \"pathtocsv_value\"");
                return;
            }
            else
            {
                // Parse applicationid.
                if (commandlineargs["applicationid"] != null)
                {
                    Console.WriteLine("applicationid value: " + commandlineargs["applicationid"]);
                    applicationid = commandlineargs["applicationid"];
                }
                else
                {
                    Console.WriteLine("Arg applicationid not defined.");
                    return;
                }

                // Parse siteid.
                if (commandlineargs["siteid"] != null)
                {
                    Console.WriteLine("siteid value: " + commandlineargs["siteid"]);
                    siteid = commandlineargs["siteid"];
                }
                else
                {
                    Console.WriteLine("Arg siteid not defined.");
                    return;
                }

                // Parse pathtocsv.
                if (commandlineargs["pathtocsv"] != null)
                {
                    Console.WriteLine("pathtocsv value: " + commandlineargs["pathtocsv"]);
                    pathtocsv = commandlineargs["pathtocsv"];
                }
                else
                {
                    Console.WriteLine("Arg pathtocsv not defined.");
                    return;
                }
            }

            // Hardcoded test values.
            // applicationid = "E1DFF5B7-8EC1-4F14-91A1-357C1BB968A0";
            // siteid = "E1DFF5B7-8EC1-4F14-91A1-357C1BB968A0";
            // pathtocsv = @"C:\TheWholeTree\main\src\Tools\TaxImporter\TaxImporter\bin\Debug\test.csv";

            // Load csv file into dataset.
            Console.WriteLine("");
            Console.WriteLine("Importing CSV file.");

            // Check if the file exists.
            if (!System.IO.File.Exists(pathtocsv))
            {
                Console.WriteLine("The file for argument pathtocsv does not exist.");
                return;
            }

            // Create parser.
            try
            {
                Mediachase.MetaDataPlus.Import.Parser.CsvIncomingDataParser parser = 
                    new Mediachase.MetaDataPlus.Import.Parser.CsvIncomingDataParser(
                        Path.GetDirectoryName(pathtocsv), 
                        true, 
                        ','
                    );
                        
                TaxDataSet = parser.Parse(Path.GetFileName(pathtocsv), null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            
            Console.WriteLine("");
            Console.WriteLine("Running record insert routine.");
            Console.WriteLine("");

            // Parse records, run insert loop routine with checking policies.
            string filename = Path.GetFileName(pathtocsv);

            for (int i = 0; i <= TaxDataSet.Tables[filename].Rows.Count - 1; i++)
            {
                // for (int j = 0; j <= TaxDataSet.Tables[filename].Columns.Count - 1; j++)
                // {
                //     Console.WriteLine(j + ": " + TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(j));
                // }

                // Populate dto's (code is order dependent on CSV)
                JurisdictionDto jurisdiction = new JurisdictionDto();
                JurisdictionGroupDto jurisdictionGroup = new JurisdictionGroupDto();
                JurisdictionRelationDto jurisdictionRelation = new JurisdictionRelationDto();
                TaxDto tax = new TaxDto();
                TaxLanguageDto taxLanguage = new TaxLanguageDto();
                TaxValueDto taxValue = new TaxValueDto();

                // JurisdictionDto
                jurisdiction.DisplayName = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(0).ToString();
                jurisdiction.StateProvinceCode = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(1).ToString();
                jurisdiction.CountryCode = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(2).ToString();
                jurisdiction.JurisdictionType = 1; // hardcoded
                jurisdiction.ZipPostalCodeStart = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(3).ToString();
                jurisdiction.ZipPostalCodeEnd = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(4).ToString();
                jurisdiction.City = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(5).ToString();
                jurisdiction.District = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(6).ToString();
                jurisdiction.County = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(7).ToString();
                jurisdiction.GeoCode = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(8).ToString();
                jurisdiction.ApplicationId = new System.Guid(applicationid);
                jurisdiction.Code = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(9).ToString();

                int jurisdictionId = -1;
                // Test for Jurisdiction record existence.
                if (!JurisdictionDao.Exists(jurisdiction))
                {
                    Console.WriteLine("Inserting Jurisdiction record.");
                    jurisdictionId = JurisdictionDao.InsertJurisdiction(jurisdiction);
                }
                else
                {
                    // The record exists; get JurisdictionId.
                    Console.WriteLine("Jurisdiction record exists, getting JurisdictionId.");
                    jurisdictionId = JurisdictionDao.GetJurisdictionId(jurisdiction);
                }
                
                // JurisdictionGroupDto
                jurisdictionGroup.ApplicationId = new System.Guid(applicationid);
                jurisdictionGroup.DisplayName = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(10).ToString();
                jurisdictionGroup.JurisdictionType = 1; // hardcoded
                jurisdictionGroup.Code = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(11).ToString();

                int jurisdictionGroupId = -1;
                // Test for JurisdictionGroup record existence.
                if(!JurisdictionGroupDao.Exists(jurisdictionGroup))
                {
                    Console.WriteLine("Inserting JurisdictionGroup record.");
                    jurisdictionGroupId = JurisdictionGroupDao.InsertJurisdictionGroup(jurisdictionGroup);
                }
                else
                {
                    // The record exists; get JurisdictionId.
                    Console.WriteLine("JurisdictionGroup record exists, getting JurisdictionGroupId.");
                    jurisdictionGroupId = JurisdictionGroupDao.GetJurisdictionGroupId(jurisdictionGroup);
                }

                // JurisdictionRelationDto
                jurisdictionRelation.JurisdictionId = jurisdictionId;
                jurisdictionRelation.JurisdictionGroupId = jurisdictionGroupId;

                // Test for JurisdictionRelation record existence.
                if(!JurisdictionRelationDao.Exists(jurisdictionRelation))
                {
                    Console.WriteLine("Inserting JurisdictionRelation record.");
                    JurisdictionRelationDao.InsertJurisdictionRelation(jurisdictionRelation);
                }

                // TaxDto
                tax.TaxType = 1; // hardcoded
                tax.Name = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(13).ToString();
                tax.SortOrder = int.Parse(TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(14).ToString());
                tax.ApplicationId = new System.Guid(applicationid);

                int taxId = -1;
                // Test for Tax record existence.
                if (!TaxDao.Exists(tax))
                {
                    Console.WriteLine("Inserting Tax record.");
                    taxId = TaxDao.InsertTax(tax);
                }
                else
                {
                    // The record exists; get TaxId.
                    Console.WriteLine("Tax record exists, getting TaxId.");
                    taxId = TaxDao.GetTaxId(tax);
                }

                // TaxLanguageDto
                taxLanguage.DisplayName = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(12).ToString();
                taxLanguage.LanguageCode = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(15).ToString();
                taxLanguage.TaxId = taxId; // from above insert

                int taxLangaugeId = -1;
                // Test for TaxLanguage record existence.
                if (!TaxLanguageDao.Exists(taxLanguage))
                {
                    Console.WriteLine("Inserting TaxLanguage record.");
                    taxLangaugeId = TaxLanguageDao.InsertTaxLanguage(taxLanguage);
                }
                else
                {
                    // The record exists; get TaxLanguageId.
                    Console.WriteLine("TaxLanguage record exists, getting TaxLanguageId.");
                    taxLangaugeId = TaxLanguageDao.GetTaxLanguageId(taxLanguage);
                }

                // TaxValueDto
                taxValue.TaxCategory = TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(16).ToString();
                taxValue.Percentage = float.Parse(TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(17).ToString());
                taxValue.AffectiveDate = DateTime.Parse(TaxDataSet.Tables[filename].Rows[i].ItemArray.GetValue(18).ToString());
                taxValue.SiteId = new System.Guid(siteid);
                taxValue.TaxId = taxId; // from above insert
                taxValue.JurisdictionGroupId = jurisdictionGroupId; // from above insert

                int taxValueId = -1;
                // Test for TaxValue record existence.
                if (!TaxValueDao.Exists(taxValue))
                {
                    Console.WriteLine("Inserting TaxValue record.");
                    taxValueId = TaxValueDao.InsertTaxValue(taxValue);
                }
                //else
                //{
                //    // The record exists; get TaxValueId.
                //    Console.WriteLine("TaxValue record exists, getting TaxValueId.");
                //    taxValueId = TaxValueDao.GetTaxValueId(taxValue);
                //}

            }

            #region Insert Order
            /* (1) Jurisdiction
             * - Does it already exist? 
             * - Check for uniqueness by JurisdictionType, Code, ApplicationId(?)
             * (2) JurisdictionGroup
             * - Does it already exist? 
             * - Check for uniqueness by JurisdictionType, Code, ApplicationId(?)
             * (3) JurisdictionRelation
             * - Does it already exist? 
             * - Check for uniqueness by JurisdictionId, JurisdictionGroupId
             * (4) Tax
             * - Does it already exist? 
             * - Check for uniqueness by  ApplicationId
             * (5) TaxLanguage
             * - Does it already exist? 
             * - Check for uniqueness by DisplayName, LanguageCode, TaxId
             * (6) TaxValue
             * - Does it already exist? 
             * - Check for uniqueness by DisplayName, LanguageCode, TaxId
             * - FK: JurisdictionGroupId
             */
            #endregion

            // TODO: If record already exists overwrite for now.
            // TODO: 20081001 - Update stored procs are gone. Leaving existing records alone for now.
            
            Console.WriteLine("");
            Console.WriteLine("CSV file successfully imported. Thank you.");
            Console.ReadLine();
        }
    }
}
