using BCrypt.Net;
using DataAccess;
using DataAccess.DAOs;
using GoatverseService.GoatverseService;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Numerics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService {
    public partial class ServiceImplementation : IUsersManager {

        //
        public string ServiceGetEmail(string username) {
            UsersDAO userDAO = new UsersDAO();
            int userId = userDAO.GetIdUserByUsername(username);
            string email = userDAO.GetEmailByIdUser(userId);
            return email;
        }

        //
        public bool ServicePasswordChanged(UserData userData) {
            UsersDAO userDAO = new UsersDAO();
            var changeData = new Users() {
                password = userData.Password,
                email = userData.Email
            };
            int result = userDAO.UpdatePasswordByEmail(changeData);
            return result == 1;
        }

        public bool ServiceUsernameChanged(UserData userData) {
            UsersDAO userDAO = new UsersDAO();
            var changeData = new Users() {
                username = userData.Username,
                email = userData.Email
            };
            int result = userDAO.UpdateUsernameByEmail(changeData);
            return result == 1;
        }

        public bool ServicePasswordAndUsernameChanged(UserData userData) {
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

            using(var database = new GoatverseEntities()) {

                var user = database.Users.SingleOrDefault(u => u.username == userData.Username);

                if(user == null) {
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

            if(ServiceUserExistsByUsername(userData.Username)) {
                return false;
            }

            using(var database = new GoatverseEntities()) {

                var newSignIn = new Users {
                    username = userData.Username,
                    password = BCrypt.Net.BCrypt.HashPassword(userData.Password),
                    email = userData.Email,
                };

                UsersDAO usersDAO = new UsersDAO();
                ProfileDAO profileDAO = new ProfileDAO();
                int result = usersDAO.AddUser(newSignIn);

                if(result == 1) {

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

            using(var database = new GoatverseEntities()) {
                return database.Users.Any(u => u.username == userName);
            }
        }

        public bool ServiceVerifyPassword(string password, string username) {
            using(var database = new GoatverseEntities()) {

                var user = database.Users.SingleOrDefault(u => u.username == username);

                if(user != null && BCrypt.Net.BCrypt.Verify(password, user.password)) {
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

        public bool ServiceCreateLobby(string username, string lobbyCode) {

            if(lobbiesDictionary.ContainsKey(lobbyCode)) {
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];

                return false;

            } else {

                var callback = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = new ConcurrentDictionary<string, ILobbyServiceCallback>();
                lobbiesDictionary.TryAdd(lobbyCode, lobby);
                Console.WriteLine($"Usuario {username} ha creado el lobby {lobbyCode}");
                return true;
            }
        }

        public bool ServiceConnectToLobby(string username, string lobbyCode) {

            if(lobbiesDictionary.ContainsKey(lobbyCode)) {
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];

                if(lobby.ContainsKey(username)) {
                    return false;

                } else {
                    var callbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();
                    lobby.TryAdd(username, callbackChannel);
                    Console.WriteLine($"Usuario {username} se ha unido al lobby {lobbyCode}");
                    ServiceNotifyPlayersInLobby(lobbyCode);
                    return true;

                }
            } else {
                return false;

            }
        }

        public bool ServiceDisconnectFromLobby(string username, string lobbyCode) {
            if(lobbiesDictionary.ContainsKey(lobbyCode)) {

                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];
                if(lobby.ContainsKey(username)) {

                    ILobbyServiceCallback callback;
                    lobby.TryRemove(username, out callback);

                    if(lobby.IsEmpty) {

                        ConcurrentDictionary<string, ILobbyServiceCallback> removedUser;
                        lobbiesDictionary.TryRemove(lobbyCode, out removedUser);
                    } else {
                        ServiceNotifyPlayersInLobby(lobbyCode);
                    }

                    return true;
                }
            }

            return false;
        }

        public void ServiceSendMessageToLobby(MessageData messageData) {

            Console.WriteLine($"Mensaje recibido de {messageData.Username}: {messageData.Message}");
            var callbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();

            if(lobbiesDictionary.ContainsKey(messageData.LobbyCode)) {

                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[messageData.LobbyCode];

                if(!lobby.ContainsKey(messageData.Username)) {

                    bool userAdded = lobby.TryAdd(messageData.Username, callbackChannel);
                    if(userAdded) {
                        Console.WriteLine($"Usuario {messageData.Username} agregado al Lobby.");
                    } else {

                    }
                } else {

                    lobby[messageData.Username] = callbackChannel;

                }

                Console.WriteLine($"Usuarios actuales en el lobby {messageData.LobbyCode}:");
                foreach(var usersInLobby in lobby) {
                    try {
                        Console.WriteLine($"Usuario: {usersInLobby.Key}");
                        Console.WriteLine($"Mensaje enviado a: {usersInLobby.Key}");
                        usersInLobby.Value.ServiceGetMessage(messageData);
                    } catch(Exception ex) {

                    }
                }
            } else {
            }
        }

        private void ServiceNotifyPlayersInLobby(string lobbyCode) {
            if(lobbiesDictionary.ContainsKey(lobbyCode)) {
                var lobby = lobbiesDictionary[lobbyCode];

                List<PlayerData> playerList = new List<PlayerData>();

                foreach(var player in lobby.Keys) {
                    UsersDAO usersDAO = new UsersDAO();
                    ProfileDAO profileDAO = new ProfileDAO();

                    int idUser = usersDAO.GetIdUserByUsername(player);
                    int profileLevel = profileDAO.GetProfileLevelByIdUser(idUser);
                    int profileImageId = profileDAO.GetImageIdByIdUser(idUser);

                    playerList.Add(new PlayerData {
                        Username = player,
                        Level = profileLevel,
                        ImageId = profileImageId,
                    });
                }

                Task.Run(() => {
                    foreach(var playerCallback in lobby.Values) {
                        try {
                            playerCallback.ServiceUpdatePlayersInLobby(playerList);
                        } catch(Exception ex) {
                            Console.WriteLine($"Error al enviar datos al jugador: {ex.Message}");
                        }
                    }
                });
            }
        }

        //
        public int ServiceCountPlayersInLobby(string lobbyCode) {
            int countUsers = 0;
            if(lobbiesDictionary.ContainsKey(lobbyCode)) {
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];
                foreach(var usersInLobby in lobby) {
                    countUsers++;
                }
            }

            return countUsers;
        }




    }

    public partial class ServiceImplementation : IProfilesManager {
        //
        public ProfileData ServiceLoadProfileData(string username) {
            UsersDAO usersDAO = new UsersDAO();
            int idUser = usersDAO.GetIdUserByUsername(username);

            ProfileDAO profileDAO = new ProfileDAO();
            var profileData = new ProfileData() {
                ProfileLevel = profileDAO.GetProfileLevelByIdUser(idUser),
                MatchesWon = profileDAO.GetMatchesWonByIdUser(idUser),
                ImageId = profileDAO.GetImageIdByIdUser(idUser),
            };

            return profileData;
        }

        public bool ServiceChangeProfileImage(string username, int imageId) {
            UsersDAO usersDAO = new UsersDAO();
            int idUser = usersDAO.GetIdUserByUsername(username);
            ProfileDAO profileDAO = new ProfileDAO();
            int result = profileDAO.ChangeProfileImageByIdUser(idUser, imageId);

            return result == 1;
        }
    }

    public partial class ServiceImplementation : IFriendsManager {

        public bool ServiceAcceptFriendRequest(string username1, string username2) {
            UsersDAO usersDAO = new UsersDAO();
            int idSender = usersDAO.GetIdUserByUsername(username1);
            int idReceiver = usersDAO.GetIdUserByUsername(username2);

            FriendsDAO friendsDAO = new FriendsDAO();
            int result = friendsDAO.AcceptFriendRequest(idSender, idReceiver);

            return result == 1;
        }

        public bool ServiceSendFriendRequest(string username1, string username2) {
            UsersDAO usersDAO = new UsersDAO();
            int idSender = usersDAO.GetIdUserByUsername(username1);
            int idReceiver = usersDAO.GetIdUserByUsername(username2);

            FriendsDAO friendsDAO = new FriendsDAO();
            int result = friendsDAO.AddFriend(idSender, idReceiver);
            return result == 1;
        }

        public List<PlayerData> ServiceGetFriends(string username) {
            UsersDAO usersDAO = new UsersDAO();
            int idSender = usersDAO.GetIdUserByUsername(username);

            FriendsDAO friendsDAO = new FriendsDAO();
            List<int> listIdFriends = friendsDAO.GetFriends(idSender);
            List<PlayerData> friendsData = new List<PlayerData>();

            foreach(int idFriend in listIdFriends) {
                ProfileDAO profileDAO = new ProfileDAO();
                string usernameFriend = usersDAO.GetUsernameByIdUser(idFriend);
                int friendLevel = profileDAO.GetProfileLevelByIdUser(idFriend);
                int friendProfileImageId = profileDAO.GetImageIdByIdUser(idFriend);

                friendsData.Add(new PlayerData {
                    Username = usernameFriend,
                    Level = friendLevel,
                    ImageId = friendProfileImageId,
                });
            }

            return friendsData;
        }

        public bool ServiceRemoveFriend(string username1, string username2) {
            UsersDAO usersDAO = new UsersDAO();
            int idSender = usersDAO.GetIdUserByUsername(username1);
            int idReceiver = usersDAO.GetIdUserByUsername(username2);

            FriendsDAO friendsDAO = new FriendsDAO();
            int result = friendsDAO.DeleteFriend(idSender, idReceiver);
            return result == 1;
        }

        public bool ServiceIsPendingFriendRequest(string username1, string username2) {
            UsersDAO usersDAO = new UsersDAO();
            int idSender = usersDAO.GetIdUserByUsername(username1);
            int idReceiver = usersDAO.GetIdUserByUsername(username2);

            FriendsDAO friendsDAO = new FriendsDAO();
            bool result = friendsDAO.IsFriendRequestPending(idSender, idReceiver);
            return result;
        }

        public List<PlayerData> ServiceGetPendingFriendRequest(string username) {
            UsersDAO usersDAO = new UsersDAO();
            int idReceiver = usersDAO.GetIdUserByUsername(username);

            FriendsDAO friendsDAO = new FriendsDAO();
            List<int> listIdFriends = friendsDAO.GetPendingFriendRequests(idReceiver);
            List<PlayerData> friendsData = new List<PlayerData>();

            foreach(int idPendingFriend in listIdFriends) {
                ProfileDAO profileDAO = new ProfileDAO();
                string usernameFriend = usersDAO.GetUsernameByIdUser(idPendingFriend);
                int friendLevel = profileDAO.GetProfileLevelByIdUser(idPendingFriend);
                int friendProfileImageId = profileDAO.GetImageIdByIdUser(idPendingFriend);

                friendsData.Add(new PlayerData {
                    Username = usernameFriend,
                    Level = friendLevel,
                    ImageId = friendProfileImageId,
                });
            }

            return friendsData;
        }


    }

    public partial class MatchServiceImplementation : IMatchManager {
        private static ConcurrentDictionary<string, MatchSession> matchSessions =
            new ConcurrentDictionary<string, MatchSession>();

        public bool ServiceJoinMatch(string username, string matchId) {
            if(!matchSessions.ContainsKey(matchId)) {
                matchSessions.TryAdd(matchId, new MatchSession());
            }

            var match = matchSessions[matchId];
            var callbackChannel = OperationContext.Current.GetCallbackChannel<IMatchServiceCallback>();

            if(match.Players.ContainsKey(username)) {
                return false; 
            }

            match.Players.TryAdd(username, new PlayerSession {
                Username = username,
                Callback = callbackChannel,
                Cards = DrawInitialCards()
            });

            Console.WriteLine($"{username} joined match {matchId}");
            return true;
        }

        public void ServicePlayStack(string matchId, StackData stack) {
            if(matchSessions.TryGetValue(matchId, out var match)) {
                foreach(var player in match.Players.Values) {
                    try {
                        player.Callback.NotifyStackPlayed(stack);
                    }
                    catch(Exception ex) {
                        Console.WriteLine($"Error notifying player {player.Username}: {ex.Message}");
                    }
                }
            }
        }

        public List<CardData> ServiceDrawCards(string username, string matchId) {
            if(matchSessions.TryGetValue(matchId, out var match) &&
                match.Players.TryGetValue(username, out var player)) {
                while(player.Cards.Count < 5) {
                    player.Cards.Add(DrawRandomCard());
                }

                Task.Run(() => {
                    foreach(var otherPlayer in match.Players.Values) {
                        try {
                            otherPlayer.Callback.NotifyPlayerCardUpdate(username, player.Cards);
                        }
                        catch(Exception ex) {
                            Console.WriteLine($"Error notifying player {otherPlayer.Username}: {ex.Message}");
                        }
                    }
                });

                return player.Cards;
            }

            return null;
        }

        private List<CardData> DrawInitialCards() {
            var cards = new List<CardData>();
            for(int i = 0; i < 5; i++) {
                cards.Add(DrawRandomCard());
            }
            return cards;
        }

        private CardData DrawRandomCard() {
            var random = new Random();
            return new CardData {
                CardName = $"Card {random.Next(1, 50)}",
                CardId = random.Next(1, 50)
            };
        }
    }

    public class MatchSession {
        public ConcurrentDictionary<string, PlayerSession> Players { get; } =
            new ConcurrentDictionary<string, PlayerSession>();
    }

    public class PlayerSession {
        public string Username { get; set; }
        public List<CardData> Cards { get; set; }
        public IMatchServiceCallback Callback { get; set; }
    }

}
