using System;
using AjourBT.Controllers;
using AjourBT.Models;
using System.Collections.Generic;
using AjourBT.Domain.Abstract;
using AjourBT.Domain.Entities;
using Moq;
using System.Linq;
using System.Web.Mvc;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Globalization;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class EmployeeControllerTest
    {
        Mock<IRepository> mock;
        Mock<IRepository> departments;
        [SetUp]
        public void SetUp()
        {
            mock = Mock_Repository.CreateMock();

        }

        #region GetEmployee

        [Test]
        public void GetEmployee_Null_NullSelectedDepartment()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);

            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var view = controller.GetEmployee(null);
            string selectedDepartment = null;
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)view).ViewBag.JSDatePattern);

        }

        [Test]
        public void GetEmployee_StringEmpty_StringEmptySelectedDepartment()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);

            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var view = controller.GetEmployee("");
            string selectedDepartment = "";
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)view).ViewBag.JSDatePattern);

        }

        [Test]
        public void GetEmployee_SDDDA_SDDDASelectedDepartment()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);

            MvcApplication.JSDatePattern = "dd.mm.yy";
            //Act
            var view = controller.GetEmployee("SDDDA");
            string selectedDepartment = "SDDDA";
            IEnumerable<Department> depList = from rep in mock.Object.Departments
                                              orderby rep.DepartmentName
                                              select rep;
            //Assert
            Assert.IsInstanceOf(typeof(ViewResult), view);
            Assert.AreEqual(null, ((ViewResult)view).Model);
            Assert.AreEqual("", ((ViewResult)view).ViewName);
            Assert.AreEqual(depList.ToList(), ((ViewResult)view).ViewBag.DepartmentList.Items);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((ViewResult)view).ViewBag.JSDatePattern);

        }

        #endregion

        #region GetEmployeeData
        [Test]
        public void GetEmployeeData_Null_AllEmployees()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = null;

            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(24, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].FirstName, "Oleksiy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].LastName, "Struz");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].FirstName, "Anatoliy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[3].LastName, "Kowood");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[4].LastName, "Manowens");
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[5].LastName);
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[7].LastName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetEmployeeData_SDDDA_SDDDAEmployees()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "SDDDA";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeData(selectedDepartment);
            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(5, result.ToArray<EmployeeViewModel>().Length);
            Assert.IsTrue(result.ToArray<EmployeeViewModel>().Length == 5);
            Assert.AreEqual(5, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].LastName, "Kowwood");
            Assert.AreEqual("Chuck", result.ToArray<EmployeeViewModel>()[1].FirstName);
            Assert.AreEqual("Pyorge", result.ToArray<EmployeeViewModel>()[2].LastName);

            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);

        }

        [Test]
        public void GetEmployeeData_StringEmpty_AllEmployees()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.AreEqual(24, result.ToArray<EmployeeViewModel>().Length);
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[0].FirstName, "Oleksiy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].LastName, "Struz");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[1].FirstName, "Anatoliy");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[3].LastName, "Kowood");
            Assert.AreEqual(result.ToArray<EmployeeViewModel>()[4].LastName, "Manowens");
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[5].LastName);
            Assert.AreEqual("Only", result.ToArray<EmployeeViewModel>()[7].LastName);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        [Test]
        public void GetEmployeeData_NonExistingDepartment_NoResult()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yy";
            string selectedDepartment = "assdsa";
            // Act
            IEnumerable<EmployeeViewModel> result = (IEnumerable<EmployeeViewModel>)controller.GetEmployeeData(selectedDepartment).Model;
            var view = controller.GetEmployeeData(selectedDepartment);


            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsInstanceOf(typeof(IEnumerable<EmployeeViewModel>), ((PartialViewResult)view).Model);
            Assert.IsTrue(result.ToArray<EmployeeViewModel>().Length == 0);
            Assert.IsTrue(view.ViewName == "");
            Assert.AreEqual(selectedDepartment, ((PartialViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual("dd.mm.yy", ((PartialViewResult)view).ViewBag.JSDatePattern);
        }

        #endregion

        [Test]
        public void DepartmentsDropDownList_Default_ListOfAllDepartments()
        {
            //Arrange

            EmployeeController controller = new EmployeeController(mock.Object);
            //Act
            var result = controller.DepartmentsDropDownList();

            //Asset
            Assert.IsInstanceOf(typeof(SelectList), result);
            Assert.IsTrue(result.ToArray().Length == 7);
            Assert.AreEqual("RAAA1", result.ToArray()[0].Text);
            Assert.AreEqual("RAAA4", result.ToArray()[3].Text);
            Assert.AreEqual("TAAAA", result.ToArray()[6].Text);
        }

        [Test]
        public void GetCreate_SelectedDepartmentNull_ViewCreateSelectedDepartmentNull()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = null;
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.Create(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void GetCreate_StringEmpty_ViewCreateSelectedDepartmentStringEmpty()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.Create(selectedDepartment);
            SelectList selectList = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, selectList.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, selectList.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, selectList.ToArray()[6].Value);
        }

        [Test]
        public void GetCalendarVU_selectedDepartmentEmpty_View()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "";

            //Act
            var result = controller.GetCalendarVU(selectedDepartment);
            SelectList select = ((ViewResult)result).ViewBag.DepartmentList as SelectList;

            //Assert
            Assert.IsTrue(((ViewResult)result).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.AreEqual(7, select.ToArray().Length);
            Assert.AreEqual("RAAA1", select.ToArray()[0].Value);
            Assert.AreEqual("TAAAA", select.ToArray()[6].Value);
        }


        [Test]
        public void GetCalendarVU_selectedDepartmentRAAA1_View()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "RAAA1";

            //Act
            var result = controller.GetCalendarVU(selectedDepartment);
            SelectList select = ((ViewResult)result).ViewBag.DepartmentList as SelectList;

            //Assert
            Assert.IsTrue(((ViewResult)result).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)result).ViewBag.SelectedDepartment);
            Assert.AreEqual(7, select.ToArray().Length);
        }

        [Test]
        public void GetCreate_RAAA1_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "RAAA1";
            SelectList departmentDropDownList = controller.DepartmentsDropDownList();

            // Act
            var view = controller.Create(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }
        #region DropDownListWithSelectedDepartment

        [Test]
        public void DropDownListWithSelectedDepartment_Default_ListOfAllDepartments()
        {
            //Arrange

            EmployeeController controller = new EmployeeController(mock.Object);
            //Act
            string selectedDepartment = "";
            var result = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            //Asset
            Assert.IsInstanceOf(typeof(SelectList), result);
            Assert.IsTrue(result.ToArray().Length == 7);
            Assert.AreEqual("RAAA1", result.ToArray()[0].Text);
            Assert.AreEqual("RAAA4", result.ToArray()[3].Text);
            Assert.AreEqual("TAAAA", result.ToArray()[6].Text);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_SelectedDepartmentNull_ViewCreateSelectedDepartmentNull()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = null;
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.Create(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_StringEmpty_ViewCreateSelectedDepartmentStringEmpty()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.Create(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_RAAA1_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "RAAA1";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);

            // Act
            var view = controller.Create(selectedDepartment);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;


            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_True_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "RAAA1";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            SelectListItem selectListItem3 = new SelectListItem { Text = "RAAA1", Value = "3", Selected = true };
            SelectListItem selectListItem4 = new SelectListItem { Text = "RAAA2", Value = "4", Selected = false };
            SelectListItem selectListItem5 = new SelectListItem { Text = "RAAA3", Value = "5", Selected = false };
            SelectListItem selectListItem6 = new SelectListItem { Text = "RAAA4", Value = "6", Selected = false };
            SelectListItem selectListItem7 = new SelectListItem { Text = "RAAA5", Value = "7", Selected = false };
            SelectListItem selectListItem1 = new SelectListItem { Text = "SDDDA", Value = "1", Selected = false };
            SelectListItem selectListItem2 = new SelectListItem { Text = "TAAAA", Value = "2", Selected = false };
            selectListItems.Add(selectListItem3);
            selectListItems.Add(selectListItem4);
            selectListItems.Add(selectListItem5);
            selectListItems.Add(selectListItem6);
            selectListItems.Add(selectListItem7);
            selectListItems.Add(selectListItem1);
            selectListItems.Add(selectListItem2);

            // Act
            var view = controller.Create(selectedDepartment);

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, selectListItems.Count());
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, selectListItems[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, selectListItems[6].Value);
        }

        [Test]
        public void DropDownListWithSelectedDepartment_SelectedFalse_ViewCreateSelectedDepartmentRAAA1()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "";
            SelectList departmentDropDownList = controller.DropDownListWithSelectedDepartment(selectedDepartment);
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            SelectListItem selectListItem3 = new SelectListItem { Text = "RAAA1", Value = "3", Selected = true };
            SelectListItem selectListItem4 = new SelectListItem { Text = "RAAA2", Value = "4", Selected = false };
            SelectListItem selectListItem5 = new SelectListItem { Text = "RAAA3", Value = "5", Selected = false };
            SelectListItem selectListItem6 = new SelectListItem { Text = "RAAA4", Value = "6", Selected = false };
            SelectListItem selectListItem7 = new SelectListItem { Text = "RAAA5", Value = "7", Selected = false };
            SelectListItem selectListItem1 = new SelectListItem { Text = "SDDDA", Value = "1", Selected = false };
            SelectListItem selectListItem2 = new SelectListItem { Text = "TAAAA", Value = "2", Selected = false };
            selectListItems.Add(selectListItem3);
            selectListItems.Add(selectListItem4);
            selectListItems.Add(selectListItem5);
            selectListItems.Add(selectListItem6);
            selectListItems.Add(selectListItem7);
            selectListItems.Add(selectListItem1);
            selectListItems.Add(selectListItem2);

            // Act
            var view = controller.Create(selectedDepartment);

            // Assert
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, selectListItems.Count());
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, selectListItems[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, selectListItems[6].Value);
        }

