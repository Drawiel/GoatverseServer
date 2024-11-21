using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DAOs;
using System;

namespace DataAccess.Tests {
    [TestClass]
    public class ProfileDAOTest {

        [TestMethod]
        public void TestAddProfileSuccess() {
            // Arrange
            var newUser = new Users {
                username = "ProfileUser",
                password = BCrypt.Net.BCrypt.HashPassword("ProfilePassword"),
                email = "profileuser@example.com"
            };
            UsersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = UsersDAO.GetIdUserByUsername(newUser.username),
                profileLevel = 1,
                totalPoints = 50,
                matchesWon = 5,
                imageId = 100
            };

            // Act
            int result = ProfileDAO.AddProfile(newProfile);

            // Assert
            Assert.AreEqual(1, result); 
            CleanupUserAndProfile(newUser.username);
        }

        [TestMethod]
        public void TestDeleteProfileSuccess() {
            // Arrange
            var newUser = new Users {
                username = "DeleteProfileUser",
                password = BCrypt.Net.BCrypt.HashPassword("DeletePassword"),
                email = "deleteprofile@example.com"
            };
            UsersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = UsersDAO.GetIdUserByUsername(newUser.username),
                profileLevel = 1,
                totalPoints = 50,
                matchesWon = 5,
                imageId = 100
            };
            ProfileDAO.AddProfile(newProfile);

            // Act
            int result = ProfileDAO.DeleteProfile((int)newProfile.idUser);

            // Assert
            Assert.AreEqual(1, result); 
            UsersDAO.DeleteUser(newUser.username); 
        }

        [TestMethod]
        public void TestUpdateProfileSuccess() {
            // Arrange
            var newUser = new Users {
                username = "UpdateProfileUser",
                password = BCrypt.Net.BCrypt.HashPassword("UpdatePassword"),
                email = "updateprofile@example.com"
            };
            UsersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = UsersDAO.GetIdUserByUsername(newUser.username),
                profileLevel = 1,
                totalPoints = 50,
                matchesWon = 5,
                imageId = 100
            };
            ProfileDAO.AddProfile(newProfile);

            var updatedProfile = new Profile {
                idUser = newProfile.idUser,
                profileLevel = 2,
                totalPoints = 100,
                matchesWon = 10,
                imageId = 100
            };

            // Act
            int result = ProfileDAO.UpdateProfile(updatedProfile);

            // Assert
            Assert.AreEqual(1, result); 
            CleanupUserAndProfile(newUser.username);
        }

        [TestMethod]
        public void TestChangeProfileImageByIdUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "ChangeImageUser",
                password = BCrypt.Net.BCrypt.HashPassword("ImagePassword"),
                email = "changeimage@example.com"
            };
            UsersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = UsersDAO.GetIdUserByUsername(newUser.username),
                profileLevel = 1,
                totalPoints = 50,
                matchesWon = 5,
                imageId = 100
            };
            ProfileDAO.AddProfile(newProfile);

            var updatedProfile = new Profile {
                idUser = newProfile.idUser,
                imageId = 200
            };

            // Act
            int result = ProfileDAO.ChangeProfileImageByIdUser((int)updatedProfile.idUser, (int)updatedProfile.imageId);

            // Assert
            Assert.AreEqual(1, result);
            CleanupUserAndProfile(newUser.username);
        }

        [TestMethod]
        public void TestGetImageIdByIdUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "GetImageUser",
                password = BCrypt.Net.BCrypt.HashPassword("ImagePassword"),
                email = "getimage@example.com"
            };
            UsersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = UsersDAO.GetIdUserByUsername(newUser.username),
                profileLevel = 1,
                totalPoints = 50,
                matchesWon = 5,
                imageId = 300
            };
            ProfileDAO.AddProfile(newProfile);

            // Act
            int imageId = ProfileDAO.GetImageIdByIdUser((int)newProfile.idUser);

            // Assert
            Assert.AreEqual(300, imageId);
            CleanupUserAndProfile(newUser.username);
        }

        [TestMethod]
        public void TestGetProfileLevelByIdUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "GetLevelUser",
                password = BCrypt.Net.BCrypt.HashPassword("LevelPassword"),
                email = "getlevel@example.com"
            };
            UsersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = UsersDAO.GetIdUserByUsername(newUser.username),
                profileLevel = 3,
                totalPoints = 200,
                matchesWon = 20,
                imageId = 150
            };
            ProfileDAO.AddProfile(newProfile);

            // Act
            int profileLevel = ProfileDAO.GetProfileLevelByIdUser((int)newProfile.idUser);

            // Assert
            Assert.AreEqual(3, profileLevel);
            CleanupUserAndProfile(newUser.username);
        }

        [TestMethod]
        public void TestGetMatchesWonByIdUserSuccess() {
            // Arrange
            var newUser = new Users {
                username = "GetMatchesUser",
                password = BCrypt.Net.BCrypt.HashPassword("MatchesPassword"),
                email = "getmatches@example.com"
            };
            UsersDAO.AddUser(newUser);

            var newProfile = new Profile {
                idUser = UsersDAO.GetIdUserByUsername(newUser.username),
                profileLevel = 1,
                totalPoints = 50,
                matchesWon = 15,
                imageId = 100
            };
            ProfileDAO.AddProfile(newProfile);

            // Act
            int matchesWon = ProfileDAO.GetMatchesWonByIdUser((int)newProfile.idUser);

            // Assert
            Assert.AreEqual(15, matchesWon);
            CleanupUserAndProfile(newUser.username);
        }

        
        private void CleanupUserAndProfile(string username) {
            int userId = UsersDAO.GetIdUserByUsername(username);
            ProfileDAO.DeleteProfile(userId);
            UsersDAO.DeleteUser(username);
        }
    }
}
