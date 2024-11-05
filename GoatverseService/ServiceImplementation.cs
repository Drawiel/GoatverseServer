using BCrypt.Net;
using DataAccess;
using DataAccess.DAOs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService
{
    public partial class ServiceImplementation : IUsersManager {

        public string ServiceGetEmail(string username) {
            UsersDAO userDAO = new UsersDAO();
            int userId = userDAO.GetIdUserByUsername(username);
            string email = userDAO.GetEmailByIdUser(userId);
            return email;
        }

        public bool ServicePasswordChanged(UserData userData) {
            UsersDAO userDAO = new UsersDAO();
            var changeData = new Users() {
                username = userData.Username,
                password = userData.Password,
                email = userData.Email
            };
            int result = userDAO.UpdateUserPasswordAndUsernameByEmail(changeData);
            return result == 1;
        }

        public bool ServiceTryLogin(UserData userData) {

            using (var database = new GoatverseEntities()) {
                
                var user = database.Users.SingleOrDefault(u => u.username == userData.Username);

                if (user == null) {
                    return false;
                }
                    
                        
                 if(BCrypt.Net.BCrypt.Verify(userData.Password, user.password)) {
                    Console.WriteLine("User: " + userData.Username + " has loged in");
                    return true;
                } else {
                    Console.WriteLine("Attemp to log in with username: " + userData.Username);
                    return false;
                }
            }
            
        }

        public bool ServiceTrySignIn(UserData userData) {

            if (ServiceUserExistsByUsername(userData.Username)) {
                return false;
            }

            using (var database = new GoatverseEntities()) {
                
                var newSignIn = new Users {
                    username = userData.Username,
                    password = BCrypt.Net.BCrypt.HashPassword(userData.Password),
                    email = userData.Email,
                };

                UsersDAO usersDAO = new UsersDAO();
                ProfileDAO profileDAO = new ProfileDAO();
                int result = usersDAO.AddUser(newSignIn);

                if (result == 1) {

                    var newProfile = new Profile {
                        idUser = usersDAO.GetIdUserByUsername(userData.Username),
                        profileLevel = 0,
                        totalPoints = 0,
                        matchesWon = 0,
                        imageId = 0,
                    };

                    int result2 = profileDAO.AddProfile(newProfile);

                    if(result2 == 1) {
                        Console.WriteLine("User added");
                        return true;
                    } else {
                        usersDAO.DeleteUser(userData.Username);
                        return false;
                    }
                } else {
                    return false;
                }
            }
        }

        public bool ServiceUserExistsByUsername(string userName) {

            using (var database = new GoatverseEntities()) {
                return database.Users.Any(u => u.username == userName);
            }
        }

        public bool ServiceVerifyPassword(string password, string username) {
            using (var database = new GoatverseEntities()) {
                
                var user = database.Users.SingleOrDefault(u => u.username == username);

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password)) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ServiceImplementation : ILobbyManager {

        private static ConcurrentDictionary<string, ConcurrentDictionary<string, ILobbyServiceCallback>> lobbiesDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, ILobbyServiceCallback>>();

        public bool ServiceConnectToLobby(string username, string lobbyCode) {

            if (lobbiesDictionary.ContainsKey(lobbyCode)){
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];

                if (lobby.ContainsKey(username)) {

                    return false;
                } else {

                    var callbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();
                    lobby.TryAdd(username, callbackChannel);
                    Console.WriteLine($"Usuario {username} se ha unido al lobby {lobbyCode}");
                    NotifyAllPlayersInLobby(lobbyCode);
                    return true;
                }
            } else {

                var callback = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = new ConcurrentDictionary<string, ILobbyServiceCallback>();
                lobby.TryAdd(username, callback);
                lobbiesDictionary.TryAdd(lobbyCode, lobby);
                Console.WriteLine($"Usuario {username} se ha unido al lobby {lobbyCode}");
                NotifyAllPlayersInLobby(lobbyCode);
                return true;
            }
        }

        public bool ServiceDisconnectFromLobby(string username, string lobbyCode) {
            if (lobbiesDictionary.ContainsKey(lobbyCode)) {

                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];
                if (lobby.ContainsKey(username)) {

                    ILobbyServiceCallback callback;
                    lobby.TryRemove(username, out callback);

                    if (lobby.IsEmpty) {

                        ConcurrentDictionary<string, ILobbyServiceCallback> removedUser;
                        lobbiesDictionary.TryRemove(lobbyCode, out removedUser);
                    } else {
                        NotifyAllPlayersInLobby(lobbyCode);
                    }

                    return true;
                }
            }

            return false;
        }

        public void ServiceSendMessageToLobby(MessageData messageData) {

            Console.WriteLine($"Mensaje recibido de {messageData.Username}: {messageData.Message}");
            var callbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();

            if (lobbiesDictionary.ContainsKey(messageData.LobbyCode)) {

                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[messageData.LobbyCode];

                if (!lobby.ContainsKey(messageData.Username)) {

                    bool userAdded = lobby.TryAdd(messageData.Username, callbackChannel);
                    if (userAdded) {
                        Console.WriteLine($"Usuario {messageData.Username} agregado al Lobby.");
                    } else {

                    }
                } else {

                    lobby[messageData.Username] = callbackChannel;

                }

                Console.WriteLine($"Usuarios actuales en el lobby {messageData.LobbyCode}:");
                foreach (var usersInLobby in lobby) {
                    try {
                        Console.WriteLine($"Usuario: {usersInLobby.Key}");
                        Console.WriteLine($"Mensaje enviado a: {usersInLobby.Key}");
                        usersInLobby.Value.ServiceGetMessage(messageData);
                    } catch (Exception ex) { 

                    }
                }
            } else { 
            }
        }

        private void NotifyAllPlayersInLobby(string lobbyCode) {
            if (lobbiesDictionary.ContainsKey(lobbyCode)) {
                var lobby = lobbiesDictionary[lobbyCode];

                List<PlayerData> playerList = new List<PlayerData>();

                foreach (var player in lobby.Keys) {
                    UsersDAO usersDAO = new UsersDAO();
                    ProfileDAO profileDAO = new ProfileDAO();

                    int idUser = usersDAO.GetIdUserByUsername(player);
                    int profileLevel = profileDAO.GetProfileLevelByIdUser(idUser);

                    playerList.Add(new PlayerData {
                        Username = player,
                        Level = profileLevel
                    });
                }

                Task.Run(() => { 
                    foreach (var playerCallback in lobby.Values) {
                        try {
                            playerCallback.ServiceUpdatePlayersInLobby(playerList);
                        } catch (Exception ex) {
                            Console.WriteLine($"Error al enviar datos al jugador: {ex.Message}");
                        }
                    }
                });
            }
        }

        public int ServiceCountPlayersInLobby(string lobbyCode) {
            int countUsers = 0;
            if (lobbiesDictionary.ContainsKey(lobbyCode)) {
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];
                foreach (var usersInLobby in lobby) {
                    countUsers++;
                }
            }

            return countUsers;
        } 


        

    }

    public partial class ServiceImplementation : IProfilesManager {
        public ProfileData ServiceLoadProfileData(string userName) {
            using(var database = new GoatverseEntities()) {
                UsersDAO usersDAO = new UsersDAO(); 
                int idUser = usersDAO.GetIdUserByUsername(userName);
                 

                ProfileDAO profileDAO = new ProfileDAO();
                var profileData = new ProfileData() {
                    ProfileLevel = profileDAO.GetProfileLevelByIdUser(idUser),
                    MatchesWon = profileDAO.GetMatchesWonByIdUser(idUser),
                    ImageId = profileDAO.GetImageIdByIdUser(idUser),

                };
                return profileData;
            }


        }
    }
}
