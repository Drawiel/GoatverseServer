using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs {
    public class UsersDAO {

        public int AddUser(Users newUser) {
            using (var database = new GoatverseEntities()) {
                database.Users.Add(newUser);
                int result = database.SaveChanges();
                return result;
            }
        }

        public int DeleteUser(string username) {
            using (var database = new GoatverseEntities()) {
                var delete = (from user in database.Users where user.username == username select user).Single();
                database.Users.Remove(delete);
                int result = database.SaveChanges();
                return result;
            }
        }

        public int UpdateUserPasswordAndUsernameByEmail(Users updatedUser) {
            using (var database = new GoatverseEntities()) { 
                var update = (from user in database.Users where user.email == updatedUser.email select user).Single();
                string newPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.password);
                update.username = updatedUser.username;
                update.password = newPassword;
                int result = database.SaveChanges();
                return result;
            }
        }

        public int UpdateUsernameByEmail(Users updatedUser) {
            using (var database = new GoatverseEntities()) {
                var update = (from user in database.Users where user.email == updatedUser.email select user).Single();
                update.username = updatedUser.username;
                int result = database.SaveChanges();
                return result;
            }
        }

        public int UpdatePasswordByEmail(Users updatedUser) {
            using (var database = new GoatverseEntities()) {
                var update = (from user in database.Users where user.email == updatedUser.email select user).Single();
                string newPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.password);
                update.password = newPassword;
                int result = database.SaveChanges();
                return result;
            }
        }

        public int GetIdUserByUsername(string username) { 
            using (var database = new GoatverseEntities()) {
                var userId = database.Users.Where(u => u.username == username).Select(u => u.idUser).FirstOrDefault();
                return userId;
            }
        }

        public string GetEmailByIdUser(int idUser) {
            using (var database = new GoatverseEntities()) {
                var email = database.Users.Where(u => u.idUser == idUser).Select(u => u.email).FirstOrDefault();
                return email;
            }
        }
    }
}
