/** TEST!!
 * VTLogicModel
 * Author: Austin rTuong
 * Date: 09/04/2012
 * Varian Medical Systems
 **/

using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Odbc;
using System.Data;
using System.Collections;
using System.IO;

namespace VT.Classes
{
    public class VTLogicModel
    {
        private string is_connect;
        private OdbcConnection conn;
        private VTHelper helper;

        public VTLogicModel()
        {
            helper = new VTHelper();
        }

        /**
         * Method : Connect 
         * Function : Connects to the database
         * Paramaters : None
         * Return : void
         **/
        private void connect()
        {
            try
            {
                is_connect = VT.Properties.Settings.Default.DBAddress; // Property of which database server to conncet to
                conn = new OdbcConnection(is_connect);
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection Error: '{0}'", e);
            }
        }

        /**
         * Method : getHeaderInfo
         * Function : Goes into the database, looks for the SN given and the type, finds the correct value to return. Will return the an array where array[0] = return
         * and array[1] = message
         * string formatted. Will call a database prompt.
         * Paramaters : (sn - The Unique Serial Number) (header - header you are looking in)
         * Return : An array with the return and message
         * */
        private string[] getHeaderInfo(string sn, string header)
        {
            string str_cmd = "exec dbo.usp_getActivity '" + header + "','" + sn + "' ";
            string[] arr = helperReadArrCmd(str_cmd);
            return arr;
        }

        /**
        * Method : helperReadArrCmd
        * Function : Helper method which reads the return/messsage values given from the cmd ran.
        * Paramater : (str_cmd - the string command of the execution call)
        * Return : Array of the return message of the proc
        * */
        private string[] helperReadArrCmd(string str_cmd)
        {
            string[] arr = new string[2];
            string _return = "";
            string _msg = "";
            connect();
            OdbcCommand cmd = new OdbcCommand(str_cmd, conn);
            OdbcDataReader reader;

            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                _return = reader["return"].ToString().Trim();
                _msg = reader["message"].ToString().Trim();
                break;
            }
            arr[0] = _return;
            arr[1] = _msg;
            reader.Close();
            cmd.Dispose();
            conn.Close();
            return arr;
        }

