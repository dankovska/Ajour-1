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
    [Authorize(Roles = "PU")]
    public class PositionController : Controller
    {
        private IRepository repository;
        public PositionController(IRepository repo)
        {

            this.repository = repo;
        }

        // GET: /Position/

        public ViewResult Index()
        {
            return View(repository.Positions.ToList());
        }

        //
        // GET: /Position/Create

        public ViewResult Create()
        {
            return View();
        }

        //
        // POST: /Position/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Position position)
        {
            if (ModelState.IsValid)
            {
                repository.SavePosition(position);

                return RedirectToAction("PUView", "Home", new { tab = 3 });
            }

            return View(position);
        }

        
         //GET: /Position/Edit/

        public ActionResult Edit(int id = 0)
        {
            Position position = (from p in repository.Positions where p.PositionID == id select p).FirstOrDefault();
            if (position == null)
            {
                return HttpNotFound();
            }
            return View(position);
        }

        //
        // POST: /Location/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Position position)
        {
            string ModelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    repository.SavePosition(position);
                    return RedirectToAction("PUView", "Home", new { tab = 3 });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelError = "The record you attempted to edit "
                                  + "was modified by another user after you got the original value. The "
                                  + "edit operation was canceled.";
            }

            return Json(new { error = ModelError });
            //return View(position);
        }

        //
        // GET: /Position/Delete/

        public ActionResult Delete(int id = 0)      
        {
            Position position = (from p in repository.Positions where p.PositionID == id select p).FirstOrDefault();
            if (position == null)
            {
                return HttpNotFound();
            }

            if (position.Employees.Count != 0)
            {
                return View("CannotDelete");
            }
            else
                return View(position);
        }

        //
        // POST: /Location/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                repository.DeletePosition(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            return RedirectToAction("PUView", "Home", new { tab = 3 });
        }

    }
}
