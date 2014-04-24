using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Domain.Concrete;
using System.Data.Entity.Infrastructure;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    [Authorize(Roles = "PU")]
    public class DepartmentController : Controller
    {
        private IRepository repository;
         
        public DepartmentController(IRepository repo)
        {
            
            this.repository = repo;
        }

        //
        // GET: /Department/

        public ViewResult Index()
        {
            return View(repository.Departments);
        }

        //
        // GET: /Department/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Department/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            if (ModelState.IsValid)
            {
                repository.SaveDepartment(department);
                return RedirectToAction("PUView", "Home");
            }

            return View(department);
        }

        //
        // GET: /Department/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Department department = repository.Departments.FirstOrDefault(d => d.DepartmentID == id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // POST: /Department/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Department department)
        {
            string ModelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    repository.SaveDepartment(department);
                    return RedirectToAction("PUView", "Home");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelError =          "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";
            }

            return Json(new { error = ModelError });
        }

        //
        // GET: /Department/Delete/5
        public ActionResult Delete(int id = 0)
        {
            Department department = repository.Departments.Where(d => d.DepartmentID == id).FirstOrDefault();

            if (department == null)
            {
                return HttpNotFound();
            }

            if (department.Employees.Count != 0)
            {
                return View("CannotDelete");
            }
            else
                return View(department);
        }

        //
        // POST: /Department/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                repository.DeleteDepartment(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            return RedirectToAction("PUView", "Home");
        }
    }
}