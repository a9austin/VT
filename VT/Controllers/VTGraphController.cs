/**
 * VTGraphController
 * Author: Austin Truong
 * Date: 05/08/2013
 * Varian Medical Systems
 **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VT.Models;
using VT.Classes;
using DotNet.Highcharts;

namespace VT.Controllers
{
    public class VTGraphController : Controller
    {

        private VTLogicModel logic;
        private VTHelper helper;

        public VTGraphController()
        {
            logic = new VTLogicModel();
            helper = new VTHelper();
        }
        
        [HttpGet]
        public ActionResult GraphView()
        {
            VTGraphModel model = new VTGraphModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult GraphView(VTGraphModel model)
        {
            List<VTSPCObject> list = logic.getSPCInfo(model.FinalAssembly, model.DataToken, model.HeaderToken, model.Amount);
            Highcharts chart = helper.generateSPC(list);
            model.Go = true;
            model.ChartHTML = chart.ToHtmlString();

            return View(model);
        }

    }
}
