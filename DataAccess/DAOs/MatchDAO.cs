using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess.DAOs {
    public static class MatchDAO {
        // Crear una nueva partida
        public static int CreateMatch(DateTime startTime) {
            try {
                using(var database = new GoatverseEntities()) {
                    var newMatch = new Matches {
                        startTime = startTime,
                        endTime = null,
                        idWinner = null
                    };

                    database.Matches.Add(newMatch);
                    database.SaveChanges();

                    return newMatch.idMatch;
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

        public static Matches GetMatchById(int idMatch) {
            try {
                using(var database = new GoatverseEntities()) {
                    return database.Matches.FirstOrDefault(m => m.idMatch == idMatch);
                }
            } catch (SqlException sqlEx) {
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

        public static int UpdateMatch(int idMatch, int? idWinner, DateTime? endTime) {
            try {
                using(var database = new GoatverseEntities()) {
                    var match = database.Matches.FirstOrDefault(m => m.idMatch == idMatch);

                    if(match != null) {
                        match.idWinner = idWinner;
                        match.endTime = endTime;

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

        public static List<Matches> GetRecentMatches(int topN) {
            try {
                using(var database = new GoatverseEntities()) {
                    return database.Matches
                                   .OrderByDescending(m => m.startTime)
                                   .Take(topN)
                                   .ToList();
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

        /* Obtener los perfiles de los jugadores en una partida
        public static List<Profile> GetProfilesInMatch(int matchId) {
            using(var database = new GoatverseEntities()) {
                return database.Profile
                               .Where(p => p.IdMatch == matchId) 
                               .ToList();  
            }
        }*/
    }
}

