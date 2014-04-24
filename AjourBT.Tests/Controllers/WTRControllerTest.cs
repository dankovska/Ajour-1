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
            Assert.AreEqual(6, result[0].CalendarItems.Count);
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
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[0].FactorDetails.Count);
            Assert.AreEqual("Tanya", ((List<WTRViewModel>)result.Model).ToArray()[2].FirstName);
            Assert.AreEqual("Kowood", ((List<WTRViewModel>)result.Model).ToArray()[2].LastName);
            Assert.AreEqual("tadk", ((List<WTRViewModel>)result.Model).ToArray()[2].ID);
            Assert.AreEqual(1, ((List<WTRViewModel>)result.Model).ToArray()[2].FactorDetails.Count);
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
            Assert.AreEqual("GetWTRData", result.ViewName);
            Assert.AreEqual(2010, result.ViewBag.FromYear);
            Assert.AreEqual(2017, result.ViewBag.ToYear);
            Assert.AreEqual(8, fData.Count);
            Assert.AreEqual(CalendarItemType.PaidVacation, fData.ToArray()[0].Factor);
            Assert.AreEqual(new DateTime(2014,01,25), fData.ToArray()[0].From);
            Assert.AreEqual(new DateTime(2014,02,05), fData.ToArray()[0].To);
            Assert.AreEqual(4, fData.ToArray()[0].WeekNumber);

            Assert.AreEqual(CalendarItemType.SickAbsence, fData.ToArray()[4].Factor);
            Assert.AreEqual(new DateTime(2014,02,21), fData.ToArray()[4].From);
            Assert.AreEqual(new DateTime(2014,02,27), fData.ToArray()[4].To);
            Assert.AreEqual(8, fData.ToArray()[4].WeekNumber);
            Assert.AreEqual(CalendarItemType.BT, fData.ToArray()[5].Factor);
            Assert.AreEqual(CalendarItemType.BT, fData.ToArray()[6].Factor);
            Assert.AreEqual(CalendarItemType.BT, fData.ToArray()[7].Factor); 
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
    }
}