        /**
         * Method : parseTMS
         * Function : Parses a given time code into the correct format.
         * Paramater : (time_code - string time code)
         * Return : The time and date
         * */
        public string parseTMS(string time_code)
        {
            connect();
            string time = "";
            string str_cmd = "select dbo.uf_parseTMS('" + time_code + "')";
            OdbcCommand cmd = new OdbcCommand(str_cmd, conn);
            OdbcDataReader reader;
            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                time = reader[0].ToString().Trim();
            }
            reader.Close();
            cmd.Dispose();
            conn.Close();
            return time;
        }

        /**
         * Method : helperReadCmdSetField
         * Function : Reads the command. Sets the Field Object with the correct data from the database.
         * Paramaters : string cmd, the cmd in a string format
         * Return : A List filled with FieldObjects
         * */
        private List<VTFieldObject> helperReadCmdSetField(string str_cmd)
        {
            connect();

            List<VTFieldObject> field_obj_list = new List<VTFieldObject>();

            OdbcCommand cmd = new OdbcCommand(str_cmd, conn);
            OdbcDataReader reader;
           
            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                VTFieldObject field_obj = new VTFieldObject();
                string _return = reader["return"].ToString().Trim();
                string _msg = reader["message"].ToString().Trim();

                // If return is 0, this mean success
                if (_return == "0")
                {

                    // Getting all the database calls and setting the model properties

                    field_obj.HeaderToken = reader["Header_Type"].ToString().Trim();

                    field_obj.HeaderName = reader["Header_Name"].ToString().Trim();

                    field_obj.Token = reader["Detail_Token"].ToString().Trim();

                    //// Covers TMS case to convert
                    if (field_obj.Token == "TMS")
                    {
                        string temp_value = reader["Detail_Data"].ToString().Trim();
                        if (temp_value != null)
                        {
                            field_obj.Value = parseTMS(temp_value);
                        }
                    }
                    else
                    {
                        field_obj.Value = reader["Detail_Data"].ToString().Trim();
                    }


                    field_obj.Spec = reader["Default_Spec"].ToString().Trim();

                    field_obj.ProductLine = reader["Product_Line"].ToString().Trim();

                    field_obj.TubeType = reader["Tube_Type"].ToString().Trim();

                    field_obj.DataType = reader["Detail_Data_Type"].ToString().Trim();

                    field_obj.Username = reader["Username"].ToString().Trim();

                    field_obj.DetailOptionsString = reader["Detail_Options"].ToString().Trim();

                    field_obj.FinalAssembly = reader["Final_Assembly"].ToString().Trim();

                    field_obj.ChildRelation = reader["Child_Relation"].ToString().Trim();

                    field_obj.DetailOptions = helper.parseDetailOptions(reader["Detail_Options"].ToString().Trim());

                    bool isHidden = helper.intToBool(Int32.Parse(reader["IsHidden"].ToString().Trim()));
                    field_obj.isHidden = isHidden;

                    bool isDefault = helper.intToBool(Int32.Parse(reader["IsDefault"].ToString().Trim()));
                    field_obj.Default = isDefault;

                    if (isDefault)
                    {
                        field_obj.Label = reader["Default_Label"].ToString().Trim() + reader["Default_Spec"].ToString().Trim() + " " + field_obj.Value;

                    }
                    else
                    {
                        field_obj.Label = reader["Default_Label"].ToString().Trim() + reader["Default_Spec"].ToString().Trim() + " "; 

                    }

                    field_obj.return_num = _return;
                    field_obj.error_message = _msg;

                }
                // If doesn't pass, set the error message and return number
                else
                {
                    field_obj.return_num = _return;
                    field_obj.error_message = _msg;
                }
                field_obj_list.Add(field_obj);
            
            }

            conn.Close();
            cmd.Dispose();
            reader.Close();
            return field_obj_list;
        }


        /**
         * Method : SetLayoutList
         * Function : Reads the command. Sets the Field with the correct data from the database.
         * Paramat  ers : string cmd, the cmd in a string format
         * Return : A List filled with FieldObjects
         * */
        private List<List<VTFieldObject>> SetLayoutList(string str_cmd)
        {
            connect();

            List<List<VTFieldObject>> field_obj_list = new List<List<VTFieldObject>>();

            OdbcCommand cmd = new OdbcCommand(str_cmd, conn);
            OdbcDataReader reader;

            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                VTFieldObject field_obj = new VTFieldObject();
                string _return = reader["rtn"].ToString().Trim();
                string _msg = reader["msg"].ToString().Trim();

                if (_return == "0")
                {

                    field_obj.HeaderToken = reader["Header_Type"].ToString().Trim();

                    field_obj.HeaderName = reader["Header_Name"].ToString().Trim();

                    field_obj.Token = reader["Detail_Token"].ToString().Trim();

                    field_obj.Label = reader["Default_Label"].ToString().Trim() + reader["Default_Spec"].ToString().Trim() + " " + reader["Detail_Data"].ToString().Trim();

                    field_obj.Spec = reader["Default_Spec"].ToString().Trim();

                    field_obj.ProductLine = reader["Product_Line"].ToString().Trim();

                    field_obj.TubeType = reader["Tube_Type"].ToString().Trim();

                    field_obj.DataType = reader["Detail_Data_Type"].ToString().Trim();

                    field_obj.Username = reader["Username"].ToString().Trim();

                    field_obj.Value = reader["Detail_Data"].ToString().Trim();

                    field_obj.DetailOptionsString = reader["Detail_Options"].ToString().Trim();

                    field_obj.DetailOptions = helper.parseDetailOptions(reader["Detail_Options"].ToString().Trim());

                    bool isHidden = helper.intToBool(Int32.Parse(reader["IsHidden"].ToString().Trim()));
                    field_obj.isHidden = isHidden;

                    bool isDefault = helper.intToBool(Int32.Parse(reader["IsDefault"].ToString().Trim()));
                    field_obj.Default = isDefault;

                    field_obj.return_num = _return;

                }
                // If doesn't pass, set the error message and return number
                else
                {
                    field_obj.return_num = _return;
                    field_obj.error_message = _msg;
                }

                int row = Int32.Parse(reader["row"].ToString().Trim()); 
                int col = Int32.Parse(reader["col"].ToString().Trim());

                helper.expandList(field_obj_list, row, col);
                field_obj_list[row][col] = field_obj;

            }

            conn.Close();
            cmd.Dispose();
            reader.Close();
            return field_obj_list;
        }


        private List<VTFormObject> helperSummary(string str_cmd)
        {
            connect();

            List<VTFormObject> list_VTFormObject = new List<VTFormObject>();
                    
            OdbcCommand cmd = new OdbcCommand(str_cmd, conn);
            OdbcDataReader reader;
            VTFormObject formObject = null;
            string header_name = string.Empty;

            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                string _return = reader["return"].ToString().Trim();
                string _msg = reader["message"].ToString().Trim();
                // If the query passes
                if (_return == "0")
                {
                    // Check if it is the first call
                    if (header_name != reader["Header_Name"].ToString().Trim())
                    {
                        if (!(header_name == ""))
                        {
                            list_VTFormObject.Add(formObject);
                        }
 
                        header_name = reader["Header_Name"].ToString().Trim();
                        formObject = new VTFormObject();
                        formObject.HeaderTitle = header_name;
                        formObject.SerialNumber = reader["Serial_Number"].ToString().Trim();
                        formObject.ProductLine = reader["Product_Line"].ToString().Trim();
                        formObject.TubeType = reader["Tube_Type"].ToString().Trim();
                        formObject.TimeStamp = reader["Header_Date"].ToString().Trim();
                        formObject.return_num = _return;
                    }

                    VTFieldObject fieldObject = new VTFieldObject();
                    fieldObject.HeaderName = header_name;
                    fieldObject.Token = reader["Detail_Token"].ToString().Trim();
                    fieldObject.Label = reader["Default_Label"].ToString().Trim() + ": ";
                    fieldObject.Spec = reader["Default_Spec"].ToString().Trim();
                    fieldObject.Value = reader["Detail_Data"].ToString().Trim();
                    // Add the object to the formObjects list of fieldObjects
                    formObject.FieldList.Add(fieldObject);
                    
                }

                // Cover the error case
                else
                {
                    formObject = new VTFormObject();
                    formObject.error_message = _msg;
                    formObject.return_num = _return;
                    list_VTFormObject.Add(formObject);
                    return list_VTFormObject; // Returns, due to an error case
                }
            }
            list_VTFormObject.Add(formObject); // Adds the last header
            return list_VTFormObject;
        }


        /**
         * Method : helperReadCmdAllHeaders
         * Fuction : Reads the cmd, parses all of the header titles and header token into a list.
         * Paramaters : string cmd to the db
         * Return : A list filled with tokens and titles of the forms.@
         * */
        private List<VTFormObject> helperReadCmdAllHeaders(string str_cmd)
        {
            connect();

            List<VTFormObject> list_VTFormObject = new List<VTFormObject>();

            OdbcCommand cmd = new OdbcCommand(str_cmd, conn);
            OdbcDataReader reader;


            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (reader.Read())
            {
                string _return = reader["return"].ToString().Trim();
                string _msg = reader["message"].ToString().Trim();
                VTFormObject formObject = new VTFormObject();
                if (_return == "0")
                {
                    // Sets the form object properties
                    formObject.HeaderTitle = reader["Obj_Name"].ToString().Trim();
                    formObject.HeaderToken = reader["Obj_Token"].ToString().Trim();
                    formObject.Username = reader["Username"].ToString().Trim();
                    formObject.TubeType = reader["Tube_Type"].ToString().Trim();
                    formObject.ProductLine = reader["Product_Line"].ToString().Trim();
                    formObject.HasActivity = reader["hasActivity"].ToString().Trim();
                    formObject.Status = reader["Status"].ToString().Trim();
                    if (formObject.Status == "0")
                    {
                        formObject.Status = "1";
                    }
                    formObject.return_num = _return;
                }
                else
                {
                    formObject.error_message = _msg;
                    formObject.return_num = _return;
                }
                list_VTFormObject.Add(formObject);
            }

            return list_VTFormObject;   
        }

        private List<VTFieldObject> helperGetBatch(string str_cmd)
        {
            connect();

            List<VTFieldObject> list = new List<VTFieldObject>();

            OdbcCommand cmd = new OdbcCommand(str_cmd, conn);
            OdbcDataReader reader;

            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                VTFieldBatchObject field = new VTFieldBatchObject();
                field.return_num = reader["return"].ToString().Trim();
                field.error_message = reader["message"].ToString().Trim();

                if (field.return_num == "0")
                {
                    field.Token = reader["Obj_Token"].ToString().Trim();
                    field.Label = reader["Obj_Name"].ToString().Trim();
                }
                list.Add(field);
            }
            return list;
        }

        private List<VTSPCObject> helperGetSPC(string str_cmd)
        {
            connect();

            List<VTSPCObject> list = new List<VTSPCObject>();

            OdbcCommand cmd = new OdbcCommand(str_cmd, conn);
            OdbcDataReader reader;

            reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while (reader.Read())
            {
                VTSPCObject obj = new VTSPCObject();
                obj.return_num = reader["return"].ToString().Trim();
                obj.error_message = reader["message"].ToString().Trim();

                if (obj.return_num == "0")
                {
                    obj.Date = reader["Test_Date"].ToString().Trim();
                    obj.ValueName = reader["Value_Name"].ToString().Trim();
                    obj.Value = reader["Test_Value"].ToString().Trim();
                    obj.UpperSpec = reader["Upper_Spec"].ToString().Trim();
                    obj.LowerSpec = reader["Lower_Spec"].ToString().Trim();
                    obj.TargetSpec = reader["Target_Spec"].ToString().Trim();
                    obj.UpperControl = reader["Upper_Ctrl"].ToString().Trim();
                    obj.LowerControl = reader["Lower_Ctrl"].ToString().Trim();
                    obj.SerialNumber = reader["Serial_Number"].ToString().Trim();
                    obj.UOM = reader["UOM_Type"].ToString().Trim();
                }
                list.Add(obj);
            }

            return list;
        }

        /**
         * Method : getFormInfo
         * Function : With the given serial number, will call a helper method which will parse and place all the information into a list of FieldObjects.
         * Paramaters : string serial number | optional rework, if its a rework or not
         * Return : A List filled with FieldObjects
         * */
        public List<VTFieldObject> getFormInfo(string sn, string employeeID, string header = "")
        {
            // Time how long getForm takes

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            string str_cmd = "";
            if (header == "" || header == null)
            {
                str_cmd = "exec dbo.usp_getForm '" + sn + "','" + employeeID + "' "; 
            }
            else
            {
                str_cmd = "exec dbo.usp_getForm '" + sn + "','" + employeeID + "','" + header + "' ";
            }

            List<VTFieldObject> list_field_objects = helperReadCmdSetField(str_cmd);
            

            return list_field_objects; 
        }

        /**
         * Method : getFormInfo_LayoutOrder
         * Function : With the given serial number, will call a helper method which will parse and place all the information into a list of FieldObjects.
         * Paramaters : string serial number | optional rework, if its a rework or not
         * Return : A List filled with FieldObjects
         * */
        public List<List<VTFieldObject>> getFormInfo_LayoutOrder(string sn, string employeeID, string header = "")
        {
            string str_cmd = "";
            if (header == "")
            {
                str_cmd = "exec dbo.usp_getForm '" + sn + "','" + employeeID + "' ";
            }
            else
            {
                str_cmd = "exec dbo.usp_getForm '" + sn + "','" + employeeID + "','" + header + "' ";
            }

            List<List<VTFieldObject>> list_field_objects = SetLayoutList(str_cmd);
            return list_field_objects;
        }

        /**
         * Method : getAllHeaders
         * Function : With the given serial number, calls a helper to get all of the form titles and the token
         * Paramaters : string serial number, string employee id
         * Return : A list filled with string form titles and tokens | [i] title, [i+1] header token
         * */
        public List<VTFormObject> getAllHeaders(string sn, string employeeID)
        {
            string str_cmd = ""; 
            str_cmd = "exec dbo.usp_getHeader '" + sn + "','" + employeeID + "'";
            List<VTFormObject> list_form_titles_tokens = new List<VTFormObject>();
            list_form_titles_tokens = helperReadCmdAllHeaders(str_cmd);
            return list_form_titles_tokens;
        }

        /**
         * Method : getSummaryInfo
         * Function : Goes into the database, looks for the SN given and the type, finds the correct value to return. Will return the an array where array[0] = return
         * and array[1] = message
         * string formatted. Will call a database prompt.
         * Paramaters : (sn - The Unique Serial Number) (flag - 0 for showing only tube sn information, 1 for showing tube sn info and all marriage info)
         * Return : An array with the return and message
         * */
        public List<VTFormObject> getSummaryInfo(string sn, string flag_data, string flag_sub)
        {
            string str_cmd = ""; 
            str_cmd = "exec dbo.usp_getSummary '" + sn + "','" + flag_data + "','" + flag_sub + "'";
            List<VTFormObject> list_form_titles_tokens = new List<VTFormObject>();
            list_form_titles_tokens = helperSummary(str_cmd);
            return list_form_titles_tokens;
        }

        /**
         * Method : getBatchInfo
         * Function : Queries to the database the current batch information
         * Parameters : (sn - Serial Number)
         * Return : A VTFieldBatchObject List with the batch information
         * */
        public List<VTFieldObject> getBatchInfo(string sn)
        {
            string str_cmd = "";
            str_cmd = "exec dbo.usp_getBatchActivity '" + sn + "'";
            List<VTFieldObject> fields = new List<VTFieldObject>();
            fields = helperGetBatch(str_cmd);
            return fields;
        }

        /**
         * Method : getSPCInfo
         * Function : Queries to the database to get the SPC information regarding the activity token given
         * Parameters : (final_assembly - final assembly number) (activity_token - actiity token)
         * Return : A list of objects with the information regarding the SPC graph
         * */
        public List<VTSPCObject> getSPCInfo(string final_assembly, string activity_token, string header_token, string count)
        {
            string str_cmd = "exec dbo.usp_getSPCActivityTopX '" + header_token + "','" + final_assembly + "','" + activity_token + "','" + count + "'";
            List<VTSPCObject> objects = new List<VTSPCObject>();
            objects = helperGetSPC(str_cmd);
            return objects;
        }

        /**
         * Method : insertTODB
         * Function : Calls a database prompt that inserts the value into the correct type
         * Paramaters : (str - String to be inserted)
         * example of paramater "|HRO|TMS1337643302|SNT34046-Q2|USR955424|TRO0.00030000|BRO0.00220000|RRO0.00950000"
         * Return : string[] - first element is the return, second element is the message.
         * */
        private string[] insertToDB(string str)
        {
            string str_cmd = "exec dbo.usp_newActivity '" + str + "'";

            string[] return_val = helperReadArrCmd(str_cmd);

            return return_val;
        }

        /**
         * Method : insertBatch
         * Function : Calls a database prompt and inserts/checks off a batch amount of serial numbers.
         * Paramaters : (str - string prompt command to insert)
         * Return : string[] - first value in the array is the return, second value is the message.
         * */
        private string[] insertBatch(string str)
        {
            string str_cmd = "exec dbo.usp_newBatchActivity '" + str + "'";

            string[] return_val = helperReadArrCmd(str_cmd);

            return return_val;
        }

        /**
         * Method : insertSerialTODB
         * Funciton : Calls a database prompt and creates a new serial number in the database.
         * Paramaters : (str - the string of info to db to be inserted)
         * Return : string[] - first value in the array is the return, second value is the message.
         * */
        private string[] insertSerialTODB(string str)
        {
            string str_cmd = "exec dbo.usp_newSerialActivity " + str;
            string[] return_val = helperReadArrCmd(str_cmd);

            return return_val;
        }

        /**
         * Method : insertCathodeSerialTODB
         * Funciton : Calls a database promprt and creates a new serial number in the database.
         * Paramaters : (str - the string of info to db to be inserted)
         * Return : string[] - first value in the array is the return, second value is the message.
         * */
        private string[] insertCathodeSerialTODB(string str)
        {
            string str_cmd = "exec dbo.usp_newSerialActivity2 " + str;
            string[] return_val = helperReadArrCmd(str_cmd);

            return return_val;
        }

        private string[] setPINTODB(string str)
        {
            string str_cmd = "exec dbo.usp_setUserPwd" + str;
            string[] return_val = helperReadArrCmd(str_cmd);
            return return_val;
        }

        /**
        * Method : helperSubmit
        * Function : A helper method which formats the string and inserts into the database.
        * Paramaters : (data - a list filled with all of data to be inserted) (header - the header of which part to deal with) (int - 1:insertTODB the normal insert 2:insertSerialTODB 3:insertSerialCathode)
        * Return : string[], string[0] the code, [1] the message
        * */
        public string[] helperSubmit(List<string> data, int _switch, string header="")
        {
            // Formats into correct format
            string input = "";
            string check_submit = "";
            string check_message = "";
            string[] check = null;
            // Inserts into Database
            switch (_switch)
            {
                case 1: // NORMAL VALUE INSERTION
                    input = helper.formatStringPipe(header, data);
                    check = insertToDB(input);
                    break;
                case 2: // GENERAL TUBE S/N INSERTION
                    input = helper.formatStringComma(header, data);
                    check = insertSerialTODB(input);
                    break;
                case 3: // CATHODE/CHILD S/N INSERTION
                    input = helper.formatStringComma(header, data);
                    check = insertCathodeSerialTODB(input);
                    break;
                case 4: // BATCH INSERTION
                    input = helper.formatStringPipe(header, data);
                    check = insertBatch(input);
                    break;
                case 5: // SET PASSWORD
                    input = helper.formatStringComma(header, data);
                    check = setPINTODB(input);
                    break;
            }
            check_submit = check[0];
            check_message = check[1];
            return check;
        }


    }
 
}