using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService.DAO {
    public class UsersDAO {
        public int GetUserIdByUsername(string username) { 
            using (var database = new GoatverseEntities()) {
                var userId = database.Users.Where(u => u.username == username).Select(u => u.idUser).FirstOrDefault();
                return userId;
            }
        }
    }
}
