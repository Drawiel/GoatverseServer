using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using static GoatverseService.ServiceImplementation;

namespace GoatverseService {
    [ServiceContract]
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

        [OperationContract]
        List<PlayerData> ServiceGetPendingFriendRequest(string username);

        [OperationContract]
        bool ServiceIsUserBlocked(string usernameBlocker, string usernameBlocked);

        [OperationContract]
        List<PlayerData> ServiceGetBlockedUsers(string username);

        [OperationContract]
        bool ServiceRemoveBlock(string usernameBlocker, string usernameBlocked);

        [OperationContract]
        bool ServiceBlockUser(string usernameBlocker, string usernameBlocked);
    }

    [DataContract]
    public class FriendsData {

        [DataMember]
        public string Sender { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public int IdFriendship { get; set; }
        [DataMember]
        public string Receiver { get; set; }

    }
}
