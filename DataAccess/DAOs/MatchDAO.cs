using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DataAccess.DAOs {
    public static class MatchDAO {
        public static int CreateMatch(DateTime startTime) {

            try {
                using(var database = new GoatverseEntities()) {
                    if(startTime == null) {
                        throw new ArgumentException("startTime no puede ser null.");
                    }
                    var newMatch = new Matches {
                        startTime = startTime,
                    };

                    database.Matches.Add(newMatch);
                    int result = database.SaveChanges();

                    Console.WriteLine($"El resultado de la operación fue: {result}");
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
                if(ex.InnerException != null) {
                    Console.WriteLine($"Detalle de la excepción interna: {ex.InnerException.Message}");
                    Console.WriteLine($"StackTrace: {ex.InnerException.StackTrace}");
                }
                return -1;
            }
        }

        public static int GetMatchByStartTime(DateTime startTime) {
            try {
                using(var database = new GoatverseEntities()) {
                    return database.Matches.Where(m => m.startTime == startTime).Select(m => m.idMatch).FirstOrDefault();
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL al buscar partida por hora de inicio: {sqlEx.Message}");
                return -1;
            } catch(InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida al buscar partida por hora de inicio: {invOpEx.Message}");
                return -1;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado al buscar partida por hora de inicio: {ex.Message}");
                return -1;
            }
        }

        public static Matches GetMatchById(int idMatch) {
            try {
                using(var database = new GoatverseEntities()) {
                    return database.Matches.FirstOrDefault(m => m.idMatch == idMatch);
                }
            } catch(SqlException sqlEx) {
                Console.WriteLine($"Error SQL al buscar partida por ID: {sqlEx.Message}");
                return null;
            } catch(InvalidOperationException invOpEx) {
                Console.WriteLine($"Operación inválida al buscar partida por ID: {invOpEx.Message}");
                return null;
            } catch(Exception ex) {
                Console.WriteLine($"Error inesperado al buscar partida por ID: {ex.Message}");
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

    }
}

