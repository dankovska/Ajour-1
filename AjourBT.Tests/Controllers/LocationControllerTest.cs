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
    public class LocationControllerTest
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
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            ViewResult result = target.Index();

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void Index_Default_AllLocations()
        {
            // Arrange - create the controller     
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            IEnumerable<Location> result = (IEnumerable<Location>)target.Index().Model;
            List<Location> locationView = result.ToList<Location>();

            // Assert - check the result 
            CollectionAssert.AreEqual(mock.Object.Locations, locationView);
        }


        [Test]
        public void CountriesDropDownList_Default_ListOfAllCountries()
        {
            //Arrange
            LocationController controller = new LocationController(mock.Object);

            //Act
            var result = controller.CountriesDropDownList();
            var expected = result.ToArray();

            //Asset
            Assert.IsInstanceOf(typeof(SelectList), result);
            Assert.AreEqual(5, expected.Length);
            Assert.AreEqual("Belarus", expected[0].Text);       
            Assert.AreEqual("Poland", expected[1].Text);
            Assert.AreEqual("Sweden", expected[2].Text);
            Assert.AreEqual("Ukraine", expected[3].Text);
            Assert.AreEqual("Zimbabve", expected[4].Text);
        }

        #region Create

        [Test]
        [Category("View names")]
        public void CreateGet_View_True()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);
            IEnumerable<Country> countriesList = from c in mock.Object.Countries
                                                      orderby c.CountryName
                                                      select c;

            // Act - call the action method 
            var result = target.Create() as ViewResult;

            // Assert - check the result 
            Assert.AreEqual(countriesList.ToList(), ((ViewResult)result).ViewBag.CountryList.Items);
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void CreatePost_CanCreate_ValidLocationNullResponsibleID()
        {
            // Arrange - create the controller                 
            LocationController target = new LocationController(mock.Object);
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St." };

            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.Create(location);

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(location), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidLocation()
        {
            // Arrange - create the controller
            Location location = new Location();
            LocationController target = new LocationController(mock.Object);
            IEnumerable<Country> countriesList = from c in mock.Object.Countries
                                                 orderby c.CountryName
                                                 select c;

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            ViewResult result = target.Create(location) as ViewResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(It.IsAny<Location>()), Times.Never);
            Assert.AreEqual(countriesList.ToList(), ((ViewResult)result).ViewBag.CountryList.Items);
            Assert.IsInstanceOf(typeof(Location), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

       
        [Test]
        public void CreatePost_CannotCreate_InvalidResponsibleID()
        {
            // Arrange - create the controller
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc="dan" };
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            var result = target.Create(location);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(It.IsAny<Location>()), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("Not existing Employee's EID", data);
        }


        [Test]
        public void CreatePost_CanCreate_ValidLocationWithResponsibleID()
        {
            // Arrange - create the controller
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc = "daol" };
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            RedirectToRouteResult result = target.Create(location) as RedirectToRouteResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(location), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
        }

        [Test]
        public void CreatePost_CanCreate_ValidLocationWithEmptyResponsibleID()
        {
            // Arrange - create the controller
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc = "" };
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            RedirectToRouteResult result = target.Create(location) as RedirectToRouteResult;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(location), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
        }

        #endregion

        //#region AddResponsibleForLoc


        //[Test]
        //public void AddResponsibleForLocGet_0_HttpNotFound()
        //{
        //    // Arrange - create the controller
        //    LocationController target = new LocationController(mock.Object);

        //    // Act - call the action method 
        //    var result = target.AddResponsibleForLoc(0);

        //    // Assert - check the result 
        //    Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        //    Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        //}

        //[Test]
        //public void AddResponsibleForLocGet_DefaultNull_HttpNotFound()
        //{
        //    // Arrange - create the controller
        //    LocationController target = new LocationController(mock.Object);

        //    // Act - call the action method 
        //    var result = target.AddResponsibleForLoc();

        //    // Assert - check the result 
        //    Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        //    Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        //}

        //[Test]
        //public void AddResponsibleForLocGet_CanAdd_View()
        //{
        //    // Arrange - create the controller
        //    Location location = mock.Object.Locations.Where(l => l.LocationID == 1).FirstOrDefault();
        //    LocationController target = new LocationController(mock.Object);

        //    // Act - call the action method 
        //    var result = target.AddResponsibleForLoc(1);

        //    // Assert - check the result 
        //    Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        //    Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        //}
 
        //#endregion


        #region IsExistingID
        [Test]
        public void IsExistingID_true_ExistingIds()
        {
            // Arrange - create the controller
            LocationController target = new LocationController(mock.Object);
            string ids = "tedk,daol";

            // Act - call the action method          
            bool result = target.IsExistingID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingID_ExistingIdsWithEmptySpacesInTheEndAndBegin_True()
        {
            // Arrange - create the controller
            LocationController target = new LocationController(mock.Object);
            string ids = "    tedk  ,     daol    ";

            // Act - call the action method          
            bool result = target.IsExistingID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingID_true_ExistingIds2()
        {
            // Arrange - create the controller
            LocationController target = new LocationController(mock.Object);
            string ids = "tedk, daol";

            // Act - call the action method          
            bool result = target.IsExistingID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingID_true_ExistingIds3()
        {
            // Arrange - create the controller
            LocationController target = new LocationController(mock.Object);
            string ids = "tedk;   daol";

            // Act - call the action method          
            bool result = target.IsExistingID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingID_false_NotExistingIds()
        {
            // Arrange - create the controller
            LocationController target = new LocationController(mock.Object);
            string ids = "tedk;;*&?$#!/-+=[]{}:''<>.,   dni";

            // Act - call the action method          
            bool result = target.IsExistingID(ids);

            // Assert - check the result            
            Assert.IsFalse(result);
        }

        [Test]
        public void IsExistingID_false_NotExistingIds2()
        {
            // Arrange - create the controller
            LocationController target = new LocationController(mock.Object);
            string ids = "tepydani";

            // Act - call the action method          
            bool result = target.IsExistingID(ids);

            // Assert - check the result            
            Assert.IsFalse(result);
        }

        [Test]
        public void IsExistingID_EmptyString_True()
        {
            // Arrange - create the controller
            LocationController target = new LocationController(mock.Object);
            string ids = "";

            // Act - call the action method          
            bool result = target.IsExistingID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        [Test]
        public void IsExistingID_Null_True()
        {
            // Arrange - create the controller
            LocationController target = new LocationController(mock.Object);
            string ids = null;

            // Act - call the action method          
            bool result = target.IsExistingID(ids);

            // Assert - check the result            
            Assert.IsTrue(result);
        }

        #endregion

        #region EditGet
        [Test]
        [Category("View names")]
        public void EditView_True()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void EditGet_CanEdit_ValidLocation()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(2) as ViewResult;

            // Assert - check the result
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", result.ViewName);
            Assert.IsNotNull(result.ViewData.Model);
        }

        [Test]
        public void EditGet_CannotEdit_InvalidLocation()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Edit(15);
            Location location = mock.Object.Locations.Where(m => m.LocationID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(location);
            Assert.AreEqual(404, result.StatusCode);
        }

        #endregion

        #region EditPost

        [Test]
        public void EditPost_CanEdit_ValidLocationNullResponsibleID()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St." };


            // Act - call the action method 
            var result = (RedirectToRouteResult)target.Edit(location);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
            mock.Verify(m => m.SaveLocation(location), Times.Once);
        }


        [Test]
        public void EditPost_CanEdit_ValidLocationWithResponsibleID()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc= "daol" };


            // Act - call the action method 
            var result = (RedirectToRouteResult)target.Edit(location);

            // Assert - check the result
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
            mock.Verify(m => m.SaveLocation(location), Times.Once);
        }



        [Test]
        public void EditPost_CannotEdit_InvalidLocation()
        {
            // Arrange - create the controller 
            Location location = new Location { };
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            target.ModelState.AddModelError("error", "error");
            var result = target.Edit(location);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(location), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("", data);
            //Assert.IsInstanceOf(typeof(Location), result.ViewData.Model);
        }

        [Test]
        public void EditPost_CannotCreate_InvalidResponsibleID()
        {
            // Arrange - create the controller
            Location location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", ResponsibleForLoc = "dan" };
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            var result = target.Edit(location);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert - check the result 
            mock.Verify(m => m.SaveLocation(It.IsAny<Location>()), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("Not existing Employee's EID", data);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            LocationController controller = new LocationController(mock.Object);
            mock.Setup(m => m.SaveLocation(It.IsAny<Location>())).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.Edit(mock.Object.Locations.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveLocation(It.IsAny<Location>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region DeleteGet
        [Test]
        public void DeleteGet_ValidLocationWithAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(2) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("CannotDelete", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void DeleteGet_ValidLocationWithoutAssociatedDate_CannotDelete()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            var result = target.Delete(3) as ViewResult;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Location), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
        }


        [Test]
        public void DeleteGet_InvalidLocation()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)target.Delete(15);
            Location location = mock.Object.Locations.Where(m => m.LocationID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(location);
            Assert.AreEqual(404, result.StatusCode);
        }

        #endregion

        #region DeletePost
        [Test]
        public void DeletePost_CanDelete_ValidLocation()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);

            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(1);

            // Assert - check the result 
            mock.Verify(m => m.DeleteLocation(1), Times.Once);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("PUView", result.RouteValues["action"]);
            Assert.AreEqual(2, result.RouteValues["tab"]);
            Assert.IsFalse(result.Permanent);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void DeletePost_CannotDelete_ValidLocation()
        {
            // Arrange - create the controller 
            LocationController target = new LocationController(mock.Object);
            mock.Setup(x => x.DeleteLocation(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method 
            RedirectToRouteResult result = (RedirectToRouteResult)target.DeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteLocation(2), Times.Once);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);


        }

        #endregion
    }
}
