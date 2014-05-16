using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
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
        public class ErrorControllerTest
        {
            Mock<IRepository> mock;

            [SetUp]
            public void SetUp()
            {
                mock = Mock_Repository.CreateMock();
            }

            #region ShowErrorPage

            //[Test]
            //public void ShowErrorPage404_View()
            //{
            //    //Arrange
            //    ErrorController controller = new ErrorController();
            //    int statusCode = 404;
            //    Exception error = new Exception ();
            //    //Act
            //    var result = controller.ShowErrorPage(statusCode, error) as ViewResult;
            //    var resModel = result.Model as ErrorModel;

            //    //Assert
            //    Assert.AreEqual("", result.ViewName);
            //    Assert.AreEqual(404, resModel.statusCode);
            //    Assert.AreEqual("", resModel.RequestedURL);
            //}

            #endregion
        }
    }
