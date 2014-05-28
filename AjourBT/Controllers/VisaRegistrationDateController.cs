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
    [Authorize(Roles = "BTM")]
    public class VisaRegistrationDateController : Controller
    {
        private IRepository repository;
        private IMessenger messenger;

        public VisaRegistrationDateController(IRepository repo, IMessenger messenger)
        {
            repository = repo;
            this.messenger = messenger;
        }

        string ModelError = "The record you attempted to edit "
                             + "was modified by another user after you got the original value. The "
                             + "edit operation was canceled.";

        //
        // GET: /VisaRegistrationDate/Create

        public ActionResult Create(int id = 0, string searchString = "")
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
        // POST: /VisaRegistrationDate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VisaRegistrationDate visaRegDate, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = ModelError });
                }
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);

                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                Employee empWithVisa = (from emp in repository.Employees where emp.EmployeeID == visaRegDate.EmployeeID select emp).FirstOrDefault();
                messenger.Notify(new Message(MessageType.BTMCreateVisaRegistrationDateToEMP, null, author, empWithVisa));

                return View("TableViewVisasAndPermitsBTM", empList);
            }

            RegistrationDateViewModel visaRegistrationDate = new RegistrationDateViewModel(visaRegDate);

            return View(visaRegistrationDate);
        }

        //
        // GET: /VisaRegistrationDate/Edit/5
        public ActionResult Edit(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            VisaRegistrationDate visaRegDate = (from v in repository.VisaRegistrationDates where v.EmployeeID == id select v).FirstOrDefault();

            if (visaRegDate == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = visaRegDate.VisaRegistrationDateOf.LastName + " " + visaRegDate.VisaRegistrationDateOf.FirstName + " (" + visaRegDate.VisaRegistrationDateOf.EID + ") from " + visaRegDate.VisaRegistrationDateOf.Department.DepartmentName;
                ViewBag.EmployeeID = id;

                RegistrationDateViewModel visaRegistrationDate = new RegistrationDateViewModel(visaRegDate);

                return View(visaRegistrationDate);
            }
        }

        //
        // POST: /VisaRegistrationDate/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VisaRegistrationDate visaRegDate, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = ModelError });
                }

                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);

                Employee author = repository.Users.Where(e => e.EID == HttpContext.User.Identity.Name).FirstOrDefault();
                Employee empWithVisa = (from emp in repository.Employees where emp.EmployeeID == visaRegDate.EmployeeID select emp).FirstOrDefault();
                messenger.Notify(new Message(MessageType.BTMUpdateVisaRegistrationDateToEMP, null, author, empWithVisa));

                return View("TableViewVisasAndPermitsBTM", empList);
            }

            RegistrationDateViewModel visaRegistrationDate = new RegistrationDateViewModel(visaRegDate);
            ViewBag.SearchString = searchString;
            return View(visaRegistrationDate);
        }

        //
        // POST: /VisaRegistrationDate/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string searchString = "")
        {
            VisaRegistrationDate visaRegistrationDate = (from v in repository.VisaRegistrationDates where v.EmployeeID == id select v).FirstOrDefault();
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            if (visaRegistrationDate == null)
            {
                return HttpNotFound();
            }
            else
            {
                repository.DeleteVisaRegistrationDate(id);
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

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