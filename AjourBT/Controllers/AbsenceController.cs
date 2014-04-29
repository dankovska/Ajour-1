using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Models;
using ExcelLibrary.SpreadSheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class AbsenceController : Controller
    {
        private IRepository repository;

        public AbsenceController(IRepository repo)
        {
            repository = repo;
        }
        //
        // GET: /Absence/

        public ActionResult GetAbsence(string searchString = "")
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;

            ViewBag.SearchString = searchString;
            ViewBag.FromValue = new DateTime(currYear, currMonth, 01);
            ViewBag.ToValue = new DateTime(currYear, currMonth, DateTime.DaysInMonth(currYear, currMonth));
            return View();
        }

        public ActionResult GetAbsenceData(string fromDate, string toDate, string searchString = "")
        {
            DateTime parseFromDate = new DateTime();
            DateTime parseToDate = new DateTime();

            try
            {
                parseFromDate = DateTime.ParseExact(fromDate, "dd.MM.yyyy", null);
                parseToDate = DateTime.ParseExact(toDate, "dd.MM.yyyy", null);
            }
            catch (SystemException)
            {
                return View("NoData");
            }

            searchString = searchString != "" ? searchString.Trim() : "";
            List<CalendarItem> calendarItemsList = SearchEmployeeData(parseFromDate, parseToDate, searchString);
            if (calendarItemsList.Count == 0)
            {
                return View("NoData");
            }

            List<int> empID = calendarItemsList.Select(id => id.EmployeeID).Distinct().ToList();
            empID.Sort();
            List<AbsenceViewModel> absenceData = new List<AbsenceViewModel>();

            foreach (int id in empID)
            {
                AbsenceViewModel model = new AbsenceViewModel();

                Employee emp = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();

                model.Department = emp.Department.DepartmentName;
                model.EID = emp.EID;
                model.EmployeeID = emp.EmployeeID;
                model.FirstName = emp.FirstName;
                model.LastName = emp.LastName;
                model.Journeys = new List<CalendarItem>();
                model.Overtimes = new List<CalendarItem>();
                model.Sickness = new List<CalendarItem>();
                model.Vacations = new List<CalendarItem>();
                model.BusinessTrips = new List<CalendarItem>();

                absenceData.Add(model);
            }

            foreach (CalendarItem item in calendarItemsList)
            {
                //if ((item.From <= parseFromDate && item.To >= parseFromDate)
                //               || (item.From >= parseFromDate && item.From <= parseToDate))
                //{
                    AbsenceViewModel temp = absenceData.Where(id => id.EmployeeID == item.EmployeeID).FirstOrDefault();
                    if (temp != null)
                    {
                        switch (item.Type)
                        {
                            case CalendarItemType.Journey:

                                temp.Journeys.Add(item);
                                break;

                            case CalendarItemType.ReclaimedOvertime:
                            case CalendarItemType.PrivateMinus:

                                temp.Overtimes.Add(item);
                                break;

                            case CalendarItemType.PaidVacation:
                            case CalendarItemType.UnpaidVacation:

                                temp.Vacations.Add(item);
                                break;

                            case CalendarItemType.SickAbsence:

                                temp.Sickness.Add(item);
                                break;

                            case CalendarItemType.BT:
                                temp.BusinessTrips.Add(item);
                                break;
                        }
                    }
                }
            foreach(AbsenceViewModel model in absenceData)
            {
                model.Journeys.Sort((x, y) => x.From.CompareTo(y.From));
                model.Overtimes.Sort((x, y) => x.From.CompareTo(y.From));
                model.Vacations.Sort((x, y) => x.From.CompareTo(y.From));
                model.Sickness.Sort((x, y) => x.From.CompareTo(y.From));
                model.BusinessTrips.Sort((x,y) => x.From.CompareTo(y.From));
            }
            //}
            return View(absenceData);
        }

        public List<CalendarItem> SearchEmployeeData(DateTime fromDate, DateTime toDate, string searchString = "")
        {
            List<Employee> empList = repository.Employees.ToList();

            List<CalendarItem> query = (from emp in empList
                                    where ((emp.CalendarItems.Count != 0) && ((emp.Department.DepartmentName.ToLower().Contains(searchString.ToLower()))||(emp.FirstName.ToLower().Contains(searchString.ToLower())) ||
                                        emp.LastName.ToLower().Contains(searchString.ToLower()) ||
                                        emp.EID.ToLower().Contains(searchString.ToLower())))
                                    from f in emp.CalendarItems
                                    where ((f.From <= fromDate && f.To >= fromDate) || (f.From >= fromDate && f.From <= toDate))
                                    orderby emp.Department.DepartmentID, emp.LastName
                                    select f).Distinct().ToList();
            return query;
        }


        public List<AbsenceViewModel> SearchAbsenceData(string fromDate, string toDate, string searchString = "")
        {

            DateTime parseFromDate = new DateTime();
            DateTime parseToDate = new DateTime();

            try
            {
                parseFromDate = DateTime.ParseExact(fromDate, "dd.MM.yyyy", null);
                parseToDate = DateTime.ParseExact(toDate, "dd.MM.yyyy", null);
            }
            catch (SystemException)
            {
               return new List<AbsenceViewModel>();
            }

            searchString = searchString != "" ? searchString.Trim() : "";
            List<CalendarItem> calendarItemsList = SearchEmployeeData(parseFromDate, parseToDate, searchString);
           
            List<int> empID = calendarItemsList.Select(id => id.EmployeeID).Distinct().ToList();
            empID.Sort();
            List<AbsenceViewModel> absenceData = new List<AbsenceViewModel>();

            foreach (int id in empID)
            {
                AbsenceViewModel model = new AbsenceViewModel();

                Employee emp = (from e in repository.Employees where e.EmployeeID == id select e).FirstOrDefault();

                model.Department = emp.Department.DepartmentName;
                model.EID = emp.EID;
                model.EmployeeID = emp.EmployeeID;
                model.FirstName = emp.FirstName;
                model.LastName = emp.LastName;
                model.Journeys = new List<CalendarItem>();
                model.Overtimes = new List<CalendarItem>();
                model.Sickness = new List<CalendarItem>();
                model.Vacations = new List<CalendarItem>();
                model.BusinessTrips = new List<CalendarItem>();

                absenceData.Add(model);
            }

            foreach (CalendarItem item in calendarItemsList)
            {
                
                AbsenceViewModel temp = absenceData.Where(id => id.EmployeeID == item.EmployeeID).FirstOrDefault();
                if (temp != null)
                {
                    switch (item.Type)
                    {
                        case CalendarItemType.Journey:

                            temp.Journeys.Add(item);
                            break;

                        case CalendarItemType.ReclaimedOvertime:
                        case CalendarItemType.PrivateMinus:

                            temp.Overtimes.Add(item);
                            break;

                        case CalendarItemType.PaidVacation:
                        case CalendarItemType.UnpaidVacation:

                            temp.Vacations.Add(item);
                            break;

                        case CalendarItemType.SickAbsence:

                            temp.Sickness.Add(item);
                            break;

                        case CalendarItemType.BT:
                            temp.BusinessTrips.Add(item);
                            break;
                    }
                }
            }
            foreach (AbsenceViewModel model in absenceData)
            {
                model.Journeys.Sort((x, y) => x.From.CompareTo(y.From));
                model.Overtimes.Sort((x, y) => x.From.CompareTo(y.From));
                model.Vacations.Sort((x, y) => x.From.CompareTo(y.From));
                model.Sickness.Sort((x, y) => x.From.CompareTo(y.From));
                model.BusinessTrips.Sort((x, y) => x.From.CompareTo(y.From));
            }
            
            return absenceData;
        }

     
        [HttpPost]
        public ActionResult ExportAbsenceToExcel(string from, string to, string searchString = "")
       {
            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet("First Sheet");
            CreateCaption(workSheet);
            WriteAbsenceData(workSheet, from, to, searchString);
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return File(stream.ToArray(), "application/vnd.ms-excel", "Absence.xls");
        }

        public void CreateCaption(Worksheet workSheet)
        {
            string[] caption = new string[] { "Department", "Name", "EID", "Journeys", "BusinessTrips", "Overtimes", "Sickness", "Vacations" };
            for (int i = 0; i < caption.Length; i++)
            {
               // workSheet.Cells.ColumnWidth[0, 1] = 3000;
                workSheet.Cells[0, i] = new Cell(caption[i]);
            }
            workSheet.Cells.ColumnWidth[0] = 3000;
            workSheet.Cells.ColumnWidth[1] = 6000;
            workSheet.Cells.ColumnWidth[2] = 3000;
            workSheet.Cells.ColumnWidth[3] = 6000;
            workSheet.Cells.ColumnWidth[4, 5] = 6000;
            workSheet.Cells.ColumnWidth[6, 7] = 6000;
        }

    
        
            public void WriteAbsenceData(Worksheet workSheet, string from, string to, string searchString = "")
        {
            int i = 1;
           searchString = searchString != "" ? searchString.Trim() : "";
         
          List< AbsenceViewModel>  absences = SearchAbsenceData(from, to, searchString);
      
            for(int j=0; j < absences.Count(); j++)
           {

               if (absences[j].Journeys.Count != 0 || absences[j].BusinessTrips.Count != 0 || absences[j].Overtimes.Count != 0 || absences[j].Sickness.Count != 0 || absences[j].Vacations.Count != 0)
               {
                   workSheet.Cells[i, 0] = new Cell(absences[j].Department);
                   workSheet.Cells[i, 1] = new Cell(absences[j].LastName + " " + absences[j].FirstName);
                   workSheet.Cells[i, 2] = new Cell(absences[j].EID);
               }
              
                foreach (var item in absences[j].Journeys.AsEnumerable())
                {
                    if (absences[j].Journeys.Count != 0)
                    {
                        workSheet.Cells[i, 3] = new Cell(item.From.Date.ToString(String.Format("dd.MM.yyyy")) + " - " + item.To.Date.ToString(String.Format("dd.MM.yyyy")));
                        i++;
                    }

                }

               foreach (var item in absences[j].BusinessTrips.AsEnumerable())
               {
                   if (absences[j].BusinessTrips.Count != 0)
                   {
                       workSheet.Cells[i, 4] = new Cell(item.From.Date.ToString(String.Format("dd.MM.yyyy")) + " - " + item.To.Date.ToString(String.Format("dd.MM.yyyy")));
                       i++;
                   }
               }
                 
                  
               foreach(var item in  absences[j].Overtimes.AsEnumerable())
               {
                   if (absences[j].Overtimes.Count != 0)
                   {
                       workSheet.Cells[i, 5] = new Cell(item.From.Date.ToString(String.Format("dd.MM.yyyy")) + " - " + item.To.Date.ToString(String.Format("dd.MM.yyyy")));
                       i++;
                   }
               }
                
                foreach(var item in  absences[j].Sickness.AsEnumerable())
               {
                   if (absences[j].Sickness.Count != 0)
                   {
                       workSheet.Cells[i, 6] = new Cell(item.From.Date.ToString(String.Format("dd.MM.yyyy")) + " - " + item.To.Date.ToString(String.Format("dd.MM.yyyy")));
                       i++;
                   }
               }
                
                foreach(var item in  absences[j].Vacations.AsEnumerable())
                {
                    if (absences[j].Vacations.Count != 0)
                    {
                        workSheet.Cells[i, 7] = new Cell(item.From.Date.ToString(String.Format("dd.MM.yyyy")) + " - " + item.To.Date.ToString(String.Format("dd.MM.yyyy")));
                        i++;
                    }
               }
           
            }
        }

    }
}
