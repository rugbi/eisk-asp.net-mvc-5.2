﻿using Eisk.BusinessRules;
using Eisk.Controllers;
using Eisk.DataAccess;
using Eisk.Helpers;
using Eisk.Models;
using Microsoft.Practices.Unity;
using Moq;
using Xunit;

namespace Eisk.Tests
{
    public class EmployeesControllerMockTests
    {
        [Fact]
        public void Edit_Negative_Test_Post_Mock_Test()
        {
            //Arrange
            _employee = TestDataHelper.CreateEmployeeWithValidData();
            _employee.Id = 100;
            _fakeEmployeeDbSet.Add(_employee);

            _employee = TestDataHelper.CreateEmployeeWithValidData();
            _employee.Id = 200;
            _fakeEmployeeDbSet.Add(_employee);

            _mockDbContext.Setup(db => db.EmployeeRepository).Returns(_fakeEmployeeDbSet);

            //Act
            _employeeController.Validate(_employee);
            
            //Assert
            Assert.True(EmployeeAddressMustBeUnique.IsErrorAvalilableIn(_employeeController, _employee));
        }

        [Fact]
        public void UpdateEmployee_SupervisorsCountryIsNotSame_ShouldThrowException()
        {
            /************* Arrange: Setting up data containers ************************************************/

            //constants to be used in test containers (optional), for test readability

            const int SUPERVISOR_ID = 100;
            const string SUPERVISOR_COUNTRY = "USA";
            const int SUBORDINATE_ID = 200;
            const string SUBORDINATE_DIFFERENT_COUNTRY = "UK";

            //creating sample data for other employee, that would contain the different COUNTRY of the employee

            var supervisorEmployee = TestDataHelper.CreateEmployeeWithValidData();
            supervisorEmployee.Id = SUPERVISOR_ID;
            supervisorEmployee.Address.Country = SUPERVISOR_COUNTRY;

            _employee = TestDataHelper.CreateEmployeeWithValidData();
            _employee.Id = SUBORDINATE_ID;
            _employee.Address.Country = SUBORDINATE_DIFFERENT_COUNTRY;
            _employee.ReportsTo = SUPERVISOR_ID;

            /************* Arrange: Setting up mock test ************************************************/

            //populating sample data to ObjectSet container that would be considered as the data source in mock database for employee entities
            _fakeEmployeeDbSet.Add(supervisorEmployee);
            _fakeEmployeeDbSet.Add(_employee);

            //setting up the mock database with sample object
            _mockDbContext.Setup(db => db.EmployeeRepository).Returns(_fakeEmployeeDbSet);

            /************* Action ************************************************/
            _employeeController.Validate(_employee);
            
            /************* Assert ************************************************/
            Assert.True(SupervisorCountryMustBeSame.IsErrorAvalilableIn(_employeeController));
        }

        #region Test Assets

        private readonly Mock<DatabaseContext> _mockDbContext;
        private Employee _employee;
        private readonly EmployeesController _employeeController;
        private readonly FakeEmployeeSet _fakeEmployeeDbSet;

        public EmployeesControllerMockTests()
        {
            DependencyHelper.Initialize();
            var container = DependencyHelper.Container;
            _mockDbContext = new Mock<DatabaseContext>();
            container.RegisterInstance(_mockDbContext.Object);
            _employeeController = container.Resolve<EmployeesController>();
            _fakeEmployeeDbSet = new FakeEmployeeSet();
        }

        ~EmployeesControllerMockTests()
        {
            DependencyHelper.ClearContainer();
        }

        #endregion
    }
}