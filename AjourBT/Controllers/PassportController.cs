using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    public class PassportController : Controller
    {

        private IRepository repository;
        string ModelError = "The record you attempted to edit "
                             + "was modified by another user after you got the original value. The "
                             + "edit operation was canceled.";


        public PassportController(IRepository repo)
        {
            repository = repo;
        }

        [HttpPost]
        public ActionResult ModifyPassport(string id, string isChecked,string searchString="")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            int employeeID;
            bool AddPassport;
            AddPassport = isChecked != null ? true : false;

            Int32.TryParse(id, out employeeID);
            if (employeeID != 0)
            {
                Employee emp = (from e in repository.Employees where e.EmployeeID == employeeID select e).FirstOrDefault();
                if (emp == null)
                {
                    return HttpNotFound();
                }

                if (AddPassport)
                {
                    //if (emp.Passport == null)
                    //{
                        try
                        {
                            repository.SavePassport(new Passport { EmployeeID = employeeID });
                        }
                        catch (System.InvalidOperationException)
                        {
                            return Json(new { error = ModelError });
                        }
                    //}
                }       
                else
                {
                    if (emp.Passport != null)
                        repository.DeletePassport(employeeID);
                }

                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            return  HttpNotFound();
        }

        //
        // GET: /Passport/AddDate

        public ActionResult AddDate(int id = 0, string searchString = "")
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
            }

            Passport passport = repository.Passports.Where(p => p.EmployeeID == id).FirstOrDefault();
            PassportViewModel model = new PassportViewModel(passport);
            return View(model);
        }

        //
        // POST: /Passport/AddDate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDate(Passport passport, string searchString = "")
        {
            if (passport.EndDate == null)
            {
                ModelState.AddModelError("error", "error");
            }
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SavePassport(passport);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = ModelError });
                }
                Employee emp = repository.Employees.Where(e => e.EmployeeID == passport.EmployeeID).FirstOrDefault();
                ViewBag.SearchString = searchString;
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(),searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
                // return RedirectToAction("BTMView", "Home", new { searchString = searchString });
            }

            PassportViewModel passportModel = new PassportViewModel(passport);
            ViewBag.SearchString = searchString;
            return View(passportModel);
        }

        // GET: /Passport/Edit/5

        public ActionResult EditDate(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Passport passport = (from p in repository.Passports where p.EmployeeID == id select p).FirstOrDefault();

            if (passport == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = passport.PassportOf.LastName + " " + passport.PassportOf.FirstName + " (" + passport.PassportOf.EID + ") from " + passport.PassportOf.Department.DepartmentName;

                PassportViewModel passportModel = new PassportViewModel(passport);

                return View(passportModel);
            }
        }

        //
        // POST: /Passport/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDate(Passport passport, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SavePassport(passport);
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = ModelError });
                }
                List<Employee> empList = GetEmployeeData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
                //return RedirectToAction("BTMView", "Home", new { searchString = searchString });
            }

            PassportViewModel passportModel = new PassportViewModel(passport);
            ViewBag.SearchString = searchString;
            return View(passportModel);
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
