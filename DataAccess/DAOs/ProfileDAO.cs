using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs {
    public static class ProfileDAO {

        public static int AddProfile(Profile newProfile) {
            using (var database = new GoatverseEntities()) {
                database.Profile.Add(newProfile);
                int result = database.SaveChanges();
                return result;
            }
        }

        public static int DeleteProfile(int usernameId) {
            using (var database = new GoatverseEntities()) {
                var delete = (from profile in database.Profile where profile.idUser == usernameId select profile).Single();
                database.Profile.Remove(delete);
                int result = database.SaveChanges();
                return result;
            }
        }

        public static int UpdateProfile(Profile newProfile) {
            using (var database = new GoatverseEntities()) {
                var update = (from profile in database.Profile where profile.idUser == newProfile.idUser select profile).Single();
                update.profileLevel = newProfile.profileLevel;
                update.totalPoints = newProfile.totalPoints;
                update.matchesWon = newProfile.matchesWon;
                int result = database.SaveChanges();
                return result;
            }
        }

        public static int ChangeProfileImageByIdUser(int idUser, int imageId) { 
            using (var database = new GoatverseEntities()) {
                var change = (from profile in database.Profile where profile.idUser == idUser select profile).Single();
                change.imageId = imageId;
                int result = database.SaveChanges();
                return result;
            }
        }

        public static int GetImageIdByIdUser(int idUser) {
            using (var database = new GoatverseEntities()) {
                var imageId = database.Profile.Where(p => p.idUser == idUser).Select(p => p.imageId).FirstOrDefault();

                if(imageId == null) {
                    return 0;
                } else {
                    return (int)imageId;
                }
                
            }
        }

        public static int GetProfileLevelByIdUser(int idUser) {
            using (var database = new GoatverseEntities()) {
                var profileLevel = database.Profile.Where(p => p.idUser == idUser).Select(p => p.profileLevel).FirstOrDefault();

                if(profileLevel == null) {
                    return 0;
                }
                return (int)profileLevel;
            }
        }

        public static int GetMatchesWonByIdUser(int idUser) {
            using (var database = new GoatverseEntities()) {
                var matchesWon = database.Profile.Where(p => p.idUser == idUser).Select(p => p.matchesWon).FirstOrDefault();

                if (matchesWon == null) {
                    return 0;
                }
                return (int)matchesWon;
            }
        }
    }
}
