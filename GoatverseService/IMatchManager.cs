using DataAccess.DAOs;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using static GoatverseService.ServiceImplementation;

namespace GoatverseService {

    [ServiceContract(CallbackContract = typeof(IMatchServiceCallback))]
    public interface IMatchManager {

        [OperationContract]
        MatchData ServiceCreateMatch(DateTime startTime);

        [OperationContract]
        MatchData ServiceGetMatchById(string matchId);

        [OperationContract]
        bool ServiceUpdateMatch(string matchId, string idWinner, DateTime? endTime);

        [OperationContract]
        List<MatchData> ServiceGetRecentMatches(int topN);

        [OperationContract]
        List<CardData> ServiceGetCards();

        // Nuevos métodos para manejar los turnos
        [OperationContract]
        void ServiceInitializeGameTurns(string gameCode, List<string> gamertags);

        [OperationContract]
        void ServiceNotifyEndTurn(string gameCode, string currentGamertag);

        [OperationContract]
        string ServiceGetCurrentTurn(string gameCode);

    }

    [ServiceContract]
    public interface IMatchServiceCallback {
        [OperationContract(IsOneWay = true)]
        void NotifyEndGame(string matchId, string winnerUsername);

        [OperationContract(IsOneWay = true)]
        void UpdateCurrentTurn(string currentTurn);

        [OperationContract(IsOneWay = true)]
        void SyncTimer();
    }

    public class MatchData {
        [DataMember]
        public string IdMatch { get; set; } // Corresponde a la columna "idMatch" de la base de datos

        [DataMember]
        public DateTime? StartTime { get; set; } // Corresponde a la columna "startTime"

        [DataMember]
        public DateTime? EndTime { get; set; } // Corresponde a la columna "endTime"

        [DataMember]
        public string IdWinner { get; set; } // Corresponde a la columna "idWinner"
    }
}
