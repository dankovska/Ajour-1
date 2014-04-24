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
        public class PositionControllerTest
        {
            Mock<IRepository> mock;

            [SetUp]
            public void Setup()
            {

                mock = Mock_Repository.CreateMock();
            //List<Department> departments = new List<Department>{
            //         new Department{DepartmentID = 1, DepartmentName = "SDDDA",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 2, DepartmentName = "TAAAA",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 3, DepartmentName = "RAAA1",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 4, DepartmentName = "RAAA2",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 5, DepartmentName = "RAAA3",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 6, DepartmentName = "RAAA4",Employees = new List<Employee>()},
            //         new Department{DepartmentID = 7, DepartmentName = "RAAA5",Employees = new List<Employee>()}};

            //List<Employee> employees = new List<Employee>
            // {
            //    new Employee {EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, EID = "andl", PositionID = 2, DateDismissed = new DateTime(2013,11,01), DateEmployed = new DateTime(2011,11,01), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 2, FirstName = "Anatoliy", LastName = "Struz", DepartmentID = 2, EID = "ascr", PositionID = 2, DateEmployed = new DateTime(2013,04,11), IsManager = true, BusinessTrips = new List<BusinessTrip>()},          
            //    new Employee {EmployeeID = 3, FirstName = "Tymur", LastName = "Pyorge", DepartmentID = 1, EID = "tedk", PositionID = 2, DateEmployed = new DateTime(2013,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 4, FirstName = "Tanya", LastName = "Kowood", DepartmentID = 4 , EID = "tadk", PositionID = 2, DateEmployed = new DateTime(2012,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 5, FirstName = "Ivan", LastName = "Daolson", DepartmentID = 6, EID = "daol", PositionID = 2, DateEmployed = new DateTime(2013,07,21), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 6, FirstName = "Boryslav", LastName = "Teshaw", DepartmentID = 5, EID = "tebl", PositionID = 2, DateEmployed = new DateTime(2011,04,11), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 7, FirstName = "Tanya", LastName = "Manowens", DepartmentID = 5, EID = "xtwe", PositionID = 4, DateEmployed = new DateTime(2012,09,04), IsManager = false, BusinessTrips = new List<BusinessTrip>()},
            //    new Employee {EmployeeID = 8, FirstName = "Oleksiy", LastName = "Kowwood", DepartmentID = 1, EID = "xomi",PositionID = 3, DateEmployed = new DateTime(11/02/2011), IsManager = true }
            // };

            //List<Position> positions = new List<Position>
            //    {

            //    new Position {PositionID = 1, TitleEn = "Employee", TitleUk = "Працівник" , Employees = new List<Employee>()},
            //    new Position {PositionID = 2, TitleEn = "Software developer", TitleUk = "Розробник програмного забезпечення", Employees = new List<Employee>()},
            //    new Position {PositionID = 3, TitleEn = "Director", TitleUk = "Директор", Employees = new List<Employee>() },
            //    new Position {PositionID = 4, TitleEn = "Manager", TitleUk = "Лайн-менеджер", Employees = new List<Employee>() },
            //    };
         


            //mock.Setup(m => m.Positions).Returns(positions.AsQueryable());
            //mock.Setup(m => m.Employees).Returns(employees.AsQueryable());
            //mock.Setup(m => m.Departments).Returns(departments.AsQueryable());



            //employees.Find(b => b.EmployeeID == 1).Position = (positions.Find(l => l.PositionID == 2));
            //employees.Find(b => b.EmployeeID == 2).Position = (positions.Find(l => l.PositionID == 2));
            //employees.Find(b => b.EmployeeID == 3).Position = (positions.Find(l => l.PositionID == 2));
            //employees.Find(b => b.EmployeeID == 4).Position = (positions.Find(l => l.PositionID == 2));
            //employees.Find(b => b.EmployeeID == 5).Position = (positions.Find(l => l.PositionID == 2));
            //employees.Find(b => b.EmployeeID == 6).Position = (positions.Find(l => l.PositionID == 2));
            //employees.Find(b => b.EmployeeID == 7).Position = (positions.Find(l => l.PositionID == 4));
            //employees.Find(b => b.EmployeeID == 8).Position = (positions.Find(l => l.PositionID == 3));
 
            //positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e=> e.EmployeeID == 1));
            //positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e=> e.EmployeeID == 2));
            //positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e=> e.EmployeeID == 3));
            //positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e=> e.EmployeeID == 4));
            //positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e=> e.EmployeeID == 5));
            //positions.Find(l => l.PositionID == 2).Employees.Add(employees.Find(e=> e.EmployeeID == 6));
            //positions.Find(l => l.PositionID == 4).Employees.Add(employees.Find(e=> e.EmployeeID == 7));
            //positions.Find(l => l.PositionID == 3).Employees.Add(employees.Find(e=> e.EmployeeID == 8));


            //departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 1));
            //departments.Find(d => d.DepartmentID == 2).Employees.Add(employees.Find(e => e.EmployeeID == 2));
            //departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 3));
            //departments.Find(d => d.DepartmentID == 4).Employees.Add(employees.Find(e => e.EmployeeID == 4));
            //departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 5));
            //departments.Find(d => d.DepartmentID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 6));
            //departments.Find(d => d.DepartmentID == 3).Employees.Add(employees.Find(e => e.EmployeeID == 7));
            //departments.Find(d => d.DepartmentID == 1).Employees.Add(employees.Find(e => e.EmployeeID == 8));

            //employees.Find(e => e.EmployeeID == 1).Department = departments.Find(v => v.DepartmentID == 1);
            //employees.Find(e => e.EmployeeID == 2).Department = departments.Find(v => v.DepartmentID == 2);
            //employees.Find(e => e.EmployeeID == 3).Department = departments.Find(v => v.DepartmentID == 1);
            //employees.Find(e => e.EmployeeID == 4).Department = departments.Find(v => v.DepartmentID == 4);
            //employees.Find(e => e.EmployeeID == 5).Department = departments.Find(v => v.DepartmentID == 6);
            //employees.Find(e => e.EmployeeID == 6).Department = departments.Find(v => v.DepartmentID == 5);
            //employees.Find(e => e.EmployeeID == 7).Department = departments.Find(v => v.DepartmentID == 5);
            //employees.Find(e => e.EmployeeID == 8).Department = departments.Find(v => v.DepartmentID == 1);
            
            }



            [Test]
            [Category("View names")]
            public void IndexView_True()
            {
                // Arrange - create the controller

                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                ViewResult result = target.Index();

                // Assert - check the result 
                Assert.AreEqual("", result.ViewName);
            }

            [Test]
            public void Index_Default_Allpositions()
            {
                // Arrange - create the controller     
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                IEnumerable<Position> result = (IEnumerable<Position>)target.Index().Model;
                List<Position> positionView = result.ToList<Position>();

                // Assert - check the result 
                CollectionAssert.AreEqual(mock.Object.Positions, positionView);
            }

            [Test]
            [Category("View names")]
            public void CreateGetView_True()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                var result = target.Create() as ViewResult;

                // Assert - check the result 
                Assert.AreEqual("", result.ViewName);
            }

            [Test]
            public void CreatePost_CanCreate_ValidPosition()
            {
                // Arrange - create the controller                 
                PositionController target = new PositionController(mock.Object);
                Position position = new Position { PositionID = 5, TitleEn="Accountant", TitleUk="Бухгалтер", Employees=new List<Employee> ()};

                // Act - call the action method 
                RedirectToRouteResult result = (RedirectToRouteResult)target.Create(position);

                // Assert - check the result 
                mock.Verify(m => m.SavePosition(position), Times.Once);
                Assert.IsFalse(result.Permanent);
                Assert.AreEqual("Home", result.RouteValues["controller"]);
                Assert.AreEqual("PUView", result.RouteValues["action"]);
                Assert.AreEqual(3, result.RouteValues["tab"]);
            }

            [Test]
            public void CreatePost_CannotCreate_InvalidPosition()
            {
          
                // Arrange - create the controller
                Mock<IRepository> mRepository = new Mock<IRepository>();
                mRepository.Setup(d => d.Positions).Returns(new Position[]{
                new Position{PositionID = 4}
                }.AsQueryable());
                Position position = new Position();
                PositionController target = new PositionController(mRepository.Object);

                // Act - call the action method 
                target.ModelState.AddModelError("error", "error");
                var result = target.Create(position);

                // Assert - check the result 
                mRepository.Verify(m => m.SavePosition(It.IsAny<Position>()), Times.Never);
                Assert.AreEqual(false, ((ViewResult)result).ViewData.ModelState.IsValid);
                Assert.IsInstanceOf(typeof(Position), ((ViewResult)result).ViewData.Model);
                Assert.IsInstanceOf(typeof(ViewResult), result);
            }

            [Test]
            [Category("View names")]
            public void EditView_True()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                var result = target.Edit(2);

                // Assert - check the result 
                Assert.AreEqual("", ((ViewResult)result).ViewName);
            }

            [Test]
            public void EditGet_CanEdit_ValidPosition()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                var result = target.Edit(2);

                // Assert - check the result
                Assert.IsInstanceOf(typeof(ViewResult), result);
                Assert.AreEqual("", ((ViewResult)result).ViewName);
                Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            }

            [Test]
            public void EditGet_CannotEdit_InvalidPosition()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                var result = target.Edit(15);
                Position position = mock.Object.Positions.Where(m => m.PositionID == 150).FirstOrDefault();

                // Assert - check the result 
                Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
                Assert.IsNull(position);
                Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            }

            [Test]
            public void EditPost_CanEdit_ValidPosition()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);
                Position position = new Position { PositionID = 6, TitleEn = "Software developer", TitleUk = "Розробник програмного забезпечення" };

                // Act - call the action method 
                var result = target.Edit(position);

                // Assert - check the result
                Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
                Assert.IsFalse(((RedirectToRouteResult)result).Permanent);
                Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
                Assert.AreEqual("PUView", ((RedirectToRouteResult)result).RouteValues["action"]);
                mock.Verify(m => m.SavePosition(position), Times.Once);
                Assert.AreEqual(3, ((RedirectToRouteResult)result).RouteValues["tab"]);
            }


            [Test]
            public void EditPost_CannotEdit_InvalidPosition()
            {
                // Arrange - create the controller 
                Position position = new Position();
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                target.ModelState.AddModelError("error", "error");
                var result = target.Edit(position);
                string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

                // Assert - check the result 
                mock.Verify(m => m.SavePosition(position), Times.Never);
                //Assert.AreEqual(false, ((ViewResult)result).ViewData.ModelState.IsValid);
                Assert.IsInstanceOf(typeof(JsonResult), result);
                Assert.AreEqual("", data);
                //Assert.IsInstanceOf(typeof(Position), ((ViewResult)result).ViewData.Model);
            }

            [Test]
            public void EditPost_ValidModelConcurrency_ErrorReturned()
            {
                //Arrange
                PositionController controller = new PositionController(mock.Object);
                string modelError = "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled.";
                Position position = mock.Object.Positions.Where(p => p.PositionID == 1).FirstOrDefault();
                mock.Setup(m => m.SavePosition(position)).Throws(new DbUpdateConcurrencyException());

                //Act
                var result = controller.Edit(position);
                string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

                //Assert
                mock.Verify(d => d.SavePosition(position), Times.Once());
                Assert.AreEqual(typeof(JsonResult), result.GetType());
                Assert.AreEqual(modelError, data);
            }



            [Test]
            public void DeleteGet_ValidPositionWithAssociatedDate_CannotDelete()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                var result = target.Delete(4);

                // Assert - check the result 
                Assert.AreEqual("CannotDelete", ((ViewResult)result).ViewName);
                Assert.IsInstanceOf(typeof(ViewResult), result);
            }
         
            [Test]
            public void DeleteGet_ValidPositionWithoutAssociatedDate_CannotDelete()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                var result = target.Delete(1);

                // Assert - check the result 
                Assert.AreEqual("", ((ViewResult)result).ViewName);
                Assert.IsInstanceOf(typeof(Position), ((ViewResult)result).ViewData.Model);
                Assert.IsInstanceOf(typeof(ViewResult), result);
                Assert.IsNotNull(((ViewResult)result).ViewData.Model);
            }


            [Test]
            public void DeleteGet_InvalidPosition()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                var result = target.Delete(150);
                Position position = mock.Object.Positions.Where(m => m.PositionID == 150).FirstOrDefault();

                // Assert - check the result 
                Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
                Assert.IsNull(position);
                Assert.AreEqual(404, ((HttpNotFoundResult)result).StatusCode);
            }

            [Test]
            public void DeletePost_CanDelete_ValidPosition()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);

                // Act - call the action method 
                var result = target.DeleteConfirmed(1);

                // Assert - check the result 
                mock.Verify(m => m.DeletePosition(1), Times.Once);
                Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
                Assert.AreEqual("PUView", ((RedirectToRouteResult)result).RouteValues["action"]);
                Assert.AreEqual(3, ((RedirectToRouteResult)result).RouteValues["tab"]);
                Assert.IsFalse(((RedirectToRouteResult)result).Permanent);
                Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            }

            [Test]
            public void DeletePost_CannotDelete_ValidPosition()
            {
                // Arrange - create the controller 
                PositionController target = new PositionController(mock.Object);
                mock.Setup(x => x.DeletePosition(It.IsAny<int>()))
                    .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


                // Act - call the action method 
                var result = target.DeleteConfirmed(2);

                // Assert - check the result 
                mock.Verify(m => m.DeletePosition(2), Times.Once);
                Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
                Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
                Assert.AreEqual("DataBaseDeleteError", ((RedirectToRouteResult)result).RouteValues["action"]);


            }

        }
    
}
