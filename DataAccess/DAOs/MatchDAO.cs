using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.DAOs {
    public static class MatchDAO {
        // Crear una nueva partida
        public static int CreateMatch(DateTime startTime) {
            using(var database = new GoatverseEntities()) {
                var newMatch = new Matches {
                    startTime = startTime,
                    endTime = null,
                    idWinner = null
                };

                database.Matches.Add(newMatch);
                database.SaveChanges();

                return newMatch.idMatch; // Retorna el idMatch como int
            }
        }

        // Obtener una partida por su id
        public static Matches GetMatchById(int idMatch) {
            using(var database = new GoatverseEntities()) {
                return database.Matches.FirstOrDefault(m => m.idMatch == idMatch);
            }
        }

        // Actualizar una partida (ganador y tiempo de finalización)
        public static int UpdateMatch(int idMatch, int? idWinner, DateTime? endTime) {
            using(var database = new GoatverseEntities()) {
                var match = database.Matches.FirstOrDefault(m => m.idMatch == idMatch);

                if(match != null) {
                    match.idWinner = idWinner;  // Asignamos el ganador
                    match.endTime = endTime;    // Asignamos el tiempo de fin

                    return database.SaveChanges(); // Guardamos los cambios en la base de datos
                }

                return 0; // Retornamos 0 si no encontramos la partida
            }
        }

        // Obtener las últimas N partidas
        public static List<Matches> GetRecentMatches(int topN) {
            using(var database = new GoatverseEntities()) {
                return database.Matches
                               .OrderByDescending(m => m.startTime)  // Ordenamos por tiempo de inicio, de más reciente a más antiguo
                               .Take(topN)  // Limitar el número de partidas
                               .ToList();  // Devolvemos la lista
            }
        }

        /* Obtener los perfiles de los jugadores en una partida
        public static List<Profile> GetProfilesInMatch(int matchId) {
            using(var database = new GoatverseEntities()) {
                return database.Profile
                               .Where(p => p.IdMatch == matchId)  // Filtrar por la relación existente entre partida y perfil
                               .ToList();  // Devolver la lista de perfiles
            }
        }*/
    }
}

