using System;
using System.IO;
using NUnit.Framework;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using AjourBT.Controllers;
using System.Web.Mvc;
using AjourBT.Models;
using System.Web;
using System.Web.Routing;
using System.Security.Principal;
using AjourBT.Infrastructure;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;


namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class PermitControllerTest
    {
        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        Mock<ControllerContext> controllerContext;

        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
            messengerMock = new Mock<IMessenger>();

            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMCancelsPermitToADM))).Verifiable();

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost:50616", ""), new HttpResponse(new StringWriter()));
            var routeCollection = new RouteCollection();
            if (RouteTable.Routes.Count == 0)
            {
                routeCollection.MapRoute("Default", "Home/Index");
                System.Web.Routing.RouteTable.Routes.MapRoute("Default", "Home/Index");
            }

            var fakeHttpContext = new Mock<HttpContextBase>();
            var fakeIdentity = new GenericIdentity("User");
            var principal = new GenericPrincipal(fakeIdentity, null);

            fakeHttpContext.Setup(t => t.User).Returns(principal);
            fakeHttpContext.Setup(t => t.User.Identity.Name).Returns("User");
            controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);
            controllerContext.Setup(t => t.HttpContext.User.Identity.Name).Returns("User");
        }

        [Test]
        public void CreateGet_PermitOf_SearchStringEmpty_ExistingEmployee()
        {
            // Arrange - create the controller                 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Create(5) as ViewResult;
            Permit permit = (Permit)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(permit);
        }

        [Test]
        public void CreateGet_PermitOf_SearchStringA_ExistingEmployee()
        {
            // Arrange - create the controller                 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Create(5,"A") as ViewResult;
            Permit permit = (Permit)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.IsNull(permit);
        }
        [Test]
        public void CreateGet_PermitOf_NotExistingEmployee()
        {
            // Arrange - create the controller                 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Create(1500);
            Permit permit = mock.Object.Permits.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(permit);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }

        [Test]
        public void CreatePost_CanCreateSearchStringEmpty_ValidPermit()
        {
            // Arrange - create the controller                 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            Permit permit = new Permit { EmployeeID = 5, Number = "11/2013", StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), IsKartaPolaka = false };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Create(permit);

            // Assert - check the result 
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void CreatePost_CanCreateSearchStringA_ValidPermit()
        {
            // Arrange - create the controller                 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            Permit permit = new Permit { EmployeeID = 5, Number = "11/2013", StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), IsKartaPolaka = false };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Create(permit,"A");

            // Assert - check the result 
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidPermit()
        {
            // Arrange - create the controller
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            Permit permit = new Permit();

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.Create(permit) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SavePermit(It.IsAny<Permit>(), It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(PermitViewModel), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void CreatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Permit permit = mock.Object.Permits.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SavePermit(permit, permit.EmployeeID)).Throws(new InvalidOperationException());

            //Act
            var result = target.Create(permit);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePermit(permit, permit.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void EditGet_CanEditSearchStringEmpty_ValidEmployeeID()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;
            PermitViewModel permit = (PermitViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, permit.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void EditGet_CanEditSearchStringA_ValidEmployeeID()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Edit(2,"A") as ViewResult;
            PermitViewModel permit = (PermitViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePAttern);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);

            Assert.AreEqual(2, permit.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void EditGet_CannotEdit_InvalidEmployeeID()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Edit(15);
            Permit permit = mock.Object.Permits.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(permit);
        }

        [Test]
        public void EditPost_CanEditSearchStringEmpty_ValidPermit()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            Permit permit = new Permit { EmployeeID = 5, Number = "11/2013", StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), IsKartaPolaka = false };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Edit(permit);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCancelsPermitToADM))), Times.Never);
        }
        [Test]
        public void EditPost_CanEditSearchStringA_ValidPermit()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            Permit permit = new Permit { EmployeeID = 5, Number = "11/2013", StartDate = new DateTime(2013, 11, 01), EndDate = new DateTime(2014, 05, 01), IsKartaPolaka = false };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Edit(permit,"A");

            // Assert - check the result 
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCancelsPermitToADM))), Times.Never);
        }

        [Test]
        public void EditPost_CancellationRequestCanEdit_ValidPermit()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            Permit permit = mock.Object.Permits.Where(p => p.EmployeeID == 5).FirstOrDefault();
            target.ControllerContext = controllerContext.Object;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.Edit(permit);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SavePermit(permit, 5), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => (msg.messageType.Equals(MessageType.BTMCancelsPermitToADM)) && msg.ReplyTo == "User User")), Times.Once);
        }

        [Test]
        public void EditPost_CannotEdit_InvalidPermit()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            Permit permit = new Permit();

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.Edit(permit);

            // Assert - check the result 
            mock.Verify(m => m.SavePermit(permit, 1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMCancelsPermitToADM))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PermitViewModel), ((ViewResult)result).Model);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Permit permit = mock.Object.Permits.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SavePermit(permit, permit.EmployeeID)).Throws(new DbUpdateConcurrencyException());

            //Act
            var result = target.Edit(permit);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePermit(permit, permit.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void DeleteGet_SearchStringEmptyValidEmployeeID()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.Delete(2) as ViewResult;
            Permit permit = (Permit)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(Permit), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, permit.EmployeeID);
        }

        [Test]
        public void DeleteGet_SearchStringDValidEmployeeID()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = target.Delete(2,"D") as ViewResult;
            Permit permit = (Permit)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual("D", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(Permit), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, permit.EmployeeID);
        }

        [Test]
        public void DeleteGet_InvalidEmployeeID()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Delete(15);
            Permit permit = mock.Object.Permits.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.DeletePermit(It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(permit);
        }

        [Test]
        public void DeletePost_SearchStringEmptyValidPermit()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.DeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeletePermit(1), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void DeletePost_SearchStringDValidPermit()
        {
            // Arrange - create the controller 
            PermitController target = new PermitController(mock.Object, messengerMock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = target.DeleteConfirmed(1,"D");

            // Assert - check the result 
            mock.Verify(m => m.DeletePermit(1), Times.Once);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("D", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        #region GetEmployeeData
        [Test]
        public void GetEmployeeData_ListNotEmptySearchStringEmpty_AllEmployees()
        {
            //Arrange
            PermitController controller = new PermitController(mock.Object, messengerMock.Object);
            List<Employee> empList = mock.Object.Employees.ToList();
            List<Employee> resultList = new List<Employee>();
            string searchString = "";
            //Act
            resultList = controller.GetEmployeeData(empList, searchString);
            //Assert
            Assert.AreEqual(empList.Count, resultList.Count);
            Assert.AreEqual("Oleksiy", resultList.ToArray()[0].FirstName);
            Assert.AreEqual("Kowwood", resultList.ToArray()[0].LastName);
            Assert.AreEqual("PaidVac", resultList.ToArray()[6].FirstName);
            Assert.AreEqual("Only", resultList.ToArray()[6].LastName);
        }
        [Test]
        public void GetEmployeeData_ListEmptySearchStringEmpty_AllEmployees()
        {
            //Arrange
            PermitController controller = new PermitController(mock.Object, messengerMock.Object);
            List<Employee> empList = new List<Employee>();
            List<Employee> resultList = new List<Employee>();
            string searchString = "";
            //Act
            resultList = controller.GetEmployeeData(empList, searchString);
            //Assert
            Assert.AreEqual(0, resultList.Count);

        }
        [Test]
        public void GetEmployeeData_ListNotEmptySearchStringAnd_AllEmployees()
        {
            //Arrange
            PermitController controller = new PermitController(mock.Object, messengerMock.Object);
            List<Employee> empList = mock.Object.Employees.ToList();
            List<Employee> resultList = new List<Employee>();
            string searchString = "And";
            //Act
            resultList = controller.GetEmployeeData(empList, searchString);
            //Assert
            Assert.AreEqual(1, resultList.Count);
            Assert.AreEqual("Anastasia", resultList.ToArray()[0].FirstName);
            Assert.AreEqual("Zarose", resultList.ToArray()[0].LastName);

        }


        #endregion

    }
}