using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VT.Classes
{
    public class VTSPCObject
    {
        public string ProductLine { get; set; }
        public string ActivityName { get; set; }
        public string FinalAssembly { get; set; }
        public string TubeType { get; set; }
        public string SerialNumber { get; set; }
        public string Date { get; set; }
        public string ValueName { get; set; }
        public string Value { get; set; }
        public string UpperSpec { get; set; }
        public string LowerSpec { get; set; }
        public string TargetSpec { get; set; }
        public string UpperControl { get; set; }
        public string LowerControl { get; set; }
        public string UOM { get; set; }

        // Two properties are only if there is a failure.
        public string return_num { get; set; }
        public string error_message { get; set; }

    }
}