using System;
using NUnit.Framework;
using Moq;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using AjourBT.Controllers;
using System.Web.Mvc;
using System.Data.SqlClient;
using AjourBT.Domain.Infrastructure;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class UnitControllerTest
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
        }


        [Test]
        [Category("View names")]
        public void IndexView_True()
        {
            // Arrange - create the controller
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            ViewResult result = target.Index();

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void Index_Default_AllUnits()
        {
            // Arrange - create the controller     
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            IEnumerable<Unit> result = (IEnumerable<Unit>)target.Index().Model;
            List<Unit> unitView = result.ToList<Unit>();

            // Assert - check the result 
            CollectionAssert.AreEqual(mock.Object.Units, unitView);
        }


        //[Test]
        //public void CountriesDropDownList_Default_ListOfAllCountries()
        //{
        //    //Arrange

        //    UnitController controller = new UnitController(mock.Object);
        //    //Act
        //    var result = controller.CountriesDropDownList();
        //    var expected = result.ToArray();



        //    //Asset
        //    Assert.IsInstanceOf(typeof(SelectList), result);
        //    Assert.AreEqual(4, expected.Length);
        //    Assert.AreEqual("Belarus", expected[0].Text);
        //    Assert.AreEqual("Sweden", expected[2].Text);
        //    Assert.AreEqual("Poland", expected[1].Text);
        //    Assert.AreEqual("Ukraine", expected[3].Text);
        //}

        [Test]
        [Category("View names")]
        public void CreateGetView_True()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            var result = target.Create() as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void CreatePost_CanCreate_ValidUnit()
        {
            // Arrange - create the controller                 
            UnitController target = new UnitController(mock.Object);
            Unit unit = new Unit { UnitID = 1, ShortTitle = "Umknown", Title = "Unknown" };

            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.Create(unit);

            // Assert - check the result 
            mock.Verify(m => m.SaveUnit(unit), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidUnit()
        {
            // Arrange - create the controller
            Unit unit = new Unit();
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.Create(unit) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveUnit(It.IsAny<Unit>()), Times.Never);
            Assert.IsInstanceOf(typeof(Unit), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        [Category("View names")]
        public void EditView_True()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void EditGet_CanEdit_ValidUnit()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void EditGet_CannotEdit_InvalidUnit()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Edit(15);
            Unit unit = mock.Object.Units.Where(m => m.UnitID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(unit);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void EditPost_CanEdit_ValidUnit()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);
            Unit unit = new Unit { UnitID = 1, ShortTitle = "Unknown", Title = "Unknown" };


            // Act - call the action method 
            var result = (RedirectToRouteResult)target.Edit(unit);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
            mock.Verify(m => m.SaveUnit(unit), Times.Once);
        }


        [Test]
        public void EditPost_CannotEdit_InvalidUnit()
        {
            // Arrange - create the controller 
            Unit unit = new Unit { };
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.Edit(unit);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SaveUnit(unit), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("", data);
            //Assert.IsInstanceOf(typeof(Unit), result.ViewData.Model);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            UnitController controller = new UnitController(mock.Object);
            mock.Setup(m => m.SaveUnit(It.IsAny<Unit>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.Edit(mock.Object.Units.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveUnit(It.IsAny<Unit>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void DeleteGet_ValidUnitWithAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(1) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("CannotDelete", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void DeleteGet_ValidUnitWithoutAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(3) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Unit), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
        }


        [Test]
        public void DeleteGet_InvalidUnit()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Delete(15);
            Unit unit = mock.Object.Units.Where(m => m.UnitID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(unit);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void DeletePost_CanDelete_ValidUnit()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);

            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeleteUnit(1), Times.Once);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
            Assert.AreEqual(4, result.RouteValues["tab"]);
            Assert.IsFalse(result.Permanent);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void DeletePost_CannotDelete_ValidUnit()
        {
            // Arrange - create the controller 
            UnitController target = new UnitController(mock.Object);
            mock.Setup(x => x.DeleteUnit(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteUnit(2), Times.Once);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);


        }

    }
}
