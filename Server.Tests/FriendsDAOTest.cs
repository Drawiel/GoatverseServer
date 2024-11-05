using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DAOs;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace DataAccess.Tests {
    [TestClass]
    public class FriendsDAOTest {
        private UsersDAO usersDAO = new UsersDAO();
        private ProfileDAO profileDAO = new ProfileDAO();
        private FriendsDAO friendsDAO = new FriendsDAO();

        [TestMethod]
        public void TestAddFriendSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_AddFriend", "User1Password", "user1@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_AddFriend");
            var user2 = CreateUserWithProfile("User2_AddFriend", "User2Password", "user2@example.com");
            int idUser2 = usersDAO.GetIdUserByUsername("User2_AddFriend");

            // Act
            int result = friendsDAO.AddFriend(idUser1, idUser2);

            // Assert
            Assert.AreEqual(1, result);
            friendsDAO.DeleteFriend(idUser1, idUser2);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
            
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public void TestAddFriendFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_FailAddFriend", "User1Password", "user1fail@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_FailAddFriend");


            // Act
            int result = friendsDAO.AddFriend(idUser1, 9999);

            CleanupUserAndProfile(user1.username);
        }

        [TestMethod]
        public void TestAcceptFriendRequestSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_AcceptFriend", "User1Password", "user1accept@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_AcceptFriend");
            var user2 = CreateUserWithProfile("User2_AcceptFriend", "User2Password", "user2accept@example.com");
            int idUser2 = usersDAO.GetIdUserByUsername("User2_AcceptFriend");

            friendsDAO.AddFriend(idUser1, idUser2);

            // Act
            int result = friendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Assert
            Assert.AreEqual(1, result);
            friendsDAO.DeleteFriend(idUser1, idUser2);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestAcceptFriendRequestFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_FailAcceptFriend", "User1Password", "user1failaccept@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_FailAcceptFriend");
            var user2 = CreateUserWithProfile("User2_FailAcceptFriend", "User2Password", "user2failaccept@example.com");
            int idUser2 = usersDAO.GetIdUserByUsername("User2_FailAcceptFriend");


            // Act (no friend request setup)
            int result = friendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Assert
            Assert.AreEqual(0, result);
            CleanupUserAndProfile(user1.username); 
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestDeleteFriendSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_DeleteFriend", "User1Password", "user1delete@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_DeleteFriend");
            var user2 = CreateUserWithProfile("User2_DeleteFriend", "User2Password", "user2delete@example.com");
            int idUser2 = usersDAO.GetIdUserByUsername("User2_DeleteFriend");

            friendsDAO.AddFriend(idUser1, idUser2);
            friendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Act
            int result = friendsDAO.DeleteFriend(idUser1, idUser2);

            // Assert
            Assert.AreEqual(1, result);
            CleanupUserAndProfile(user1.username); 
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestDeleteFriendFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_FailDeleteFriend", "User1Password", "user1faildelete@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_FailDeleteFriend");
            var user2 = CreateUserWithProfile("User2_FailDeleteFriend", "User2Password", "user2faildelete@example.com");
            int idUser2 = usersDAO.GetIdUserByUsername("User2_FailDeleteFriend");


            // Act (sin amistad previa)
            int result = friendsDAO.DeleteFriend(idUser1, idUser2);

            // Assert
            Assert.AreEqual(0, result);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestGetFriendsSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_GetFriends", "User1Password", "user1getfriends@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_GetFriends");
            var user2 = CreateUserWithProfile("User2_GetFriends", "User2Password", "user2getfriends@example.com");
            int idUser2 = usersDAO.GetIdUserByUsername("User2_GetFriends");

            friendsDAO.AddFriend(idUser1, idUser2);
            friendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Act
            List<int> friends = friendsDAO.GetFriends((int)user1.idUser);

            // Assert
            Assert.IsTrue(friends.Contains((int)user2.idUser));
            friendsDAO.DeleteFriend(idUser1, idUser2);
            CleanupUserAndProfile(user1.username); 
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestGetFriendsFailure() {
            // Arrange
            var user = CreateUserWithProfile("User1_NoFriends", "User1Password", "user1nofriends@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_AddFriend");

            // Act
            List<int> friends = friendsDAO.GetFriends((int)user.idUser);

            // Assert
            Assert.AreEqual(0, friends.Count);
            CleanupUserAndProfile(user.username);
        }

        [TestMethod]
        public void TestIsFriendSuccess() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_IsFriend", "User1Password", "user1isfriend@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_IsFriend");
            var user2 = CreateUserWithProfile("User2_IsFriend", "User2Password", "user2isfriend@example.com");
            int idUser2 = usersDAO.GetIdUserByUsername("User2_IsFriend");

            friendsDAO.AddFriend(idUser1, idUser2);
            friendsDAO.AcceptFriendRequest(idUser1, idUser2);

            // Act
            bool isFriend = friendsDAO.IsFriend(idUser1, idUser2);

            // Assert
            Assert.IsTrue(isFriend);
            friendsDAO.DeleteFriend(idUser1, idUser2);
            CleanupUserAndProfile(user1.username);
            CleanupUserAndProfile(user2.username);
        }

        [TestMethod]
        public void TestIsFriendFailure() {
            // Arrange
            var user1 = CreateUserWithProfile("User1_NotFriend", "User1Password", "user1notfriend@example.com");
            int idUser1 = usersDAO.GetIdUserByUsername("User1_NotFriend");

            var user2 = CreateUserWithProfile("User2_NotFriend", "User2Password", "user2notfriend@example.com");
            int idUser2 = usersDAO.GetIdUserByUsername("User2_NotFriend");

            // Act
            bool isFriend = friendsDAO.IsFriend(idUser1, idUser2);

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
            usersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = usersDAO.GetIdUserByUsername(username),
                profileLevel = 1,
                totalPoints = 50,
                matchesWon = 5,
                imageId = 100
            };
            profileDAO.AddProfile(newProfile);

            return newUser;
        }

        private void CleanupUserAndProfile(string username) {
            int userId = usersDAO.GetIdUserByUsername(username);
            profileDAO.DeleteProfile(userId);
            usersDAO.DeleteUser(username);
        }
    }
}
