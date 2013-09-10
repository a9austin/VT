/**
 * VTController
 * Author: Austin Truong
 * Date: 09/04/2012
 * Varian Medical Systems
 **/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VT.Models;
using VT.Classes;
using System.IO;

namespace VT.Controllers
{
    public class VTController : Controller
    {
        private VTLogicModel logic;
        private VTHelper helper;


        public VTController()
        {
            logic = new VTLogicModel();
            helper = new VTHelper();
        }

        [HttpGet]
        public ActionResult NewSerial()
        {
            NewSerialModel model = new NewSerialModel();
    
            return View(model);

        }

        [HttpPost]
        public ActionResult NewSerial(NewSerialModel model)
        {

            if (ModelState.IsValid)
            {
                ModelState.Clear();

                string EmployeeID = "";
                if (VT.Properties.Settings.Default.UsingBinaryBadgeID)
                {
                    try
                    {
                        EmployeeID = helper.VTIDtoEMPID(model.EmployeeID);
                    }
                    catch
                    {
                        model.DisplayMessage = "Please Scan your VT ID";
                        return View(model);
                    }
                }
                else
                {
                    EmployeeID = model.EmployeeID;
                }

                List<string> list_data = new List<string>();
                list_data.Add(model.SerialNumber);
                list_data.Add(model.AssemblyNumber);
                list_data.Add("0");
                list_data.Add(model.WorkStation);
                list_data.Add(EmployeeID);
                

                int _switch = 0;
                if (model.SelectedItem == "SNT")
                {
                    _switch = 2;
                }
                else
                {
                    list_data.Add(model.Pin); // Only required PIN on cathode temporairly
                    _switch = 3;
                }

                string[] insert_check = logic.helperSubmit(list_data, _switch, model.SelectedItem); // Inserts into DB, takes in the list of values to submit.
                string error_number = insert_check[0];
                string error_message = insert_check[1];
                if (error_number == "0")
                {
                    var new_model = new NewSerialModel();
                    new_model.Success = true;
                    new_model.DisplayMessage = "New Serial: " + model.SerialNumber;
                    return View(new_model);
                }
                else
                {
                    model.DisplayMessage = helper.checkErrors(error_number, error_message);
                    return View(model);
                }
            }
            return View(model);
        }
        

