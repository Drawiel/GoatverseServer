﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using GoatverseService;
using System.Linq;
using DataAccess;
using Moq;
using System.Numerics;
using System.Data.Entity;
using System.Collections.Generic;
using DataAccess.DAOs;

namespace Server.Tests {

    [TestClass]
    public class ServiceImplementationTest {

        private UsersDAO usersDAO = new UsersDAO();
        private ProfileDAO profileDAO = new ProfileDAO();


        [TestMethod]
        public void TestServiceTryLoginValidUserTrue() {
            // Arrange
            var service = new ServiceImplementation();
            var user = new UserData {
                Username = "Cuau",
                Password = ""
            };

            // Act
            bool result = service.ServiceTryLogin(user);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestServiceTryLoginValidUserFalse() {
            //Arrange
            var service = new ServiceImplementation();
            var user = new UserData {
                Username = "testFalseUser",
                Password = "testFalsePassword"
            };

            //Act
            bool result = service.ServiceTryLogin(user);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestServiceTrySignInTrue() {
            // Arrange
            var service = new ServiceImplementation();
            var newUser = new UserData {
                Username = "newUser123",
                Password = "NewPass@123",
                Email = "newuser@example.com"
            };

            // Act
            bool result = service.ServiceTrySignIn(newUser);
            profileDAO.DeleteProfile(usersDAO.GetIdUserByUsername(newUser.Username));
            usersDAO.DeleteUser(newUser.Username);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestServiceTrySignInFalse() {
            // Arrange
            var service = new ServiceImplementation();
            var existingUser = new UserData {
                Username = "existingUser",
                Password = "password123",
                Email = "existinguser@example.com"
            };

            // Act
            service.ServiceTrySignIn(existingUser);
            bool result = service.ServiceTrySignIn(existingUser);
            profileDAO.DeleteProfile(usersDAO.GetIdUserByUsername(existingUser.Username));
            usersDAO.DeleteUser(existingUser.Username);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestServiceUserExistsByUsernameTrue() {
            // Arrange
            var service = new ServiceImplementation();
            string existingUsername = "test";

            // Act
            bool result = service.ServiceUserExistsByUsername(existingUsername);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestServiceUserExistsByUsernameFalse() {
            // Arrange
            var service = new ServiceImplementation();
            string nonExistingUsername = "nonExistingUser";

            // Act
            bool result = service.ServiceUserExistsByUsername(nonExistingUsername);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestServiceVerifyPasswordValidPasswordTrue() {
            // Arrange
            var service = new ServiceImplementation();
            string username = "Cuau";
            string password = "";

            // Act
            bool result = service.ServiceVerifyPassword(password, username);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestServiceVerifyPasswordValidPasswordFalse() {
            // Arrange
            var service = new ServiceImplementation();
            string username = "Cuau";
            string password = "contraseña";

            // Act
            bool result = service.ServiceVerifyPassword(password, username);

            // Assert
            Assert.IsFalse(result);
        }

    }
}
