using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AjourBT.Controllers;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    class DepartmentControllerTest
    {
        Mock<IRepository> mock;
        string modelError =           "The record you attempted to edit "
                                      + "was modified by another user after you got the original value. The "
                                      + "edit operation was canceled.";

        [TestFixtureSetUp]
        public void Setup()
        {
            mock = Mock_Repository.CreateMock();           
        }

        [Test]
        public void CanReturnView()
        {
            //Arrange
            var controller = new DepartmentController(mock.Object);
            //Act 
            var result = controller.Index();
            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void ShowRightView()
        {
            //Arrange
            DepartmentController target = new DepartmentController(mock.Object);
            //Act
            ViewResult result = (ViewResult)target.Index();
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.ViewName, "");
        }

  
        [Test]
        public void ViewDepartmentList()
        {
            //Arrange
            DepartmentController target = new DepartmentController(mock.Object);
            //Act
            IEnumerable<Department> result = (IEnumerable<Department>)target.Index().Model;
            Department[] depArray = result.ToArray();
            //Assert
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Department));
            Assert.IsTrue(depArray.Length == 7);
            Assert.IsTrue(depArray[0].DepartmentName == "SDDDA");
            Assert.IsTrue(depArray[3].DepartmentName == "RAAA2");
            Assert.IsTrue(depArray[6].DepartmentName == "RAAA5");

        }

        [Test]
        public void CreateDepartment_ReturnView()
        {
            //Arrange
            DepartmentController target = new DepartmentController(mock.Object);
            //Act
            ViewResult result = (ViewResult)target.Create();
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void AddDepertment_ValidModel_Added()
        {
            //Arrange
            Mock<IRepository> mRepository = new Mock<IRepository>();
            mRepository.Setup(d => d.Departments).Returns(new Department[]{
            new Department{DepartmentID = 1, DepartmentName = "EPUA"}
            }.AsQueryable());
            DepartmentController target = new DepartmentController(mRepository.Object);
            Department department = new Department();
            //Act
            RedirectToRouteResult result = (RedirectToRouteResult)target.Create(department);
            //Assert
            mRepository.Verify(d => d.SaveDepartment(It.IsAny<Department>()), Times.Once());
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
        }

        [Test]
        public void AddDepartment_InvalidModel_NotAdded()
        {
            //Arrange
            Mock<IRepository> mRepository = new Mock<IRepository>();
            mRepository.Setup(d => d.Departments).Returns(new Department[]{
            new Department{DepartmentID = 1, DepartmentName = "EPUA"}
            }.AsQueryable());
            DepartmentController target = new DepartmentController(mRepository.Object);
            Department department = new Department();
            target.ModelState.AddModelError("error", "error");
            //Act
            ViewResult result = (ViewResult)target.Create(department);
            //Assert
            mRepository.Verify(d => d.SaveDepartment(It.IsAny<Department>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [Test]
        public void EditDepartmentGetMethodWithValidID()
        {
            //Arrange

            DepartmentController target = new DepartmentController(mock.Object);
            //   Act
            ViewResult result = (ViewResult)target.Edit(1);
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void EditDepartmentGetMethodWithInvalidID()
        {
            //Arrange

            DepartmentController target = new DepartmentController(mock.Object);
            //   Act
            var result = target.Edit(100);
            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
        }

        [Test]
        public void EditDepartment_ValidModel_Save()
        {
            //Arrange
            Mock<IRepository> mRepository = new Mock<IRepository>();
            mRepository.Setup(d => d.Departments).Returns(new Department[]{
            new Department{DepartmentID = 1, DepartmentName = "EPUA"}
            }.AsQueryable());
            DepartmentController target = new DepartmentController(mRepository.Object);
            Department department = new Department();
            //Act
            RedirectToRouteResult result = (RedirectToRouteResult)target.Edit(department);
            //Assert
            mRepository.Verify(d => d.SaveDepartment(It.IsAny<Department>()), Times.Once());
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
        }

        [Test]
        public void EditDepartment_InValidModel_NotSave()
        {
            //Arrange
            Mock<IRepository> mRepository = new Mock<IRepository>();
            mRepository.Setup(d => d.Departments).Returns(new Department[]{
            new Department{DepartmentID = 1, DepartmentName = "EPUA"}
            }.AsQueryable());

            DepartmentController target = new DepartmentController(mRepository.Object);
            Department department = new Department();
            target.ModelState.AddModelError("error", "error");
            //Act
            JsonResult result = (JsonResult)target.Edit(department);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mRepository.Verify(d => d.SaveDepartment(It.IsAny<Department>()), Times.Never());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("", data);
        }

        [Test]
        public void GetDelete_CorrectIDDepartmentWithoutEmployees_ConfirmDeleting()
        {
            //Arrange

            DepartmentController target = new DepartmentController(mock.Object);
            //   Act
            ViewResult result = (ViewResult)target.Delete(3);
            // Assert
            Assert.IsInstanceOf(typeof(Department), result.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void GetDelete_CorrectIDDepartmentWithEmployees_CannotDelete()
        {
            //Arrange

            DepartmentController target = new DepartmentController(mock.Object);
            //   Act
            ViewResult result = (ViewResult)target.Delete(1);
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("CannotDelete", result.ViewName);
        }



        [Test]
        public void GetDelete_IncorrectID_Error()
        {
            //Arrange

            DepartmentController target = new DepartmentController(mock.Object);
            //   Act
            HttpNotFoundResult result = (HttpNotFoundResult)target.Delete(100);
            // Assert
            Assert.IsTrue(result.StatusCode == 404);
        }
        [Test]
        public void PostDelete_CanDelete_Redirect_to_PUView()
        {
            //Arrange
            DepartmentController target = new DepartmentController(mock.Object);
            //Act
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(6);

            //Assert
            mock.Verify(m => m.DeleteDepartment(6), Times.Once);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);

        }

        [Test]
        public void PostDelete_CannotDelete()
        {
            // Arrange - create the controller
            DepartmentController target = new DepartmentController(mock.Object);
            mock.Setup(x => x.DeleteDepartment(It.IsAny<int>()))
              .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });

            // Act - call the action method
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(2);

            // Assert - check the result
            mock.Verify(m => m.DeleteDepartment(2), Times.Once);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);
        }

        [Test]
        public void EditDepartment_ValidModelComcurrency_JsonErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SaveDepartment(It.IsAny<Department>())).Throws(new DbUpdateConcurrencyException());
            DepartmentController target = new DepartmentController(mock.Object);

            //Act
            JsonResult result = (JsonResult)target.Edit(mock.Object.Departments.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveDepartment(It.IsAny<Department>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }
    }
}