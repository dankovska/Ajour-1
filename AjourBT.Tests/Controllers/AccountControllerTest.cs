﻿﻿using System;
using NUnit.Framework;
using Moq;
using AjourBT.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using AjourBT.Domain.Entities;
using AjourBT.Controllers;
using AjourBT.Infrastructure;
using System.Threading.Tasks;
using System.Threading;
using AjourBT.Domain.Abstract;
using System.Web.Security;
using System.Data.SqlClient;
using AjourBT.Tests.MockRepository;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class AccountControllerTest
    {
        Mock<ControllerContext> mock;
        AccountController controller;
        ViewContext viewContext;

        [SetUp]
        public void SetUp()
        {
            var mock1 = Mock_Repository.CreateMock();
            mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("briv");
            mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            controller = new AccountController(mock1.Object);
            controller.ControllerContext = mock.Object;
            viewContext = new ViewContext();
            viewContext.HttpContext = new FakeHttpContext();
        }
        private class FakeHttpContext : HttpContextBase
        {
            private Dictionary<object, object> _items = new Dictionary<object, object>();
            public override IDictionary Items { get { return _items; } }
        }

        private class FakeViewDataContainer : IViewDataContainer
        {
            private ViewDataDictionary _viewData = new ViewDataDictionary();
            public ViewDataDictionary ViewData { get { return _viewData; } set { _viewData = value; } }
        }

        [Test]
        public void GetLoggedInUsers_LoggedInUsersNull_UserName()
        {
            //Arrange
            HttpRuntime.Cache.Remove("LoggedInUsers");

            //Act
            PartialViewResult result = controller.GetLoginUser();

            //Assert        
            Assert.AreEqual("_NoOnlineUsers", result.ViewName);
           
        }

        [Test]
        public void GetLoggedInUsers_LoggedInUsersNotContainCurrentUser_UserName()
        {
            //Arrange
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();

            //Act
            var result = controller.GetLoginUser();


            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsFalse((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.IsNull(result.ViewBag.FullListOfOnlineUsers);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_NoOnlineUsers");
        }

        [Test]
        public void GetLoggedInUsers_LoggedInNewUsersContainNewUser_UserName()
        {
            //Arrange
            mock.Setup(p => p.HttpContext.User.IsInRole("BTM")).Returns(true);
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();

            //Act
            var result = controller.GetLoginUser();


            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.IsNull(result.ViewBag.FullListOfOnlineUsers);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
        }

        [Test]
        public void GetLoggedInUsers_ADMLoggedInUsersContainsCurrentUser_UserName()
        {
            //Arrange
            mock.Setup(p => p.HttpContext.User.IsInRole("ADM")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            Thread.Sleep(2);

            //Act
            var result = controller.GetLoginUser();

            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.Greater((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>)["briv"], timeBefore);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
        }

        [Test]
        public void GetLoggedInUsers_BTMLoggedInUsersContainsCurrentUser_UserName()
        {
            //Arrange
            mock.Setup(p => p.HttpContext.User.IsInRole("BTM")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            Thread.Sleep(2);

            //Act
            var result = controller.GetLoginUser();

            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.Greater((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>)["briv"], timeBefore);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
        }
       
        [Test]
        public void GetLoggedInUsers_DIRLoggedInUsersContainsCurrentUser_UserName()
        {
            //Arrange
            mock.Setup(p => p.HttpContext.User.IsInRole("DIR")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            Thread.Sleep(2);

            //Act
            var result = controller.GetLoginUser();

            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.Greater((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>)["briv"], timeBefore);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
        }

        [Test]
        public void GetLoggedInUsers_PULoggedInUsersContainsCurrentUser_UserName()
        {
            //Arrange
        
            mock.Setup(p => p.HttpContext.User.IsInRole("PU")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            Thread.Sleep(2);

            //Act
            var result = controller.GetLoginUser();

            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.Greater((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>)["briv"], timeBefore);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
        }

        [Test]
        public void GetLoggedInUsers_ACCLoggedInUsersContainsCurrentUser_UserName()
        {
            //Arrange
            mock.Setup(p => p.HttpContext.User.IsInRole("ACC")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            Thread.Sleep(2);

            //Act
            var result = controller.GetLoginUser();


            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.Greater((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>)["briv"], timeBefore);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
        }

        [Test]
        public void GetLoggedInUsers_EMPLoggedInUsersContainsCurrentUser_View()
        {
            //Arrange
            mock.Setup(p => p.HttpContext.User.IsInRole("EMP")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            Thread.Sleep(2);

            //Act
            var result = controller.GetLoginUser();

            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.IsNull(result.ViewBag.FullListOfOnlineUsers);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
        }

        [Test]
        public void GetLoggedInUsers_LoggedInUsersContainsOldDateTimeForUsers_UserName()
        {
            //Arrange
                    
            mock.Setup(p => p.HttpContext.User.IsInRole("ADM")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["rkni"] = DateTime.Now.ToLocalTimeAzure().AddMinutes(-10);
            Thread.Sleep(1);

            //Act
            var result = controller.GetLoginUser();


            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.IsFalse((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("rkni"));
            Assert.Greater((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>)["briv"], timeBefore);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
            Assert.AreEqual(2, result.ViewEngineCollection.Count());
        }

        [Test]
        public void GetLoggedInUsers_LoggedInUsersContainsOldDateTimeForUsersMinus1Second_UserName()
        {
            //Arrange
            mock.Setup(p => p.HttpContext.User.IsInRole("ADM")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["rkni"] = DateTime.Now.ToLocalTimeAzure().AddMinutes(-5).AddSeconds(-1);
            Thread.Sleep(1);

            //Act
            PartialViewResult result = controller.GetLoginUser();


            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.IsFalse((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("rkni"));
            Assert.Greater((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>)["briv"], timeBefore);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
            Assert.AreEqual(2, result.ViewEngineCollection.Count());
        }

        [Test]
        public void GetLoggedInUsers_LoggedInUsersContainsOldDateTimeForUsersPlus1Second_UserName()
        {
            //Arrange
            mock.Setup(p => p.HttpContext.User.IsInRole("ADM")).Returns(true);
            DateTime timeBefore = DateTime.Now.ToLocalTimeAzure();
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["briv"] = timeBefore;
            ((HttpRuntime.Cache["LoggedInUsers"]) as Dictionary<string, DateTime>)["rkni"] = DateTime.Now.ToLocalTimeAzure().AddMinutes(-5).AddSeconds(1);
            Thread.Sleep(1);

            //Act
            var result = controller.GetLoginUser();

            //Assert        
            Assert.IsTrue(HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime> != null);
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("briv"));
            Assert.IsTrue((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>).ContainsKey("rkni"));
            Assert.Greater((HttpRuntime.Cache["LoggedInUsers"] as Dictionary<string, DateTime>)["briv"], timeBefore);
            Assert.AreEqual(typeof(PartialViewResult), result.GetType());
            Assert.AreEqual(((PartialViewResult)result).ViewName, "_GetOnlineUsersShort");
            Assert.AreEqual(2, result.ViewEngineCollection.Count());
           }

        [Test]
        public void FullOnlineUsersList_StringEmpty_HttpNotFound()
        {
            //Arrange
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();
    
            //Act
            var result = controller.FullOnlineUsersList("");


            //Assert        
            Assert.AreEqual(typeof(HttpNotFoundResult), result.GetType());
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }
      
        [Test]
        public void FullOnlineUsersList_Null_HttpNotFound()
        {
            //Arrange
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();

            //Act
            var result = controller.FullOnlineUsersList(null);

            //Assert        
            Assert.AreEqual(typeof(HttpNotFoundResult), result.GetType());
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void FullOnlineUsersList_ValidString_View()
        {
            //Arrange
            string loggedInUsersIDs = "briv";
            HttpRuntime.Cache["LoggedInUsers"] = new Dictionary<string, DateTime>();

            //Act
            var result = controller.FullOnlineUsersList(loggedInUsersIDs);

            //Assert        
            Assert.AreEqual(typeof(ViewResult), result.GetType());
            Assert.AreEqual(((ViewResult)result).ViewName, "");
            Assert.AreEqual("briv", ((ViewResult)result).ViewBag.FullListOfOnlineUsers);
        }


        [Test]
        public void ConvertEIDToName_ProperEID_ProperName()
        {
            //Arrange

            //Act
            string result = controller.ConvertEIDToName("andl");

            //Assert   
            Assert.AreEqual("Anastasia", result);                       
        }

        [Test]
        public void ConvertEIDToName_NotValidEID_EmptyString()
        {
            //Arrange

            //Act
            string result = controller.ConvertEIDToName("andz1");

            //Assert   
            Assert.AreEqual("", result);
        }


     
    }
}
