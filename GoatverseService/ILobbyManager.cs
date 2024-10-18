using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService {

    [ServiceContract(CallbackContract = typeof(ILobbyServiceCallback))]
    public interface ILobbyManager {

        [OperationContract(IsOneWay = true)]
        void ServiceSendMessageToLobby(MessageData messageData);

        [OperationContract]
        bool ServiceConnectToLobby(string username, string lobbyCode);

        [OperationContract]
        bool ServiceDisconnectFromLobby(string username, string lobbyCode);
    }

    [ServiceContract]
    public interface ILobbyServiceCallback {

        [OperationContract(IsOneWay = true)]
        void ServiceGetMessage(MessageData messageData);

        [OperationContract]
        bool ServiceSuccessfulJoin();

        [OperationContract]
        bool ServiceSucessfulLeave();
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
