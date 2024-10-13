using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService
{
    public partial class ServiceImplementation : IUsersManager {

        public bool tryLogin(string username, string password) {

            if (username == "null" && password == "null") {
                return true;
            } else {
                return false;
            }
        }
    }
}
