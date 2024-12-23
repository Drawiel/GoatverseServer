using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs {
    public static class ProfileDAO {

        public static int AddProfile(Profile newProfile) {
            try {
                using(var database = new GoatverseEntities()) {
                    database.Profile.Add(newProfile);
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

        public static int DeleteProfile(int usernameId) {
            try {
                using(var database = new GoatverseEntities()) {
                    var delete = (from profile in database.Profile where profile.idUser == usernameId select profile).Single();
                    database.Profile.Remove(delete);
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

        public static int UpdateProfile(Profile newProfile) {
            try {
                using(var database = new GoatverseEntities()) {
                    var update = (from profile in database.Profile where profile.idUser == newProfile.idUser select profile).Single();
                    update.profileLevel = newProfile.profileLevel;
                    update.totalPoints = newProfile.totalPoints;
                    update.matchesWon = newProfile.matchesWon;
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

        public static int ChangeProfileImageByIdUser(int idUser, int imageId) {
            try {
                using(var database = new GoatverseEntities()) {
                    var change = (from profile in database.Profile where profile.idUser == idUser select profile).Single();
                    change.imageId = imageId;
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

        public static void IncrementMatchesWon(string username) {
            try {
                using(var database = new GoatverseEntities()) {
                    var user = database.Users.FirstOrDefault(u => u.username == username);

                    if(user != null) {
                        var userProfile = database.Profile.FirstOrDefault(p => p.idUser == user.idUser);

                        if(userProfile != null) {
                            userProfile.matchesWon += 1;


                            database.SaveChanges();
                        } else {
                            Console.WriteLine($"No se encontró el perfil para el usuario con username: {username}");
                        }
                    } else {
                        Console.WriteLine($"No se encontró un usuario con el username: {username}");
                    }
                }
            } catch(Exception ex) {

                Console.WriteLine($"Error al incrementar las partidas ganadas para el usuario {username}: {ex.Message}");
            }
        }

        public static int GetWonMatchesByUsername(string username) {
            try {
                using(var database = new GoatverseEntities()) {
                    var user = database.Users.FirstOrDefault(u => u.username == username);

                    if(user != null) {
                        var userProfile = database.Profile.FirstOrDefault(p => p.idUser == user.idUser);

                        if(userProfile != null) {
                            return userProfile.matchesWon ?? 0;
                        }
                    }

                    return 0;
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al obtener el número de partidas ganadas para el usuario {username}: {ex.Message}");
                return -1;
            }
        }


        public static int GetImageIdByIdUser(int idUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var imageId = database.Profile.Where(p => p.idUser == idUser).Select(p => p.imageId).FirstOrDefault();

                    if(imageId == null) {
                        return 0;
                    }
                    else {
                        return (int)imageId;
                    }

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

        public static int GetProfileLevelByIdUser(int idUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var profileLevel = database.Profile.Where(p => p.idUser == idUser).Select(p => p.profileLevel).FirstOrDefault();

                    if(profileLevel == null) {
                        return 0;
                    }
                    return (int)profileLevel;
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

        public static int GetMatchesWonByIdUser(int idUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var matchesWon = database.Profile.Where(p => p.idUser == idUser).Select(p => p.matchesWon).FirstOrDefault();

                    if(matchesWon == null) {
                        return 0;
                    }
                    return (int)matchesWon;
                }
            } catch(SqlException sqlEx) {
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

        public static Profile GetProfileByUserId(int idUser) {
            try {
                using(var database = new GoatverseEntities()) {
                    var profile = (from p in database.Profile
                                   where p.idUser == idUser
                                   select p).SingleOrDefault();

                    return profile;
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL: {sqlEx.Message}");
                Profile profile = new Profile() { idProfile = -1};
                return profile;
            } catch(InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida: {invOpEx.Message}");
                Profile profile = new Profile() { idProfile = -1 };
                return profile;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                Profile profile = new Profile() { idProfile = -1 };
                return profile;
            }
        }

    }
}
