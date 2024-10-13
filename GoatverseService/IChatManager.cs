using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService {

    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatManager {
    }

    [ServiceContract]
    public interface IChatServiceCallback { 
    }

    [DataContract]
    public class User {

        private String username;
        private String idUser;

        [DataMember]
        public String Username { get { return username; } set { username = value; } }
        [DataMember] 
        public String IdUser { get { return idUser; } set { idUser = value; } }
    }
}
