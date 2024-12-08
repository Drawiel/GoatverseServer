using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DataAccess.DAOs {
    public static class UsersDAO {

        public static int AddUser(Users newUser) {
            try {

                using(var database = new GoatverseEntities()) {
                    database.Users.Add(newUser);
                    int result = database.SaveChanges();
                    return result;
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

        public static int DeleteUser(string username) {
            try {
                using(var database = new GoatverseEntities()) {
                    var delete = (from user in database.Users where user.username == username select user).Single();
                    database.Users.Remove(delete);
                    int result = database.SaveChanges();
                    return result;
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

        public static int UpdateUserPasswordAndUsernameByEmail(Users updatedUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var update = (from user in database.Users where user.email == updatedUser.email select user).Single();
                    string newPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.password);
                    update.username = updatedUser.username;
                    update.password = newPassword;
                    int result = database.SaveChanges();
                    return result;
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

        public static int UpdateUsernameByEmail(Users updatedUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var update = (from user in database.Users where user.email == updatedUser.email select user).Single();
                    update.username = updatedUser.username;
                    int result = database.SaveChanges();
                    return result;
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

        public static int UpdatePasswordByEmail(Users updatedUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var update = (from user in database.Users where user.email == updatedUser.email select user).Single();
                    string newPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.password);
                    update.password = newPassword;
                    int result = database.SaveChanges();
                    return result;
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

        public static int GetIdUserByUsername(string username) {
            try {
                using(var database = new GoatverseEntities()) {
                    var userId = database.Users.Where(u => u.username == username).Select(u => u.idUser).FirstOrDefault();
                    return userId;
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

        public static string GetEmailByIdUser(int idUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var email = database.Users.Where(u => u.idUser == idUser).Select(u => u.email).FirstOrDefault();
                    return email;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return null;
            } catch (InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                return null;
            } catch (Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return null;
            }
        }

        public static string GetUsernameByIdUser(int idUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var username = database.Users.Where(u => u.idUser == idUser).Select(u => u.username).FirstOrDefault();
                    return username;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                return null;
            } catch(InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                return null;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return null;
            }
        }
    }
}
