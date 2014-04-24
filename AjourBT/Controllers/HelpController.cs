using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    [AllowAnonymous]
    public class HelpController : Controller
    {
        //
        // GET: /Help/

        public ActionResult Map()
        {
            
            return View();
        }        
    }
}
