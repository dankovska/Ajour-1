using System;
using AjourBT.Domain.Abstract;
using NUnit.Framework;
using Moq;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using AjourBT.Controllers;
using System.Linq;
using AjourBT.Models;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class PassportControllerTest
    {
        Mock<IRepository> mock;
        PassportController controller;

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            controller = new PassportController(mock.Object);

         }


        #region ModifyPassport
        [Test]
        public void ModifyPassport_ValidIDisCheckedNullEmployeeHasPassport_TrueDeletePassport()
        {
            //Arrange
            PassportController controller = new PassportController(mock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            ActionResult result = controller.ModifyPassport("1", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ModifyPassport_ValidIDisCheckedNullEmployeeHasNoPassport_True()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            //Act
            ActionResult result = controller.ModifyPassport("2", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ModifyPassport_SearckStringAValidIDisCheckedNotNullEmployeeHasPassport_TrueSavePassport()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            //Act
            ActionResult result = controller.ModifyPassport("2", "checked","A");

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Once);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ModifyPassport_SearchStringEmptyValidIDisCheckedNotNullEmployeeHasNoPassport_True()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            //Act
            var result = controller.ModifyPassport("1", "checked");

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Once);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

        }

        [Test]
        public void ModifyPassport_NotValidIDNotParsable_False()
        {
            //Arrange


            //Act
            var result = (HttpNotFoundResult)controller.ModifyPassport("abc", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ModifyPassport_NotValidIDZero_False()
        {
            //Arrange
            PassportController controller = new PassportController(mock.Object);

            //Act
            var result = (HttpNotFoundResult)controller.ModifyPassport("0", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ModifyPassport_NotValidIDNull_False()
        {
            //Arrange


            //Act
            var result = (HttpNotFoundResult)controller.ModifyPassport(null, null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ModifyPassport_NotValidIDNotExistingEmployee_False()
        {
            //Arrange

            //Act
            var result = (HttpNotFoundResult)controller.ModifyPassport("10000", null);

            //Assert  
            mock.Verify(m => m.SavePassport(It.IsAny<Passport>()), Times.Never);
            mock.Verify(m => m.DeletePassport(It.IsAny<int>()), Times.Never);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void ModifyPassport_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            PassportController controller = new PassportController(mock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            mock.Setup(m => m.SavePassport(It.IsAny<Passport>())).Throws(new InvalidOperationException());

            //Act
            var result = controller.ModifyPassport("2", "checked");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePassport(It.IsAny<Passport>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }



        #endregion

        #region AddDateGet

        [Test]
        public void AddDateGet_DaSearchString_ExistingEmployee()
        {
            // Arrange                          
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.AddDate(5, "da") as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("da", result.ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.IsInstanceOf(typeof(PassportViewModel),passport);
        }

        [Test]
        public void AddDateGet_ABBBSSSearchString_ExistingEmployee()
        {
            // Arrange                          

            // Act - call the action method 
            var result = controller.AddDate(5, "ABBBSS") as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("ABBBSS", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(PassportViewModel), passport);
        }

        [Test]
        public void AddDateGet_EmptySearchString_ExistingEmployee()
        {
            // Arrange                          

            // Act - call the action method 
            var result = controller.AddDate(5) as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Daolson Ivan (daol) from RAAA4", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("", result.ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(PassportViewModel), passport);
        }

        [Test]
        public void AddDateGet_PassportOf_NotExistingEmployee()
        {
            // Arrange                

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.AddDate(1500);
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(passport);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);

        }

        #endregion

        #region AddDatePost

        [Test]
        public void AddDatePost_CanCreateAndEmptySearchString_ValidPassport()
        {
            // Arrange                
            Passport passport = new Passport { EmployeeID = 5, EndDate = new DateTime(2013,04,01)};
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.AddDate(passport);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);

        }

        [Test]
        public void AddDatePost_CanCreateAndADDDASearchString_ValidPassport()
        {
            // Arrange                
            Passport passport = new Passport { EmployeeID = 5, EndDate = new DateTime(2013, 04, 01) };

            // Act - call the action method 
            var result = controller.AddDate(passport, "ADDDA");

            // Assert - check the result 
            mock.Verify(m => m.SavePassport(passport), Times.Once);
            Assert.AreEqual("ADDDA", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void AddDatePost_CannotCreateAndEmptySearchString_InvalidPassport()
        {
            // Arrange
            Passport passport = new Passport();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.AddDate(passport, "");
           // string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SavePassport(passport), Times.Never);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(PassportViewModel), ((ViewResult)result).ViewData.Model);
        }

        [Test]
        public void AddDatePost_CannotCreateAndDaSearchString_InvalidPassport()
        {
            // Arrange
            Passport passport = new Passport();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.AddDate(passport, "Da");

            // Assert - check the result 
            mock.Verify(m => m.SavePassport(passport), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Da", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(PassportViewModel), ((ViewResult)result).ViewData.Model);
        }

        [Test]
        public void AddDatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SavePassport(It.IsAny<Passport>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                                    + "was modified by another user after you got the original value. The "
                                    + "edit operation was canceled.";
            Passport passport = new Passport { EmployeeID = 5, EndDate = new DateTime(2013, 04, 01) };

            //Act
            JsonResult result = (JsonResult)controller.AddDate(passport);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePassport(It.IsAny<Passport>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion


        #region EditDateGET

        [Test]
        public void EditDateGet_CannotEdit_InvalidEmployeeIDEmptySearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.EditDate(15, "");
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(passport);

        }

        [Test]
        public void EditDateGet_CannotEdit_InvalidEmployeeIDValidSearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.EditDate(1500, "a");
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(passport);

        }

        [Test]
        public void EditDateGet_CannotEdit_InvalidEmployeeIDNullSearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.EditDate(1500, null);
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(passport);

        }

        [Test]
        public void EditDateGet_CanEdit_ValidEmployeeID()
        {
            // Arrange

            // Act - call the action method 
            var result = controller.EditDate(1) as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(1, passport.EmployeeID);
            Assert.AreEqual("Zarose Anastasia (andl) from SDDDA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void EditDateGet_CanEdit_ValidEmployeeIDAndEmptySearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = controller.EditDate(1, "") as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(1, passport.EmployeeID);
            Assert.AreEqual("Zarose Anastasia (andl) from SDDDA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void EditDateGet_CanEdit_ValidEmployeeIDAndSearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = controller.EditDate(1, "A") as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(1, passport.EmployeeID);
            Assert.AreEqual("Zarose Anastasia (andl) from SDDDA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void EditDateGet_CanEdit_ValidEmployeeIDNullSearchString()
        {
            // Arrange

            // Act - call the action method 
            var result = controller.EditDate(1, null) as ViewResult;
            PassportViewModel passport = (PassportViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(1, passport.EmployeeID);
            Assert.AreEqual("Zarose Anastasia (andl) from SDDDA", result.ViewBag.EmployeeInformation);
        }

        [Test]
        public void EditDateGet_CannotEdit_InvalidEmployeeID()
        {
            // Arrange - create the controller 

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.EditDate(1500);
            Passport passport = mock.Object.Passports.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(passport);
        }
        #endregion

        #region EditDatePost

        [Test]
        public void EditDatePost_CanEdit_ValidPassport()
        {
            // Arrange - create the controller 
            Passport passport = new Passport { EmployeeID = 5, EndDate = new DateTime(2013, 04, 01) };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.EditDate(passport);

            // Assert - check the result 
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SavePassport(passport), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }


        [Test]
        public void EditDatePost_CannotEdit_InvalidPassport()
        {
            // Arrange - create the controller 
            Passport passport = new Passport();

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.EditDate(passport);

            // Assert - check the result 
            mock.Verify(m => m.SavePassport(passport), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(PassportViewModel), ((ViewResult)result).Model);
        }

        [Test]
        public void EditDatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SavePassport(It.IsAny<Passport>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.EditDate(mock.Object.Passports.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SavePassport(It.IsAny<Passport>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

 
        #endregion


        #region GetEmployeeData
        [Test]
        public void GetEmployeeData_ListNotEmptySearchStringEmpty_AllEmployees()
        {
            //Arrange
            PassportController controller = new PassportController(mock.Object);
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
            PassportController controller = new PassportController(mock.Object);
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
            PassportController controller = new PassportController(mock.Object);
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
