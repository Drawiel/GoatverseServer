using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess.DAOs {
    public static class CardsDAO {

        public static List<Cards> GetAllCards() {
            try {
                using(var database = new GoatverseEntities()) {
                    return database.Cards.ToList();
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return new List<Cards>();
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new List<Cards>();
            }
        }

        public static Cards GetCardById(int id) {
            try {
                using(var database = new GoatverseEntities()) {
                    return database.Cards.SingleOrDefault(c => c.idCard == id);
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                Cards cards = new Cards() { idCard = -1};
                return cards;
            } catch(InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                Cards cards = new Cards() { idCard = -1 };
                return cards;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                Cards cards = new Cards() { idCard = -1 };
                return cards;
            }
        }

        public static int AddCard(Cards card) {
            try {
                using(var database = new GoatverseEntities()) {
                    database.Cards.Add(card);
                    return database.SaveChanges();
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return -1;
            } catch(InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                return -1;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return -1;
            }
        }

        public static int DeleteCard(int id) {
            try {
                using(var database = new GoatverseEntities()) {
                    var card = database.Cards.SingleOrDefault(c => c.idCard == id);
                    if(card != null) {
                        database.Cards.Remove(card);
                        return database.SaveChanges();
                    }
                    return 0;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return -1;
            } catch(InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                return -1;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return -1;
            }
        }
    }
}
