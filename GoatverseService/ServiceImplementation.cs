using BCrypt.Net;
using DataAccess;
using DataAccess.DAOs;
using static GoatverseService.ServiceImplementation;
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
            int userId = UsersDAO.GetIdUserByUsername(username);
            string email = UsersDAO.GetEmailByIdUser(userId);
            return email;
        }

        //
        public bool ServicePasswordChanged(UserData userData) {
            var changeData = new Users() {
                password = userData.Password,
                email = userData.Email
            };
            int result = UsersDAO.UpdatePasswordByEmail(changeData);
            return result == 1;
        }

        public bool ServiceUsernameChanged(UserData userData) {
            var changeData = new Users() {
                username = userData.Username,
                email = userData.Email
            };
            int result = UsersDAO.UpdateUsernameByEmail(changeData);
            return result == 1;
        }

        public bool ServicePasswordAndUsernameChanged(UserData userData) {
            var changeData = new Users() {
                username = userData.Username,
                password = userData.Password,
                email = userData.Email
            };
            int result = UsersDAO.UpdateUserPasswordAndUsernameByEmail(changeData);
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

                int result = UsersDAO.AddUser(newSignIn);

                if(result == 1) {

                    var newProfile = new Profile {
                        idUser = UsersDAO.GetIdUserByUsername(userData.Username),
                        profileLevel = 0,
                        totalPoints = 0,
                        matchesWon = 0,
                        imageId = 0,
                    };

                    int result2 = ProfileDAO.AddProfile(newProfile);

                    if(result2 == 1) {
                        Console.WriteLine("User added");
                        return true;
                    } else {
                        UsersDAO.DeleteUser(userData.Username);
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

                    lobby.TryRemove(username, out ILobbyServiceCallback callback);

                    if (lobby.IsEmpty) {

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
                        Console.WriteLine($"Error debido enviado a: {ex.Message}");
                    }
                }
            } 
        }

        private void ServiceNotifyPlayersInLobby(string lobbyCode) {
            if(lobbiesDictionary.ContainsKey(lobbyCode)) {
                var lobby = lobbiesDictionary[lobbyCode];

                List<PlayerData> playerList = new List<PlayerData>();

                foreach(var player in lobby.Keys) {

                    int idUser = UsersDAO.GetIdUserByUsername(player);
                    int profileLevel = ProfileDAO.GetProfileLevelByIdUser(idUser);
                    int profileImageId = ProfileDAO.GetImageIdByIdUser(idUser);

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

        public static void StartMatch(string lobbyCode) {
            if(lobbiesDictionary.TryGetValue(lobbyCode, out var lobby)) {
                Task.Run(() => {
                    foreach(var playerCallback in lobby.Values) {
                        try {
                            playerCallback.ServiceNotifyMatchStart();
                        }
                        catch(Exception ex) {
                            Console.WriteLine($"Error notificando al jugador: {ex.Message}");
                        }
                    }
                });
            }
        }

        public bool ServiceStartLobbyMatch(string lobbyCode) {
            if(lobbiesDictionary.ContainsKey(lobbyCode)) {
                StartMatch(lobbyCode);
                return true;
            }
            return false;
        }


    }

    public partial class ServiceImplementation : IProfilesManager {
 
        public ProfileData ServiceLoadProfileData(string username) {
            int idUser = UsersDAO.GetIdUserByUsername(username);

            var profileData = new ProfileData() {
                ProfileLevel = ProfileDAO.GetProfileLevelByIdUser(idUser),
                MatchesWon = ProfileDAO.GetMatchesWonByIdUser(idUser),
                ImageId = ProfileDAO.GetImageIdByIdUser(idUser),
            };

            return profileData;
        }

        public bool ServiceChangeProfileImage(string username, int imageId) {
            int idUser = UsersDAO.GetIdUserByUsername(username);
            int result = ProfileDAO.ChangeProfileImageByIdUser(idUser, imageId);

            return result == 1;
        }

        public ProfileData ServiceGetProfileByUserId(string userId) {
            return ServiceGetProfileByUserId(userId); 
        }

    }

    public partial class ServiceImplementation : IFriendsManager {

        public bool ServiceAcceptFriendRequest(string username1, string username2) {
            
            int idSender = UsersDAO.GetIdUserByUsername(username1);
            int idReceiver = UsersDAO.GetIdUserByUsername(username2);

            int result = FriendsDAO.AcceptFriendRequest(idSender, idReceiver);

            return result == 1;
        }

        public bool ServiceSendFriendRequest(string username1, string username2) {
            
            int idSender = UsersDAO.GetIdUserByUsername(username1);
            int idReceiver = UsersDAO.GetIdUserByUsername(username2);

            int result = FriendsDAO.AddFriend(idSender, idReceiver);
            return result == 1;
        }

        public List<PlayerData> ServiceGetFriends(string username) {
            
            int idRequest = UsersDAO.GetIdUserByUsername(username);

            List<int> listIdFriends = FriendsDAO.GetFriends(idRequest);
            List<PlayerData> friendsData = new List<PlayerData>();


            foreach(int idFriend in listIdFriends) {
                if (!BlockedDAO.IsUserBlocked(idRequest, idFriend)) {
                    string usernameFriend = UsersDAO.GetUsernameByIdUser(idFriend);
                    int friendLevel = ProfileDAO.GetProfileLevelByIdUser(idFriend);
                    int friendProfileImageId = ProfileDAO.GetImageIdByIdUser(idFriend);

                    friendsData.Add(new PlayerData {
                        Username = usernameFriend,
                        Level = friendLevel,
                        ImageId = friendProfileImageId,
                    });
                }
            }

            return friendsData;
        }

        public List<PlayerData> ServiceGetBlockedUsers(string username) {
            int idRequest = UsersDAO.GetIdUserByUsername(username);
            
            List<int> listIdFriends = BlockedDAO.GetBlockedUsers(idRequest);
            List<PlayerData> blockedData = new List<PlayerData>();

            foreach (int idBlocked in listIdFriends) {
                
                string usernameFriend = UsersDAO.GetUsernameByIdUser(idBlocked);
                int friendLevel = ProfileDAO.GetProfileLevelByIdUser(idBlocked);
                int friendProfileImageId = ProfileDAO.GetImageIdByIdUser(idBlocked);

                blockedData.Add(new PlayerData {
                    Username = usernameFriend,
                    Level = friendLevel,
                    ImageId = friendProfileImageId,
                });
                
            }

            return blockedData;
        }

        public bool ServiceRemoveFriend(string username1, string username2) {
            
            int idSender = UsersDAO.GetIdUserByUsername(username1);
            int idReceiver = UsersDAO.GetIdUserByUsername(username2);

            int result = FriendsDAO.DeleteFriend(idSender, idReceiver);
            return result == 1;
        }

        public bool ServiceIsPendingFriendRequest(string username1, string username2) {
            
            int idSender = UsersDAO.GetIdUserByUsername(username1);
            int idReceiver = UsersDAO.GetIdUserByUsername(username2);

            bool result = FriendsDAO.IsFriendRequestPending(idSender, idReceiver);
            return result;
        }

        public List<PlayerData> ServiceGetPendingFriendRequest(string username) {
            
            int idReceiver = UsersDAO.GetIdUserByUsername(username);

            List<int> listIdFriends = FriendsDAO.GetPendingFriendRequests(idReceiver);
            List<PlayerData> friendsData = new List<PlayerData>();

            foreach(int idPendingFriend in listIdFriends) {
                string usernameFriend = UsersDAO.GetUsernameByIdUser(idPendingFriend);
                int friendLevel = ProfileDAO.GetProfileLevelByIdUser(idPendingFriend);
                int friendProfileImageId = ProfileDAO.GetImageIdByIdUser(idPendingFriend);

                friendsData.Add(new PlayerData {
                    Username = usernameFriend,
                    Level = friendLevel,
                    ImageId = friendProfileImageId,
                });
            }

            return friendsData;
        }

        public bool ServiceIsUserBlocked(string usernameBlocker, string usernameBlocked) {
            
            int idBlocker = UsersDAO.GetIdUserByUsername(usernameBlocker);
            int idBlocked = UsersDAO.GetIdUserByUsername(usernameBlocked);

            bool result = BlockedDAO.IsUserBlocked(idBlocker, idBlocked);
            return result;
        }

        public bool ServiceRemoveBlock(string usernameBlocker, string usernameBlocked) {
            
            int idBlocker = UsersDAO.GetIdUserByUsername(usernameBlocker);
            int idBlocked = UsersDAO.GetIdUserByUsername(usernameBlocked);

            int result = BlockedDAO.DeleteBlock(idBlocker, idBlocked);
            return result == 1;
        }

        public bool ServiceBlockUser(string usernameBlocker, string usernameBlocked) {
            
            int idBlocker = UsersDAO.GetIdUserByUsername(usernameBlocker);
            int idBlocked = UsersDAO.GetIdUserByUsername(usernameBlocked);

            int result = BlockedDAO.BlockUser(idBlocker, idBlocked);
            return result == 1;
        }


    }

    public partial class ServiceImplementation : IMatchManager {
        private static Dictionary<string, List<string>> playersInGame = new Dictionary<string, List<string>>();
        private static Dictionary<string, string> currentTurnByGame = new Dictionary<string, string>();
        private static Dictionary<string, bool> turnTransitionState = new Dictionary<string, bool>();
        private static Dictionary<string, Dictionary<string, IMatchServiceCallback>> gameConnections = new Dictionary<string, Dictionary<string, IMatchServiceCallback>>();
        private static Dictionary<string, string> currentTurnsByGame = new Dictionary<string, string>(); 
        private static Dictionary<string, DateTime> gameTimers = new Dictionary<string, DateTime>();  


        public void ServiceInitializeGameTurns(string gameCode, List<string> gamertags) {
            playersInGame[gameCode] = gamertags.OrderBy(_ => Guid.NewGuid()).ToList();
            var firstPlayer = playersInGame[gameCode].First();
            currentTurnByGame[gameCode] = firstPlayer;
            turnTransitionState[gameCode] = false;
            ServiceNotifyClientOfTurn(gameCode, firstPlayer);
        }

        public void ServiceNotifyEndTurn(string gameCode, string currentGamertag) {
            if(turnTransitionState.ContainsKey(gameCode) && !turnTransitionState[gameCode]) {
                if(playersInGame.ContainsKey(gameCode)) {
                    var players = playersInGame[gameCode];
                    int currentIndex = players.IndexOf(currentGamertag);

                    int nextIndex = (currentIndex + 1) % players.Count;
                    var nextGametag = players[nextIndex];

                    currentTurnByGame[gameCode] = nextGametag;
                    turnTransitionState[gameCode] = true;
                    ServiceNotifyClientOfTurn(gameCode, nextGametag);
                }
            }
        }

        public string ServiceGetCurrentTurn(string gameCode) {
            return currentTurnByGame.ContainsKey(gameCode) ? currentTurnByGame[gameCode] : null;
        }

        private void ServiceNotifyClientOfTurn(string gameCode, string nextGametag) {
            if(gameConnections.ContainsKey(gameCode)) {
                var playersInGame = gameConnections[gameCode];
                foreach(var player in playersInGame) {
                    IMatchServiceCallback callback = player.Value as IMatchServiceCallback;
                    if(callback != null) {
                        callback.ServiceUpdateCurrentTurn(nextGametag); 
                        callback.ServiceSyncTimer();  
                    }
                }
                ServiceResetTurnTransitionState(gameCode);
            }
        }

        private void ServiceResetTurnTransitionState(string gameCode) {
            if(turnTransitionState.ContainsKey(gameCode)) {
                turnTransitionState[gameCode] = false;
            }
        }

        public MatchData ServiceCreateMatch(DateTime startTime) {
            int idMatch = MatchDAO.CreateMatch(startTime);

            return new MatchData {
                IdMatch = idMatch.ToString(),
                StartTime = startTime,
                EndTime = null,
                IdWinner = null
            };
        }

        public MatchData ServiceGetMatchById(string matchId) {
            if(!int.TryParse(matchId, out int id))
                throw new ArgumentException("El ID de la partida debe ser un número.");

            var match = MatchDAO.GetMatchById(id);

            if(match == null)
                return null;

            return new MatchData {
                IdMatch = match.idMatch.ToString(),
                StartTime = match.startTime,
                EndTime = match.endTime,
                IdWinner = match.idWinner?.ToString()
            };
        }

        public bool ServiceUpdateMatch(string matchId, string idWinner, DateTime? endTime) {
            if(!int.TryParse(matchId, out int idMatch))
                throw new ArgumentException("El ID de la partida debe ser un número.");

            int? idWinnerParsed = null;
            if(!string.IsNullOrEmpty(idWinner)) {
                if(!int.TryParse(idWinner, out int id))
                    throw new ArgumentException("El ID del ganador debe ser un número.");
                idWinnerParsed = id;
            }

            int result = MatchDAO.UpdateMatch(idMatch, idWinnerParsed, endTime);

            return result > 0; 
        }

        public List<MatchData> ServiceGetRecentMatches(int topN) {
            var matches = MatchDAO.GetRecentMatches(topN);

            var matchDataList = new List<MatchData>();
            foreach(var match in matches) {
                matchDataList.Add(new MatchData {
                    IdMatch = match.idMatch.ToString(),
                    StartTime = match.startTime,
                    EndTime = match.endTime,
                    IdWinner = match.idWinner?.ToString()
                });
            }

            return matchDataList;
        }

        // Obtener todas las cartas
        public List<CardData> ServiceGetCards() {
            var cards = CardsDAO.GetAllCards();

            var cardDataList = new List<CardData>();
            foreach(var card in cards) {
                cardDataList.Add(new CardData {
                    IdCard = card.idCard, 
                    CardName = card.cardName,
                    Points = card.points ?? 0, 
                    CardType = card.cardType,
                    Description = card.description,
                    EffectDescription = card.effectDescription,
                    ImageCardId = card.imageCardId.HasValue ? card.imageCardId.Value : 0
                });
            }

            return cardDataList;
        }

    }

    public partial class ServiceImplementation : ICardsManager {

        public List<CardData> ServiceGetAllCards() {
            var cards = CardsDAO.GetAllCards();

            // Convertimos las entidades a un formato más amigable para el cliente
            var cardDataList = new List<CardData>();
            foreach(var card in cards) {
                cardDataList.Add(new CardData {
                    IdCard = card.idCard,
                    CardName = card.cardName,
                    Points = card.points ?? 0,
                    CardType = card.cardType,
                    Description = card.description,
                    EffectDescription = card.effectDescription,
                    ImageCardId = card.imageCardId ?? 0
                });
            }

            return cardDataList;
        }

        public CardData ServiceGetCardById(int id) {
            var card = CardsDAO.GetCardById(id);

            if(card != null) {
                return new CardData {
                    IdCard = card.idCard,
                    CardName = card.cardName,
                    Points = card.points ?? 0,
                    CardType = card.cardType,
                    Description = card.description,
                    EffectDescription = card.effectDescription,
                    ImageCardId = card.imageCardId ?? 0
                };
            }

            return null;
        }

        public bool ServiceAddCard(CardData cardData) {

            var newCard = new Cards {
                cardName = cardData.CardName,
                points = cardData.Points,
                cardType = cardData.CardType,
                description = cardData.Description,
                effectDescription = cardData.EffectDescription,
                imageCardId = cardData.ImageCardId
            };

            int result = CardsDAO.AddCard(newCard);
            return result > 0;
        }

        public bool ServiceDeleteCard(int id) {
            int result = CardsDAO.DeleteCard(id);
            return result > 0;
        }
    }

}
