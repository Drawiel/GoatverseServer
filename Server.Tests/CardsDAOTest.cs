using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DAOs;
using System.Linq;

namespace DataAccess.Tests {

    [TestClass]
    public class CardsDAOTests {

        private CardsDAO cardsDAO = new CardsDAO();

        [TestMethod]
        public void TestAddCard() {
            // Arrange
            var newCard = new Cards {
                cardName = "TestCard",
                points = 100,
                cardType = "Attack",
                imageCardId = 1,
                description = "This is a test card.",
                effectDescription = "No special effect"
            };

            // Act
            int result = cardsDAO.AddCard(newCard);

            // Assert
            Assert.AreEqual(1, result);
            cardsDAO.DeleteCard(newCard.cardName);
        }

        [TestMethod]
        public void TestDeleteCard() {
            // Arrange
            var newCard = new Cards {
                cardName = "TestCard",
                points = 100,
                cardType = "Attack",
                imageCardId = 1,
                description = "This is a test card.",
                effectDescription = "No special effect"
            };

            
            cardsDAO.AddCard(newCard);

            // Act
            int result = cardsDAO.DeleteCard(newCard.cardName);

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
