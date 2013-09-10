/**
 * VTFormObject
 * Author: Austin Truong
 * Date: 09/04/2012
 * Varian Medical Systems
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace VT.Classes
{
    public class VTFormObject
    {
        public string Username { get; set; }
        public string SerialNumber { get; set; }
        public string TubeType { get; set; }
        public string ProductLine { get; set; }
        public string HeaderToken { get; set; }
        public string HeaderTitle { get; set; }
        public string HasActivity { get; set; }
        public string Status { get; set; }
        public string return_num { get; set; }
        public string error_message { get; set; }
        public List<VTFieldObject> FieldList { get; set; }
        //public List<List<VTFieldObject>> LayoutFieldList { get; set; }
        public VTFormObject(){
            FieldList = new List<VTFieldObject>();
        }
    }
}