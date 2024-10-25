using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs {
    public class CardsDAO {

        public int AddCard(Cards card) {
            using (var database = new GoatverseEntities()) {
                database.Cards.Add(card);
                int result = database.SaveChanges();
                return result;
            }
        }

        public int DeleteCard(string cardname) {
            using (var database = new GoatverseEntities()) {
                var delete = (from cards in database.Cards where cards.cardName == cardname select cards).Single();
                database.Cards.Remove(delete);
                int result = database.SaveChanges();
                return result;
            }
        }
    }
}
