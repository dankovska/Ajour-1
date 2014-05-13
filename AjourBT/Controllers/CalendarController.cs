using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Models;
using PDFjet.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AjourBT.Infrastructure;

namespace AjourBT.Controllers
{
    public class CalendarController : Controller
    {
        private IRepository repository;
        public CalendarController(IRepository repo)
        {
            this.repository = repo;
        }
        //
        // GET: /Calendar/
        [Authorize(Roles = "ABM, VU")]
        public ViewResult GetCalendar(string selectedDepartment = null)
        {
            ViewBag.DepartmentDropDownList = DepartmentDropDownList();
            ViewBag.SelectedDepartment = selectedDepartment;
            return View();
        }

        public List<DateTime> GetHolidaysData()
        {
            var holidays = (from holiday in repository.Holidays
                            where (holiday.CountryID == 1 && holiday.IsPostponed == false)
                            orderby holiday.HolidayDate
                            select holiday.HolidayDate).ToList();
            return holidays;
        }

        public List<DateTime> GetPostponedHolidaysData()
        {
            var holidays = (from holiday in repository.Holidays
                            where (holiday.CountryID == 1 && holiday.IsPostponed == true)
                            orderby holiday.HolidayDate
                            select holiday.HolidayDate).ToList();
            return holidays;
        }
        public SelectList DepartmentDropDownList()
        {
            var depList = from dep in repository.Departments
                          orderby dep.DepartmentName
                          select dep;

            return new SelectList(depList, "DepartmentName", "DepartmentName");

        }

        [Authorize(Roles = "ABM, VU")]
        public PartialViewResult GetCalendarData(string calendarFromDate, string calendarToDate, string selectedDepartment = null)
        {
            DateTime parseFromDate, parseToDate;
            int currentYear = DateTime.Now.Year;

            if (calendarFromDate == "" && calendarToDate == "")
            {
                parseFromDate = new DateTime(currentYear, 01, 01);
                parseToDate = new DateTime(currentYear, 12, 31);
            }

            try
            {
                parseFromDate = DateTime.ParseExact(calendarFromDate, "dd.MM.yyyy", null);
                parseToDate = DateTime.ParseExact(calendarToDate, "dd.MM.yyyy", null);
            }
            catch (SystemException)            
            { 
                parseFromDate = new DateTime(currentYear,01,01);
                parseToDate = new DateTime(currentYear,12,31);
            }

            ViewBag.Holidays = GetHolidaysData();
            ViewBag.PostponedHolidays = GetPostponedHolidaysData();
            List<Employee> empList = SearchEmployeeData(selectedDepartment);
            List<CalendarRowViewModel> rowList = GetCalendarRowData(empList, parseFromDate, parseToDate);

            //var currentUser = HttpContext.User.Identity.Name;
            if (Request.UrlReferrer != null)
            {
                string myUrl = Request.UrlReferrer.OriginalString;
                if (myUrl.Contains("ABMView"))
                {
                    ViewBag.ItemsPerPage = empList.Count +  1; //+1 for fake row  
                    return PartialView(rowList);
                }
            }
            ViewBag.ItemsPerPage = empList.Count + 1;
            return PartialView("GetCalendarDataVU", rowList);
        }

        public List<Employee> SearchEmployeeData(string selectedDepartment)
        {
            List<Employee> searchList = (from emp in repository.Employees
                                         where emp.DateDismissed == null
                                         join dep in repository.Departments
                                         on emp.DepartmentID equals dep.DepartmentID
                                         where (selectedDepartment == null || selectedDepartment == String.Empty || dep.DepartmentName == selectedDepartment)

                                         orderby emp.LastName ascending
                                         select emp).ToList();
            return searchList;
        }

