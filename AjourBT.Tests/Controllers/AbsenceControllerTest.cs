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
        public class AbsenceControllerTest
        {
            Mock<IRepository> mock;

            [SetUp]
            public void SetUp()
            {
                mock = Mock_Repository.CreateMock();
            }

            #region GetAbsence

            [Test]
            public void GetAbsence_View()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);

                //Act
                var result = controller.GetAbsence() as ViewResult;

                //Assert
                Assert.AreEqual("", result.ViewName);
            }

            [Test]
            public void GetAbsenceSearchString_View()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);
                string searchString = "ABC";

                //Act
                var result = controller.GetAbsence(searchString) as ViewResult;

                //Assert
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual("ABC", result.ViewBag.SearchString);
            }
            #endregion

            #region GetAbsenceData

            [Test]
            public void GetAbsenceDataBadDates_NoDataView()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);
                string fromDate = "123.456.789";
                string toDate = "45.67.89";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate) as PartialViewResult;

                //Assert
                Assert.AreEqual("NoAbsenceData", result.ViewName);
            }

            [Test]
            public void GetAbsenceDataCalendarItemsNull_NoDataView()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);
                string fromDate = "10.11.1000";
                string toDate = "10.11.1000";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate) as PartialViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoAbsenceData", result.ViewName);
            }

            [Test]
            public void GetAbsenceDataCalendarItemsNullAndsearchString_NoDataView()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);
                string fromDate = "10.11.1000";
                string toDate = "10.11.1000";
                string searchString = "ABC";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as PartialViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoAbsenceData", result.ViewName);
            }


            [Test]
            public void GetAbsenceDataCorrectDates_View()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                string fromDate = "01.01.2000";
                string toDate = "28.07.2017";
                string searchString = "";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as ViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual(9, modelResult.Count);

                Assert.AreEqual(0, modelResult.ToArray()[0].Journeys.Count);
                Assert.AreEqual(1, modelResult.ToArray()[0].Overtimes.Count);
                Assert.AreEqual(1, modelResult.ToArray()[0].Sickness.Count);
                Assert.AreEqual(2, modelResult.ToArray()[0].Vacations.Count);
                Assert.AreEqual(1, modelResult.ToArray()[0].EmployeeID);
                Assert.AreEqual(3, modelResult.ToArray()[0].BusinessTrips.Count);

                Assert.AreEqual(0, modelResult.ToArray()[8].Journeys.Count);
                Assert.AreEqual(1, modelResult.ToArray()[8].Overtimes.Count);
                Assert.AreEqual(0, modelResult.ToArray()[8].Sickness.Count);
                Assert.AreEqual(0, modelResult.ToArray()[8].Vacations.Count);
                Assert.AreEqual(13, modelResult.ToArray()[8].EmployeeID);
                Assert.AreEqual(0, modelResult.ToArray()[8].BusinessTrips.Count);

                Assert.AreEqual(1, modelResult.ToArray()[2].Journeys.Count);
                Assert.AreEqual(0, modelResult.ToArray()[2].Overtimes.Count);
                Assert.AreEqual(0, modelResult.ToArray()[2].Sickness.Count);
                Assert.AreEqual(0, modelResult.ToArray()[2].Vacations.Count);
                Assert.AreEqual(3, modelResult.ToArray()[2].EmployeeID);
                Assert.AreEqual(0, modelResult.ToArray()[2].BusinessTrips.Count);

                //Assert.AreEqual(5, modelResult.ToArray()[8].BusinessTrips.ToArray()[0].EmployeeID);
                //Assert.AreEqual(new DateTime(2014, 06, 12), modelResult.ToArray()[8].Vacations.ToArray()[0].From);
                //Assert.AreEqual(new DateTime(2014, 06, 28), modelResult.ToArray()[8].Vacations.ToArray()[0].To);


            }
            #endregion

            #region SearchEmployeeData

            [Test]
            public void SearchStringEmpty_View()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);
                string fromDate = "01.01.2012";
                string toDate = "01.01.2015";
                string searchString = "";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as ViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual(9, modelResult.Count);
            }

            [Test]
            public void SearchStringNotEmpty_View()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);
                string fromDate = "01.01.2012";
                string toDate = "01.01.2015";
                string searchString = "andl";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as ViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("", result.ViewName);
                Assert.AreEqual(1, modelResult.Count);
            }

            [Test]
            public void SearchStringNotEmptyabcdef_View()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);
                string fromDate = "01.01.2012";
                string toDate = "01.01.2015";
                string searchString = "abcdef";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as PartialViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoAbsenceData", result.ViewName);
            }

            #endregion

            #region SearchAbsenseData

            [Test]
            public void ExportAbsenceToExcel_DateNotParsed_NullReference()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                //Act 
                controller.SearchAbsenceData("23.32,1990", "23.32,1990");

                //Assert 
                Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));
            }

            [Test]
            public void ExportAbsenceToExcel_DateIsEmpy_NullReference()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                //Act 
                controller.SearchAbsenceData("", "");

                //Assert 
                Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));
            }
            #endregion

            #region CreateCaption
            [Test]
            public void CreateCaption_Null_Exception()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                string[] caption = new string[] { "Department", "Name", "EID", "Journeys", "BusinessTrips", "Overtimes", "Sickness", "Vacations" };

                //Act 

                //Assert 
                Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));
            }

            [Test]
            public void CreateCaption_workSheet_ValidCaptionOnWorksheet()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                string[] caption = new string[] { "Department", "Name", "EID", "Journeys", "BusinessTrips", "Overtimes", "Sickness", "Vacations" };

                //Act 
                controller.CreateCaption(workSheet);

                //Assert 
                for (int i = 0; i < caption.Length; i++)
                {
                    Assert.AreEqual(caption[i], workSheet.Cells[0, i].Value);
                }
            }
            
            #endregion

            #region ExportAbsenceToExcel
            [Test]
            public void ExportAbsenceToExcel_FromToDatesSearchStringEmpty_FileResult()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);

                //Act 
                FileResult file = controller.ExportAbsenceToExcel("01.01.2014", "01.02.2014") as FileResult;

                //Assert 
                Assert.IsInstanceOf(typeof(FileResult), file);
            }

            [Test]
            public void ExportAbsenceToExcel_ProperCaptionOnWorksheet()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                string[] caption = new string[] { "Department", "Name", "EID", "Journeys", "BusinessTrips", "Overtimes", "Sickness", "Vacations" };

                //Act 
                controller.CreateCaption(workSheet);

                //Assert 
                for (int i = 0; i < caption.Length; i++)
                {
                    Assert.AreEqual(caption[i], workSheet.Cells[0, i].Value);
                }
            }


            [Test]
            public void CreateCaption_Null_NullReferenceException()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                string[] caption = new string[] { "Department", "Name", "EID", "Journeys", "BusinessTrips", "Overtimes", "Sickness", "Vacations" };

                //Act 

                //Assert 
                Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));
            }

            [Test]
            public void CreateCaption_workSheet_ProperCaptionOnWorksheet()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                string[] caption = new string[] { "Department", "Name", "EID", "Journeys", "BusinessTrips", "Overtimes", "Sickness", "Vacations" };

                //Act 
                controller.CreateCaption(workSheet);

                //Assert 
                for (int i = 0; i < caption.Length; i++)
                {
                    Assert.AreEqual(caption[i], workSheet.Cells[0, i].Value);
                }
            }

           
            [Test]
            public void ExportAbsenceToExcel_DateNotParsed_NullReferense()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                //Act 
                controller.SearchAbsenceData("efwery", "dsacf");

                //Assert 
                Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));
            }



            #endregion

            #region WriteAbsenceData
            [Test]
            public void ExportAbsenceToExcel_FromDateinvalid_Exception()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                //Act 
                controller.WriteAbsenceData(workSheet, "0101.2014", "28.07.2014");

                //Assert 

                Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));

            }
            [Test]
            public void ExportAbsenceToExcel_DateIsEmpty_Exception()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                //Act 
                controller.WriteAbsenceData(workSheet, "", "");

                //Assert 

                Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));

            }

            [Test]
            public void ExportAbsenceToExcel_DateIsNull_Exception()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                //Act 
                controller.WriteAbsenceData(workSheet, null, null);

                //Assert 

                Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));

            }
            [Test]
            public void ExportAbsenceToExcel_From01012014To_28072014SearchStringIsEmpty_FileResult()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                //Act 
                controller.WriteAbsenceData(workSheet, "01.01.2014", "28.07.2014");

                //Assert 
                Assert.AreEqual("SDDDA", workSheet.Cells[1, 0].Value.ToString());
                Assert.AreEqual("Zarose Anastasia", workSheet.Cells[1, 1].Value.ToString());
                Assert.AreEqual("andl", workSheet.Cells[1, 2].Value.ToString());
                Assert.AreEqual("01.01.2014 - 01.01.2014", workSheet.Cells[1, 4].Value.ToString());
                Assert.AreEqual("21.02.2014 - 27.02.2014", workSheet.Cells[1, 6].Value.ToString());
                Assert.AreEqual("25.01.2014 - 05.02.2014\n12.02.2014 - 28.02.2014", workSheet.Cells[1, 7].Value.ToString());
           
            }

            [Test]
            public void ExportAbsenceToExcel_From01012014To_28072014SearchStringNotEmpty_FileResult()
            {
                //Arrange 
                AbsenceController controller = new AbsenceController(mock.Object);
                Worksheet workSheet = new Worksheet("Absence");
                //Act 
                controller.WriteAbsenceData(workSheet, "01.01.2014", "28.07.2014", "TAAA");

                //Assert 
                Assert.AreEqual("TAAAA", workSheet.Cells[1, 0].Value.ToString());
                Assert.AreEqual("Struz Anatoliy", workSheet.Cells[1, 1].Value.ToString());
                Assert.AreEqual("ascr", workSheet.Cells[1, 2].Value.ToString());
                Assert.AreEqual("01.02.2014 - 14.02.2014\n09.05.2014 - 09.06.2014", workSheet.Cells[1, 4].Value.ToString());
                Assert.AreEqual("21.02.2014 - 27.03.2014\n11.03.2014 - 27.03.2014", workSheet.Cells[1, 6].Value.ToString());
                Assert.AreEqual("01.03.2014 - 14.03.2014\n12.03.2014 - 28.03.2014", workSheet.Cells[1, 7].Value.ToString());

            }
            #endregion

        }
    }