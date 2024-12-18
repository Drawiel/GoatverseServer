using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DAOs;
using System.Linq;
using System;

namespace DataAccess.Tests {
    [TestClass]
    public class UsersDAOTest {

        [TestMethod]
        public void TestAddUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "TestUser123",
                password = BCrypt.Net.BCrypt.HashPassword("TestPassword123"),
                email = "testuser123@example.com"
            };

            // Act
            int result = UsersDAO.AddUser(newUser);

            // Assert
            Assert.AreEqual(1, result);
            UsersDAO.DeleteUser(newUser.username); 
        }


        [TestMethod]
        public void TestDeleteUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "UserToDelete",
                password = BCrypt.Net.BCrypt.HashPassword("PasswordToDelete"),
                email = "deleteuser@example.com"
            };
            UsersDAO.AddUser(newUser); 

            // Act
            int result = UsersDAO.DeleteUser(newUser.username);

            // Assert
            Assert.AreEqual(1, result); 
        }

        [TestMethod]
        public void TestDeleteUserNonExisting() {
            // Act
            int result = UsersDAO.DeleteUser("NonExistingUser");

            // Assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void TestUpdateUserPasswordAndUsernameByEmailSuccess() {
            // Arrange
            var newUser = new Users {
                username = "OldUsername",
                password = BCrypt.Net.BCrypt.HashPassword("OldPassword"),
                email = "updateuser@example.com"
            };
            UsersDAO.AddUser(newUser);

            var updatedUser = new Users {
                username = "NewUsername",
                password = "NewPassword",
                email = "updateuser@example.com"
            };

            // Act
            int result = UsersDAO.UpdateUserPasswordAndUsernameByEmail(updatedUser);

            // Assert
            Assert.AreEqual(1, result);
            UsersDAO.DeleteUser(updatedUser.username); 
        }

        [TestMethod]
        public void TestGetIdUserByUsernameSuccess() {
            // Arrange
            var newUser = new Users {
                username = "UserForId",
                password = BCrypt.Net.BCrypt.HashPassword("PasswordForId"),
                email = "getuserid@example.com"
            };
            UsersDAO.AddUser(newUser);

            // Act
            int userId = UsersDAO.GetIdUserByUsername(newUser.username);

            // Assert
            Assert.AreNotEqual(0, userId); 
            UsersDAO.DeleteUser(newUser.username); 
        }

        [TestMethod]
        public void TestGetIdUserByUsernameNonExisting() {
            // Act
            int userId = UsersDAO.GetIdUserByUsername("NonExistingUser");

            // Assert
            Assert.AreEqual(0, userId);
        }

        [TestMethod]
        public void TestGetEmailByIdUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "UserForEmail",
                password = BCrypt.Net.BCrypt.HashPassword("PasswordForEmail"),
                email = "getemail@example.com"
            };
            UsersDAO.AddUser(newUser);
            int userId = UsersDAO.GetIdUserByUsername(newUser.username);

            // Act
            string email = UsersDAO.GetEmailByIdUser(userId);

            // Assert
            Assert.AreEqual("getemail@example.com", email);
            UsersDAO.DeleteUser(newUser.username); 
        }

        [TestMethod]
        public void TestGetEmailByIdUserNonExisting() {
            // Act
            string email = UsersDAO.GetEmailByIdUser(-1);

            // Assert
            Assert.IsNull(email); 
        }

        [TestMethod]
        public void TestGetUsernameByIdUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "UsernameById",
                password = BCrypt.Net.BCrypt.HashPassword("PasswordForUsername"),
                email = "getusername@example.com"
            };
            UsersDAO.AddUser(newUser);
            int userId = UsersDAO.GetIdUserByUsername(newUser.username);

            // Act
            string username = UsersDAO.GetUsernameByIdUser(userId);

            // Assert
            Assert.AreEqual("UsernameById", username);
            UsersDAO.DeleteUser(newUser.username);
        }

        [TestMethod]
        public void TestGetUsernameByIdUserNonExisting() {
            // Act
            string username = UsersDAO.GetUsernameByIdUser(-1);

            // Assert
            Assert.IsNull(username);
        }
    }
}
