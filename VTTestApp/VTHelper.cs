/**
 * VTHelper
 * Author: Austin Truong
 * Date: 09/04/2012
 * Varian Medical Systems
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace VT.Classes
{
    public class VTHelper
    {
        /**
         *  Method : formatStringSerialActivity
         *  Function : Formats the elements in a given array to the correct protocol format for the database.
         *  Paramaters : (header - Header of type of values) (items - Array of all the items to be inserted)
         *  Return : String formatted correctly for the database
         * */
        public string formatStringSerialActivity(string header, List<string> items)
        {
            string format = "'" + header + "',";
            for (int i = 0; i < items.Count; i++)
            {
                format += "'" + items.ElementAt(i) + "',";
            }
            string formatted = format.Remove(format.Count() - 1, 1); // Cuts the last element which will be a ","
            return formatted;
        }

        /**
         *  Method : formatStringActivity
         *  Function : Formats the elements in a given array to the correct protocol format for the database.
         *  Paramaters : (header - Header of type of values) (items - Array of all the items to be inserted)
         *  Return : String formatted correctly for the database
         * */
        public string formatStringActivity(string header, List<string> items)
        {
            string format = "|" + header + "|";
            for (int i = 0; i < items.Count; i++)
            {
                format += items.ElementAt(i) + "|";
            }
            int formatlength = format.Length;
            string formatted = format.Remove(format.Count() - 1, 1); // Cuts the last element which will be a "|"
            return formatted;
        }

        /**
         * Method : parseDetailOptions
         * Function : Parses out a string into a list of all the options
         * Paramaters : The string to be parsed
         * Return : A list of all of the detail options.
         * */
        public List<string> parseDetailOptions(string str)
        {
            List<string> options = new List<string>();
            string option = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ',')
                {
                    options.Add(option);
                    option = "";    // Resets Option
                }
                else
                {
                    option += str[i];
                }
            }
            options.Add(option);
            
            return options;
        }

        /**
         * Method : parseStringActivity
         * Function : Parses the information the user wants from the string given.
         * Paramaters : (str - The string to be parsed) (token - Whichever token wanted from the string)
         * Return : String format of the token wanted
         * Protocol : String token must be in CAPS
         **/
        public string parseStringActivity(string cmd, string token)
        {
            string parseString = "";
            for (int i = 0; i < cmd.Length; i++)
            {
                if (cmd[i] == '|')
                {
                    // If the token after the pipe is the same as the token given.
                    if (cmd[i + 1] == token[0] && cmd[i + 2] == token[1] && cmd[i + 3] == token[2])
                    {
                        // parse the value within the bounds
                        int curr_loc = i + 3;
                        for (int j = curr_loc + 1; j < cmd.Length; j++)
                        {
                            if (cmd[j] != '|')
                            {
                                parseString += cmd[j];
                            }
                            else
                            {
                                return parseString;
                            }
                        }
                    }
                }
            }
            return parseString;
        }

        /**
         * Method : checkCorrectForm
         * Function : Given a list of field objects, it determines if form is filled correctly, and all the data is filled.
         * Paramater : A list of field objects
         * Return : String the error message or a string "0" for pass
         * */
        public string checkCorrectForm(List<VTFieldObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Value == null)
                {
                    return list[i].Label + "is missing";
                }


                // Handling Decimal 
                
                //if (list[i].DataType == "DATA-DECIMAL")
                //{
                //    bool containDecimal = false;
                //    for (int j = 0; j < list[i].Value.Length; j++)
                //    {
                //        if (list[i].Value[j] == '.')
                //        {
                //            containDecimal = true;
                //        }
                //    }

                //    if (!(containDecimal))
                //    {
                //        return list[i].Label + "is missing a deicmal";
                //    }

                //}

            }
            return "0";
        }

        /**
         * Method : parseInDecimal
         * Function : Given a decimal, will determine if it is necessary to parse in a decimal or not.
         * Paramater : A decimal
         * Return : The Decimal
         * */
        public string parseInDecimal(string _decimal)
        {
            bool hasDecimal = false;
            foreach (char c in _decimal)
            {
                if (c == '.' || c == '.')
                {
                    hasDecimal = true;
                    break;
                }
            }

            if (hasDecimal)
            {
                return _decimal;
            }
            else
            {
                string newDecimal = "." + _decimal;
                return newDecimal;
            }

        }

        /**
         * Method : checkErrors
         * Function : Given a "number" string will be the error code. The method wll check which error the errorcode actually is.
         * Paramater : (string - errorcode)
         * Return : string of the error message.
         * */
        public string checkErrors(string errorcode, string errorsmg)
        {
            string msg = "";
            switch (errorcode)
            {
                case "0":
                    msg = "pass";
                    break;
                case "51001":
                    msg = "The serial number does not exist";
                    break;
                case "51002":
                    msg = "Submit a new process";
                    break;
                case "51009":
                    msg = "A field(s) is missing data";
                    break;
                case "51011":
                    msg = "Part / Assembly / Serial Number does not exist";
                    break;
                case "51013":
                    msg = "Configuration has not been set for Part/Asm Number";
                    break;
                case "51015":
                    msg = "VT ID does not exist";
                    break;
                case "51016":
                    msg = "Workflow does not exist for this Serial Number";
                    break;
                case "51020":
                    msg = "Workflow for this serial number has been completed";
                    break;
                case "8114":
                    msg = "A field(s) has the wrong data type, expecting a decimal value";
                    break;
                default:
                    msg = errorsmg;
                    break;
            }
                return msg;
        }

        /**
         * Method : expandList
         * Function : Expands the multidimension list whenever an item to be inserted.
         * Paramaters : A multidimensional list, int a number for a row, number for the column
         * Return : None, just expands the list
         * */
        public void expandList(List<List<VTFieldObject>> list, int row, int col)
        {

            while (list.Count < row)
            {
                //list.Add(default(String));
                List<VTFieldObject> tmp = new List<VTFieldObject>();
                list.Add(tmp);
            }
            row = row - 1;
            while (list[row] == null || list[row].Count < col)
            {
                VTFieldObject tmpObject = new VTFieldObject(); // An empty VTFieldObject
                if (list[row] == null)
                {
                    list[0].Add(tmpObject);
                }
                else
                {
                    list[row].Add(tmpObject);
                }
            }
        }

        public string trueFalseStringtToIntBool(string _string)
        {
            if (_string == "True")
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }

        public int boolToInt(bool flag)
        {
            if (flag)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public bool intToBool(int num)
        {
            if (num == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public string VTIDtoEMPID(string VTID)
        {
            string EMPID = (Convert.ToInt64(VTID, 2)).ToString();

            if (EMPID.Length == 4)
            {
                EMPID = "0" + EMPID;
            }

            return EMPID;
        }

        public string BuildDocument(string filepath, string sn, List<VTFormObject> forms)
        {
            try
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document))
                //using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filepath, true))
                {

                    MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document();

                    var margin_size = 100;

                    PageMargin pargeMargins = new PageMargin();
                    pargeMargins.Top = margin_size;
                    pargeMargins.Bottom = margin_size;
                    Columns columns = new Columns();
                    columns.ColumnCount = 2;
                    

                    SectionProperties sectionProps = new SectionProperties();
                    sectionProps.Append(columns);
                    sectionProps.Append(pargeMargins);
                    
                    Body body = mainPart.Document.AppendChild(new Body());
                    body.Append(sectionProps);
                    ParagraphProperties paragraphProperties = new ParagraphProperties
                       (
                           //new ParagraphStyleId() { Val = "No Spacing" },
                           new SpacingBetweenLines() { After = "0" }
                       );


                    
                    Paragraph para_main = body.AppendChild(new Paragraph(paragraphProperties));

                    // Creating the Header where the Serial Number will exist

                    // Serial Number
                    Run run_mainHeader = para_main.AppendChild(new Run());
                    RunProperties runProp_mainHeader = new RunProperties();    // Create run properties.
                    FontSize size_mainHeader = new FontSize();
                    size_mainHeader.Val = new StringValue("48");                     
                    runProp_mainHeader.Append(size_mainHeader);
                    run_mainHeader.Append(runProp_mainHeader);                              // Append all of the properties
                    run_mainHeader.Append(new Text("S/N: " + sn));
                    run_mainHeader.Append(new Break());

                    // Serial Barcode
                    Run run_barcode = para_main.AppendChild(new Run());
                    RunProperties runProp_barcode = new RunProperties();                    // Create run properties.
                    RunFonts runFontMain_barcode = new RunFonts();                          // Create font
                    runFontMain_barcode.Ascii = "Code39AzaleaNarrow1";                      // Specify font family
                    FontSize size_barcode = new FontSize();
                    size_barcode.Val = new StringValue("56");

                    runProp_barcode.Append(runFontMain_barcode);
                    runProp_barcode.Append(size_barcode);

                    run_barcode.PrependChild<RunProperties>(runProp_barcode);
                    sn = sn.ToUpper();                                                      // Sets all the values to uppercase to be a barcode format
                    run_barcode.AppendChild(new Text("*" + sn + "*"));
                    run_barcode.AppendChild(new Break());

                    // Tube Type
                    Run run_tubetype = para_main.AppendChild(new Run());
                    RunProperties runProp_tubetype = new RunProperties();                   // Create run properties.
                    FontSize size_tubetype = new FontSize();
                    size_tubetype.Val = new StringValue("38");
                    runProp_tubetype.Append(size_tubetype);
                    run_tubetype.Append(runProp_tubetype);                                  // Append all of the properties
                    run_tubetype.Append(new Text("Tube Type: " + forms[0].TubeType + " "));
                    run_tubetype.Append(new Break());

                    // Tube Barcode
                    Run run_barcode_tube = para_main.AppendChild(new Run());
                    RunProperties runProp_barcode_tube = new RunProperties();               // Create run properties.
                    RunFonts runFontMain_barcode_tube = new RunFonts();                     // Create font
                    runFontMain_barcode_tube.Ascii = "Code39AzaleaNarrow1";                 // Specify font family
                    FontSize size_barcode_tube = new FontSize();
                    size_barcode_tube.Val = new StringValue("56");

                    runProp_barcode_tube.Append(runFontMain_barcode_tube);
                    runProp_barcode_tube.Append(size_barcode_tube);

                    run_barcode_tube.PrependChild<RunProperties>(runProp_barcode_tube);
                    sn = sn.ToUpper();                                                       // Sets all the values to uppercase to be a barcode format
                    run_barcode_tube.AppendChild(new Text("*" + forms[0].TubeType + "*"));
                    run_barcode_tube.AppendChild(new Break());



                    // Goes through all of the forms
                    foreach (var form in forms)
                    {
                        
                        // Set up a header per form
                        Run run_header = para_main.AppendChild(new Run());
                        RunProperties runProp_formHeader = new RunProperties();
                        Bold bold = new Bold();
                        Underline ul = new Underline() { Val = DocumentFormat.OpenXml.Wordprocessing.UnderlineValues.Single };
                        FontSize size_formHeader = new FontSize();
                        size_formHeader.Val = new StringValue("24");
                        runProp_formHeader.Append(size_formHeader);
                        runProp_formHeader.Append(bold);
                        runProp_formHeader.Append(ul);
                        run_header.AppendChild(new RunProperties(runProp_formHeader));
                        //run_header.AppendChild(new RunProperties(new Bold(), new Underline()));

                        string username = form.Username;
                        string proces_header = form.HeaderTitle;

                        run_header.AppendChild(new Text(proces_header));
                        run_header.AppendChild(new Break());

                        // Goes through all of the fields that each form contains.
                        for (int i = 0; i < form.FieldList.Count; i++)
                        {
                            // Do not need to print out user or serial for each form.
                            if (!(form.FieldList[i].Token == "SNT"))
                            {

                                Run run_data = para_main.AppendChild(new Run());
                                if (form.FieldList[i].Default)
                                {
                                    run_data.AppendChild(new Text(form.FieldList[i].Label));
                                }
                                else
                                {
                                    run_data.AppendChild(new Text(form.FieldList[i].Label + " " + form.FieldList[i].Spec + form.FieldList[i].Value));
                                }
                                run_data.AppendChild(new Break());
                            }
                        }
                    
                    }

                    mainPart.Document.Save();
                    wordDoc.Close();
                    return "Success";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
    }

}