using AjourBT.Models;
using PDFjet.NET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace AjourBT.Infrastructure
{
    public static class CalendarToPdfExporter
    {
        static MemoryStream pdfMemoryStream;
        static PDF pdf;
        static Font font;
        static List<List<Cell>> tableData = new List<List<Cell>>();
        static int headerRowsCount;
        static int fakeEmployeesCount;

        static CalendarToPdfExporter()
        {
            pdfMemoryStream = new MemoryStream();
            PDF pdf = new PDF(pdfMemoryStream);
            tableData = new List<List<Cell>>();
            font = new Font(pdf, CoreFont.HELVETICA);
            font.SetSize(7f);
            headerRowsCount = 5;
            fakeEmployeesCount = 1;
        }

        public static List<List<Cell>> CreateCalendar(List<CalendarRowViewModel> calendarData, DateTime from, DateTime to)
        {
            List<List<Cell>> calendarHeader = CreateCalendarHeader(from, to);
            List<List<Cell>> calendar = CreateCalendarLeftPanel(calendarData);
            List<List<Cell>> calendarbody = CreateCalendarBody(calendarData, from, to);

            for (int i = 0; i < headerRowsCount; i++)
            {
                calendar[i] = calendar[i].Concat(calendarHeader[i]).ToList();
            }

            for (int i = headerRowsCount; i < calendar.Count; i++)
            {
                calendar[i] = calendar[i].Concat(calendarbody[i-headerRowsCount]).ToList();
            }
            return calendar;

        }

        public static List<List<Cell>> CreateCalendarHeader(DateTime from, DateTime to)
        {
            List<List<Cell>> calendarTable = new List<List<Cell>>();
            if (from <= to)
            {
                calendarTable.Add(getYearRow(from, to));
                calendarTable.Add(getMonthRow(from, to));
                calendarTable.Add(getWeekRow(from, to));
                calendarTable.Add(getDaysRow(from, to));
                calendarTable.Add(getDaysOfWeekRow(from, to));
                alignTextInAllCellsByCenter(calendarTable);
            }
            return calendarTable;
        }

        public static List<List<Cell>> CreateCalendarBody(List<CalendarRowViewModel> calendarData, DateTime from, DateTime to)
        {
            List<List<Cell>> calendarTable;
            int colSpan;
            List<Cell> emptyRow = new List<Cell>();
            int colIndex;

             calendarTable = CreateEmptyCalendarBody(calendarData, from, to);

            for (int i = 0; i < calendarData.Count - fakeEmployeesCount; i++)
            {

                foreach (CalendarItemViewModel calendarItem in  calendarData[i].values)
                {
                    colSpan = (calendarItem.to.Date - calendarItem.from.Date).Days+1;
                    colIndex = GetColumnIndexForCalendarItem(calendarItem, from, to);
                    if (colSpan + colIndex > (to.Date - from.Date).Days)
                        colSpan = (to.Date - calendarItem.from.Date).Days +1;
                    calendarTable[i][colIndex].SetColSpan(colSpan);
                    calendarTable[i][colIndex].SetBgColor(Color.goldenrod);
                    //calendarTable[i][colIndex].SetText("s");

                }
            }

            return calendarTable;
        }

        public static int GetColumnIndexForCalendarItem(CalendarItemViewModel calendarItem, DateTime from, DateTime to) 
        {
            int colIndex; 
            colIndex = (calendarItem.from.Date - from.Date).Days;
            if (colIndex < 0 && calendarItem.to >= from.Date)
                        colIndex = 0;
            if (colIndex > (to.Date - from.Date).Days)
                colIndex = -1;
            //colIndex lesser than 0 means wrong input data
            return colIndex ;
        }

        public static int GetColumnSpanForCalendarItem(CalendarItemViewModel calendarItem, DateTime from, DateTime to)
        {
            int colSpan = 0;
            int colIndex = GetColumnIndexForCalendarItem(calendarItem, from, to);

            if (colIndex < 0)
                return -1;

                colSpan = (calendarItem.to.Date - calendarItem.from.Date).Days + 1;
            if (colSpan + colIndex > (to.Date - from.Date).Days)
                colSpan -= (calendarItem.to.Date - to.Date).Days;
            if (calendarItem.from.Date < from)
                colSpan -=   (from.Date - calendarItem.from.Date).Days;

            return colSpan;
        }

        public static List<List<Cell>> CreateEmptyCalendarBody(List<CalendarRowViewModel> calendarData, DateTime from, DateTime to)
        {
            List<List<Cell>> calendarTable = new List<List<Cell>>(); 
            if(calendarData!=null)
            for (int i = 0; i < calendarData.Count - fakeEmployeesCount; i++)
            {
                calendarTable.Add(new List<Cell>());
                for (DateTime date = from.Date; date <= to.Date; date = date.AddDays(1))
                {
                    calendarTable[i].Add(new Cell(font) {  });
                }
            }

            return calendarTable;
        }

        public static List<List<Cell>> CreateCalendarLeftPanel(List<CalendarRowViewModel> calendarData)
        {
            List<List<Cell>> calendarTable = new List<List<Cell>>();
            if (calendarData != null && calendarData.Count != 0)
            {
                CreateHeaderSpacer(calendarData, calendarTable);

                CreateEmployeesList(calendarData, calendarTable);
            }
            return calendarTable;

        }

        public static void CreateEmployeesList(List<CalendarRowViewModel> calendarData, List<List<Cell>> calendarTable)
        {
            if (calendarTable != null && calendarData != null)
            {
                for (int i = 0; i < calendarData.Count - fakeEmployeesCount; i++)
                {
                    calendarTable.Add(new List<Cell>() { new Cell(font, calendarData[i].name) });

                }
            }
        }

        public static void CreateHeaderSpacer(List<CalendarRowViewModel> calendarData, List<List<Cell>> calendarTable)
        {
            if (calendarTable != null && calendarData != null)
            {
                if (calendarData.Count != 0)
                {
                    for (int i = 0; i < headerRowsCount; i++)
                    {
                        calendarTable.Add(new List<Cell>() { new Cell(font) });
                        if (i != 0)
                            calendarTable[i][0].SetBorder(Border.TOP, false);
                        if (i != headerRowsCount - 1)
                            calendarTable[i][0].SetBorder(Border.BOTTOM, false);
                    }
                }
            }
        }


        public static List<Cell> getYearRow(DateTime from, DateTime to)
        {
            List<Cell> yearRow = new List<Cell>();
            Cell yearCell;
            while (from.Date <= to.Date)
            {
                yearCell = new Cell(font, from.Year.ToString());
                int colSpan = getColSpanForYear(from, to);
                yearCell.SetColSpan(colSpan);
                yearRow.Add(yearCell);
                for (int i = 1; i < colSpan; i++)
                {
                    yearRow.Add(new Cell(font));
                }
                from = new DateTime(from.Year + 1, 1, 1);
            }

            return yearRow;
        }

        public static int getColSpanForYear(DateTime from, DateTime to)
        {
            DateTime lastDate = new DateTime(from.Year + 1, 1, 1).AddDays(-1).Date;

            return getLesserOfTimeSpans(from, to, lastDate);
        }

        public static List<Cell> getMonthRow(DateTime from, DateTime to)
        {
            List<Cell> monthRow = new List<Cell>();
            Cell monthCell;
            DateTimeFormatInfo dfi = new DateTimeFormatInfo() { FirstDayOfWeek = DayOfWeek.Monday, CalendarWeekRule = CalendarWeekRule.FirstDay };

            while (from.Date <= to.Date)
            {
                monthCell = new Cell(font, dfi.GetMonthName(from.Month));
                int colSpan = getColSpanForMonth(from, to);
                monthCell.SetColSpan(colSpan);
                monthRow.Add(monthCell);
                for (int i = 1; i < colSpan; i++)
                {
                    monthRow.Add(new Cell(font));
                }
                DateTime lastDate = new DateTime(from.Year + (int)(from.Month / 12), from.Month % 12 + 1, 1);
                from = lastDate;
            }

            return monthRow;
        }

        public static int getColSpanForMonth(DateTime from, DateTime to)
        {
            DateTime lastDate = (new DateTime(from.Year + (int)(from.Month / 12), from.Month % 12 + 1, 1).AddDays(-1)).Date;

            return getLesserOfTimeSpans(from, to, lastDate);
        }

        public static List<Cell> getWeekRow(DateTime from, DateTime to)
        {
            List<Cell> weekRow = new List<Cell>();
            Cell weekCell;
            DateTimeFormatInfo dfi = new DateTimeFormatInfo() { FirstDayOfWeek = DayOfWeek.Monday, CalendarWeekRule = CalendarWeekRule.FirstDay };
            Calendar cal = dfi.Calendar;
            int weekNumber;
            int colSpan;

            while (from.Date <= to.Date)
            {
                colSpan = getColSpanForWeek(from, to);
                weekNumber = cal.GetWeekOfYear(from.AddDays(colSpan - 1).Date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
                weekCell = new Cell(font, "W" + weekNumber.ToString());
                weekCell.SetColSpan(colSpan);
                weekRow.Add(weekCell);
                for (int i = 1; i < colSpan; i++)
                {
                    weekRow.Add(new Cell(font));
                }

                from = from.AddDays(colSpan);
            }

            return weekRow;
        }

        public static List<Cell> getDaysRow(DateTime from, DateTime to)
        {
            List<Cell> daysRow = new List<Cell>();
            for (DateTime date = from; date <= to; date = date.AddDays(1))
            {
                daysRow.Add(new Cell(font, date.Day.ToString()));
            }
            return daysRow;
        }

        public static List<Cell> getDaysOfWeekRow(DateTime from, DateTime to)
        {
            List<Cell> daysOfWeekRow = new List<Cell>();

            DateTimeFormatInfo dfi = new DateTimeFormatInfo() { FirstDayOfWeek = DayOfWeek.Monday, CalendarWeekRule = CalendarWeekRule.FirstDay };

            for (DateTime date = from; date <= to; date = date.AddDays(1))
            {
                daysOfWeekRow.Add(new Cell(font, dfi.GetShortestDayName(date.DayOfWeek)));
            }
            return daysOfWeekRow;
        }

        public static int getColSpanForWeek(DateTime from, DateTime to)
        {
            DateTime lastDate = from.AddDays((7 - (int)from.DayOfWeek) % 7).Date;

            return getLesserOfTimeSpans(from, to, lastDate);
        }

        public static int getLesserOfTimeSpans(DateTime from, DateTime to, DateTime lastDate)
        {
            TimeSpan diff;
            if (to.Date < lastDate.Date)
                diff = to.Date.AddDays(1) - from.Date;
            else
                diff = lastDate.Date.AddDays(1) - from.Date;

            //Zero or less result means wrong input parameters (from is greater than to or lastDate)
            return diff.Days;
        }
        public static void alignTextInAllCellsByCenter(List<List<Cell>> table)
        {
            foreach (List<Cell> row in table)
            {
                foreach (Cell cell in row)
                {
                    cell.SetTextAlignment(Align.CENTER);
                }
            }
        }
    }
}