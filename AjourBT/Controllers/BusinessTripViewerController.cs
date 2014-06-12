using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Helpers;
using AjourBT.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Infrastructure;
using AjourBT.Domain.Infrastructure;
using System.Text;
using ExcelLibrary.SpreadSheet;
using System.IO;

namespace AjourBT.Controllers
{
#if !DEBUG
//[RequireHttps] //apply to all actions in controller
#endif
    public class BusinessTripViewerController : Controller
    {
        private IRepository repository;
        private StringBuilder comment = new StringBuilder();
        private string defaultAccComment;

        public BusinessTripViewerController(IRepository repo)
        {
            repository = repo;
            this.comment = this.comment.Append("ВКО №   від   , cума:   UAH.");
            this.comment = this.comment.AppendLine();
            this.comment = comment.Append("ВКО №   від   , cума:   USD.");
            this.defaultAccComment = comment.ToString();
        }

        [Authorize(Roles = "VU")]
        public ViewResult GetBusinessTripDataInQuarterVU(int selectedKey, string selectedDepartment = "", string searchString = "")
        {
            if (searchString != "")
            {
                searchString = searchString.ToLower().Trim();
            }

            DateTime currentDate = DateTime.Now.ToLocalTimeAzure().Date;
            DateTime selectedStartPeriod = currentDate;
            DateTime selectedStartPeriod2 = currentDate;
            string viewName = "";
            List<int> monthes = new List<int>();
            List<int> years = new List<int>();
            switch (selectedKey)
            {
                case 0:
                    viewName = "GetBusinessTripDataInMonthesVU";
                    break;
                case 1:
                    selectedStartPeriod = currentDate.AddMonths(-1);
                    selectedStartPeriod2 = currentDate.AddMonths(-1);
                    viewName = "GetBusinessTripDataInMonthesVU";
                    break;
                case 3:
                    selectedStartPeriod = currentDate.AddMonths(-3);
                    selectedStartPeriod2 = currentDate.AddMonths(-3);
                    viewName = "GetBusinessTripDataInMonthesVU";
                    break;
                case 6:
                    selectedStartPeriod = currentDate.AddMonths(-6);
                    selectedStartPeriod2 = currentDate.AddMonths(-6);
                    viewName = "GetBusinessTripDataInMonthesVU";
                    break;
                default:
                    monthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                    selectedStartPeriod = new DateTime(selectedKey, 1, 1);
                    break;
            }

            if (selectedKey < 7)
            {
                //for (int i = 0; i <= ((selectedStartPeriod2.Month + selectedKey) - selectedStartPeriod2.Month); i++)
                for (int i = 0; i <= selectedKey; i++)
                {
                    monthes.Add(selectedStartPeriod2.Month);
                    selectedStartPeriod2 = selectedStartPeriod2.AddMonths(1);
                }

            }
            ViewBag.MonthList = monthes;
            ViewBag.SelectedKey = selectedKey;
            ViewBag.SearchString = searchString;


            var employeeGroups = from e in repository.Employees
                                 where ((selectedDepartment == String.Empty ||
                                        e.Department.DepartmentName == selectedDepartment)
                                        && (e.FirstName.ToLower().Contains(searchString) ||
                                            e.LastName.ToLower().Contains(searchString) ||
                                            e.EID.ToLower().Contains(searchString)))


                                 orderby e.LastName
                                 select new
                                 {
                                     e.LastName,
                                     e.FirstName,
                                     e.EID,
                                     e.IsManager,
                                     e.DateDismissed,
                                     MonthGroups = from bt in e.BusinessTrips
                                                   where ((
                                                        (bt.StartDate.Year == selectedKey && selectedKey >= 7) ||
                                                            (selectedKey < 7 &&
                                                                ((bt.StartDate.Month >= selectedStartPeriod.Month && bt.StartDate.Year == selectedStartPeriod.Year && bt.StartDate <= currentDate)
                                                                || (bt.StartDate.Month < selectedStartPeriod2.Month && bt.StartDate.Year == selectedStartPeriod2.Year && bt.StartDate <= currentDate))))
                                                        && (bt.Status == (BTStatus.Confirmed | BTStatus.Reported) || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled))
                                                        )
                                                   group bt by bt.StartDate.Month into MonthGroup
                                                   select new { Month = MonthGroup.Key, Bts = MonthGroup }
                                 };


