using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataAccess.DAOs {
    public class MatchDAO {
        public int CreateMatch(DateTime startTime) {
            using(var database = new GoatverseEntities()) {
                // Crear una nueva instancia de la partida
                var newMatch = new Matches {
                    startTime = startTime, // Establecer la hora de inicio
                    endTime = null,        // La hora de fin es nula al principio
                    idWinner = null        // No hay ganador al principio
                };

                // Agregar el nuevo objeto Matches a la base de datos
                database.Matches.Add(newMatch);

                // Guardar los cambios en la base de datos
                int result = database.SaveChanges();

                // Retornar el resultado (cantidad de registros afectados)
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
                    match.idWinner = idWinner;  // Puede ser null si no hay ganador aún
                    match.endTime = endTime;    // Puede ser null si la partida sigue en curso

                    return database.SaveChanges();
                }

                return 0; // Si no se encuentra la partida
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
