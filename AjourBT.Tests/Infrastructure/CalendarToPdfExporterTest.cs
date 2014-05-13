using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using AjourBT.Domain.Entities;
using AjourBT.Infrastructure;
using PDFjet.NET;
using AjourBT.Models;

namespace AjourBT.Tests.Infrastructure
{
    [TestFixture]
    public class CalendarToPdfExporterTest
    {
        List<CalendarRowViewModel> calendarModel;
        List<List<Cell>> calendarTable;
        List<CalendarRowViewModel> calendarModelForColIndexAndColSpan;

        [SetUp]
        public void SetUpFixture()
        {
            calendarModel = new List<CalendarRowViewModel>();
            for (int i = 0; i < 6; i++)
            {
                calendarModel.Add(new CalendarRowViewModel());
            }
            calendarModel[0].id = "1";
            calendarModel[0].name = "Rose Johnny";
            calendarModel[1].id = "2";
            calendarModel[1].name = "Cruz Norma";
            calendarModel[2].id = "3";
            calendarModel[2].name = "George Anatoliy";
            calendarModel[3].id = "4";
            calendarModel[3].name = "Wood Harold";
            calendarModel[4].id = "5";
            calendarModel[4].name = "Olson Wayne";
            calendarModel[5].id = "6";
            calendarModel[5].name = "";

            calendarTable = new List<List<Cell>>(); 
                        
        }

        #region GetLesserOfTimeSpans

        [Test]
        public void GetlesserOfTimeSpans_ToDateIsGreaterThanLastDate_LastDateMinusFromDate()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2013, 01, 09);
            DateTime lastDate = new DateTime(2013, 01, 08);

            //Act
            int result = CalendarToPdfExporter.getLesserOfTimeSpans(from, to, lastDate);

