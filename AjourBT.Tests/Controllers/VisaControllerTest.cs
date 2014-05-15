using System;
using NUnit.Framework;
using AjourBT.Domain.Abstract;
using Moq;
using AjourBT.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using AjourBT.Controllers;
using System.Web.Mvc;
using AjourBT.Models;
using System.Data.Entity.Infrastructure;
using AjourBT.Tests.MockRepository;
using ExcelLibrary.SpreadSheet;

namespace AjourBT.Tests.Controllers
{
    [TestFixture]
    public class VisaControllerTest
    {
        Mock<IRepository> mock;

        [SetUp]
        public void SetupMock()
        {
            mock = Mock_Repository.CreateMock();
        }

        #region SearchVisaData
    [Test]
	public void SearchVisaData_ListNotEmptySearchStringEmpty_AllEmployees()
	{
		//Arrange
        VisaController controller = new VisaController(mock.Object);
        List<Employee> empList = mock.Object.Employees.ToList();
        List<Employee> resultList = new List<Employee>();
        string searchString = "";
        //Act
        resultList = controller.SearchVisaData(empList, searchString);
        //Assert
        Assert.AreEqual(empList.Count, resultList.Count);
        Assert.AreEqual("Oleksiy", resultList.ToArray()[0].FirstName);
        Assert.AreEqual("Kowwood", resultList.ToArray()[0].LastName);
        Assert.AreEqual("Tanya", resultList.ToArray()[3].FirstName);
        Assert.AreEqual("Kowood", resultList.ToArray()[3].LastName);
        Assert.AreEqual("UPaidVac", resultList.ToArray()[7].FirstName);
        Assert.AreEqual("Only", resultList.ToArray()[7].LastName);
	}
     [Test]
    public void SearchVisaData_ListEmptySearchStringEmpty_AllEmployees()
    {
        //Arrange
        VisaController controller = new VisaController(mock.Object);
        List<Employee> empList = new List<Employee>();
        List<Employee> resultList = new List<Employee>();
        string searchString = "";
        //Act
        resultList = controller.SearchVisaData(empList, searchString);
        //Assert
        Assert.AreEqual(0, resultList.Count);

    }
    [Test]
    public void SearchVisaData_ListNotEmptySearchStringAnd_AllEmployees()
    {
        //Arrange
        VisaController controller = new VisaController(mock.Object);
        List<Employee> empList = mock.Object.Employees.ToList();
        List<Employee> resultList = new List<Employee>();
        string searchString = "And";
        //Act
        resultList = controller.SearchVisaData(empList, searchString);
        //Assert
        Assert.AreEqual(1, resultList.Count);
        Assert.AreEqual("Anastasia", resultList.ToArray()[0].FirstName);
        Assert.AreEqual("Zarose", resultList.ToArray()[0].LastName);

    }

	
        #endregion

