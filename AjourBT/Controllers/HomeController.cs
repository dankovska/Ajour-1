using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    public class HomeController : Controller
    {
        [Authorize(Roles="DIR")]
        public ActionResult DIRView(int tab = 0)
        {
            return View(tab);
        }
        
        [Authorize(Roles = "PU")]
        public ActionResult PUView(int tab = 0, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(tab);
        }

        [Authorize(Roles = "BTM")]
        public ActionResult BTMView(int tab = 0, string searchString="")
        {
            //ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(tab);
        }

        [Authorize(Roles = "ADM")]
        public ActionResult ADMView(int tab = 0, string selectedDepartment=null)
        {
            ViewBag.SelectedDepartment = selectedDepartment; 
            return View(tab);
        }

        [Authorize(Roles = "ACC")]
        public ActionResult ACCView(int tab = 0, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(tab);
        }

        [Authorize(Roles = "VU")]
        public ActionResult VUView(int tab = 0)
        {
            return View(tab);
        }

        [Authorize(Roles = "EMP")]
        public ActionResult EMPView(int tab = 0)
        {
            return View(tab);
        }

        [Authorize(Roles = "ABM")]
        public ActionResult ABMView(int tab = 0)
        {
            return View(tab);
        }

        public ActionResult DataBaseDeleteError()
        {
            return View();
        }

    }
}