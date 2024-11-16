using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static GoatverseService.ServiceImplementation;

namespace GoatverseService {
    [ServiceContract(CallbackContract = typeof(IFriendsManagerCallback))]
    public interface IFriendsManager {
        [OperationContract]
        bool ServiceSendFriendRequest(string username1,  string username2);

        [OperationContract]
        bool ServiceRemoveFriend(string username1, string username2);

        [OperationContract]
        bool ServiceAcceptFriendRequest(string username1, string username2);

        [OperationContract]
        List<PlayerData> ServiceGetFriends(string username);

        [OperationContract]
        bool ServiceIsPendingFriendRequest(string username1, string username2);
    }

    [ServiceContract]
    public interface IFriendsManagerCallback { }

    [DataContract]
    public class FriendsData {
        private string sender;
        private string receiver;
        private string status;
        private int idFriendship;

        [DataMember]
        public string Sender { get { return sender; } set { sender = value; } }
        [DataMember]
        public string Status { get { return status; } set { status = value; } }
        [DataMember]
        public int IdFriendship { get { return idFriendship; } set { idFriendship = value; } }
        [DataMember]
        public string Receiver { get { return receiver; } set { receiver = value; } }

    }
}
