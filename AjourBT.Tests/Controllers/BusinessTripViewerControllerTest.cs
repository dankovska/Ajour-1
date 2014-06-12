using System;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using AjourBT.Controllers;
using AjourBT.Models;
using System.Web.Mvc;
using AjourBT.Domain.Infrastructure;
using System.Collections;
using AjourBT.Tests.MockRepository;
using ExcelLibrary.SpreadSheet;


namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class BusinessTripViewerControllerTest
    {
        Mock<IRepository> mock;
        Mock<IRepository> mock2;

        static List<Employee> employees2;
        static List<BusinessTrip> businessTrips2;
        static List<Location> locations2;
        static List<Department> departments2;

        [SetUp]
        public void SetUp()
        {

            mock = Mock_Repository.CreateMock();

        }

        public void SetUp2()
        {
            mock2 = new Mock<IRepository>();

            employees2 = new List<Employee>
            {
                 new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Andzzzz", DepartmentID = 1, EID = "andl", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
                 new Employee {EmployeeID = 2, FirstName = "And", LastName = "Zarose", DepartmentID = 2, EID = "kaaa", DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            };

            departments2 = new List<Department>()
            {
                new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
                new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
            };

            locations2 = new List<Location>
            { 
                new Location {LocationID = 1, Title = "LDF", Address = "Kyiv, Shevchenka St.", BusinessTrips = new List<BusinessTrip>()}, 
                new Location {LocationID = 2, Title = "LDL", Address = "Kyiv, Gorodotska St.", BusinessTrips = new List<BusinessTrip>()}                        
            };

            DateTime currentDate = DateTime.Now.ToLocalTimeAzure().Date;

            businessTrips2 = new List<BusinessTrip>( );
            {
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 1, StartDate = currentDate.AddMonths(-1 ).Date, EndDate = currentDate.AddMonths(-1).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 2, StartDate = currentDate.AddMonths(-1).Date, EndDate = currentDate.AddMonths(-1).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 3, StartDate = currentDate.AddMonths(-7).Date, EndDate = currentDate.AddMonths(-7).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 4, StartDate = currentDate.Date,   EndDate = currentDate.Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 5, StartDate = currentDate.Date, EndDate = currentDate.Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 6, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 7, StartDate = currentDate.AddDays(1).Date, EndDate = currentDate.AddDays(1).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 8, StartDate = currentDate.AddMonths(-3).Date, EndDate = currentDate.AddMonths(-3).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 9, StartDate = currentDate.AddMonths(-6).Date, EndDate = currentDate.AddMonths(-6).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 10, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 11, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Cancelled | BTStatus.Modified, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 12, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 13, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Registered | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 14, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
                AddBusinessTrip(new BusinessTrip { BusinessTripID = 15, StartDate = currentDate, EndDate = currentDate, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Planned | BTStatus.Modified | BTStatus.Cancelled, EmployeeID = 1, LocationID = 1 });
            };

            mock2.Setup(m => m.BusinessTrips).Returns(businessTrips2.AsQueryable());
            mock2.Setup(m => m.Employees).Returns(employees2.AsQueryable());
            mock2.Setup(m => m.Departments).Returns(departments2.AsQueryable());
            mock2.Setup(m => m.Locations).Returns(locations2.AsQueryable());


            departments2.Find(d => d.DepartmentID == 1).Employees.Add(employees2.Find(e => e.EmployeeID == 1));
            departments2.Find(d => d.DepartmentID == 2).Employees.Add(employees2.Find(e => e.EmployeeID == 2));

            employees2.Find(e => e.EmployeeID == 1).Department = departments2.Find(v => v.DepartmentID == 1);
            employees2.Find(e => e.EmployeeID == 2).Department = departments2.Find(v => v.DepartmentID == 2);

            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 1));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 2));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 3));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 4));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 5));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 6));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 7));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 8));
            //employees2.Find(e => e.EmployeeID == 1).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == 9));


            //businessTrips2.Find(b => b.BusinessTripID == 1).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 2).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 3).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 4).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 5).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 6).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 7).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 8).Location = (locations2.Find(l => l.LocationID == 1));
            //businessTrips2.Find(b => b.BusinessTripID == 9).Location = (locations2.Find(l => l.LocationID == 1));

            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 1));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 2));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 3));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 4));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 5));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 6));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 7));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 8));
            //locations2.Find(l => l.LocationID == 1).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == 9));

        }

        public static void AddBusinessTrip(BusinessTrip bt)
        {
            businessTrips2.Add(bt);
            SetBusinessTripRelations(bt);
        }

        private static void SetBusinessTripRelations(BusinessTrip bt)
        {
            employees2.Find(e => e.EmployeeID == bt.EmployeeID).BusinessTrips.Add(businessTrips2.Find(v => v.BusinessTripID == bt.BusinessTripID));
            businessTrips2.Find(b => b.BusinessTripID == bt.BusinessTripID).BTof = (employees2.Find(l => l.EmployeeID == bt.EmployeeID));
            businessTrips2.Find(b => b.BusinessTripID == bt.BusinessTripID).Location = (locations2.Find(l => l.LocationID == bt.LocationID));
            locations2.Find(l => l.LocationID == bt.LocationID).BusinessTrips.Add(businessTrips2.Find(b => b.BusinessTripID == bt.BusinessTripID));
        }


        #region GetListOfDepartments

        [Test]
        public void GetListOfDepartmentsVU_Default_ListOfDepartments()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = "";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void GetListOfDepartmentsVU_DefaultYearAndSDDDA_ListOfDepartments()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = "SDDDA";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void GetListOfDepartmentsVU_DefaultYearAndNull_ListOfDepartments()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = null;
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            IEnumerable<Department> departmentsList = from rep in mock.Object.Departments
                                                      orderby rep.DepartmentName
                                                      select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetListOfDepartmentsVU_Year2012AndDefaultDep_ListOfDepartments()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = 2012;
            string selectedDepartment = "";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);

        }


        [Test]
        public void GetListOfDepartmentsVU_Year2012AndAndSDDDA_ListOfDepartments()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = 2012;
            string selectedDepartment = "SDDDA";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);

        }


        [Test]
        public void GetListOfDepartmentsVU_Year2012AndAndNull_ListOfDepartments()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = null;
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            IEnumerable<Department> departmentsList = from rep in mock.Object.Departments
                                                      orderby rep.DepartmentName
                                                      select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetListOfDepartmentsVU_0AndDefaultDep_ListOfDepartments()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = 0;
            string selectedDepartment = "";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }


        [Test]
        public void GetListOfDepartmentsVU_0AndAndSDDDA_ListOfDepartments()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            // Act  
            int selectedYear = 0;
            string selectedDepartment = "SDDDA";
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);

        }


        [Test]
        public void GetListOfDepartmentsVU_0AndAndNull_ListOfDepartments()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            string selectedDepartment = null;
            var view = controller.GetListOfDepartmentsVU(selectedYear, selectedDepartment);

            IEnumerable<Department> departmentsList = from rep in mock.Object.Departments
                                                      orderby rep.DepartmentName
                                                      select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
        }



        #endregion

        #region GetBusinessTripDataVU

        //[Test]
        //public void GetBusinessTripDataVU_2013AndDefaultDepartment_BTsIn2013()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 2013;
        //    var view = controller.GetBusinessTripDataVU(selectedYear);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(8, employees.Count());
        //    Assert.AreEqual(employees[0].EID, "daol");
        //    Assert.AreEqual(employees[4].EID, "tedk");
        //    Assert.AreEqual(employees[7].EID, "andl");
        //    Assert.AreEqual(employees[0].DaysUsedInBt, 8);
        //    Assert.AreEqual(employees[2].DaysUsedInBt, 22);
        //    Assert.AreEqual(employees[3].DaysUsedInBt, 55);
        //    Assert.AreEqual(employees[4].DaysUsedInBt, 20);
        //    Assert.AreEqual(((employees[2].BusinessTripsByMonth).Values).Count(), 1);
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes); 
        //}

        //[Test]
        //public void GetBusinessTripDataVU_2013AndSDDDA_BTsIn2013OfSDDDA()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 2013;
        //    string selectedDepartment = "SDDDA";
        //    var view = controller.GetBusinessTripDataVU(selectedYear, selectedDepartment);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear,selectedDepartment).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(3, employees.Count());
        //    Assert.AreEqual(employees[0].EID, "xomi");
        //    Assert.AreEqual(employees[1].EID, "tedk");
        //    Assert.AreEqual(employees[2].EID, "andl");
        //    Assert.AreEqual(employees[0].DaysUsedInBt, 22);
        //    Assert.AreEqual(employees[1].DaysUsedInBt, 20);
        //    Assert.AreEqual(employees[2].DaysUsedInBt, 0);
        //    Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 1);
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}

        //[Test]
        //public void GetBusinessTripDataVU_2013AndNull_NoBTs()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 2013;
        //    string selectedDepartment = null;
        //    var view = controller.GetBusinessTripDataVU(selectedYear, selectedDepartment);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear, selectedDepartment).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(0, employees.Count());
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}


        //[Test]
        //public void GetBusinessTripDataVU_2012AndSDDDA_BTsIn2012OfSDDDA()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 2012;
        //    string selectedDepartment = "SDDDA";
        //    var view = controller.GetBusinessTripDataVU(selectedYear, selectedDepartment);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear, selectedDepartment).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(3, employees.Count());
        //    Assert.AreEqual(employees[0].EID, "xomi");
        //    Assert.AreEqual(employees[1].EID, "tedk");
        //    Assert.AreEqual(employees[2].EID, "andl");
        //    Assert.AreEqual(employees[0].DaysUsedInBt, 30);
        //    Assert.AreEqual(employees[1].DaysUsedInBt, 92);
        //    Assert.AreEqual(employees[2].DaysUsedInBt, 0);
        //    Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 1);
        //    Assert.AreEqual(((employees[1].BusinessTripsByMonth).Values).Count(), 1);
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}

        //[Test]
        //public void GetBusinessTripDataVU_2012YearAndDefaultDepartment_BTs2012()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 2012;
        //    var view = controller.GetBusinessTripDataVU(selectedYear);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(8, employees.Count());
        //    Assert.AreEqual(employees[0].EID, "daol");
        //    Assert.AreEqual(employees[4].EID, "tedk");
        //    Assert.AreEqual(employees[7].EID, "andl");
        //    Assert.AreEqual(employees[0].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[2].DaysUsedInBt, 30); 
        //    Assert.AreEqual(employees[4].DaysUsedInBt, 92);
        //    Assert.AreEqual(((employees[2].BusinessTripsByMonth).Values).Count(), 1);
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}

        //[Test]
        //public void GetBusinessTripDataVU_2012YearAndNull_NoBTs()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 2012;
        //    string selectedDepartment = null;
        //    var view = controller.GetBusinessTripDataVU(selectedYear, selectedDepartment);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear, selectedDepartment).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(0, employees.Count());
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}


        //[Test]
        //public void GetBusinessTripDataVU_2017YearAndDefaultDepartment_NoBTs()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int year = 2017;
        //    var view = controller.GetBusinessTripDataVU(year);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(year).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(8, employees.Count());
        //    Assert.AreEqual(employees[0].EID, "daol");
        //    Assert.AreEqual(employees[4].EID, "tedk");
        //    Assert.AreEqual(employees[7].EID, "andl");
        //    Assert.AreEqual(employees[0].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[4].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[7].DaysUsedInBt, 0);
        //    Assert.AreEqual(((employees[4].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.AreEqual(((employees[2].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.AreEqual(((employees[1].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.AreEqual(((employees[7].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}

        //[Test]
        //public void GetBusinessTripDataVU_2017YearAndSDDDA_NoBTs()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 2017;
        //    string selectedDepartment = "SDDDA";
        //    var view = controller.GetBusinessTripDataVU(selectedYear, selectedDepartment);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear, selectedDepartment).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(3, employees.Count());
        //    Assert.AreEqual(employees[0].EID, "xomi");
        //    Assert.AreEqual(employees[1].EID, "tedk");
        //    Assert.AreEqual(employees[2].EID, "andl");
        //    Assert.AreEqual(employees[0].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[1].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[2].DaysUsedInBt, 0);
        //    Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.AreEqual(((employees[1].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);

        //}

        //[Test]
        //public void GetBusinessTripDataVU_2017YearAndNull_NoBTs()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 2017;
        //    string selectedDepartment = null;
        //    var view = controller.GetBusinessTripDataVU(selectedYear, selectedDepartment);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear, selectedDepartment).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(0, employees.Count());
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}

        //[Test]
        //public void GetBusinessTripDataVU_0YearAndDefaultDepartment_NoBTs()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 0;
        //    var view = controller.GetBusinessTripDataVU(selectedYear);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(8, employees.Count());
        //    Assert.AreEqual(employees[0].EID, "daol");
        //    Assert.AreEqual(employees[4].EID, "tedk");
        //    Assert.AreEqual(employees[7].EID, "andl");
        //    Assert.AreEqual(employees[0].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[4].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[7].DaysUsedInBt, 0);
        //    Assert.AreEqual(((employees[4].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.AreEqual(((employees[2].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.AreEqual(((employees[1].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.AreEqual(((employees[7].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}

        //[Test]
        //public void GetBusinessTripDataVU_0YearAndSDDDA_NoBTs()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 0;
        //    string selectedDepartment = "SDDDA";
        //    var view = controller.GetBusinessTripDataVU(selectedYear, selectedDepartment);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear, selectedDepartment).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(3, employees.Count());
        //    Assert.AreEqual(employees[0].EID, "xomi");
        //    Assert.AreEqual(employees[1].EID, "tedk");
        //    Assert.AreEqual(employees[2].EID, "andl");
        //    Assert.AreEqual(employees[0].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[1].DaysUsedInBt, 0);
        //    Assert.AreEqual(employees[2].DaysUsedInBt, 0);
        //    Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.AreEqual(((employees[1].BusinessTripsByMonth).Values).Count(), 0);
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);

        //}

        //[Test]
        //public void GetBusinessTripDataVU_0YearAndNull_NoBTs()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act    
        //    int selectedYear = 0;
        //    string selectedDepartment = null;
        //    var view = controller.GetBusinessTripDataVU(selectedYear, selectedDepartment);
        //    IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataVU(selectedYear, selectedDepartment).Model;
        //    List<EmployeeViewModelForVU> employees = result.ToList();

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("", view.ViewName);
        //    Assert.AreEqual(0, employees.Count());
        //    Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.Monthes);
        //}



        #endregion

        #region GetBusinessTripDataInQuarterVU


        [Test]
        public void GetBusinessTripDataInQuarterVU_2011SDDDA_BTsIn2011()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int currentDay = DateTime.Now.ToLocalTimeAzure().Day;
            int currentYear = DateTime.Now.ToLocalTimeAzure().Year;
            DateTime startDateOfBT = new DateTime(currentYear, currentMonth, currentDay);
            BusinessTrip btInCurrentMonth = new BusinessTrip { BusinessTripID = 1000, StartDate = startDateOfBT, EndDate = startDateOfBT.AddDays(1).Date, LastCRUDedBy = "ncru", LastCRUDTimestamp = new DateTime(2012, 12, 02), Status = BTStatus.Confirmed | BTStatus.Reported, EmployeeID = 2, LocationID = 1, BTof = mock2.Object.Employees.Where(l => l.EmployeeID == 2).FirstOrDefault() };
            mock2.Object.BusinessTrips.ToList().Add(btInCurrentMonth);
            Employee employee = mock2.Object.Employees.Where(e => e.EmployeeID == 2).FirstOrDefault();
            employee.BusinessTrips.Add(btInCurrentMonth);


            // Act    
            int selectedKey = 2011;
            string selectedDepartment = "";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(2, employees.Count());
            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);

            //CollectionAssert.IsEmpty((employees[0].BusinessTripsByMonth).Values);
            //CollectionAssert.IsEmpty((employees[1].BusinessTripsByMonth).Values);

            //Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());
            //Assert.AreEqual(1, employees[1].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());


            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(2011, ((ViewResult)view).ViewBag.SelectedKey);
        }


        [Test]
        public void GetBusinessTripDataInQuarterVU_2013AndDefaultDepartment_BTsIn2013()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2013;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            int daysInDanisBT = ((DateTime.Now.ToLocalTimeAzure().AddDays(-3).Date - new DateTime(2013, 10, 10).Date).Days + 1) +15;

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(24, employees.Count());
            Assert.AreEqual("daol", employees[0].EID);
            Assert.AreEqual("siol", employees[4].EID);
            Assert.AreEqual("ovol", employees[7].EID);
            Assert.AreEqual("Manowens", employees[3].LastName);

            Assert.AreEqual(daysInDanisBT, employees[0].DaysUsedInBt);
            Assert.AreEqual(22, employees[2].DaysUsedInBt);
            Assert.AreEqual(87, employees[3].DaysUsedInBt);
            Assert.AreEqual(0, employees[4].DaysUsedInBt);
            Assert.AreEqual(1, ((employees[2].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2013AndSDDDA_BTsIn2013OfSDDDA()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2013;
            string selectedDepartment = "SDDDA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(5, employees.Count());
            Assert.AreEqual(employees[0].EID, "xomi");
            Assert.AreEqual(employees[1].EID, "chap");
            Assert.AreEqual(employees[2].EID, "tedk");
            Assert.AreEqual(22, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(20, employees[2].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 1);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2013AndNull_NoBTs()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2013;
            string selectedDepartment = null;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }


        [Test]
        public void GetBusinessTripDataInQuarterVU_2012AndSDDDA_BTsIn2012OfSDDDA()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2012;
            string selectedDepartment = "SDDDA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(5, employees.Count());
            Assert.AreEqual(employees[0].EID, "xomi");
            Assert.AreEqual(employees[1].EID, "chap");
            Assert.AreEqual(employees[2].EID, "tedk");
            Assert.AreEqual(employees[0].DaysUsedInBt, 91);
            Assert.AreEqual(91, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 1);
            Assert.AreEqual(0, ((employees[1].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012YearAndDefaultDepartment_BTs2012()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2012;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(24, employees.Count());
            Assert.AreEqual(employees[0].EID, "daol");
            Assert.AreEqual("siol",employees[4].EID);
            Assert.AreEqual("ovol",employees[7].EID);
            Assert.AreEqual(employees[0].DaysUsedInBt, 0);
            Assert.AreEqual(employees[2].DaysUsedInBt, 91);
            Assert.AreEqual(0 ,employees[4].DaysUsedInBt);
            Assert.AreEqual(((employees[2].BusinessTripsByMonth).Values).Count(), 1);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012YearAndNull_NoBTs()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2012;
            string selectedDepartment = null;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }


        [Test]
        public void GetBusinessTripDataInQuarterVU_2017YearAndDefaultDepartment_NoBTs()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2017;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(24, employees.Count());
            Assert.AreEqual(employees[0].EID, "daol");
            Assert.AreEqual("siol",employees[4].EID);
            Assert.AreEqual("ovol",employees[7].EID);
            Assert.AreEqual(employees[0].DaysUsedInBt, 0);
            Assert.AreEqual(0, employees[4].DaysUsedInBt);
            Assert.AreEqual(employees[7].DaysUsedInBt, 0);
            Assert.AreEqual(0, ((employees[4].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(((employees[2].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(((employees[1].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(((employees[7].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2017YearAndSDDDA_NoBTs()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2017;
            string selectedDepartment = "SDDDA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(5, employees.Count());
            Assert.AreEqual(employees[0].EID, "xomi");
            Assert.AreEqual(employees[1].EID, "chap");
            Assert.AreEqual(employees[2].EID, "tedk");
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            //Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(0, employees[2].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(0, ((employees[1].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2017YearAndNull_NoBTs()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2017;
            string selectedDepartment = null;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_0selectedKeyAndDefaultDepartment_BTsInCurrentMonth()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 0;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            List<int> expectedMonthes = new List<int> { currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(2, employees.Count());
            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(4, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);

            Assert.AreEqual(3, (employees[0].BusinessTripsByMonth.Values).Count());
            CollectionAssert.IsEmpty(employees[1].BusinessTripsByMonth);

            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray()[0].BusinessTripID);
            //Assert.AreEqual(10, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray()[2].BusinessTripID);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_0selectedKeyAndTAAAA_NoBts()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 0;
            string selectedDepartment = "TAAAA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            List<int> expectedMonthes = new List<int> { currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(1, employees.Count());
            Assert.AreEqual("kaaa", employees[0].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_0selectedKeyAndNull_NoBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 0;
            string selectedDepartment = null;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            List<int> expectedMonthes = new List<int> { currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }


        [Test]
        public void GetBusinessTripDataInQuarterVU_1selectedKeyAndDefaultDepartment_LastMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 1;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            List<int> expectedMonthes = new List<int> { previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(2, employees.Count());

            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);

            Assert.AreEqual(4, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);

            Assert.AreEqual(3, employees[0].BusinessTripsByMonth.Values.Count);
            CollectionAssert.IsEmpty(employees[1].BusinessTripsByMonth);

            //previous month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray()[0].BusinessTripID);
            //current month
            Assert.AreEqual(2, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray().Count());
            Assert.AreEqual(5, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[0].BusinessTripID);
            Assert.AreEqual(6, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[1].BusinessTripID);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_1selectedKeyAndTAAAA_NoLastMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 1;
            string selectedDepartment = "TAAAA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            List<int> expectedMonthes = new List<int> { previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(1, employees.Count());
            Assert.AreEqual("kaaa", employees[0].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_1selectedKeyAndNull_NoLastMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 1;
            string selectedDepartment = null;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            List<int> expectedMonthes = new List<int> { previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }


        [Test]
        public void GetBusinessTripDataInQuarterVU_3selectedKeyAndDefaultDepartment_LastThreeMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 3;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            List<int> expectedMonthes = new List<int> { thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(2, employees.Count());

            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);

            Assert.AreEqual(4, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[1].BusinessTripsByMonth);
            Assert.AreEqual(3, (employees[0].BusinessTripsByMonth.Values).Count());
            //prevoius month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray().Count());
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[0].Value.ToArray()[0].BusinessTripID);


            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth.Where(k => k.Key == secondPreviousMonth));
            //thirdprevoius month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.ToArray()[2].Value.ToArray().Count());
            Assert.AreEqual(8, employees[0].BusinessTripsByMonth.ToArray()[2].Value.ToArray()[0].BusinessTripID);
            //current month
            Assert.AreEqual(2, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray().Count());
            Assert.AreEqual(5, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[0].BusinessTripID);
            Assert.AreEqual(6, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[1].BusinessTripID);


        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_3selectedKeyAndAndTAAAA_NoLastThreeMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 3;
            string selectedDepartment = "TAAAA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            List<int> expectedMonthes = new List<int> { thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(1, employees.Count());
            Assert.AreEqual("kaaa", employees[0].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_3selectedKeyAndNull_NoLastThreeMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 3;
            string selectedDepartment = null;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            List<int> expectedMonthes = new List<int> { thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }


        [Test]
        public void GetBusinessTripDataInQuarterVU_6selectedKeyAndDefaultDepartment_LastSixMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 6;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            int fourthPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-4).Month;
            int fifthPrevoiusMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-5).Month;
            int sixthPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-6).Month;
            List<int> expectedMonthes = new List<int> { sixthPreviousMonth, fifthPrevoiusMonth, fourthPreviousMonth, thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(2, employees.Count());

            Assert.AreEqual("andl", employees[0].EID);
            Assert.AreEqual("kaaa", employees[1].EID);

            Assert.AreEqual(5, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[1].BusinessTripsByMonth);
            Assert.AreEqual(4, (employees[0].BusinessTripsByMonth.Values).Count());

            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == currentMonth).Count());
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == previousMonth).Count());
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth.Where(k => k.Key == secondPreviousMonth));
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == thirdPreviousMonth).Count());
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth.Where(k => k.Key == fourthPreviousMonth));
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth.Where(k => k.Key == fifthPrevoiusMonth));
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == sixthPreviousMonth).Count());
                        
            //current month
            Assert.AreEqual(2, employees[0].BusinessTripsByMonth.Where(k => k.Key == currentMonth).FirstOrDefault().Value.Count());
            Assert.AreEqual(5, employees[0].BusinessTripsByMonth.Where(k => k.Key == currentMonth).FirstOrDefault().Value[0].BusinessTripID);
            Assert.AreEqual(6, employees[0].BusinessTripsByMonth.Where(k => k.Key == currentMonth).FirstOrDefault().Value[1].BusinessTripID); 

            //previous month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == previousMonth).FirstOrDefault().Value.Count());
            //Assert.AreEqual(3, employees[0].BusinessTripsByMonth.ToArray()[1].Value.ToArray()[0].BusinessTripID);
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == previousMonth).FirstOrDefault().Value[0].BusinessTripID);

            //thirdprevious month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == thirdPreviousMonth).FirstOrDefault().Value.Count());
            Assert.AreEqual(8, employees[0].BusinessTripsByMonth.Where(k => k.Key == thirdPreviousMonth).FirstOrDefault().Value[0].BusinessTripID);

            //sixprevoius month
            Assert.AreEqual(1, employees[0].BusinessTripsByMonth.Where(k => k.Key == sixthPreviousMonth).FirstOrDefault().Value.Count());
            Assert.AreEqual(9, employees[0].BusinessTripsByMonth.Where(k => k.Key == sixthPreviousMonth).FirstOrDefault().Value[0].BusinessTripID);

        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_6selectedKeyAndAndTAAAA_NoLastSixMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 6;
            string selectedDepartment = "TAAAA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            int fourthPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-4).Month;
            int fifthPrevoiusMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-5).Month;
            int sixPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-6).Month;
            List<int> expectedMonthes = new List<int> { sixPreviousMonth, fifthPrevoiusMonth, fourthPreviousMonth, thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
            Assert.AreEqual(1, employees.Count());
            Assert.AreEqual("kaaa", employees[0].EID);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            CollectionAssert.IsEmpty(employees[0].BusinessTripsByMonth);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_6selectedKeyAndNull_NoLastSixMonthBTs()
        {
            // Arrange
            SetUp2();
            BusinessTripViewerController controller = new BusinessTripViewerController(mock2.Object);

            // Act    
            int selectedKey = 6;
            string selectedDepartment = null;
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment).Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            int currentMonth = DateTime.Now.ToLocalTimeAzure().Month;
            int previousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-1).Month;
            int secondPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-2).Month;
            int thirdPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-3).Month;
            int fourthPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-4).Month;
            int fifthPrevoiusMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-5).Month;
            int sixPreviousMonth = DateTime.Now.ToLocalTimeAzure().Date.AddMonths(-6).Month;
            List<int> expectedMonthes = new List<int> { sixPreviousMonth, fifthPrevoiusMonth, fourthPreviousMonth, thirdPreviousMonth, secondPreviousMonth, previousMonth, currentMonth };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("GetBusinessTripDataInMonthesVU", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012selectedKeyAndTAAAA_emptySearchString()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2012;
            string selectedDepartment = "SDDDA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment, "").Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(5, employees.Count());
            Assert.AreEqual(employees[0].EID, "xomi");
            Assert.AreEqual(employees[1].EID, "chap");
            Assert.AreEqual(employees[2].EID, "tedk");
            Assert.AreEqual(employees[0].DaysUsedInBt, 91);
            Assert.AreEqual(91, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 1);
            Assert.AreEqual(0, ((employees[1].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);

        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012selectedKeyAndTAAAA_LongSearchString_empty()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            int selectedKey = 2012;
            string selectedDepartment = "SDDDA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment);
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment, "ADJHAAS").Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());         
        }

        [Test]
        public void GetBusinessTripDataInQuarterVU_2012selectedKeyAndTAAAA_SearchString()
        {

            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            int selectedKey = 2012;
            string selectedDepartment = "SDDDA";
            var view = controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment, "a");
            IEnumerable<EmployeeViewModelForVU> result = (IEnumerable<EmployeeViewModelForVU>)controller.GetBusinessTripDataInQuarterVU(selectedKey, selectedDepartment, "a").Model;
            List<EmployeeViewModelForVU> employees = result.ToList();
            List<int> expectedMonthes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(3, employees.Count());
            Assert.AreEqual(employees[0].EID, "chap");
            Assert.AreEqual(employees[1].EID, "ivte");
            Assert.AreEqual(employees[2].EID, "andl");
            Assert.AreEqual(employees[0].DaysUsedInBt, 0);
            Assert.AreEqual(0, employees[0].DaysUsedInBt);
            Assert.AreEqual(0, employees[1].DaysUsedInBt);
            Assert.AreEqual(((employees[0].BusinessTripsByMonth).Values).Count(), 0);
            Assert.AreEqual(0, ((employees[1].BusinessTripsByMonth).Values).Count());
            Assert.AreEqual(expectedMonthes, ((ViewResult)view).ViewBag.MonthList);
        }


        #endregion


        #region GetBusinessTripVu

        //[Test]
        //public void GetListOfYearsVU_Default_ListOfYear()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act
        //    int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
        //    var view = controller.GetListOfYearsVU(selectedYear);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ActionResult), view);
        //    Assert.AreEqual("", ((ViewResult)view).ViewName);
        //    Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
        //   // Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.SelectedYear);
        //}

        //[Test]
        //public void GetListOfYearsVU_2012_ListOfYear()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act
        //    int selectedYear = 2012;
        //    var view = controller.GetListOfYearsVU(selectedYear);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ActionResult), view);
        //    Assert.AreEqual("", ((ViewResult)view).ViewName);
        //    Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
        //}

        //[Test]
        //public void GetListOfYearsVU_0_ListOfYear()
        //{
        //    // Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act
        //    int selectedYear = 0;
        //    var view = controller.GetListOfYearsVU(selectedYear);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ActionResult), view);
        //    Assert.AreEqual("", ((ViewResult)view).ViewName);
        //    Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
        //}

        #endregion

        #region GetListOfYearsForQuarterVU

        [Test]
        public void GetListOfYearsForQuarterVU_YearNow_ListOfYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            var view = controller.GetListOfYearsForQuarterVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            // Assert.IsInstanceOf(typeof(int[]), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetListOfYearsForQuarterVU_2012_ListOfYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = 2012;
            var view = controller.GetListOfYearsForQuarterVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
        }

        [Test]
        public void GetListOfYearsForQuarterVU_0_ListOfYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = 0;
            var view = controller.GetListOfYearsForQuarterVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
        }

        #endregion

        #region ShowBTInformation

        [Test]
        public void ShowBTInformation_NotExistingBT_HttpNotFound()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
            var view = controller.ShowBTInformation(100);

            // Assert
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
            Assert.IsNull(businessTrip);
            Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        }


        [Test]
        public void ShowBTInformation_ExistingBT_ViewBT()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
            var view = controller.ShowBTInformation(25);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(25, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        }

        [Test]
        public void ShowBTInformation_ExistingReportedBTButCancelled_HttpNotFound()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 18).FirstOrDefault();
            var view = controller.ShowBTInformation(18);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(18, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        }

        [Test]
        public void ShowBTInformation_ExistingConfirmedBTButNotReported_HttpNotFound()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
            var view = controller.ShowBTInformation(3);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(3, businessTrip.BusinessTripID);
            Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        }


        #endregion

        #region ShowPrepsBTInformation

        //[Test]
        //public void ShowPrepsBTInformation_NotExistingBT_HttpNotFound()
        //{
        //    //Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
        //    BusinessTrip bt = new BusinessTrip { BusinessTripID = 100, StartDate = new DateTime(2014, 12, 01), EndDate = new DateTime(2014, 12, 10), Status = BTStatus.Planned, EmployeeID = 1, LocationID = 1 };

        //    // Act
        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 100).FirstOrDefault();
        //    var view = controller.ShowPrepsBTInformation(100);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(HttpNotFoundResult), view);
        //    Assert.IsNull(businessTrip);
        //    Assert.AreEqual(404, ((HttpNotFoundResult)view).StatusCode);
        //}


        //[Test]
        //public void ShowPrepsBTInformation_ExistingBT_ViewBT()
        //{
        //    //Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act
        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 25).FirstOrDefault();
        //    var view = controller.ShowPrepsBTInformation(25);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("ShowBTInformation", ((ViewResult)view).ViewName);
        //    Assert.AreEqual(25, businessTrip.BusinessTripID);
        //    Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        //}

        //[Test]
        //public void ShowPrepsBTInformation_ExistingReportedBTButCancelled_ViewBT()
        //{
        //    //Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act
        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 18).FirstOrDefault();
        //    var view = controller.ShowPrepsBTInformation(18);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("ShowBTInformation", ((ViewResult)view).ViewName);
        //    Assert.AreEqual(18, businessTrip.BusinessTripID);
        //    Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        //}

        //[Test]
        //public void ShowPrepsBTInformation_ExistingConfirmedBTButNotReported_ViewBT()
        //{
        //    //Arrange
        //    BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

        //    // Act
        //    BusinessTrip businessTrip = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 3).FirstOrDefault();
        //    var view = controller.ShowPrepsBTInformation(3);

        //    // Assert
        //    Assert.IsInstanceOf(typeof(ViewResult), view);
        //    Assert.AreEqual("ShowBTInformation", ((ViewResult)view).ViewName);
        //    Assert.AreEqual(3, businessTrip.BusinessTripID);
        //    Assert.IsInstanceOf(typeof(BusinessTrip), ((ViewResult)view).Model);
        //}


        #endregion

        #region GetPrepBusinessTripDataVU

        [Test]
        public void GetPrepBusinessTripDataVU_Default_ListOfEmployee()
        {
            // Arrange

            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);


            // Act    
            var view = controller.GetPrepBusinessTripDataVU();
            IEnumerable<PrepBusinessTripViewModelForVU> result = (IEnumerable<PrepBusinessTripViewModelForVU>)controller.GetPrepBusinessTripDataVU().Model;
            List<PrepBusinessTripViewModelForVU> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(10, employees.Count());
            Assert.AreEqual(employees[0].EID, "daol");
            Assert.AreEqual(employees[2].EID, "xomi");
            Assert.AreEqual(employees[3].EID, "xtwe");
            Assert.AreEqual(employees[0].BusinessTripsByEmployee.Values.Count(), 3);
            Assert.AreEqual(employees[2].BusinessTripsByEmployee.Values.Count(), 1);
            Assert.AreEqual(employees[3].BusinessTripsByEmployee.Values.Count(), 4);
        }

        #endregion

        #region GetBusinessTripByDatesVU

        [Test]
        public void GetBusinessTripByDatesVU_Default_DefaultYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesVU();

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesVU_0Year_ListofYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripByDatesVU_Current_ListOfYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = DateTime.Now.ToLocalTimeAzure().Year;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesVU_2012_ListOfYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = 2012;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesVU_0_ListOfYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = 0;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripByDatesVU_NotExistingYear_ListOfYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act
            int selectedYear = DateTime.Now.AddYears(10).Year;
            var view = controller.GetBusinessTripByDatesVU(selectedYear);

            // Assert
            Assert.IsInstanceOf(typeof(ActionResult), view);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(selectedYear, ((ViewResult)view).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)view).ViewBag.SelectedYear);
        }

        #endregion

        #region GetBusinessTripDataByDatesVU

        [Test]
        public void GetBusinessTripDataByDatesVU_0_ListOfYear()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            int selectedYear = 0;

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IEnumerable<BusinessTripViewModel> result = (IEnumerable<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
            Assert.IsInstanceOf(typeof(int), ((PartialViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripDataByDatesVU_2013Year_BTs2013()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            int selectedYear = 2013;
            BusinessTrip btStartDateToBeChanged = mock.Object.BusinessTrips.Where(b => b.BusinessTripID == 29).FirstOrDefault();
            btStartDateToBeChanged.StartDate = new DateTime(2018, 01, 01);

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IEnumerable<BusinessTripViewModel> result = (IEnumerable<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> bts = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(13, bts.Count());
            Assert.AreEqual(bts[0].BTof.EID, "daol");
            Assert.AreEqual(bts[6].BTof.EID, "tadk");
            Assert.AreEqual(bts[7].BTof.EID, "xomi");
            Assert.AreEqual(bts[8].BTof.EID, "xtwe");
            Assert.AreEqual("iwpe", bts[10].BTof.EID);
            Assert.AreEqual("tedk", bts[11].BTof.EID);

            Assert.AreEqual(23, bts[0].BusinessTripID);
            Assert.AreEqual(24, bts[1].BusinessTripID);
            Assert.AreEqual(25, bts[2].BusinessTripID);
            Assert.AreEqual(33, bts[3].BusinessTripID);
            Assert.AreEqual(26, bts[4].BusinessTripID);
            Assert.AreEqual(27, bts[5].BusinessTripID);
            Assert.AreEqual(18, bts[6].BusinessTripID);
            Assert.AreEqual(35, bts[7].BusinessTripID);
            Assert.AreEqual(32, bts[8].BusinessTripID);
            Assert.AreEqual(21, bts[9].BusinessTripID);
            Assert.AreEqual(39, bts[10].BusinessTripID);
            Assert.AreEqual(36, bts[11].BusinessTripID);

            Assert.AreEqual(2013, ((PartialViewResult)view).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripDataByDatesVU_2012Year_BTs2012()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            int selectedYear = 2012;
            var status = BTStatus.Confirmed | BTStatus.Reported;


            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IEnumerable<BusinessTripViewModel> result = (IEnumerable<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(2, employees.Count());
            Assert.AreEqual(employees[0].BTof.EID, "xomi");
            Assert.AreEqual(employees[1].BTof.EID, "tedk");
            Assert.AreEqual(employees[0].Status, status);
            Assert.AreEqual(employees[1].Status, status);
            Assert.IsInstanceOf(typeof(int), ((PartialViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripDataByDatesVU_2014Year_BTs2014()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            int selectedYear = 2014;

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IEnumerable<BusinessTripViewModel> result = (IEnumerable<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> bts = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(7, bts.Count());
            Assert.AreEqual("xtwe", bts[0].BTof.EID);
            Assert.AreEqual("xtwe", bts[1].BTof.EID);
            Assert.AreEqual("chap", bts[2].BTof.EID);
            Assert.AreEqual("tedk", bts[3].BTof.EID);

            Assert.AreEqual(14, bts[0].BusinessTripID);
            Assert.AreEqual(15, bts[1].BusinessTripID);
            Assert.AreEqual(38, bts[2].BusinessTripID);
            Assert.AreEqual(37, bts[3].BusinessTripID);
            Assert.AreEqual(2014, ((PartialViewResult)view).ViewBag.SelectedYear);
        }


        [Test]
        public void GetBusinessTripDataByDatesVU_DefaultYear_NoBTs()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU();
            IEnumerable<BusinessTripViewModel> result = (IEnumerable<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU().Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
        }

        [Test]
        public void GetBusinessTripDataByDatesVU_NotExistingYear_NoBTs()
        {
            // Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            int selectedYear = DateTime.Now.AddYears(-10).Year;

            // Act    
            var view = controller.GetBusinessTripDataByDatesVU(selectedYear);
            IEnumerable<BusinessTripViewModel> result = (IEnumerable<BusinessTripViewModel>)controller.GetBusinessTripDataByDatesVU(selectedYear).Model;
            List<BusinessTripViewModel> employees = result.ToList();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(0, employees.Count());
        }

        #endregion

        #region ExportBusinessTripByDatesToExcel
        [Test]
        public void ExportBusinessTripByDatesToExcel_Year2014_FileResult()
        {
            //Arrange 
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act 
            FileResult file = controller.ExportBusinessTripByDatesToExcel(2014) as FileResult;

            //Assert 
            Assert.IsInstanceOf(typeof(FileResult), file);
        }

        [Test]
        public void CreateCaption_workSheet_ProperCaptionOnWorksheet()
        {
            //Arrange 
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            Worksheet workSheet = new Worksheet("BusinessTripsByDates");
            string[] caption = new string[] { "ID", "EID", "Name", "Loc", "From", "To", "Unit", "Purpose", "Mgr", "Resp" };

            //Act 
            controller.CreateCaption(workSheet);

            //Assert 
            for (int i = 0; i < caption.Length; i++)
            {
                Assert.AreEqual(caption[i], workSheet.Cells[0, i].Value);
            }
        }

        [Test]
        public void CreateCaption_Null_NullReferenceException()
        {
            //Arrange 
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            string[] caption = new string[] { "ID", "EID", "Name", "Loc", "From", "To", "Unit", "Purpose", "Mgr", "Resp" };

            //Act 

            //Assert 
            Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));
        }
        #endregion

        #region GetBusinessTripByUnitsVU

        [Test]
        public void GetBusinessTripByUnitsVU()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act
            var result = controller.GetBusinessTripByUnitsVU(2014);

            //Assert
            Assert.IsInstanceOf(typeof(ActionResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.AreEqual(2014, ((ViewResult)result).Model);
            Assert.IsInstanceOf(typeof(SelectList), ((ViewResult)result).ViewBag.SelectedYear);
        }

        [Test]
        public void GetBusinessTripDataByUnitsVU()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act
            var result = controller.GetBusinessTripDataByUnitsVU(2014);

            //Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.AreEqual("", ((PartialViewResult)result).ViewName);
            Assert.AreEqual(2014, ((PartialViewResult)result).ViewBag.SelectedYear);
            Assert.IsInstanceOf(typeof(Int32), ((PartialViewResult)result).ViewBag.SelectedYear);            
        }

        #endregion

        [Test]
        public void BusinessTripDataByUnitsWithoutCancelledAndDismissedQuery_AnyYear_CorrectQuery()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act
            IEnumerable<BusinessTripViewModel> result = controller.BusinessTripDataByUnitsWithoutCancelledAndDismissedQuery(2014);

            //Assert        
            Assert.AreEqual(6, result.Count());
            foreach (BusinessTripViewModel item in result)
            {
                Assert.AreEqual(false, item.Status.HasFlag(BTStatus.Cancelled));
                Assert.AreEqual(null, item.BTof.DateDismissed);
            }
        }

        [Test]
        public void WriteBusinessTripsData_WorksheetNull_Exception()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act

            //Assert        
            Assert.Throws<NullReferenceException>(() => controller.WriteBusinessTripsData(null, 2014));            
        }

        [Test]
        public void WriteBusinessTripsData_WorksheetNotNull_Exception()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);
            Worksheet workSheet = new Worksheet("New worksheet");

            //Act
            controller.WriteBusinessTripsData(workSheet, 2014);

            //Assert        
            Assert.AreEqual("1", workSheet.Cells[1, 0].Value.ToString());
            Assert.AreEqual("2014-10-01 To be updated soon", workSheet.Cells[1, 4].Value.ToString());
            Assert.AreEqual("2014-03-01", workSheet.Cells[5, 4].Value.ToString());
            Assert.AreEqual("xtwe", workSheet.Cells[2, 1].Value.ToString());
            Assert.AreEqual("LDF", workSheet.Cells[3, 3].Value.ToString());
        }

        #region GetBusinessTripIDInYear

        [Test]
        public void GetFirstBusinessTripIdInYear_YearContainsBTs_IDOfFirstBT()
        {
            //Arrange
            BusinessTripViewerController controller = new BusinessTripViewerController(mock.Object);

            //Act
            int result = controller.GetFirstBusinessTripIdInYear(DateTime.Now.ToLocalTimeAzure().Year);

            //Assert     
            Assert.AreEqual(4, result);

        }

        #endregion

    }
}
