using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService {
    
    [ServiceContract]
    public interface IUsersManager {

        [OperationContract]
        bool ServiceTryLogin(UserData userData);

        [OperationContract]
        bool ServiceTrySignIn(UserData userData);

        [OperationContract]
        bool ServiceUserExistsByUsername(string userName);

        [OperationContract]
        bool ServiceVerifyPassword(string password, string username);

        [OperationContract]
        bool ServicePasswordChanged(UserData userData);

        [OperationContract]
        bool ServiceUsernameChanged(UserData userData);

        [OperationContract]
        bool ServicePasswordAndUsernameChanged(UserData userData);

        [OperationContract]
        string ServiceGetEmail(string username);
    }


    [DataContract]
    public class UserData {

        [DataMember]
        public String Username { get; set; }
        [DataMember]
        public String IdUser { get; set; }
        [DataMember]
        public String Message { get; set; }
        [DataMember]
        public String Email { get; set; }
        [DataMember]
        public String Password { get; set; }
    }

    [DataContract]
    public class PlayerData {

        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public int ImageId { get; set; }
    }
}
