using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.DAOs {
    public class CardsDAO {

        public List<Cards> GetAllCards() {
            using(var database = new GoatverseEntities()) {
                return database.Cards.ToList();
            }
        }

        public Cards GetCardById(int id) {
            using(var database = new GoatverseEntities()) {
                return database.Cards.SingleOrDefault(c => c.idCard == id);
            }
        }

        public int AddCard(Cards card) {
            using(var database = new GoatverseEntities()) {
                database.Cards.Add(card);
                return database.SaveChanges();
            }
        }

        public int DeleteCard(int id) {
            using(var database = new GoatverseEntities()) {
                var card = database.Cards.SingleOrDefault(c => c.idCard == id);
                if(card != null) {
                    database.Cards.Remove(card);
                    return database.SaveChanges();
                }
                return 0; 
            }
        }
    }
}

