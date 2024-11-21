using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs {
    public class BlockedDAO {
        public int BlockUser(int idUserBlocked, int idUserBlocker) {
            using (var database = new GoatverseEntities()) {
                var newBlockUser = new Blocked {
                    idBlockedUser = idUserBlocked,
                    idBlocker = idUserBlocker
                };

                database.Blocked.Add(newBlockUser);
                return database.SaveChanges();
            }
        }

        public int DeleteBlock(int idUserBlocked, int idUserBlocker) {
            using (var database = new GoatverseEntities()) {
                var block = database.Blocked.Where(b => (b.idBlocker == idUserBlocker) && (b.idBlockedUser == idUserBlocked)).SingleOrDefault();

                if (block != null) { 
                    database.Blocked.Remove(block);
                    return database.SaveChanges();
                }

                return 0;
            }
        }

        public List<int> GetBlockedUsers(int idUserBlocker) {
            using (var database = new GoatverseEntities()) {
                var idBlockedUsers = database.Blocked.Where(b => (b.idBlocker == idUserBlocker)).Select(b => b.idBlockedUser).ToList();

                return idBlockedUsers;
            }
        }

        public bool IsUserBlocked(int idUserBlocker, int idUserBlocked) {
            using (var database = new GoatverseEntities()) {
                var isBlocked = database.Blocked.Where(b => (b.idBlocker == idUserBlocker) && (b.idBlockedUser == idUserBlocked)).SingleOrDefault();

                return isBlocked != null;
            }
        
        }
    }
}
