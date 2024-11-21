﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.AreEqual(1, result);  // Asume que se espera 1 cuando la carta se agrega con éxito

            // Cleanup: Eliminar la carta agregada para no dejar datos residuales
            var cardToDelete = cardsDAO.GetAllCards().FirstOrDefault(c => c.cardName == "TestCard");
            if(cardToDelete != null) {
                cardsDAO.DeleteCard(cardToDelete.idCard);
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
            cardsDAO.AddCard(newCard);

            // Obtener el ID de la carta recién agregada
            var cardToDelete = cardsDAO.GetAllCards().FirstOrDefault(c => c.cardName == "TestCard");

            // Act
            int result = 0;
            if(cardToDelete != null) {
                result = cardsDAO.DeleteCard(cardToDelete.idCard);
            }

            // Assert
            Assert.AreEqual(1, result);  // Asume que se espera 1 cuando la carta se elimina con éxito
        }

        [TestMethod]
        public void TestGetAllCards() {
            // Act
            var allCards = cardsDAO.GetAllCards();

            // Assert
            Assert.IsNotNull(allCards);  // Asegurarse de que se obtiene una lista no nula
            Assert.IsTrue(allCards.Count > 0);  // Asegurarse de que haya al menos una carta
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
            cardsDAO.AddCard(newCard);

            // Obtener el ID de la carta recién agregada
            var cardToFetch = cardsDAO.GetAllCards().FirstOrDefault(c => c.cardName == "TestCard");

            // Act
            Cards fetchedCard = null;
            if(cardToFetch != null) {
                fetchedCard = cardsDAO.GetCardById(cardToFetch.idCard);
            }

            // Assert
            Assert.IsNotNull(fetchedCard);  // Asegurarse de que se haya encontrado la carta
            Assert.AreEqual(newCard.cardName, fetchedCard.cardName);  // Verificar que el nombre coincida
        }
    }
}