        public List<CalendarRowViewModel> GetCalendarRowData(List<Employee> empList, DateTime fromDate, DateTime toDate)
        {

            List<CalendarRowViewModel> calendarDataList = new List<CalendarRowViewModel>();

            foreach (var emp in empList)
            {
                CalendarRowViewModel oneRow = new CalendarRowViewModel { id = emp.EmployeeID.ToString(), name = emp.LastName + " " + emp.FirstName, desc = "  ", values = new List<CalendarItemViewModel>() };

                foreach (var calendarItem in emp.CalendarItems)
                {
                    if ((calendarItem.From <= fromDate && calendarItem.To >= fromDate) || (calendarItem.From >= fromDate && calendarItem.From <= toDate))
                    {
                        CalendarItemViewModel oneItem = new CalendarItemViewModel(calendarItem);
                        oneItem.desc += String.Format(" From: {0} - To: {1}", calendarItem.From.ToShortDateString(), calendarItem.To.ToShortDateString());
                        if (calendarItem.Type == CalendarItemType.SickAbsence)
                        {
                            Sickness selectedSick = (from s in repository.Sicknesses where ((s.From == calendarItem.From) && (s.To == calendarItem.To) && s.EmployeeID == calendarItem.EmployeeID) select s).FirstOrDefault();
                            if (selectedSick != null)
                                oneItem.sickType = selectedSick.SicknessType;
                        }

                        oneRow.values.Add(oneItem);
                    }
                }


                calendarDataList.Add(oneRow);
            }



            List<CalendarRowViewModel> result = InsertFakeEmployee(calendarDataList, fromDate , toDate);
            return result;
            //return calendarDataList;
        }

        public List<CalendarRowViewModel> InsertFakeEmployee(List<CalendarRowViewModel> dataList, DateTime from, DateTime to)
        {
            DateTime present = from;
            DateTime yearly = to;

            CalendarRowViewModel fakeRow = new CalendarRowViewModel
            {
                id = "fake_row",
                name = " ",
                desc = " ",
                values = new List<CalendarItemViewModel>() { new CalendarItemViewModel{ id = 1, from = present, to = present, customClass = "ganttWhite", desc = "" },
                                                             new CalendarItemViewModel{ id = 1, from = yearly, to = yearly ,customClass = "ganttWhite", desc = ""} }
            };

            //var isValue = (from item in dataList where item.values.Count != 0 select item).FirstOrDefault();
            //if (isValue == null)
            //{
            dataList.Add(fakeRow);
            //}

            return dataList;
        }

