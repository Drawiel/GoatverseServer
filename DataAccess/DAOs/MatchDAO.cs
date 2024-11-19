using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataAccess.DAOs {
    public class MatchDAO {
        public int CreateMatch(DateTime startTime) {
            using(var database = new GoatverseEntities()) {
                var newMatch = new Matches {
                    startTime = startTime, 
                    endTime = null,        
                    idWinner = null        
                };

                database.Matches.Add(newMatch);

                int result = database.SaveChanges();

                return result;
            }
        }

        public Matches GetMatchById(int idMatch) {
            using(var database = new GoatverseEntities()) {
                return database.Matches.FirstOrDefault(m => m.idMatch == idMatch);
            }
        }

        public int UpdateMatch(int idMatch, int? idWinner, DateTime? endTime) {
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

        public List<Matches> GetRecentMatches(int topN) {
            using(var database = new GoatverseEntities()) {
                return database.Matches.OrderByDescending(m => m.startTime)
                                       .Take(topN)
                                       .ToList();
            }
        }



    }
}
