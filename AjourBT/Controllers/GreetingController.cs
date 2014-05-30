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
    public class GreetingController : Controller
    {
       //
        // GET: /Greeting/
        private IRepository repository;
        public GreetingController(IRepository repo)
        {

            this.repository = repo;
        }

      
        public ViewResult Index()
        {
            return View(repository.Greetings.ToList());
        }
        

        ////
        //// GET: /Greeting/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        ////
        //// POST: /Greeting/Create

        [HttpPost]
        public ActionResult Create(Greeting Greeting)
        {
            if (ModelState.IsValid)
            {
                repository.SaveGreeting(Greeting);
                return RedirectToAction("BDMView", "Home");
            }
            return View(Greeting);
        }

        ////
        //// GET: /Greeting/Edit/5
        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            Greeting Greeting = (from c in repository.Greetings where c.GreetingId == id select c).FirstOrDefault();
            if (Greeting == null)
            {
                return HttpNotFound();
            }
            return View(Greeting);
        }

        ////
        //// POST: /Greeting/Edit/5

        [HttpPost]
        public ActionResult Edit(Greeting Greeting)
        {
            string ModelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    repository.SaveGreeting(Greeting);
                    return RedirectToAction("BDMView", "Home");
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
        //// GET: /Greeting/Delete/5
        [HttpGet]
        public ActionResult Delete(int id = 0)
        {
            Greeting Greeting = (from c in repository.Greetings where c.GreetingId == id select c).FirstOrDefault();
            if (Greeting == null)
            {
                return HttpNotFound();
            }
                return View(Greeting);
        }

        ////
        //// POST: /Greeting/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                repository.DeleteGreeting(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            return RedirectToAction("BDMView", "Home");
        }
    }
}