        public ActionResult printCalendarToPdf()
        {
            MemoryStream pdfMemoryStream = new MemoryStream();
            PDF pdf = new PDF(pdfMemoryStream);
            ////Font f1 = new Font(pdf, CoreFont.HELVETICA_BOLD);
            ////Font f2 = new Font(pdf, CoreFont.HELVETICA);
            ////f1.SetSize(7f);
            ////f2.SetSize(7f);

            ////List<List<Cell>> tableData = new List<List<Cell>>();
            ////    List<Cell> row = new List<Cell>(); 
            ////DateTime date = new DateTime(DateTime.Now.Date.Year, 1, 1);
            ////DateTime lastdate = new DateTime(date.Year+1, 1, 1);
            ////TimeSpan diff = lastdate - date;

            ////string day = String.Empty; 
            ////    for (int i = 0; i < diff.TotalDays; i++)
            ////    {
            ////        day = i.ToString();
            ////        if (i < 10)
            ////            day=day.Insert(0, "  ");
            ////        else if (i < 100)
            ////           day=day.Insert(0, " ");

            ////        Cell cell = new Cell(f2, day);

            ////        // WITH:
            ////        cell.SetTopPadding(2f);
            ////        cell.SetBottomPadding(2f);
            ////        cell.SetLeftPadding(2f);
            ////        cell.SetRightPadding(2f);

            ////        row.Add(cell);
            ////    }
            ////    tableData.Add(row);

            Table table = new Table();
            //table.SetData(CalendarToPdfExporter.CreateCalendarHeader(new DateTime(2011, 12, 29), new DateTime(2013, 01, 02)), 1); 
            //table.SetData(CalendarToPdfExporter.CreateCalendarLeftPanel(GetCalendarRowData(repository.Employees.ToList(), new DateTime(2011, 12, 29), new DateTime(2014, 01, 02))));
            table.SetData(CalendarToPdfExporter.CreateCalendar(GetCalendarRowData(repository.Employees.ToList(), new DateTime(2012, 12, 31), new DateTime(2014, 1, 1)), new DateTime(2012, 12, 31), new DateTime(2014, 1, 1)));
            //table.SetLocation(100f, 50f);

            // REPLACED:
            // table.SetCellMargin(2f);

            ////table.RemoveLineBetweenRows(0, 1);

            //Cell cell3 = table.GetCellAt(1, 1);
            //cell3.SetBorder(Border.TOP, true);

            //cell3 = table.GetCellAt(1, 2);
            //cell3.SetBorder(Border.TOP, true);

            //SetFontForRow(table, 0, f1);
            //SetFontForRow(table, 1, f1);

            table.AutoAdjustColumnWidths();

            //List<Cell> column = table.GetColumn(7);
            //for (int i = 0; i < column.Count; i++)
            //{
            //    Cell cell = column[i];
            //    cell.SetTextAlignment(Align.CENTER);
            //}

            //column = table.GetColumn(4);
            //for (int i = 2; i < column.Count; i++)
            //{
            //    Cell cell = column[i];
            //    try
            //    {
            //        cell.SetTextAlignment(Align.CENTER);
            //        if (Int32.Parse(cell.GetText()) > 40)
            //        {
            //            cell.SetBgColor(Color.darkseagreen);
            //        }
            //        else
            //        {
            //            cell.SetBgColor(Color.yellow);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //    }
            //}

            //Cell cell2 = table.GetCellAt(0, 1);
            //cell2.SetColSpan(2);
            //cell2.SetTextAlignment(Align.CENTER);

              //SetBgColorForRow(table, 0, Color.lightgray);
            //SetBgColorForRow(table, 1, Color.lightgray);

            //table.SetColumnWidth(3, 10);
            //blankOutColumn(table, 3);

            //table.SetColumnWidth(8, 10f);
            //blankOutColumn(table, 8);

            Page page = new Page(pdf, A0.LANDSCAPE);
            //table.AutoAdjustColumnWidths();
            //int numOfPages = table.GetNumberOfPages(page);
            int pageNumber = 1; 
            while (true)
            {
                table.DrawOn(page);
                ////TextLine text = new TextLine(f1);
                ////text.SetText("Page " + pageNumber++ + " of " + numOfPages);
                ////text.SetLocation(300f, 780f);
                ////text.DrawOn(page);

                if (!table.HasMoreData())
                {
                    table.ResetRenderedPagesCount();
                    break;
                }

                page = new Page(pdf, A0.LANDSCAPE);
            }

            pdf.Close();

            return File(pdfMemoryStream.ToArray(), "application/pdf", "Calendar.pdf");
        }

        public List<List<Cell>> generateCalendarHeader(DateTime from, DateTime to)
        {
            List<List<Cell>> tableData = new List<List<Cell>>();
            List<Cell> row = new List<Cell>();



            return tableData;
        }

        

        public class A0
        {
            public static float[] PORTRAIT = new float[] { 3368.0f, 7146.0f };
            public static float[] LANDSCAPE = new float[] { 7146.0f, 3368.0f };
        }

        public void blankOutColumn(Table table, int index)
        {
            List<Cell> column = table.GetColumn(index);
            for (int i = 0; i < column.Count; i++)
            {
                Cell cell = column[i];
                cell.SetBgColor(Color.white);
                cell.SetBorder(Border.TOP, false);
                cell.SetBorder(Border.BOTTOM, false);
            }
        }


        public void SetBgColorForRow(Table table, int index, int color)
        {
            List<Cell> row = table.GetRow(index);
            for (int i = 0; i < row.Count; i++)
            {
                Cell cell = row[i];
                cell.SetBgColor(color);
            }
        }


        public void SetFontForRow(Table table, int index, Font font)
        {
            List<Cell> row = table.GetRow(index);
            for (int i = 0; i < row.Count; i++)
            {
                row[i].SetFont(font);
            }
        }

    }
}
