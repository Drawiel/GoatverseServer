using DataAccess.DAOs;
using DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Tests {
    internal class MatchDAOTest {
        [TestMethod]
        public void TestCreateMatchSuccess() {
            // Arrange
            DateTime startTime = DateTime.Now;

            // Act
            int result = MatchDAO.CreateMatch(startTime);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestCreateMatchFailure() {
            // Arrange
            DateTime? invalidStartTime = null;

            // Act
            int result = MatchDAO.CreateMatch(invalidStartTime ?? DateTime.MinValue);

            // Assert
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void TestGetMatchByStartTimeSuccess() {
            // Arrange
            DateTime startTime = DateTime.Now;
            MatchDAO.CreateMatch(startTime);

            // Act
            int matchId = MatchDAO.GetMatchByStartTime(startTime);

            // Assert
            Assert.IsTrue(matchId > 0);
        }

        [TestMethod]
        public void TestGetMatchByStartTimeFailure() {
            // Arrange
            DateTime nonExistentStartTime = DateTime.Now.AddYears(-10);

            // Act
            int matchId = MatchDAO.GetMatchByStartTime(nonExistentStartTime);

            // Assert
            Assert.AreEqual(0, matchId); 
        }

        [TestMethod]
        public void TestGetMatchByIdSuccess() {
            // Arrange
            DateTime startTime = DateTime.Now;
            MatchDAO.CreateMatch(startTime);
            int matchId = MatchDAO.GetMatchByStartTime(startTime);

            // Act
            var match = MatchDAO.GetMatchById(matchId);

            // Assert
            Assert.IsNotNull(match);
            Assert.AreEqual(startTime, match.startTime);
        }

        [TestMethod]
        public void TestGetMatchByIdFailure() {
            // Arrange
            int nonExistentId = 9999;

            // Act
            var match = MatchDAO.GetMatchById(nonExistentId);

            // Assert
            Assert.IsNull(match);
        }

        [TestMethod]
        public void TestUpdateMatchSuccess() {
            // Arrange
            DateTime startTime = DateTime.Now;
            MatchDAO.CreateMatch(startTime);
            int matchId = MatchDAO.GetMatchByStartTime(startTime);
            int? winnerId = 1;
            DateTime? endTime = DateTime.Now.AddHours(1);

            // Act
            int result = MatchDAO.UpdateMatch(matchId, winnerId, endTime);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestUpdateMatchFailure() {
            // Arrange
            int nonExistentId = 9999;
            int? winnerId = 1;
            DateTime? endTime = DateTime.Now;

            // Act
            int result = MatchDAO.UpdateMatch(nonExistentId, winnerId, endTime);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetRecentMatchesSuccess() {
            // Arrange
            DateTime startTime1 = DateTime.Now.AddHours(-2);
            DateTime startTime2 = DateTime.Now.AddHours(-1);
            MatchDAO.CreateMatch(startTime1);
            MatchDAO.CreateMatch(startTime2);

            // Act
            List<Matches> recentMatches = MatchDAO.GetRecentMatches(2);

            // Assert
            Assert.AreEqual(2, recentMatches.Count);
            Assert.IsTrue(recentMatches[0].startTime > recentMatches[1].startTime);
        }

        [TestMethod]
        public void TestGetRecentMatchesFailure() {
            // Act
            List<Matches> recentMatches = MatchDAO.GetRecentMatches(0);

            // Assert
            Assert.IsNotNull(recentMatches);
            Assert.AreEqual(0, recentMatches.Count);
        }
    }
}