#endregion
        #region CreatePost
        [Test]
        public void PostCreate_NotValidModelSelectedDepartmentNull_ViewCreate()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            controller.ModelState.AddModelError("DepartmentID", "Field DepartmentID must be not null");
            Employee emp = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            EmployeeViewModel employee = new EmployeeViewModel(emp);
            SelectList departmentDropDownList = controller.DepartmentsDropDownList();

            // Act
            var view = controller.Create(emp);
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never());
            Assert.IsTrue(view != null);
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), ((ViewResult)view).Model);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);
        }

        [Test]
        public void PostCreate_NotValidModelSelectedDepartmentRAAA1_ViewCreate()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            controller.ModelState.AddModelError("DepartmentID", "Field DepartmentID must be not null");
            Employee emp = (from e in mock.Object.Employees where e.EmployeeID == 1 select e).FirstOrDefault();
            EmployeeViewModel employee = new EmployeeViewModel(emp);

            SelectList departmentDropDownList = controller.DepartmentsDropDownList();

            // Act
            var view = controller.Create(emp, null, "RAAA1");
            SelectList select = ((ViewResult)view).ViewBag.DepartmentsList as SelectList;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never());
            Assert.IsTrue(view != null);
            Assert.IsTrue(((ViewResult)view).ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), ((ViewResult)view).Model);
            Assert.AreEqual(departmentDropDownList.ToArray().Length, select.ToArray().Length);
            Assert.AreEqual(departmentDropDownList.ToArray()[0].Value, select.ToArray()[0].Value);
            Assert.AreEqual(departmentDropDownList.ToArray()[6].Value, select.ToArray()[6].Value);

        }

        [Test]
        public void PostCreate_ValidModelSelectedDepartmentNull_ViewAllEmployees()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = null;

            // Act
            var view = controller.Create(employee, null, selectedDepartment) as RedirectToRouteResult;

            // Assert
            mock.Verify(m => m.SaveEmployee(employee));
            Assert.IsFalse(view.Permanent);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual("PUView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual(1, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);
            Assert.IsNotInstanceOf(typeof(ViewResult), view);
        }

        [Test]
        public void PostCreate_ValidModelSelectedDepartmentEmptyString_ViewAllEmployees()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "";
            MvcApplication.JSDatePattern = "dd.mm.yy";

            // Act
            var view = controller.Create(employee, null, selectedDepartment) as RedirectToRouteResult;

            // Assert
            mock.Verify(m => m.SaveEmployee(employee));
            Assert.IsFalse(view.Permanent);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual("PUView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual(1, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual("", ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);
            Assert.IsNotInstanceOf(typeof(ViewResult), view);
        }

        [Test]
        public void PostCreate_ValidModelSelectedDepartmentRAAA1_ViewRAAA1Employees()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "RAAA1";

            // Act
            var view = controller.Create(employee, null, selectedDepartment) as RedirectToRouteResult;

            // Assert
            mock.Verify(m => m.SaveEmployee(employee));
            Assert.IsFalse(view.Permanent);
            Assert.AreEqual("Home", ((RedirectToRouteResult)view).RouteValues["controller"]);
            Assert.AreEqual("PUView", ((RedirectToRouteResult)view).RouteValues["action"]);
            Assert.AreEqual(null, ((RedirectToRouteResult)view).RouteValues["id"]);
            Assert.AreEqual(1, ((RedirectToRouteResult)view).RouteValues["tab"]);
            Assert.AreEqual("RAAA1", ((RedirectToRouteResult)view).RouteValues["selectedDepartment"]);
            Assert.IsNotInstanceOf(typeof(ViewResult), view);
        }

        #endregion

        #region EditGet

        [Test]
        public void GetEdit_ExistingEmployeeSelectedDepartmentNull_ViewEditSelectedDepartmentNull()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = null;
            var expectedDepartmentList = (from d in mock.Object.Departments select d).ToList();
            // Act
            var view = controller.Edit(2, selectedDepartment) as ViewResult;
            EmployeeViewModel employee = view.ViewData.Model as EmployeeViewModel;

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), view.Model);
            Assert.IsTrue(employee.EmployeeID == 2);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
        }

        [Test]
        public void GetEdit_ExistingEmployeeSelectedDepartmentStringEmpty_ViewEditSelectedDepartmentStringEmpty()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "";
            var expectedDepartmentList = (from d in mock.Object.Departments select d).ToList();
            // Act
            var view = controller.Edit(2, selectedDepartment) as ViewResult;
            EmployeeViewModel employee = view.ViewData.Model as EmployeeViewModel;

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), view.Model);
            Assert.IsTrue(employee.EmployeeID == 2);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
        }

        [Test]
        public void GetEdit_ExistingEmployeeSelectedDepartmentTAAAA_ViewEditSelectedDepartmentTAAAA()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "TAAAA";
            var expectedDepartmentList = (from d in mock.Object.Departments select d).ToList();
            // Act
            var view = controller.Edit(2, selectedDepartment) as ViewResult;
            EmployeeViewModel employee = view.ViewData.Model as EmployeeViewModel;

            // Assert
            Assert.IsTrue(view.ViewName == "");
            Assert.IsInstanceOf(typeof(EmployeeViewModel), view.Model);
            Assert.IsTrue(employee.EmployeeID == 2);
            Assert.AreEqual(selectedDepartment, ((ViewResult)view).ViewBag.SelectedDepartment);
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
        }

        [Test]
        public void GetEdit_NotExistingEmployee_Error()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);

            // Act
            HttpNotFoundResult result = controller.Edit(1000) as HttpNotFoundResult;

            // Assert
            Assert.IsTrue(result.StatusCode == 404);
        }

        #endregion


        [Test]
        public void PostEdit_ValidModelSelectedDepartmentNull_ViewAllEmployee()
        {
            EmployeeController controller = new EmployeeController(mock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = null;
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            SelectList departmentList = (from d in mock.Object.Departments select d) as SelectList;
            // Act
            var view = controller.Edit(employee, null, selectedDepartment);

            // Assert
            mock.Verify(m => m.SaveEmployee(employee), Times.Once);
            mock.Verify(m => m.SaveRolesForEmployee(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            Assert.IsTrue(((ViewResult)view).ViewName == "OneRowPU");
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(ViewResult), view.GetType());
        }

        [Test]
        public void PostEdit_ValidModelSelectedDepartmentStringEmpty_ViewAllEmployee()
        {
            EmployeeController controller = new EmployeeController(mock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "";
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            SelectList departmentList = (from d in mock.Object.Departments select d) as SelectList;
            // Act
            var view = controller.Edit(employee, null, selectedDepartment);

            // Assert
            mock.Verify(m => m.SaveEmployee(employee), Times.Once);
            mock.Verify(m => m.SaveRolesForEmployee(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            Assert.IsTrue(((ViewResult)view).ViewName == "OneRowPU");
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(ViewResult), view.GetType());
        }

        [Test]
        public void PostEdit_ValidModelSelectedDepartmentRAAA1_ViewAllEmployee()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "RAAA1";
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            SelectList departmentList = (from d in mock.Object.Departments select d) as SelectList;
            // Act
            var view = controller.Edit(employee, null, selectedDepartment);

            // Assert
            mock.Verify(m => m.SaveEmployee(employee), Times.Once);
            mock.Verify(m => m.SaveRolesForEmployee(It.IsAny<string>(), It.IsAny<string[]>()), Times.Once);
            Assert.IsTrue(((ViewResult)view).ViewName == "OneRowPU");
            Assert.AreEqual(expectedDepartmentList, ((ViewResult)view).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(ViewResult), view.GetType());

        }

        [Test]
        public void PostEdit_NotValidModelSelectedDepartmentNull_ViewEdit()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            controller.ModelState.AddModelError("DateEmployed", "Field DateEmployed must be not null");
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = null;
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            MvcApplication.JSDatePattern = "dd.mm.yy";

            // Act
            JsonResult result = (JsonResult)controller.Edit(employee, null, selectedDepartment);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never);
            Assert.IsTrue(result != null);
            //Assert.IsTrue(((ViewResult)result).ViewName == "");
            //Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
            //Assert.AreEqual(expectedDepartmentList, ((ViewResult)result).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("", data);
        }

        [Test]
        public void PostEdit_NotValidModelSelectedDepartmentStringEmpty_ViewEdit()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            controller.ModelState.AddModelError("DateEmployed", "Field DateEmployed must be not null");
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "";
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            MvcApplication.JSDatePattern = "dd.mm.yy";

            // Act
            JsonResult result = (JsonResult)controller.Edit(employee, null, selectedDepartment);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never);
            Assert.IsTrue(result != null);
            //Assert.IsTrue(((ViewResult)result).ViewName == "");
            //Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
            //Assert.AreEqual(expectedDepartmentList, ((ViewResult)result).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("", data);
        }

        [Test]
        public void PostEdit_NotValidModelSelectedDepartmentRAAA5_ViewEdit()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            controller.ModelState.AddModelError("DateEmployed", "Field DateEmployed must be not null");
            Employee employee = new Employee { EmployeeID = 1, FirstName = "Anastasia", LastName = "Zarose", DepartmentID = 1, DateDismissed = new DateTime(11 / 01 / 2013), DateEmployed = new DateTime(11 / 02 / 2011), IsManager = false };
            string selectedDepartment = "RAAA5";
            var expectedDepartmentList = (from m in mock.Object.Departments select m).ToList();
            MvcApplication.JSDatePattern = "dd.mm.yy";

            // Act
            JsonResult result = (JsonResult)controller.Edit(employee, null, selectedDepartment);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            // Assert
            mock.Verify(m => m.SaveEmployee(It.IsAny<Employee>()), Times.Never);
            Assert.IsTrue(result != null);
            //Assert.IsTrue(((ViewResult)result).ViewName == "");
            //Assert.AreEqual("dd.mm.yy", ((ViewResult)result).ViewBag.JSDatePattern);
            //Assert.AreEqual(expectedDepartmentList, ((ViewResult)result).ViewBag.DepartmentList);
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual("", data);
        }

        [Test]
        public void PostEdit_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            mock.Setup(m => m.SaveEmployee(It.IsAny<Employee>())).Throws(new DbUpdateConcurrencyException());
            EmployeeController controller = new EmployeeController(mock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";

            //Act
            JsonResult result = (JsonResult)controller.Edit(mock.Object.Employees.FirstOrDefault());
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(result.Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveEmployee(It.IsAny<Employee>()), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        [Test]
        public void GetDelete_EmpWithoutAssociatedDataDepartmentnull_ViewConfrimDeleteSelectedDepartmentnull()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = null;

            // Act
            ViewResult result = controller.Delete(15, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.AreEqual(result.ViewName, "");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.IsInstanceOf(typeof(Employee), result.Model);
            Assert.IsTrue(employee.EmployeeID == 15);
        }

        [Test]
        public void GetDelete_EmpWithBTDepartmentSDDDA_ViewCannotDeleteDelete()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "SDDDA";

            // Act
            ViewResult result = controller.Delete(1, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "CannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);

        }

        [Test]
        public void GetDelete_WithVisaRegDateSelectedDepartmentRAAA3_ViewCannotDeleteDelete()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "RAAA3";

            // Act
            ViewResult result = controller.Delete(6, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "CannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetDelete_WithVisaRegDateAndVisaAndBTSelectedDepartmentRAAA3_ViewCannotDeleteDelete()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "SDDDA";

            // Act
            ViewResult result = controller.Delete(3, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "CannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetDelete_WithVisaAndBTSelectedDepartmentRAAA3_ViewCannotDeleteDelete()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "RAAA3";

            // Act
            ViewResult result = controller.Delete(7, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "CannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetDelete_WithPermitSelectedDepartmentRAAA1_ViewCannotDeleteDelete()
        {
            // Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = "RAAA1";

            // Act
            ViewResult result = controller.Delete(5, selectedDepartment) as ViewResult;
            Employee employee = result.Model as Employee;

            // Assert
            Assert.IsTrue(result.ViewName == "CannotDelete");
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
            Assert.AreEqual(selectedDepartment, result.ViewBag.SelectedDepartment);
        }

        [Test]
        public void GetDelete_IncorrectId_404NotFound()
        {
            // Arrange 
            EmployeeController controller = new EmployeeController(mock.Object);
            // Act 
            HttpNotFoundResult result = (HttpNotFoundResult)controller.Delete(1000);
            // Assert 
            Assert.IsTrue(result.StatusCode == 404);
        }

        [Test]
        public void DeletePost_IdSelectedDepartmentNull_RedirectToPUView()
        {
            // Arrange
            //List<Employee> empList = mock.Object.Employees.ToList();
            EmployeeController controller = new EmployeeController(mock.Object);
            string selectedDepartment = null;

            // Act
            var result = controller.DeleteConfirmed(3, selectedDepartment);


            // Assert
            mock.Verify(m => m.DeleteEmployee(3), Times.Once);
            Assert.AreEqual(((ViewResult)result).GetType(), typeof(ViewResult));
            Assert.AreEqual(((ViewResult)result).ViewName, "OneRowPU");
        }

        [Test]
        public void DeletePost_CannotDelete_DataBaseDeleteError()
        {
            // Arrange - create the controller
            EmployeeController controller = new EmployeeController(mock.Object);
            mock.Setup(x => x.DeleteEmployee(It.IsAny<int>()))
                .Callback(() => { throw new System.Data.Entity.Infrastructure.DbUpdateException(); });


            // Act - call the action method
            RedirectToRouteResult result = (RedirectToRouteResult)controller.DeleteConfirmed(2);

            // Assert - check the result
            mock.Verify(m => m.DeleteEmployee(2), Times.Once);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("DataBaseDeleteError", result.RouteValues["action"]);

        }

        [Test]
        public void SearchEmployeeDataEmployee_NotEmptyDepartmentEmptySearchStringEmpty_AllEmployees()
        {
            //Arrange
            List<Employee> empList = mock.Object.Employees.ToList();
            EmployeeController controller = new EmployeeController(mock.Object);
            //Act
            List<EmployeeViewModel> result = controller.SearchEmployeeData(empList, "", "");
            //Assert
            Assert.AreEqual(24, result.Count());
            Assert.AreEqual(8, result.First().EmployeeID);
            Assert.AreEqual(1, result.Last().EmployeeID);
        }


        //[Test]
        //public void SearchEmployeeDataEmployee_SDDDADepartmentEmptySearchStringASOHNFOI_EmptyEmployeesList()
        //{
        //    //Arrange
        //    List<Employee> empList = mock.Object.Employees.ToList();
        //    EmployeeController controller = new EmployeeController(mock.Object);
        //    //Act
        //    List<EmployeeViewModel> result = controller.SearchEmployeeData(empList, "", "ASOHNFOI");
        //    //Assert
        //    Assert.AreEqual(0, result.Count());
        //}


        //[Test]
        //public void SearchEmployeeDataEmployee_SDDDADepartmentEmptySearchStringAN_EmptyEmployeesList()
        //{
        //    //Arrange
        //    List<Employee> empList = mock.Object.Employees.ToList();
        //    EmployeeController controller = new EmployeeController(mock.Object);
        //    //Act
        //    List<EmployeeViewModel> result = controller.SearchEmployeeData(empList, "SDDDA", "AN");
        //    //Assert
        //    Assert.AreEqual(1, result.Count());
        //    Assert.AreEqual(7, result.First().EmployeeID);

        //}

        [Test]
        public void ResetPasswordGET_AllParametersNotNull_View()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string EID = "andl";
            string[] Roles = { "ADM", "PU" };
            //Act
            var result = controller.ResetPassword(EID, Roles);
            //Assert
            Assert.AreEqual(result.GetType(), typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "");
        }

        [Test]
        public void ResetPasswordGET_RolesIsNull_View()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string EID = "andl";
            string[] Roles = null;
            //Act
            var result = controller.ResetPassword(EID, Roles);
            //Assert
            Assert.AreEqual(result.GetType(), typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "");
        }

        [Test]
        public void ResetPasswordGET_EIDIsNull_View()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string EID = null;
            string[] Roles = { "ADM", "PU" };
            //Act
            var result = controller.ResetPassword(EID, Roles);
            //Assert
            Assert.AreEqual(result.GetType(), typeof(ViewResult));
            Assert.AreEqual(result.ViewName, "");
        }


        [Test]
        public void ResetPasswordConfirmPost_AllParametersNotNull_FormattedString()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string EID = "andl";
            string[] Roles = { "ADM", "PU" };
            //Act
            var result = controller.ResetPasswordConfirmed(EID, Roles);
            //Assert
            mock.Verify(m => m.SaveRolesForEmployee("andl", new string[] { "ADM", "PU" }), Times.Exactly(1));
            mock.Verify(m => m.SaveRolesForEmployee(EID, null), Times.Exactly(1));
            Assert.AreEqual(result.GetType(), typeof(string));
            Assert.AreEqual(result, "Password has been changed");
        }

        [Test]
        public void ResetPasswordConfirmPost_EIDIsNull_FormattedString()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string EID = null;
            string[] Roles = { "ADM", "PU" };
            //Act
            var result = controller.ResetPasswordConfirmed(EID, Roles);
            //Assert
            mock.Verify(m => m.SaveRolesForEmployee(EID, Roles), Times.Exactly(1));
            mock.Verify(m => m.SaveRolesForEmployee(EID, null), Times.Exactly(1));
            Assert.AreEqual(result.GetType(), typeof(string));
            Assert.AreEqual(result, "Password has been changed");
        }

        [Test]
        public void ResetPasswordConfirmPost_RolesIsNull_FormattedString()
        {
            //Arrange
            EmployeeController controller = new EmployeeController(mock.Object);
            string EID = "andl";
            string[] Roles = null;
            //Act
            var result = controller.ResetPasswordConfirmed(EID, Roles);
            //Assert
            mock.Verify(m => m.SaveRolesForEmployee(EID, Roles), Times.Exactly(2));
            Assert.AreEqual(result.GetType(), typeof(string));
            Assert.AreEqual(result, "Password has been changed");
        }
    }

}



