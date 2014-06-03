using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Abstract;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using AjourBT.Domain.Infrastructure;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    [Authorize(Roles = "PU")]
    public class LocationController : Controller
    {
        private IRepository db;
        public LocationController(IRepository repository)
        {
            db = repository;
        }

        //
        // GET: /Location/

        public ViewResult Index()
        {
            return View(db.Locations.ToList());
        }

        public SelectList CountriesDropDownList()
        {
            var countryList = from rep in db.Countries
                              orderby rep.CountryName
                              select rep;

            return new SelectList(countryList, "CountryID", "CountryName");
        }

        //
        // GET: /Location/Create

        public ViewResult Create()
        {
            ViewBag.CountryList = CountriesDropDownList();
            return View();
        }

        //
        // POST: /Location/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Location location)
        {
            if (ModelState.IsValid)
            {                              
                bool isExistingID = IsExistingID(location.ResponsibleForLoc);
                if (isExistingID == false)
                {
                    return Json(new { error = "Not existing Employee's EID" });
                }
                db.SaveLocation(location);

                return RedirectToAction("PUView", "Home", new { tab = Tabs.PU.Locations });
            }

            ViewBag.CountryList = CountriesDropDownList();
            return View(location);
        }

        public bool IsExistingID(string responsibleForLoc)
        {
            if (responsibleForLoc == null || responsibleForLoc == String.Empty)
            { 
                return true; 
            }

            string[] ids = Regex.Split(responsibleForLoc, @"\W+"); 
            foreach (string id in ids)
            {
                if (id != "")
                {
                    var result = (from e in db.Employees where e.EID == id select e).FirstOrDefault();
                    if (result == null)
                        return false;
                }            
            }
            return true;
        }

        

        //
        // GET: /Location/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Location location = (from loc in db.Locations where loc.LocationID == id select loc).FirstOrDefault();
            ViewBag.CountryList = (from c in db.Countries select c).ToList();

            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        //
        // POST: /Location/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Location location)
        {
            ViewBag.CountryList = (from c in db.Countries select c).ToList();

            string ModelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    bool isExistingID = IsExistingID(location.ResponsibleForLoc);
                    if (isExistingID == false)
                    {
                        return Json(new { error = "Not existing Employee's EID" });
                    }
                    db.SaveLocation(location);
                    return RedirectToAction("PUView", "Home", new { tab = Tabs.PU.Locations });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelError = "The record you attempted to edit "
                                  + "was modified by another user after you got the original value. The "
                                  + "edit operation was canceled.";
            }
            //return View(location);
            return Json(new { error = ModelError });
        }

        // GET: /Location/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Location location = (from loc in db.Locations where loc.LocationID == id select loc).FirstOrDefault();
            if (location == null)
            {
                return HttpNotFound();
            }

            if (location.BusinessTrips.Count != 0)
            {
                return View("CannotDelete");

            }
            else
                return View(location);
        }

        //
        // POST: /Location/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                db.DeleteLocation(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");

            }

            return RedirectToAction("PUView", "Home", new { tab = Tabs.PU.Locations });
        }
    }
}