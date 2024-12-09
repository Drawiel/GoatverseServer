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

        [OperationContract(IsOneWay = true)]
        void ServiceInitializeGameTurns(string gameCode);

        [OperationContract(IsOneWay = true)]
        void ServiceNotifyEndTurn(string gameCode, string currentGamertag);

        [OperationContract]
        string ServiceGetCurrentTurn(string gameCode);

        [OperationContract]
        bool ServiceConnectToGame(string username, string lobbyCode);

    }

    [ServiceContract]
    public interface IMatchServiceCallback {
        [OperationContract(IsOneWay = true)]
        void ServiceNotifyEndGame(string matchId, string winnerUsername);

        [OperationContract(IsOneWay = true)]
        void ServiceUpdateCurrentTurn(string currentTurn);

        [OperationContract(IsOneWay = true)]
        void ServiceSyncTimer();
    }

    public class MatchData {
        [DataMember]
        public string IdMatch { get; set; }

        [DataMember]
        public DateTime? StartTime { get; set; } 

        [DataMember]
        public DateTime? EndTime { get; set; } 

        [DataMember]
        public string IdWinner { get; set; }
    }
}
