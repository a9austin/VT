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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Drawing.Imaging;
using iTextSharp.text.pdf.draw;
using System.Net.NetworkInformation;
using System.Web.Helpers;
using System.Web.Mvc;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;


namespace VT.Classes
{
    public class VTHelper
    {
        /**
         *  Method : formatStringComma
         *  Function : Formats the elements in a given array to the correct protocol format for the database.
         *  Paramaters : (header - Header of type of values) (items - Array of all the items to be inserted)
         *  Return : String formatted correctly for the database
         * */
        public string formatStringComma(string header, List<string> items)
        {
            string format = "";
            if (!(header == ""))
            {
                format = "'" + header + "',";
            }
            
            for (int i = 0; i < items.Count; i++)
            {
                format += "'" + items.ElementAt(i) + "',";
            }
            string formatted = format.Remove(format.Count() - 1, 1); // Cuts the last element which will be a ","
            return formatted;
        }

        /**
         *  Method : formatStringPipe
         *  Function : Formats the elements in a given array to the correct protocol format for the database.
         *  Paramaters : (header - Header of type of values) (items - Array of all the items to be inserted)
         *  Return : String formatted correctly for the database
         * */
        public string formatStringPipe(string header, List<string> items)
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
        
        /**
         *  0 is false 1 is true
         * */
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

