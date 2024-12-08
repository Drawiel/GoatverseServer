using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs {
    public static class BlockedDAO {
        public static int BlockUser(int idUserBlocked, int idUserBlocker) {
            try {
                using (var database = new GoatverseEntities()) {
                    var newBlockUser = new Blocked {
                        idBlockedUser = idUserBlocked,
                        idBlocker = idUserBlocker
                    };

                    database.Blocked.Add(newBlockUser);
                    return database.SaveChanges();
                }
            } catch (SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return -1;
            } catch (InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                return -1;
            } catch (Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return -1;
            }
        }

        public static int DeleteBlock(int idUserBlocked, int idUserBlocker) {
            try {
                using (var database = new GoatverseEntities()) {
                    var block = database.Blocked
                        .Where(b => (b.idBlocker == idUserBlocker) && (b.idBlockedUser == idUserBlocked))
                        .SingleOrDefault();

                    if (block != null) {
                        database.Blocked.Remove(block);
                        return database.SaveChanges();
                    }

                    return 0;
                }
            } catch (SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return -1;
            } catch (InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                return -1;
            } catch (Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return -1;
            }
        }

        public static List<int> GetBlockedUsers(int idUserBlocker) {
            try {
                using (var database = new GoatverseEntities()) {
                    var idBlockedUsers = database.Blocked
                        .Where(b => (b.idBlocker == idUserBlocker))
                        .Select(b => b.idBlockedUser)
                        .ToList();

                    return idBlockedUsers;
                }
            } catch (SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return new List<int>();
            } catch (InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                return new List<int>();
            } catch (Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new List<int>();
            }
        }

        public static bool IsUserBlocked(int idUserBlocker, int idUserBlocked) {
            try {
                using (var database = new GoatverseEntities()) {
                    var isBlocked = database.Blocked
                        .Where(b => (b.idBlocker == idUserBlocker) && (b.idBlockedUser == idUserBlocked))
                        .SingleOrDefault();

                    return isBlocked != null;
                }
            } catch (SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return false;
            } catch (InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                return false;
            } catch (Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }
    }
}
