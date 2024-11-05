using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs {
    public class FriendsDAO {

        public int AddFriend(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var newFriendship = new Friends {
                    idUser1 = idUser1,
                    idUser2 = idUser2,
                    statusRequest = "Pending",
                };

                database.Friends.Add(newFriendship);
                return database.SaveChanges();
            }
        }

        public int AcceptFriendRequest(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var friendship = database.Friends.Where(f => (f.idUser1 == idUser1) && (f.idUser2 == idUser2)).SingleOrDefault();

                if (friendship != null && friendship.statusRequest == "Pending") {
                    friendship.statusRequest = "Accepted";
                    return database.SaveChanges();
                }

                return 0; 
            }
        }

        public int DeleteFriend(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var friendship = database.Friends.Where(f => (f.idUser1 == idUser1) && (f.idUser2 == idUser2)).SingleOrDefault();

                if (friendship != null) {
                    database.Friends.Remove(friendship);
                    return database.SaveChanges();
                }

                return 0; 
            }
        }

        public List<int> GetFriends(int userId) {
            using (var database = new GoatverseEntities()) {
                var friends = database.Friends.Where(f => (f.idUser1 == userId || f.idUser2 == userId) && f.statusRequest == "Accepted").Select(f => f.idUser1 == userId ? f.idUser2 : f.idUser1).ToList();

                return friends;
            }
        }

        public bool IsFriend(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var friendship = database.Friends.Where(f => (f.idUser1 == idUser1) && (f.idUser2 == idUser2) && f.statusRequest == "Accepted").SingleOrDefault();

                return friendship != null;
            }
        }

        public bool IsFriendRequestPending(int idUser1, int idUser2) {
            using (var database = new GoatverseEntities()) {
                var pendingRequest = database.Friends.Where(f => (f.idUser1 == idUser1) && (f.idUser2 == idUser2) && f.statusRequest == "Pending").SingleOrDefault();

                return pendingRequest != null;
            }
        }
    }
}
