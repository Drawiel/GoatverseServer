using DataAccess.DAOs;
using DataAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlTypes;

namespace DataAccess.Tests {
    [TestClass]
    public  class MatchDAOTest {
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
        public void TestGetMatchByStartTimeFailure() {
            // Arrange
            DateTime nonExistentStartTime = DateTime.Now.AddYears(-10);

            // Act
            int matchId = MatchDAO.GetMatchByStartTime(nonExistentStartTime);

            // Assert
            Assert.AreEqual(0, matchId); 
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
