using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess.DAOs;
using System.Linq;

namespace DataAccess.Tests {

    [TestClass]
    public class CardsDAOTests {


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
            int result = CardsDAO.AddCard(newCard);

            // Assert
            Assert.AreEqual(1, result);  // Asume que se espera 1 cuando la carta se agrega con éxito

            // Cleanup: Eliminar la carta agregada para no dejar datos residuales
            var cardToDelete = CardsDAO.GetAllCards().FirstOrDefault(c => c.cardName == "TestCard");
            if(cardToDelete != null) {
                CardsDAO.DeleteCard(cardToDelete.idCard);
            }
        }

        [TestMethod]
        public void TestAddCardFailure() {
            // Arrange
            var newCard = new Cards {
                cardName = "TestCard",
            };

            // Act
            int result = CardsDAO.AddCard(newCard);

            // Assert
            Assert.AreEqual(-1, result);  

            // Cleanup:
            var cardToDelete = CardsDAO.GetAllCards().FirstOrDefault(c => c.cardName == "TestCard");
            if (cardToDelete != null) {
                CardsDAO.DeleteCard(cardToDelete.idCard);
            }
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

            // Agregar la carta antes de intentar eliminarla
            CardsDAO.AddCard(newCard);

            // Obtener el ID de la carta recién agregada
            var cardToDelete = CardsDAO.GetAllCards().FirstOrDefault(c => c.cardName == "TestCard");

            // Act
            int result = 0;
            if(cardToDelete != null) {
                result = CardsDAO.DeleteCard(cardToDelete.idCard);
            }

            // Assert
            Assert.AreEqual(1, result);  // Asume que se espera 1 cuando la carta se elimina con éxito
        }

        [TestMethod]
        public void TestGetAllCardsNotNull() {
            // Act
            var allCards = CardsDAO.GetAllCards();

            // Assert
            Assert.IsNotNull(allCards); 
        }

        [TestMethod]
        public void TestGetAllCardsExists() {
            // Act
            var allCards = CardsDAO.GetAllCards();

            // Assert
            Assert.IsTrue(allCards.Count > 0);
        }

        [TestMethod]
        public void TestGetCardByIdNotNull() {
            // Arrange
            var newCard = new Cards {
                cardName = "TestCard",
                points = 100,
                cardType = "Attack",
                imageCardId = 1,
                description = "This is a test card.",
                effectDescription = "No special effect"
            };

            // Agregar la carta antes de intentar obtenerla
            CardsDAO.AddCard(newCard);

            // Obtener el ID de la carta recién agregada
            var cardToFetch = CardsDAO.GetAllCards().FirstOrDefault(c => c.cardName == "TestCard");

            // Act
            Cards fetchedCard = null;
            if(cardToFetch != null) {
                fetchedCard = CardsDAO.GetCardById(cardToFetch.idCard);
            }

            // Assert
            Assert.IsNotNull(fetchedCard);
        }

        [TestMethod]
        public void TestGetCardById() {
            // Arrange
            var newCard = new Cards {
                cardName = "TestCard",
                points = 100,
                cardType = "Attack",
                imageCardId = 1,
                description = "This is a test card.",
                effectDescription = "No special effect"
            };

            // Agregar la carta antes de intentar obtenerla
            CardsDAO.AddCard(newCard);

            // Obtener el ID de la carta recién agregada
            var cardToFetch = CardsDAO.GetAllCards().FirstOrDefault(c => c.cardName == "TestCard");

            // Act
            Cards fetchedCard = null;
            if (cardToFetch != null) {
                fetchedCard = CardsDAO.GetCardById(cardToFetch.idCard);
            }

            // Assert
            Assert.AreEqual(newCard.cardName, fetchedCard.cardName);
        }
    }
}
