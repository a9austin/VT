/**
 * VTViewFormModel
 * Author: Austin Truong
 * Date: 09/04/2012
 * Varian Medical Systems
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using VT.Classes;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;

namespace VT.Models
{
    /**
     *  Parent of all of the models for the forms.
     * */
    public class VTModel
    {
        [Required(ErrorMessage = "Please Insert Serial Number")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Please Insert Employee ID")]
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        public bool Success { get; set; }

        public string DisplayMessage { get; set; }

        public bool Go { get; set; }

        public string CompleteDate { get; set; }

    }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //
    //-------------------------------------------------------------- CHILD CLASSES ---------------------------------------------------------------------------------------------------------------//
    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ //

    public class VTViewModel : VTModel
    {

        public string HeaderToken { get; set; }

        public string HeaderTitle { get; set; }
 
        public List<VTFieldObject> ListFields { get; set; }

        public List<VTFieldObject> ListSPCOptions { get; set; }

        // For testing purposes
        public bool Submitted { get; set; }

        public List<string> ListSPCHTML { get; set; }

        public VTViewModel()
        {
            ListFields = new List<VTFieldObject>();
            ListSPCOptions = new List<VTFieldObject>();
            ListSPCHTML = new List<string>();
        }
    }

    public class VTReworkViewModel : VTModel
    {

        public string SelectedHeaderToken { get; set; }

        public List<VTFormObject> ListForms { get; set; }

        public List<VTFieldObject> ReworkListFields { get; set; }

        public bool Update { get; set; }

        public bool Rework { get; set; }

        public VTReworkViewModel()
        {
            ListForms = new List<VTFormObject>();
            ReworkListFields = new List<VTFieldObject>();
        }
    }

    public class VTBatchModel
    {

        [Required(ErrorMessage = "Please Insert Serial Number")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Display(Name = "Employee ID")]
        [DataType(DataType.Password)]
        public string EmployeeID { get; set; }

        [Display(Name = "Enter PIN")]
        public string PIN { get; set; }

        public string SelectedItem { get; set; }

        public string HeaderToken { get; set; }

        public string HeaderTitle { get; set; }

        public int BatchAmount { get; set; }

        public bool GoBatch { get; set; }

        public bool Success { get; set; }

        public string DisplayMessage { get; set; }

        public string DisplaySerialError { get; set; }

        public bool Go { get; set; }

        public string CompleteDate { get; set; }

        public List<VTFieldObject> ListBatchFields { get; set; }
        public List<BatchOption> Options { get; set; }
        public List<String> SerialNumbers { get; set; }

        public List<VTFieldObject> ListFields { get; set; }

        public VTBatchModel()
        {
            ListBatchFields = new List<VTFieldObject>();
            Options = new List<BatchOption>();
            ListFields = new List<VTFieldObject>();
            SerialNumbers = new List<string>();
        }
    }

    public class BatchOption
    {
        public String Text { get; set; }
        public String Value { get; set; }
    }

    public class VTSummaryModel
    {
        [Required(ErrorMessage = "Please Insert Serial Number")]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        public string DisplayMessage { get; set; }

        public bool Success { get; set; }
        public List<VTFormObject> ListForm { get; set; }

        public bool Update { get; set; }

        public string SwitchSubAssembly { get; set; } // This switch is for showing only tube info or marriage infos also.

        public string SwitchAllData { get; set; }

        public bool Go { get; set; }

        public string CompleteDate { get; set; }

        public VTSummaryModel()
        {
            ListForm = new List<VTFormObject>();
        }
    }

    public class VTOutputModel : VTModel
    {
        public List<VTFormObject> ListForm { get; set; }

        public bool CompletedWorkflow { get; set; }

        public bool Go { get; set; }

        public string OutputLocation { get; set; }

        public VTOutputModel()
        {
            ListForm = new List<VTFormObject>();
        }

    }

   
    public class NewSerialModel : VTModel
    {
        [Display(Name = "Work Station")]
        public string WorkStation { get; set; }

        [Display(Name = "Assembly Number")]
        public string AssemblyNumber { get; set; }

        [Display(Name = "Tube Type")]
        public string TubeType { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "PIN")]
        public string Pin { get; set; }

        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
        public SelectList SerialTypes { get; set; } // Remember to parse
        public string SelectedItem { get; set; }

        public NewSerialModel()
        {
            if (VT.Properties.Settings.Default.IncludeSNT)
            {
                var values = new[] {
                new { Value = "SNT", Text = "Tube Serial Number"},
                new { Value = "SNC", Text = "Cathode Serial Number"}
                };

                SerialTypes = new SelectList(values, "Value", "Text");
                WorkStation = "DEFAULT";
            }
            else
            {
                var values = new[] {
                new { Value = "SNC", Text = "Cathode Serial Number"}
                };

                SerialTypes = new SelectList(values, "Value", "Text");
                WorkStation = "DEFAULT";
            }
        }
    }

    public class VTPINModel
    {
        [Required(ErrorMessage="Please Insert Employee ID")]
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }
        [Required(ErrorMessage = "Please insert your desired PIN")]
        [Display(Name = "PIN")]
        public string Pin { get; set; }
        [Required(ErrorMessage = "Please reinsert your desired PIN")]
        [Display(Name = "Reenter PIN")]
        public string PinVerification { get; set; }
        public string DisplayMessage { get; set; }
        public bool Success { get; set; }

    }

    public class VTSPCModel
    {
        public string FinalAssembly { get; set; }
        public string Token { get; set; }
        public List<VTFormObject> ListForm { get; set; }
        public string DisplayMessage { get; set; }
        public Boolean Success { get; set; }
        public Boolean Go { get; set; }

        public VTSPCModel()
        {
            ListForm = new List<VTFormObject>();
        }
    }
}