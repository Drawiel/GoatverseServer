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
    }

    [ServiceContract]
    public interface IUsersManagerCallback {
        void loginUser(string message);
    }
}
