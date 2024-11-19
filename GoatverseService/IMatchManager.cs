using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService {
    namespace GoatverseService {

        [ServiceContract(CallbackContract = typeof(IMatchServiceCallback))]
        public interface IMatchManager {
            [OperationContract]
            bool ServiceJoinMatch(string username, string matchId);

            [OperationContract(IsOneWay = true)]
            void ServicePlayStack(string matchId, StackData stack);

            [OperationContract]
            List<CardData> ServiceDrawCards(string username, string matchId);
        }

        [ServiceContract]
        public interface IMatchServiceCallback {
            [OperationContract(IsOneWay = true)]
            void NotifyStackPlayed(StackData stack);

            [OperationContract(IsOneWay = true)]
            void NotifyPlayerCardUpdate(string username, List<CardData> newHand);
        }

        [DataContract]
        public class CardData {
            [DataMember]
            public string CardName { get; set; }

            [DataMember]
            public int CardId { get; set; }
        }

        [DataContract]
        public class StackData {
            [DataMember]
            public string Username { get; set; }

            [DataMember]
            public List<CardData> CardsInStack { get; set; }

            [DataMember]
            public string MatchId { get; set; }
        }

    }
}
