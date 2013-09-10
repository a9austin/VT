/**
 * VTMasterController
 * Author: Austin Truong
 * Date: 09/11/2012
 * Varian Medical Systems
 **/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VT.Classes;
using VT.Models;

namespace VT.Controllers
{
    public class VTMasterController : Controller
    {
        //
        // GET: /MasterIndex/

        public ActionResult MasterIndex()
        {
            return View();
        }

    }
}
