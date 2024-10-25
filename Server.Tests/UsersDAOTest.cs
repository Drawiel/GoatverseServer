using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DAOs;
using System.Linq;
using System;

namespace DataAccess.Tests {
    [TestClass]
    public class UsersDAOTest {
        private UsersDAO usersDAO = new UsersDAO();

        [TestMethod]
        public void TestAddUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "TestUser123",
                password = BCrypt.Net.BCrypt.HashPassword("TestPassword123"),
                email = "testuser123@example.com"
            };

            // Act
            int result = usersDAO.AddUser(newUser);

            // Assert
            Assert.AreEqual(1, result);
            usersDAO.DeleteUser(newUser.username); 
        }


        [TestMethod]
        public void TestDeleteUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "UserToDelete",
                password = BCrypt.Net.BCrypt.HashPassword("PasswordToDelete"),
                email = "deleteuser@example.com"
            };
            usersDAO.AddUser(newUser); 

            // Act
            int result = usersDAO.DeleteUser(newUser.username);

            // Assert
            Assert.AreEqual(1, result); 
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDeleteUserNonExisting() {
            // Act
            usersDAO.DeleteUser("NonExistingUser"); 
        }

        [TestMethod]
        public void TestUpdateUserPasswordAndUsernameByEmailSuccess() {
            // Arrange
            var newUser = new Users {
                username = "OldUsername",
                password = BCrypt.Net.BCrypt.HashPassword("OldPassword"),
                email = "updateuser@example.com"
            };
            usersDAO.AddUser(newUser);

            var updatedUser = new Users {
                username = "NewUsername",
                password = "NewPassword",
                email = "updateuser@example.com"
            };

            // Act
            int result = usersDAO.UpdateUserPasswordAndUsernameByEmail(updatedUser);

            // Assert
            Assert.AreEqual(1, result);
            usersDAO.DeleteUser(updatedUser.username); 
        }

        [TestMethod]
        public void TestGetIdUserByUsernameSuccess() {
            // Arrange
            var newUser = new Users {
                username = "UserForId",
                password = BCrypt.Net.BCrypt.HashPassword("PasswordForId"),
                email = "getuserid@example.com"
            };
            usersDAO.AddUser(newUser);

            // Act
            int userId = usersDAO.GetIdUserByUsername(newUser.username);

            // Assert
            Assert.AreNotEqual(0, userId); 
            usersDAO.DeleteUser(newUser.username); 
        }

        [TestMethod]
        public void TestGetIdUserByUsernameNonExisting() {
            // Act
            int userId = usersDAO.GetIdUserByUsername("NonExistingUser");

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
            usersDAO.AddUser(newUser);
            int userId = usersDAO.GetIdUserByUsername(newUser.username);

            // Act
            string email = usersDAO.GetEmailByIdUser(userId);

            // Assert
            Assert.AreEqual("getemail@example.com", email);
            usersDAO.DeleteUser(newUser.username); 
        }

        [TestMethod]
        public void TestGetEmailByIdUserNonExisting() {
            // Act
            string email = usersDAO.GetEmailByIdUser(-1);

            // Assert
            Assert.IsNull(email); 
        }
    }
}