        /**
        *  1 is false 0 is true
        * */
        public bool intToBoolZeroIsTrue(int num)
        {
            if (num == 0)
            {
                return true;
            }
            else
            {
                return false;
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

        /**     
         *  Min and Max Datatype Values
         *  DATA-TINY Integer values 0 to 255 (cannot be negative)
         *  DATA-INT Integer values from -2,147,483,648 to 2,147,483,647
         *  DATA-BIGINT Integer values from -2^63 to 2^63. (plus or minus 92 quintrillion)
         *  DATA-DECIMAL Defined as 10 digits before the decimal point, and 5 digits after the decimal point. 
         *
         *  DATA-SMCHAR up to a length of 50 characters
         *  DATA-LGCHAR up to a length of 800 characters
         *
         *  DATA-CKBOX uses the Tiny value type, 1 for checked, 0 for unchecked
         *  DATA-RADIO uses the SMCHAR value type, up to a length of 50 characters
         *  DATA-LIST uses the SMCHAR value type, up to a length of 50 charactezrs
         *  
         *  If the message is "0" that means the check has passed.
         *
         * */
        public string checkDataValidation(string val, string type, string label)
        {
            string message = "";
            int val_int_ver = 0;
            switch (type)
            {
                case "DATA-TINY":
                    try{
                        val_int_ver = Convert.ToInt32(val);
                    }
                    catch (Exception e){
                        message = e.Message;
                    }

                    // DATA-TINY Integer values 0 to 255 (cannot be negative)
                    if (val_int_ver > 0 || val_int_ver < 225)
                    {
                        message = "The field's value cannot be above 225, or negative.";
                    }
                    else
                    {
                        message = "0";
                    }
                    break;
                case "DATA-INT":
                    try{
                        val_int_ver = Convert.ToInt32(val);
                    }
                    catch (Exception e){
                        message = e.Message;
                    }

                    // DATA-INT Integer values from -2,147,483,648 to 2,147,483,647
                    if (val_int_ver > 2147483647 || val_int_ver < -2147483648)
                    {
                        message = "This field cannot be above 2,147,483,648 or below -2,147,483,648.";
                    }
                    else
                    {
                        message = "0";
                    }
                    break;
                case "DATA-BIGINT":
                    try
                    {
                        val_int_ver = Convert.ToInt32(val);
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                    }

                    //Integer values from -2^63 to 2^63. (plus or minus 92 quintrillion)
                    if (val_int_ver > (2 ^ 63) || val_int_ver < (-2 ^ 63))
                    {
                        message = "This field cannot be above 2^63 or -2^63 (+- 92 quintrillion)";

                    }
                    else
                    {
                        message = "0";
                    }
                    break;
                case "DATA-DECIMAL":
                    try
                    {
                        decimal val_dec_ver = Convert.ToDecimal(val);
                    }
                    catch (Exception e)
                    {
                        message = e.Message;
                    }

                    // DATA-DECIMAL Defined as 10 digits before the decimal point, and 5 digits after the decimal point. 
                    int count = 0;
                    foreach (char c in val)
                    {
                        bool afterDec = false;
                        count++;
                        if (c == '.')
                        {
                            afterDec = true;
                            if (count > 15)
                            {
                                message = "Cannot have more then 15 digits before the decimal";
                                return message;
                            }
                            count = 0;
                        }
                        if (afterDec)
                        {
                            count++;
                        }
                    }

                    if (count > 5)
                    {
                        message = "Cannot have more then 5 digits before the decimal";
                    }
                    else
                    {
                        message = "0";
                    }
                    
                    
                    break;
                case "DATA-SMCHAR":
                    // DATA-SMCHAR up to a length of 50 characters
                    if (val.Length > 50)
                    {
                        message = "The input field cannot be above 50 characters";
                    }
                    else
                    {
                        message = "0";
                    }
                    break;
                case "DATA-LGCHAR":
                    // DATA-LGCHAR up to a length of 800 characters
                    if (val.Length > 800)
                    {
                        message = "The input field cannot be above 800 characters";
                    }
                    else
                    {
                        message = "0";
                    }

                    break;
                case "DATA-CKBOX":
                    // DATA-CKBOX uses the Tiny value type, 1 for checked, 0 for unchecked
                    if (!(val == "0" || val == "1"))
                    {
                        message = "The check box value must be a 0 or 1";
                    }
                    else
                    {
                        message = "0";
                    }
                    break;
                case "DATA-RADIO":
                    // DATA-RADIO uses the SMCHAR value type, up to a length of 50 characters
                    if (val.Length > 50)
                    {
                        message = "Radio field must be less then 50 characters";
                    }
                    else
                    {
                        message = "0";
                    }
                    break;
                case"DATA-LIST":
                    // DATA-RADIO uses the SMCHAR value type, up to a length of 50 characters
                    if (val.Length > 50)
                    {
                        message = "List field must be less then 50 characters";
                    }
                    else
                    {
                        message = "0";
                    }
                    break;

            }
            return message;
        }

        /**
         * Builds an output pdf file regarding to a tube serial number, essentially it is the tube record form in output form.
         * Paramaters : SN-serial number, forms-List of all the information to be outputted, path-path of where the pdf should be outputted.
         * Return : A message of passing failing.
         * */
        public string iTextBuildDocument(string sn, List<VTFormObject> forms, string path)
        {
            var doc = new Document();
            try
            {
                var curr_serial = sn;
                var subtitle = 15f;
                var field_font_size = 10f;
                var form_header_font_size = 12f;
                PdfWriter writer = null;
                //String path_name = Properties.Settings.Default.FilePathOutput + sn + "_output.pdf";

                //PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(@path, FileMode.Create));
                try
                {
                    writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create)); 
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                BaseFont helvetica = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
                doc.SetMargins(0f, 0f, 0f, 0f);

                doc.Open();

                PdfContentByte cb = writer.DirectContent;

                PdfPTable table = new PdfPTable(2);
                //table.HorizontalAlignment = Element.ALIGN_LEFT;

                PdfPCell main_title_cell = new PdfPCell(new Phrase("Tube Record Sheet", new Font(helvetica, 30f)));
                main_title_cell.Colspan = 2;
                main_title_cell.HorizontalAlignment = 1;
                table.AddCell(main_title_cell);

                // Serial Barcode, Image initalization 
                Barcode39 sn_code = new Barcode39();
                sn_code.Code = sn;
                Image sn_code_image = sn_code.CreateImageWithBarcode(cb, null, null);
                sn_code_image.ScalePercent(80f);
                Barcode39 tube_code = new Barcode39();
                tube_code.Code = forms[0].TubeType;

                Image tube_code_image = tube_code.CreateImageWithBarcode(cb, null, null);
                tube_code_image.ScalePercent(80f);
                PdfPCell sn_image_cell = new PdfPCell(sn_code_image);
                sn_image_cell.HorizontalAlignment = 1;
                PdfPCell tube_image_cell = new PdfPCell(tube_code_image);
                tube_image_cell.HorizontalAlignment = 1;

                PdfPCell sn_label_cell = new PdfPCell(new Phrase(sn, new Font(helvetica, subtitle)));
                sn_label_cell.HorizontalAlignment = 1;
                PdfPCell tube_label_cell = new PdfPCell(new Phrase(forms[0].TubeType, new Font(helvetica, subtitle)));
                tube_label_cell.HorizontalAlignment = 1;

                table.AddCell(sn_label_cell);
                table.AddCell(sn_image_cell);
                table.AddCell(tube_image_cell);
                table.AddCell(tube_label_cell);


                int cell_count = 0;
                for (int i = 0; i < forms.Count; i++)
                {
                    int j = 0;
                    if (curr_serial != forms[i].SerialNumber)
                    {
                        doc.Add(table); // Add the last table to the document
                        table = new PdfPTable(2); // Reset the table
                        curr_serial = forms[i].SerialNumber; // Set the current serial to current
                        doc.NewPage(); // Page break;


                        PdfPCell curr_title_cell = new PdfPCell(new Phrase(forms[i].TubeType, new Font(helvetica, 30f)));
                        curr_title_cell.Colspan = 2;
                        curr_title_cell.HorizontalAlignment = 1;
                        table.AddCell(curr_title_cell);
                        cell_count = cell_count + 2;

                        // Adds a serial number as a title to the new page break.
                        PdfPCell curr_sn_cell = new PdfPCell(new Phrase(curr_serial, new Font(helvetica, 20f)));
                        curr_sn_cell.Colspan = 2;
                        curr_sn_cell.HorizontalAlignment = 1;
                        table.AddCell(curr_sn_cell);
                        cell_count = cell_count + 2;
                    }
                    // If i is not even add a blank cell.
                    if (!((cell_count % 2) == 0))
                    {
                        table.AddCell("");
                        cell_count++;
                    }

                    PdfPCell header_cell = new PdfPCell(new Phrase(forms[i].HeaderTitle, new Font(helvetica, form_header_font_size, Font.BOLD)));
                    header_cell.Colspan = 2;
                    header_cell.HorizontalAlignment = 1;
                    table.AddCell(header_cell);
                    cell_count = cell_count + 2;
                    

                    // Timestamp
                    PdfPCell timestamp_cell = new PdfPCell(new Phrase("Date: " + forms[i].TimeStamp, new Font(helvetica, field_font_size)));
                    table.AddCell(timestamp_cell);
                    cell_count++;

                    for (j = 0; j < forms[i].FieldList.Count; j++)
                    {
                        PdfPCell field_cell = new PdfPCell(new Phrase(forms[i].FieldList[j].Label + forms[i].FieldList[j].Value, new Font(helvetica, field_font_size)));
                        table.AddCell(field_cell);
                        cell_count++;
                    }


                }


                doc.Add(table);

                doc.Close();

                // Copy over to the Shared Drive
                //File.Copy("@" + path, "@" + Properties.Settings.Default.SharedMappedOutput);

                String dest_path = Properties.Settings.Default.SharedMappedOutput;
                String test = Path.Combine(dest_path, Path.GetFileName(path));
                File.Copy(path, Path.Combine(@dest_path, @Path.GetFileName(path)));
                

            }
            catch (Exception e)
            {
                doc.Close();
                return e.Message;
            }
            return null;
        }

        public string parseMonthDay(string date)
        {
            int count = 0;
            string result = "";
            foreach (char s in date)
            {
                if (s == '/')
                {
                    count++;
                    if (count >= 2)
                    {
                        break;
                    }
                }
                result += s;
                
            }
            return result;
        }

        public string listToString(List<string> list)
        {
            string result = "[ ";

            for (int i = 0; i < list.Count; i++)
            {
                result += "'" + list[i] + "',"; 
            }

            result = result.Remove(result.Length - 1);

            result += " ]";

            return result;
        }

        public Highcharts generateSPC(List<VTSPCObject> list)
        {
            Highcharts chart = new Highcharts("chart");

            string uom = list[0].UOM;
            double upperspec = double.Parse(list[0].UpperSpec);
            double lowerspec = double.Parse(list[0].LowerSpec);

            // Set default max and min to be upper and lower specs.
            double max = upperspec;
            double min = lowerspec; 

            double uppercontrol = upperspec - 0.00005; // Currently static for demoing purposes
            double lowercontrol = lowerspec + 0.00005; // Currently static for demoing purposes
            string title = list[0].ValueName;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Value == "")
                {
                    break;
                }

                double curr_value = double.Parse(list[i].Value);
                if (curr_value > max)
                {
                    max = curr_value; 
                }
                if (curr_value < min)
                {
                    min = curr_value;
                }
            }

