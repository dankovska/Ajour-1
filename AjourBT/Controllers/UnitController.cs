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

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    [Authorize(Roles = "PU")]
    public class UnitController : Controller
    {
        private IRepository db;
        public UnitController(IRepository repository)
        {
            this.db = repository;
        }

        //
        // GET: /Unit/

        public ViewResult Index()
        {
            return View(db.Units.ToList());
        }

        public ViewResult Create()
        {
            return View();
        }


        // POST: /Unit/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Unit unit)
        {
            if (ModelState.IsValid)
            {
                db.SaveUnit(unit);

                return RedirectToAction("PUView", "Home", new { tab = 5});
            }

            return View(unit);
        }

        //
        // GET: /Units/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Unit unit = (from un in db.Units where un.UnitID == id select un).FirstOrDefault();


            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        //
        // POST: /Unit/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Unit unit)
        {
            string ModelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    db.SaveUnit(unit);
                    return RedirectToAction("PUView", "Home", new { tab = 5 });
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

        //
        // GET: /Unit/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Unit unit = (from un in db.Units where un.UnitID == id select un).FirstOrDefault();
            if (unit == null)
            {
                return HttpNotFound();
            }

            if (unit.BusinessTrips.Count != 0)
            {
                return View("CannotDelete");

            }
            else
                return View(unit);
        }

        //
        // POST: /Unit/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                db.DeleteUnit(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");

            }

            return RedirectToAction("PUView", "Home", new { tab = 5 });
        }
    }
}