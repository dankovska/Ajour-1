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
    public class PrivateTripController : Controller
    {
        private IRepository repository;

        public PrivateTripController(IRepository repo)
        {
            repository = repo;
        }

        //
        // GET: /PrivateTrip/

        [Authorize(Roles = "BTM")]
        public PartialViewResult GetPrivateTripDataBTM(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<Employee> selected = repository.Employees.ToList(); ;


            selected = SearchPrivateTripData(selected, searchString);

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            return PartialView(selected);
        }

        [Authorize(Roles = "VU")]
        public ViewResult GetPrivateTripDataVU(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<Employee> selected = repository.Employees.ToList(); ;

            selected = SearchPrivateTripData(selected, searchString);
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            return View(selected.ToList());
        }

        [Authorize(Roles = "BTM")]
        public ActionResult GetPrivateTripBTM(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }

        [Authorize(Roles = "VU")]
        public ActionResult GetPrivateTripVU(string searchString = "")
        {
            return View((object)searchString);
        }

        public List<Employee> SearchPrivateTripData(List<Employee> empList, string searchString)
        {
            List<Employee> selected = (from emp in empList
                                      where emp.DateDismissed == null
                                           && (emp.EID.ToLower().Contains(searchString.ToLower())
                                           || emp.FirstName.ToLower().Contains(searchString.ToLower())
                                           || emp.LastName.ToLower().Contains(searchString.ToLower())) 
                                      orderby emp.IsManager descending, emp.LastName
                                      select emp).ToList();
            return selected;

        }

        // GET: /PrivateTrip/Create
        [Authorize(Roles = "BTM")]
        public ActionResult Create(int id = 0, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            Employee employee = (from e in repository.Employees.AsEnumerable() where e.EmployeeID == id select e).FirstOrDefault();

            if (employee == null || employee.Visa == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
                ViewBag.SearchString = searchString;
            }
            return View();
        }

        //
        // POST: /PrivateTrip/Create
        [Authorize(Roles = "BTM")]
        [HttpPost]
        public ActionResult Create(PrivateTrip PTrip, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            PrivateTrip privateTrip = PTrip;

            if (ModelState.IsValid)
            {
                Visa visa = repository.Visas.Where(v => v.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();
                if (visa != null)
                {
                    visa.DaysUsedInPrivateTrips += CountingDaysUsedInPT(privateTrip);
                    visa.EntriesUsedInPrivateTrips++;

                    repository.SaveVisa(visa, visa.EmployeeID);
                }
                repository.SavePrivateTrip(privateTrip);
                List<Employee> emplist = SearchPrivateTripData(repository.Employees.ToList(), searchString);
                return View("TableViewPTBTM", emplist);
            }

            PrivateTripViewModel pTripModel = new PrivateTripViewModel(privateTrip);

            return View(pTripModel);
        }

        //
        // GET: /PrivateTrip/Edit/5
        [Authorize(Roles = "BTM")]
        public ActionResult Edit(int id, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            PrivateTrip pTrip = repository.PrivateTrips.Where(p => p.PrivateTripID == id).FirstOrDefault();

            if (pTrip == null)
            {
                return HttpNotFound();
            }
            else
            {
                Employee employee = repository.Employees.Where(e => e.EmployeeID == pTrip.EmployeeID).FirstOrDefault();
                ViewBag.EmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
            }

            PrivateTripViewModel privateTripModel = new PrivateTripViewModel(pTrip);
            return View(privateTripModel);
        }

        //POST: /PrivateTrip/Edit/5
        [Authorize(Roles = "BTM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PrivateTrip privateTrip, string searchString = null)
        {

            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            string ModelError = "";
            try
            {
                if (ModelState.IsValid)
                {
                    Visa visa = repository.Visas.Where(v => v.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();
                    if (visa != null)
                    {
                        if (visa.DaysUsedInPrivateTrips != null)
                        {
                            int oldDaysUsedInBT = visa.DaysUsedInPrivateTrips.Value;
                            visa.DaysUsedInPrivateTrips -= oldDaysUsedInBT;
                        }
                        visa.DaysUsedInPrivateTrips += CountingDaysUsedInPT(privateTrip);

                        repository.SaveVisa(visa, visa.EmployeeID);
                    }

                    repository.SavePrivateTrip(privateTrip);
                    List<Employee> emplist = SearchPrivateTripData(repository.Employees.ToList(), searchString);
                    return View("TableViewPTBTM", emplist);
                }
            }
            catch(DbUpdateConcurrencyException)
            {
                ModelError = "The record you attempted to edit "
                                    + "was modified by another user after you got the original value. The "
                                    + "edit operation was canceled.";
            }
            //PrivateTripViewModel privateTripModel = new PrivateTripViewModel(privateTrip);
            //return View(privateTripModel);
            return Json(new { error = ModelError });
        }

        // GET: /PrivateTrip/Delete/5
        [Authorize(Roles = "BTM")]
        public ActionResult Delete(int id, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            PrivateTrip privateTrip = (from pt in repository.PrivateTrips where pt.PrivateTripID == id select pt).FirstOrDefault();

            if (privateTrip == null)
            {
                return HttpNotFound();
            }

            Employee employee = (from e in repository.Employees.AsEnumerable() where e.EmployeeID == privateTrip.EmployeeID select e).FirstOrDefault();

            if (employee == null)
            {
                return HttpNotFound();
            }

            ViewBag.EmployeeInformation = "Delete Private trip of " + employee.LastName + " " + employee.FirstName;

            return View(privateTrip);
        }

        //
        // POST: /PrivateTrip/Delete/5
        [Authorize(Roles = "BTM")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string searchString = "")
        {
            PrivateTrip privateTrip = (from pt in repository.PrivateTrips where pt.PrivateTripID == id select pt).FirstOrDefault();
            if (privateTrip != null)
            {
                Visa visa = (from v in repository.Visas where v.EmployeeID == privateTrip.EmployeeID select v).FirstOrDefault();
                if (visa != null)
                {
                    visa.DaysUsedInPrivateTrips -= CountingDaysUsedInPT(privateTrip);
                    visa.EntriesUsedInPrivateTrips--;
                    repository.SaveVisa(visa, visa.EmployeeID);
                }

                repository.DeletePrivateTrip(id);
            }
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            List<Employee> emplist = SearchPrivateTripData(repository.Employees.ToList(), searchString);
            return View("TableViewPTBTM", emplist);
        }

        [Authorize(Roles = "BTM")]
        public int CountingDaysUsedInPT(PrivateTrip privateTrip)
        {
            //'+1' day for counting last day too
            return ((privateTrip.EndDate - privateTrip.StartDate).Days + 1);
        }

    }
}
