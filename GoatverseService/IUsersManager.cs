using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService {
    
    [ServiceContract(CallbackContract = typeof(IUsersManagerCallback))]
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

    [ServiceContract]
    public interface IUsersManagerCallback {
        void ServiceLoginUser(string message);
    }

    [DataContract]
    public class UserData {

        private String username;
        private String idUser;
        private String message;
        private String email;
        private String password;

        [DataMember]
        public String Username { get { return username; } set { username = value; } }
        [DataMember]
        public String IdUser { get { return idUser; } set { idUser = value; } }
        [DataMember]
        public String Message { get { return message; } set { message = value; } }
        [DataMember]
        public String Email { get { return email; } set { email = value; } }
        [DataMember]
        public String Password { get { return password; } set { password = value; } }
    }

    [DataContract]
    public class PlayerData {
        private String username;
        private int level;

        [DataMember]
        public string Username { get { return username; } set { username = value; } }
        [DataMember]
        public int Level { get { return level; } set { level = value; } }
    }
}
