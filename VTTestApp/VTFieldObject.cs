/**
 * VTFieldObject
 * Author: Austin Truong
 * Date: 09/04/2012
 * Varian Medical Systems
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VT.Classes
{
    public class VTFieldObject
    {
        public string Username { get; set; }
        public string HeaderToken { get; set; }
        public string HeaderName { get; set; }
        public string Token { get; set; }
        public string Label { get; set; }
        public bool Default { get; set; }
        public bool isHidden { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
        public bool BoolValue { get; set; }
        public string Spec { get; set; }
        public string TubeType { get; set; }
        public string ProductLine { get; set; }
        public string isNew { get; set; }  // Returns a 1 or 0, 1 for new, 0 for not.

        // Radio Button Properties
        public string DetailOptionsString { get; set; }
        public List<String> DetailOptions { get; set; }


        // Two properties are only if there is a failure.
        public string return_num { get; set; }
        public string error_message { get; set; }

        public VTFieldObject()
        {
            DetailOptions = new List<String>();
        }

    }
}