            List<EmployeeViewModelForVU> employeesBTsByMonthList = new List<EmployeeViewModelForVU>();

            foreach (var emp in employeeGroups)
            {
                EmployeeViewModelForVU employee = new EmployeeViewModelForVU();

                employee.LastName = emp.LastName;
                employee.FirstName = emp.FirstName;
                employee.EID = emp.EID;
                employee.IsManager = emp.IsManager;
                employee.DateDismissed = String.Format("{0:d}", emp.DateDismissed);
                employee.BusinessTripsByMonth = new Dictionary<int, List<BusinessTrip>>();
                employee.DaysUsedInBt = 0;

                foreach (var month in emp.MonthGroups)
                {
                    List<BusinessTrip> monthBTs = new List<BusinessTrip>();
                    int days = 0;

                    foreach (var bt in month.Bts.Where(b => b.Status == (BTStatus.Confirmed | BTStatus.Reported)))
                    {
                        days = (bt.EndDate - bt.StartDate).Days + 1;
                        employee.DaysUsedInBt += days;
                        monthBTs.Add(bt);
                    }

                    employee.BusinessTripsByMonth.Add(month.Month, monthBTs);
                }

                employeesBTsByMonthList.Add(employee);

            }
            return View(viewName, employeesBTsByMonthList);
        }


        [Authorize(Roles = "VU")]
        public ActionResult GetListOfYearsForQuarterVU(int selectedKey, string selectedDepartment = "")
        {
            var selected = from bt in repository.BusinessTrips
                           group bt by bt.StartDate.Year into yearGroup
                           select new { Year = yearGroup.Key };

            Dictionary<int, string> values = new Dictionary<int, string>();

            values.Add(0, "current month");
            values.Add(1, "last month(till today)");
            values.Add(3, "last 3 monthes(till today)");
            values.Add(6, "last 6 monthes(till today)");
            for (int i = 0; i < selected.AsEnumerable().Count(); i++)
            {
                values.Add(selected.AsEnumerable().ToArray()[i].Year, selected.AsEnumerable().ToArray()[i].Year.ToString());
            }
            ViewBag.SelectedValues = new SelectList(values, "Key", "Value");


            return View(selectedKey);
        }


        public ActionResult GetListOfDepartmentsVU(int selectedKey, string selectedDepartment = null)
        {
            var departmentList = from dep in repository.Departments
                                 orderby dep.DepartmentName
                                 select dep;

            ViewBag.DepartmentList = new SelectList(departmentList, "DepartmentName", "DepartmentName");
            ViewBag.SelectedDepartment = selectedDepartment;

            return View();
        }

        [Authorize(Roles = "VU")]
        public ActionResult ShowBTInformation(int id = 0)
        {
            BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

            if (businessTrip == null)
            {
                return HttpNotFound();
            }

            ViewBag.DefaultAccComment = defaultAccComment;
            return View(businessTrip);
        }

        //[Authorize(Roles = "VU")]
        //public ActionResult ShowPrepsBTInformation(int id = 0)
        //{
        //    BusinessTrip businessTrip = repository.BusinessTrips.Where(b => b.BusinessTripID == id).FirstOrDefault();

        //    if (businessTrip == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View("ShowBTInformation", businessTrip);
        //}

        // GET: /BusinessTripViewer/
        //[Authorize(Roles = "VU")]
        //public ViewResult GetBusinessTripDataVU(int selectedYear, string selectedDepartment = "")
        //{

