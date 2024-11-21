using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.DAOs {
    public static class CardsDAO {

        public static List<Cards> GetAllCards() {
            using(var database = new GoatverseEntities()) {
                return database.Cards.ToList();
            }
        }

        public static Cards GetCardById(int id) {
            using(var database = new GoatverseEntities()) {
                return database.Cards.SingleOrDefault(c => c.idCard == id);
            }
        }

        public static int AddCard(Cards card) {
            using(var database = new GoatverseEntities()) {
                database.Cards.Add(card);
                return database.SaveChanges();
            }
        }

        public static int DeleteCard(int id) {
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

