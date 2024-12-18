using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DAOs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace DataAccess.Tests {
    [TestClass]
    public class BlockedDAOTest {

        [TestMethod]
        public void TestAddBlockSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_AddBlock", "User1Password", "user1addblock@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_AddBlock");
            var user2 = CreateUserWithProfile("User2_AddBlock", "User2Password", "user2addblock@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_AddBlock");

            // Act
            int result = BlockedDAO.BlockUser(idUser1, idUser2);

            // Assert
            Assert.AreEqual(1, result); // 1 fila modificada
            BlockedDAO.DeleteBlock(idUser1, idUser2); // Limpieza
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestAddBlockFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_FailAddBlock", "User1Password", "user1failaddblock@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_FailAddBlock");

            // Act
            int result = BlockedDAO.BlockUser(idUser1, 9999); // Usuario inexistente

            Assert.AreEqual(-1, result); // 1 fila modificada

            // Cleanup
            CleanupUserAndProfile(user1.username);
        }

        [TestMethod]
        public void TestDeleteBlockSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_DeleteBlock", "User1Password", "user1deleteblock@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_DeleteBlock");
            var user2 = CreateUserWithProfile("User2_DeleteBlock", "User2Password", "user2deleteblock@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_DeleteBlock");

            BlockedDAO.BlockUser(idUser1, idUser2); // Bloquea al usuario previamente

            // Act
            int result = BlockedDAO.DeleteBlock(idUser1, idUser2);

            // Assert
            Assert.AreEqual(1, result); // 1 fila eliminada
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestDeleteBlockFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_FailDeleteBlock", "User1Password", "user1faildeleteblock@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_FailDeleteBlock");
            var user2 = CreateUserWithProfile("User2_FailDeleteBlock", "User2Password", "user2faildeleteblock@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_FailDeleteBlock");

            // Act
            int result = BlockedDAO.DeleteBlock(idUser1, idUser2); // No se bloqueó previamente

            // Assert
            Assert.AreEqual(0, result); // Ninguna fila eliminada
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestGetBlockedUsersSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_GetBlocked", "User1Password", "user1getblocked@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_GetBlocked");
            var user2 = CreateUserWithProfile("User2_GetBlocked", "User2Password", "user2getblocked@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_GetBlocked");

            BlockedDAO.BlockUser(idUser2, idUser1);

            // Act
            List<int> blockedUsers = BlockedDAO.GetBlockedUsers(idUser1);

            // Assert
            Assert.IsTrue(blockedUsers.Contains(idUser2));
            BlockedDAO.DeleteBlock(idUser1, idUser2); // Limpieza
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestGetBlockedUsersFailure() {
            // Arrange
            var user = CreateUserWithProfile("User1_NoBlocked", "User1Password", "user1noblocked@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_NoBlocked");

            // Act
            List<int> blockedUsers = BlockedDAO.GetBlockedUsers(idUser1);

            // Assert
            Assert.AreEqual(0, blockedUsers.Count); // No hay usuarios bloqueados
            CleanupUserAndProfile(user.username);
        }

        [TestMethod]
        public void TestIsUserBlockedSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_IsBlocked", "User1Password", "user1isblocked@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_IsBlocked");
            var user2 = CreateUserWithProfile("User2_IsBlocked", "User2Password", "user2isblocked@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_IsBlocked");

            BlockedDAO.BlockUser(idUser2, idUser1);

            // Act
            bool isBlocked = BlockedDAO.IsUserBlocked(idUser1, idUser2);

            // Assert
            Assert.IsTrue(isBlocked);
            BlockedDAO.DeleteBlock(idUser1, idUser2); 
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestIsUserBlockedFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_NotBlocked", "User1Password", "user1notblocked@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_NotBlocked");
            var user2 = CreateUserWithProfile("User2_NotBlocked", "User2Password", "user2notblocked@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_NotBlocked");

            // Act
            bool isBlocked = BlockedDAO.IsUserBlocked(idUser1, idUser2);

            // Assert
            Assert.IsFalse(isBlocked);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        private Users CreateUserWithProfile(string username, string password, string email) {
            var newUser = new Users {
                username = username,
                password = BCrypt.Net.BCrypt.HashPassword(password),
                email = email
            };
            UsersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = UsersDAO.GetIdUserByUsername(username),
                profileLevel = 1,
                totalPoints = 50,
                matchesWon = 5,
                imageId = 100
            };
            ProfileDAO.AddProfile(newProfile);

            return newUser;
        }

        private void CleanupUserAndProfile(string username) {
            int userId = UsersDAO.GetIdUserByUsername(username);
            ProfileDAO.DeleteProfile(userId);
            UsersDAO.DeleteUser(username);
        }
    }
}
