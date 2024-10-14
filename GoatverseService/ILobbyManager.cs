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
        void sendMessageToLobby(User user);

        [OperationContract]
        bool connectToLobby(string username, string lobbyCode);

        [OperationContract]
        bool disconnectFromLobby(string username, string lobbyCode);
    }

    [ServiceContract]
    public interface ILobbyServiceCallback {

        [OperationContract(IsOneWay = true)]
        void GetMessage(User user);

        [OperationContract]
        bool SuccessfulJoin();

        [OperationContract]
        bool SucessfulLeave();
    }

    [DataContract]
    public class User {

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
