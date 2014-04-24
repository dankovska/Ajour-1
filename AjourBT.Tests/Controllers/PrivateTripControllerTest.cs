using System;
using Moq;
using NUnit.Framework;
using AjourBT.Domain.Abstract;
using AjourBT.Controllers;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AjourBT.Models;
using AjourBT.Infrastructure;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;


namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class PrivateTripControllerTest
    {
        Mock<IRepository> mock;
        PrivateTripController controller;

        [SetUp]
        public void SetUp()
        {
            mock = mock = Mock_Repository.CreateMock();

            controller = new PrivateTripController(mock.Object);

        }

        #region GetPrivateTripBTM

        [Test]
        public void GetPrivateTripBTM_EmptyString_searchString()
        {
            // Arrange
            string searchString = "";

            // Act

            var view = controller.GetPrivateTripBTM(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripBTM_NullString_searchString()
        {
            // Arrange
            string searchString = null;

            // Act
            var view = controller.GetPrivateTripBTM(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
        }

        [Test]
        public void GetPrivateTripBTM_dan_searchString()
        {
            // Arrange
            string searchString = "dan";

            // Act

            var view = controller.GetPrivateTripBTM(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual("dan", ((ViewResult)view).ViewBag.SearchString);
        }

        #endregion

        #region GetPrivateTripVU

        [Test]
        public void GetPrivateTripVU_EmptyString_searchString()
        {
            // Arrange
            string searchString = "";

            // Act

            var view = controller.GetPrivateTripVU(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual("", ((ViewResult)view).Model);
        }

        [Test]
        public void GetPrivateTripVU_NullString_searchString()
        {
            // Arrange
            string searchString = null;

            // Act
            var view = controller.GetPrivateTripVU(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
        }

        [Test]
        public void GetPrivateTripVU_dan_searchString()
        {
            // Arrange
            string searchString = "dan";

            // Act

            var view = controller.GetPrivateTripVU(searchString);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual("dan", ((ViewResult)view).Model);
        }

        #endregion

        #region GetPrivateTripDataBTM

        [Test]
        public void GetPrivateTripData_Default_AllEmployees()
        {
            // Arrange
            string searchString = "";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataBTM(searchString).Model;
            var view = controller.GetPrivateTripDataBTM(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            CollectionAssert.AreEqual(employees, result);
            Assert.AreEqual(result.ToArray()[0].LastName, "Kowwood");
            Assert.AreEqual(result.ToArray()[1].LastName, "Struz");
            Assert.AreEqual(23, result.ToArray().Length);
            Assert.AreEqual(result.ToArray()[2].LastName, "Daolson");
            Assert.AreEqual(result.ToArray()[3].LastName, "Kowood");
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripDataBTM_FilterTep_EmployeesContain_Tep()
        {
            // Arrange - create the controller     
            string searchString = "ted";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataBTM(searchString).Model;
            var view = controller.GetPrivateTripDataBTM(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            CollectionAssert.AreEqual(employees, result);
            Assert.AreEqual(result.ToArray()[0].LastName, "Pyorge");
            Assert.AreEqual(result.ToArray()[0].FirstName, "Tymur");
            Assert.AreEqual(result.ToArray()[0].EID, "tedk");
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripDataBTM_Filteraa_EmployeesContain_aa()
        {
            // Arrange
            string searchString = "aa";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataBTM(searchString).Model;
            var view = controller.GetPrivateTripDataBTM(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual(0, result.ToArray().Length);
            Assert.AreEqual(searchString, ((PartialViewResult)view).ViewBag.SearchString);
        }

        #endregion

        #region GetPrivateTripDataVU

        [Test]
        public void GetPrivateTripDataVU_Default_AllEmployees()
        {
            // Arrange
            string searchString = "";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataVU(searchString).Model;
            var view = controller.GetPrivateTripDataVU(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            CollectionAssert.AreEqual(employees, result);
            Assert.AreEqual(result.ToArray()[0].LastName, "Kowwood");
            Assert.AreEqual(result.ToArray()[1].LastName, "Struz");
            Assert.AreEqual(23, result.ToArray().Length);
            Assert.AreEqual(result.ToArray()[2].LastName, "Daolson");
            Assert.AreEqual(result.ToArray()[3].LastName, "Kowood");
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripDataVU_FilterTep_EmployeesContain_Tep()
        {
            // Arrange - create the controller     
            string searchString = "ted";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataVU(searchString).Model;
            var view = controller.GetPrivateTripDataVU(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            CollectionAssert.AreEqual(employees, result);
            Assert.AreEqual(result.ToArray()[0].LastName, "Pyorge");
            Assert.AreEqual(result.ToArray()[0].FirstName, "Tymur");
            Assert.AreEqual(result.ToArray()[0].EID, "tedk");
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void GetPrivateTripDataVU_Filteraa_EmployeesContain_aa()
        {
            // Arrange
            string searchString = "aa";

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetPrivateTripDataVU(searchString).Model;
            var view = controller.GetPrivateTripDataVU(searchString);
            IEnumerable<Employee> employees = mock.Object.Employees
                                            .Where(e => e.DateDismissed == null
                                                    && (e.EID.Contains(searchString)
                                                    || e.LastName.Contains(searchString)
                                                    || e.FirstName.Contains(searchString)))
                                            .OrderByDescending(e => e.IsManager)
                                            .ThenBy(e => e.LastName);

            // Assert - check the result
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(0, result.ToArray().Length);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        #endregion

        #region CreateGet
        [Test]
        public void Create_ExistingVisaDefault_CreatePTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            var viewBagEmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;

            //Act
            var result = controller.Create(1);

            //Assert       
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(viewBagEmployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
        }

        [Test]
        public void Create_ExistingVisaStringEmpty_CreatePTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            var viewBagEmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;

            //Act
            string searchString = "";
            var result = controller.Create(1, searchString);

            //Assert       
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(viewBagEmployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void Create_ExistingVisa_dan_CreatePTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            var viewBagEmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;

            //Act
            string searchString = "dan";
            var result = controller.Create(1, searchString);

            //Assert       
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(viewBagEmployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void Create_NonExistingVisa_CreateBTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1000 select e).FirstOrDefault();

            //Act
            var result = controller.Create(1000);
            //Assert       
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        #endregion

        #region CreatePost

        [Test]
        public void CreatePost_CreatedPT_CreatePTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 1 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.Create(privateTrip,"sd");

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(32, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(2, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("sd", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void CreatePost_PTSerchStringEmpty_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 1 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.Create(privateTrip, "");

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(32, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(2, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void CreatePost_PTSerchString_dan_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 1 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.Create(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(32, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(2, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void CreatePost_PTSerchString_danVisaNull_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 10 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 10).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.Create(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(null, visa);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void CreatePost_PTSerchString_danVisaNotNull_AndDaysEntriesAreNull_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 1000, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-190), EmployeeID = 11 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.Create(privateTrip, searchString);

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 11), Times.Once);
            Assert.IsNotNull(visa);
            Assert.AreEqual(null, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(null, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void CreatePost_NotValidBT_dan_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 0, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-180), EmployeeID = 1 };
            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = (ViewResult)controller.Create(privateTrip, "dan");

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Never);
            mock.Verify(m => m.SaveVisa(It.IsAny<Visa>(), It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), result.Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void CreatePost_NotValidBTNull_CreateBTForm()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 0, StartDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-200), EndDate = DateTime.Now.ToLocalTimeAzure().ToLocalTimeAzure().AddDays(-180), EmployeeID = 1 };

            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = (ViewResult)controller.Create(privateTrip);

            //Assert   
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Never);
            mock.Verify(m => m.SaveVisa(It.IsAny<Visa>(), It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), result.Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
        }

        
        #endregion

        #region EditGet

        [Test]
        public void EditGet_CannotEdit_InvalidPrivateTripID()
        {
            // Arrange 
            string searchString = "t";

            // Act - call the action method
            var result = (HttpNotFoundResult)controller.Edit(0, searchString);
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(p => p.PrivateTripID == 0).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(privateTrip);
        }

        [Test]
        public void EditGet_CanEdit_ValidPrivateTripID()
        {
            // Arrange 
            string searchString = "";

            // Act - call the action method
            var result = controller.Edit(2, searchString);
            var privateTripModel = (result as ViewResult).Model;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), privateTripModel);
            Assert.AreEqual(2, ((PrivateTripViewModel)privateTripModel).PrivateTripID);
            Assert.AreEqual(3, ((PrivateTripViewModel)privateTripModel).EmployeeID);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void EditGet_CanEditNullSearchString_ValidPrivateTripID()
        {
            // Arrange 
            string searchString = null;

            // Act - call the action method
            var result = controller.Edit(2, searchString);
            var privateTripModel = (result as ViewResult).Model;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), privateTripModel);
            Assert.AreEqual(2, ((PrivateTripViewModel)privateTripModel).PrivateTripID);
            Assert.AreEqual(3, ((PrivateTripViewModel)privateTripModel).EmployeeID);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        [Test]
        public void EditGet_CanEdit_TeSearchString_ValidPrivateTripID()
        {
            // Arrange 
            string searchString = "Te";

            // Act - call the action method
            var result = controller.Edit(2, searchString);
            var privateTripModel = (result as ViewResult).Model;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            Assert.IsInstanceOf(typeof(PrivateTripViewModel), privateTripModel);
            Assert.AreEqual(2, ((PrivateTripViewModel)privateTripModel).PrivateTripID);
            Assert.AreEqual(3, ((PrivateTripViewModel)privateTripModel).EmployeeID);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
        }

        #endregion

        #region EditPost

        [Test]
        public void EditPost_CanEdit_ValidPrivateTrip()
        {
            // Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 2, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-25), EmployeeID = 3 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 3).FirstOrDefault();
            string searchString = "Te";
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.Edit(privateTrip, searchString);

            // Assert - check the result 
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Te", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(1, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(16,visa.DaysUsedInPrivateTrips);
        }

        [Test]
        public void EditPost_CanEdit_VisaNull_ValidPrivateTrip()
        {
            // Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 8, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-40), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-25), EmployeeID = 10 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 10).FirstOrDefault();
            string searchString = "Te";
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.Edit(privateTrip, searchString);

            // Assert - check the result 
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Te", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(null, visa);
        }

        [Test]
        public void EditPost_CanEdit_VisaNotNull_DaysEntriesAreNull_ValidPrivateTrip()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 9, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-8), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 11 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.Edit(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 11), Times.Once);
            Assert.IsNotNull(visa);
            Assert.AreEqual(null, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(null, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void EditPost_CanEdit_VisaNotNull_EntriesAreNull_ValidPrivateTrip()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 10, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-9), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 12 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.Edit(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 12), Times.Once);
            Assert.IsNotNull(visa);
            Assert.AreEqual(null, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(3, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void EditPost_CanEdit_VisaNotNull_DaysAreNull_ValidPrivateTrip()
        {
            //Arrange
            PrivateTrip privateTrip = new PrivateTrip { PrivateTripID = 11, StartDate = DateTime.Now.ToLocalTimeAzure().AddDays(-9), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(-7), EmployeeID = 13 };
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            string searchString = "dan";
            var result = controller.Edit(privateTrip, searchString);

            //Assert  
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 13), Times.Once);
            Assert.IsNotNull(visa);
            Assert.AreEqual(1, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual(null, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void EditPost_CannotEdit_InvalidPrivateTrip()
        {
            // Arrange
            PrivateTrip privateTrip = new PrivateTrip();
            string searchString = "";

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            JsonResult result = (JsonResult)controller.Edit(privateTrip, searchString);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SavePrivateTrip(privateTrip), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("", data);
            //Assert.AreEqual("", result.ViewName);
            //Assert.IsInstanceOf(typeof(PrivateTripViewModel), result.ViewData.Model);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SavePrivateTrip(It.IsAny<PrivateTrip>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.Edit(mock.Object.PrivateTrips.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePrivateTrip(It.IsAny<PrivateTrip>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region DeleteGet
        [Test]
        public void DeleteGet_ValidPrivateTripIDAndStringEmpty()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();

            // Act - call the action method 
            string searchString = "";
            var result = controller.Delete(1);
            var view = ((ViewResult)result).Model;
            string mployeeInformation = "Delete Private trip of Zarose Anastasia";

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PrivateTrip), view);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsNotNull(view);
            Assert.AreEqual(1, ((PrivateTrip)view).PrivateTripID);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual(mployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
        }

        [Test]
        public void DeleteGet_ValidPrivateTripIDAndEmptyString()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();

            // Act - call the action method 
            string searchString = "";
            var result = controller.Delete(1, searchString);
            var view = ((ViewResult)result).Model;
            string mployeeInformation = "Delete Private trip of Zarose Anastasia";

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PrivateTrip), view);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsNotNull(view);
            Assert.AreEqual(1, ((PrivateTrip)view).PrivateTripID);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual(mployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
        }

        [Test]
        public void DeleteGet_ValidPrivateTripIDAnd_tep()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();

            // Act - call the action method 
            string searchString = "tep";
            var result = controller.Delete(1, searchString);
            var view = ((ViewResult)result).Model;
            string mployeeInformation = "Delete Private trip of Zarose Anastasia";

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PrivateTrip), view);
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.IsNotNull(view);
            Assert.AreEqual(1, ((PrivateTrip)view).PrivateTripID);
            Assert.AreEqual(searchString, ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual(mployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
        }


        [Test]
        public void DeleteGet_ValidPrivateTripID_AndNull_AndInvalidEmployeeID()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();
            privateTrip.EmployeeID = 100;
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();

            // Act - call the action method 
            var result = controller.Delete(1);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNotNull(privateTrip);
            Assert.IsNull(employee);
        }

        [Test]
        public void DeleteGet_ValidPrivateTripID_AndEmptyString_AndInvalidEmployeeID()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();
            privateTrip.EmployeeID = 100;
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();

            // Act - call the action method 
            string searchString = "";
            var result = controller.Delete(1, searchString);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNotNull(privateTrip);
            Assert.IsNull(employee);
        }

        [Test]
        public void DeleteGet_ValidPrivateTripID_And_tep_AndInvalidEmployeeID()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 1).FirstOrDefault();
            privateTrip.EmployeeID = 100;
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();

            // Act - call the action method 
            string searchString = "tep";
            var result = controller.Delete(1, searchString);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNotNull(privateTrip);
            Assert.IsNull(employee);
        }

        [Test]
        public void DeleteGet_InValidPrivateTripID_AndNull()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 100).FirstOrDefault();

            // Act - call the action method 
            var result = controller.Delete(100);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNull(privateTrip);
        }

        [Test]
        public void DeleteGet_InValidPrivateTripID_AndEmptyString()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 100).FirstOrDefault();

            // Act - call the action method 
            string searchString = "";
            var result = controller.Delete(100, searchString);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNull(privateTrip);
        }

        [Test]
        public void DeleteGet_InValidPrivateTripID_And_tep()
        {
            // Arrange - create the controller 
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(pt => pt.PrivateTripID == 100).FirstOrDefault();

            // Act - call the action method 
            string searchString = "tep";
            var result = controller.Delete(100, searchString);

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            Assert.IsNull(privateTrip);
        }

        #endregion

        #region DeleteConfirmed

        [Test]
        public void DeleteConfirmed_ValidPrivateTripID_AndNull__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method
            var result = controller.DeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeletePrivateTrip(1), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(0, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(0, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void DeleteConfirmed_ValidPrivateTripID_AndStringNotEmpty__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method
            string searchString = "a";
            var result = controller.DeleteConfirmed(1, searchString);

            // Assert - check the result
            mock.Verify(m => m.DeletePrivateTrip(1), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(0, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(0, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("a", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void DeleteConfirmed_ValidPrivateTripID_And_tep__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method
            string searchString = "tep";
            var result = controller.DeleteConfirmed(1, searchString);

            // Assert - check the result
            mock.Verify(m => m.DeletePrivateTrip(1), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(0, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(0, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("tep", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void DeleteConfirmed_ValidPrivateTripID_And_tep_VisaNotNullAndDaysEntriesNull__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            // Act - call the action method
            string searchString = "tep";
            var result = controller.DeleteConfirmed(9, searchString);

            // Assert - check the result
            mock.Verify(m => m.DeletePrivateTrip(9), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, visa.EmployeeID), Times.Once);
            Assert.AreEqual(null, visa.DaysUsedInPrivateTrips);
            Assert.AreEqual(null, visa.EntriesUsedInPrivateTrips);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("tep", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void DeleteConfirmed_ValidPrivateTripID_tep_VisaNull__PTIsDeleted()
        {
            //Arrange
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(p => p.EmployeeID == 10).FirstOrDefault();
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == privateTrip.EmployeeID).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            string searchString = "tep";
            var result = controller.DeleteConfirmed(privateTrip.PrivateTripID, searchString);

            // Assert - check the result 
            mock.Verify(m => m.DeletePrivateTrip(8), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(null, visa);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("tep", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void DeleteConfirmed_ValidPrivateTripID_EmptySearchString_VisaNull__PTIsDeleted()
        {
            //Arrange
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 10).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            // Act - call the action method 
            string searchString = "";
            var result = controller.DeleteConfirmed(8, searchString);

            // Assert - check the result 
            mock.Verify(m => m.DeletePrivateTrip(8), Times.Once);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        [Test]
        public void DeleteConfirmed_InValidPrivateTripID_EmptySearchString_VisaNull__PTIsNotDeleted()
        {
            //Arrange
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(p => p.EmployeeID == 100).FirstOrDefault();
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 100).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            string searchString = "";
            var result = controller.DeleteConfirmed(100, searchString);

            // Assert - check the result 
            mock.Verify(m => m.DeletePrivateTrip(100), Times.Never);
            mock.Verify(m => m.SaveVisa(visa, 10), Times.Never);
            Assert.AreEqual(null, privateTrip);
            Assert.AreEqual(null, visa);
            Assert.AreEqual("TableViewPTBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }

        
        #endregion


        #region CountingDaysUsedInPT

        [Test]
        public void CountingDaysUsedInBT_ReportedBT_IncreasedDaysUsedInBT()
        {
            //Arrange
            PrivateTrip privateTrip = mock.Object.PrivateTrips.Where(b => b.PrivateTripID == 1).FirstOrDefault();

            //Act
            int result = controller.CountingDaysUsedInPT(privateTrip);

            //Assert
            Assert.AreEqual(21, result);
        }

        #endregion

     [Test]
	public void SearchPrivateTripData_StringEmpty_AllEmpployees()
	{
         //Arrange

         //Act
        var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "");

         //Assert
        Assert.AreEqual(23, resultList.Count);
        Assert.AreEqual("Oleksiy", resultList.ToArray()[0].FirstName);
        Assert.AreEqual("Kowwood", resultList.ToArray()[0].LastName);
        Assert.AreEqual("PaidVac", resultList.ToArray()[6].FirstName);
        Assert.AreEqual("Only", resultList.ToArray()[6].LastName);
        CollectionAssert.AllItemsAreInstancesOfType(resultList, typeof(Employee));
		
	}
     [Test]
     public void SearchPrivateTripData_StringABRAKADARA_NothingFound()
     {
         //Arrange

         //Act
         var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "ABRAKADARA");

         //Assert
         Assert.AreEqual(0, resultList.Count);


     }

     [Test]
     public void SearchPrivateTripData_StringAN_SelectedEmployees()
     {
         //Arrange

         //Act
         var resultList = controller.SearchPrivateTripData(mock.Object.Employees.ToList(), "AN");

         //Assert
         Assert.AreEqual(6, resultList.Count);
         Assert.AreEqual("Anatoliy", resultList.ToArray()[0].FirstName);
         Assert.AreEqual("Struz", resultList.ToArray()[0].LastName);
         Assert.AreEqual("Ivan", resultList.ToArray()[1].FirstName);
         Assert.AreEqual("Daolson", resultList.ToArray()[1].LastName);


     }
	
    }
}
