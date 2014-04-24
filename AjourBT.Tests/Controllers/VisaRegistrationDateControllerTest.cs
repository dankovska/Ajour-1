using System;
using System.Web;
using NUnit.Framework;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using AjourBT.Controllers;
using System.Web.Mvc;
using AjourBT.Models;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;
using System.Web.Routing;
using System.Security.Principal;
using System.IO;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class VisaRegistrationDateControllerTest
    {
        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        Mock<ControllerContext> controllerContext;

        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
            messengerMock = new Mock<IMessenger>();

            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMUpdateVisaRegistrationDateToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMCreateVisaRegistrationDateToEMP))).Verifiable();

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost:50616", ""), new HttpResponse(new StringWriter()));
            var routeCollection = new RouteCollection();
            if (RouteTable.Routes.Count == 0)
            {
                routeCollection.MapRoute("Default", "Home/Index");
                System.Web.Routing.RouteTable.Routes.MapRoute("Default", "Home/Index");
            }

            var fakeHttpContext = new Mock<HttpContextBase>();
            var fakeIdentity = new GenericIdentity("andl");
            var principal = new GenericPrincipal(fakeIdentity, null);

            fakeHttpContext.Setup(t => t.User).Returns(principal);
            fakeHttpContext.Setup(t => t.User.Identity.Name).Returns("andl");
            controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);
            controllerContext.Setup(t => t.HttpContext.User.Identity.Name).Returns("andl");

            //List<Department> departments = new List<Department>{
            //    new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
            //    new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
            //    new Department{DepartmentID = 3, DepartmentName = "RAAA1",Employees = new List<Employee>()},
            //    new Department{DepartmentID = 4, DepartmentName = "RAAA2",Employees = new List<Employee>()},
            //    new Department{DepartmentID = 5, DepartmentName = "RAAA3",Employees = new List<Employee>()},
            //    new Department{DepartmentID = 6, DepartmentName = "RAAA4",Employees = new List<Employee>()},
            //    new Department{DepartmentID = 7, DepartmentName = "RAAA5",Employees = new List<Employee>()}
            //};

            //List<Employee> employees = new List<Employee>
            //{
            //    new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(11/01/2013), DateEmployed = new DateTime(11/02/2011), IsManager = false},
            //    new Employee {EmployeeID = 2, FirstName = "Anatoliy",LastName = "Struz",DepartmentID = 2, EID = "ascr", DateEmployed = new DateTime(11/04/2013), IsManager = true},          
            //    new Employee {EmployeeID = 3, FirstName = "Tymur",LastName = "Pyorge",DepartmentID = 1, EID = "tedk", DateEmployed = new DateTime(11/04/2013), IsManager = false, Department = departments.Find(d => d.DepartmentID == 1)},
            //    new Employee {EmployeeID = 4, FirstName = "Tanya",LastName = "Kowood",DepartmentID = 4 , EID = "tadk", DateEmployed = new DateTime(11/04/2012), IsManager = false},
            //    new Employee {EmployeeID = 5, FirstName = "Ivan",LastName = "Daolson",DepartmentID = 6, EID = "daol", DateEmployed = new DateTime(21/07/2013), IsManager = false},
            //    new Employee {EmployeeID = 6, FirstName = "Boryslav",LastName = "Teshaw",DepartmentID = 5, EID = "tebl", DateEmployed = new DateTime(11/04/2011), IsManager = false},
            //    new Employee {EmployeeID = 7, FirstName = "Tanya",LastName = "Manowens",DepartmentID = 5, EID = "xtwe", DateEmployed = new DateTime(09/04/2012), IsManager = false}
            //};

            //List<Visa> visas = new List<Visa>
            //{
            //    new Visa { EmployeeID = 1, VisaType = "D08", StartDate = new DateTime(2013,8,1), DueDate = new DateTime (2013,2,1 ), Days = 90,  DaysUsedInBT = 0, Entries = 0, EntriesUsedInBT = 0, VisaOf = employees.Find(e => e.EmployeeID == 1) },
            //    new Visa { EmployeeID = 2, VisaType = "C07", StartDate = new DateTime(2012,2,13), DueDate = new DateTime (2013,5,13), Days = 20,  DaysUsedInBT = 5, Entries = 2, EntriesUsedInBT = 4, VisaOf = employees.Find(e => e.EmployeeID == 2) }
            //};

            //List<VisaRegistrationDate> visaRegistrationDates = new List<VisaRegistrationDate>
            //{
            //    new VisaRegistrationDate {EmployeeID = 1, RegistrationDate = new DateTime(01/01/2013), VisaType="C04", VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 1)},
            //    new VisaRegistrationDate {EmployeeID = 2, RegistrationDate = new DateTime(02/10/2013), VisaType="D07", VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 2)},   
            //    new VisaRegistrationDate {EmployeeID = 3, RegistrationDate = new DateTime(03/01/2013), VisaType="C04", VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 3)},
            //    new VisaRegistrationDate {EmployeeID = 4, RegistrationDate = new DateTime(04/01/2013), VisaType="D07", VisaRegistrationDateOf = employees.Find(e => e.EmployeeID == 4)}
            //};

            //mock.Setup(m => m.Employees).Returns(employees.AsQueryable());
            //mock.Setup(m => m.Departments).Returns(departments.AsQueryable());
            //mock.Setup(m => m.Visas).Returns(visas.AsQueryable());
            //mock.Setup(m => m.VisaRegistrationDates).Returns(visaRegistrationDates.AsQueryable());

            //employees.Find(e => e.EmployeeID == 1).Department = departments.Find(v => v.DepartmentID == 1);
            //employees.Find(e => e.EmployeeID == 2).Department = departments.Find(v => v.DepartmentID == 2);
            //employees.Find(e => e.EmployeeID == 3).Department = departments.Find(v => v.DepartmentID == 1);
            //employees.Find(e => e.EmployeeID == 4).Department = departments.Find(v => v.DepartmentID == 4);
            //employees.Find(e => e.EmployeeID == 5).Department = departments.Find(v => v.DepartmentID == 6);
            //employees.Find(e => e.EmployeeID == 6).Department = departments.Find(v => v.DepartmentID == 5);
            //employees.Find(e => e.EmployeeID == 7).Department = departments.Find(v => v.DepartmentID == 5);

            //employees.Find(e => e.EmployeeID == 1).Visa = visas.Find(v => v.EmployeeID == 1);
            //employees.Find(e => e.EmployeeID == 2).Visa = visas.Find(v => v.EmployeeID == 2);

            //employees.Find(e => e.EmployeeID == 1).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 1);
            //employees.Find(e => e.EmployeeID == 2).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 2);
            //employees.Find(e => e.EmployeeID == 3).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 3);
            //employees.Find(e => e.EmployeeID == 4).VisaRegistrationDate = visaRegistrationDates.Find(vrd => vrd.EmployeeID == 4);
        }


        [Test]
        public void CreateGet_VisaRegistrationDateOf_ExistingEmployee()
        {
            // Arrange - create the controller                 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            // Act - call the action method 
            var result = target.Create(5,"as") as ViewResult;
            VisaRegistrationDate visaRegDate = (VisaRegistrationDate)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("as", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", result.ViewBag.JSDatePattern);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visaRegDate);
        }

        [Test]
        public void CreateGet_VisaRegistrationDateOf_ExistingEmployeeSearchStringEmpty()
        {
            // Arrange - create the controller                 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            // Act - call the action method 
            var result = target.Create(5) as ViewResult;
            VisaRegistrationDate visaRegDate = (VisaRegistrationDate)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", result.ViewBag.JSDatePattern);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visaRegDate);
        }

        [Test]
        public void CreateGet_VisaRegistrationDateOf_NotExistingEmployee()
        {
            // Arrange - create the controller                 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Create(1500);
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(visaRegDate);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void CreatePost_CanCreate_ValidVisaRegistrationDate()
        {
            // Arrange - create the controller                 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            VisaRegistrationDate visaRegDate = new VisaRegistrationDate { EmployeeID = 5, RegistrationDate = new DateTime(2013, 04, 01), VisaType = "D10", City = "Kyiv", RegistrationTime = "09:00", RegistrationNumber = "0001" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            target.ControllerContext = controllerContext.Object;

            // Act - call the action method 
            var result = target.Create(visaRegDate,"as");

            // Assert - check the result 
            mock.Verify(m => m.SaveVisaRegistrationDate(visaRegDate, 5), Times.Once);
            Assert.AreEqual("as", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

        }

        [Test]
        public void CreatePost_CannotCreate_InvalidVisaRegistrationDate()
        {
            // Arrange - create the controller
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            VisaRegistrationDate visaRegDate = new VisaRegistrationDate();

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.Create(visaRegDate) as ViewResult;


            // Assert - check the result 
            mock.Verify(m => m.SaveVisaRegistrationDate(It.IsAny<VisaRegistrationDate>(), It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(RegistrationDateViewModel), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void CreatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            VisaRegistrationDateController controller = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID)).Throws(new InvalidOperationException());

            //Act
            var result = controller.Create(visaRegDate);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void EditGet_CanEdit_ValidEmployeeID()
        {
            // Arrange - create the controller 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Edit(5,"st") as ViewResult;
            RegistrationDateViewModel visaRegDate = (RegistrationDateViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(5, visaRegDate.EmployeeID);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("st", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", result.ViewBag.JSDatePattern);

        }

        [Test]
        public void EditGet_CanEdit_ValidEmployeeIDSearchStruingEmpty()
        {
            // Arrange - create the controller 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Edit(5) as ViewResult;
            RegistrationDateViewModel visaRegDate = (RegistrationDateViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(5, visaRegDate.EmployeeID);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", result.ViewBag.JSDatePattern);

        }

        [Test]
        public void EditGet_CannotEdit_InvalidEmployeeID()
        {
            // Arrange - create the controller 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Edit(15);
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visaRegDate);
        }

        [Test]
        public void EditPost_CanEdit_ValidVisaRegistrationDate()
        {
            // Arrange - create the controller 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            VisaRegistrationDate visaRegDate = new VisaRegistrationDate { EmployeeID = 5, RegistrationDate = new DateTime(2013, 04, 01), VisaType = "D10" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            target.ControllerContext = controllerContext.Object;

            // Act - call the action method 
            var result = target.Edit(visaRegDate);

            // Assert - check the result 
            mock.Verify(m => m.SaveVisaRegistrationDate(visaRegDate, 5), Times.Once);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);

        }

        [Test]
        public void EditPost_CannotEdit_InvalidVisaRegistrationDate()
        {
            // Arrange - create the controller 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            VisaRegistrationDate visaRegDate = new VisaRegistrationDate();

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.Edit(visaRegDate);

            // Assert - check the result 
            mock.Verify(m => m.SaveVisaRegistrationDate(visaRegDate, 1), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(RegistrationDateViewModel),((ViewResult)result).ViewData.Model);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            VisaRegistrationDateController controller = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID)).Throws(new DbUpdateConcurrencyException());

            //Act
            var result = controller.Edit(visaRegDate);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveVisaRegistrationDate(visaRegDate, visaRegDate.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void DeletePost_ValidVisaRegistrationDate()
        {
            // Arrange - create the controller 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.DeleteConfirmed(1,"as");

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisaRegistrationDate(1), Times.Once);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("as", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);

        }

        [Test]
        public void DeletePost_InvalidEmployeeID()
        {
            // Arrange - create the controller 
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.DeleteConfirmed(15);
            VisaRegistrationDate visaRegDate = mock.Object.VisaRegistrationDates.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisaRegistrationDate(15), Times.Never);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visaRegDate);
        }
        [Test]
        public void GetEmployeeData_StringEmpty_AllEmployees()
        {
            //Arrange
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            // Act - call the action method 
            var result = target.GetEmployeeData(mock.Object.Employees.ToList(), "");
            // Assert - check the result 
            Assert.AreEqual(24, result.ToArray().Length);
            Assert.AreEqual("Kowwood", result.ToArray()[0].LastName);
            Assert.AreEqual("Only", result.ToArray()[6].LastName);
            CollectionAssert.AllItemsAreInstancesOfType(result.ToList(), typeof(Employee));

        }

        [Test]
        public void GetEmployeeData_String0801_SelectedEmployees()
        {
            //Arrange
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            // Act - call the action method 
            var result = target.GetEmployeeData(mock.Object.Employees.ToList(), "8/1/2012");
            // Assert - check the result 
            Assert.AreEqual(1, result.ToArray().Length);
            Assert.AreEqual("Zarose", result.ToArray()[0].LastName);
            CollectionAssert.AllItemsAreInstancesOfType(result.ToList(), typeof(Employee));

        }
        [Test]
        public void GetEmployeeData_StringABRAKADABRA_NothingFound()
        {
            //Arrange
            VisaRegistrationDateController target = new VisaRegistrationDateController(mock.Object, messengerMock.Object);
            // Act - call the action method 
            var result = target.GetEmployeeData(mock.Object.Employees.ToList(), "ABRAKADABRA");
            // Assert - check the result 
            Assert.AreEqual(0, result.ToArray().Length);

        }

    }
}