        //    var employeeGroups = from e in repository.Employees
        //                         where selectedDepartment == String.Empty || e.Department.DepartmentName == selectedDepartment
        //                         orderby e.LastName
        //                         select new
        //                         {
        //                             e.LastName,
        //                             e.FirstName,
        //                             e.EID,
        //                             MonthGroups = from bt in e.BusinessTrips
        //                                           where bt.StartDate.Year == selectedYear
        //                                                 && bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
        //                                                 && bt.Status != BTStatus.Cancelled
        //                                           group bt by bt.StartDate.Month into MonthGroup
        //                                           select new { Month = MonthGroup.Key, Bts = MonthGroup }
        //                         };

        //    List<EmployeeViewModelForVU> employeesBTsByMonthList = new List<EmployeeViewModelForVU>();

        //    foreach (var emp in employeeGroups)
        //    {
        //        EmployeeViewModelForVU employee = new EmployeeViewModelForVU();

        //        employee.LastName = emp.LastName;
        //        employee.FirstName = emp.FirstName;
        //        employee.EID = emp.EID;
        //        employee.BusinessTripsByMonth = new Dictionary<int, List<BusinessTrip>>();
        //        employee.BusinessTripID = new int();
        //        employee.DaysUsedInBt = 0;

        //        foreach (var month in emp.MonthGroups)
        //        {
        //            List<BusinessTrip> monthBTs = new List<BusinessTrip>();
        //            int days = 0;

        //            foreach (var bt in month.Bts)
        //            {
        //                days = (bt.EndDate - bt.StartDate).Days + 1;
        //                employee.DaysUsedInBt += days;
        //                monthBTs.Add(bt);
        //                employee.BusinessTripID = bt.BusinessTripID;
        //            }

        //            employee.BusinessTripsByMonth.Add(month.Month, monthBTs); //@"KR/B: 1.01 - 10.01<br>LD/S: 10.01 - 15.01"
        //        }

        //        employeesBTsByMonthList.Add(employee);

        //    }

        //    int[] monthes = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        //    ViewBag.Monthes = monthes;

        //    return View(employeesBTsByMonthList);
        //}


        //[Authorize(Roles = "VU")]
        //public ActionResult GetListOfYearsVU(int selectedYear, string selectedDepartment = "")
        //{
        //    var selected = from bt in repository.BusinessTrips
        //                   group bt by bt.StartDate.Year into yearGroup
        //                   select new { Year = yearGroup.Key };

        //    ViewBag.SelectedYear = new SelectList(selected, "Year", "Year");

        //    return View(selectedYear);
        //}

        [Authorize(Roles = "VU")]
        public ViewResult GetPrepBusinessTripDataVU()
        {
            DateTime today = DateTime.Now.ToLocalTimeAzure().Date;
            var empGroups = from e in repository.Employees
                            orderby e.LastName
                            select new
                            {
                                e.EmployeeID,
                                e.LastName,
                                e.FirstName,
                                e.EID,
                                IsManager = e.IsManager,
                                BTGroups = from b in e.BusinessTrips
                                           where ((b.Status != BTStatus.Planned)
                                            && b.Status != (BTStatus.Planned | BTStatus.Modified)
                                            && b.Status != (BTStatus.Planned | BTStatus.Cancelled)
                                               //&& b.StartDate > today
                                            )

                                           group b by b.Status into EmployeeGroup
                                           select new { EmpID = EmployeeGroup.Key, Bts = EmployeeGroup }
                            };

            var employeeGroup = from e in empGroups
                                where e.BTGroups.Count() != 0
                                select e;

            List<PrepBusinessTripViewModelForVU> employeeList = new List<PrepBusinessTripViewModelForVU>();

            foreach (var e in employeeGroup)
            {

                PrepBusinessTripViewModelForVU employee = new PrepBusinessTripViewModelForVU();

                employee.EmployeeID = e.EmployeeID;
                employee.LastName = e.LastName;
                employee.FirstName = e.FirstName;
                employee.EID = e.EID;
                employee.IsManager = e.IsManager;
                employee.BusinessTripsByEmployee = new Dictionary<BTStatus, List<BusinessTrip>>();

                foreach (var eGroup in e.BTGroups)
                {
                    List<BusinessTrip> empBTs = new List<BusinessTrip>();

                    foreach (var bt in eGroup.Bts)
                    {
                        empBTs.Add(bt);
                    }

                    employee.BusinessTripsByEmployee.Add(eGroup.EmpID, empBTs);

                }
                employeeList.Add(employee);
            }
            return View(employeeList);
        }


