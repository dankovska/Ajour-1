using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using AjourBT.Models;
using AjourBT.Tests.MockRepository;
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
                var result = controller.GetAbsenceData(fromDate, toDate) as ViewResult;

                //Assert
                Assert.AreEqual("NoData", result.ViewName);
            }

            [Test]
            public void GetAbsenceDataCalendarItemsNull_NoDataView()
            {
                //Arrange
                AbsenceController controller = new AbsenceController(mock.Object);
                string fromDate = "10.11.1000";
                string toDate = "10.11.1000";

                //Act
                var result = controller.GetAbsenceData(fromDate, toDate) as ViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoData", result.ViewName);
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
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as ViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoData", result.ViewName);
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
                var result = controller.GetAbsenceData(fromDate, toDate, searchString) as ViewResult;
                var modelResult = result.Model as List<AbsenceViewModel>;

                //Assert
                Assert.AreEqual("NoData", result.ViewName);
            }

            #endregion

        }
    }