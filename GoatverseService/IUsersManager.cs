using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService {
    
    [ServiceContract(CallbackContract = typeof(IUsersManagerCallback))]
    public interface IUsersManager {

        [OperationContract]
        bool tryLogin(string username, string password);

        [OperationContract]
        bool trySignIn(string username, string password, string email);
    }

    [ServiceContract]
    public interface IUsersManagerCallback {
        void loginUser(string message);
    }
}
