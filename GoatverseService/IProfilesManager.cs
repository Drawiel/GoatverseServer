using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService {
    [ServiceContract(CallbackContract = typeof(IProfilesManagerCallback))]
    public interface IProfilesManager {
        [OperationContract]
        ProfileData ServiceLoadProfileData(string username);

        [OperationContract]
        bool ServiceChangeProfileImage(string username, int imageId);

    }
    [ServiceContract]
    internal interface IProfilesManagerCallback {

    }


    [DataContract]
    public class ProfileData {
        private int idProfile;
        private int profileLevel;
        private int totalPoints;
        private int matchesWon;
        private int idUser;
        private int imageId;

        [DataMember]
        public int IdProfile { get { return idProfile; } set { idProfile = value; } }
        [DataMember] 
        public int ProfileLevel { get { return profileLevel; } set { profileLevel = value; } }
        [DataMember] 
        public int TotalPoints { get { return totalPoints; } set { totalPoints = value; } }
        [DataMember]
        public int MatchesWon { get { return matchesWon; } set {  matchesWon = value; } }
        [DataMember]
        public int IdUser { get {   return idUser; } set {  idUser = value; } }
        [DataMember]
        public int ImageId { get { return imageId; } set { imageId = value; } }



    }
}
