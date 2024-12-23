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
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace GoatverseService {
    public partial class ServiceImplementation : IUsersManager {

        //
        public string ServiceGetEmail(string username) {
            try {
                int userId = UsersDAO.GetIdUserByUsername(username);
                string email = UsersDAO.GetEmailByIdUser(userId);
                return email;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        //
        public bool ServicePasswordChanged(UserData userData) {
            try {
                var changeData = new Users() {
                    password = userData.Password,
                    email = userData.Email
                };
                int result = UsersDAO.UpdatePasswordByEmail(changeData);
                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceUsernameChanged(UserData userData) {
            try {
                var changeData = new Users() {
                    username = userData.Username,
                    email = userData.Email
                };
                int result = UsersDAO.UpdateUsernameByEmail(changeData);
                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServicePasswordAndUsernameChanged(UserData userData) {
            try {
                var changeData = new Users() {
                    username = userData.Username,
                    password = userData.Password,
                    email = userData.Email
                };
                int result = UsersDAO.UpdateUserPasswordAndUsernameByEmail(changeData);
                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceTryLogin(UserData userData) {
            try {

                using (var database = new GoatverseEntities()) {

                    var user = database.Users.SingleOrDefault(u => u.username == userData.Username);

                    if (user == null) {
                        return false;
                    }


                    if (BCrypt.Net.BCrypt.Verify(userData.Password, user.password)) {
                        Console.WriteLine("User: " + userData.Username + " has loged in");
                        return true;
                    } else {
                        Console.WriteLine("Attemp to log in with username: " + userData.Username);
                        return false;
                    }
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }

        }

        public bool ServiceTrySignIn(UserData userData) {
            try {

                if (ServiceUserExistsByUsername(userData.Username)) {
                    return false;
                }

                using (var database = new GoatverseEntities()) {

                    var newSignIn = new Users {
                        username = userData.Username,
                        password = BCrypt.Net.BCrypt.HashPassword(userData.Password),
                        email = userData.Email,
                    };

                    int result = UsersDAO.AddUser(newSignIn);

                    if (result == 1) {

                        var newProfile = new Profile {
                            idUser = UsersDAO.GetIdUserByUsername(userData.Username),
                            profileLevel = 0,
                            totalPoints = 0,
                            matchesWon = 0,
                            imageId = 0,
                        };

                        int result2 = ProfileDAO.AddProfile(newProfile);

                        if (result2 == 1) {
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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceUserExistsByUsername(string userName) {
            try {

                using (var database = new GoatverseEntities()) {
                    return database.Users.Any(u => u.username == userName);
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceVerifyPassword(string password, string username) {
            try {
                using (var database = new GoatverseEntities()) {

                    var user = database.Users.SingleOrDefault(u => u.username == username);

                    if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password)) {
                        return true;
                    } else {
                        return false;
                    }
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public partial class ServiceImplementation : ILobbyManager {

        private static ConcurrentDictionary<string, ConcurrentDictionary<string, ILobbyServiceCallback>> lobbiesDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, ILobbyServiceCallback>>();
        private static ConcurrentDictionary<string, string> lobbyOwnersDictionary = new ConcurrentDictionary<string, string>();


        public bool ServiceCreateLobby(string username, string lobbyCode) {
            try {

                if (lobbiesDictionary.ContainsKey(lobbyCode)) {

                    return false;
                } else {

                    ConcurrentDictionary<string, ILobbyServiceCallback> lobby = new ConcurrentDictionary<string, ILobbyServiceCallback>();
                    lobbiesDictionary.TryAdd(lobbyCode, lobby);
                    lobbyOwnersDictionary.TryAdd(lobbyCode, username);
                    Console.WriteLine($"Usuario {username} ha creado el lobby {lobbyCode}");
                    return true;
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceConnectToLobby(string username, string lobbyCode) {
            try {

                if (lobbiesDictionary.ContainsKey(lobbyCode)) {
                    ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];

                    if (lobby.ContainsKey(username)) {
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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceDisconnectFromLobby(string username, string lobbyCode) {
            try {
                if (lobbiesDictionary.TryGetValue(lobbyCode, out var lobby)) {
                    if (lobby.TryRemove(username, out _)) {
                        if (lobbyOwnersDictionary.TryGetValue(lobbyCode, out var owner) && owner == username) {
                            ReassignLobbyOwner(lobbyCode, lobby);
                        }

                        if (lobby.IsEmpty) {
                            lobbiesDictionary.TryRemove(lobbyCode, out _);
                            lobbyOwnersDictionary.TryRemove(lobbyCode, out _);

                            Console.WriteLine($"Lobby {lobbyCode} eliminado porque quedó vacío.");
                        } else {
                            ServiceNotifyPlayersInLobby(lobbyCode);
                        }

                        Console.WriteLine($"Usuario {username} salió del lobby {lobbyCode}");
                        return true;
                    }
                }
                return false;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        private void ReassignLobbyOwner(string lobbyCode, ConcurrentDictionary<string, ILobbyServiceCallback> lobby) {
            try {
                if (lobby.Keys.FirstOrDefault() is string newOwner) {
                    lobbyOwnersDictionary[lobbyCode] = newOwner;

                    Task.Run(() => {
                        foreach (var callback in lobby.Values) {
                            try {
                                callback.ServiceOwnerLeftLobby(newOwner);
                            } catch (Exception ex) {
                                Console.WriteLine($"Error notificando cambio de propietario: {ex.Message}");
                            }
                        }
                    });

                    Console.WriteLine($"Nuevo propietario del lobby {lobbyCode}: {newOwner}");
                } else {
                    Console.WriteLine($"Lobby {lobbyCode} quedó sin propietario y será eliminado.");
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public void ServiceSendMessageToLobby(MessageData messageData) {
            try {
                Console.WriteLine($"Mensaje recibido de {messageData.Username}: {messageData.Message}");
                var callbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();

                if (lobbiesDictionary.ContainsKey(messageData.LobbyCode)) {

                    ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[messageData.LobbyCode];

                    if (!lobby.ContainsKey(messageData.Username)) {

                        bool userAdded = lobby.TryAdd(messageData.Username, callbackChannel);
                        if (userAdded) {
                            Console.WriteLine($"Usuario {messageData.Username} agregado al Lobby.");
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
                            Console.WriteLine($"Error debido enviado a: {ex.Message}");
                        }
                    }
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        private void ServiceNotifyPlayersInLobby(string lobbyCode) {
            try {
                if (lobbiesDictionary.ContainsKey(lobbyCode)) {
                    var lobby = lobbiesDictionary[lobbyCode];

                    List<PlayerData> playerList = new List<PlayerData>();

                    foreach (var player in lobby.Keys) {

                        if (player.Contains("Guest")) {

                            playerList.Add(new PlayerData {
                                Username = player,
                                Level = 0,
                                ImageId = 0,
                            });

                        } else {

                            int idUser = UsersDAO.GetIdUserByUsername(player);
                            int profileLevel = ProfileDAO.GetProfileLevelByIdUser(idUser);
                            int profileImageId = ProfileDAO.GetImageIdByIdUser(idUser);

                            playerList.Add(new PlayerData {
                                Username = player,
                                Level = profileLevel,
                                ImageId = profileImageId,
                            });
                        }
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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        //
        public int ServiceCountPlayersInLobby(string lobbyCode) {
            try {
                int countUsers = 0;
                if (lobbiesDictionary.ContainsKey(lobbyCode)) {
                    ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];
                    foreach (var usersInLobby in lobby) {
                        countUsers++;
                    }
                }

                return countUsers;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public static void StartMatch(string lobbyCode) {
            try {
                if (lobbiesDictionary.TryGetValue(lobbyCode, out var lobby)) {
                    Task.Run(() => {
                        foreach (var playerCallback in lobby.Values) {
                            try {
                                playerCallback.ServiceNotifyMatchStart();
                            } catch (Exception ex) {
                                Console.WriteLine($"Error notificando al jugador: {ex.Message}");
                            }
                        }
                    });
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public bool ServiceStartLobbyMatch(string lobbyCode, string username) {
            try {
                if (lobbiesDictionary.ContainsKey(lobbyCode)) {
                    if (lobbyOwnersDictionary.TryGetValue(lobbyCode, out var owner) && owner == username) {
                        StartMatch(lobbyCode);
                        DateTime startTime = DateTime.Now;
                        MatchDAO.CreateMatch(startTime);
                        gameStartDates.Add(lobbyCode, startTime);
                        return true;
                    } else {
                        Console.WriteLine($"Usuario {username} intentó iniciar la partida, pero no es el propietario del lobby {lobbyCode}.");
                        return false;
                    }
                }
                return false;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }


    }

    public partial class ServiceImplementation : IProfilesManager {

        public ProfileData ServiceLoadProfileData(string username) {
            try {
                int idUser = UsersDAO.GetIdUserByUsername(username);

                var profileData = new ProfileData() {
                    ProfileLevel = ProfileDAO.GetProfileLevelByIdUser(idUser),
                    MatchesWon = ProfileDAO.GetMatchesWonByIdUser(idUser),
                    ImageId = ProfileDAO.GetImageIdByIdUser(idUser),
                };

                return profileData;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public void ServiceIncrementMatchesWonByUserName(string username) {
            try {
                ProfileDAO.IncrementMatchesWon(username);
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public int ServiceGetWonMatchesByUsername(string username) {
            try {
                int matchesWon = ProfileDAO.GetWonMatchesByUsername(username);

                return matchesWon;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;

            }
        }


        public bool ServiceChangeProfileImage(string username, int imageId) {
            try { 
                int idUser = UsersDAO.GetIdUserByUsername(username);
                int result = ProfileDAO.ChangeProfileImageByIdUser(idUser, imageId);

                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                return false;
            }
        }

        public ProfileData ServiceGetProfileByUserId(string userId) {
            try {
                return ServiceGetProfileByUserId(userId);
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

    }

    public partial class ServiceImplementation : IFriendsManager {

        public bool ServiceAcceptFriendRequest(string username1, string username2) {
            try {

                int idSender = UsersDAO.GetIdUserByUsername(username1);
                int idReceiver = UsersDAO.GetIdUserByUsername(username2);

                int result = FriendsDAO.AcceptFriendRequest(idSender, idReceiver);

                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceSendFriendRequest(string username1, string username2) {
            try {

                int idSender = UsersDAO.GetIdUserByUsername(username1);
                int idReceiver = UsersDAO.GetIdUserByUsername(username2);

                int result = FriendsDAO.AddFriend(idSender, idReceiver);
                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public List<PlayerData> ServiceGetFriends(string username) {
            try {

                int idRequest = UsersDAO.GetIdUserByUsername(username);

                List<int> listIdFriends = FriendsDAO.GetFriends(idRequest);
                List<PlayerData> friendsData = new List<PlayerData>();


                foreach (int idFriend in listIdFriends) {
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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public List<PlayerData> ServiceGetBlockedUsers(string username) {
            try {
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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceRemoveFriend(string username1, string username2) {
            try {

                int idSender = UsersDAO.GetIdUserByUsername(username1);
                int idReceiver = UsersDAO.GetIdUserByUsername(username2);

                int result = FriendsDAO.DeleteFriend(idSender, idReceiver);
                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceIsPendingFriendRequest(string username1, string username2) {
            try {

                int idSender = UsersDAO.GetIdUserByUsername(username1);
                int idReceiver = UsersDAO.GetIdUserByUsername(username2);

                bool result = FriendsDAO.IsFriendRequestPending(idSender, idReceiver);
                return result;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public List<PlayerData> ServiceGetPendingFriendRequest(string username) {
            try {
                int idReceiver = UsersDAO.GetIdUserByUsername(username);

                List<int> listIdFriends = FriendsDAO.GetPendingFriendRequests(idReceiver);
                List<PlayerData> friendsData = new List<PlayerData>();

                foreach (int idPendingFriend in listIdFriends) {
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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                List<PlayerData> nonPlayerData = new List<PlayerData>();
                throw;
            }
        }

        public bool ServiceIsUserBlocked(string usernameBlocker, string usernameBlocked) {
            try {

                int idBlocker = UsersDAO.GetIdUserByUsername(usernameBlocker);
                int idBlocked = UsersDAO.GetIdUserByUsername(usernameBlocked);

                bool result = BlockedDAO.IsUserBlocked(idBlocker, idBlocked);
                return result;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceRemoveBlock(string usernameBlocked, string usernameBlocker) {
            try {

                int idBlocker = UsersDAO.GetIdUserByUsername(usernameBlocker);
                int idBlocked = UsersDAO.GetIdUserByUsername(usernameBlocked);

                int result = BlockedDAO.DeleteBlock(idBlocked, idBlocker);
                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceBlockUser(string usernameBlocked, string usernameBlocker) {
            try {

                int idBlocker = UsersDAO.GetIdUserByUsername(usernameBlocker);
                int idBlocked = UsersDAO.GetIdUserByUsername(usernameBlocked);
                FriendsDAO.DeleteFriend(idBlocker, idBlocked);
                FriendsDAO.DeleteFriend(idBlocked, idBlocker);

                int result = BlockedDAO.BlockUser(idBlocked, idBlocker);
                return result == 1;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }


    }

    public partial class ServiceImplementation : IMatchManager {
        private static Dictionary<string, List<string>> playersInGame = new Dictionary<string, List<string>>();
        private static Dictionary<string, string> currentTurnByGame = new Dictionary<string, string>();
        private static Dictionary<string, bool> turnTransitionState = new Dictionary<string, bool>();
        private static Dictionary<string, string> currentTurnsByGame = new Dictionary<string, string>();
        private static Dictionary<string, DateTime> gameTimers = new Dictionary<string, DateTime>();
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, IMatchServiceCallback>> gameConnectionsDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, IMatchServiceCallback>>();
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, int>> pointsPerPlayer = new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>();
        private static Dictionary<string, DateTime> gameStartDates = new Dictionary<string, DateTime>();

        public bool ServiceConnectToGame(string username, string lobbyCode) {
            try {
                var callback = OperationContext.Current.GetCallbackChannel<IMatchServiceCallback>();
                bool connected = false;
                if (!gameConnectionsDictionary.ContainsKey(lobbyCode)) {
                    gameConnectionsDictionary[lobbyCode] = new ConcurrentDictionary<string, IMatchServiceCallback>();
                }
                gameConnectionsDictionary[lobbyCode][username] = callback;
                connected = true;
                return connected;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }
        public void ServiceInitializeGameTurns(string lobbyCode) {
            try {
                var callbackChannel = OperationContext.Current.GetCallbackChannel<IMatchServiceCallback>();
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];
                ConcurrentDictionary<string, IMatchServiceCallback> connections = new ConcurrentDictionary<string, IMatchServiceCallback>();
                ConcurrentDictionary<string, int> playerPoints = new ConcurrentDictionary<string, int>();

                List<string> usersInLobby = new List<string>();
                if (lobbiesDictionary.ContainsKey(lobbyCode)) {


                    Console.WriteLine($"Usuarios actuales en el lobby {lobbyCode}:");
                    foreach (var users in lobby) {
                        try {
                            Console.WriteLine($"Usuario: {users.Key}");
                            usersInLobby.Add(users.Key);

                        } catch (Exception ex) {
                            Console.WriteLine($"Error debido enviado a: {ex.Message}");
                        }
                    }
                }
                usersInLobby = usersInLobby.OrderBy(_ => Guid.NewGuid()).ToList();
                playersInGame[lobbyCode] = usersInLobby;

                foreach (var users in usersInLobby) {
                    playerPoints.TryAdd(users, 0);
                }

                pointsPerPlayer[lobbyCode] = playerPoints;

                var firstPlayer = playersInGame[lobbyCode].First();
                currentTurnByGame[lobbyCode] = firstPlayer;
                turnTransitionState[lobbyCode] = false;
                ServiceNotifyClientOfTurn(lobbyCode, firstPlayer);
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public void ServiceNotifyEndTurn(string gameCode, string currentGamertag) {
            try {
                if (!turnTransitionState.ContainsKey(gameCode) || turnTransitionState[gameCode]) return;
                if (playersInGame.TryGetValue(gameCode, out var players)) {
                    int currentIndex = players.IndexOf(currentGamertag);

                    int nextIndex = (currentIndex + 1) % players.Count;
                    var nextGametag = players[nextIndex];

                    currentTurnByGame[gameCode] = nextGametag;

                    ServiceNotifyClientOfTurn(gameCode, nextGametag);

                    turnTransitionState[gameCode] = true;

                    ServiceResetTurnTransitionState(gameCode);
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public string ServiceGetCurrentTurn(string gameCode) {
            try {
                return currentTurnByGame.ContainsKey(gameCode) ? currentTurnByGame[gameCode] : null;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        private void ServiceNotifyClientOfTurn(string gameCode, string nextGametag) {
            try {
                ConcurrentDictionary<string, IMatchServiceCallback> connections = gameConnectionsDictionary[gameCode];
                ConcurrentDictionary<string, int> playerPoints = pointsPerPlayer[gameCode];

                List<string> disconnectedClients = new List<string>();
                bool hasDisconnected = false;

                if (gameConnectionsDictionary.ContainsKey(gameCode)) {
                    foreach (var connection in connections) {
                        try {
                            connection.Value.ServiceUpdateCurrentTurn(nextGametag, playerPoints);
                            connection.Value.ServiceSyncTimer();
                        } catch (CommunicationObjectAbortedException) {
                            Console.WriteLine($"Cliente desconectado: {connection.Key}");
                            disconnectedClients.Add(connection.Key);
                            hasDisconnected = true;
                        } catch (Exception ex) {
                            Console.WriteLine($"Error al notificar al cliente {connection.Key}: {ex.Message}");
                            disconnectedClients.Add(connection.Key);
                            hasDisconnected = true;
                        }
                    }

                    foreach (var client in disconnectedClients) {
                        connections.TryRemove(client, out _);
                    }

                    if (hasDisconnected) {
                        Console.WriteLine("Desconexión detectada. Notificando a todos los clientes para volver a la ventana Start.");
                        foreach (var connection in connections.Values) {
                            try {
                                connection.ServiceNotifyReturnToStart();
                            } catch (Exception ex) {
                                Console.WriteLine($"Error al notificar retorno a Start: {ex.Message}");
                            }
                        }
                        CleanUpGameResources(gameCode);
                    } else {
                        ServiceResetTurnTransitionState(gameCode);
                    }
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        private void ServiceResetTurnTransitionState(string gameCode) {
            try {
                if (turnTransitionState.ContainsKey(gameCode)) {
                    turnTransitionState[gameCode] = false;
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public MatchData ServiceCreateMatch(DateTime startTime) {
            try {
                int idMatch = MatchDAO.CreateMatch(startTime);

                return new MatchData {
                    IdMatch = idMatch.ToString(),
                    StartTime = startTime,
                    EndTime = null,
                    IdWinner = null
                };
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public MatchData ServiceGetMatchById(string matchId) {
            try {
                if (!int.TryParse(matchId, out int id))
                    throw new ArgumentException("El ID de la partida debe ser un número.");

                var match = MatchDAO.GetMatchById(id);

                if (match == null)
                    return null;

                return new MatchData {
                    IdMatch = match.idMatch.ToString(),
                    StartTime = match.startTime,
                    EndTime = match.endTime,
                    IdWinner = match.idWinner?.ToString()
                };
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceUpdateMatch(string matchId, string idWinner, DateTime? endTime) {
            try {
                if (!int.TryParse(matchId, out int idMatch))
                    throw new ArgumentException("El ID de la partida debe ser un número.");

                int? idWinnerParsed = null;
                if (!string.IsNullOrEmpty(idWinner)) {
                    if (!int.TryParse(idWinner, out int id))
                        throw new ArgumentException("El ID del ganador debe ser un número.");
                    idWinnerParsed = id;
                }

                int result = MatchDAO.UpdateMatch(idMatch, idWinnerParsed, endTime);

                return result > 0;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public List<MatchData> ServiceGetRecentMatches(int topN) {
            try {
                var matches = MatchDAO.GetRecentMatches(topN);

                var matchDataList = new List<MatchData>();

                if(matches != null) {
                    foreach (var match in matches) {
                        matchDataList.Add(new MatchData {
                            IdMatch = match.idMatch.ToString(),
                            StartTime = match.startTime,
                            EndTime = match.endTime,
                            IdWinner = match.idWinner?.ToString()
                        });
                    }

                    return matchDataList;
                } else { 
                    return matchDataList; 
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public void ServiceCreateDeck(string lobbyCode) {
            try {
                List<CardData> deck = new List<CardData>();
                AddCardsToDeck(deck, 1, 12);
                AddCardsToDeck(deck, 2, 12);
                AddCardsToDeck(deck, 3, 10);
                AddCardsToDeck(deck, 4, 10);
                AddCardsToDeck(deck, 5, 8);
                AddCardsToDeck(deck, 6, 8);
                AddCardsToDeck(deck, 7, 6);
                AddCardsToDeck(deck, 8, 6);
                AddCardsToDeck(deck, 9, 4);

                List<CardData> deckShuffled = deck.OrderBy(newDeck => Guid.NewGuid()).ToList();
                Stack<CardData> deckStacked = new Stack<CardData>(deckShuffled);

                ConcurrentDictionary<string, IMatchServiceCallback> connections = gameConnectionsDictionary[lobbyCode];
                if (gameConnectionsDictionary.ContainsKey(lobbyCode)) {
                    foreach (var connection in connections.Values) {
                        connection.ServiceReceiveDeck(deckStacked);
                    }
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public void AddCardsToDeck(List<CardData> cardsList, int idCard, int cardQuantity) {
            try {

                CardData cardData = ServiceGetCardById(idCard);
                for (int i = 0; i < cardQuantity; i++) {
                    cardsList.Add(cardData);
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public void ServiceNotifyDrawCard(string lobbyCode) {
            try {
                ConcurrentDictionary<string, IMatchServiceCallback> connections = gameConnectionsDictionary[lobbyCode];

                if (gameConnectionsDictionary.ContainsKey(lobbyCode)) {
                    foreach (var connection in connections.Values) {
                        connection.ServiceRemoveCardFromDeck();
                    }
                    ServiceResetTurnTransitionState(lobbyCode);

                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public void ServiceUpdatePointsFromPlayer(string lobbyCode, string username, int points) {
            try {
                pointsPerPlayer[lobbyCode][username] = points;
                Console.WriteLine($"Puntos del jugador{username} son: {points}");
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }

        }

        public void ServiceEndGame(string lobbyCode) {
            try {
                string winnerUsername = CheckWinner(lobbyCode);
                ConcurrentDictionary<string, IMatchServiceCallback> connections = gameConnectionsDictionary[lobbyCode];
                int idUser = 0;
                if (winnerUsername.Contains("Guest")) {
                    idUser = 0;
                } else {
                    idUser = UsersDAO.GetIdUserByUsername(winnerUsername);
                    try {
                        ProfileDAO.IncrementMatchesWon(winnerUsername);
                    } catch (Exception ex) {
                        Console.WriteLine($"Error al incrementar las partidas ganadas para el usuario {idUser}: {ex.Message}");
                    }
                }
                DateTime startTime = gameStartDates[lobbyCode];
                var match = MatchDAO.GetMatchByStartTime(startTime);
                DateTime endTime = DateTime.Now;
                MatchDAO.UpdateMatch(match, idUser, endTime);
                Task.Run(() => {
                    if (gameConnectionsDictionary.ContainsKey(lobbyCode)) {
                        foreach (var connection in connections.Values) {
                            connection.ServiceNotifyEndGame(winnerUsername);
                        }
                    }
                });

                CleanUpGameResources(lobbyCode);
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        public string CheckWinner(string lobbyCode) {
            try {
                var maxPair = pointsPerPlayer[lobbyCode].Aggregate((l, r) => l.Value > r.Value ? l : r);
                return maxPair.Key;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public void ServiceAttackPlayers(string lobbyCode, string usernameAttacker, int attackPoints) {
            try {
                var playerPoints = pointsPerPlayer[lobbyCode];
                foreach (var player in playerPoints) {
                    if (player.Key != usernameAttacker) {
                        playerPoints[player.Key] = player.Value - attackPoints;
                    }
                }
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }

        private void CleanUpGameResources(string lobbyCode) {
            try {
                gameConnectionsDictionary.TryRemove(lobbyCode, out _);

                playersInGame.Remove(lobbyCode);

                pointsPerPlayer.TryRemove(lobbyCode, out _);

                currentTurnByGame.Remove(lobbyCode);
                turnTransitionState.Remove(lobbyCode);

                gameTimers.Remove(lobbyCode);
                gameStartDates.Remove(lobbyCode);

                Console.WriteLine($"Recursos limpiados para la partida: {lobbyCode}");
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
            }
        }


    }

    public partial class ServiceImplementation : ICardsManager {

        public List<CardData> ServiceGetAllCards() {
            try {
                var cards = CardsDAO.GetAllCards();

                var cardDataList = new List<CardData>();
                foreach (var card in cards) {
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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public CardData ServiceGetCardById(int id) {
            try {
                var card = CardsDAO.GetCardById(id);

                if (card != null) {
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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceAddCard(CardData cardData) {
            try {

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
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }

        public bool ServiceDeleteCard(int id) {
            try {
                int result = CardsDAO.DeleteCard(id);
                return result > 0;
            } catch (Exception ex) {
                ServiceExceptionHandler.HandleServiceException(ex);
                throw;
            }
        }
    }

}