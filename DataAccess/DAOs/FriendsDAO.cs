using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs {
    public static class FriendsDAO {

        private const string constPending = "Pending";
        private const string constAccepted = "Accepted";

        public static int AddFriend(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var newFriendship = new Friends {
                    idUser1 = idUser1,
                    idUser2 = idUser2,
                    statusRequest = constPending,
                };

                database.Friends.Add(newFriendship);
                return database.SaveChanges();
            }
        }

        public static int AcceptFriendRequest(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var friendship = database.Friends.Where(f => (f.idUser1 == idUser1) && (f.idUser2 == idUser2)).SingleOrDefault();

                if (friendship != null && friendship.statusRequest == constPending) {
                    friendship.statusRequest = constAccepted;
                    return database.SaveChanges();
                }

                return 0; 
            }
        }

        public static int DeleteFriend(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var friendship = database.Friends.Where(f => (f.idUser1 == idUser1) && (f.idUser2 == idUser2)).SingleOrDefault();

                if (friendship != null) {
                    database.Friends.Remove(friendship);
                    return database.SaveChanges();
                }

                return 0; 
            }
        }

        public static List<int> GetFriends(int userId) {
            using (var database = new GoatverseEntities()) {
                var idFriends = database.Friends.Where(f => (f.idUser1 == userId || f.idUser2 == userId) && f.statusRequest == constAccepted).Select(f => f.idUser1 == userId ? f.idUser2 : f.idUser1).ToList();

                return idFriends;
            }
        }

        public static bool IsFriend(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var friendship = database.Friends.Where(f => (f.idUser1 == idUser1) && (f.idUser2 == idUser2) && f.statusRequest == constAccepted).SingleOrDefault();

                return friendship != null;
            }
        }

        public static bool IsFriendRequestPending(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var pendingRequest = database.Friends.Where(f => (f.idUser1 == idUser1) && (f.idUser2 == idUser2) && f.statusRequest == constPending).SingleOrDefault();

                return pendingRequest != null;
            }
        }

        public static List<int> GetPendingFriendRequests(int userId) {
            using (var database = new GoatverseEntities()) {
                var idFriends = database.Friends.Where(f => (f.idUser2 == userId) && f.statusRequest == constPending).Select(f => f.idUser1).ToList();

                return idFriends;
            }
        }
    }
}