        #region GetVisaADM
        [Test]
        public void GetVisaADM_AllEmployeesInUserDepartment_TAAAA()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            string userName = "ascr";
            //Act
            var resultView = controller.GetVisaADM(userName);
            //Assert        
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.IsInstanceOf(typeof(string), ((ViewResult)resultView).Model);
            Assert.AreEqual("TAAAA", ((ViewResult)resultView).Model);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)resultView).ViewBag.SelectedDepartment.Items);
        }

        [Test]
        public void GetVisaADM_AllEmployeesInUserDepartment_SDDDA()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);
            IEnumerable<Department> departmentsList = from d in mock.Object.Departments
                                                      orderby d.DepartmentName
                                                      select d;
            string userName = "andl";
            //Act
            var resultView = controller.GetVisaADM(userName);
            //Assert        
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.IsInstanceOf(typeof(string), ((ViewResult)resultView).Model);
            Assert.AreEqual("SDDDA", ((ViewResult)resultView).Model);
            Assert.AreEqual(departmentsList.ToList(), ((ViewResult)resultView).ViewBag.SelectedDepartment.Items);
        }

        #endregion

        #region GetVisaDataADM
        [Test]
        public void GetVisaDataADM_AllEmployeesInUserDepartment_TAAAA()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);
            string departmentName = "";
            string userName = "ascr";
            string selectedUserDepartment = "TAAAA";

            // Act - call the action method
            PartialViewResult resultView = (PartialViewResult)controller.GetVisaDataADM(departmentName, userName, selectedUserDepartment);
            string selectedDepartment = departmentName;
          
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataADM("", "ascr", "TAAAA").Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", resultView.ViewName);
            Assert.IsTrue(employeeVisaView.Length == 1);
            Assert.AreEqual("Struz", employeeVisaView[0].LastName);
            Assert.AreEqual("Struz", employeeVisaView[0].LastName);

            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
        }

        [Test]
        public void GetVisaDataADM_AllEmployeesInUserDepartment_SDDDA()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method

            PartialViewResult resultView = (PartialViewResult)controller.GetVisaDataADM("", "zdul", "SDDDA");
            
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataADM("", "zdul", "SDDDA").Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", resultView.ViewName);
            Assert.AreEqual(4, employeeVisaView.Length);
            Assert.AreEqual("Kowwood", employeeVisaView[0].LastName);
        }

        [Test]
        public void GetVisaDataADM_AllEmployees()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            PartialViewResult resultView = (PartialViewResult)controller.GetVisaDataADM("", "", "");

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataADM("", "", "").Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", resultView.ViewName);
            Assert.AreEqual(23, employeeVisaView.Length);
            //Assert.AreEqual("Zdvigkova", employeeVisaView[0].LastName);

            //Assert.AreEqual("Tymur", employeeVisaView[1].FirstName);
        }
      

        [Test]
        public void GetVisaDataADM_andzAllEmployeesInUserDepartment_SDDDA()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            PartialViewResult resultView = (PartialViewResult)controller.GetVisaDataADM("", "andl", "SDDDA");

            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataADM("", "andl", "SDDDA").Model;
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", resultView.ViewName);
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.AreEqual(employeeVisaView[0].LastName, "Kowwood");
            Assert.AreEqual(employeeVisaView[0].EID, "xomi");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Oleksiy");
            Assert.AreEqual(4, employeeVisaView.Length);
            Assert.AreEqual("Petrenko",employeeVisaView[1].LastName);
            Assert.AreEqual("chap", employeeVisaView[1].EID);
            Assert.AreEqual("Chuck", employeeVisaView[1].FirstName);
        }
        #endregion

        #region GetVisaBTM

        [Test]
        public void GetVisaBTM_AllEmployees()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);

            //Act
            var resultView = controller.GetVisaBTM("");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("", ((ViewResult)resultView).ViewBag.SearchString);
        }

        [Test]
        public void GetVisaBTM_EmployeesContain_Te()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);

            //Act
            var resultView = controller.GetVisaBTM("Te");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("Te", ((ViewResult)resultView).ViewBag.SearchString);
        }

        [Test]
        public void GetVisaBTM_EmployeesContain_qq()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);

            //Act
            var resultView = controller.GetVisaBTM("qq");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("qq", ((ViewResult)resultView).ViewBag.SearchString);
        }

        #endregion

        #region GetVisaDataBTM
        [Test]
        public void GetVisaDataBTM_Default_AllEmployees()
        {     
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method


            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("").Model;
            var view = controller.GetVisaDataBTM("");


            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual("Oleksiy", result.ToArray()[0].FirstName);
            Assert.AreEqual("Kowwood", result.ToArray()[0].LastName);
            Assert.AreEqual("Tanya", result.ToArray()[3].FirstName);
            Assert.AreEqual("Kowood", result.ToArray()[3].LastName);
            Assert.AreEqual("UPaidVac", result.ToArray()[7].FirstName);
            Assert.AreEqual("Only", result.ToArray()[7].LastName);
        }


        [Test]
        public void GetVisaDataBTM_FilterTe_EmployeesContain_Te()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("Te").Model;
            var view = controller.GetVisaDataBTM("Te");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(employeeVisaView[0].LastName, "Pyorge");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Tymur");
            Assert.AreEqual(employeeVisaView[0].EID, "tedk");
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.IsTrue(employeeVisaView.Length == 3);
            Assert.AreEqual(employeeVisaView[1].LastName, "Teshaw");
            Assert.AreEqual(employeeVisaView[1].FirstName, "Boryslav");
            Assert.AreEqual(employeeVisaView[1].EID, "tebl");

        }
        [Test]
        public void GetVisaDataBTM_FilterTe0301_EmployeesContain_Te0301()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("Te").Model;
            var view = controller.GetVisaDataBTM("Tep");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(employeeVisaView[0].LastName, "Pyorge");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Tymur");
            Assert.AreEqual(employeeVisaView[0].EID, "tedk");
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.IsTrue(employeeVisaView.Length == 3);

        }

        [Test]
        public void GetVisaDataBTM_Filteraa_EmployeesContain_aa()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("aa").Model;
            var view = controller.GetVisaDataBTM("aa");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsTrue(employeeVisaView.Length == 0);

        }

        [Test]
        public void GetVisaDataBTM_FilterD0_EmployeesWithVisaTypeContainD0()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM(" D0  ").Model;
            var view = controller.GetVisaDataBTM(" D0  ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.IsTrue(employeeVisaView.Length == 1);
            Assert.AreEqual(employeeVisaView[0].LastName, "Zarose");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Anastasia");
            Assert.AreEqual(employeeVisaView[0].EID, "andl");

        }

        [Test]
        public void GetVisaDataBTM_Filter1082012_EmployeesWithVisaStartDateContain01082012()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("  8/1/2012").Model;
            var view = controller.GetVisaDataBTM("  8/1/2012");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.IsTrue(employeeVisaView.Length == 1);
            Assert.AreEqual(employeeVisaView[0].LastName, "Zarose");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Anastasia");
            Assert.AreEqual(employeeVisaView[0].EID, "andl");

        }

        [Test]
        public void GetVisaDataBTM_Filter13505_EmployeesWithVisaDueDateContain13505()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("  5/13 ").Model;
            var view = controller.GetVisaDataBTM("  5/13  ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);

            Assert.AreEqual(employeeVisaView.Length, 1);
            Assert.AreEqual(employeeVisaView[0].LastName, "Kowwood");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Oleksiy");
            Assert.AreEqual(employeeVisaView[0].EID, "xomi");


        }

        [Test]
        public void GetVisaDataBTM_FilterMULT_EmployeesWithVisaMULT()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM(" MULT").Model;
            var view = controller.GetVisaDataBTM(" MULT");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(employeeVisaView.Length, 9);
            Assert.AreEqual(employeeVisaView[0].LastName, "Kowwood");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Oleksiy");
            Assert.AreEqual(employeeVisaView[0].EID, "xomi");

        }

        [Test]
        public void GetVisaDataBTM_Filter0401_EmployeesWithVisaRegDate0401()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM(" 1/4/2013 ").Model;
            var view = controller.GetVisaDataBTM(" 1/4/2013 ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(employeeVisaView.Length,3);
            Assert.AreEqual(employeeVisaView[0].LastName, "Kowood");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Tanya");
            Assert.AreEqual(employeeVisaView[0].EID, "tadk");

        }

        [Test]
        public void GetVisaDataBTM_Filter01082012_EmployeesWithPermitStartDate01082012()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("8/1/2012 ").Model;
            var view = controller.GetVisaDataBTM("8/1 ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(1, employeeVisaView.Length);
            Assert.AreEqual("Zarose",employeeVisaView[0].LastName);
            Assert.AreEqual("Anastasia",employeeVisaView[0].FirstName);
            Assert.AreEqual("andl",employeeVisaView[0].EID);

        }

        [Test]
        public void GetVisaDataBTM_Filter08082014_EmployeesWithPermitEndDate08082014()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataBTM("8/8/2014 ").Model;
            var view = controller.GetVisaDataBTM("8/8/2014 ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(1, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].LastName, "Daolson");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Ivan");
            Assert.AreEqual(employeeVisaView[0].EID, "daol");

        }

        #endregion

        #region GetVisaVU

        [Test]
        public void GetVisaVU_AllEmployees()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);

            //Act
            var resultView = controller.GetVisaVU("");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("", ((ViewResult)resultView).ViewBag.SearchString);
        }

        [Test]
        public void GetVisaVU_EmployeesContain_Te()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);

            //Act
            var resultView = controller.GetVisaVU("Te");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("Te", ((ViewResult)resultView).ViewBag.SearchString);
        }

        [Test]
        public void GetVisaVU_EmployeesContain_qq()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);

            //Act
            var resultView = controller.GetVisaVU("qq");

            //Assert        
            Assert.IsInstanceOf(typeof(ViewResult), resultView);
            Assert.AreEqual("", ((ViewResult)resultView).ViewName);
            Assert.AreEqual("qq", ((ViewResult)resultView).ViewBag.SearchString);
        }

        #endregion

        #region GetVisaDataVU
        [Test]
        public void GetVisaDataVU_Default_AllEmployees()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method


            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU("").Model;
            var view = controller.GetVisaDataVU("");


            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", ((PartialViewResult)view).ViewName);
            Assert.AreEqual("Oleksiy", result.ToArray()[0].FirstName);
            Assert.AreEqual("Kowwood", result.ToArray()[0].LastName);
            Assert.AreEqual("Tanya", result.ToArray()[3].FirstName);
            Assert.AreEqual("Kowood", result.ToArray()[3].LastName);
            Assert.AreEqual("UPaidVac", result.ToArray()[7].FirstName);
            Assert.AreEqual("Only", result.ToArray()[7].LastName);
        }


        [Test]
        public void GetVisaDataVU_FilterTe_EmployeesContain_Te()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU("Te").Model;
            var view = controller.GetVisaDataVU("Te");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(employeeVisaView[0].LastName, "Pyorge");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Tymur");
            Assert.AreEqual(employeeVisaView[0].EID, "tedk");
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.IsTrue(employeeVisaView.Length == 3);
            Assert.AreEqual(employeeVisaView[1].LastName, "Teshaw");
            Assert.AreEqual(employeeVisaView[1].FirstName, "Boryslav");
            Assert.AreEqual(employeeVisaView[1].EID, "tebl");

        }
        [Test]
        public void GetVisaDataVU_FilterTe0301_EmployeesContain_Te0301()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU("Te").Model;
            var view = controller.GetVisaDataVU("Tep");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(employeeVisaView[0].LastName, "Pyorge");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Tymur");
            Assert.AreEqual(employeeVisaView[0].EID, "tedk");
            CollectionAssert.AllItemsAreInstancesOfType(result, typeof(Employee));
            Assert.IsTrue(employeeVisaView.Length == 3);

        }

        [Test]
        public void GetVisaDataVU_Filteraa_EmployeesContain_aa()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU("aa").Model;
            var view = controller.GetVisaDataVU("aa");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.AreEqual("", view.ViewName);
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.IsTrue(employeeVisaView.Length == 0);

        }

        [Test]
        public void GetVisaDataVU_FilterD0_EmployeesWithVisaTypeContainD0()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU(" D0  ").Model;
            var view = controller.GetVisaDataVU(" D0  ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.IsTrue(employeeVisaView.Length == 1);
            Assert.AreEqual(employeeVisaView[0].LastName, "Zarose");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Anastasia");
            Assert.AreEqual(employeeVisaView[0].EID, "andl");

        }

        [Test]
        public void GetVisaDataVU_Filter1082012_EmployeesWithVisaStartDateContain01082012()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU("  8/1/2012").Model;
            var view = controller.GetVisaDataVU("  8/1/2012");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.IsTrue(employeeVisaView.Length == 1);
            Assert.AreEqual(employeeVisaView[0].LastName, "Zarose");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Anastasia");
            Assert.AreEqual(employeeVisaView[0].EID, "andl");

        }

        [Test]
        public void GetVisaDataVU_Filter13505_EmployeesWithVisaDueDateContain13505()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU("  5/13  ").Model;
            var view = controller.GetVisaDataVU("  5/13  ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.IsTrue(employeeVisaView.Length == 1);
            Assert.AreEqual(employeeVisaView[0].LastName, "Kowwood");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Oleksiy");
            Assert.AreEqual(employeeVisaView[0].EID, "xomi");
 
        }

        [Test]
        public void GetVisaDataVU_FilterMULT_EmployeesWithVisaMULT()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU(" MULT").Model;
            var view = controller.GetVisaDataVU(" MULT");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(employeeVisaView.Length, 9);
            Assert.AreEqual(employeeVisaView[0].LastName, "Kowwood");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Oleksiy");
            Assert.AreEqual(employeeVisaView[0].EID, "xomi");

        }

        [Test]
        public void GetVisaDataVU_Filter0401_EmployeesWithVisaRegDate0401()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU(" 1/4/2013 ").Model;
            var view = controller.GetVisaDataVU(" 1/4/2013 ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(employeeVisaView.Length, 3);
            Assert.AreEqual(employeeVisaView[0].LastName, "Kowood");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Tanya");
            Assert.AreEqual(employeeVisaView[0].EID, "tadk");

        }

        [Test]
        public void GetVisaDataVU_Filter01082012_EmployeesWithPermitStartDate01082012()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU("8/1/2012 ").Model;
            var view = controller.GetVisaDataVU("8/1/2012 ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(1, employeeVisaView.Length);
            Assert.AreEqual("Zarose", employeeVisaView[0].LastName);
            Assert.AreEqual("Anastasia", employeeVisaView[0].FirstName);
            Assert.AreEqual("andl", employeeVisaView[0].EID);

        }

        [Test]
        public void GetVisaDataVU_Filter08082014_EmployeesWithPermitEndDate08082014()
        {
            // Arrange - create the controller     
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method
            IEnumerable<Employee> result = (IEnumerable<Employee>)controller.GetVisaDataVU("8/8/2014 ").Model;
            var view = controller.GetVisaDataVU("8/8/2014 ");
            Employee[] employeeVisaView = result.ToArray();

            // Assert - check the result
            Assert.IsInstanceOf(typeof(PartialViewResult), view);
            Assert.AreEqual("", view.ViewName);
            Assert.AreEqual(1, employeeVisaView.Length);
            Assert.AreEqual(employeeVisaView[0].LastName, "Daolson");
            Assert.AreEqual(employeeVisaView[0].FirstName, "Ivan");
            Assert.AreEqual(employeeVisaView[0].EID, "daol");

        }

        #endregion

        #region CreateGet
        [Test]
        public void CreateGet_VisaOf_ExistingEmployeeAndDefault()
        {
            // Arrange - create the controller                 
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method 
            var result = controller.Create(3) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visa);
            Assert.AreEqual("", result.ViewBag.SearchString);
        }

        [Test]
        public void CreateGet_VisaOf_ExistingEmployeeAndNull()
        {
            // Arrange - create the controller                 
            VisaController controller = new VisaController(mock.Object);
            string searchString = null;

            // Act - call the action method 
            var result = controller.Create(3, searchString) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visa);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }


        [Test]
        public void CreateGet_VisaOf_ExistingEmployeeAnd_dan_()
        {
            // Arrange - create the controller                 
            VisaController controller = new VisaController(mock.Object);
            string searchString = "dan";

            // Act - call the action method 
            var result = controller.Create(3, searchString) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("Pyorge Tymur (tedk) from SDDDA", result.ViewBag.EmployeeInformation);
            Assert.IsNull(visa);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void CreateGet_VisaOf_NotExistingEmployee()
        {
            // Arrange - create the controller                 
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.Create(1500);
            Visa visa = mock.Object.Visas.Where(m => m.EmployeeID == 1500).FirstOrDefault();
            Employee employee = mock.Object.Employees.Where(m => m.EmployeeID == 1500).FirstOrDefault();

            // Assert - check the result 
            Assert.IsNull(visa);
            Assert.IsNull(employee);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
        }
        #endregion

        #region CreatePost
        [Test]
        public void CreatePost_CanCreate_ValidVisaSearchStringEmpty()
        {
            // Arrange - create the controller                 
            VisaController controller = new VisaController(mock.Object);
            Visa visa = new Visa { EmployeeID = 3, VisaType = "D08", StartDate = new DateTime(01 / 08 / 2012), DueDate = new DateTime(01 / 02 / 2013), Days = 90, Entries = 0, CorrectionForVisaDays = null, CorrectionForVisaEntries = null };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.Create(visa);
            Visa visaAfter = mock.Object.Visas.Where(v => v.EmployeeID == visa.EmployeeID).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, 3), Times.Once);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void CreatePost_CanCreate_ValidVisaSearchStringNotEmpty()
        {
            // Arrange - create the controller                 
            VisaController controller = new VisaController(mock.Object);
            Visa visa = new Visa { EmployeeID = 4, VisaType = "C08", StartDate = new DateTime(01 / 08 / 2012), DueDate = new DateTime(01 / 02 / 2013), Days = 90, Entries = 20, CorrectionForVisaDays = null, CorrectionForVisaEntries = null };
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.Create(visa,"b");
            Visa visaAfter = mock.Object.Visas.Where(v => v.EmployeeID == visa.EmployeeID).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, 4), Times.Once);
            Assert.AreEqual("b", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
        }

        [Test]
        public void CreatePost_CannotCreate_InvalidVisa()
        {
            // Arrange - create the controller
            VisaController controller = new VisaController(mock.Object);
            Visa visa = new Visa { EmployeeID = 2, VisaOf = mock.Object.Employees.Where(e => e.EmployeeID == 2).FirstOrDefault() };

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            ViewResult result = controller.Create(visa) as ViewResult;


            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(VisaViewModel), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void CreatePost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Visa visa = mock.Object.Visas.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveVisa(visa, visa.EmployeeID)).Throws(new InvalidOperationException());

            //Act
            var result = controller.Create(visa);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveVisa(visa, visa.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }


        #endregion

        #region EditGet

        [Test]
        public void EditGet_ValidEmployeeIDAndDefault_CanEdit()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method 
            var result = controller.Edit(2) as ViewResult;
            VisaViewModel visa = (VisaViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
            Assert.AreEqual("", result.ViewBag.SearchString);
        }

        [Test]
        public void EditGet_ValidEmployeeIDAndNull_CanEdit()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            string searchString = null;

            // Act - call the action method 
            var result = controller.Edit(2, searchString) as ViewResult;
            VisaViewModel visa = (VisaViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void EditGet_ValidEmployeeIDAnd_dan__CanEdit()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            string searchString = "dan";

            // Act - call the action method 
            var result = controller.Edit(2, searchString) as ViewResult;
            VisaViewModel visa = (VisaViewModel)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual("Struz Anatoliy (ascr) from TAAAA", result.ViewBag.EmployeeInformation);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void EditGet_InvalidEmployeeIDAndDefault_CannotEdit()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.Edit(15);
            Visa visa = mock.Object.Visas.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visa);
        }

        [Test]
        public void EditGet_InvalidEmployeeIDAndNull_CannotEdit()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            string searchString = null;

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.Edit(15, searchString);
            var viewResult = controller.Edit(15, searchString) as ViewResult;
            Visa visa = mock.Object.Visas.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visa);
        }


        #endregion

        #region EditPost

        [Test]
        public void EditPost_ValidVisaAndDefault_CanEdit()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);

            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            visa.CorrectionForVisaDays = 10;
            visa.CorrectionForVisaEntries = 1;
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.Edit(visa);
            Visa visaAfter = mock.Object.Visas.Where(v => v.EmployeeID == visa.EmployeeID).FirstOrDefault();

            // Assert - check the result 
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
            mock.Verify(m => m.SaveVisa(visa, 1), Times.Once);
            Assert.AreEqual(visaAfter.CorrectionForVisaEntries, 1);
            Assert.AreEqual(visaAfter.CorrectionForVisaDays, 10);
        }



        [Test]
        public void EditPost_ValidVisaAnd_dan_CanEdit()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            visa.CorrectionForVisaDays = 10;
            visa.CorrectionForVisaEntries = 1;
            string searchString = "dan";
            MvcApplication.JSDatePattern = "dd.mm.yyyy";


            // Act - call the action method 
            var result = controller.Edit(visa, searchString);
            Visa visaAfter = mock.Object.Visas.Where(v => v.EmployeeID == visa.EmployeeID).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, 1), Times.Once);
            Assert.AreEqual(visaAfter.CorrectionForVisaEntries, 1);
            Assert.AreEqual(visaAfter.CorrectionForVisaDays, 10);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("dan", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }



        [Test]
        public void EditPost_CannotEdit_InvalidVisa()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            Visa visa = mock.Object.Visas.Where(v => v.EmployeeID == 1).FirstOrDefault();
            visa.Days = 0;
            visa.Entries = 0;

            // Act - call the action method 
            controller.ModelState.AddModelError("error", "error");
            var result = controller.Edit(visa);
            

            // Assert - check the result 
            mock.Verify(m => m.SaveVisa(visa, 1), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.AreEqual("", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(VisaViewModel), ((ViewResult)result).Model);
        }

        [Test]
        public void EditPost_ValidModelConcurrency_ErrorReturned()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);
            string modelError = "The record you attempted to edit "
                  + "was modified by another user after you got the original value. The "
                  + "edit operation was canceled.";
            Visa visa = mock.Object.Visas.Where(p => p.EmployeeID == 1).FirstOrDefault();
            mock.Setup(m => m.SaveVisa(visa, visa.EmployeeID)).Throws(new DbUpdateConcurrencyException());

            //Act
            var result = controller.Edit(visa);
            string data = (string)(new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(((JsonResult)result).Data, "error")).Target;

            //Assert
            mock.Verify(d => d.SaveVisa(visa, visa.EmployeeID), Times.Once());
            Assert.AreEqual(typeof(JsonResult), result.GetType());
            Assert.AreEqual(modelError, data);
        }

        #endregion

        #region DeleteGet
        [Test]
        public void DeleteGet_ValidEmployeeIDAndDefault()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method 
            var result = controller.Delete(2) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Visa), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual("", result.ViewBag.SearchString);
        }

        [Test]
        public void DeleteGet_ValidEmployeeIDAndEmptyString()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            string searchString = "";

            // Act - call the action method 
            var result = controller.Delete(2, searchString) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Visa), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void DeleteGet_ValidEmployeeIDAnd_dan_()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            string searchString = "dan";

            // Act - call the action method 
            var result = controller.Delete(2, searchString) as ViewResult;
            Visa visa = (Visa)result.ViewData.Model;

            // Assert - check the result 
            Assert.AreEqual("", result.ViewName);
            Assert.IsInstanceOf(typeof(Visa), result.ViewData.Model);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.ViewData.Model);
            Assert.AreEqual(2, visa.EmployeeID);
            Assert.AreEqual(searchString, result.ViewBag.SearchString);
        }

        [Test]
        public void DeleteGet_InvalidEmployeeID()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);

            // Act - call the action method 
            var result = (HttpNotFoundResult)controller.Delete(15);
            Visa visa = mock.Object.Visas.Where(m => m.EmployeeID == 15).FirstOrDefault();

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisa(It.IsAny<int>()), Times.Never);
            Assert.IsInstanceOf(typeof(HttpNotFoundResult), result);
            Assert.AreEqual(404, result.StatusCode);
            Assert.IsNull(visa);
        }

        #endregion 

        #region DeleteConfirmed
        [Test]
        public void DeleteConfirmed_ValidVisa()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.DeleteConfirmed(1,"as");

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisa(1), Times.Once);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("as", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }
        [Test]
        public void DeleteConfirmed_ValidVisaSearchStringEmpty()
        {
            // Arrange - create the controller 
            VisaController controller = new VisaController(mock.Object);
            MvcApplication.JSDatePattern = "dd.mm.yyyy";

            // Act - call the action method 
            var result = controller.DeleteConfirmed(2);

            // Assert - check the result 
            mock.Verify(m => m.DeleteVisa(2), Times.Once);
            Assert.AreEqual("TableViewVisasAndPermitsBTM", ((ViewResult)result).ViewName);
            Assert.IsInstanceOf(typeof(List<Employee>), ((ViewResult)result).Model);
            Assert.AreEqual("", ((ViewResult)result).ViewBag.SearchString);
            Assert.AreEqual("dd.mm.yyyy", ((ViewResult)result).ViewBag.JSDatePattern);
        }
        #endregion

        #region Export VisasAndPermits into Excel File

        [Test]
        public void ExportVisasAndPermits_FileResult()
        {
            //Arrange 
            VisaController controller = new VisaController(mock.Object);

            //Act 
            FileResult file = controller.ExportVisasAndPermits("") as FileResult;

            //Assert 
            Assert.IsInstanceOf(typeof(FileResult), file);
        }

        [Test]
        public void CreateCaption_workSheet_ProperCaptionOnWorksheet()
        {
            //Arrange 
            VisaController controller = new VisaController(mock.Object);
            Worksheet workSheet = new Worksheet("VisasAndPermits");
            string[] caption = new string[] { "EID", "Name", "Passport", "Type", "Visa From", "Visa To", "Entries", "Days", "Registration", "Num", "Permit From - To", "Last BT", "Status" };

            //Act 
            controller.CreateCaption(workSheet);

            //Assert 
            for (int i = 0; i < caption.Length; i++)
            {
                Assert.AreEqual(caption[i], workSheet.Cells[0, i].Value);
            }
        }

        public void CreateCaption_Null_NullReferenceException()
        {
            //Arrange 
            VisaController controller = new VisaController(mock.Object);
            string[] caption = new string[] { "EID", "Name", "Passport", "Type", "Visa From", "Visa To", "Entries", "Days", "Registration", "Num", "Permit From - To", "Last BT", "Status" };

            //Act 

            //Assert 
            Assert.Throws<NullReferenceException>(() => controller.CreateCaption(null));
        }


        [Test]
        public void WriteVisasAndPermitData_WorksheetNull_Exception()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);

            //Act

            //Assert        
            Assert.Throws<NullReferenceException>(() => controller.WriteVisasAndPermitsData(null, ""));
        }


        [Test]
        public void WriteVisasAndPermitsData_WorksheetNotNull_Exception()
        {
            //Arrange
            VisaController controller = new VisaController(mock.Object);
            Worksheet workSheet = new Worksheet("New worksheet");
            string visaDate = DateTime.Now.AddDays(-10).ToString(String.Format("yyyy-MM-dd"));

            //Act
            controller.WriteVisasAndPermitsData(workSheet, "");

            //Assert        
            Assert.AreEqual("xomi", workSheet.Cells[1, 0].Value.ToString());
            Assert.AreEqual("Struz Anatoliy", workSheet.Cells[2, 1].Value.ToString());
            Assert.AreEqual("C07", workSheet.Cells[3, 3].Value.ToString());
            Assert.AreEqual("yes", workSheet.Cells[3, 2].Value.ToString());
            Assert.AreEqual("C07", workSheet.Cells[4, 3].Value.ToString());
            Assert.AreEqual(visaDate, workSheet.Cells[5, 4].Value.ToString());
            Assert.AreEqual("No Visa", workSheet.Cells[6, 5].Value.ToString());
            Assert.AreEqual("", workSheet.Cells[7, 6].Value.ToString());
            Assert.AreEqual("", workSheet.Cells[8, 7].Value.ToString());
            Assert.AreEqual("", workSheet.Cells[9, 8].Value.ToString());
            Assert.AreEqual("", workSheet.Cells[10, 8].Value.ToString());
            Assert.AreEqual("", workSheet.Cells[11, 8].Value.ToString());
            Assert.AreEqual("", workSheet.Cells[12, 8].Value.ToString());
        }

        #endregion

    }
}
