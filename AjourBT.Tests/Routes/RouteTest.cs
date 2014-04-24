﻿using System;
using System.Web;
using System.Web.Routing;
using Moq;
using System.Reflection;
using NUnit.Framework;

namespace AjourBT.Tests.Routes
{
    [TestFixture]
    public class RouteTest
    {
        private HttpContextBase CreateHttpContext(string targetUrl = null, string httpMethod = "GET")
        {
            // create the mock request 
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(m => m.AppRelativeCurrentExecutionFilePath).Returns(targetUrl);
            mockRequest.Setup(m => m.HttpMethod).Returns(httpMethod);

            // create the mock response 
            Mock<HttpResponseBase> mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Setup(m => m.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            // create the mock context, using the request and response 
            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(m => m.Request).Returns(mockRequest.Object);
            mockContext.Setup(m => m.Response).Returns(mockResponse.Object);

            // return the mocked context 
            return mockContext.Object;
        }

        public void TestRouteMatch(string url, string controller, string action, object routeProperties = null, string httpMethod = "GET")
        {
            // Arrange 
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            // Act - process the route 
            RouteData result = routes.GetRouteData(CreateHttpContext(url, httpMethod));
            // Assert 
            Assert.IsNotNull(result);
            Assert.IsTrue(TestIncomingRouteResult(result, controller, action, routeProperties));
        }

        private bool TestIncomingRouteResult(RouteData routeResult, string controller, string action, object propertySet = null)
        {
            Func<object, object, bool> valCompare = (v1, v2) =>
            {
                return StringComparer.InvariantCultureIgnoreCase.Compare(v1, v2) == 0;
            };
            bool result = valCompare(routeResult.Values["controller"], controller)
            && valCompare(routeResult.Values["action"], action);
            if (propertySet != null)
            {
                PropertyInfo[] propInfo = propertySet.GetType().GetProperties();
                foreach (PropertyInfo pi in propInfo)
                {
                    if (!(routeResult.Values.ContainsKey(pi.Name) && valCompare(routeResult.Values[pi.Name], pi.GetValue(propertySet, null))))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        private void TestRouteFail(string url)
        {
            // Arrange 
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            // Act - process the route 
            RouteData result = routes.GetRouteData(CreateHttpContext(url));
            // Assert 
            Assert.IsTrue(result == null || result.Route == null);
        }

        [Test]
        public void TestIncomingRoutes()
        {
            TestRouteMatch("~/", "Account", "Login");
            TestRouteMatch("~/Account/Login", "Account", "Login");
            TestRouteMatch("~/Account", "Account", "Login");
            TestRouteMatch("~/Account/Login/UnknownSegment", "Account", "Login");
            TestRouteMatch("~/Account/Login/UnknownSegment", "Account", "Login", new { id = "UnknownSegment" });
            TestRouteFail("~/Account/Login/UnknownSegment/UnknownSegment");
            TestRouteMatch("~/Home/PUView", "Home", "PUView");
        }
    }
}
