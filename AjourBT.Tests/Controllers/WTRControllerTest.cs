using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Models;
using AjourBT.Tests.MockRepository;
using ExcelLibrary.SpreadSheet;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class WTRControllerTest
    {
        Mock<IRepository> mock;
        WTRController controller;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            controller = new WTRController(mock.Object);
        }

        #region GetWTR

        [Test]
        public void GetWTR_returnView()
        {
            //Arrange
            //Act
            var result = controller.GetWTR();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        #endregion

        #region SearchEmployeeData

        [Test]
        public void SearchEmployeeData_emptyListAndsearchString_Empty()
        {
            //Arrange
            List<Employee> empList = new List<Employee>();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2012, 01, 01), new DateTime(2014, 01, 01), empList, "");
            //Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void SearchEmployeeData_NotEmptyListAndEmptySearchString_list()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2010, 01, 01), new DateTime(2014, 01, 01), empList, "");
            //Assert
            Assert.AreEqual(6, result.Count);
            Assert.AreEqual(8, result[0].EmployeeID);
            Assert.AreEqual("Oleksiy", result[0].FirstName);
            Assert.AreEqual(1, result[0].CalendarItems.Count);
        }

        [Test]
        public void SearchEmployeeData_NotEmptyListAndNotEmptySearchString_list()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2010, 01, 01), new DateTime(2015, 01, 01), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].EmployeeID);
            Assert.AreEqual("Anatoliy", result[0].FirstName);
            Assert.AreEqual("Struz", result[0].LastName);
            Assert.AreEqual(7, result[0].CalendarItems.Count);
        }

        [Test]
        public void SearchEmployeeData_CalendarItemStartedJustAfterToDate_EmptyList()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2014, 01, 30), new DateTime(2014, 01, 31), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SearchEmployeeData_CalendarItemEndedJustBeforeFromDate_EmptyList()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2014, 02, 15), new DateTime(2014, 02, 16), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void SearchEmployeeData_CalendarItemStartedJustOnToDate_EmptyList()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2014, 01, 30), new DateTime(2014, 02, 01), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void SearchEmployeeData_CalendarItemEndedJustOnFromDate_EmptyList()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2014, 02, 14), new DateTime(2014, 02, 16), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void SearchEmployeeData_CalendarItemStartedJustOnFromDate_EmptyList()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2014, 02, 01), new DateTime(2014, 02, 16), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void SearchEmployeeData_CalendarItemEndedJustOnToDate_EmptyList()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2014, 01, 30), new DateTime(2014, 02, 14), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void SearchEmployeeData_CalendarItemStartedAndEndedBetwwenDates_EmptyList()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2014, 01, 30), new DateTime(2014, 02, 16), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void SearchEmployeeData_CalendarItemStartedBeforeAndEndedAfterDates_EmptyList()
        {
            //Arrange
            List<Employee> empList = (from emp in mock.Object.Employees where (emp.FirstName != "") select emp).ToList();
            //Act
            var result = controller.SearchEmployeeData(new DateTime(2014, 02, 03), new DateTime(2014, 02, 12), empList, "Anatoliy");
            //Assert
            Assert.AreEqual(1, result.Count);
        }

        #endregion

        #region GetWTRData

        [Test]
        public void GetWTRData_EmptyDates_EmptyList()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("", "", "");
            //Assert
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
            Assert.IsNull(result.ViewData.Model);
        }

        [Test]
        public void GetWTRData_CorrectDates_List()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("01.01.2014", "12.12.2014", "");
            //Assert
            Assert.AreEqual(6, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Boryslav", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Teshaw", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("tebl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual("Tanya", ((List<WTRViewModel>)result.Model).ToArray()[2].FirstName);
            Assert.AreEqual("Kowood", ((List<WTRViewModel>)result.Model).ToArray()[2].LastName);
            Assert.AreEqual("tadk", ((List<WTRViewModel>)result.Model).ToArray()[2].ID);
            Assert.AreEqual(2, ((List<WTRViewModel>)result.Model).ToArray()[2].FactorDetails.Count);
        }

        [Test]
        public void GetWTRData_CorrectDates_NotEmptySearchStringFromEqualsToGreater_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("25.01.2014", "05.02.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(3, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014,01,25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 26), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4 , ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRData_IncorrectDates_NotEmptySearchStringToIsGreaterThanFrom_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("25.01.2014", "24.01.2014", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);    
        }

        [Test]
        public void GetWTRData_IncorrectDates_NotEmptySearchStringFromEqualsToNull_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("25.01.201456", "24.01.2014", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }

        [Test]
        public void GetWTRData_IncorrectDates_NotEmptySearchStringToEqualsToNull_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("25.01.2014", "24.01.201456", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }


        [Test]
        public void GetWTRData_CorrectDates_NotEmptySearchStringFromEqualsToEquals_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("25.01.2014", "25.01.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRData_CorrectDates_NotEmptySearchStringFromLesserToEquals_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("02.01.2014", "25.01.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRData_CorrectDates_NotEmptySearchStringFromLesserToGreater_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("02.01.2014", "26.01.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 26), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRData_CorrectDates_NotEmptySearchStringFromGreaterThanFromLesserThanTo_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("26.01.2014", "26.01.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 26), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 26), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRData_CorrectDates_NotEmptySearchStringFromGreaterThanFromEqual_To_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("05.02.2014", "06.02.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 02, 05), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 02, 05), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(6, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRData_CorrectDates_NotEmptySearchStringFromGreaterThanFromGreaterThanCalendarItemTo_EmptyList()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData("06.02.2014", "06.02.2014", "andl");
            //Assert
            Assert.AreEqual(0, ((List<WTRViewModel>)result.ViewData.Model).Count);
           
        }

        [Test]
        public void GetWTRData_returnPartialView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRData();
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }

        #endregion

        #region GetAbsencePerEMP

        [Test]
        public void GetAbsensePerEmp_View()
        {
            //Arrange
            //Act
            var result = controller.GetAbsencePerEMP() as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

        #endregion

        #region GetWTRPerEMP

        [Test]
        public void GetWTRPerEMP_View()
        {
            //Arrange
            //Act
            var result = controller.GetWTRPerEMP() as ViewResult;

            //Assert
            Assert.AreEqual("", result.ViewName);
        }

#endregion

        #region GetWTRDataPerEMP

        [Test]
        public void GetWTRDataPerEMP_BadDates_EmptyView()
        {
            //Arrange
            string fromDate = "";
            string toDate = "";
            string userName = "";

            //Act
            var result = controller.GetWTRDataPerEMP(fromDate, toDate, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("GetWTRDataEmpty", result.ViewName);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDatesBadUserName_NoDataView()
        {
            //Arrange
            string fromDate = "21.01.2014";
            string toDate = "30.03.2014";
            string userName = "gggrr";

            //Act
            var result = controller.GetWTRDataPerEMP(fromDate, toDate, userName) as PartialViewResult;

            //Assert
            Assert.AreEqual("NoData", result.ViewName);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDatesCorrectUserName_View()
        {
            //Arrange
            string fromDate = "01.01.2010";
            string toDate = "31.12.2017";
            string userName = "andl";

            //Act
            var result = controller.GetWTRDataPerEMP(fromDate, toDate, userName) as PartialViewResult;
            var resModel = result.Model as List<WTRViewModel>;
            List<FactorData> fData = resModel.ToArray()[0].FactorDetails;

            //Assert
            Assert.AreEqual("GetWTRDataPerEMP", result.ViewName);
            Assert.AreEqual(2010, result.ViewBag.FromYear);
            Assert.AreEqual(2017, result.ViewBag.ToYear);
            Assert.AreEqual(13, fData.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, fData.ToArray()[0].Factor);
            Assert.AreEqual(new DateTime(2014,01,25), fData.ToArray()[0].From);
            Assert.AreEqual(new DateTime(2014,01,26), fData.ToArray()[0].To);
            Assert.AreEqual(4, fData.ToArray()[0].WeekNumber);

            Assert.AreEqual(CalendarItemType.ReclaimedOvertime, fData.ToArray()[4].Factor);
            Assert.AreEqual(new DateTime(2013,01,01), fData.ToArray()[4].From);
            Assert.AreEqual(new DateTime(2013,01,01), fData.ToArray()[4].To);
            Assert.AreEqual(1, fData.ToArray()[4].WeekNumber);
            Assert.AreEqual(CalendarItemType.PaidVacation, fData.ToArray()[5].Factor);
            Assert.AreEqual(CalendarItemType.PaidVacation, fData.ToArray()[6].Factor);
            Assert.AreEqual(CalendarItemType.BT, fData.ToArray()[12].Factor); 
        }


        [Test]
        public void GetWTRDataPerEMP_CorrectDates_NotEmptySearchStringFromEqualsToGreater_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("25.01.2014", "05.02.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(3, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 26), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRDataPerEMP_IncorrectDates_NotEmptySearchStringToIsGreaterThanFrom_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("25.01.2014", "24.01.2014", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }

        [Test]
        public void GetWTRDataPerEMP_IncorrectDates_NotEmptySearchStringFromEqualsToNull_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("25.01.201456", "24.01.2014", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }

        [Test]
        public void GetWTRDataPerEMP_IncorrectDates_NotEmptySearchStringToEqualsToNull_EmptyView()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("25.01.2014", "24.01.201456", "andl");
            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
        }


        [Test]
        public void GetWTRDataPerEMP_CorrectDates_NotEmptySearchStringFromEqualsToEquals_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("25.01.2014", "25.01.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDates_NotEmptySearchStringFromLesserToEquals_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("02.01.2014", "25.01.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDates_NotEmptySearchStringFromLesserToGreater_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("02.01.2014", "26.01.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 25), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 26), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDates_NotEmptySearchStringFromGreaterThanFromLesserThanTo_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("26.01.2014", "26.01.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 01, 26), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 01, 26), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(4, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDates_NotEmptySearchStringFromGreaterThanFromEqual_To_CorrectListNoCalendarItemsFromPrevYears()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("05.02.2014", "06.02.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Factor);
            Assert.AreEqual(new DateTime(2014, 02, 05), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].From);
            Assert.AreEqual(new DateTime(2014, 02, 05), ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].To);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Hours);
            Assert.AreEqual(null, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].Location);
            Assert.AreEqual(6, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails[0].WeekNumber);
        }

        [Test]
        public void GetWTRDataPerEMP_CorrectDates_NotEmptySearchStringFromGreaterThanFromGreaterThanCalendarItemTo_EmptyList()
        {
            //Arrange
            //Act
            var result = controller.GetWTRDataPerEMP("07.02.2014", "07.02.2014", "andl");
            //Assert
            Assert.AreEqual(1, ((List<WTRViewModel>)result.ViewData.Model).Count);
            Assert.AreEqual("Anastasia", ((List<WTRViewModel>)result.Model).ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", ((List<WTRViewModel>)result.Model).ToArray()[0].LastName);
            Assert.AreEqual("andl", ((List<WTRViewModel>)result.Model).ToArray()[0].ID);
            Assert.AreEqual(0, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);

        }
        #endregion

        #region WTRExportToExcel

        [Test]
        public void CreateCaption_workSheet_ProperCaptionWorkSheet()
        {
            //Arrange
            WTRController controller = new WTRController(mock.Object);
            Worksheet workSheet = new Worksheet("WTR");
            string[] caption = { "Employee", "Location", "Factor", "Dates", "Hours" };

            //Act
            controller.CreateCaption(workSheet);

            //Assert
            for (int i = 0; i < caption.Length; i++)
            {
                Assert.AreEqual(caption[i], workSheet.Cells[0, i].Value);
            }
        }

        [Test]
        public void ExportWTR_FileResult()
        {
            //Arrange
            WTRController controller = new WTRController(mock.Object);
            string searchString = "";
            string fromDate = "01.01.2013";
            string toDate = "25.12.2014";

            //Act
            FileResult file = controller.ExportWTR(searchString, fromDate, toDate) as FileResult;

            //Assert
            Assert.IsInstanceOf(typeof(FileResult), file);
        }

        [Test]
        public void WriteWTRData_WorkSheetNull_Exception()
        {
            //Arrange
            WTRController controller = new WTRController(mock.Object);
            string searchString = "";
            string fromDate = "01.01.2013";
            string toDate = "25.12.2014";

            //Act

            //Assert
            Assert.Throws<NullReferenceException>(() => controller.WriteWTRData(null, searchString, fromDate, toDate));
        }

        [Test]
        public void WriteWTRData_WorkSheetNotNull_DataWrittenToFile()
        {
            //Arrange
            WTRController controller = new WTRController(mock.Object);
            Worksheet workSheet = new Worksheet("NewWTR");
            string searchString = "";
            string fromDate = "01.01.2013";
            string toDate = "25.12.2014";

            //Act
            controller.WriteWTRData(workSheet, searchString, fromDate, toDate);
            
            //Assert
            Assert.AreEqual("", workSheet.Cells[1, 0].Value.ToString());
            Assert.AreEqual("2013- W 1", workSheet.Cells[2, 0].Value.ToString());
            Assert.AreEqual("", workSheet.Cells[3, 0].Value.ToString());
            Assert.AreEqual("Zarose Anastasia(andl)", workSheet.Cells[4, 0].Value.ToString());
            //Assert.AreEqual("", workSheet.Cells[4, 1].Value.ToString());
            //Assert.AreEqual("", workSheet.Cells[4, 2].Value.ToString());
            //Assert.AreEqual("", workSheet.Cells[4, 3].Value.ToString());
            //Assert.AreEqual("", workSheet.Cells[4, 4].Value.ToString());
        }

        #endregion 


        #region WTRExportToExcelForEmp

        [Test]
        public void CreateCaption_WorkSheet_ProperCaptionWorkSheet()
        {
            //Arrange
            WTRController controller = new WTRController(mock.Object);
            Worksheet workSheet = new Worksheet("WTR");
            string[] caption = { "Employee", "Location", "Factor", "Dates", "Hours" };

            //Act
            controller.CreateCaption(workSheet);

            //Assert
            for (int i = 0; i < caption.Length; i++)
            {
                Assert.AreEqual(caption[i], workSheet.Cells[0, i].Value);
            }
        }

        [Test]
        public void ExportWTRForEMP_FileResult()
        {
            //Arrange
            WTRController controller = new WTRController(mock.Object);
            string userName = "andl";
            string fromDate = "01.01.2013";
            string toDate = "25.12.2014";

            //Act
            FileResult file = controller.ExportWTRForEMP(userName, fromDate, toDate) as FileResult;

            //Assert
            Assert.IsInstanceOf(typeof(FileResult), file);
        }

        [Test]
        public void WriteWTRDataForEMP_WorkSheetNull_Exception()
        {
            //Arrange
            WTRController controller = new WTRController(mock.Object);
            string userName = "andl";
            string fromDate = "01.01.2013";
            string toDate = "25.12.2014";

            //Act

            //Assert
            Assert.Throws<NullReferenceException>(() => controller.WriteWTRDataForEMP(null, userName, fromDate, toDate));
        }

        [Test]
        public void WriteWTRDataForEMP_WorkSheetNotNull_DataWrittenToFile()
        {
            //Arrange
            WTRController controller = new WTRController(mock.Object);
            Worksheet workSheet = new Worksheet("NewWTR");
            string userName = "andl";
            string fromDate = "01.01.2013";
            string toDate = "25.12.2014";

            //Act
            controller.WriteWTRDataForEMP(workSheet, userName, fromDate, toDate);

            //Assert
            Assert.AreEqual("", workSheet.Cells[1, 0].Value.ToString());
            Assert.AreEqual("2013- W 1", workSheet.Cells[2, 0].Value.ToString());
            Assert.AreEqual("", workSheet.Cells[3, 0].Value.ToString());
            Assert.AreEqual("Zarose Anastasia(andl)", workSheet.Cells[4, 0].Value.ToString());
        }

        #endregion 
        #region GetWeeksInTimeSpan
            [Test]
        public void GetWeeksInTimeSpan_SingleDay_SingleWeek()
            {
                //Arrange
                DateTime from = new DateTime(2014, 04, 23);
                DateTime to = new DateTime(2014, 04, 23);
                WTRController controller = new WTRController(mock.Object);

                //Act
                Dictionary<int, WTRController.StartEndDatePair> result = controller.GetWeeksInTimeSpan(from, to);

                //Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(17, result.Keys.FirstOrDefault());
                Assert.AreEqual(new DateTime(2014, 04, 23), result[17].startDate);
                Assert.AreEqual(new DateTime(2014, 04, 23), result[17].endDate);
            }

            [Test]
            public void GetWeeksInTimeSpan_TwoDaysAtBeginningOfWeek_SingleWeek()
            {
                //Arrange
                DateTime from = new DateTime(2014, 04, 21);
                DateTime to = new DateTime(2014, 04, 22);
                WTRController controller = new WTRController(mock.Object);

                //Act
                Dictionary<int, WTRController.StartEndDatePair> result = controller.GetWeeksInTimeSpan(from, to);
                
                //Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(17, result.Keys.FirstOrDefault());
                Assert.AreEqual(new DateTime(2014, 04, 21), result[17].startDate);
                Assert.AreEqual(new DateTime(2014, 04, 22), result[17].endDate);
            }

            [Test]
            public void GetWeeksInTimeSpan_TwoDaysAtEndOfWeek_SingleWeek()
            {
                //Arrange
                DateTime from = new DateTime(2014, 04, 26);
                DateTime to = new DateTime(2014, 04, 27);
                WTRController controller = new WTRController(mock.Object);

                //Act
                Dictionary<int, WTRController.StartEndDatePair> result = controller.GetWeeksInTimeSpan(from, to);

                //Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(17, result.Keys.FirstOrDefault());
                Assert.AreEqual(new DateTime(2014, 04, 26), result[17].startDate);
                Assert.AreEqual(new DateTime(2014, 04, 27), result[17].endDate);
            }

            [Test]
            public void GetWeeksInTimeSpan_TwoDaysAtEdgeOfTwoWeeks_TwoWeeks()
            {
                //Arrange
                DateTime from = new DateTime(2014, 04, 27);
                DateTime to = new DateTime(2014, 04, 28);
                WTRController controller = new WTRController(mock.Object);

                //Act
                Dictionary<int, WTRController.StartEndDatePair> result = controller.GetWeeksInTimeSpan(from, to);

                //Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(17, result.Keys.FirstOrDefault());
                Assert.AreEqual(18, result.Keys.Skip(1).FirstOrDefault());
                Assert.AreEqual(new DateTime(2014, 04, 27), result[17].startDate);
                Assert.AreEqual(new DateTime(2014, 04, 27), result[17].endDate);
                Assert.AreEqual(new DateTime(2014, 04, 28), result[18].startDate);
                Assert.AreEqual(new DateTime(2014, 04, 28), result[18].endDate);
            }

            [Test]
            public void GetWeeksInTimeSpan_ManyDaysAcrossThreeWeeks_ThreeWeeks()
            {
                //Arrange
                DateTime from = new DateTime(2014, 04, 26);
                DateTime to = new DateTime(2014, 05, 07);
                WTRController controller = new WTRController(mock.Object);

                //Act
                Dictionary<int, WTRController.StartEndDatePair> result = controller.GetWeeksInTimeSpan(from, to);

                //Assert
                Assert.AreEqual(3 , result.Count);
                Assert.AreEqual(17, result.Keys.FirstOrDefault());
                Assert.AreEqual(18, result.Keys.Skip(1).FirstOrDefault());
                Assert.AreEqual(19, result.Keys.Skip(2).FirstOrDefault());
                Assert.AreEqual(new DateTime(2014, 04, 26), result[17].startDate);
                Assert.AreEqual(new DateTime(2014, 04, 27), result[17].endDate);
                Assert.AreEqual(new DateTime(2014, 04, 28), result[18].startDate);
                Assert.AreEqual(new DateTime(2014, 05, 04), result[18].endDate);
                Assert.AreEqual(new DateTime(2014, 05, 05), result[19].startDate);
                Assert.AreEqual(new DateTime(2014, 05, 07), result[19].endDate);
            }

            [Test]
            public void GetWeeksInTimeSpan_DaysAcrossTwoYears_TwoWeeksSecondOneHasNumber1()
            {
                //Arrange
                DateTime from = new DateTime(2014, 12, 29);
                DateTime to = new DateTime(2015, 01, 02);
                WTRController controller = new WTRController(mock.Object);

                //Act
                Dictionary<int, WTRController.StartEndDatePair> result = controller.GetWeeksInTimeSpan(from, to);

                //Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(53, result.Keys.FirstOrDefault());
                Assert.AreEqual(1, result.Keys.Skip(1).FirstOrDefault());
                Assert.AreEqual(new DateTime(2014, 12, 29), result[53].startDate);
                Assert.AreEqual(new DateTime(2014, 12, 31), result[53].endDate);
                Assert.AreEqual(new DateTime(2015, 01, 01), result[1].startDate);
                Assert.AreEqual(new DateTime(2015, 01, 02), result[1].endDate);
            }

            [Test]
            public void GetWeeksInTimeSpan_IllegalDatesToIsLesserThanFrom_0Weeks()
            {
                //Arrange
                DateTime from = new DateTime(2014, 01, 02);
                DateTime to = new DateTime(2014, 01, 01);
                WTRController controller = new WTRController(mock.Object);

                //Act
                Dictionary<int, WTRController.StartEndDatePair> result = controller.GetWeeksInTimeSpan(from, to);

                //Assert
                Assert.AreEqual( 0, result.Count);
            }

        #endregion

        #region IntersectDatePairDicts
            
            [Test]
            public void IntersectDatePairDicts_CalendarItemStartedBeforeAndEndedAfterDates_TwoKeysDictionary()
            {
                //Arrange
                DateTime from = new DateTime(2014, 02, 03);
                DateTime to = new DateTime(2014, 02, 12);
               
                DateTime wtrFrom = new DateTime(2014, 02, 01);
                DateTime wtrTo = new DateTime(2014, 02, 28);
                Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                //Act
                var result = controller.IntersectDatePairDicts(dict, wtrDict);
                //Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(6, result.Keys.FirstOrDefault());
                Assert.AreEqual(7, result.Keys.Last());
                Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
                Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);
            
            }

            public void IntersectDatePairDicts_CalendarItemStartINAndEndedAfterDates_TwoKeysDictionary()
            {
                //Arrange
                DateTime from = new DateTime(2014, 02, 03);
                DateTime to = new DateTime(2014, 02, 12);

                DateTime wtrFrom = new DateTime(2014, 02, 03);
                DateTime wtrTo = new DateTime(2014, 02, 28);
                Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                //Act
                var result = controller.IntersectDatePairDicts(dict, wtrDict);
                //Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(6, result.Keys.FirstOrDefault());
                Assert.AreEqual(7, result.Keys.Last());
                Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
                Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);

            }
            public void IntersectDatePairDicts_CalendarItemStartIAndEndIn_TwoKeysDictionary()
            {
                //Arrange
                DateTime from = new DateTime(2014, 02, 03);
                DateTime to = new DateTime(2014, 02, 12);

                DateTime wtrFrom = new DateTime(2014, 02, 03);
                DateTime wtrTo = new DateTime(2014, 02, 28);
                Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                //Act
                var result = controller.IntersectDatePairDicts(dict, wtrDict);
                //Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(6, result.Keys.FirstOrDefault());
                Assert.AreEqual(7, result.Keys.Last());
                Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
                Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);
            }

            public void IntersectDatePairDicts_CalendarItemStartBeforeEndIn_TwoKeysDictionary()
            {
                //Arrange
                DateTime from = new DateTime(2014, 02, 03);
                DateTime to = new DateTime(2014, 02, 12);

                DateTime wtrFrom = new DateTime(2014, 02, 03);
                DateTime wtrTo = new DateTime(2014, 02, 28);
                Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                //Act
                var result = controller.IntersectDatePairDicts(dict, wtrDict);
                //Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(6, result.Keys.FirstOrDefault());
                Assert.AreEqual(7, result.Keys.Last());
                Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
                Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);

            }
            [Test]
            public void IntersectDatePairDicts_WTRFromInLastYearToCurrentYear_TwoKeysDictionary()
            {
                //Arrange
                DateTime from = new DateTime(2014, 02, 03);
                DateTime to = new DateTime(2014, 02, 12);

                DateTime wtrFrom = new DateTime(2013, 12, 12);
                DateTime wtrTo = new DateTime(2014, 02, 28);
                Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                //Act
                var result = controller.IntersectDatePairDicts(dict, wtrDict);
                //Assert
                Assert.AreEqual(2, result.Count);
                Assert.AreEqual(6, result.Keys.FirstOrDefault());
                Assert.AreEqual(7, result.Keys.Last());
                Assert.AreEqual(new DateTime(2014, 02, 03), result[6].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 09), result[6].endDate);
                Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
                Assert.AreEqual(new DateTime(2014, 02, 12), result[7].endDate);
            }

             [Test]
            public void IntersectDatePairDicts_CalendarItemDuesForOneDay_OneKeyDictionary()
            {
                //Arrange
                DateTime from = new DateTime(2014, 01, 01);
                DateTime to = new DateTime(2014, 01, 01);

                DateTime wtrFrom = new DateTime(2013, 12, 12);
                DateTime wtrTo = new DateTime(2014, 01, 10);
                Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                //Act
                var result = controller.IntersectDatePairDicts(dict, wtrDict);
                //Assert
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(1, result.Keys.FirstOrDefault());
                Assert.AreEqual(1, result.Keys.Last());
                Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
                Assert.AreEqual(new DateTime(2014, 01, 01), result[1].endDate);
             }

             [Test]
             public void IntersectDatePairDicts_WTRFromToNotContainCalendarItem_OneKeyDictionary()
             {
                 //Arrange
                 DateTime from = new DateTime(2014, 01, 01);
                 DateTime to = new DateTime(2014, 01, 01);

                 DateTime wtrFrom = new DateTime(2015, 12, 12);
                 DateTime wtrTo = new DateTime(2015, 01, 10);
                 Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                 Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                 //Act
                 var result = controller.IntersectDatePairDicts(dict, wtrDict);
                 //Assert
                 Assert.AreEqual(0, result.Count);
             }

             [Test]
             public void IntersectDatePairDicts_CalendarItemContainWTRFromTo_OneKeyDictionary()
             {
                 //Arrange
                 DateTime from = new DateTime(2013, 12, 01);
                 DateTime to = new DateTime(2014, 02, 25);

                 DateTime wtrFrom = new DateTime(2014, 01, 01);
                 DateTime wtrTo = new DateTime(2014, 2, 10);
                 Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                 Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                 //Act
                 var result = controller.IntersectDatePairDicts(dict, wtrDict);
                 //Assert
                 Assert.AreEqual(7, result.Count);
                 Assert.AreEqual(1, result.Keys.FirstOrDefault());
                 Assert.AreEqual(7, result.Keys.Last());
                 Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
                 Assert.AreEqual(new DateTime(2014, 01, 05), result[1].endDate);
                 Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
                 Assert.AreEqual(new DateTime(2014, 01, 05), result[1].endDate);
                 Assert.AreEqual(new DateTime(2014, 02, 10), result[7].startDate);
                 Assert.AreEqual(new DateTime(2014, 02, 10), result[7].endDate);
             }

             [Test]
             public void IntersectDatePairDicts_WTRFromToContainsToDateOfCalendarItem_FifthKeyDictionary()
             {
                 //Arrange
                 DateTime from = new DateTime(2013, 12, 01);
                 DateTime to = new DateTime(2014, 02, 01);

                 DateTime wtrFrom = new DateTime(2014, 01, 01);
                 DateTime wtrTo = new DateTime(2014, 2, 10);
                 Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                 Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                 //Act
                 var result = controller.IntersectDatePairDicts(dict, wtrDict);
                 //Assert
                 Assert.AreEqual(5, result.Count);
                 Assert.AreEqual(1, result.Keys.FirstOrDefault());
                 Assert.AreEqual(5, result.Keys.Last());
                 Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
                 Assert.AreEqual(new DateTime(2014, 01, 05), result[1].endDate);
                 Assert.AreEqual(new DateTime(2014, 02, 01), result[5].endDate);
                 Assert.AreEqual(new DateTime(2014, 01, 27), result[5].startDate);
             }

             [Test]
             public void IntersectDatePairDicts_WTRFromToContainsFromDateOfCalendarItem_SixKeyDictionary()
             {
                 //Arrange
                 DateTime from = new DateTime(2013, 12, 01);
                 DateTime to = new DateTime(2014, 02, 01);

                 DateTime wtrFrom = new DateTime(2013, 11, 01);
                 DateTime wtrTo = new DateTime(2014, 01, 01);
                 Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                 Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                 //Act
                 var result = controller.IntersectDatePairDicts(dict, wtrDict);
                 //Assert
                 Assert.AreEqual(7, result.Count);
                 Assert.AreEqual(48, result.Keys.FirstOrDefault());
                 Assert.AreEqual(1, result.Keys.Last());
                 Assert.AreEqual(new DateTime(2013, 12, 01), result[48].startDate);
                 Assert.AreEqual(new DateTime(2013, 12, 01), result[48].endDate);
                 Assert.AreEqual(new DateTime(2013, 12, 29), result[52].endDate);
                 Assert.AreEqual(new DateTime(2013, 12, 30), result[53].startDate);
                 Assert.AreEqual(new DateTime(2013, 12, 31), result[53].endDate);
                 Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
                 Assert.AreEqual(new DateTime(2014, 01, 01), result[1].endDate);
             }

             [Test]
             public void IntersectDatePairDicts_CalendarItemContainsFromDateOfWTRFromTo_SixKeyDictionary()
             {
                 //Arrange
                 DateTime from = new DateTime(2013, 12, 01);
                 DateTime to = new DateTime(2014, 02, 25);

                 DateTime wtrFrom = new DateTime(2014, 01, 01);
                 DateTime wtrTo = new DateTime(2014, 02, 28);
                 Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                 Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                 //Act
                 var result = controller.IntersectDatePairDicts(dict, wtrDict);
                 //Assert
                 Assert.AreEqual(9, result.Count);
                 Assert.AreEqual(1, result.Keys.FirstOrDefault());
                 Assert.AreEqual(9, result.Keys.Last());
                 Assert.AreEqual(new DateTime(2014, 01, 01), result[1].startDate);
                 Assert.AreEqual(new DateTime(2014, 01, 05), result[1].endDate);
                 Assert.AreEqual(new DateTime(2014, 02, 24), result[9].startDate);
                 Assert.AreEqual(new DateTime(2014, 02, 25), result[9].endDate);
             }
             [Test]
             public void IntersectDatePairDicts_CalendarItemContainsFromAndToDatesOfWTRFromTo_SixKeyDictionary()
             {
                 //Arrange
                 DateTime from = new DateTime(2013, 12, 01);
                 DateTime to = new DateTime(2014, 02, 25);

                 DateTime wtrFrom = new DateTime(2013, 11, 30);
                 DateTime wtrTo = new DateTime(2014, 02, 28);
                 Dictionary<int, WTRController.StartEndDatePair> dict = controller.GetWeeksInTimeSpan(from, to);
                 Dictionary<int, WTRController.StartEndDatePair> wtrDict = controller.GetWeeksInTimeSpan(wtrFrom, wtrTo);

                 //Act
                 var result = controller.IntersectDatePairDicts(dict, wtrDict);
                 //Assert
                 Assert.AreEqual(15, result.Count);
                 Assert.AreEqual(48, result.Keys.FirstOrDefault());
                 Assert.AreEqual(9, result.Keys.Last());
                 Assert.AreEqual(new DateTime(2013, 12, 01), result[48].startDate);
                 Assert.AreEqual(new DateTime(2013, 12, 01), result[48].endDate);
                 Assert.AreEqual(new DateTime(2014, 02, 24), result[9].startDate);
                 Assert.AreEqual(new DateTime(2014, 02, 25), result[9].endDate);
             }
        #endregion
    }
}
