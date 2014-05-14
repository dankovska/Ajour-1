using AjourBT.Domain.Entities;
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
        static int daysOfWeekRowNumber;
        static int daysRowNumber;
        static int weeksRowNumber;
        static int monthesRowNumber;

        public static class PdfColors
        {
            public const int ganttGreen = 0x9acd32;
            public const int ganttDarkGreen = 0x78a407;
            public const int ganttOrange = 0xff7f00;
            public const int ganttBlue = 0x64c8fa;
            public const int ganttViolet = 0x5a009d;
            public const int ganttRed = 0xff0000;
            public const int ganttMagneta = 0xff00ff;
            public const int ganttYellow = 0xffd800;
            public const int ganttWhite = 0xffffff;

            public const int holidayYellow = 0xfff3b3;
            public const int holidayPink = 0xffe5e5;

            public const int todayGreen = 0xe4f2bf;
            public const int holidayOrange = 0xffd265;

            public const int headerWeekYellow = 0xfff3b3;

            public const int headerMonthBlue = 0xe3ffff;

        }

        static CalendarToPdfExporter()
        {
            pdfMemoryStream = new MemoryStream();
            PDF pdf = new PDF(pdfMemoryStream);
            tableData = new List<List<Cell>>();
            font = new Font(pdf, CoreFont.HELVETICA);
            font.SetSize(7f);
            headerRowsCount = 5;
            fakeEmployeesCount = 1;
            daysOfWeekRowNumber = 4;
            daysRowNumber = 3;
            weeksRowNumber = 2;
            monthesRowNumber = 1;
        }

        public static List<List<Cell>> CreateCalendar(List<CalendarRowViewModel> calendarData, List<Holiday> holidays, DateTime from, DateTime to)
        {
            List<List<Cell>> calendarHeader = CreateCalendarHeader(from, to);
            List<List<Cell>> calendar = CreateCalendarLeftPanel(calendarData);
            List<List<Cell>> calendarbody = CreateCalendarBody(calendarData, holidays, from, to);

            for (int i = 0; i < headerRowsCount; i++)
            {
                calendar[i] = calendar[i].Concat(calendarHeader[i]).ToList();
            }

            for (int i = headerRowsCount; i < calendar.Count; i++)
            {
                calendar[i] = calendar[i].Concat(calendarbody[i - headerRowsCount]).ToList();
            }

            ColorHeader(calendar, holidays, from, to);


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

        public static List<List<Cell>> CreateCalendarBody(List<CalendarRowViewModel> calendarData, List<Holiday> holidays, DateTime from, DateTime to)
        {
            List<List<Cell>> calendarTable;
            int colSpan;
            List<Cell> emptyRow = new List<Cell>();
            int colIndex;

            calendarTable = CreateEmptyCalendarBody(calendarData, from, to);
            ApplyWeekends(calendarTable, from, to);
            ApplyHolidays(calendarTable, holidays, from, to);

            for (int i = 0; i < calendarData.Count - fakeEmployeesCount; i++)
            {

                foreach (CalendarItemViewModel calendarItem in calendarData[i].values)
                {
                    colSpan = (calendarItem.to.Date - calendarItem.from.Date).Days + 1;
                    colIndex = GetColumnIndexForCalendarItem(calendarItem, from, to);
                    if (colSpan + colIndex > (to.Date - from.Date).Days)
                        colSpan = (to.Date - calendarItem.from.Date).Days + 1;
                    calendarTable[i][colIndex].SetColSpan(colSpan);
                    calendarTable[i][colIndex].SetBgColor(GetColorForCalendarItem(calendarItem));
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
            return colIndex;
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
                colSpan -= (from.Date - calendarItem.from.Date).Days;

            return colSpan;
        }

        public static int GetColorForCalendarItem(CalendarItemViewModel calendaritem)
        {

            switch (calendaritem.customClass)
            {
                case "ganttGreen":
                    return PdfColors.ganttGreen;

                case "ganttDarkGreen":
                    return PdfColors.ganttDarkGreen;

                case "ganttOrange":
                    return PdfColors.ganttOrange;

                case "ganttBlue":
                    return PdfColors.ganttBlue;

                case "ganttViolet":
                    return PdfColors.ganttViolet;

                case "ganttRed":
                    return PdfColors.ganttRed;

                case "ganttMagneta":
                    return PdfColors.ganttMagneta;

                case "ganttYellow":
                    return PdfColors.ganttYellow;

                default:
                    return PdfColors.ganttWhite;
            }

        }

        public static void ApplyHolidaysToHeader(List<List<Cell>> calendar, List<Holiday> holidays, DateTime from, DateTime to)
        {
            if (calendar != null && holidays != null && calendar.Count != 0 && (to - from).Days +2 == calendar[0].Count)
            {
                foreach (Holiday holiday in holidays)
                {
                    if (holiday.HolidayDate.Date >= from.Date && holiday.HolidayDate.Date <= to.Date)
                    {
                            calendar[daysRowNumber][(holiday.HolidayDate.Date - from.Date).Days + 1].SetBgColor(PdfColors.holidayOrange);
                            calendar[daysOfWeekRowNumber][(holiday.HolidayDate.Date - from.Date).Days + 1].SetBgColor(PdfColors.holidayOrange);
                    }
                }
            }
        }

        public static void ColorHeader(List<List<Cell>> calendar, List<Holiday> holidays, DateTime from, DateTime to)
        {
            ColorRow(calendar, weeksRowNumber, PdfColors.headerWeekYellow);
            ColorRow(calendar, monthesRowNumber, PdfColors.headerMonthBlue);
            ApplyHolidaysToHeader(calendar, holidays, from, to) ;
        }

        public static void ColorRow(List<List<Cell>> calendar, int rowNumber, int color)
        {
            if (calendar != null && calendar.Count >= rowNumber)
            {
                for (int i = 1; i< calendar[0].Count(); i++)
                {
                    calendar[rowNumber][i].SetBgColor(color);
                }
            }
        }

        public static void ApplyWeekends(List<List<Cell>> calendar, DateTime from, DateTime to)
        {
            if (calendar != null && calendar.Count != 0 && (to - from).Days + 1 == calendar[0].Count)
                for (DateTime date = from; date <= to; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        foreach (List<Cell> row in calendar)
                        {
                            row[(date - from).Days].SetBgColor(PdfColors.holidayPink);
                        }
                    }
                }
        }

        public static void ApplyHolidays(List<List<Cell>> calendar, List<Holiday> holidays, DateTime from, DateTime to)
        {
            int color;
            if (calendar != null && holidays != null && calendar.Count != 0 && (to - from).Days + 1 == calendar[0].Count)
            {
                foreach (Holiday holiday in holidays)
                {
                    if (holiday.HolidayDate.Date >= from.Date && holiday.HolidayDate.Date <= to.Date)
                    {
                        if (holiday.IsPostponed)
                            color = PdfColors.holidayPink;
                        else
                            color = PdfColors.holidayYellow;
                        foreach (List<Cell> row in calendar)
                        {

                            row[(holiday.HolidayDate.Date - from.Date).Days].SetBgColor(color);
                        }
                    }
                }
            }
        }

        public static List<List<Cell>> CreateEmptyCalendarBody(List<CalendarRowViewModel> calendarData, DateTime from, DateTime to)
        {
            List<List<Cell>> calendarTable = new List<List<Cell>>();
            if (calendarData != null)
                for (int i = 0; i < calendarData.Count - fakeEmployeesCount; i++)
                {
                    calendarTable.Add(new List<Cell>());
                    for (DateTime date = from.Date; date <= to.Date; date = date.AddDays(1))
                    {
                        calendarTable[i].Add(new Cell(font) { });
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