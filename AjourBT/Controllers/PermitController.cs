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
using AjourBT.Models;
using System.Data.Entity.Infrastructure;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    [Authorize(Roles = "ADM,BTM")]
    public class PermitController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;

        string ModelError = "The record you attempted to edit "
                             + "was modified by another user after you got the original value. The "
                             + "edit operation was canceled.";

        public PermitController(IRepository repo, IMessenger messenger)
        {
            repository = repo;
            this.messenger = messenger;
        }
      
        //
        // GET: /Permit/Create

        public ActionResult Create(int id = 0,string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            Employee employee = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();
            if (employee == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
                ViewBag.EmployeeID = id;
            }

            return View();
        }

        //
        // POST: /Permit/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Permit permit, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;     
     
            if (ModelState.IsValid)
            {
                try 
                {
                    repository.SavePermit(permit, permit.EmployeeID);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = ModelError });
                }

                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            PermitViewModel permitModel = new PermitViewModel(permit);
            return View(permitModel);
        }

        //
        // GET: /Permit/Edit/5

        public ActionResult Edit(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Permit permit = (from p in repository.Permits where p.EmployeeID == id select p).FirstOrDefault();

            if (permit == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = permit.PermitOf.LastName + " " + permit.PermitOf.FirstName + " (" + permit.PermitOf.EID + ") from " + permit.PermitOf.Department.DepartmentName;
                ViewBag.EmployeeID = id;
                PermitViewModel permitModel = new PermitViewModel(permit);

                return View(permitModel);
            }
        }

        //
        // POST: /Permit/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Permit permit,string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
     
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SavePermit(permit, permit.EmployeeID);
                }
                catch(DbUpdateConcurrencyException)
                {
                    return Json(new { error = ModelError });
                }

                Employee emp = repository.Employees.Where(e => e.EmployeeID == permit.EmployeeID).FirstOrDefault();
                if (permit.CancelRequestDate != null)
                {
                    Employee author = repository.Employees.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                    Employee employeeForPermit = repository.Employees.Where(p => p.EmployeeID == permit.EmployeeID).FirstOrDefault();
                    messenger.Notify(new Message(MessageType.BTMCancelsPermitToADM, null, author, employeeForPermit));
                }
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
                //return RedirectToAction("BTMView", "Home", new { searchString = searchString });
            }
            
            PermitViewModel permitModel = new PermitViewModel(permit);
            return View(permitModel);
        }

        //
        // GET: /Permit/Delete/5

        public ActionResult Delete(int id = 0, string searchString = "")
        {
            Permit permit = (from p in repository.Permits where p.EmployeeID == id select p).FirstOrDefault();
            ViewBag.SearchString = searchString;

            if (permit == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeID = id;
            return View(permit);
        }

        //
        // POST: /Permit/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id,string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;

            repository.DeletePermit(id);
            Employee emp = repository.Employees.Where(e => e.EmployeeID == id).FirstOrDefault();
            List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
            return View("TableViewVisasAndPermitsBTM", empList);
            //return RedirectToAction("BTMView", "Home", new { searchString = searchString });
        }

        public List<Employee> GetEmployeeData(List<Employee> empList, string searchString)
        {
            List<Employee> selected = (from emp in empList
                                       where emp.EID.ToLower().Contains(searchString.ToLower())
                                            || emp.FirstName.ToLower().Contains(searchString.ToLower())
                                            || emp.LastName.ToLower().Contains(searchString.ToLower())
                                 || (emp.Visa != null)
                                                 && (emp.Visa.VisaType.ToLower().Contains(searchString.ToLower())
                                      || emp.Visa.StartDate.ToString().Contains(searchString)
                                      || emp.Visa.DueDate.ToString().Contains(searchString)
                                                 || emp.Visa.Entries == 0 && searchString.ToLower().Contains("mult"))
                                 || (emp.VisaRegistrationDate != null
                                      && emp.VisaRegistrationDate.RegistrationDate.ToString().Contains(searchString))
                                 || (emp.Permit != null)
                                      && (emp.Permit.StartDate.ToString().Contains(searchString)
                                      || emp.Permit.EndDate.ToString().Contains(searchString))

                                       orderby emp.IsManager descending, emp.DateDismissed, emp.LastName
                                       select emp).ToList();
            return selected;
        }
    }
}