            //Assert
            Assert.AreEqual(9, result);
        }

        [Test]
        public void GetlesserOfTimeSpans_LastDateIsGreaterThanToDate_LastDateMinusFromDate()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2013, 01, 05);
            DateTime lastDate = new DateTime(2013, 01, 08);

            //Act
            int result = CalendarToPdfExporter.getLesserOfTimeSpans(from, to, lastDate);

            //Assert
            Assert.AreEqual(6, result);
        }

        [Test]
        public void GetlesserOfTimeSpans_DatesAreEqual_LastDateMinusFromDate()
        {
            //Arrange
            DateTime from = new DateTime(2013, 01, 05);
            DateTime to = new DateTime(2013, 01, 05);
            DateTime lastDate = new DateTime(2013, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getLesserOfTimeSpans(from, to, lastDate);

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetlesserOfTimeSpans_FromDateIsGreaterThanStartDate_LastDateMinusFromDateNegativeValue()
        {
            //Arrange
            DateTime from = new DateTime(2013, 01, 07);
            DateTime to = new DateTime(2013, 01, 05);
            DateTime lastDate = new DateTime(2013, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getLesserOfTimeSpans(from, to, lastDate);

            //Assert
            Assert.AreEqual(-1, result);
        }

        #endregion

        #region GetColSpanForYear

        [Test]
        public void GetColSpanForYear_ToIsGreaterThanEndOfYear_TimespanUpToEndOfYear()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2014, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(11, result);
        }

        [Test]
        public void GetColSpanForYear_ToIsEqualToTheEndOfYear_TimespanUpToEndOfYear()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 31);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(11, result);
        }

        [Test]
        public void GetColSpanForYear_ToIsLesserThanEndOfYear_TimespanToDateTo()
        {
            //Arrange
            DateTime from = new DateTime(2013, 01, 01);
            DateTime to = new DateTime(2013, 12, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(355, result);
        }

        [Test]
        public void GetColSpanForYear_ToIsEqualToFrom_1()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetColSpanForYear_ToIsLesserThanFrom_ZeroOrNegativeValue()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForYear(from, to);

            //Assert
            Assert.AreEqual(0, result);
        }

        #endregion

        #region GetColSpanForMonth

        [Test]
        public void GetColSpanForMonth_ToIsGreaterThanEndOfMonth_TimespanUpToEndOfMonth()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2014, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(11, result);

        }

        [Test]
        public void GetColSpanForMonth_ToIsEqualToTheEndOfMonth_TimespanUpToEndOfMonth()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 31);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(11, result);
        }

        [Test]
        public void GetColSpanForMonth_ToIsLesserThanEndOfMonth_TimespanToDateTo()
        {
            //Arrange
            DateTime from = new DateTime(2013, 01, 01);
            DateTime to = new DateTime(2013, 12, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(31, result);
        }

        [Test]
        public void GetColSpanForMonth_ToIsEqualToFrom_1()
        {
            //Arrange
            DateTime from = new DateTime(2013, 11, 21);
            DateTime to = new DateTime(2013, 11, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetColSpanForMonth_ToIsLesserThanFrom_ZeroOrNegativeValue()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForMonth(from, to);

            //Assert
            Assert.AreEqual(0, result);
        }

        #endregion

        #region GetColSpanForWeek

        [Test]
        public void GetColSpanForWeek_ToIsGreaterThanEndOfWeek_TimespanUpToEndOfWeek()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2014, 01, 05);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void GetColSpanForWeek_ToIsEqualToTheEndOfWeek_TimespanUpToEndOfWeek()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 22);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(2, result);
        }

        [Test]
        public void GetColSpanForWeek_ToIsLesserThanEndOfWeek_TimespanToDateTo()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2013, 01, 02);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(3, result);
        }

        [Test]
        public void GetColSpanForWeek_ToIsEqualToFrom_1()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 21);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GetColSpanForWeek_ToIsLesserThanFrom_ZeroOrNegativeValue()
        {
            //Arrange
            DateTime from = new DateTime(2013, 12, 21);
            DateTime to = new DateTime(2013, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(0, result);
        }

        [Test]
        public void GetColSpanForWeek_FromIsMonday_ToIsGreaterThanEndOfWeek()
        {
            //Arrange
            DateTime from = new DateTime(2014, 05, 05);
            DateTime to = new DateTime(2014, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(7, result);
        }

        [Test]
        public void GetColSpanForWeek_FromIsSunday_ToIsGreaterThanEndOfWeek()
        {
            //Arrange
            DateTime from = new DateTime(2014, 05, 04);
            DateTime to = new DateTime(2014, 12, 20);

            //Act
            int result = CalendarToPdfExporter.getColSpanForWeek(from, to);

            //Assert
            Assert.AreEqual(1, result);
        }

        #endregion

        #region CreateCalendarHeader

        [Test]
        public void CreateCalendarheader_ProperDates_ProperHeader()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2014, 1, 1);

            //Act
            System.Collections.Generic.List<System.Collections.Generic.List<Cell>> result = CalendarToPdfExporter.CreateCalendarHeader(from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(734, result[0].Count());
            Assert.AreEqual("2011", result[0][0].GetText());
            Assert.AreEqual("2012", result[0][2].GetText());
            Assert.AreEqual("2013", result[0][368].GetText());
            Assert.AreEqual("2014", result[0][733].GetText());
            Assert.AreEqual(2, result[0][0].GetColSpan());
            Assert.AreEqual(366, result[0][2].GetColSpan());
            Assert.AreEqual(365, result[0][368].GetColSpan());
            Assert.AreEqual(1, result[0][733].GetColSpan());
            Assert.AreEqual(734, result[1].Count);
            Assert.AreEqual("December", result[1][0].GetText());
            Assert.AreEqual("January", result[1][733].GetText());
            Assert.AreEqual(734, result[2].Count);
            Assert.AreEqual("W1", result[2][0].GetText());
            Assert.AreEqual("W1", result[2][731].GetText());
            Assert.AreEqual(734, result[3].Count);
            Assert.AreEqual("30", result[3][0].GetText());
            Assert.AreEqual("1", result[3][733].GetText());
            Assert.AreEqual(734, result[4].Count);
            Assert.AreEqual("Fr", result[4][0].GetText());
            Assert.AreEqual("We", result[4][733].GetText());
            Assert.AreEqual(result[0][0].GetTextAlignment(), Align.CENTER);
            Assert.AreEqual(result[4][733].GetTextAlignment(), Align.CENTER);

        }

        [Test]
        public void CreateCalendarheader_EqualDates_ProperHeader()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 30);

            //Act
            System.Collections.Generic.List<System.Collections.Generic.List<Cell>> result = CalendarToPdfExporter.CreateCalendarHeader(from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(1, result[0].Count);
            Assert.AreEqual("2011", result[0][0].GetText());
            Assert.AreEqual(1, result[0][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[0][0].GetTextAlignment());
            Assert.AreEqual(1, result[1].Count);
            Assert.AreEqual("December", result[1][0].GetText());
            Assert.AreEqual(1, result[1][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[1][0].GetTextAlignment());
            Assert.AreEqual(1, result[2].Count);
            Assert.AreEqual("W53", result[2][0].GetText());
            Assert.AreEqual(1, result[2][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[2][0].GetTextAlignment());
            Assert.AreEqual(1, result[3].Count);
            Assert.AreEqual("30", result[3][0].GetText());
            Assert.AreEqual(1, result[3][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[3][0].GetTextAlignment());
            Assert.AreEqual(1, result[4].Count);
            Assert.AreEqual("Fr", result[4][0].GetText());
            Assert.AreEqual(1, result[3][0].GetColSpan());
            Assert.AreEqual(Align.CENTER, result[4][0].GetTextAlignment());

        }

        [Test]
        public void CreateCalendarheader_FromGreaterThanTo_EmptyHeader()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 31);
            DateTime to = new DateTime(2011, 12, 30);

            //Act
            System.Collections.Generic.List<System.Collections.Generic.List<Cell>> result = CalendarToPdfExporter.CreateCalendarHeader(from, to);

            //Assert        
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region getYearRow

        [Test]
        public void getYearRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2014, 1, 1);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getYearRow(from, to);

            //Assert
            Assert.AreEqual(734, result.Count);
            Assert.AreEqual("2011", result[0].GetText());
            Assert.AreEqual("2012", result[2].GetText());
            Assert.AreEqual("2013", result[368].GetText());
            Assert.AreEqual("2014", result[733].GetText());
            Assert.AreEqual(2, result[0].GetColSpan());
            Assert.AreEqual(366, result[2].GetColSpan());
            Assert.AreEqual(365, result[368].GetColSpan());
            Assert.AreEqual(1, result[733].GetColSpan());

        }

        [Test]
        public void getYearRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 30);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getYearRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("2011", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getYearRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getYearRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region getMonthRow

        [Test]
        public void getMonthRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2013, 1, 31);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getMonthRow(from, to);

            //Assert
            Assert.AreEqual(399, result.Count);
            Assert.AreEqual("December", result[0].GetText());
            Assert.AreEqual("February", result[33].GetText());
            Assert.AreEqual("December", result[337].GetText());
            Assert.AreEqual("January", result[368].GetText());
            Assert.AreEqual(2, result[0].GetColSpan());
            Assert.AreEqual(29, result[33].GetColSpan());
            Assert.AreEqual(31, result[337].GetColSpan());
            Assert.AreEqual(31, result[368].GetColSpan());

        }

        [Test]
        public void getMonthRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 30);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getMonthRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("December", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getMonthRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getMonthRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }


        #endregion

        #region getWeekRow

        [Test]
        public void getWeekRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 31);
            DateTime to = new DateTime(2013, 1, 23);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getWeekRow(from, to);

            //Assert
            Assert.AreEqual(390, result.Count);
            Assert.AreEqual("W1", result[0].GetText());
            Assert.AreEqual("W2", result[2].GetText());
            Assert.AreEqual("W3", result[380].GetText());
            Assert.AreEqual("W4", result[387].GetText());
            Assert.AreEqual(2, result[0].GetColSpan());
            Assert.AreEqual(7, result[2].GetColSpan());
            Assert.AreEqual(7, result[380].GetColSpan());
            Assert.AreEqual(3, result[387].GetColSpan());

        }

        [Test]
        public void getWeekRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2012, 12, 31);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getWeekRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("W54", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getWeekRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getWeekRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }


        #endregion

        #region getDaysRow

        [Test]
        public void getDaysRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 31);
            DateTime to = new DateTime(2013, 1, 23);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysRow(from, to);

            //Assert
            Assert.AreEqual(390, result.Count);
            Assert.AreEqual("31", result[0].GetText());
            Assert.AreEqual("2", result[2].GetText());
            Assert.AreEqual("14", result[380].GetText());
            Assert.AreEqual("21", result[387].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());
            Assert.AreEqual(1, result[2].GetColSpan());
            Assert.AreEqual(1, result[380].GetColSpan());
            Assert.AreEqual(1, result[387].GetColSpan());

        }

        [Test]
        public void getDaysRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2012, 12, 31);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("31", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getDaysRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region getDaysOfWeekRow

        [Test]
        public void getDaysOfWeekRow_ProperDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 31);
            DateTime to = new DateTime(2013, 1, 23);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysOfWeekRow(from, to);

            //Assert
            Assert.AreEqual(390, result.Count);
            Assert.AreEqual("Sa", result[0].GetText());
            Assert.AreEqual("Su", result[1].GetText());
            Assert.AreEqual("Mo", result[2].GetText());
            Assert.AreEqual("Tu", result[3].GetText());
            Assert.AreEqual("We", result[4].GetText());
            Assert.AreEqual("Th", result[5].GetText());
            Assert.AreEqual("Fr", result[6].GetText());
            Assert.AreEqual("Sa", result[7].GetText());
            Assert.AreEqual("Mo", result[380].GetText());
            Assert.AreEqual("Mo", result[387].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());
            Assert.AreEqual(1, result[1].GetColSpan());
            Assert.AreEqual(1, result[2].GetColSpan());
            Assert.AreEqual(1, result[3].GetColSpan());
            Assert.AreEqual(1, result[4].GetColSpan());
            Assert.AreEqual(1, result[5].GetColSpan());
            Assert.AreEqual(1, result[6].GetColSpan());
            Assert.AreEqual(1, result[7].GetColSpan());
            Assert.AreEqual(1, result[380].GetColSpan());
            Assert.AreEqual(1, result[387].GetColSpan());

        }

        [Test]
        public void getDaysOfWeekRow_EqualDates_ProperRow()
        {
            //Arrange
            DateTime from = new DateTime(2012, 12, 31);
            DateTime to = new DateTime(2012, 12, 31);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysOfWeekRow(from, to);

            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Mo", result[0].GetText());
            Assert.AreEqual(1, result[0].GetColSpan());

        }

        [Test]
        public void getDaysOfWeekRow_FromGreaterThanTo_EmptyRow()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2011, 12, 29);

            //Act 
            List<Cell> result = CalendarToPdfExporter.getDaysOfWeekRow(from, to);

            //Assert
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region CreateCalendarLeftPanel

        [Test]
        public void CreateCalendarLeftPanel_CalendarModelIsNotEmpty_ProperLeftPanel()
        {

            //Arrange

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarLeftPanel(calendarModel);

            //Assert        

            Assert.AreEqual(10, result.Count);
            Assert.AreEqual(true, result[0][0].GetBorder(Border.TOP));
            Assert.AreEqual(false, result[0][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual(false, result[2][0].GetBorder(Border.TOP));
            Assert.AreEqual(false, result[2][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual(false, result[4][0].GetBorder(Border.TOP));
            Assert.AreEqual(true, result[4][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual("Rose Johnny", result[5][0].GetText());
            Assert.AreEqual("Olson Wayne", result[9][0].GetText());

        }

        [Test]
        public void CreateCalendarLeftPanel_CalendarModelIsEmpty_EmptyLeftPanel()
        {

            //Arrange

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarLeftPanel(new List<CalendarRowViewModel>());

            //Assert        

            Assert.AreEqual(0, result.Count);

        }

        [Test]
        public void CreateCalendarLeftPanel_CalendarModelIsNull_EmptyLeftPanel()
        {

            //Arrange

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateCalendarLeftPanel(null);

            //Assert        

            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region CreateHeaderSpacer

        [Test]
        public void CreateEmployeesList_CalendarModelIsNotEmpty_ProperEmployeesList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateEmployeesList(calendarModel, calendarTable);

            //Assert        

            Assert.AreEqual(5, calendarTable.Count);

            Assert.AreEqual("Rose Johnny", calendarTable[0][0].GetText());
            Assert.AreEqual("Olson Wayne", calendarTable[4][0].GetText());

        }

        [Test]
        public void CreateEmployeesList_CalendarModelIsEmpty_EmptyEmplyeesList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateEmployeesList(new List<CalendarRowViewModel>(), calendarTable);

            //Assert        

            Assert.AreEqual(0, calendarTable.Count);

        }

        [Test]
        public void CreateEmployeesList_CalendarModelIsNull_EmptyEmployeeList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateEmployeesList(null, calendarTable);

            //Assert        

            Assert.AreEqual(0, calendarTable.Count);

        }
        [Test]
        public void CreateEmployeesList_CalendaTableIsNull_NoException()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateEmployeesList(calendarModel, null);

            //Assert        

        }

        #endregion

        #region CreateHeaderSpacer

        [Test]
        public void CreateHeaderSpacer_CalendarModelIsNotEmpty_ProperEmployeesList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateHeaderSpacer(calendarModel, calendarTable);

            //Assert        

            Assert.AreEqual(5, calendarTable.Count);

            Assert.AreEqual(true, calendarTable[0][0].GetBorder(Border.TOP));
            Assert.AreEqual(false, calendarTable[0][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual(false, calendarTable[2][0].GetBorder(Border.TOP));
            Assert.AreEqual(false, calendarTable[2][0].GetBorder(Border.BOTTOM));
            Assert.AreEqual(false, calendarTable[4][0].GetBorder(Border.TOP));
            Assert.AreEqual(true, calendarTable[4][0].GetBorder(Border.BOTTOM));

        }

        [Test]
        public void CreateHeaderSpacer_CalendarModelIsEmpty_EmptyEmplyeesList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateHeaderSpacer(new List<CalendarRowViewModel>(), calendarTable);

            //Assert        

            Assert.AreEqual(0, calendarTable.Count);

        }

        [Test]
        public void CreateHeaderSpacer_CalendarModelIsNull_EmptyEmployeeList()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateHeaderSpacer(null, calendarTable);

            //Assert        

            Assert.AreEqual(0, calendarTable.Count);

        }
        [Test]
        public void CreateHeaderSpacer_CalendarTableIsNull_NoException()
        {

            //Arrange

            //Act
            CalendarToPdfExporter.CreateHeaderSpacer(calendarModel, null);

            //Assert        

        }

        #endregion

        #region CreateEmptyCalendarBody

        [Test]
        public void CreateEmptyCalendarBody_FromLesserThanToProperCalendarData_TableWithEmptyCells()
        {      
        //Arrange
            DateTime from = new DateTime(2010,12,31); 
            DateTime to = new DateTime(2011,01,11);
      
        //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);
        
        //Assert        
            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(12, result[0].Count);
            Assert.AreEqual(12, result[4].Count);
            Assert.IsNull(result[0][0].GetText());
            Assert.IsNull(result[4][11].GetText());
        }

        [Test]
        public void CreateEmptyCalendarBody_ToEqualsToFromProperCalendarData_TableWithEmptyCells()
        {
            //Arrange
            DateTime from = new DateTime(2009, 01, 10);
            DateTime to = new DateTime(2009, 01, 10);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(1, result[0].Count);
            Assert.AreEqual(1, result[4].Count);
            Assert.IsNull(result[0][0].GetText());
            Assert.IsNull(result[4][0].GetText());
        }

        [Test]
        public void CreateEmptyCalendarBody_FromGreaterThanToProperCalendarData_TableWithEmptyRows()
        {
            //Arrange
            DateTime from = new DateTime(2010, 12, 29);
            DateTime to = new DateTime(2010, 12, 28);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(calendarModel, from, to);

            //Assert        
            Assert.AreEqual(5, result.Count);

            Assert.AreEqual(0, result[0].Count);
            Assert.AreEqual(0, result[4].Count);
        }

        [Test]
        public void CreateEmptyCalendarBody_EmptyCalendarData_EmptyTable()
        {
            //Arrange
            DateTime from = new DateTime(2010, 12, 29);
            DateTime to = new DateTime(2010, 12, 28);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(new List<CalendarRowViewModel>(), from, to);

            //Assert        
            Assert.AreEqual(0, result.Count);

        }

        [Test]
        public void CreateEmptyCalendarBody_NullCalendarData_EmptyTable()
        {
            //Arrange
            DateTime from = new DateTime(2010, 12, 29);
            DateTime to = new DateTime(2010, 12, 28);

            //Act
            List<List<Cell>> result = CalendarToPdfExporter.CreateEmptyCalendarBody(null, from, to);

            //Assert        
            Assert.AreEqual(0, result.Count);

        }

        #endregion

        #region GetColumnIndexForCalendarItem

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsAndEndsBetweenDates_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 31),
                To = new DateTime(2011, 12, 31),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsAtStartAndEndsAtEndDate_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 30),
                To = new DateTime(2012, 1, 1),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(0, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsBeforeStartAndEndsAtStartDate_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2011, 12, 30),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(0, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsAtEndAndEndsAfterEndDate_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2012, 1, 1),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(2, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsAfterEndDate_NegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2012, 1, 2),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.Greater(0, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btEndsBeforeEndDate_NegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2011, 12, 29),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.Greater(0, result);

        }

        [Test]
        public void GetColumnIndexForCalendarItem_btStartsBeforeStartAndEndsAfterEndDate_NonNegativeIndex()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnIndexForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual( 0, result);

        }

