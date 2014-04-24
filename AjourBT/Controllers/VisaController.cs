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
using ExcelLibrary.SpreadSheet;
using System.IO;
using AjourBT.Domain.Infrastructure;
using AjourBT.Helpers;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    [Authorize(Roles = "ADM, BTM, VU")]
    public class VisaController : Controller
    {
        private IRepository repository;

        public VisaController(IRepository repo)
        {
            repository = repo;
        }

        string ModelError = "The record you attempted to edit "
                                  + "was modified by another user after you got the original value. The "
                                  + "edit operation was canceled.";


        [Authorize(Roles = "ADM")]
        public PartialViewResult GetVisaDataADM(string departmentName = "", string userName = "", string selectedUserDepartment = "")
        {
            string selectedDepartment = departmentName;

            var selected = from emp in repository.Employees
                           join dep in repository.Departments on emp.DepartmentID equals dep.DepartmentID
                           where (((emp.Department.DepartmentName == selectedUserDepartment || (selectedDepartment == String.Empty && userName == String.Empty))
                                   || (dep.DepartmentName == selectedDepartment))
                                   && (emp.DateDismissed == null))
                           orderby emp.IsManager descending, emp.LastName
                           select emp;

            return PartialView(selected.ToList());
        }

        [Authorize(Roles = "ADM")]
        public ViewResult GetVisaADM(string userName = "")
        {

            var selectedUserDepartment = (from e in repository.Employees
                                          where e.EID == userName
                                          select e.Department.DepartmentName).FirstOrDefault();

            var allDepartments = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.SelectedDepartment = new SelectList(allDepartments, "DepartmentName", "DepartmentName", selectedUserDepartment);
            return View((object)selectedUserDepartment);
        }

        [Authorize(Roles = "BTM")]
        public ActionResult GetVisaBTM(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }

        [Authorize(Roles = "BTM")]
        public PartialViewResult GetVisaDataBTM(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";

            List<Employee> selected = repository.Employees.ToList(); ;
            //if (searchString != "")
            //{
            //    string[] searchWords = searchString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
            //    for (int i = 0; i < searchWords.Length; i++)
            //    {
            //        selected = SearchVisaData(selected, searchWords[i]);

            //    }
            //}
            //else
            //{
            selected = SearchVisaData(selected, searchString);
            //}

            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            return PartialView(selected);

        }

        public List<Employee> SearchVisaData(List<Employee> empList, string searchString)
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

        [Authorize(Roles = "VU")]
        public ActionResult GetVisaVU(string searchString = "")
        {
            ViewBag.SearchString = searchString;
            return View();
        }
        [Authorize(Roles = "VU")]
        public PartialViewResult GetVisaDataVU(string searchString = "")
        {
            searchString = searchString != "" ? searchString.Trim() : "";

            List<Employee> selected = repository.Employees.ToList();
            selected = SearchVisaData(selected, searchString);
            ViewBag.SearchString = searchString;
            return PartialView(selected);

        }



        // GET: /Visa/Create
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Visa visa, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveVisa(visa, visa.EmployeeID);
                }
                catch (System.InvalidOperationException)
                {
                    return Json(new { error = ModelError });
                }

                List<Employee> empList = SearchVisaData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            VisaViewModel visaModel = new VisaViewModel(visa);
            return View(visaModel);
        }

        //
        // GET: /Visa/Edit/5
        public ActionResult Edit(int id = 0, string searchString = "")
        {
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;

            Visa visa = (from v in repository.Visas where v.EmployeeID == id select v).FirstOrDefault();

            if (visa == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.EmployeeInformation = visa.VisaOf.LastName + " " + visa.VisaOf.FirstName + " (" + visa.VisaOf.EID + ") from " + visa.VisaOf.Department.DepartmentName;
                ViewBag.EmployeeID = id;
                VisaViewModel visaModel = new VisaViewModel(visa);

                return View(visaModel);
            }
        }

        //
        // POST: /Visa/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Visa visa, string searchString = "")
        {
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            ViewBag.SearchString = searchString;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveVisa(visa, visa.EmployeeID);
                }

                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { error = ModelError });
                }

                List<Employee> empList = SearchVisaData(repository.Employees.ToList(), searchString);
                return View("TableViewVisasAndPermitsBTM", empList);
            }

            VisaViewModel visaModel = new VisaViewModel(visa);
            ViewBag.SearchString = searchString;
            return View(visaModel);
        }

        //
        // GET: /Visa/Delete/5

        public ActionResult Delete(int id = 0, string searchString = "")
        {
            Visa visa = (from v in repository.Visas where v.EmployeeID == id select v).FirstOrDefault();

            if (visa == null)
            {
                return HttpNotFound();
            }
            ViewBag.SearchString = searchString;
            ViewBag.EmployeeID = id;
            return View(visa);
        }

        //
        // POST: /Visa/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string searchString = "")
        {
            repository.DeleteVisa(id);
            ViewBag.SearchString = searchString;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            List<Employee> empList = SearchVisaData(repository.Employees.ToList(), searchString);
            return View("TableViewVisasAndPermitsBTM", empList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "VU")]
        public ActionResult ExportVisasAndPermits(string searchString)
        {
            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet("First Sheet");
            CreateCaption(workSheet);
            WriteVisasAndPermitsData(workSheet, searchString);
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return File(stream.ToArray(), "application/vnd.ms-excel", "VisasAndPermits.xls");
        }

        [Authorize(Roles = "VU")]
        public void CreateCaption(Worksheet workSheet)
        {
            string[] caption = new string[] { "EID", "Name", "Passport", "Type", "Visa From", "Visa To", "Entries", "Days", "Registration", "Num", "Permit From - To", "Last BT", "Status" };
            for (int i = 0; i < caption.Length; i++)
            {
                workSheet.Cells[0, i] = new Cell(caption[i]);
            }
        }

        [Authorize(Roles = "VU")]
        public void WriteVisasAndPermitsData(Worksheet workSheet, string searchString = "")
        {
            int i = 1;
            searchString = searchString != "" ? searchString.Trim() : "";

            List<Employee> selected = repository.Employees.ToList();
            List<Employee> empsWithVisasAndPermits = SearchVisaData(selected, searchString);

            foreach (Employee emp in empsWithVisasAndPermits)
            {
                workSheet.Cells[i, 0] = new Cell(emp.EID);

                if (emp.DateDismissed != null)
                {
                    workSheet.Cells[i, 1] = new Cell(emp.LastName + " " + emp.FirstName + "\n" + emp.DateDismissed.Value.ToShortDateString());
                }
                else
                {
                    workSheet.Cells[i, 1] = new Cell(emp.LastName + " " + emp.FirstName);
                }

                if (emp.Passport != null)
                {
                    if(emp.Passport.EndDate.HasValue)
                    {
                        workSheet.Cells[i,2] = new Cell("till\n" + emp.Passport.EndDate.Value.ToString(String.Format("dd.MM.yyyy")));
                    }
                    else
                    {
                        workSheet.Cells[i,2] = new Cell("yes");
                    }
                }
                else
                {
                    workSheet.Cells[i,2] = new Cell("no");
                }

                if(emp.Visa != null)
                {

                    if (! emp.Visa.DaysUsedInBT.HasValue)
                    {
                        emp.Visa.DaysUsedInBT = 0;
                    }

                    if (!emp.Visa.EntriesUsedInPrivateTrips.HasValue)
                    {
                        emp.Visa.EntriesUsedInPrivateTrips = 0;
                    }

                    if (!emp.Visa.EntriesUsedInBT.HasValue)
                    {
                        emp.Visa.EntriesUsedInBT = 0;
                    }

                    if (!emp.Visa.CorrectionForVisaEntries.HasValue)
                    {
                        emp.Visa.CorrectionForVisaEntries = 0;
                    }

                    if (!emp.Visa.DaysUsedInPrivateTrips.HasValue)
                    {
                        emp.Visa.DaysUsedInPrivateTrips = 0;
                    }

                    if (!emp.Visa.DaysUsedInBT.HasValue)
                    {
                        emp.Visa.DaysUsedInBT = 0;
                    }

                    if (!emp.Visa.CorrectionForVisaDays.HasValue)
                    {
                        emp.Visa.CorrectionForVisaDays = 0;
                    }


                    workSheet.Cells[i,3] = new Cell(emp.Visa.VisaType);
                    workSheet.Cells[i,4] = new Cell(emp.Visa.StartDate.ToString(String.Format("yyyy-MM-dd")));
                    workSheet.Cells[i,5] = new Cell(emp.Visa.DueDate.ToString(String.Format("yyyy-MM-dd")));

                    if (emp.Visa.Entries == 0)
                    {
                        workSheet.Cells[i, 6] = new Cell("MULT");
                    }
                    else
                    {
                        workSheet.Cells[i, 6] = new Cell(emp.Visa.Entries.ToString() + "(" + (emp.Visa.EntriesUsedInPrivateTrips.Value + emp.Visa.EntriesUsedInBT.Value + emp.Visa.CorrectionForVisaEntries.Value).ToString() + ")");                        
                    }

                    workSheet.Cells[i,7] = new Cell(emp.Visa.Days.ToString() + "("+(emp.Visa.DaysUsedInPrivateTrips.Value + emp.Visa.DaysUsedInBT.Value + emp.Visa.CorrectionForVisaDays.Value).ToString()+")");
                }
                else
                {
                    workSheet.Cells[i,3] = new Cell("");
                    workSheet.Cells[i,4] = new Cell("No Visa");
                    workSheet.Cells[i,5] = new Cell("No Visa");
                    workSheet.Cells[i,6] = new Cell("");
                    workSheet.Cells[i,7] = new Cell("");
                }

                if(emp.VisaRegistrationDate != null)
                {
                    workSheet.Cells[i,8] = new Cell(emp.VisaRegistrationDate.RegistrationDate.ToString(String.Format("dd.MM.yyyy")));

                }
                else
                {
                    workSheet.Cells[i,8] = new Cell("");
                }

                if (emp.Permit != null)
                {
                    if (emp.Permit.Number != null || emp.Permit.StartDate != null || emp.Permit.EndDate != null)
                    {
                        workSheet.Cells[i, 9] = new Cell(emp.Permit.Number);

                        if (emp.Permit.IsKartaPolaka)
                        {
                            workSheet.Cells[i, 10] = new Cell("Karta Polaka");

                            if (emp.Permit.StartDate != null && emp.Permit.EndDate != null)
                            {
                                workSheet.Cells[i, 10] = new Cell("Karta Polaka" + "\n" + emp.Permit.StartDate.Value.ToString(String.Format("dd.MM.yyyy")) + " - " + emp.Permit.EndDate.Value.ToString(String.Format("dd.MM.yyyy")));
                            }
                        }
                        else
                        {
                            workSheet.Cells[i, 10] = new Cell(emp.Permit.StartDate.Value.ToShortDateString() + " - " + emp.Permit.EndDate.Value.ToShortDateString());
                        }
                    }
                    else
                    {
                        if (emp.Permit.IsKartaPolaka == true && emp.Permit.Number == null && emp.Permit.StartDate == null && emp.Permit.EndDate == null)
                        {
                            workSheet.Cells[i, 10] = new Cell("Karta Polaka");
                        }
                    }
                }
                else
                {
                    workSheet.Cells[i, 10] = new Cell("No Permit");
                }

                if (emp.BusinessTrips != null)
                {
                    BusinessTrip lastBT = emp.BusinessTrips
                                                  .Where(b => (b.EndDate < DateTime.Now.ToLocalTimeAzure().Date)
                                                               && (b.Status == (BTStatus.Confirmed | BTStatus.Reported)))
                                                  .LastOrDefault();
                    if (lastBT != null)
                    {
                        workSheet.Cells[i, 11] = new Cell(lastBT.Location.Title + ":" + lastBT.StartDate.ToString("dd.MM.yy") + " - " + lastBT.EndDate.ToString("dd.MM.yy"));
                    }
                    else
                    {
                        workSheet.Cells[i, 11] = new Cell("");
                    }
                }

                if (emp.Permit != null && emp.Permit.EndDate != null)
                {
                    if (emp.Permit.CancelRequestDate != null)
                    {
                        workSheet.Cells[i, 12] = new Cell(emp.Permit.CancelRequestDate.Value.ToString(String.Format("dd.MM.yyyy")));
                    }
                    if (emp.Permit.ProlongRequestDate != null)
                    {
                        workSheet.Cells[i, 12] = new Cell(emp.Permit.ProlongRequestDate.Value.ToString(String.Format("dd.MM.yyyy")));
                    }

                    DateTime StartingPoint = DisplayPermitStatusHelper.GetStartingDateTimePointForPermitExpiration(emp);

                    if (DisplayPermitStatusHelper.Is60ToLessThan90Days(StartingPoint))
                    {
                        workSheet.Cells[i, 12] = new Cell("Contact Gov");
                    }
                    if (DisplayPermitStatusHelper.Is90DaysAndAbove(StartingPoint))
                    {
                        workSheet.Cells[i, 12] = new Cell("Contact Gov");
                    }

                }
                else
                {
                    workSheet.Cells[i, 12] = new Cell("");
                }
              
                i++;
            }
        }
    }
}