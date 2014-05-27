using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Helpers;
using AjourBT.Models;
using ExcelLibrary.SpreadSheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Controllers
{
    public class WTRController : Controller
    {
        public WTRController()
        {
            CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfo _uiculture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();

            _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            _uiculture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = _uiculture;
        }

        private class FakeHttpContext : HttpContextBase
        {
            private Dictionary<object, object> _items = new Dictionary<object, object>();
            public override IDictionary Items { get { return _items; } }
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            private ViewDataDictionary _viewData = new ViewDataDictionary();
            public ViewDataDictionary ViewData { get { return _viewData; } set { _viewData = value; } }
        }


        private IRepository repository;
        public WTRController(IRepository repo)
            : this()
        {

            this.repository = repo;
        }
        //
        // GET: /WRT/
        [Authorize(Roles = "ABM")]
        public ActionResult GetWTR()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;

            ViewBag.FromText = new DateTime(currYear, currMonth, 01);
            ViewBag.ToText = new DateTime(currYear, currMonth, DateTime.DaysInMonth(currYear, currMonth));

            return View();
        }

        [Authorize(Roles = "EMP")]
        public ViewResult GetAbsencePerEMP()
        {
            return View();
        }

        [Authorize(Roles = "EMP")]
        public ViewResult GetWTRPerEMP()
        {
            int currYear = DateTime.Now.Year;
            int currMonth = DateTime.Now.Month;

            ViewBag.FromText = new DateTime(currYear, currMonth, 01);
            ViewBag.ToText = new DateTime(currYear, currMonth, DateTime.DaysInMonth(currYear, currMonth));

            return View();
        }

        [Authorize(Roles = "ABM")]
        public PartialViewResult GetWTRData(string From = "", string To = "", string searchString = "")
        {
            DateTime fromParsed = DateTime.Now;
            DateTime toParse = DateTime.Now;
            int FromYear = DateTime.Now.Year;
            int ToYear = DateTime.Now.Year;

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            if (From != "" && To != "")
            {
                try
                {
                    fromParsed = DateTime.ParseExact(From, "dd.MM.yyyy", null);
                    toParse = DateTime.ParseExact(To, "dd.MM.yyyy", null);
                    FromYear = fromParsed.Year;
                    ToYear = toParse.Year;
                }
                catch (System.FormatException)
                {
                    return PartialView("GetWTRDataEmpty");
                }
            }
            else
            {
                return PartialView("GetWTRDataEmpty");
            }
            if (fromParsed > toParse)
            {
                return PartialView("GetWTRDataEmpty");
            }

            searchString = searchString != "" ? searchString.Trim() : "";
            List<Employee> selectedData = new List<Employee>();
            List<WTRViewModel> wtrDataList = new List<WTRViewModel>();
            Dictionary<int, StartEndDatePair> timeSpanDatePairDict = GetWeeksInTimeSpan(fromParsed, toParse);
            Dictionary<int, StartEndDatePair> calendarItemDatePairDict = new Dictionary<int, StartEndDatePair>();
            Dictionary<int, StartEndDatePair> resultDatePairDict = new Dictionary<int, StartEndDatePair>();

            selectedData = SearchEmployeeData(fromParsed, toParse, repository.Employees.ToList(), searchString);

            foreach (var emp in selectedData)
            {
                WTRViewModel onePerson = new WTRViewModel { ID = emp.EID, FirstName = emp.FirstName, LastName = emp.LastName, FactorDetails = new List<FactorData>() };
                foreach (var calendarItems in emp.CalendarItems.Where((f => (f.From <= fromParsed && f.To >= fromParsed) || (f.From >= fromParsed && f.From <= toParse))))
                {
                    calendarItemDatePairDict = GetWeeksInTimeSpan(calendarItems.From, calendarItems.To);

                    resultDatePairDict = IntersectDatePairDicts(timeSpanDatePairDict, calendarItemDatePairDict);

                    foreach (int week in resultDatePairDict.Keys)
                    {
                        FactorData data = new FactorData();
                        data.Factor = calendarItems.Type;
                        data.Location = calendarItems.Location;
                        data.From = resultDatePairDict[week].startDate;
                        data.To = resultDatePairDict[week].endDate;
                        data.Hours = 0;
                        data.WeekNumber = week;
                        onePerson.FactorDetails.Add(data);
                    }
                }
                wtrDataList.Add(onePerson);

            }

            ViewBag.FromWeek = cal.GetWeekOfYear(fromParsed, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.ToWeek = cal.GetWeekOfYear(toParse, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.FromYear = FromYear;
            ViewBag.ToYear = ToYear;

            ViewBag.fromDate = From;
            ViewBag.toDate = To;

            return PartialView(wtrDataList);
        }

        public struct StartEndDatePair
        {
            public DateTime startDate;
            public DateTime endDate;
        }

        public Dictionary<int, StartEndDatePair> IntersectDatePairDicts(
            Dictionary<int, StartEndDatePair> firstDatePairDict,
            Dictionary<int, StartEndDatePair> secondDatePairDict)
        {
            DateTime from;
            DateTime to;
            Dictionary<int, StartEndDatePair> resultDataPairs = new Dictionary<int, StartEndDatePair>();

            foreach (int week in firstDatePairDict.Keys.Intersect(secondDatePairDict.Keys))
            {
                from = firstDatePairDict[week].startDate > secondDatePairDict[week].startDate ?
                    firstDatePairDict[week].startDate :
                    secondDatePairDict[week].startDate;
                to = firstDatePairDict[week].endDate < secondDatePairDict[week].endDate ?
                    firstDatePairDict[week].endDate :
                    secondDatePairDict[week].endDate;
                resultDataPairs.Add(week, new StartEndDatePair { startDate = from, endDate = to });
            }

            return resultDataPairs;
        }

        public Dictionary<int, StartEndDatePair> GetWeeksInTimeSpan(DateTime from, DateTime to)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            Dictionary<int, StartEndDatePair> weeks = new Dictionary<int, StartEndDatePair>();
            int weekNumber = 0;

            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                weekNumber = cal.GetWeekOfYear(day, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                if (weeks.Count == 0 || !weeks.Keys.Contains(weekNumber))
                {
                    weeks.Add(weekNumber, new StartEndDatePair { startDate = day, endDate = day });
                }
                else if (weeks.Keys.Contains(weekNumber))
                {
                    weeks[weekNumber] = new StartEndDatePair { startDate = weeks[weekNumber].startDate, endDate = day };
                }

            }

            return weeks;
        }

        public List<Employee> SearchEmployeeData(DateTime FromDate, DateTime ToDate, List<Employee> empList, string searchString = "")
        {
            List<Employee> query = (from emp in empList
                                    where ((emp.CalendarItems.Count != 0))
                                    from f in emp.CalendarItems
                                    where (((f.From <= FromDate && f.To >= FromDate) || (f.From >= FromDate && f.From <= ToDate)) &&
                                        ((emp.FirstName.ToLower().Contains(searchString.ToLower())) ||
                                        emp.LastName.ToLower().Contains(searchString.ToLower()) ||
                                        emp.EID.ToLower().Contains(searchString.ToLower())))
                                    orderby emp.DateEmployed, emp.LastName
                                    select emp).Distinct().ToList();
            return query;
        }


        [Authorize(Roles = "EMP")]
         public PartialViewResult GetWTRDataPerEMP(string From = "", string To = "", string userName = "")
        {
            DateTime fromParsed = DateTime.Now;
            DateTime toParse = DateTime.Now;
            int FromYear = DateTime.Now.Year;
            int ToYear = DateTime.Now.Year;

            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            if (From != "" && To != "")
            {
                try
                {
                    fromParsed = DateTime.ParseExact(From, "dd.MM.yyyy", null);
                    toParse = DateTime.ParseExact(To, "dd.MM.yyyy", null);
                    FromYear = fromParsed.Year;
                    ToYear = toParse.Year;
                }
                catch (System.FormatException)
                {
                    return PartialView("GetWTRDataEmpty");
                }
            }
            else
            {
                return PartialView("GetWTRDataEmpty");
            }
            if (fromParsed > toParse)
            {
                return PartialView("GetWTRDataEmpty");
            }

            List<WTRViewModel> wtrDataList = new List<WTRViewModel>();
            Employee employee = repository.Employees.Where(e => e.EID == userName).FirstOrDefault();

            if (employee == null)
            {
                return PartialView("NoData");
            }

            Dictionary<int, StartEndDatePair> timeSpanDatePairDict = GetWeeksInTimeSpan(fromParsed, toParse);
            Dictionary<int, StartEndDatePair> calendarItemDatePairDict = new Dictionary<int, StartEndDatePair>();
            Dictionary<int, StartEndDatePair> resultDatePairDict = new Dictionary<int, StartEndDatePair>();

            WTRViewModel onePerson = new WTRViewModel { ID = employee.EID, FirstName = employee.FirstName, LastName = employee.LastName, FactorDetails = new List<FactorData>() };
            foreach (var calendarItems in employee.CalendarItems.Where((f=> (f.From <= fromParsed && f.To >= fromParsed) || (f.From >= fromParsed && f.From <= toParse))))
            {
                calendarItemDatePairDict = GetWeeksInTimeSpan(calendarItems.From, calendarItems.To);
                resultDatePairDict = IntersectDatePairDicts(timeSpanDatePairDict, calendarItemDatePairDict);
                foreach (int week in resultDatePairDict.Keys)
                {
                    FactorData data = new FactorData();
                    data.Factor = calendarItems.Type;
                    data.Location = calendarItems.Location;
                    data.From = resultDatePairDict[week].startDate;
                    data.To = resultDatePairDict[week].endDate;
                    data.Hours = 0;
                    data.WeekNumber = week;
                    onePerson.FactorDetails.Add(data);
                }
            }
            wtrDataList.Add(onePerson);

            ViewBag.FromWeek = cal.GetWeekOfYear(fromParsed, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.ToWeek = cal.GetWeekOfYear(toParse, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            ViewBag.FromYear = FromYear;
            ViewBag.ToYear = ToYear;
            ViewBag.UserName = userName;
            ViewBag.fromDate = From;
            ViewBag.toDate = To;

            return PartialView("GetWTRDataPerEMP", wtrDataList);
        }

        [Authorize(Roles = "ABM")]
        public ActionResult ExportWTR(string searchString, string fromDate, string toDate)
        {
            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet("First Sheet");
            CreateCaption(workSheet);
            WriteWTRData(workSheet, searchString, fromDate, toDate);
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return File(stream.ToArray(), "application/vnd.ms-excel", "WTR.xls");
        }

        [Authorize(Roles = "ABM,EMP")]
        public void CreateCaption(Worksheet workSheet)
        {
            string[] caption = new string[] { "Employee", "Location", "Factor", "Dates", "Hours" };
            for (int i = 0; i < caption.Length; i++)
            {
                workSheet.Cells[0, i] = new Cell(caption[i]);
            }
            workSheet.Cells.ColumnWidth[0] = 6000;
            workSheet.Cells.ColumnWidth[1] = 3000;
            workSheet.Cells.ColumnWidth[2] = 6000;
            workSheet.Cells.ColumnWidth[3] = 6000;
            workSheet.Cells.ColumnWidth[4] = 3000;
        }

        [Authorize(Roles = "ABM")]
        public void WriteWTRData(Worksheet workSheet, string searchString, string fromDate, string toDate)
        {

            PartialViewResult wtrViewResult = GetWTRData(fromDate, toDate, searchString);
            List<WTRViewModel> wtrDataList = wtrViewResult.Model as List<WTRViewModel>;

            int FromYear = wtrViewResult.ViewBag.FromYear;
            int ToYear = wtrViewResult.ViewBag.ToYear;
            int FromWeek = wtrViewResult.ViewBag.FromWeek;
            int ToWeek = wtrViewResult.ViewBag.ToWeek;

            var vc = new ViewContext();
            var helper = new HtmlHelper(vc, new FakeViewDataContainer());

            var weekInterval = helper.CustomCreateWeekInterval(FromYear, ToYear, FromWeek, ToWeek);

            int count = 1;

            foreach (var weekInt in weekInterval)
            {
                for (int i = weekInt.weekFrom; i <= weekInt.weekTo; i++)
                {
                    var week = helper.CustomSelectEmployeeByWeek(wtrDataList, i, weekInt.year);
                    if (week.ToList().Count == 0)
                    {
                        workSheet.Cells[count, 0] = new Cell("");
                        workSheet.Cells[count + 1, 0] = new Cell(weekInt.year + "- W " + i);
                        workSheet.Cells[count + 2, 0] = new Cell("No absence data");
                        workSheet.Cells[count + 3, 0] = new Cell("");
                        count += 4;
                    }
                    else
                    {
                        workSheet.Cells[count, 0] = new Cell("");
                        workSheet.Cells[count + 1, 0] = new Cell(weekInt.year + "- W " + i);
                        workSheet.Cells[count + 2, 0] = new Cell("");
                        count += 3;
                    }

                    if (week.ToList().Count != 0)
                    {
                        foreach (var emp in week)
                        {
                            var SortedList = helper.CustomSortingFactorDataByStartDate(emp.FactorDetails);

                            workSheet.Cells[count, 0] = new Cell(emp.LastName + ' ' + emp.FirstName  + "(" + emp.ID + ")");


                            foreach (var factor in SortedList)
                            {
                                workSheet.Cells[count, 1] = new Cell(factor.Location);
                                workSheet.Cells[count, 2] = new Cell(factor.Factor.ToString());
                                workSheet.Cells[count, 3] = new Cell(factor.From.ToString(String.Format("dd.MM.yyyy")) + " - " + factor.To.ToString(String.Format("dd.MM.yyyy")));
                                workSheet.Cells[count, 4] = new Cell(factor.Hours);
                                count++;
                            }
                        }
                    }
                }
            }
        }

        [Authorize(Roles = "EMP")]
        public ActionResult ExportWTRForEMP(string searchString, string fromDate, string toDate)
        {
            Workbook workBook = new Workbook();
            Worksheet workSheet = new Worksheet("First Sheet");
            CreateCaption(workSheet);
            WriteWTRDataForEMP(workSheet, searchString, fromDate, toDate);
            workBook.Worksheets.Add(workSheet);
            MemoryStream stream = new MemoryStream();
            workBook.SaveToStream(stream);
            return File(stream.ToArray(), "application/vnd.ms-excel", "WTR.xls");
        }


        [Authorize(Roles = "EMP")]
        public void WriteWTRDataForEMP(Worksheet workSheet, string userName, string fromDate, string toDate)
        {

            PartialViewResult wtrViewResult = GetWTRDataPerEMP(fromDate, toDate, userName);
            List<WTRViewModel> wtrDataList = wtrViewResult.Model as List<WTRViewModel>;

            int FromYear = wtrViewResult.ViewBag.FromYear;
            int ToYear = wtrViewResult.ViewBag.ToYear;
            int FromWeek = wtrViewResult.ViewBag.FromWeek;
            int ToWeek = wtrViewResult.ViewBag.ToWeek;

            var vc = new ViewContext();
            var helper = new HtmlHelper(vc, new FakeViewDataContainer());

            var weekInterval = helper.CustomCreateWeekInterval(FromYear, ToYear, FromWeek, ToWeek);

            int count = 1;

            foreach (var weekInt in weekInterval)
            {
                for (int i = weekInt.weekFrom; i <= weekInt.weekTo; i++)
                {
                    var week = helper.CustomSelectEmployeeByWeek(wtrDataList, i, weekInt.year);
                    if (week.ToList().Count == 0)
                    {
                        workSheet.Cells[count, 0] = new Cell("");
                        workSheet.Cells[count + 1, 0] = new Cell(weekInt.year + "- W " + i);
                        workSheet.Cells[count + 2, 0] = new Cell("No absence data");
                        workSheet.Cells[count + 3, 0] = new Cell("");
                        count += 4;
                    }
                    else
                    {
                        workSheet.Cells[count, 0] = new Cell("");
                        workSheet.Cells[count + 1, 0] = new Cell(weekInt.year + "- W " + i);
                        workSheet.Cells[count + 2, 0] = new Cell("");
                        count += 3;
                    }

                    if (week.ToList().Count != 0)
                    {
                        foreach (var emp in week)
                        {
                            var SortedList = helper.CustomSortingFactorDataByStartDate(emp.FactorDetails);

                            workSheet.Cells[count, 0] = new Cell(emp.LastName + ' ' + emp.FirstName + "(" + emp.ID + ")");


                            foreach (var factor in SortedList)
                            {
                                workSheet.Cells[count, 1] = new Cell(factor.Location);
                                workSheet.Cells[count, 2] = new Cell(factor.Factor.ToString());
                                workSheet.Cells[count, 3] = new Cell(factor.From.ToString(String.Format("dd.MM.yyyy")) + " - " + factor.To.ToString(String.Format("dd.MM.yyyy")));
                                workSheet.Cells[count, 4] = new Cell(factor.Hours);
                                count++;
                            }
                        }
                    }
                }
            }
        }
    }
}
