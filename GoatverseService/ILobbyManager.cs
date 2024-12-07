using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static GoatverseService.ServiceImplementation;

namespace GoatverseService {

    [ServiceContract(CallbackContract = typeof(ILobbyServiceCallback))]
    public interface ILobbyManager {

        [OperationContract(IsOneWay = true)]
        void ServiceSendMessageToLobby(MessageData messageData);

        [OperationContract]
        bool ServiceConnectToLobby(string username, string lobbyCode);

        [OperationContract]
        bool ServiceDisconnectFromLobby(string username, string lobbyCode);

        [OperationContract]
        int ServiceCountPlayersInLobby(string lobbyCode);

        [OperationContract]
        bool ServiceCreateLobby(string username, string lobbyCode);

        [OperationContract]
        bool ServiceStartLobbyMatch(string lobbyCode, string username);
    }

    [ServiceContract]
    public interface ILobbyServiceCallback {

        [OperationContract(IsOneWay = true)]
        void ServiceGetMessage(MessageData messageData);

        [OperationContract(IsOneWay = true)]
        void ServiceUpdatePlayersInLobby(List<PlayerData> players);

        [OperationContract(IsOneWay = true)]
        void ServiceStartMatch(List<PlayerData> players);

        [OperationContract(IsOneWay = true)]
        void ServiceNotifyMatchStart();

        [OperationContract(IsOneWay = true)]
        void ServiceOwnerLeftLobby(string newOwner);

    }

    [DataContract]
    public class MessageData {

        [DataMember]
        public String Username { get; set; }
        [DataMember]
        public String IdUser { get; set; }
        [DataMember]
        public String Message { get; set; }
        [DataMember]
        public String LobbyCode { get; set; }
    }
}

    