        [HttpGet]
        public ActionResult MainForm()
        {
            var model = new VTViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult MainForm(VTViewModel model, string buttontype)
        {
            if (ModelState.IsValid)
            {
                string EmployeeID = "";

                // Determine to use VTID or EmployeeID, depdending on the properties settings.
                if (VT.Properties.Settings.Default.UsingBinaryBadgeID)
                {
                    try
                    {
                        EmployeeID = helper.VTIDtoEMPID(model.EmployeeID);
                    }
                    catch
                    {
                        model.DisplayMessage = "Please Scan your VT ID";
                        return View(model);
                    }
                }
                else
                {
                    EmployeeID = model.EmployeeID;
                }

                if (buttontype == "Go")
                {

                    // Time how long getForm takes
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // GET Fields depending on the serial number.
                    model.ListFields = logic.getFormInfo(model.SerialNumber, EmployeeID);

                    stopwatch.Stop();

                    // Write to a text file to catch the elapsed time.
                    DateTime today_date = new DateTime();
                    today_date = DateTime.Now;
                    using (StreamWriter writer = new StreamWriter("C:\\log\\timerlog_" + today_date.ToString("MM-dd-yyyy") + ".txt", true))
                    {
                        writer.WriteLine(DateTime.Now);
                        writer.WriteLine("Serial Number: " + model.SerialNumber + " | Process: " + model.ListFields[0].HeaderName + " | USING GETFORM");
                        writer.WriteLine("Badge ID: " + model.EmployeeID);
                        writer.WriteLine("Time Elapsed: " + stopwatch.Elapsed);
                        writer.WriteLine(" ");
                    }
                
                    if (model.ListFields[0].return_num == "0")
                    {
                        model.Go = true;

                        ViewData["HEADER"] = model.ListFields[0].HeaderName + " | " + model.ListFields[0].TubeType + " | " + model.ListFields[0].ProductLine;
                        model.DisplayMessage = model.ListFields[0].error_message;
                    }
                    else
                    {
                        if (model.ListFields[0].return_num == "0")
                        {
                            model.DisplayMessage = "Submitted";
                            
                        }
                        else
                        {
                            model.DisplayMessage = helper.checkErrors(model.ListFields[0].return_num, model.ListFields[0].error_message);
                        }
                    }
                }
                else if (buttontype == "Submit")
                {
                    // Variables
                    List<VTFieldObject> spc_options = new List<VTFieldObject>();
                    string header = model.ListFields[0].HeaderToken;
                    string[] check;                                                        

                    model.HeaderToken = model.ListFields[0].HeaderToken; 
                    // If the submission worked
                    //SUBMIT HERE INTO DB then Clear
                    List<string> values_to_submit = new List<string>(); // Creates a list to store the values to submit
                    for (int i = 0; i < model.ListFields.Count; i++)
                    {
                        // Fills in the hidden values.
                        if (model.ListFields[i].isHidden)
                        {
                            if (model.ListFields[i].Token == "SNT" || model.ListFields[i].Token == "SNC")
                            {
                                model.ListFields[i].Value = model.SerialNumber;
                            }
                            else if (model.ListFields[i].Token == "USR")
                            {
                                model.ListFields[i].Value = EmployeeID;
                            }
                            else if (model.ListFields[i].Token == "TMS")
                            {
                                model.ListFields[i].Value = "0";
                            }

                        }
                        // Have the radio list built
                        if (model.ListFields[i].DataType == "DATA-RADIO")
                        {
                            string radiobuttons = model.ListFields[i].DetailOptionsString;
                            model.ListFields[i].DetailOptions = helper.parseDetailOptions(radiobuttons);
                        }

                        // If it is a check box do the right conversion.
                        if (model.ListFields[i].DataType == "DATA-CKBOX")
                        {
                            string convertedValue = helper.boolToInt(model.ListFields[i].BoolValue).ToString();
                            model.ListFields[i].Value = convertedValue;
                        }

                        // If the field has a spec value add it to the list of SPC graph options
                        if (model.ListFields[i].ChildRelation == "R-VALUE")
                        {
                            if (model.ListFields[i].DataType == "DATA-DECIMAL")
                            {
                                spc_options.Add(model.ListFields[i]); // Add the option to the list
                            }
                        }

                        values_to_submit.Add(model.ListFields[i].Token + model.ListFields[i].Value);

                    }
                    // Makes sure it passes the correctly filled form test
                    string check_correct_form = helper.checkCorrectForm(model.ListFields);
                    if (check_correct_form != "0")
                    {
                        // If it doesn't pass set display message to the error
                        model.DisplayMessage = check_correct_form;

                        // Set go to true so it will display the form
                        model.Go = true;

                        return View(model);
                    }
                    check = logic.helperSubmit(values_to_submit, 1, header);
                    string error_number = check[0];
                    string error_message = check[1];
                    if (error_number == "0")
                    {
                        ModelState.Clear();
                        VTViewModel clear_model = new VTViewModel(); // Creates an empty model

                        if (VT.Properties.Settings.Default.ShowSPC)
                        {
                            // Call the database to get SPC charts
                            for (int i = 0; i < spc_options.Count; i++)
                            {
                                List<VTSPCObject> list = new List<VTSPCObject>();
                                list = logic.getSPCInfo(spc_options[i].FinalAssembly,
                                                                               spc_options[i].Token,
                                                                               spc_options[i].HeaderToken,
                                                                               "5");
                                string temp = helper.generateSPC(list).ToHtmlString();
                                // Change html so it will generate a unique graph per spec data
                                string gen_spc_html = temp.Replace("chart_container", "chart_container_" + i);

                                clear_model.ListSPCHTML.Add(gen_spc_html);
                            }
                        }
                        clear_model.Success = true;
                        clear_model.DisplayMessage = "Submitted\n" + model.SerialNumber + "\n" + DateTime.Now;
                        return View(clear_model);
                    }
                    else
                    {
                        model.DisplayMessage = helper.checkErrors(error_number, error_message); // Sets the display message to the error.
                    }

                }
                else if (buttontype == "Clear")
                {
                    // Clears the screen and model
                    ModelState.Clear();
                    VTViewModel clear_model = new VTViewModel(); // Creates an empty model
                    clear_model.Submitted = true;
                    return View(clear_model);
                }

            }
            return View(model);
            
        }

        [HttpGet]
        public ActionResult GetReworkForm()
        {
            var model = new VTReworkViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult GetReworkForm(VTReworkViewModel model, string buttontype)
        {
            if (ModelState.IsValid)
            {
                // Determine Which ID to use depending on the settings.
                string EmployeeID = "";
                if (VT.Properties.Settings.Default.UsingBinaryBadgeID)
                {
                    try
                    {
                        EmployeeID = helper.VTIDtoEMPID(model.EmployeeID);
                    }
                    catch
                    {
                        model.DisplayMessage = "Please Scan your VT ID";
                        return View(model);
                    }
                }
                else
                {
                    EmployeeID = model.EmployeeID;
                }

                if (buttontype == "Go")
                {
                    ModelState.Clear();
                    model.Go = true;

                    List<VTFormObject> list_HeaderForm = new List<VTFormObject>(); // [i] Title, [i+1] Token

                    list_HeaderForm = logic.getAllHeaders(model.SerialNumber, EmployeeID); // Gets all headers
                    
                    if (list_HeaderForm[0].return_num == "0")
                    {
                        // Loop through all of the headers and add it to the model header list
                        for (int i = 0; i < list_HeaderForm.Count; i++)
                        {
                            string headerToken = list_HeaderForm[i].HeaderToken;

                            model.ListForms.Add(list_HeaderForm[i]);
                        }
                    }
                    else
                    {
                        if (list_HeaderForm[0].return_num == "0")
                        {
                            model.DisplayMessage = "Submitted";
                        }
                        else
                        {
                            model.DisplayMessage = helper.checkErrors(list_HeaderForm[0].return_num, list_HeaderForm[0].error_message);
                        }
                    }
                    
                }
              
                else if (buttontype == "Rework")
                {
                    model.Rework = true;

                    model.ReworkListFields = logic.getFormInfo(model.SerialNumber, EmployeeID, model.SelectedHeaderToken);

                    if (!(model.ReworkListFields[0].return_num == "0"))
                    {
                        if (model.ReworkListFields[0].return_num == "0")
                        {
                            model.DisplayMessage = "Submitted";
                        }
                        else
                        {
                            model.DisplayMessage = helper.checkErrors(model.ReworkListFields[0].return_num, model.ReworkListFields[0].error_message);
                        }
                    }
                }

                else if (buttontype == "Update")
                {
                    model.Update = true;
                    string header = model.SelectedHeaderToken;
                    List<string> submit_list = new List<string>();
                    for (int i = 0; i < model.ReworkListFields.Count; i++)
                    {
                        // Covers the case if it is a update TMS, to update with a "0" instead of the previous TMS.
                        if (model.ReworkListFields[i].Token == "TMS")
                        {
                            model.ReworkListFields[i].Value = "0";
                        }

                        // If it is a check box do the right conversion.
                        if (model.ReworkListFields[i].DataType == "DATA-CKBOX")
                        {
                            string convertedValue = helper.boolToInt(model.ReworkListFields[i].BoolValue).ToString();
                            model.ReworkListFields[i].Value = convertedValue;
                        }

                        submit_list.Add(model.ReworkListFields[i].Token + model.ReworkListFields[i].Value); 
                    }

                    string[] check = logic.helperSubmit(submit_list, 1, header);
                    string error_number = check[0];
                    string error_message = check[1];

                    if (error_number == "0")
                    {
                        ModelState.Clear();
                        VTReworkViewModel clear_model = new VTReworkViewModel(); // Creates an empty model
                        clear_model.DisplayMessage = "Updated";

                        clear_model.Success = helper.intToBoolZeroIsTrue(Convert.ToInt32(error_number));
                        return View(clear_model);
                    }
                    else
                    {
                        model.DisplayMessage = helper.checkErrors(error_number, error_message); // Sets the display message to the error.
                    }
                }
                else if (buttontype == "Update")
                {
                    ModelState.Clear();
                    VTReworkViewModel clear_model = new VTReworkViewModel(); // Creates an empty model
                    //clear_model.DisplayMessage = "Updated";
                    return View(clear_model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult BatchForm()
        {
            VTBatchModel model = new VTBatchModel();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult BatchForm(VTBatchModel model, string buttontype)
        {
            if (ModelState.IsValid)
            {
                // Determine Which ID to use depending on the settings.
                string EmployeeID = "";
                if (VT.Properties.Settings.Default.UsingBinaryBadgeID)
                {
                    try
                    {
                        EmployeeID = helper.VTIDtoEMPID(model.EmployeeID);
                    }
                    catch
                    {
                        model.DisplayMessage = "Please Scan your VT ID";
                        return View(model);
                    }
                }
                else
                {
                    EmployeeID = model.EmployeeID;
                }

                if (buttontype == "Go")
                {
                    // Add the choices in the select down 
                    model.ListBatchFields = logic.getBatchInfo(model.SerialNumber);

                    // There was an error
                    if (model.ListBatchFields[0].return_num != "0")
                    {
                        model.DisplayMessage = model.ListBatchFields[0].error_message;
                        return View(model);
                    }

                    for (int i = 0; i < model.ListBatchFields.Count; i++)
                    {
                        BatchOption option = new BatchOption();
                        option.Text = model.ListBatchFields[i].Label;
                        option.Value = model.ListBatchFields[i].Token;
                        model.Options.Add(option);
                    }

                    if (model.BatchAmount != 0)
                    {
                        model.ListFields = logic.getFormInfo(model.SerialNumber, model.EmployeeID, model.SelectedItem); // It is only going to fill the first element
                        // There was an error
                        if (model.ListFields[0].return_num != "0")
                        {
                            model.DisplayMessage = model.ListFields[0].error_message;
                            return View(model);
                        }

                        // Add the first serial number scanned to the batch list
                        model.SerialNumbers.Add(model.SerialNumber);

                        for (int i = 0; i < model.BatchAmount-1; i++)
                        {
                            model.SerialNumbers.Add("");
                        }
                        model.GoBatch = true;
                    }
                    model.Go = true;
                }
                else if (buttontype == "Submit")
                {
                    string[] check;
                    List<String> submit_list = new List<String>();
                    // Fills in the hidden values.
                    for (int i = 0; i < model.ListFields.Count; i++)
                    {
                        if (model.ListFields[i].isHidden)
                        {
                            if (model.ListFields[i].Token == "SNT" || model.ListFields[i].Token == "SNC")
                            {
                                string serial_numbers = model.SerialNumber;
                                // Add all of the remaining serial numbers
                                for (int j = 0; j < model.SerialNumbers.Count; j++)
                                {
                                    serial_numbers += "," + model.SerialNumbers[j];
                                }
                                // Remove the extra comma at the end
                                model.ListFields[i].Value = serial_numbers;

                            }
                            else if (model.ListFields[i].Token == "USR")
                            {
                                model.ListFields[i].Value = EmployeeID;
                            }
                            else if (model.ListFields[i].Token == "TMS")
                            {
                                model.ListFields[i].Value = "0";
                            }
                        }
                        if (model.ListFields[i].DataType == "DATA-PIN")
                        {
                            model.ListFields[i].Value = model.PIN;
                        }
                        submit_list.Add(model.ListFields[i].Token + model.ListFields[i].Value);
                    }

                    check = logic.helperSubmit(submit_list, 4, model.SelectedItem);
                    string error_number = check[0];
                    string error_message = check[1];
                    if (error_number == "0")
                    {
                        ModelState.Clear();
                        VTBatchModel clear_model = new VTBatchModel(); // Creates an empty model
                        clear_model.Success = true;
                        clear_model.DisplayMessage = "Submitted\n";
                        for (int i = 0; i < model.SerialNumbers.Count; i++)
                        {
                            clear_model.DisplayMessage += model.SerialNumbers[i] + ",";
                        }
                        clear_model.DisplayMessage += "\n" + DateTime.Now;
                        clear_model.DisplaySerialError = error_message;
                        return View(clear_model);
                    }
                    else
                    {
                        model.DisplayMessage = helper.checkErrors(error_number, error_message); // Sets the display message to the error.
                    }

                }

            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Summary()
        {
            VTSummaryModel model = new VTSummaryModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Summary(VTSummaryModel model, string buttontype)
        {
            if (ModelState.IsValid)
            {

                if (buttontype == "Go")
                {
                    model.Go = true;

                    List<VTFormObject> list_HeaderForm = new List<VTFormObject>(); // [i] Title, [i+1] Token

                    list_HeaderForm = logic.getSummaryInfo(model.SerialNumber, model.SwitchAllData, model.SwitchSubAssembly);

                    if (!(list_HeaderForm[0].return_num == "0"))
                    {
                        model.DisplayMessage = helper.checkErrors(list_HeaderForm[0].return_num, list_HeaderForm[0].error_message);
                    }
                    model.ListForm = list_HeaderForm;
                }
                
                else if (buttontype == "Clear")
                {
                    // Clears the screen and model
                    ModelState.Clear();
                    VTSummaryModel clear_model = new VTSummaryModel(); // Creates an empty model
                    return View(clear_model);
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Output()
        {
            VTOutputModel model = new VTOutputModel();
            return View(model);
        }

        [HttpPost]
        public PartialViewResult Output(VTOutputModel model)
        {
            
            if (ModelState.IsValid)
            {
                // Determine Which ID to use depending on the settings.
                string EmployeeID = "";
                if (VT.Properties.Settings.Default.UsingBinaryBadgeID)
                {
                    try
                    {
                        EmployeeID = helper.VTIDtoEMPID(model.EmployeeID);
                    }
                    catch
                    {
                        model.DisplayMessage = "Please Scan your VT ID";
                        return PartialView(model);
                    }
                }
                else
                {
                    EmployeeID = model.EmployeeID;
                }
                
                List<VTFormObject> list_HeaderForm = new List<VTFormObject>(); // [i] Title, [i+1] Token

                list_HeaderForm = logic.getSummaryInfo(model.SerialNumber,"0", "1");

                if (list_HeaderForm[0].return_num == "0")
                {
                    model.Go = true;
                    int i = 0;

                    for (; i < list_HeaderForm.Count; i++)
                    {

                        if (list_HeaderForm[i].FieldList == null)
                        {
                            model.DisplayMessage = "Error, Tube has not been finished";
                            break;
                        }
                        
                        // Checks if the work flow is completed
                        if (i == list_HeaderForm.Count-1)
                        {
                            // Will need to switch back to correct employeeID variable later
                            if (logic.getFormInfo(model.SerialNumber, EmployeeID)[0].return_num == "51020") 
                            {
                                model.CompletedWorkflow = true;
                            }
                        }

                    }
                    model.ListForm = list_HeaderForm;
                    //string check = helper.BuildDocument(Properties.Settings.Default.FilePathOutput + model.SerialNumber + "_output.docx", model.SerialNumber, model.ListForm);

                    model.OutputLocation = Properties.Settings.Default.FilePathOutput;
                    String pdf_path = model.OutputLocation + model.SerialNumber + "_output.pdf";
                    string check = helper.iTextBuildDocument(model.SerialNumber, model.ListForm, pdf_path);
                   

                    model.DisplayMessage = check;
                    return PartialView("OutputPartialView", model);

                }
                else
                {
                    model.DisplayMessage = helper.checkErrors(list_HeaderForm[0].return_num, list_HeaderForm[0].error_message);
                    return PartialView(model);
                }
                
            }
            else
            {
                return PartialView(model);
            }

        }

        [HttpGet]
        public ActionResult PINPage()
        {
            VTPINModel model = new VTPINModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult PINPage(VTPINModel model)
        {
            if (ModelState.IsValid)
            {
                string[] check; // This array is for checking if the password submission passed [0] = code, [1] = msg
                // Determine Which ID to use depending on the settings.
                string EmployeeID = "";
                if (VT.Properties.Settings.Default.UsingBinaryBadgeID)
                {
                    try
                    {
                        EmployeeID = helper.VTIDtoEMPID(model.EmployeeID);
                    }
                    catch
                    {
                        model.DisplayMessage = "Please Scan your VT ID";
                        return View(model);
                    }
                }
                else
                {
                    EmployeeID = model.EmployeeID;
                }
                // Checks to see if the two passwords inputted match
                if (model.Pin != model.PinVerification)
                {
                    model.DisplayMessage = "Please ensure the passwords match";
                    return View(model);
                }
                List<string> ListFields = new List<String>();
                ListFields.Add("USR");
                ListFields.Add(EmployeeID);
                ListFields.Add(model.Pin);
                check = logic.helperSubmit(ListFields, 5);
                string error_number = check[0];
                string error_message = check[1];
                if (error_number == "0")
                {
                    ModelState.Clear();
                    VTPINModel clear_model = new VTPINModel(); // Creates an empty model
                    clear_model.DisplayMessage = "PIN has been set for " + EmployeeID;
                    clear_model.Success = true;
                    return View(clear_model);
                }
                else
                {
                    ModelState.Clear();
                    model.Success = false;
                    model.DisplayMessage = helper.checkErrors(error_number, error_message); // Sets the display message to the error.
                }

            }
            return View(model);
        }

        

    }
}
 