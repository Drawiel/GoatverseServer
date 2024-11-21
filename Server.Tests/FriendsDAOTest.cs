using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DAOs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace DataAccess.Tests {
    [TestClass]
    public class FriendsDAOTest {

        [TestMethod]
        public void TestAddFriendSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_AddFriend", "User1Password", "user1@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_AddFriend");
            var user2 = CreateUserWithProfile("User2_AddFriend", "User2Password", "user2@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_AddFriend");

            // Act
            int result = FriendsDAO.AddFriend(idUser1, idUser2);

            // Assert
            Assert.AreEqual(1, result);
            FriendsDAO.DeleteFriend(idUser1, idUser2);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
            
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void TestAddFriendFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_FailAddFriend", "User1Password", "user1fail@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_FailAddFriend");


            // Act
            int result = FriendsDAO.AddFriend(idUser1, 9999);

            CleanupUserAndProfile(user1.username);
        }

        [TestMethod]
        public void TestAcceptFriendRequestSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_AcceptFriend", "User1Password", "user1accept@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_AcceptFriend");
            var user2 = CreateUserWithProfile("User2_AcceptFriend", "User2Password", "user2accept@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_AcceptFriend");

            FriendsDAO.AddFriend(idUser1, idUser2);

            // Act
            int result = FriendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Assert
            Assert.AreEqual(1, result);
            FriendsDAO.DeleteFriend(idUser1, idUser2);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestAcceptFriendRequestFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_FailAcceptFriend", "User1Password", "user1failaccept@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_FailAcceptFriend");
            var user2 = CreateUserWithProfile("User2_FailAcceptFriend", "User2Password", "user2failaccept@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_FailAcceptFriend");


            // Act (no friend request setup)
            int result = FriendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Assert
            Assert.AreEqual(0, result);
            CleanupUserAndProfile(user1.username); 
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestDeleteFriendSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_DeleteFriend", "User1Password", "user1delete@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_DeleteFriend");
            var user2 = CreateUserWithProfile("User2_DeleteFriend", "User2Password", "user2delete@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_DeleteFriend");

            FriendsDAO.AddFriend(idUser1, idUser2);
            FriendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Act
            int result = FriendsDAO.DeleteFriend(idUser1, idUser2);

            // Assert
            Assert.AreEqual(1, result);
            CleanupUserAndProfile(user1.username); 
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestDeleteFriendFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_FailDeleteFriend", "User1Password", "user1faildelete@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_FailDeleteFriend");
            var user2 = CreateUserWithProfile("User2_FailDeleteFriend", "User2Password", "user2faildelete@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_FailDeleteFriend");


            // Act (sin amistad previa)
            int result = FriendsDAO.DeleteFriend(idUser1, idUser2);

            // Assert
            Assert.AreEqual(0, result);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestGetFriendsSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_GetFriends", "User1Password", "user1getfriends@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_GetFriends");
            var user2 = CreateUserWithProfile("User2_GetFriends", "User2Password", "user2getfriends@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_GetFriends");

            FriendsDAO.AddFriend(idUser1, idUser2);
            FriendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Act
            List<int> friends = FriendsDAO.GetFriends((int)user1.idUser);

            // Assert
            Assert.IsTrue(friends.Contains((int)user2.idUser));
            FriendsDAO.DeleteFriend(idUser1, idUser2);
            CleanupUserAndProfile(user1.username); 
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestGetFriendsFailure() {
            // Arrange
            var user = CreateUserWithProfile("User1_NoFriends", "User1Password", "user1nofriends@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_AddFriend");

            // Act
            List<int> friends = FriendsDAO.GetFriends((int)user.idUser);

            // Assert
            Assert.AreEqual(0, friends.Count);
            CleanupUserAndProfile(user.username);
        }

        [TestMethod]
        public void TestIsFriendSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_IsFriend", "User1Password", "user1isfriend@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_IsFriend");
            var user2 = CreateUserWithProfile("User2_IsFriend", "User2Password", "user2isfriend@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_IsFriend");

            FriendsDAO.AddFriend(idUser1, idUser2);
            FriendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Act
            bool isFriend = FriendsDAO.IsFriend(idUser1, idUser2);

            // Assert
            Assert.IsTrue(isFriend);
            FriendsDAO.DeleteFriend(idUser1, idUser2);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestIsFriendFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_NotFriend", "User1Password", "user1notfriend@example.com");
            int idUser1 = UsersDAO.GetIdUserByUsername("User1_NotFriend");

            var user2 = CreateUserWithProfile("User2_NotFriend", "User2Password", "user2notfriend@example.com");
            int idUser2 = UsersDAO.GetIdUserByUsername("User2_NotFriend");

            // Act
            bool isFriend = FriendsDAO.IsFriend(idUser1, idUser2);

            // Assert
            Assert.IsFalse(isFriend);
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
