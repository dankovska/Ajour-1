
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class CountryController : Controller
    {
        //
        // GET: /Country/
        private IRepository repository;
        public CountryController(IRepository repo)
        {

            this.repository = repo;
        }

      
        public ViewResult Index()
        {
            return View(repository.Countries.ToList());
        }
        

        ////
        //// GET: /Country/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        ////
        //// POST: /Country/Create

        [HttpPost]
        public ActionResult Create(Country country)
        {
            if (ModelState.IsValid)
            {
                repository.SaveCountry(country);
                return RedirectToAction("ABMView", "Home");
            }
            return View(country);
        }

        ////
        //// GET: /Country/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            Country country = (from c in repository.Countries where c.CountryID == id select c).FirstOrDefault();
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        ////
        //// POST: /Country/Edit/5

        [HttpPost]
        public ActionResult Edit(Country country)
        {
            string ModelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    repository.SaveCountry(country);
                    return RedirectToAction("ABMView", "Home");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelError = "The record you attempted to edit "
                                     + "was modified by another user after you got the original value. The "
                                     + "edit operation was canceled.";
            }

            return Json(new { error = ModelError });
        }

        ////
        //// GET: /Country/Delete/5
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            Country country = (from c in repository.Countries where c.CountryID == id select c).FirstOrDefault();
            if (country == null)
            {
                return HttpNotFound();
            }

            if (country.Locations.Count != 0 || country.Holidays.Count != 0)
            {
                return View("CannotDelete");

            }
            else
                return View(country);
        }

        ////
        //// POST: /Country/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                repository.DeleteCountry(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            return RedirectToAction("ABMView", "Home");
        }
    }
}