        [Authorize(Roles = "VU")]
        public ViewResult GetBusinessTripByDatesVU(int selectedYear = 0)
        {
            var selected = from bt in repository.BusinessTrips
                           group bt by bt.StartDate.Year into yearGroup
                           select new { Year = yearGroup.Key };

            ViewBag.SelectedYear = new SelectList(selected, "Year", "Year");

            return View(selectedYear);
        }

        [Authorize(Roles = "VU")]
        public PartialViewResult GetBusinessTripDataByDatesVU(int selectedYear = 0)
        {
            var query = from bt in repository.BusinessTrips.AsEnumerable()
                        join emp in repository.Employees on bt.EmployeeID equals emp.EmployeeID
                        join loc in repository.Locations on bt.LocationID equals loc.LocationID
                        where (bt.StartDate.Year == selectedYear
                              && (bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                                    || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled)
                                    || bt.Status == (BTStatus.Confirmed | BTStatus.Modified)))
                        orderby emp.LastName, bt.StartDate
                        select new BusinessTripViewModel(bt);

            ViewBag.SelectedYear = selectedYear;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(query.ToList());
        }



        [Authorize(Roles = "VU")]
        public ViewResult GetBusinessTripByUnitsVU(int selectedYear = 0)
        {
            var selected = from bt in repository.BusinessTrips
                           group bt by bt.StartDate.Year into yearGroup
                           select new { Year = yearGroup.Key };

            ViewBag.SelectedYear = new SelectList(selected, "Year", "Year");

            return View(selectedYear);
        }

        [Authorize(Roles = "VU")]
        public PartialViewResult GetBusinessTripDataByUnitsVU(int selectedYear = 0)
        {
            var query = BusinessTripDataByUnitsQuery(selectedYear);

            ViewBag.SelectedYear = selectedYear;
            ViewBag.JSDatePattern = MvcApplication.JSDatePattern;
            return PartialView(query.ToList());
        }

        public IEnumerable<BusinessTripViewModel> BusinessTripDataByUnitsQuery(int selectedYear)
        {
            int FirstBusinessTripIdInYear = GetFirstBusinessTripIdInYear(selectedYear);
            var query = from bt in repository.BusinessTrips.AsEnumerable()
                        join emp in repository.Employees on bt.EmployeeID equals emp.EmployeeID
                        join loc in repository.Locations on bt.LocationID equals loc.LocationID
                        where (bt.StartDate.Year == selectedYear
                              && (bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                              || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled)
                              || bt.Status == (BTStatus.Confirmed | BTStatus.Modified)))
                        orderby bt.BusinessTripID
                        select new BusinessTripViewModel(bt, CalculateId(bt.BusinessTripID, FirstBusinessTripIdInYear));
            return query;
        }

        public int GetFirstBusinessTripIdInYear(int selectedYear)
        {
            var query = (from bt in repository.BusinessTrips.AsEnumerable()
                         join emp in repository.Employees on bt.EmployeeID equals emp.EmployeeID
                         join loc in repository.Locations on bt.LocationID equals loc.LocationID
                         where (bt.StartDate.Year == selectedYear
                               && (bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                               || bt.Status == (BTStatus.Confirmed | BTStatus.Cancelled)
                               || bt.Status == (BTStatus.Confirmed | BTStatus.Modified)))
                         orderby bt.BusinessTripID
                         select bt.BusinessTripID).FirstOrDefault();
            return query;
        }

        int CalculateId(int BusinessTripID, int FirstBusinessTripID)
        {
            return BusinessTripID - FirstBusinessTripID + 1;
        }

        public IEnumerable<BusinessTripViewModel> BusinessTripDataByUnitsWithoutCancelledAndDismissedQuery(int selectedYear)
        {
            var query = BusinessTripDataByUnitsQuery(selectedYear).Where(bt => bt.BTof.DateDismissed == null &&
                bt.Status == (BTStatus.Confirmed | BTStatus.Reported)
                             || bt.Status == (BTStatus.Confirmed | BTStatus.Modified));
            return query;
        }

        [Authorize(Roles = "VU")]
        [HttpPost]
        public ActionResult ExportBusinessTripByDatesToExcel(int selectedYear)
        {
            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet("First Sheet");
            CreateCaption(workSheet);
            WriteBusinessTripsData(workSheet, selectedYear);
            workBook.Worksheets.Add(workSheet);
            //workBook.Save(fileName); 
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return File(stream.ToArray(), "application/vnd.ms-excel", "BusinessTripsByDates.xls");
        }

        [Authorize(Roles = "VU")]
        public void CreateCaption(Worksheet workSheet)
        {
            string[] caption = new string[] { "ID", "EID", "Name", "Loc", "From", "To", "Unit", "Purpose", "Mgr", "Resp" };
            for (int i = 0; i < caption.Length; i++)
            {
                workSheet.Cells[0, i] = new Cell(caption[i]);
            }
        }

        [Authorize(Roles = "VU")]
        public void WriteBusinessTripsData(Worksheet workSheet, int selectedYear)
        {
            int i = 1;
            var businessTrips = BusinessTripDataByUnitsWithoutCancelledAndDismissedQuery(selectedYear);
            workSheet.Cells.ColumnWidth[0] = 1067;
            workSheet.Cells.ColumnWidth[1] = 1700;
            workSheet.Cells.ColumnWidth[2] = 4867;
            workSheet.Cells.ColumnWidth[3] = 2067;
            workSheet.Cells.ColumnWidth[4] = 2867;
            workSheet.Cells.ColumnWidth[5] = 2867;
            workSheet.Cells.ColumnWidth[6] = 2434;
            workSheet.Cells.ColumnWidth[7] = 4867;
            workSheet.Cells.ColumnWidth[8] = 1466;
            workSheet.Cells.ColumnWidth[9] = 1631;
            foreach (BusinessTripViewModel businessTripViewModel in businessTrips)
            {
                workSheet.Cells[i, 0] = new Cell(businessTripViewModel.BusinessTripID);
                workSheet.Cells[i, 1] = new Cell(businessTripViewModel.BTof.EID);
                workSheet.Cells[i, 2] = new Cell(businessTripViewModel.BTof.LastName + " " + businessTripViewModel.BTof.FirstName);
                workSheet.Cells[i, 3] = new Cell(businessTripViewModel.Title);
                if (businessTripViewModel.Status == (BTStatus.Confirmed | BTStatus.Reported))
                {
                    workSheet.Cells[i, 4] = new Cell(businessTripViewModel.StartDateFormated);
                }
                else
                {
                    workSheet.Cells.ColumnWidth[4] = 7166;
                    workSheet.Cells[i, 4] = new Cell(businessTripViewModel.StartDateFormated + " To be updated soon");
                }
                workSheet.Cells[i, 5] = new Cell(businessTripViewModel.EndDateFormated);
                workSheet.Cells[i, 6] = new Cell(businessTripViewModel.Unit);
                workSheet.Cells[i, 7] = new Cell(businessTripViewModel.Purpose);
                workSheet.Cells[i, 8] = new Cell(businessTripViewModel.Manager);
                workSheet.Cells[i, 9] = new Cell(businessTripViewModel.Responsible);
                i++;
            }
        }

    }
}
