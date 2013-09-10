/**
 * VTGraphModel
 * Author: Austin Truong
 * Date: 05/08/2013
 * Varian Medical Systems
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VT.Classes;
using System.ComponentModel.DataAnnotations;

namespace VT.Models
{
    public class VTGraphModel
    {
        [Display(Name = "Amount to show")]
        public string Amount { get; set; }

        // This property is temporairy for testing purposes.
        [Display(Name = "Header Token")]
        public string HeaderToken { get; set; }

        // This property is temporairy for testing purposes.
        [Display(Name = "Data Token")]
        public string DataToken { get; set; }

        // This property is temporairy for testing purposes.
        [Display(Name = "Final Assembly")]
        public string FinalAssembly { get; set; }

        public string ChartHTML { get; set; }

        public bool Go { get; set; }

        public List<VTSPCObject> SPCFieldList { get; set; }

        public VTGraphModel()
        {
            SPCFieldList = new List<VTSPCObject>();
        }
    }
}