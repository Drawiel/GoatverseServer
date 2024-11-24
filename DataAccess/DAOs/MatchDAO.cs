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

                return newMatch.idMatch;
            }
        }

        public static Matches GetMatchById(int idMatch) {
            using(var database = new GoatverseEntities()) {
                return database.Matches.FirstOrDefault(m => m.idMatch == idMatch);
            }
        }

        public static int UpdateMatch(int idMatch, int? idWinner, DateTime? endTime) {
            using(var database = new GoatverseEntities()) {
                var match = database.Matches.FirstOrDefault(m => m.idMatch == idMatch);

                if(match != null) {
                    match.idWinner = idWinner; 
                    match.endTime = endTime;  

                    return database.SaveChanges(); 
                }

                return 0;
            }
        }

        public static List<Matches> GetRecentMatches(int topN) {
            using (var database = new GoatverseEntities()) {
                return database.Matches
                               .OrderByDescending(m => m.startTime) 
                               .Take(topN)  
                               .ToList(); 
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

