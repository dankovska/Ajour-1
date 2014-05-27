using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Models;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    [Authorize(Roles = "PU,VU")]
    public class EmployeeController : Controller
    {

        private IRepository db;
        public EmployeeController(IRepository repository)
        {
            db = repository;
        }

        public PartialViewResult GetEmployeeData(string selectedDepartment = null, string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";
            List<EmployeeViewModel> data = SearchEmployeeData(db.Users.ToList(), selectedDepartment, searchString);
            //IEnumerable<EmployeeViewModel> data = from emp in db.Users.AsEnumerable()
            //                                      join dep in db.Departments on emp.DepartmentID equals dep.DepartmentID
            //                                      join pos in db.Positions on emp.PositionID equals pos.PositionID
            //                                      where selectedDepartment == null || selectedDepartment == String.Empty || dep.DepartmentName == selectedDepartment
            //                                      orderby emp.IsManager descending, emp.LastName
            //                                      select new EmployeeViewModel(emp);

            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            return PartialView(data);
        }

        public ActionResult GetEmployee(string selectedDepartment = null, string searchString = "")
        {
            var departmentList = from dep in db.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return View();
        }

        public ActionResult GetCalendarVU(string selectedDepartmentVU = null)
        {
            var departmentList = from dep in db.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartmentVU;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return View();
        }

        public List<EmployeeViewModel> SearchEmployeeData(List<Employee> empList, string selectedDepartment, string searchString)
        {
            List<EmployeeViewModel> data = new List<EmployeeViewModel>();
            data = (from emp in empList
                    where ((selectedDepartment == null || selectedDepartment == String.Empty || (emp.Department !=null && emp.Department.DepartmentName == selectedDepartment))
                            && (emp.EID.ToLower().Contains(searchString.ToLower())
                            || emp.FirstName.ToLower().Contains(searchString.ToLower())
                            || emp.LastName.ToLower().Contains(searchString.ToLower())
                            || ((emp.DateEmployed != null) && emp.DateEmployed.Value != null && emp.DateEmployed.Value.ToShortDateString().Contains(searchString))
                            || ((emp.DateDismissed != null) && emp.DateDismissed.Value != null && emp.DateDismissed.Value.ToString().Contains(searchString))
                            || ((emp.BirthDay != null) && emp.BirthDay.Value != null && emp.BirthDay.Value.ToString().Contains(searchString))
                            || ((emp.FullNameUk != null) && emp.FullNameUk.ToLower().Contains(searchString.ToLower()))
                            || ((emp.Position != null) && emp.Position.TitleEn.ToLower().Contains(searchString.ToLower()))
                            ||
                                  ((System.Web.Security.Membership.GetUser(emp.EID) != null)
                                  && System.Web.Security.Roles.GetRolesForUser(emp.EID) !=null && String.Join(", ", System.Web.Security.Roles.GetRolesForUser(emp.EID)).ToLower().Contains(searchString.ToLower()))))
                    orderby emp.IsManager descending, emp.LastName
                    select new EmployeeViewModel(emp)).ToList();

            return data;

        }

        public SelectList DepartmentsDropDownList()
        {
            var depL = from rep in db.Departments
                       orderby rep.DepartmentName
                       select rep;

            return new SelectList(depL, "DepartmentID", "DepartmentName");
        }

        public SelectList PositionsDropDownList()
        {
            var posList = from pos in db.Positions
                          orderby pos.TitleEn
                          select pos;

            return new SelectList(posList, "PositionID", "TitleEn");
        }


        public SelectList DropDownListWithSelectedDepartment(string selectedDepartment)
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            foreach (Department department in db.Departments)
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = department.DepartmentName,
                    Value = department.DepartmentID.ToString(),
                    Selected = department.DepartmentName.ToString() == selectedDepartment ? true : false

                };

                selectListItems.Add(selectListItem);
            }

            var allDepartments = from rep in selectListItems.AsEnumerable().OrderBy(m => m.Text)
                                 orderby rep.Selected == true descending
                                 select rep;

            var id = from rep in selectListItems
                     where rep.Selected == true
                     select rep.Value;

            return new SelectList(allDepartments, "Value", "Text", id);
        }

        // Get
        public ViewResult Create(string selectedDepartment = null, string searchString = "")
        {

            ViewBag.PositionsList = PositionsDropDownList();
            ViewBag.DepartmentsList = DropDownListWithSelectedDepartment(selectedDepartment);
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View();
        }

        //
        // POST: /Employee/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee emp, string[] Roles = null, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            if (ModelState.IsValid)
            {
                db.SaveEmployee(emp);
                db.SaveRolesForEmployee(emp.EID, Roles);
                return RedirectToAction("PUView", "Home", new { tab = 1, selectedDepartment = selectedDepartment, SearchString = searchString });
            }

            EmployeeViewModel employee = new EmployeeViewModel(emp);

            ViewBag.DepartmentsList = DepartmentsDropDownList();
            ViewBag.PositionsList = PositionsDropDownList();
            return View(employee);
        }


        //
        // GET: /Employee/Edit/5

        public ActionResult Edit(int id = 0, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.DepartmentList = (from d in db.Departments select d).ToList();
            ViewBag.PositionList = (from p in db.Positions select p).ToList();
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            Employee emp = db.Users.FirstOrDefault(e => e.EmployeeID == id);

            if (emp == null)
            {
                return HttpNotFound();
            }

            EmployeeViewModel employee = new EmployeeViewModel(emp);

            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(employee);
        }

        //
        // POST: /Employee/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee emp, string[] Roles = null, string selectedDepartment = null, string searchString = "")
        {
            ViewBag.DepartmentList = (from d in db.Departments select d).ToList();
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.PositionList = (from p in db.Positions select p).ToList();
            ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;

            string ModelError = "";

            try
            {
                if (ModelState.IsValid)
                {
                    db.SaveEmployee(emp);
                    db.SaveRolesForEmployee(emp.EID, Roles);
                    List<Employee> empl = db.Users.ToList();
                    List<EmployeeViewModel> empList = SearchEmployeeData(empl, selectedDepartment, searchString);
                    return View("OneRowPU", empList);
                    //return RedirectToAction("PUView", "Home", new { tab = 1, selectedDepartment = selectedDepartment, SearchString = searchString });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelError = "The record you attempted to edit "
                                    + "was modified by another user after you got the original value. The "
                                    + "edit operation was canceled.";
            }
            //EmployeeViewModel employee = new EmployeeViewModel(emp);
            //return View(employee);
            return Json(new { error = ModelError });
        }

        //
        // GET: /Employee/Delete/5

        public ActionResult Delete(int id = 0, string selectedDepartment = null, string searchString = "")
        {
            Employee employee = db.Users.FirstOrDefault(e => e.EmployeeID == id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            if ( (employee.BusinessTrips!= null && employee.BusinessTrips.Count != 0) || employee.Visa != null || employee.Permit != null || employee.VisaRegistrationDate != null || employee.Passport != null)
            {
                ViewBag.SelectedDepartment = selectedDepartment;
                ViewBag.SearchString = searchString;
                ViewBag.EmployeeID = id;
                return View("CannotDelete");
            }
            else
                ViewBag.SelectedDepartment = selectedDepartment;
            ViewBag.SearchString = searchString;
            return View(employee);
        }

        //
        // POST: /Employee/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string selectedDepartment = null, string searchString = "")
        {
            try
            {
                db.DeleteUser(db.Users.FirstOrDefault(e => e.EmployeeID == id).EID);
                db.DeleteEmployee(id);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return RedirectToAction("DataBaseDeleteError", "Home");
            }

            List<Employee> empl = db.Users.ToList();
            List<EmployeeViewModel> empList = SearchEmployeeData(empl, selectedDepartment, searchString);
            return View("OneRowPU", empList);

            //return RedirectToAction("PUView", "Home", new { tab = 1, selectedDepartment = selectedDepartment, SearchString = searchString });
        }

        [HttpGet]
        public ViewResult ResetPassword(string EID, string[] Roles)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string ResetPasswordConfirmed(string EID, string[] Roles)
        {
            db.SaveRolesForEmployee(EID, null);
            db.SaveRolesForEmployee(EID, Roles);
            return "Password has been changed";
        }
    }
}