            chart.SetTitle(new Title { Text = "SPC Chart for " + title});
            
            chart.SetYAxis(new YAxis
            {
                Title = new XAxisTitle { Text = uom },
                Max = max,
                Min = min,
                PlotLines = new[]
                {
                    new XAxisPlotLines
                    {
                    Value = upperspec,
                    Color = System.Drawing.Color.Red,
                    DashStyle = DotNet.Highcharts.Enums.DashStyles.ShortDash,
                    Width = 2,
                    },
                    new XAxisPlotLines
                    {
                    Value = lowerspec,
                    Color = System.Drawing.Color.Red,
                    DashStyle = DotNet.Highcharts.Enums.DashStyles.ShortDash,
                    Width = 2
                    },
                    new XAxisPlotLines
                    {
                    Value = uppercontrol,
                    Color = System.Drawing.Color.Orange,
                    DashStyle = DotNet.Highcharts.Enums.DashStyles.ShortDash,
                    Width = 2,
                    },
                    new XAxisPlotLines
                    {
                    Value = lowercontrol,
                    Color = System.Drawing.Color.Orange,
                    DashStyle = DotNet.Highcharts.Enums.DashStyles.ShortDash,
                    Width = 2
                    }
                }
            });
            
            
            List<Object> data = new List<Object>();
            List<string> categories = new List<string>();
            List<string> dates = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Value == "")
                {
                    break;
                }

                string date_parse = parseMonthDay(list[i].Date);
                dates.Add(date_parse);
                string label = date_parse + " " + list[i].SerialNumber;
                categories.Add(label);
                
                data.Add(list[i].Value);
            }

            string dates_javascript_form = listToString(dates);
            chart.AddJavascripVariable("labels", dates_javascript_form);

            chart.SetXAxis(new XAxis
            {
                Categories = categories.ToArray(),
                TickInterval = 3,
                Labels = new XAxisLabels { Formatter = "function() { return labels[this.value];} " }

            });
            
            chart.SetSeries(new[]{
            new Series
            {
                Name = "Values",
                Data = new Data(data.ToArray())
            },
            new Series{
                Name = "Spec Limit",
                Color = System.Drawing.Color.Red,
            },
            new Series{
                Name = "Control Limit",
                Color = System.Drawing.Color.Orange,
            },
            
            });
            

            return chart;
        }
        
    }

}