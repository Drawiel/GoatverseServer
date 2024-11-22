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
    public interface IProfilesManager {
        [OperationContract]
        ProfileData ServiceLoadProfileData(string username);

        [OperationContract]
        bool ServiceChangeProfileImage(string username, int imageId);

        [OperationContract]
        ProfileData ServiceGetProfileByUserId(string userId);

    }


    [DataContract]
    public class ProfileData {

        [DataMember]
        public int IdProfile { get; set; }
        [DataMember] 
        public int ProfileLevel { get; set; }
        [DataMember] 
        public int TotalPoints { get; set; }
        [DataMember]
        public int MatchesWon { get; set; }
        [DataMember]
        public int IdUser { get; set; }
        [DataMember]
        public int ImageId { get; set; }



    }
}
