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
        int ServiceCountPlayersInLobby(string lobbycode);
    }

    [ServiceContract]
    public interface ILobbyServiceCallback {

        [OperationContract(IsOneWay = true)]
        void ServiceGetMessage(MessageData messageData);

        [OperationContract(IsOneWay = true)]
        void ServiceUpdatePlayersInLobby(List<PlayerData> players);
    }

    [DataContract]
    public class MessageData {

        private String username;
        private String idUser;
        private String message;
        private String lobbyCode;

        [DataMember]
        public String Username { get { return username; } set { username = value; } }
        [DataMember] 
        public String IdUser { get { return idUser; } set { idUser = value; } }
        [DataMember]
        public String Message { get { return message; } set { message = value; } }
        [DataMember]
        public String LobbyCode { get { return lobbyCode; } set { lobbyCode = value; } }
    }
}
