using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess.DAOs {
    public static class FriendsDAO {

        private const string constPending = "Pending";
        private const string constAccepted = "Accepted";

        public static int AddFriend(int idUser1, int idUser2) {
            try {
                using(var database = new GoatverseEntities()) {
                    var newFriendship = new Friends {
                        idUser1 = idUser1,
                        idUser2 = idUser2,
                        statusRequest = constPending,
                    };

                    database.Friends.Add(newFriendship);
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

        public static int AcceptFriendRequest(int idUser1, int idUser2) {
            try {
                using(var database = new GoatverseEntities()) {
                    var friendship = database.Friends
                        .Where(f => f.idUser1 == idUser1 && f.idUser2 == idUser2)
                        .SingleOrDefault();

                    if(friendship != null && friendship.statusRequest == constPending) {
                        friendship.statusRequest = constAccepted;
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

        public static int DeleteFriend(int idUser1, int idUser2) {
            try {
                using(var database = new GoatverseEntities()) {
                    var friendship = database.Friends
                        .Where(f => f.idUser1 == idUser1 && f.idUser2 == idUser2)
                        .SingleOrDefault();

                    if(friendship != null) {
                        database.Friends.Remove(friendship);
                        return database.SaveChanges();
                    }

                    return 0;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return -1;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return -1;
            }
        }

        public static List<int> GetFriends(int userId) {
            try {
                using(var database = new GoatverseEntities()) {
                    var idFriends = database.Friends
                        .Where(f => (f.idUser1 == userId || f.idUser2 == userId) && f.statusRequest == constAccepted)
                        .Select(f => f.idUser1 == userId ? f.idUser2 : f.idUser1)
                        .ToList();

                    return idFriends;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return new List<int>();
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new List<int>();
            }
        }

        public static bool IsFriend(int idUser1, int idUser2) {
            try {
                using(var database = new GoatverseEntities()) {
                    var friendship = database.Friends
                        .Where(f => f.idUser1 == idUser1 && f.idUser2 == idUser2 && f.statusRequest == constAccepted)
                        .SingleOrDefault();

                    return friendship != null;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return false;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }

        public static bool IsFriendRequestPending(int idUser1, int idUser2) {
            try {
                using(var database = new GoatverseEntities()) {
                    var pendingRequest = database.Friends
                        .Where(f => f.idUser1 == idUser1 && f.idUser2 == idUser2 && f.statusRequest == constPending)
                        .SingleOrDefault();

                    return pendingRequest != null;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return false;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }

        public static List<int> GetPendingFriendRequests(int userId) {
            try {
                using(var database = new GoatverseEntities()) {
                    var idFriends = database.Friends
                        .Where(f => f.idUser2 == userId && f.statusRequest == constPending)
                        .Select(f => f.idUser1)
                        .ToList();

                    return idFriends;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return new List<int>();
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return new List<int>();
            }
        }
    }
}
