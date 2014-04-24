using AjourBT.Controllers;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Concrete;
using AjourBT.Domain.Entities;
using AjourBT.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AjourBT.Infrastructure;
using System.Web.Configuration;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;
using Newtonsoft.Json;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class BusinessTripControllerTest
    {
        Mock<IRepository> mock;
        Mock<IMessenger> messengerMock;
        Mock<ControllerContext> controllerContext;
        BusinessTripController controller;
        
        string modelError = "The record you attempted to edit "
                              + "was modified by another user after you got the original value. The "
                              + "edit operation was canceled.";

        StringBuilder comment = new StringBuilder();
        string defaultAccComment;   

        byte[] rowVersion = { 0, 0, 0, 0, 0, 0, 8, 40 };

        public BusinessTripControllerTest()
        {
            comment.Append("ВКО №   від   , cума:   UAH.");
            comment.AppendLine();
            comment.Append("ВКО №   від   , cума:   USD.");
            defaultAccComment = comment.ToString();
        }

        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();
            messengerMock = new Mock<IMessenger>();

            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.UnknownType))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToADM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToADM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToDIR))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ACCModifiesConfirmedReportedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToDIR))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMConfirmsPlannedOrRegisteredToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsConfirmedOrConfirmedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsRegisteredOrRegisteredModifiedToADM))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.DIRRejectsConfirmedToADM))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.DIRRejectsConfirmedToEMP))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.DIRRejectsConfirmedToACC))).Verifiable();
            messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsPlannedModifiedToBTM))).Verifiable();
            //messengerMock.Setup(m => m.Notify(It.Is<IMessage>(msg => msg.messageType == MessageType.ADMCancelsPlannedModifiedToACC))).Verifiable();

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
            controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(t => t.HttpContext).Returns(fakeHttpContext.Object);

            controller = new BusinessTripController(mock.Object, messengerMock.Object);
            controller.ControllerContext = controllerContext.Object;

        }
       
        #region DropDownList
        


        [Test]
        public void LocationsDropDownList_Default_AllLocations()
        {
            // Arrange

            // Act          
            var result = controller.Plan(1);

            IEnumerable<Location> departmentsList = from l in mock.Object.Locations
                                                    orderby l.Title
                                                    select l;

            // Assert
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);
        }


        [Test]
        public void UnitsDropDownList_Default_AllUnits()
        {
            // Arrange

            // Act          
            var result = controller.Plan(1);

            IEnumerable<Unit> unitsList = from l in mock.Object.Units
                                                    orderby l.ShortTitle
                                                    select l;

            // Assert
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.UnitsList);
        }

        #endregion

        #region DIR

        #region GetBusinessTripDIR
        [Test]
        public void GetBusinessTripDIR_Null_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = null;

            // Act
            var view = controller.GetBusinessTripDIR(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTripDIR_EmptyString_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = "";

            // Act
            var view = controller.GetBusinessTripDIR(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTripDIR_SDDDA_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = "SDDDA";

            // Act
            var view = controller.GetBusinessTripDIR(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        #endregion

        #region GetBusinessTripDataDIR
        [Test]
        public void GetBusinessTripDataDIR_EmptyString_AllBusinessTrips()
        {
            //Arrange
            string selectedDepartment = "";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataDIR(selectedDepartment).Model;
            var view = controller.GetBusinessTripDataDIR(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(result.ToArray().Length, 4);
            Assert.AreEqual(result.ToArray()[0].BTof.LastName, "Kowood");
            Assert.AreEqual(result.ToArray()[1].BTof.LastName, "Manowens");
            Assert.AreEqual(result.ToArray()[2].BTof.LastName, "Pyorge");
            Assert.AreEqual(result.ToArray()[2].BusinessTripID, 4);
            Assert.AreEqual(result.ToArray()[3].BTof.LastName, "Pyorge");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTripDataDIR_Null_AllBusinessTrips()
        {
            //Arrange
            string selectedDepartment = null;

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataDIR(selectedDepartment).Model;
            var view = controller.GetBusinessTripDataDIR(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(result.ToArray().Length, 4);
            Assert.AreEqual(result.ToArray()[0].BTof.LastName, "Kowood");
            Assert.AreEqual(result.ToArray()[1].BTof.LastName, "Manowens");
            Assert.AreEqual(result.ToArray()[2].BTof.LastName, "Pyorge");
            Assert.AreEqual(result.ToArray()[2].BusinessTripID, 4);
            Assert.AreEqual(result.ToArray()[3].BTof.LastName, "Pyorge");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTripDataDIR_SDDDA_BusinessTripsOfSDDDA()
        {
            //Arrange
            string selectedDepartment = "SDDDA";

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataDIR(selectedDepartment).Model;
            var view = controller.GetBusinessTripDataDIR(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(result.ToArray().Length, 2);
            Assert.AreEqual(result.ToArray()[0].BTof.LastName, "Pyorge");
            Assert.AreEqual(result.ToArray()[1].BTof.LastName, "Pyorge");
            Assert.AreEqual(result.ToArray()[0].BusinessTripID, 4);
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
        }
        #endregion

        #region Reject_BT_DIRGet

        [Test]
        public void Reject_BT_DIRGet_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.Reject_BT_DIR();
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void Reject_BT_DIRGet_ExistingBT_CorrectViewLoads()
        {
            //Arrange

            // Act
            var view = controller.Reject_BT_DIR(3);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
        }



        [Test]
        public void Reject_BT_DIRGet_ExistingBTIdAndDefaultJsondataAndEmptyDepartment_CorrectViewLoads()
        {
            //Arrange
            BusinessTrip businessTripBeforeCall = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion };

            // Act
            var view = controller.Reject_BT_DIR(3, selectedDepartment: "");
            BusinessTrip businessTripAfterCall = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTripAfterCall.BusinessTripID);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(businessTripBeforeCall.RowVersion, businessTripAfterCall.RowVersion);

        }

        [Test]
        public void Reject_BT_DIRGet_ExistingBTAndDefaultJsondataAndSDDDA_CorrectViewLoads()
        {
            //Arrange
            BusinessTrip businessTripBeforeCall = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 100, 40 } };

            // Act
            var view = controller.Reject_BT_DIR(3, selectedDepartment: "SDDDA");
            BusinessTrip businessTripAfterCall = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTripAfterCall.BusinessTripID);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreNotEqual(businessTripBeforeCall.RowVersion, businessTripAfterCall.RowVersion);
            Assert.AreEqual(rowVersion, businessTripAfterCall.RowVersion);
        }



        [Test]
        public void Reject_BT_DIR_JsonDataNotEmpty_ExistingBT()
        {
            //Arrange
            string department = "AA";
            string jsonData = JsonConvert.SerializeObject(rowVersion);

            // Act
            var view = controller.Reject_BT_DIR(3, jsonData, department);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(department, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), (view as ViewResult).Model);
            Assert.AreEqual(rowVersion, ((view as ViewResult).Model as BusinessTripViewModel).RowVersion);
        }

        #endregion

        #region Reject_BT_DIR_Confirm

        [Test]
        public void Reject_BT_DIR_Confirm_NotExistingBT_Error404()
        {
            //Arrange
            BusinessTrip businessTrip = null;


            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip, "");

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void Reject_BT_DIR_Confirm_EmptyComment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            // Act

            var view = controller.Reject_BT_DIR_Confirm(businessTrip);
            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            selectedBusinessTripsList.Add(businessTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Never);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void Reject_BT_DIR_Confirm_EmptyCommentSelectedDepartment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion, RejectComment = "", UnitID = 1, Unit = new Unit() };

            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            selectedBusinessTripsList.Add(businessTrip);

            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip, "SDDDA");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Never);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void Reject_BT_DIR_Confirm_ExistingBTAndValidComment_CanReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 9).FirstOrDefault();
            businessTrip.RejectComment = "BT is too expensive";
            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            selectedBusinessTripsList.Add(businessTrip);
            businessTrip.DaysInBtForOrder = 9;

            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip);

            //Assert 
            Assert.IsFalse(((RedirectToRouteResult)view).Permanent);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), view);
            Assert.AreEqual("DIRView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual(0, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);
            Assert.AreEqual("BT is too expensive", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
            Assert.AreEqual(null, businessTrip.OrderStartDate);
            Assert.AreEqual(null, businessTrip.OrderEndDate);
            Assert.AreEqual(null, businessTrip.DaysInBtForOrder);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Once);

        }

        [Test]
        public void Reject_BT_DIR_Confirm_ExistingBTAndValidCommentSelectedDepartment_CanReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 9).FirstOrDefault();
            businessTrip.RejectComment = "BT is too expensive";

            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip, "SDDDA");

            //Assert 
            Assert.IsFalse(((RedirectToRouteResult)view).Permanent);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), view);
            Assert.AreEqual("DIRView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual(0, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual("SDDDA", ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);

        
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Once);

            Assert.AreEqual("BT is too expensive", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        [Test]
        public void Reject_BT_DIR_Confirm_CommentNullSelectedDepartment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 31).FirstOrDefault();
            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            selectedBusinessTripsList.Add(businessTrip);

            // Act
            var view = controller.Reject_BT_DIR_Confirm(businessTrip, "SDDDA");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(31, businessTrip.BusinessTripID);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Never);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void Reject_BT_DIR_Confirm_ExistingBTAndValidCommentConcurency_CanReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 9).FirstOrDefault();
            businessTrip.RejectComment = "BT is too expensive";
            Employee author = mock.Object.Employees.Where(e => e.EID == controller.ControllerContext.HttpContext.User.Identity.Name).FirstOrDefault();
            List<BusinessTrip> selectedBusinessTripsList = new List<BusinessTrip>();
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            selectedBusinessTripsList.Add(businessTrip);

            // Act
            JsonResult result = (JsonResult)controller.Reject_BT_DIR_Confirm(businessTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);


            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.DIRRejectsConfirmedToACC))), Times.Never);
            Assert.AreEqual("BT is too expensive", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        #endregion

        #endregion

        #region ACC

        #region GetBusinessTripACC

        [Test]
        public void GetBusinessTripACC_EmptyString_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = "";

            // Act

            var view = controller.GetBusinessTripACC(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTripACC_NullString_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = null;

            // Act

            var view = controller.GetBusinessTripACC(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTripACC_SDDDA_SelectDepartment()
        {
            // Arrange
            string selectedDepartment = "SDDDA";

            // Act

            var view = controller.GetBusinessTripACC(selectedDepartment);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        #endregion

        #region GetBusinessTripDataACC
        [Test]
        public void GetBusinessTripDataACC_EmptyString_AllBusinessTrips()
        {
            //Arrange
            string selectedDepartment = "";
            
            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataACC(selectedDepartment).Model;
            var view = controller.GetBusinessTripDataACC(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(14, result.ToArray().Length);

            Assert.AreEqual(31, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(34, result.ToArray()[1].BusinessTripID);
            Assert.AreEqual(35, result.ToArray()[2].BusinessTripID);
            Assert.AreEqual(25, result.ToArray()[3].BusinessTripID);
            Assert.AreEqual(37, result.ToArray()[11].BusinessTripID);
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetBusinessTripDataACC_Null_AllBusinessTrips()
        {
            //Arrange
            string selectedDepartment = null;

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataACC(selectedDepartment).Model;
            var view = controller.GetBusinessTripDataACC(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(result.ToList(), ((PartialViewResult)view).Model);
            Assert.AreEqual(14, result.ToArray().Length);

            Assert.AreEqual(31, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(34, result.ToArray()[1].BusinessTripID);
            Assert.AreEqual(35, result.ToArray()[2].BusinessTripID);
            Assert.AreEqual(25, result.ToArray()[3].BusinessTripID);
            Assert.AreEqual(37, result.ToArray()[11].BusinessTripID);

            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void GetBusinessTripDataACC_RAAA3_BusinessTripsOfRAAA3()
        {
            //Arrange
            string selectedDepartment = "RAAA3";


            BusinessTrip btrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 32).FirstOrDefault();
            btrip.AccComment = defaultAccComment;

            // Act
            IEnumerable<BusinessTrip> result = (IEnumerable<BusinessTrip>)controller.GetBusinessTripDataACC(selectedDepartment).Model;
            var view = controller.GetBusinessTripDataACC(selectedDepartment);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual(((PartialViewResult)view).Model, result.ToList());
            Assert.AreEqual(6, result.ToArray().Length);
            Assert.AreEqual("xtwe", result.ToArray()[0].BTof.EID);
            Assert.AreEqual("iwoo", result.ToArray()[1].BTof.EID);
            Assert.AreEqual("iwpe", result.ToArray()[2].BTof.EID);
            Assert.AreEqual("xtwe", result.ToArray()[3].BTof.EID);
            Assert.AreEqual("iwoo", result.ToArray()[4].BTof.EID);

            Assert.AreEqual(32, result.ToArray()[0].BusinessTripID);
            Assert.AreEqual(22, result.ToArray()[1].BusinessTripID);
            Assert.AreEqual(39, result.ToArray()[2].BusinessTripID);
            Assert.AreEqual(21, result.ToArray()[3].BusinessTripID);
            Assert.AreEqual(16, result.ToArray()[4].BusinessTripID);

            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
        }

        #endregion

        #region SearchSearchBusinessTripData
        
        [Test]
        public void SearchSearchBusinessTripDataACC_SelectedDepartmentNullSearchStringEmpty_AllBts()
    	{
        //Arrange
        string selectedDepartment = "";
        string searchString = "";

        BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
        bTrip.AccComment = defaultAccComment;
        bTrip.EndDate = new DateTime(2014, 12, 12);

        //Act
        List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);
            
        //Assert
        Assert.AreEqual(14, data.Count);
        Assert.AreEqual("Pyorge", data.First().BTof.LastName);
        Assert.AreEqual(new DateTime(2012,04,22),data.First().StartDate);
        Assert.AreEqual("Manowens", data.Last().BTof.LastName);
        Assert.AreEqual(new DateTime(2014, 12, 15), data.Last().StartDate);		
	    }
       
        [Test]
        public void SearchSearchBusinessTripDataACC_SelectedDepartmentStringEmptySearchStringMan_()
        {
            //Arrange
            string selectedDepartment = "";
            string searchString = "Man";
            
            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);
            
            //Assert
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual("Manowens", data.First().BTof.LastName);
            Assert.AreEqual(new DateTime(2013, 12, 25), data.First().StartDate);
            Assert.AreEqual("Manowens", data.Last().BTof.LastName);
            Assert.AreEqual(new DateTime(2014, 12, 15), data.Last().StartDate);
        }

        [Test]
        public void SearchSearchBusinessTripDataACC_SelectedDepartmentNotExistsSearchStringAn_()
        {
            //Arrange
            string selectedDepartment = "AAAA";
            string searchString = "An";
            
            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);
            
            //Assert
            Assert.AreEqual(0, data.Count);
        }

        [Test]
        public void SearchSearchBusinessTripDataACC_SelectedDepartmentRAAA3SearchA_()
        {
            //Arrange
            string selectedDepartment = "RAAA3";
            string searchString = "A";
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 32).FirstOrDefault();
            bTrip.AccComment = defaultAccComment;
            
            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);

            
            //Assert
            Assert.AreEqual(4, data.Count);
            Assert.AreEqual("Manowens", data.First().BTof.LastName);
            Assert.AreEqual(32, data.ToArray()[0].BusinessTripID);
            Assert.AreEqual(39, data.ToArray()[1].BusinessTripID);
            Assert.AreEqual(21, data.ToArray()[2].BusinessTripID);

            Assert.AreEqual(new DateTime(2013, 09, 01), data.First().StartDate);
        }

        [Test]
        public void SearchSearchBusinessTripDataACC_DefaultAccComment_()
        {
            //Arrange
            string selectedDepartment = "RAAA3";
            string searchString = "A";
            BusinessTrip btFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 32).FirstOrDefault();
            btFromRepository.AccComment = defaultAccComment;

            //Act
            List<BusinessTrip> data = controller.SearchBusinessTripDataACC(mock.Object.BusinessTrips.ToList(), selectedDepartment, searchString);

            //Assert
            Assert.AreEqual(4, data.Count);
            Assert.AreEqual(32, data.ToArray()[0].BusinessTripID);
            Assert.AreEqual(39, data.ToArray()[1].BusinessTripID);
            Assert.AreEqual(21, data.ToArray()[2].BusinessTripID);
            Assert.AreEqual(defaultAccComment, btFromRepository.AccComment);

            Assert.AreEqual(new DateTime(2013, 09, 01), data.First().StartDate);
        }

        #endregion


        #region GetEditReportedBT
        [Test]
        public void GetEditReportedBT_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.EditReportedBT(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void GetEditReportedBT_ValidBT_NotValidStatus()
        {
            //Arrange

            // Act
            var view = controller.EditReportedBT(14);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 14).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.AreEqual(14, businessTrip.BusinessTripID);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void GetEditReportedBT_ValidBT_StartDateMoreThenDateNow_ValidStatus()
        {
            //Arrange

            // Act
            var viewResult = controller.EditReportedBT(16) as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();
            BusinessTripViewModel businessTripModel = (BusinessTripViewModel)viewResult.ViewData.Model;
            //Assert 
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.AreEqual(16, businessTrip.BusinessTripID);
            Assert.AreEqual((BTStatus.Confirmed | BTStatus.Reported), businessTrip.Status);
            Assert.AreEqual("Wooody Igor (iwoo) from RAAA3", viewResult.ViewBag.EmployeeInformation);
            Assert.AreEqual(16, businessTripModel.BusinessTripID);

        }

        [Test]
        public void GetEditReportedBT_ValidBT_StartDateLessOrIsEqualToDateNow_ValidStatus()
        {
            //Arrange

            // Act
            var viewResult = controller.EditReportedBT(22) as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            BusinessTripViewModel businessTripModel = (BusinessTripViewModel)viewResult.ViewData.Model;
            //Assert 
            Assert.AreEqual("EditReportedCurrentBT", viewResult.ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual((BTStatus.Confirmed | BTStatus.Reported), businessTrip.Status);
            Assert.AreEqual("Wooody Igor (iwoo) from RAAA3", viewResult.ViewBag.EmployeeInformation);
            Assert.AreEqual(22, businessTripModel.BusinessTripID);
        }

        [Test]
        public void GetEditReportedBT_ValidBTAccComment_StartDateLessToDateNow_View()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip();
            bTrip.AccComment = defaultAccComment;

            //Act
            var result = controller.EditReportedBT(25, null) as ViewResult;

            //Assert
            Assert.AreEqual("EditReportedForAccComment", result.ViewName);
        }

        #endregion

        #region PostEditReportedBT

        //[Test]
        //public void PostEditReportedBT_NotExistingBT_HttpNotFound()
        //{
        //    //Arrange

        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();

        //    // Act
        //    var view = controller.EditReportedBT(businessTrip, "");


        //    //Assert 
        //    Assert.IsInstanceOf(typeof(HttpNotFoundResult), view.GetType());
        //    Assert.IsNull(businessTrip);
        //    Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        //}

        //[Test]
        //public void PostEditReportedBT_NullInputBT_HttpNotFound()
        //{
        //    //Arrange

        //    BusinessTrip businessTrip = null;

        //    // Act
        //    var view = controller.EditReportedBT(businessTrip, "");


        //    //Assert 
        //    Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
        //    Assert.IsNull(businessTrip);
        //    Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        //}

        [Test]
        public void PostEditReportedBT_NotValidPlannedBT_Viewresult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("LocationID", "Field LocationID must be not null");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);

            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Planned, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_ValidPlannedBT_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 1, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1 ,UnitID = 1,Unit = new Unit()};

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Planned, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_NotValidRegisteredBT_Viewresult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 2, StartDate = new DateTime(2014, 11, 01), EndDate = new DateTime(2014, 11, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered, EmployeeID = 2, LocationID = 1, Habitation = "krakow", HabitationConfirmed = true, UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("LocationID", "Field LocationID must be not null");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Registered, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_ValidRegisteredBT_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 2, StartDate = new DateTime(2014, 11, 01), EndDate = new DateTime(2014, 11, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered, EmployeeID = 2, LocationID = 1, Habitation = "krakow", HabitationConfirmed = true ,UnitID = 1,Unit = new Unit()};

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Registered, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_NotValidConfirmedBT_Viewresult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("LocationID", "Field LocationID must be not null");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_ValidConfirmedBT_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, UnitID = 1, Unit = new Unit() };

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_NotValidConfirmedModifiedBT_Viewresult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2014, 10, 01), EndDate = new DateTime(2014, 10, 12), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2, UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("LocationID", "Field LocationID must be not null");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_ValidConfirmedModifiedBT_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2014, 10, 01), EndDate = new DateTime(2014, 10, 12), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2,UnitID = 1,Unit = new Unit() };

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedFutureBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostEditReportedBT_InValidReportedCurrentBT_NullSelectedDepartment_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 29, StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" ,UnitID = 1, Unit = new Unit()};
            controller.ModelState.AddModelError("error", "error");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 29).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedCurrentBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostEditReportedBT_InValidReportedCurrentBT_EmptySelectedDepartment_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 29, StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" ,UnitID = 1, Unit = new Unit()};
            controller.ModelState.AddModelError("error", "error");

            // Act
            var view = controller.EditReportedBT(bt, "");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 29).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedCurrentBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostEditReportedBT_InValidPlannedCurrentBT_SDDDASelectedDepartment_RedirectToAction()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 1, StartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-10), EndDate = DateTime.Now.ToLocalTimeAzure().AddDays(2), OrderStartDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(-11), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting",UnitID = 1, Unit = new Unit() };
            controller.ModelState.AddModelError("error", "error");

            // Act
            var view = controller.EditReportedBT(bt, "SDDDA");
            ViewResult viewResult = view as ViewResult;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual("EditReportedCurrentBT", viewResult.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), viewResult.Model);
            Assert.AreEqual(BTStatus.Planned, businessTrip.Status);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostEditReportedBT_NoChangesInBT_Viewresult()
        {

            //Arrange
            messengerMock = new Mock<IMessenger>();
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };


            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            JsonResult result = (JsonResult)controller.EditReportedBT(businessTrip, "SDDDA");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);

            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.AreEqual(new DateTime(2013, 10, 01), bt.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bt.OrderStartDate);
        }

        [Test]
        public void PostEditReportedBT_NoChangesInBTEmptySelectedDep_Viewresult()
        {
            //Arrange
            messengerMock = new Mock<IMessenger>();
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };


            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            JsonResult result = (JsonResult)controller.EditReportedBT(businessTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);

            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.AreEqual(new DateTime(2013, 10, 01), bt.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bt.OrderStartDate);
        }

        [Test]
        public void PostEditReportedBT_NoChangesInBTNullSelectedDep_Viewresult()
        {
            //Arrange
            messengerMock = new Mock<IMessenger>();
            //BusinessTrip bt =  new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure(), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), DaysInBtForOrder = ((DateTime.Now.ToLocalTimeAzure().Date.AddDays(2)).Date - (new DateTime(2013, 09, 30)).Date).Days + 1, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };


            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            JsonResult result = (JsonResult)controller.EditReportedBT(businessTrip, null);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(businessTrip),Times.Never());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);

            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.AreEqual(new DateTime(2013, 10, 01), businessTrip.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), businessTrip.OrderStartDate);
        }

        [Test]
        public void PostEditReportedBT_StartDateChangedAndVisaNull_RedirectToAction()
        {
            //Arrange

            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2013, 10, 10), EndDate = new DateTime(2014, 12, 26), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2014, 12, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 10), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2014, 11, 30), bTrip.OrderStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.OldEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 27), bTrip.OrderEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.EndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_OrderStartDateChangedAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 26), OrderStartDate = new DateTime(2014, 11, 29), OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 14, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 14).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2014, 12, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 01), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2014, 11, 29), bTrip.OrderStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.OldEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 27), bTrip.OrderEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.EndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_OrderStartDateChangedToNullAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 26), OrderStartDate = null, OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2014, 12, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 01), bTrip.StartDate);
            Assert.AreEqual(null, bTrip.OrderStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.OldEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 27), bTrip.OrderEndDate);
            Assert.AreEqual(new DateTime(2014, 12, 26), bTrip.EndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_EndDateChangedAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(2), OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bTrip.OrderStartDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date.AddDays(2), bTrip.EndDate);
            //  Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.OldEndDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date.AddDays(1), bTrip.OrderEndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_OrderEndDateChangedAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bTrip.OrderStartDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.EndDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.OldEndDate.Value.Date);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date.AddDays(3), bTrip.OrderEndDate.Value.Date);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_OrderEndDateChangedToNullAndVisaNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, OrderStartDate = new DateTime(2013, 09, 30), OrderEndDate = null, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2013, 09, 30), bTrip.OrderStartDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.EndDate);
            Assert.AreEqual(DateTime.Now.ToLocalTimeAzure().Date, bTrip.OldEndDate.Value.Date);
            Assert.AreEqual(null, bTrip.OrderEndDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_LocatioIDChangedAndVisaNull_NullSelectedDep_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2013, 10, 01), EndDate = DateTime.Now.ToLocalTimeAzure().Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 2, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            Assert.IsNull(bTrip.BTof.Visa);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.StartDate);
            Assert.AreEqual(2, bTrip.LocationID);
            Assert.AreEqual(1, bTrip.OldLocationID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_FutureBTOrderDatesNullsAndVisaNotNull_EmptySelectedDep_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 25, StartDate = new DateTime(2014, 12, 10), EndDate = new DateTime(2014, 12, 12), OrderStartDate = null, OrderEndDate = null, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof = employee;
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.AreEqual(22, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTrip.BTof.Visa.EntriesUsedInBT);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 10), bTrip.StartDate);
            Assert.AreEqual(null, bTrip.OrderStartDate);
            Assert.AreEqual(null, bTrip.OrderEndDate);
            Assert.AreEqual(null, bTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_FutureBTDefaultOrderDatesAndVisaNotNull_NullSelectedDep_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 25, StartDate = new DateTime(2014, 12, 10), EndDate = new DateTime(2014, 12, 12), OrderStartDate = default(DateTime), OrderEndDate = default(DateTime), DaysInBtForOrder = (default(DateTime).Date - default(DateTime).Date).Days + 1, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof = employee;
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.AreEqual(22, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTrip.BTof.Visa.EntriesUsedInBT);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 10), bTrip.StartDate);
            Assert.AreEqual(default(DateTime), bTrip.OrderStartDate);
            Assert.AreEqual(default(DateTime), bTrip.OrderEndDate);
            Assert.AreEqual(1, bTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }


        [Test]
        public void PostEditReportedBT_FutureBTOrderDatesNotNullsAndVisaNotNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 25, StartDate = new DateTime(2014, 12, 10), EndDate = new DateTime(2014, 12, 12), OrderStartDate = new DateTime(2014, 12, 09), OrderEndDate = new DateTime(2014, 12, 13), DaysInBtForOrder = ((new DateTime(2014, 12, 13)).Date - (new DateTime(2014, 12, 09)).Date).Days + 1, Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof = employee;
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.AreEqual(22, bTripFromMock.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTripFromMock.BTof.Visa.EntriesUsedInBT);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 10), bTrip.StartDate);
            Assert.AreEqual(new DateTime(2014, 12, 09), bTrip.OrderStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 13), bTrip.OrderEndDate);
            Assert.AreEqual(5, bTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_CurrentBTAndVisaNotNull_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 22, StartDate = new DateTime(2014, 12, 10), EndDate = new DateTime(2014, 12, 12), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 5, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTripFromMock.BTof = employee;
            bTripFromMock.BTof.Visa = visa;
            int? NewDaysUsedInBT = bTripFromMock.BTof.Visa.DaysUsedInBT - ((DateTime.Now.ToLocalTimeAzure().Date - new DateTime(2013, 10, 01).Date).Days + 1);

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Once);
            Assert.AreEqual(NewDaysUsedInBT, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTripFromMock.BTof.Visa.EntriesUsedInBT);
            Assert.AreEqual(new DateTime(2013, 10, 01), bTrip.OldStartDate);
            Assert.AreEqual(new DateTime(2014, 12, 10), bTrip.StartDate);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip.Status);
        }

        [Test]
        public void PostEditReportedBT_StartDateChangedAndVisaNullConcurrency_RedirectToAction()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 16, StartDate = new DateTime(2013, 10, 10), EndDate = new DateTime(2014, 12, 26), OrderStartDate = new DateTime(2014, 11, 30), OrderEndDate = new DateTime(2014, 12, 27), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 7, LocationID = 1, Comment = "7 employee conf and reported", Manager = "xtwe", Purpose = "meeting" };
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == 7).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            bTrip.BTof = employee;

            // Act
            JsonResult result = (JsonResult)controller.EditReportedBT(bTrip, "");
            BusinessTrip bTripFromMock = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 16).FirstOrDefault();
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once());
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCModifiesConfirmedReportedToEMP))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion


        #region IndexACCforAccountableBTs

        [Test]
        public void IndexACCforAccountableBTs()
        {
            //Arrange
            // Act
           
            var query = controller.IndexACCforAccountableBTs();

            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), query);
            Assert.AreEqual("", ((ViewResult)query).ViewName);
            Assert.AreEqual(1,((List<BusinessTrip>)query.ViewData.Model).Count);
            Assert.AreEqual(17, ((List<BusinessTrip>)query.Model).ToArray()[0].EmployeeID);
            Assert.AreEqual("ncru", ((List<BusinessTrip>)query.Model).ToArray()[0].LastCRUDedBy);
            Assert.AreEqual(1, ((List<BusinessTrip>)query.Model).ToArray()[0].LocationID);
            Assert.AreEqual((BTStatus.Confirmed | BTStatus.Reported), ((List<BusinessTrip>)query.Model).ToArray()[0].Status);
        }

        #endregion

        #region CancelReportedBT

        [Test]
        public void CancelReportedBTGet_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBT(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void CancelReportedBTGet_ExistingBT_CorrectViewLoads()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBT(3);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTGet_ExistingBTIdAndEmptyString_CorrectViewLoads()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBT(3, "");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTGet_ExistingBTAndSDDDA_CorrectViewLoads()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBT(3, "SDDDA");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        #endregion

        #region CancelReportedBTConfirm

        [Test]
        public void CancelReportedBTConfirm_NullBusinessTrip_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(0, "", null);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void CancelReportedBTConfirm_NullBusinessTripEmptyDepartment_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(0, "too expensive", "");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void CancelReportedBTConfirm_NullCancelComment_CannotCancel()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(22, null, null);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never);
            Assert.AreEqual(null, businessTrip.CancelComment);
            Assert.IsNull(((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_StringEmptyCancelComment_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(22, "", null);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == ""
                && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never); 
            Assert.IsNull(((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_NullCancelCommentAndEmptyDepartment_CannotCancel()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(22, null, "");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never);
            Assert.AreEqual(null, businessTrip.CancelComment);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_NullCancelCommentSelectedDepartment_CannotCancel()
        {
            //Arrange

            // Act
            var view = controller.CancelReportedBTConfirm(22, null, "RAAA4");
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never);
            Assert.AreEqual(null, businessTrip.CancelComment);
            Assert.AreEqual("RAAA4", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_StringEmptyCancelCommentSelectedDepartment_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(22, "", "RAAA4");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == ""
                && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never);
            Assert.AreEqual("RAAA4", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_StatusOfBTisPlanned_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(1, "too expensive");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(1, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Planned, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "too expensive")), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndCommentIsNull_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            var view = controller.CancelReportedBTConfirm(22, null, null);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(null, businessTrip.CancelComment);

        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndCommentIsEmpty_CannotCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();
        
            // Act
            var view = controller.CancelReportedBTConfirm(22, "", "");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(22, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, businessTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "")), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNull_CanCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(22, "BT is too expensive");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("success", data);

            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "BT is too expensive"
    && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Once);
                    
            Assert.IsNull(businessTrip.BTof.Visa);

        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNullSelectedDept_CanCancel()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(22, "BT is too expensive", "RAAA4");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("success", data);

            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "BT is too expensive"
    && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Once);
            Assert.IsNull(businessTrip.BTof.Visa);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNotNull_CanCancel()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(25, "BT is too expensive");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 

            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);

            //Assert.IsFalse(((RedirectToRouteResult)view).Permanent);
            //Assert.IsInstanceOf(typeof(RedirectToRouteResult), view);
            //Assert.AreEqual("ACCView", ((RedirectToRouteResult)view).RouteValues["action"]);
            //Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            //Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            //Assert.AreEqual(0, ((RedirectToRouteResult)view).RouteValues["tab"]);
            //Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);

            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment =="BT is too expensive" 
                && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Once);

            //Assert.AreEqual("BT is too expensive", bTrip.CancelComment);
            //Assert.AreEqual((BTStatus.Confirmed | BTStatus.Cancelled), bTrip.Status);
            Assert.AreEqual(22, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTrip.BTof.Visa.EntriesUsedInBT);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNotNullSelectedDept_CanCancel()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(25, "BT is too expensive", "RAAA4");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "success")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("success", data);

            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.CancelComment == "BT is too expensive"
    && b.Status == (BTStatus.Confirmed | BTStatus.Cancelled))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Once);

            //Assert.AreEqual("BT is too expensive", bTrip.CancelComment);
            //Assert.AreEqual((BTStatus.Confirmed | BTStatus.Cancelled), bTrip.Status);
            Assert.AreEqual(22, bTrip.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(2, bTrip.BTof.Visa.EntriesUsedInBT);
        }

        [Test]
        public void CancelReportedBTConfirm_ReportedStatusAndValidCommentAndVisaNotNullSelectedDeptConcurrency_CanCancel()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            Visa visa = new Visa { VisaOf = bTrip.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 25, EmployeeID = bTrip.EmployeeID, Entries = 5, EntriesUsedInBT = 3, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip.BTof.Visa = visa;

            // Act
            JsonResult result = (JsonResult)controller.CancelReportedBTConfirm(25, "BT is too expensive", "RAAA4");
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);

            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ACCCancelsConfirmedReportedToResponsibleInLocation))), Times.Never);

            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region SaveAccComment

        [Test]
        public void SaveAccComment_nullBT_HttpNotFound()
        {
            //Arrange
            BusinessTrip bTrip = null;

            //Act

            var result = controller.SaveAccComment(bTrip);
            
            //Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.IsNull(bTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
        }

        [Test]
        public void SaveAccComment_PlannedBT_JsonError()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Act
            var result = controller.SaveAccComment(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;
            BusinessTrip bTripAfterMethodCalled = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual("error", data);
            Assert.AreEqual(BTStatus.Planned, bTripAfterMethodCalled.Status);
        }

        [Test]
        public void SaveAccComment_ReportedBT_JsonSuccess()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 15).FirstOrDefault(); 

            //Act
            var result = controller.SaveAccComment(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "success")).Target;
            BusinessTrip bTripAfterMethodCalled = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 15).FirstOrDefault();

            //Assert
            Assert.IsInstanceOf(typeof(JsonResult),result);
            Assert.AreEqual("success", data);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, bTripAfterMethodCalled.Status);
            Assert.AreEqual("Test Comment", bTripAfterMethodCalled.AccComment);
        }

        [Test]
        public void SaveAccComment_ReportedBTConcurrency_JsonErrorReturned()
        {
            // Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 15, AccComment = "test2", Status = BTStatus.Confirmed | BTStatus.Reported };
            mock.Setup(m => m.SaveBusinessTrip(bTrip)).Throws(new DbUpdateConcurrencyException());

            // Act
            var result = controller.SaveAccComment(bTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;
            BusinessTrip bTripAfterMethodCalled = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 15).FirstOrDefault();

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            Assert.AreEqual("Test Comment", bTripAfterMethodCalled.AccComment);
            Assert.AreEqual(modelError, data);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, bTripAfterMethodCalled.Status);
        }

        #endregion


        #region ShowAccountableBTData


        [Test]
        public void ShowAccountableBTData_NotExistingBusinessTripID_HttpNotFound()
        {
            //Arrange

            //Act
            var result = controller.ShowAccountableBTData(777);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void ShowAccountableBTData_NotExistingBussinessTrip_HttpNotFound()
        {
            //Arrange

            BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            var view = controller.ShowAccountableBTData(100);

            // Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }


        [Test]
        public void ShowAccountableBTData_ShowBTWithBussinessTripIDIs1_ShowAccountableBTDataView()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            var view = controller.ShowAccountableBTData(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);

            Assert.AreEqual(1, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);

        }

        [Test]
        public void ShowAccountableBTData_ExistingBT_ShowAccountableBTDataView()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            var view = controller.ShowAccountableBTData(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);

            Assert.AreEqual(10, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);

        }

        [Test]
        public void ShowAccountableBTData_ExistingBusinessTripID_ShowAccountableBTDataView()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = (ViewResult)controller.ShowAccountableBTData(12);
            BusinessTripViewModel resultModel = (BusinessTripViewModel)result.Model;

            // Assert
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)result).Model);
            Assert.AreEqual(12, resultModel.BusinessTripID);
        }

        [Test]
        public void ShowAccountableBTData_NotExistingBusinessTrip_HttpNotFound()
        {
            //Arrange

            //Act
            var result = controller.ShowAccountableBTData(1000);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }
        #endregion


        #endregion


        #region ADM

        #region GetBusinessTripADM

        [Test]
        public void GetBusinessTripADM_DefaultUserNameSelectedDepartmentNull_SelectedDepartmentNull()
        {
            // Arrange

            // Act
            string UserName = "";
            string selectedDepartment = null;
            var view = controller.GetBusinessTripADM(UserName, selectedDepartment);
            SelectList departmentsList = ((ViewResult)view).ViewBag.DepartmentList as SelectList;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.UserDepartment);

        }

        [Test]
        public void GetBusinessTripADM_UserNameAndzSelectedDepartmentNull_SelectDepartmentNull()
        {
            // Arrange

            // Act
            string UserName = "andl";
            string selectedDepartment = null;
            var view = controller.GetBusinessTripADM(UserName, selectedDepartment);
            SelectList departmentsList = ((ViewResult)view).ViewBag.DepartmentList as SelectList;


            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.UserDepartment);
        }

        [Test]
        public void GetBusinessTripADM_UserNameAndzSelectedDepartmentStringEmpty_SelectDepartmentStringEmpty()
        {
            // Arrange

            // Act
            string UserName = "andl";
            string selectedDepartment = "";
            var view = controller.GetBusinessTripADM(UserName, selectedDepartment);
            SelectList departmentsList = ((ViewResult)view).ViewBag.DepartmentList as SelectList;


            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("SDDDA", ((ViewResult)view).ViewBag.UserDepartment);
        }

        [Test]
        public void GetBusinessTripADM_UserNameAndzSelectedDepartmentRAAA1_SelectDepartmentRAAA1()
        {
            // Arrange

            // Act
            string UserName = "andl";
            string selectedDepartment = "RAAA1";
            var view = controller.GetBusinessTripADM(UserName, selectedDepartment);
            SelectList departmentsList = ((ViewResult)view).ViewBag.DepartmentList as SelectList;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.IsTrue(departmentsList.ToList().Count == 7);
            Assert.AreEqual(departmentsList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual("RAAA1", ((ViewResult)view).ViewBag.SelectedDepartment);
     
        }



        #endregion

        #region GetBusinessTripDataADM

        [Test]
        public void GetBusinessTripDataADM_AllEmployeesSelectedDepartmentStringEmpty_AllEmployees()
        {
            // Arrange - create the controller     

            // Act - call the action method
            string selectedDepartment = "";
            string selectedUserDepartment = null;

            var view = controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment);

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment).Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(23, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].EmployeeID, 8);
            Assert.AreEqual(employeeVisaView[1].EmployeeID, 2);
            Assert.AreEqual(employeeVisaView[2].EmployeeID, 5);
        }

        [Test]
        public void GetBusinessTripDataADM_AllEmployeesSelectedDepartmentNull_AllEmployeesInselectedUserDepartment()
        {
            // Arrange  

            // Act - call the action method
            string selectedDepartment = null;
            string selectedUserDepartment = "SDDDA";

            var view = controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment);

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment).Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(4, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].EmployeeID, 8);
            Assert.AreEqual(17, employeeVisaView[1].EmployeeID);
        }

        [Test]
        public void GetBusinessTripDataADM_AllEmployeesSelectedDepartmentStringEmpty_AllEmployeesInselectedUserDepartment()
        {
            // Arrange - create the controller     

            // Act - call the action method
            string selectedDepartment = "";
            string selectedUserDepartment = "SDDDA";

            var view = controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment);

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment).Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(23, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].EmployeeID, 8);
            Assert.AreEqual(employeeVisaView[1].EmployeeID, 2);
            Assert.AreEqual(employeeVisaView[2].EmployeeID, 5);
        }

        [Test]
        public void GetBusinessTripDataADM_AllEmployeesInUserDepartment_RAAA3()
        {
            // Arrange - create the controller     

            // Act - call the action method
            string selectedDepartment = "RAAA3";
            string selectedUserDepartment = "TAAAA";

            var view = controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment);

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataADM(selectedDepartment, selectedUserDepartment).Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(16, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].EmployeeID, 7);
            Assert.AreEqual(19, employeeVisaView[1].EmployeeID);
        }

        #endregion

        #region RegisterPlannedBTs

        [Test]
        public void RegisterPlannedBTs_SelectedTwoPlannedBTs_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "10", "11" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Once);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
            Assert.AreEqual(bTrip2.LastCRUDedBy, "cbur");
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_SelectedPlannedModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "12" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Once);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip1.Status);
        }

        [Test]
        public void RegisterPlannedBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);
            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);

            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(null);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_NotPlannedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { "2", "3" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Confirmed, bTrip2.Status);
        }

        [Test]
        public void RegisterPlannedBTs_SelectedPlannedModifiedBTConcurrency_JsonErrorReturned()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "12" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.RegisterPlannedBTs(selectedIDsOfPlannedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }
        #endregion

        #region RegisterPlannedBT

        //[Test]
        //public void RegisterPlannedBT_ValidBT_ChangedStatus()
        //{
        //    // Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;           
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.RegisterPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);          
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Once);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
        //    Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
        //}

        //[Test]
        //public void RegisterPlannedBT_NotValidBT_NotChangedStatus()
        //{
        //    // Arrange
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.RegisterPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
        //}

        //[Test]
        //public void RegisterPlannedBT_SelectedPlannedModifiedBT_ChangedStatus()
        //{
        //    // Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;     
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.RegisterPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Once);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip1.Status);
        //    Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
        //    Assert.IsNull(bTrip1.RejectComment);
        //}
 
        //[Test]
        //public void RegisterPlannedBT_NotValidModel_NotChangedStatus()
        //{
        //    // Arrange
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
        //    controller.ModelState.AddModelError("error", "error");
        //    // Act
        //    var result = (ViewResult)controller.RegisterPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToBTM))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMRegistersPlannedOrPlannedModifiedToEMP))), Times.Never);
        //    Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
        //    Assert.AreEqual("EditPlannedBT", result.ViewName);
        //    Assert.IsInstanceOf(typeof(BusinessTripViewModel), result.Model);
        //    Assert.IsInstanceOf(typeof(SelectList), result.ViewBag.LocationsList);


        //}


        #endregion

        #region ConfirmPlannedBTs
        [Test]
        public void ConfirmPlannedBTs_SelectedTwoPlannedBTs_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "10", "11" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.AreEqual(BTStatus.Confirmed, bTrip1.Status);
            Assert.AreEqual(BTStatus.Confirmed, bTrip2.Status);
            Assert.AreEqual(bTrip2.LastCRUDedBy, "cbur");
            Assert.AreEqual(bTrip2.LastCRUDedBy, "cbur");
        }

        [Test]
        public void ConfirmPlannedBTs_SelectedPlannedModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "12" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);
           
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
        }

        [Test]
        public void ConfirmPlannedBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmPlannedBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs();

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmPlannedBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmPlannedBTs_SelectedPlannedRejectedBT_StatusNotChanged()
        {
            // Arrange
            string[] selectedIDsOfPlannedBTs = { "20" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip1.Status);
            Assert.IsNotNull(bTrip1.RejectComment);
        }

        [Test]
        public void ConfirmPlannedBTs_SelectedPlannedModifiedBTConcurrency_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "12" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

            // Act
            var result = controller.ConfirmPlannedBTs(selectedIDsOfPlannedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }
        #endregion

        #region ConfirmRegisteredBTs
        [Test]
        public void ConfirmRegisteredBTs_SelectedRegisteredBT_StatusChanged()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "2" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfPlannedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Confirmed, bTrip1.Status);
            Assert.IsNull(bTrip1.RejectComment);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");

        }

        [Test]
        public void ConfirmRegisteredBTs_SelectedRegisteredBTAndRegisteredModified_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "2", "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert

            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Confirmed, bTrip1.Status);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip2.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");

        }

        [Test]
        public void ConfirmRegisteredBTs_SelectedRegisteredModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Once);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

        }

         [Test]
        public void ConfirmRegisteredBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }


        [Test]
        public void ConfirmRegisteredBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(null,"SDDDA");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("SDDDA", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmRegisteredBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ConfirmRegisteredBTs_SelectedRegisteredCancelledBT_StatusNotChanged()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "17" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 17).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfRegisteredBTs,"RAAA1");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA1", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
        }

        public void ConfirmRegisteredBTs_SelectedRegisteredBTConcurrency_JsonErrorResult()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfPlannedBTs = { "2" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            // Act
            var result = controller.ConfirmRegisteredBTs(selectedIDsOfPlannedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void ConfirmRegisteredBTs_ValidModelConcurrency_ErrorReturned()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();

            string[] selectedIDsOfPlannedBTs = { "2" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(bTrip1)).Throws(new DbUpdateConcurrencyException());
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            // Act
            JsonResult result = (JsonResult)controller.ConfirmRegisteredBTs(selectedIDsOfPlannedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            Assert.IsNull(bTrip1.RejectComment);

        }

        #endregion

        #region ReplanRegisteredBTs
        [Test]
        public void ReplanRegisteredBTs_SelectedTwoRegisteredBTs_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "2", "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs,"RAAA1");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA1", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip1.Status);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip2.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
            Assert.AreEqual(bTrip2.LastCRUDedBy, "cbur");
        }

        [Test]
        public void ReplanRegisteredBTs_SelectedRegisteredModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs,"");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");

        }

        [Test]
        public void ReplanRegisteredBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs,"SDDDA");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("SDDDA", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ReplanRegisteredBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(null,"");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ReplanRegisteredBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void ReplanRegisteredBTs_SelectedRegisteredCancelledBT_StatusNotChanged()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "17" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 17).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
        }

        [Test]
        public void ReplanRegisteredBTs_SelectedRegisteredModifiedBTConcurrency_JsonErrorResult()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.ReplanRegisteredBTs(selectedIDsOfRegisteredBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;


            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMReplansRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region CancelRegisteredBTs
        [Test]
        public void CancelRegisteredBTs_SelectedTwoRegisteredBTs_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "2", "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert

            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip2.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
            Assert.AreEqual(bTrip2.LastCRUDedBy, "cbur");
        }

        [Test]
        public void CancelRegisteredBTs_SelectedRegisteredModifiedBT_ChangedStatus()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");

        }

        [Test]
        public void CancelRegisteredBTs_NotSelectedBTs_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();


            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void CancelRegisteredBTs_NullSelectedBTs_NotChangedStatus()
        {
            // Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void CancelRegisteredBTs_NotParsableStringArray_NotChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "a" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            mock.Verify(m => m.SaveBusinessTrip(bTrip2), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Modified, bTrip2.Status);
        }

        [Test]
        public void CancelRegisteredBTs_SelectedRegisteredCancelledBT_StatusNotChanged()
        {
            // Arrange
            string[] selectedIDsOfRegisteredBTs = { "17" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 17).FirstOrDefault();

            // Act
            var result = controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);

            // Assert

            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(BTStatus.Registered | BTStatus.Cancelled, bTrip1.Status);
        }

        [Test]
        public void CancelRegisteredBTs_SelectedRegisteredModifiedBT_JsonErrorResult()
        {
            // Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            string[] selectedIDsOfRegisteredBTs = { "13" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.CancelRegisteredBTs(selectedIDsOfRegisteredBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsRegisteredOrRegisteredModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region CancelConfirmedBT
        [Test]
        public void CancelConfirmedBT_ExistingConfirmedBT_StatusChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(5, null);

            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual(null, result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Cancelled, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
        }

        [Test]
        public void CancelConfirmedBT_ExistingConfirmedModifiedBT_StatusChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 14).FirstOrDefault();
            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(14, "");
            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("", result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified | BTStatus.Cancelled, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");

        }

        [Test]
        public void CancelConfirmedBT_NotExistingBT_StatusChanged()
        {
            //Arrange
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();
            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(1000, "RAAA1");
            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("RAAA1", result.RouteValues["selectedDepartment"]);
            Assert.IsNull(bTrip1);
        }

        [Test]
        public void CancelConfirmedBT_ExistingReportedBT_StatusNotChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 22).FirstOrDefault();

            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(22, "RAAA3");

            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("RAAA3", result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Reported, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "ncru");
        }

        [Test]
        public void CancelConfirmedBT_ExistingPlannedBT_StatusNotChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(1, "RAAA3");

            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("RAAA3", result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "ncru");
        }

        [Test]
        public void CancelConfirmedBT_ExistingRegisteredBT_StatusNotChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            var result = (RedirectToRouteResult)controller.CancelConfirmedBT(2, "RAAA3");

            //Assert        
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsFalse(result.Permanent);
            Assert.IsFalse(result.Permanent);
            Assert.AreEqual("ADMView", result.RouteValues["action"]);
            Assert.AreEqual(null, result.RouteValues["id"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual(1, result.RouteValues["tab"]);
            Assert.AreEqual("RAAA3", result.RouteValues["selectedDepartment"]);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
            Assert.AreEqual(bTrip1.LastCRUDedBy, "ncru");
        }

        [Test]
        public void CancelConfirmedBT_ExistingConfirmedBTConcurrency_JsonErrorResult()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            JsonResult result = (JsonResult)controller.CancelConfirmedBT(5, null);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert        
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToBTM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToDIR))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region ConfirmPlannedBT
        //[Test]
        //public void ConfirmPlannedBT_SelectedPlannedBT_ChangedStatus()
        //{
        //    // Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;    
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.ConfirmPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Confirmed, bTrip1.Status);
        //    Assert.AreEqual(bTrip1.LastCRUDedBy, "cbur");
        //}

        //[Test]
        //public void ConfirmPlannedBT_SelectedPlannedModifiedBT_ChangedStatus()
        //{
        //    // Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;    
        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 28).FirstOrDefault();
        //    BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 28, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 25), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 7, LocationID = 1, Comment = "7 employee plan + modif", Manager = "xtwe", Purpose = "meeting", Habitation = "krakow", HabitationConfirmed = true };

        //    // Act
        //    var result = (RedirectToRouteResult)controller.ConfirmPlannedBT(bTrip);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Once);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Once);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
        //    Assert.AreEqual(businessTrip.LastCRUDedBy, "cbur");
        //    Assert.IsNull(businessTrip.RejectComment);
        //}

        //[Test]
        //public void ConfirmPlannedBT_SelectedNotPlannedBT_NotChangedStatus()
        //{
        //    // Arrange
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

        //    // Act
        //    var result = (RedirectToRouteResult)controller.ConfirmPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //    Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
        //}

        //[Test]
        //public void ConfirmPlannedBT_NotValidModel_NotChangedStatus()
        //{
        //    // Arrange
        //    BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();
        //    controller.ModelState.AddModelError("error", "error");
        //    // Act
        //    var result = (ViewResult)controller.ConfirmPlannedBT(bTrip1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToBTM))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToDIR))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToEMP))), Times.Never);
        //    messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMConfirmsPlannedOrRegisteredToResponsibleInLocation))), Times.Never);
        //    Assert.AreEqual(BTStatus.Planned, bTrip1.Status);
        //    Assert.AreEqual("EditPlannedBT", result.ViewName);
        //    Assert.IsInstanceOf(typeof(BusinessTripViewModel), result.Model);
        //    Assert.IsInstanceOf(typeof(SelectList), result.ViewBag.LocationsList);


        //}
        #endregion

        #region DeletePlannedBT
        [Test]
        public void Get_DeletePlannedBT_NotExistingBusinessTripID_HttpNotFound()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBT(1000);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void Get_DeletePlannedBT_ValidBusinessTripID_BTisPlanned_DeleteConfirmation()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBT(10);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(bTrip, ((ViewResult)result).Model);
        }

        [Test]
        public void Get_DeletePlannedBT_ValidBusinessTripID_BTisPlannedModified_DeleteConfirmation()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBT(12);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(bTrip, ((ViewResult)result).Model);
        }

        [Test]
        public void Get_DeletePlannedBT_ValidBusinessTripID_BTisNotPlanned_DeleteConfirmation()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBT(2);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        #endregion

        #region DeletePlannedBTConfirmed
        [Test]
        public void DeletePlannedBTConfirmed_NotExistingBusinessTripID_View()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();

            //Act
            var result = controller.DeletePlannedBTConfirmed(1000);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            mock.Verify(m => m.DeleteBusinessTrip(1000), Times.Never);


        }

        [Test]
        public void DeletePlannedBTConfirmed_ValidBusinessTripID_BTisPlanned_DeleteConfirmation()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 10).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBTConfirmed(10);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(10), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(bTrip.LastCRUDedBy, "cbur");

        }

        [Test]
        public void DeletePlannedBTConfirmed_ValidBusinessTripID_BTisPlannedModified_DeleteConfirmation()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = controller.DeletePlannedBTConfirmed(12);

            // Assert
            Assert.AreEqual(BTStatus.Planned | BTStatus.Cancelled, bTrip.Status);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual(bTrip.LastCRUDedBy, "cbur");
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsPlannedModifiedToBTM))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.ADMCancelsPlannedModifiedToACC))), Times.Once);
        }

        [Test]
        public void DeletePlannedBTConfirmed_ValidBusinessTripID_BTisNotPlanned_DeleteConfirmation()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            var result = controller.DeletePlannedBTConfirmed(2);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(2), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);

        }

        [Test]
        public void DeletePlannedBT_ValidBusinessTripID_BTisPlannedModified_Concurrency_JsonErrorResult()
        {
            //Arrange
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            JsonResult result = (JsonResult)controller.DeletePlannedBTConfirmed(12);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;
         
            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }
        #endregion

        #region PlanGet
        [Test]
        public void GetPlanBT_ExistingEmployee_PlanBTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            var viewBagEmployeeInformation = employee.LastName + " " + employee.FirstName + " (" + employee.EID + ") from " + employee.Department.DepartmentName;
            var viewBagLocationsList = from loc in mock.Object.Locations orderby loc.Title select loc;
            SelectList viewLocationsList = new SelectList(viewBagLocationsList, "LocationID", "Title");
            //Act
            var result = controller.Plan(1);
            //Assert       
            Assert.IsInstanceOf(typeof(ViewResult), (ViewResult)result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(viewBagEmployeeInformation, ((ViewResult)result).ViewBag.EmployeeInformation);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);

        }


        [Test]
        public void GetPlanBT_NonExistingEmployee_PlanBTForm()
        {
            //Arrange
            Employee employee = (from e in mock.Object.Employees where e.EmployeeID == 1000 select e).FirstOrDefault();

            //Act
            var result = controller.Plan(1000);
            //Assert       
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);

        }

        #endregion

        #region PlanPost

        [Test]
        public void PostPlanBT_PlannedBT_PlanBTForm()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 0, StartDate = new DateTime(2015, 09, 01), EndDate = new DateTime(2015, 09, 27), Status = BTStatus.Planned, EmployeeID = 2, LocationID = 1, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting to Lodz" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        }

        [Test]
        public void PostPlanBT_PlannedBTSelectedDepEmpty_PlanBTForm()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 0, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 27), Status = BTStatus.Planned, EmployeeID = 20, LocationID = 1, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting to Lodz" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.Plan(bt, "");

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        }

        [Test]
        public void PostPlanBT_NotValidBTSelectedDepartment_PlanBTForm()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 0,
                StartDate = new DateTime(2013, 09, 01),
                EndDate = new DateTime(2013, 09, 27),
                OrderStartDate = new DateTime(2013, 09, 01),
                OrderEndDate = new DateTime(2013, 09, 27),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 28,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
            controller.ModelState.AddModelError("error", "error");
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            //Act
            var result = (ViewResult)controller.Plan(bt, "RAAA1");

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Plan", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA1", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);
            Assert.AreEqual("RAAA1", ((ViewResult)result).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostPlanBT_NotValidBT_PlanBTForm()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 0,
                StartDate = new DateTime(2013, 09, 01),
                EndDate = new DateTime(2013, 09, 27),
                OrderStartDate = new DateTime(2013, 09, 01),
                OrderEndDate = new DateTime(2013, 09, 27),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 27,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
            controller.ModelState.AddModelError("error", "error");

            //Act
            var result = (ViewResult)controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Plan", result.ViewName);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), result.Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.LocationsList);
            Assert.AreEqual(null, ((ViewResult)result).ViewBag.SelectedDepartment);
        }

        [Test]
        public void PostPlanBT_TheSamePlannedModifiedBT_JsonErrorResult()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            bt.StartDate = new DateTime(2013, 08, 01);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.Plan(bt, "RAAA4");

            //Assert   
         
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(20, bt.BusinessTripID);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA4", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Kyiv - Krakow", bt.Flights);
            Assert.AreEqual(1, bt.OldLocationID);
            Assert.AreEqual("cbur", bt.LastCRUDedBy);
            Assert.AreEqual(new DateTime(2013, 08, 01), bt.StartDate);
            Assert.AreEqual(BTStatus.Planned|BTStatus.Modified, bt.Status);
        }

        [Test] public void PostPlanBT_PlannedModifiedBT_PlanTheSameBT()
        { 
            //Arrange
            var controllerContext = new Mock<ControllerContext>(); 
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur"); 
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault(); 
            MvcApplication.JSDatePattern = "dd.mm.yyyy"; 
            //Act
            var result = controller.Plan(bt, "RAAA4"); 
            //Assert 
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status); 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once); 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName); 
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA4", ((ViewResult)result).ViewBag.SelectedDepartment); 
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model); 
            Assert.IsNull(bt.RejectComment);
            Assert.AreEqual("Kyiv - Krakow", bt.Flights); 
            Assert.AreEqual(1, bt.OldLocationID);
            Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        }


        [Test]
        public void PostPlanBT_PlannedModifiedBT_LocationChanged()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 20, EmployeeID = 2, LocationID = 2 };
            var result = controller.Plan(bTrip, "RAAA4");

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bTrip), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTADM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("RAAA4", ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsNull(bTrip.RejectComment);
            Assert.AreEqual("Kyiv - Krakow", bTrip.Flights);
            Assert.AreEqual(1, bTrip.OldLocationID);
            Assert.AreEqual("LDF", bTrip.OldLocationTitle);
            Assert.AreEqual(new DateTime(2013, 09, 01), bTrip.OldStartDate);
            Assert.AreEqual(bTrip.LastCRUDedBy, "cbur");

        }


        [Test]
        public void PostPlanBT_PlannedBTConcurrency_JsonErrorResult()
        {
            //Arrange
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveBusinessTrip(bt)).Throws(new DbUpdateConcurrencyException());
            string selectedDep = "RAAA1";

            //Act
            var result = controller.Plan(bt, selectedDep);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;           
            
            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
             mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }


        [Test]
        public void PostPlanBT_StartandEndDatesOverlayStartandEndDatesOfanotherBTSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 100,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual( "BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBT_StartandEndDatesOverlayStartandEndDatesOfanotherBTAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "SSS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBT_StartDateOverlayEndDateOfAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 27),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }
        

        [Test]
        public void PostPlanBT_StartDatesOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 23),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLoo", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
   
            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
          }


        [Test]
        public void PostPlanBT_EndDateOverlayDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Confirmed BT on 27.12.2014 
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 27),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBT_BTDatesOverlaysAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has BT 21.12.2014 - 22.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 18),
                EndDate = new DateTime(2014, 12, 24),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBT_BTDatesOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBT_BTStartDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBT_BTEndDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 03),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
          
        }


        [Test]
        public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


          [Test]
        public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }
           

        [Test]
        public void PostPlanBT_BTStartDateOverlaysEndAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 13),
                EndDate = new DateTime(2014, 12, 14),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());

        }


        [Test]
        public void PostPlanBT_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 04),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_AnotherBTDatesIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBT_AnotherBTEndDateIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_AnotherBTEndDateSameasEndDateBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTDatesOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTStartDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTEndDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 03),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

           
            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }
        

        [Test]
        public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTStartDateOverlaysEndAnotherBTOfEmployeeSameLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 13),
                EndDate = new DateTime(2014, 12, 14),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);

        }
        

        [Test]
        public void PostPlanBT_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 04),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_AnotherBTDatesIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBT_AnotherBTEndDateIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_EndDateOverlayStartDateOfAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
               // BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 20),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_EndDateOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 20),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "LLL", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }


        [Test]
        public void PostPlanBT_StartDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 25),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBT_EndDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12,18 ),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBT_StartDateOverlayAnotherBTTheAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 25),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBT_EndDateOverlayStartDateAnotherBTAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 18),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
           // string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
          
        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTEndDateAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }


        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTStartDateAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

       
        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTEndDateSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTstartDateSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
         
            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTPlanTheSameBTAnotherLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBT_BTPlanTheSameBTSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBT_BTProlongsOneDayTheSameBT_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                StartDate = new DateTime(2014, 11, 06),
                EndDate = new DateTime(2014, 11, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
          
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }
       
        #endregion

        
        #region PlanPostForEditPlanned
       
        [Test]
        public void PostPlanBTEdit_EndDateOverlayDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Confirmed BT on 27.12.2014 
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 2,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 27),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_BTDatesOverlaysAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has BT 21.12.2014 - 22.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
               // BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 18),
                EndDate = new DateTime(2014, 12, 24),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTDatesOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTStartDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 03),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());

        }


        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_BTStartDateOverlaysEndAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 13),
                EndDate = new DateTime(2014, 12, 14),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert 
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());

        }


        [Test]
        public void PostPlanBTEdit_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 04),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_AnotherBTDatesIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_AnotherBTEndDateIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_AnotherBTEndDateSameasEndDateBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_BTDatesOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_BTStartDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
              //  BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 03),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 101,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };


            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 13),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_BTStartDateOverlaysEndAnotherBTOfEmployeeSameLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 13),
                EndDate = new DateTime(2014, 12, 14),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);

        }


        [Test]
        public void PostPlanBTEdit_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 04),
                EndDate = new DateTime(2014, 12, 12),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_AnotherBTDatesIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 01),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_AnotherBTEndDateIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 06),
                EndDate = new DateTime(2014, 12, 15),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_EndDateOverlayStartDateOfAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            //BusinessTrip bt = new BusinessTrip
            //{
            //    BusinessTripID = 28,
            //    StartDate = new DateTime(2014, 12, 20),
            //    EndDate = new DateTime(2014, 12, 21),
            //    OrderStartDate = new DateTime(2014, 12, 21),
            //    OrderEndDate = new DateTime(2014, 12, 28),
            //    DaysInBtForOrder = 27,
            //    Status = BTStatus.Planned,
            //    EmployeeID = 7,
            //    LocationID = 1,
            //    Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
            //    Comment = "2 employee planned and rejected(with comment)",
            //    Manager = "xtwe",
            //    Purpose = "meeting",
            //    UnitID = 1,
            //    Unit = new Unit()
            //};

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 28).FirstOrDefault();
           

            //Act
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 20),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };
          
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bTrip.Status);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_EndDateOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 20),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 3,
                Location = new Location { LocationID = 3, Title = "LLL", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }


        [Test]
        public void PostPlanBTEdit_StartDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 2,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 25),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_EndDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 18),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartDateOverlayAnotherBTTheAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 2,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 25),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }


        [Test]
        public void PostPlanBTEdit_EndDateOverlayAnotherBTAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 18),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
           // string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned|BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
           // Assert.AreEqual("BT with same dates is already planned for this user. "
                                     // + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned |BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }

        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTEndDateAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified , bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }


        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTStartDateAnotherLocation_SavedBT()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }


        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTEndDateSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 1,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayOverlayAnotherBTstartDateSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 21),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_BTPlanTheSameBTAnotherLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same dates is already planned for this user. "
                                      + "Please change \'From\' or \'To\'", data);
        }

        [Test]
        public void PostPlanBTedit_BTPlanTheSameBTSameLocation_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 11,
                StartDate = new DateTime(2014, 12, 03),
                EndDate = new DateTime(2014, 12, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }

        [Test]
        public void PostPlanBTEdit_BTProlongsOneDayTheSameBT_JsonResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 11,
                StartDate = new DateTime(2014, 11, 06),
                EndDate = new DateTime(2014, 11, 06),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartandEndDatesOverlayStartandEndDatesOfanotherBTSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
              //  BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartandEndDatesOverlayStartandEndDatesOfanotherBTAnotherLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                //BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 21),
                EndDate = new DateTime(2014, 12, 22),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "SSS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                      + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartDateOverlayEndDateOfAnotherBTTheSameLocation_JsonErrorResult()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 2,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 27),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 1,
                Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Planned, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("BT with same parameters already planned for this user. "
                                 + "Please change \'From\' or \'To\' or \'Location\'.", data);
        }


        [Test]
        public void PostPlanBTEdit_StartDatesOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip
            {
                BusinessTripID = 28,
                StartDate = new DateTime(2014, 12, 22),
                EndDate = new DateTime(2014, 12, 23),
                OrderStartDate = new DateTime(2014, 12, 21),
                OrderEndDate = new DateTime(2014, 12, 28),
                DaysInBtForOrder = 27,
                Status = BTStatus.Planned,
                EmployeeID = 7,
                LocationID = 12,
                Location = new Location { LocationID = 12, Title = "LLoo", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
                Comment = "2 employee planned and rejected(with comment)",
                Manager = "xtwe",
                Purpose = "meeting",
                UnitID = 1,
                Unit = new Unit()
            };

            //Act
            var result = controller.Plan(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual(typeof(ViewResult), result.GetType());
        }
        //[Test]
        //public void PostPlanBTEdit_EndDateOverlayDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Confirmed BT on 27.12.2014 
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 28,
        //        StartDate = new DateTime(2014, 12, 22),
        //        EndDate = new DateTime(2014, 12, 27),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTDatesOverlaysAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has BT 21.12.2014 - 22.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 28,
        //        StartDate = new DateTime(2014, 12, 18),
        //        EndDate = new DateTime(2014, 12, 24),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTDatesOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTStartDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 03),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTEndDateOverlaysStartAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 03),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert 
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());

        //}


        //[Test]
        //public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };


        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTEndDateOverlaysEndDateAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_BTStartDateOverlaysEndAnotherBTOfEmployeeAnotherLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 13),
        //        EndDate = new DateTime(2014, 12, 14),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert 
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());

        //}


        //[Test]
        //public void PostPlanBTEdit_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 04),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBTEdit_AnotherBTDatesIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 15),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBTEdit_AnotherBTEndDateIncludedIntoBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 15),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBTEdit_AnotherBTEndDateSameasEndDateBTOfEmployeeAnotherLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBTEdit_BTDatesOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBTedit_BTStartDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        BusinessTripID = 11,
        //        StartDate = new DateTime(2014, 12, 03),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTEndDateOverlaysStartAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 03),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };


        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTAndIncludeStartDateOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };


        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTEndDateOverlaysEndDateAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 13),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "llLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTStartDateOverlaysEndAnotherBTOfEmployeeSameLocation_SavedBT()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 13),
        //        EndDate = new DateTime(2014, 12, 14),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);

        //}


        //[Test]
        //public void PostPlanBT_BTDatesIncludedIntoDatesOfAnotherBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 04),
        //        EndDate = new DateTime(2014, 12, 12),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBT_AnotherBTDatesIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 01),
        //        EndDate = new DateTime(2014, 12, 15),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBT_AnotherBTEndDateIncludedIntoBTOfEmployeeSameLocation_JsonErrorResult()//Employee already has Planned BT 03.12.2014 - 13.12.2014
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 06),
        //        EndDate = new DateTime(2014, 12, 15),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBT_EndDateOverlayStartDateOfAnotherBTTheSameLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        // BusinessTripID = 101,
        //        StartDate = new DateTime(2014, 12, 20),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_EndDateOverlayEndDateOfAnotherBTAnotherLocation_BtWasSaved()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 20),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 3,
        //        Location = new Location { LocationID = 3, Title = "LLL", Address = "Kyiv, LLLL St.", BusinessTrips = new List<BusinessTrip>(), CountryID = 2 },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());
        //}


        //[Test]
        //public void PostPlanBT_StartDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 21),
        //        EndDate = new DateTime(2014, 12, 25),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBT_EndDateOverlayAnotherBTTheSameLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 18),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                              + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBT_StartDateOverlayAnotherBTTheAnotherLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 21),
        //        EndDate = new DateTime(2014, 12, 25),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLS", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}


        //[Test]
        //public void PostPlanBT_EndDateOverlayAnotherBTAnotherLocation_JsonErrorResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 18),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTAnotherLocation_SavedBT()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 22),
        //        EndDate = new DateTime(2014, 12, 22),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());
        //}

        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTEndDateAnotherLocation_SavedBT()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 22),
        //        EndDate = new DateTime(2014, 12, 22),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());
        //}


        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTStartDateAnotherLocation_SavedBT()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 21),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(typeof(ViewResult), result.GetType());
        //}


        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTEndDateSameLocation_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 22),
        //        EndDate = new DateTime(2014, 12, 22),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                         + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}


        //[Test]
        //public void PostPlanBT_BTProlongsOneDayOverlayAnotherBTstartDateSameLocation_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 21),
        //        EndDate = new DateTime(2014, 12, 21),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);
        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                         + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTPlanTheSameBTAnotherLocation_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 03),
        //        EndDate = new DateTime(2014, 12, 06),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same dates is already planned for this user. "
        //                              + "Please change \'From\' or \'To\'", data);
        //}

        //[Test]
        //public void PostPlanBT_BTPlanTheSameBTSameLocation_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 12, 03),
        //        EndDate = new DateTime(2014, 12, 06),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 1,
        //        Location = new Location { LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                         + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}

        //[Test]
        //public void PostPlanBT_BTProlongsOneDayTheSameBT_JsonResult()
        //{
        //    //Arrange
        //    BusinessTrip bt = new BusinessTrip
        //    {
        //        StartDate = new DateTime(2014, 11, 06),
        //        EndDate = new DateTime(2014, 11, 06),
        //        OrderStartDate = new DateTime(2014, 12, 21),
        //        OrderEndDate = new DateTime(2014, 12, 28),
        //        DaysInBtForOrder = 27,
        //        Status = BTStatus.Planned,
        //        EmployeeID = 7,
        //        LocationID = 12,
        //        Location = new Location { LocationID = 12, Title = "LLLDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>() },
        //        Comment = "2 employee planned and rejected(with comment)",
        //        Manager = "xtwe",
        //        Purpose = "meeting",
        //        UnitID = 1,
        //        Unit = new Unit()
        //    };

        //    //Act
        //    var result = controller.Plan(bt);

        //    string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

        //    //Assert   
        //    Assert.AreEqual(BTStatus.Planned, bt.Status);
        //    mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
        //    Assert.IsInstanceOf(typeof(JsonResult), result);
        //    Assert.AreEqual(typeof(JsonResult), result.GetType());
        //    Assert.AreEqual("BT with same parameters already planned for this user. "
        //                         + "Please change \'From\' or \'To\' or \'Location\'.", data);
        //}
        


         #endregion
        
        #region EditPlannedBT
        [Test]
        public void Get_EditPlannedBT_ExistingBusinessTripID_PlannedModified_EditPlannedBTView()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = (ViewResult)controller.EditPlannedBT(12);
            BusinessTripViewModel resultModel = (BusinessTripViewModel)result.Model;

            // Assert
            Assert.AreEqual("EditPlannedBT", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), resultModel);
            Assert.AreEqual(12, resultModel.BusinessTripID);
            Assert.IsTrue(resultModel.Status.HasFlag(BTStatus.Planned));
        }

        [Test]
        public void Get_EditPlannedBT_ExistingBusinessTripID_Planned_EditPlannedBTView()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();

            //Act
            var result = (ViewResult)controller.EditPlannedBT(12);
            BusinessTripViewModel resultModel = (BusinessTripViewModel)result.Model;
            // Assert
            Assert.AreEqual("EditPlannedBT", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), resultModel);
            Assert.AreEqual(12, resultModel.BusinessTripID);
            Assert.IsTrue(resultModel.Status.HasFlag(BTStatus.Planned));
        }

        [Test]
        public void Get_EditPlannedBT_ExistingBusinessTripID_NotPlanned_HttpNotFound()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            var result = controller.EditPlannedBT(110);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        [Test]
        public void Get_EditPlannedBT_NotExistingBusinessTripID_HttpNotFound()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();

            //Act
            var result = controller.EditPlannedBT(2);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }

        #endregion

        #region ProcessCommands
        //[Test]
        //public void ProcessCommand_EditPlannedBTWithID_RedirectToEditPlannedBTMethod()
        //{
        //    //Arrange
        //    int id = 12;
        //    string commandName = "EditPlannedBT";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, id, commandName, null);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("EditPlannedBT", result.RouteValues["action"]);
        //    Assert.AreEqual(12, result.RouteValues["id"]);
        //    Assert.AreEqual(null, result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_EditRegisteredBTWithID_RedirectToEditRegisteredBTMethod()
        //{
        //    //Arrange
        //    int id = 2;
        //    string commandName = "EditRegisteredBT";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, id, commandName, null);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("EditRegisteredBT", result.RouteValues["action"]);
        //    Assert.AreEqual(2, result.RouteValues["id"]);
        //    Assert.AreEqual(null, result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_EditConfirmedBTWithID_RedirectToEditConfirmedBTMethod()
        //{
        //    //Arrange
        //    int id = 14;
        //    string commandName = "EditConfirmedBT";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, id, commandName, null);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("EditConfirmedBT", result.RouteValues["action"]);
        //    Assert.AreEqual(14, result.RouteValues["id"]);
        //    Assert.AreEqual(null, result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_Register_RedirectToRegisterPlannedBTsMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Register";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_Confirm_RedirectToConfirmPlannedBTsMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Confirm";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);

        //}

        //[Test]
        //public void ProcessCommand_Plan__RedirectToPlanMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Plan ";
        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.FirstOrDefault();

        //    //Act
        //    var result = controller.ProcessCommand(businessTrip, 0, commandName, null);

        //    // Assert

        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");

        //}

        //[Test]
        //public void ProcessCommand_Register__RedirectToRegisterPlannedBTMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Register ";
        //    BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

        //    //Act
        //    var result = (ViewResult)controller.ProcessCommand(bt, 0, commandName);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");
        //    Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        //}

        //[Test]
        //public void ProcessCommand_Confirm__RedirectToConfirmPlannedBTMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Confirm ";
        //    BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();

        //    //Act
        //    var result = controller.ProcessCommand(bt, 0, commandName);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");
        //    Assert.AreEqual(bt.LastCRUDedBy, "cbur");
        //}

        //[Test]
        //public void ProcessCommand_Confirm___ConfirmRegisteredBTsMethod()
        //{
        //    //Arrange
        //    string commandName = "Confirm  ";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");

        //}

        //[Test]
        //public void ProcessCommand_Replan_ReplanRegisteredBTsMethod()
        //{
        //    //Arrange
        //    string commandName = "Replan";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(((ViewResult)result).ViewName, "TableViewBTADM");

        //}

        //[Test]
        //public void ProcessCommand_Cancel_CancelRegisteredBTsMethod()
        //{
        //    //Arrange
        //    string commandName = "Cancel";
        //    string[] selectedPlannedBTs = { "10", "11", "12" };

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 0, commandName, selectedPlannedBTs);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_Cancel__CancelConfirmedBTsMethod()
        //{
        //    //Arrange
        //    var controllerContext = new Mock<ControllerContext>();
        //    controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
        //    controller.ControllerContext = controllerContext.Object;
        //    string commandName = "Cancel ";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 5, commandName);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //}

        //[Test]
        //public void ProcessCommand_Default_RedirectToIndexMethod()
        //{
        //    //Arrange
        //    string commandName = "";

        //    //Act
        //    var result = (RedirectToRouteResult)controller.ProcessCommand(null, 0, commandName, null);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        //    Assert.IsFalse(result.Permanent);
        //    Assert.AreEqual("ADMView", result.RouteValues["action"]);
        //    Assert.AreEqual(null, result.RouteValues["id"]);
        //    Assert.AreEqual("Home", result.RouteValues["controller"]);
        //}
        #endregion

        #region RewriteBTsPropsAfterPlanningFromRepository

        [Test]
        public void RewriteBTsPropsAfterPlanningFromRepository_PlannedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 1, LocationID = 2, StartDate = new DateTime(2014, 11, 25), EndDate = new DateTime(2015, 10, 24), Comment = "123456" };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsAfterPlanningFromRepository(bTrip);
   
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(2, result.LocationID);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("123456", bTrip.Comment);
            Assert.AreEqual(BTStatus.Planned, bTrip.Status);
        }

        [Test]
        public void RewriteBTsPropsAfterPlanningFromRepository_PlannedModifiedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 20, LocationID = 2, StartDate = new DateTime(2014, 11, 25), EndDate = new DateTime(2015, 10, 24), Comment = "123456" };
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == bTrip.BusinessTripID).FirstOrDefault();
       
            //Act
            BusinessTrip result = controller.RewriteBTsPropsAfterPlanningFromRepository(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.BusinessTripID);
            Assert.AreEqual(1, result.OldLocationID);
            Assert.AreEqual("visa expired", result.RejectComment);
            Assert.AreEqual("LDF", result.OldLocationTitle);
            Assert.AreEqual(2, result.LocationID);
            Assert.AreEqual("123456", result.Comment);
            Assert.AreEqual(BTStatus.Modified | BTStatus.Planned, result.Status);
        }
        #endregion

        #endregion

        #region BTM
        #region GetBusinessTripBTM

        [Test]
        public void GetBusinessTripBTM_SearchStringNull_SearchStringNull()
        {
            // Arrange

            // Act
            string searchString = null;
            var view = controller.GetBusinessTripBTM(searchString);
            SelectList SelectedList = ((ViewResult)view).ViewBag.DepartmentsList;
            
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SearchString);

        }

        

        [Test]
        public void GetBusinessTripBTM_SearchStringEmpty_SearchStringEmpty()
        {
            // Arrange

            // Act
            string searchString = "";
            var view = controller.GetBusinessTripBTM(searchString);
            SelectList SelectedList = ((ViewResult)view).ViewBag.DepartmentsList;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewBag.SearchString);

        }
        [Test]
        public void GetBusinessTripBTM_SearchStingAA_SearchStingAA()
        {
            // Arrange

            // Act
            string selectedDepartment = "AA";
            var view = controller.GetBusinessTripBTM(selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("AA", ((ViewResult)view).ViewBag.SearchString);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("AA", ((ViewResult)view).ViewBag.SearchString);

        }

        #endregion

        #region GEtBusinessTripDataBTM

        [Test]
        public void GEtBusinessTripDataBTM_SearchStringEmpty_ViewAllEmployees()
        {
            // Arrange
              mock = new Mock<IRepository>();
            // Act
            string searchString = "";
            var view = controller.GetBusinessTripDataBTM(searchString);
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataBTM(searchString).Model;
            Employee[] employeeList = result.ToArray();

            // Assert

            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.AreEqual(10, employeeList.Length);
            Assert.AreEqual(8, employeeList[0].EmployeeID);
            Assert.AreEqual(17, employeeList[5].EmployeeID);

        }

        [Test]
        public void GEtBusinessTripdataBTM_SearchStringAND_ViewSelectedEmployees()
        {
            // Arrange

            // Act
            string searchString = "ANO";
            var view = controller.GetBusinessTripDataBTM(searchString);
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetBusinessTripDataBTM(searchString).Model;
            Employee[] employeeList = result.ToArray();

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.AreEqual(employeeList.Length, 1);
            Assert.AreEqual(employeeList[0].EmployeeID, 7);
        }

        #endregion

        #region BTMArrangeBT

        [Test]
        public void BTMArrangeBT_NotExistingBT_HttpNotFound()
        {
            //Arrange
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            var view = controller.BTMArrangeBT(100);

            // Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void BTMArrangeBT_ExistingPlannedBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            var view = controller.BTMArrangeBT(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(1, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
        }

        [Test]
        public void BTMArrangeBT_ExistingRegisteredBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            var view = controller.BTMArrangeBT(2);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(2, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNull(businessTrip.OrderEndDate);
            Assert.AreEqual(BTStatus.Registered, businessTrip.Status);
        }

        [Test]
        public void BTMArrangeBT_ExistingConfirmedBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            var view = controller.BTMArrangeBT(3);
            int daysInBT = (businessTrip.EndDate - businessTrip.StartDate).Days + 1;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNotNull(businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.EndDate.AddDays(1), businessTrip.OrderEndDate);
            Assert.AreEqual(daysInBT + 2, businessTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void BTMArrangeBT_ExistingConfirmedBTOrderDatesNull_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            businessTrip.OrderStartDate = null;
            businessTrip.OrderEndDate = null;
            var view = controller.BTMArrangeBT(3);
            int daysInBT = (businessTrip.EndDate - businessTrip.StartDate).Days + 1;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNotNull(businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.StartDate.AddDays(-1), businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.EndDate.AddDays(1), businessTrip.OrderEndDate);
            Assert.AreEqual(daysInBT + 2, businessTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void BTMArrangeBT_ExistingConfirmedBTOrderStartDateNull_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            businessTrip.OrderStartDate = null;
            var view = controller.BTMArrangeBT(3);
            int daysInBT = (businessTrip.EndDate - businessTrip.StartDate).Days + 1;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNull(businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.EndDate.AddDays(1), businessTrip.OrderEndDate);
            Assert.AreEqual(daysInBT + 2, businessTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
        }

        [Test]
        public void BTMArrangeBT_ExistingConfirmedModifiedBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            var view = controller.BTMArrangeBT(4);
            int daysInBT = (businessTrip.EndDate - businessTrip.StartDate).Days + 1;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(4, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);
            Assert.IsNotNull(businessTrip.OrderStartDate);
            Assert.AreEqual(businessTrip.EndDate.AddDays(1), businessTrip.OrderEndDate);
            Assert.AreEqual(daysInBT + 2, businessTrip.DaysInBtForOrder);
            Assert.AreEqual(BTStatus.Confirmed | BTStatus.Modified, businessTrip.Status);
        }

        #endregion

        #region SaveArrangeBT

        [Test]
        public void SaveArrangedBT_ValidRegisteredBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 2 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Registered, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsTrue(bt.HabitationConfirmed);

        }

        [Test]
        public void SaveArrangedBT_ValidRegisteredModifiedBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 13 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt,"");

            //Assert   
            Assert.AreEqual(BTStatus.Registered, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void SaveArrangedBT_ValidConfirmededModifiedBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 14 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Confirmed, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsTrue(bt.HabitationConfirmed);

        }

        [Test]
        public void SaveArrangedBT_ValidConfirmededBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 3 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Confirmed, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsTrue(bt.HabitationConfirmed);
        }

        [Test]
        public void SaveArrangedBT_PlannedModifiedBT_BTSaved()
        {
            //Arrange
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 12 select b).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            //Act
            var result = controller.SaveArrangedBT(bt);

            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsTrue(bt.HabitationConfirmed);
            Assert.IsTrue(bt.HabitationConfirmed);
        }

        [Test]
        public void SaveArrangedBT_NotValidBT_BTSaved()
        {
            //Arrange
            controller.ModelState.AddModelError("error", "error");
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 12 select b).FirstOrDefault();
            
            //Act
            var result = controller.SaveArrangedBT(bt);
            
            //Assert   
            Assert.AreEqual(BTStatus.Planned | BTStatus.Modified, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("BTMArrangeBT", ((ViewResult)result).ViewName);
            Assert.AreEqual("krakow", bt.Habitation);
            Assert.IsTrue(bt.HabitationConfirmed);

        }

        [Test]
        public void SaveArrangedBT_ValidRegisteredBTConcurrency_JsonErrorResult()
        {
            //Arrange
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bt = (from b in mock.Object.BusinessTrips where b.BusinessTripID == 2 select b).FirstOrDefault();

            //Act
            JsonResult result = (JsonResult)controller.SaveArrangedBT(bt);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert   
            Assert.AreEqual(BTStatus.Registered, bt.Status);
            mock.Verify(m => m.SaveBusinessTrip(bt), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMUpdatesConfirmedOrConfirmedModifiedToACC))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);

        }

        #endregion


        #region  ReportConfirmedBTs

        [Test]
        public void ReportConfirmedBTs_SelectedTwoConfirmedBTs_ChangedStatus()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "4", "5" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs,"AS");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 4
                && b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 5
                && b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("AS", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_Null_RedirectToBTMView()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = null;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_BadArrayOfString_CannotParseIDs()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "4gt" };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_BadArrayOfString_ParsedOnlyOneID()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "4", "4gr" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>()), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_NotExistingBT_RedirectToBTMView()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "100" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            Assert.IsNull(bTrip1);
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void ReportConfirmedBTs_RegisteredBTsInputs_RedirectToBTMView()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "2" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(bTrip1), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(BTStatus.Registered, bTrip1.Status);
        }

        [Test]
        public void ReportConfirmedBTs_VisaIsNull_RedirectToBTMView()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "5" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.IsNull(bTrip1.BTof.Visa);
        }

        [Test]
        public void ReportConfirmedBTs_VisaIsNotNull_DaysAndEntriesUsedInBTAdded()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "5" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();
            Visa visa = new Visa { VisaOf = bTrip1.BTof, VisaType = "C07", Days = 180, DaysUsedInBT = 0, EmployeeID = bTrip1.EmployeeID, Entries = 2, EntriesUsedInBT = 0, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip1.BTof.Visa = visa;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs,"A");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("A", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(20, bTrip1.BTof.Visa.DaysUsedInBT);
            Assert.AreEqual(1, bTrip1.BTof.Visa.EntriesUsedInBT);
        }

        [Test]
        public void ReportConfirmedBTs_TwoBtsAreAdded_DaysAndEntriesUsedInBTAddedTwice()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "3", "4" };
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(e => e.EmployeeID == bTrip1.EmployeeID).FirstOrDefault();
            Visa visa = new Visa { VisaOf = employee, VisaType = "C07", Days = 180, DaysUsedInBT = 0, EmployeeID = employee.EmployeeID, Entries = 2, EntriesUsedInBT = 0, StartDate = new DateTime(2013, 11, 01), DueDate = new DateTime(2014, 02, 01) };
            bTrip1.BTof.Visa = visa;
            bTrip2.BTof.Visa = visa;
            employee.Visa = visa;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act
            var result = controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs,"D");

            // Assert
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 3
                && b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 4
                && b.Status == (BTStatus.Confirmed | BTStatus.Reported))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("D", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual(23, employee.Visa.DaysUsedInBT);
            Assert.AreEqual(2, employee.Visa.EntriesUsedInBT);
        }

        [Test]
        public void ReportConfirmedBTs_SelectedTwoConfirmedBTsConcurrency_JsonErrorResult()
        {
            // Arrange
            string[] selectedIDsOfConfirmedBTs = { "4", "5" };
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip bTrip1 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            BusinessTrip bTrip2 = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            // Act
            JsonResult result = (JsonResult)controller.ReportConfirmedBTs(selectedIDsOfConfirmedBTs);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            Assert.IsInstanceOf(typeof(JsonResult), result);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 4)), Times.Once);
            mock.Verify(m => m.SaveBusinessTrip(It.Is<BusinessTrip>(b => b.BusinessTripID == 5)), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToACC))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMReportsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region  Reject_BT_BTM

        [Test]
        public void Reject_BT_BTM_Default_NotExistingBT()
        {
            //Arrange

            // Act
            var view = controller.Reject_BT_BTM(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void Reject_BT_BTM_EmptyString_ExistingBT()
        {
            //Arrange
            string searchString = "";

            // Act
            var view = controller.Reject_BT_BTM(3, searchString);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
        }

        [Test]
        public void Reject_BT_BTM_aaAndDefaultJsondata_ExistingBT()
        {
            //Arrange
            string searchString = "aa";
            BusinessTrip businessTripBeforeCall = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion };

            // Act
            var view = controller.Reject_BT_BTM(id: 3, searchString: searchString);
            BusinessTrip businessTripAfterCall = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTripAfterCall.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
            Assert.AreEqual(businessTripBeforeCall.RowVersion, businessTripAfterCall.RowVersion);
            Assert.AreEqual(rowVersion, businessTripAfterCall.RowVersion);
        }

        [Test]
        public void Reject_BT_BTM_aaAndEmptyJsondata_ExistingBT()
        {
            //Arrange
            string searchString = "aa";
            string json = "";
            BusinessTrip businessTripBeforeCall = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion};

            // Act
            var view = controller.Reject_BT_BTM(id: 3, jsonRowVersionData: json, searchString: searchString);
            BusinessTrip businessTripAfterCall = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTripAfterCall.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
            Assert.AreEqual(businessTripBeforeCall.RowVersion, businessTripAfterCall.RowVersion);
        }

        [Test]
        public void Reject_BT_BTM_JsonDataNotEmpty_ExistingBT()
        {
            //Arrange
            string searchString = "AA";
            string jsonData = JsonConvert.SerializeObject(rowVersion);

            // Act
            var view = controller.Reject_BT_BTM(3, jsonData, searchString);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(searchString, ((ViewResult)view).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), (view as ViewResult).Model);
            Assert.AreEqual(rowVersion, ((view as ViewResult).Model as BusinessTripViewModel).RowVersion);
        }

        #endregion

        #region  Reject_BT_BTM_Confirm
        [Test]
        public void Reject_BT_BTM_Confirm_Default_NotExistingBT()
        {
            //Arrange
            BusinessTrip bt = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            // Act
            var view = controller.Reject_BT_BTM_Confirm(bt);

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(bt);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_EmptyStringComment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            businessTrip.RejectComment = "";

            // Act
            var view = controller.Reject_BT_BTM_Confirm(businessTrip, null);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("Reject_BT_BTM", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
            Assert.AreEqual(null, ((ViewResult)view).ViewBag.SearchString);
            Assert.AreEqual("", businessTrip.RejectComment);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Never);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_aSearchAndDefaultComment_CannotReject()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();

            // Act
            var view = controller.Reject_BT_BTM_Confirm(businessTrip, "a");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("Reject_BT_BTM", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.AreEqual(BTStatus.Confirmed, businessTrip.Status);
            Assert.AreEqual("a", ((ViewResult)view).ViewBag.SearchString);
            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Never);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_ExistingRegisteredBTAndValidComment_CanReject()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            businessTrip.RejectComment = "Visa expired";

            // Act
            var result = controller.Reject_BT_BTM_Confirm(businessTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("Visa expired", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Once);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_ExistingRegisteredModifiedBTAndValidComment_CanReject()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 13).FirstOrDefault();
            businessTrip.RejectComment = "Visa expired";

            // Act
            var result = controller.Reject_BT_BTM_Confirm(businessTrip, searchString: "D");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("D", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Never);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Never);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Once);

            Assert.AreEqual("Visa expired", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_ExistingConfirmedBTAndValidComment_CanReject()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            businessTrip.RejectComment = "Visa expired";

            // Act
            var result = controller.Reject_BT_BTM_Confirm(businessTrip, "d");

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("d", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Never);

            Assert.AreEqual("Visa expired", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        [Test]
        public void Reject_BT_BTM_Confirm_ExistingConfirmedModifiedBTAndValidComment_CanReject()
        {
            //Arrange
            MvcApplication.JSDatePattern = "dd.mm.yyyy";
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            businessTrip.RejectComment = "Visa expired";

            // Act
            var result = controller.Reject_BT_BTM_Confirm(businessTrip);

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", ((ViewResult)result).ViewName);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);

            mock.Verify(m => m.SaveBusinessTrip(businessTrip), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToADM))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToEMP))), Times.Once);
            messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsConfirmedOrConfirmedModifiedToResponsibleInLocation))), Times.Once);
            //messengerMock.Verify(m => m.Notify(It.Is<IMessage>(msg => msg.messageType.Equals(MessageType.BTMRejectsRegisteredOrRegisteredModifiedToACC))), Times.Never);

            Assert.AreEqual("Visa expired", businessTrip.RejectComment);
            Assert.AreEqual((BTStatus.Planned | BTStatus.Modified), businessTrip.Status);
        }

        [Test]
        public void Reject_BT_BTM_ConfirmConcurrency_JsonErrorResult()
        {
            //Arrange
            mock.Setup(m => m.SaveBusinessTrip(It.IsAny<BusinessTrip>())).Throws(new DbUpdateConcurrencyException());
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            businessTrip.RejectComment = "reject";

            // Act
            JsonResult result = (JsonResult)controller.Reject_BT_BTM_Confirm(businessTrip);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert 
            Assert.IsInstanceOf(typeof(JsonResult), result);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region DeleteBTBTM

        [Test]
        public void DeleteBTBTM_NotExistingBT_HttpNotFound()
        {
            //Arrange

            // Act
            var view = controller.DeleteBTBTM(0);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 0).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);

        }
        [Test]
        public void DeleteBTBTM_BTNotCancelled_HttpNotFound()
        {
            //Arrange

            // Act
            var view = controller.DeleteBTBTM(1);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);

        }

        [Test]
        public void DeleteBTBTM_ExistingBT_ViewDeleteBTConfirmedBTM()
        {
            //Arrange

            // Act
            var view = controller.DeleteBTBTM(19);
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 19).FirstOrDefault();

            //Assert 
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(19, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);

        }
        #endregion

        #region DeleteBTBTMConfirmed()

        [Test]
        public void DeleteBTBTMConfirmed_BTisNotCancelled_View()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();

            //Act
            ViewResult result = (ViewResult)controller.DeleteBTBTMConfirmed(2);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(2), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", result.ViewName);
            Assert.AreEqual(controller.SearchBusinessTripDataBTM(mock.Object.Employees.ToList(), ""), result.Model);
        }

        [Test]
        public void DeleteBTBTMConfirmed_ExistingCancelledBT_View()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 19).FirstOrDefault();

            //Act
            ViewResult result = (ViewResult)controller.DeleteBTBTMConfirmed(19);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(19), Times.Once);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", result.ViewName);
            Assert.AreEqual(controller.SearchBusinessTripDataBTM(mock.Object.Employees.ToList(), ""), result.Model);
        }

        [Test]
        public void DeleteBTBTMConfirmed_NotExistingBusinessTripID_View()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1000).FirstOrDefault();

            //Act
            ViewResult result = (ViewResult)controller.DeleteBTBTMConfirmed(1000);

            // Assert
            mock.Verify(m => m.DeleteBusinessTrip(1000), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("TableViewBTM", result.ViewName);
            Assert.AreEqual(controller.SearchBusinessTripDataBTM(mock.Object.Employees.ToList(), ""), result.Model);
        }
        #endregion

        #region RewriteBTsPropsFromRepositoryBTM

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_RegisteredBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 2, LocationID = 1, StartDate = new DateTime(2014, 11, 01), EndDate = new DateTime(2014, 11, 10), OrderEndDate = new DateTime(2014, 11, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02),  EmployeeID = 2, Habitation = "Linkopin", HabitationConfirmed = false, Status = BTStatus.Registered };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual("LDF", result.Location.Title);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("Linkopin", result.Habitation);
            Assert.IsFalse(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(null, result.OrderEndDate);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_PlannedModifiedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 20).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 20, LocationID = 1, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 27), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified, EmployeeID = 2, Comment = "2 employee planned and rejected(with comment)", Manager = "xtwe", Purpose = "meeting", Habitation = "Kyiv", HabitationConfirmed = true };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.BusinessTripID);
            Assert.AreEqual(2, result.OldLocationID);
            Assert.AreEqual("LDL", result.OldLocationTitle);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual("LDF", result.Location.Title);
            Assert.AreEqual("Kyiv", result.Habitation);
            Assert.IsTrue(result.HabitationConfirmed);
            Assert.AreEqual(BTStatus.Modified | BTStatus.Planned, result.Status);
            Assert.AreEqual(new DateTime(2013, 08, 31), result.OrderStartDate);
            Assert.AreEqual(new DateTime(2013, 09, 28), result.OrderEndDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);
            Assert.AreEqual("visa expired", result.RejectComment);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_ConfirmedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 3, StartDate = new DateTime(2014, 10, 20), EndDate = new DateTime(2014, 10, 30), OrderStartDate = new DateTime(2013, 02, 01), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed, EmployeeID = 3, LocationID = 2, Habitation = "lodz", HabitationConfirmed = true, RowVersion = rowVersion };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(2, result.LocationID);
            Assert.AreEqual("LDL", result.Location.Title);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("lodz", result.Habitation);
            Assert.IsTrue(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(new DateTime(2013, 02, 01), result.OrderStartDate);
            Assert.AreEqual(null, result.OrderEndDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_ConfirmedModifiedBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 4).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 4, StartDate = new DateTime(2014, 10, 01), EndDate = new DateTime(2014, 10, 12), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Modified, EmployeeID = 3, LocationID = 2, OrderStartDate = new DateTime(2013, 02, 01), OrderEndDate = new DateTime(2013, 02, 01), DaysInBtForOrder = 1, RejectComment = "reject" };
 
            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(2, result.LocationID);
            Assert.AreEqual("LDL", result.Location.Title);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual(null, result.Habitation);
            Assert.IsFalse(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(new DateTime(2013, 02, 01), result.OrderStartDate);
            Assert.AreEqual(new DateTime(2013, 02, 01), result.OrderEndDate);
            Assert.AreEqual(1, result.DaysInBtForOrder);
            Assert.AreEqual("reject", result.RejectComment);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryBTM_CancelledBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 18).FirstOrDefault();
            businessTripFromRepository.RejectComment = "reject";
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 18, StartDate = new DateTime(2013, 09, 01), EndDate = new DateTime(2013, 09, 25), OrderStartDate = new DateTime(2014,01,10), OrderEndDate = new DateTime(2014,02,10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 4, LocationID = 1, Comment = "4 employee confirmed and cancelled", Manager = "xtwe", Purpose = "meeting" };
            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryBTM(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(18, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual("LDF", result.Location.Title);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual(null, result.Habitation);
            Assert.IsFalse(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(new DateTime(2014, 01, 10), result.OrderStartDate);
            Assert.AreEqual(new DateTime(2014, 02, 10), result.OrderEndDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);
            Assert.AreEqual("reject", result.RejectComment);
            Assert.AreEqual("visa expired", result.CancelComment);
       
        }

        #endregion

        #region RewriteBTsPropsFromRepositoryWhenReject

        [Test]
        public void RewriteBTsPropsFromRepositoryWhenReject_RegisteredBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 2, LocationID = 1, StartDate = new DateTime(2014, 11, 01), EndDate = new DateTime(2014, 11, 10), OrderEndDate = new DateTime(2014, 11, 10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), EmployeeID = 2, Habitation = "Linkopin", HabitationConfirmed = false, Status = BTStatus.Registered };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryWhenReject(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("krakow", result.Habitation);
            Assert.IsTrue(result.HabitationConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(null, result.OrderEndDate);
            Assert.AreEqual(null, result.OrderStartDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);
        }

        [Test]
        public void RewriteBTsPropsFromRepositoryWhenReject_NullPropsInBTModel_PropsRewrited()
        {
            //Arrange
            BusinessTrip businessTripFromRepository = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 2).FirstOrDefault();
            BusinessTrip bTrip = new BusinessTrip { BusinessTripID = 2, LocationID = 1, StartDate = new DateTime(2014, 11, 01), EndDate = new DateTime(2014, 11, 10), OrderStartDate = new DateTime(2012,10,10), LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), EmployeeID = 2, Status = BTStatus.Registered, FlightsConfirmed = true };

            //Act
            BusinessTrip result = controller.RewriteBTsPropsFromRepositoryWhenReject(bTrip);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.BusinessTripID);
            Assert.AreEqual(0, result.OldLocationID);
            Assert.AreEqual(1, result.LocationID);
            Assert.AreEqual(bTrip.StartDate, result.StartDate);
            Assert.AreEqual(bTrip.EndDate, result.EndDate);
            Assert.AreEqual("krakow", result.Habitation);
            Assert.IsTrue(result.HabitationConfirmed);
            Assert.IsFalse(result.FlightsConfirmed);
            Assert.AreEqual(new DateTime(2012, 12, 02), result.LastCRUDTimestamp);
            Assert.AreEqual(null, result.OrderEndDate);
            Assert.AreEqual(null, result.OrderStartDate);
            Assert.AreEqual(null, result.DaysInBtForOrder);

        }
        #endregion

         #endregion

        

        #region ADM,BTM,ACC

        #region ShowBTData

        [Test]
        public void ShowBTData_NotExistingBT_HttpNotFound()
        {
            //Arrange

            BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            var view = controller.ShowBTData(100);

            // Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }


        [Test]
        public void ShowBTData_ExistingBT_ViewBT()
        {
            //Arrange

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 1).FirstOrDefault();
            var view = controller.ShowBTData(1);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);

            Assert.AreEqual(1, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)view).Model);

        }

        [Test]
        public void ShowBTData_ExistingBusinessTripID_ShowBTDataView()
        {
            //Arrange
            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 12).FirstOrDefault();


            //Act
            var result = (ViewResult)controller.ShowBTData(12);
            BusinessTripViewModel resultModel = (BusinessTripViewModel)result.Model;

            // Assert
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsInstanceOf(typeof(BusinessTripViewModel), ((ViewResult)result).Model);
            Assert.AreEqual(12, resultModel.BusinessTripID);
        }

        [Test]
        public void ShowBTData_NotExistingBusinessTripID_HttpNotFound()
        {
            //Arrange

            //Act
            var result = controller.ShowBTData(1000);

            // Assert
            Assert.IsInstanceOf(typeof(HttpStatusCodeResult), result);
            Assert.IsTrue(404 == ((HttpStatusCodeResult)result).StatusCode);
        }
        #endregion

        //#region ShowBTsDataForEmployee

        //[Test]
        //public void Get_ShowBTsDataForEmployee_ExistingBusinessTripID_ShowBTDataView()
        //{
        //    //Arrange

        //    var data = mock.Object.Employees.Where(e => e.EmployeeID == 5).Select(e => e.FirstName + " " + e.LastName + " (" + e.EID + ")").FirstOrDefault();

        //    IEnumerable<BusinessTrip> businesstrips = mock.Object.BusinessTrips
        //        .Where(b => (b.EmployeeID == 5)
        //           && (b.Status == (BTStatus.Confirmed | BTStatus.Reported)))
        //        .OrderByDescending(b => b.StartDate);

        //    //Act
        //    var result = (ViewResult)controller.ShowBTsDataForEmployee(5);

        //    // Assert
        //    Assert.AreEqual("", result.ViewName);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.IsInstanceOf(typeof(IEnumerable<BusinessTrip>), result.Model);
        //    CollectionAssert.AreEqual(businesstrips, result.Model as IEnumerable<BusinessTrip>);
        //    Assert.AreEqual(data, result.ViewBag.EmployeeInformation);
        //}

        //[Test]
        //public void Get_ShowBTsDataForEmployee_NotExistingBusinessTripID_HttpNotFound()
        //{
        //    //Arrange
        //    var data = mock.Object.Employees.Where(e => e.EmployeeID == 1).Select(e => e.FirstName + " " + e.LastName + " (" + e.EID + ")").FirstOrDefault();

        //    //Act
        //    var result = (ViewResult)controller.ShowBTsDataForEmployee(1);

        //    // Assert
        //    Assert.AreEqual("NoLastBTs", result.ViewName);
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual(data, result.ViewBag.EmployeeInformation);
        //    Assert.IsNull(result.Model);
        //}

        //[Test]
        //public void ShowBTsDataForEmployee_ExistingReportedBusinessTrips_ShowBTDataView()
        //{
        //    //Arrange
        //    var data = mock.Object.Employees.Where(e => e.EmployeeID == 5).Select(e => e.FirstName + " " + e.LastName + " (" + e.EID + ")").FirstOrDefault();

        //    IEnumerable<BusinessTrip> businesstrips = mock.Object.BusinessTrips
        //        .Where(b => (b.EmployeeID == 5)
        //              && (b.Status == (BTStatus.Confirmed | BTStatus.Reported)) )
        //        .OrderByDescending(b => b.StartDate);

        //    //Act
        //    var result = controller.ShowBTsDataForEmployee(5);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual("", ((ViewResult)result).ViewName);
        //    Assert.AreEqual(data, ((ViewResult)result).ViewBag.EmployeeInformation);
        //    Assert.IsInstanceOf(typeof(IEnumerable<BusinessTrip>), ((ViewResult)result).Model);
        //    CollectionAssert.AreEqual(businesstrips, ((ViewResult)result).Model as IEnumerable<BusinessTrip>);
        //}

        //[Test]
        //public void ShowBTsDataForEmployee_NotExistingReportedBusinessTrips_NoLastBTView()
        //{
        //    //Arrange
        //    var data = mock.Object.Employees.Where(e => e.EmployeeID == 1).Select(e => e.FirstName + " " + e.LastName + " (" + e.EID + ")").FirstOrDefault();

        //    //Act
        //    var result = controller.ShowBTsDataForEmployee(1);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual("NoLastBTs", ((ViewResult)result).ViewName);
        //    Assert.AreEqual(data, ((ViewResult)result).ViewBag.EmployeeInformation);
        //    Assert.IsNull(((ViewResult)result).Model);
        //}

        //[Test]
        //public void ShowBTsDataForEmployee_NotExistingEmployeeID_NoLastBTView()
        //{
        //    //Arrange
        //    var data = mock.Object.Employees.Where(e => e.EmployeeID == 0).Select(e => e.FirstName + " " + e.LastName + " (" + e.EID + ")").FirstOrDefault();

        //    //Act
        //    var result = controller.ShowBTsDataForEmployee(0);
        //    IEnumerable<BusinessTrip> businesstrips = mock.Object.BusinessTrips
        //        .Where(b => (b.EmployeeID == 0)
        //           && (b.Status == (BTStatus.Confirmed | BTStatus.Reported)) )
        //        .OrderByDescending(b => b.StartDate);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), result);
        //    Assert.AreEqual("NoLastBTs", ((ViewResult)result).ViewName);
        //    Assert.AreEqual(null, ((ViewResult)result).ViewBag.EmployeeInformation);
        //    Assert.IsNull(((ViewResult)result).Model);
        //    Assert.AreEqual(0, businesstrips.Count());

        //}

        //#endregion

        #endregion

        #region Common

        [Test]
        public void AddLastCRUDDataToBT_BusinessTrip_CRUD_Data_Added()
        {
            //Arrange

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("cbur");
            controller.ControllerContext = controllerContext.Object;
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 11).FirstOrDefault();
            string oldLastCRUDedBy = businessTrip.LastCRUDedBy;
            DateTime oldLastCRUDTimestamp = businessTrip.LastCRUDTimestamp;

            //Act
            controller.AddLastCRUDDataToBT(businessTrip);

            //Assert        
            Assert.AreNotEqual(oldLastCRUDedBy, businessTrip.LastCRUDedBy);
            Assert.AreEqual(businessTrip.LastCRUDedBy, "cbur");
            Assert.Greater(businessTrip.LastCRUDTimestamp, oldLastCRUDTimestamp);
            Assert.LessOrEqual(businessTrip.LastCRUDTimestamp, DateTime.Now.ToLocalTimeAzure());
        }
        //TODO: add to every test method which uses AddLastCRUDDataToBT mock.Verify.AddLastCRUDDataToBT(m => m.(bt), Times.Once);

        #endregion

        #region BTM, ACC

        #region CountingDaysUsedInBT

        [Test]
        public void CountingDaysUsedInBT_ReportedBT_IncreasedDaysUsedInBT()
        {
            //Arrange

            BusinessTrip bTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 5).FirstOrDefault();

            //Act
            int result1 = controller.CountingDaysUsedInBT(bTrip);

            //Assert
            Assert.AreEqual(20, result1);
        }

        #endregion

        #endregion

    }
}