#endregion

        #region GetColumnSpanForCalendarItem

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsAndEndsBetweenDates_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 31),
                To = new DateTime(2011, 12, 31),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsAtStartAndEndsAtEndDate_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 30),
                To = new DateTime(2012, 1, 1),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(3, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsBeforeStartAndEndsAtStartDate_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2011, 12, 30),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsAtEndAndEndsAfterEndDate_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2012, 1, 1),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual(1, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsAfterEndDate_NegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2012, 1, 2),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.Greater(0, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btEndsBeforeEndDate_NegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2011, 12, 29),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.Greater(0, result);

        }

        [Test]
        public void GetColumnSpanForCalendarItem_btStartsBeforeStartAndEndsAfterEndDate_NonNegativeSpan()
        {
            //Arrange
            DateTime from = new DateTime(2011, 12, 30);
            DateTime to = new DateTime(2012, 1, 1);
            CalendarItemViewModel calendarItemViewMpodel = new CalendarItemViewModel(new CalendarItem()
            {
                From = new DateTime(2011, 12, 29),
                To = new DateTime(2012, 1, 2),
                CalendarItemID = 1,
                Type = CalendarItemType.Journey
            });

            //Act
            int result = CalendarToPdfExporter.GetColumnSpanForCalendarItem(calendarItemViewMpodel, from, to);

            //Assert        
            Assert.AreEqual( 3, result);

        }

        #endregion

    }
}
