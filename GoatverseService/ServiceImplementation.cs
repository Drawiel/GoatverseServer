using BCrypt.Net;
using DataAccess;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GoatverseService
{
    public partial class ServiceImplementation : IUsersManager {

        public bool tryLogin(string username, string passwordAttempt) {

            using (var database = new GoatverseEntities()) {
                
                var user = database.Users.SingleOrDefault(u => u.username == username);
                if (user != null || BCrypt.Net.BCrypt.Verify(passwordAttempt, user.password)) {
                    Console.WriteLine("User: " + username + " has loged in");
                    return true;
                } else {
                    Console.WriteLine("Attemp to log in with username: " + username);
                    return false;
                }
            }
            
        }

        public bool trySignIn(string newUsername, string newPassword, string newEmail) {

            using (var database = new GoatverseEntities()) {

                if(database.Users.Any(u => u.username == newUsername)) {

                }
                
                var newSignIn = new Users {
                    username = newUsername,
                    password = BCrypt.Net.BCrypt.HashPassword(newPassword),
                    email = newEmail
                };


                database.Users.Add(newSignIn);
                int result = database.SaveChanges();

                if (result == 1) {
                    Console.WriteLine("New User added");
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

        public bool connectToLobby(string username, string lobbyCode) {

            if (lobbiesDictionary.ContainsKey(lobbyCode)){
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];

                if (lobby.ContainsKey(username)) {

                    return false;
                } else {

                    var callbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();
                    lobby.TryAdd(username, callbackChannel);
                    return true;
                }
            } else {
                var callback = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();
                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = new ConcurrentDictionary<string, ILobbyServiceCallback>();
                lobby.TryAdd(username, callback);
                lobbiesDictionary.TryAdd(lobbyCode, lobby);
                return true;
            }
        }

        public bool disconnectFromLobby(string username, string lobbyCode) {
            if (lobbiesDictionary.ContainsKey(lobbyCode)) {

                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[lobbyCode];
                if (lobby.ContainsKey(username)) {

                    ILobbyServiceCallback callback;
                    lobby.TryRemove(username, out callback);

                    if (lobby.IsEmpty) {

                        ConcurrentDictionary<string, ILobbyServiceCallback> removedUser;
                        lobbiesDictionary.TryRemove(lobbyCode, out removedUser);
                    }

                    return true;
                }
            }

            return false;
        }

        public void sendMessageToLobby(User user) {
            var callbackChannel = OperationContext.Current.GetCallbackChannel<ILobbyServiceCallback>();

            if (lobbiesDictionary.ContainsKey(user.LobbyCode)) {

                ConcurrentDictionary<string, ILobbyServiceCallback> lobby = lobbiesDictionary[user.LobbyCode];

                if (!lobby.ContainsKey(user.Username)) {

                    bool userAdded = lobby.TryAdd(user.Username, callbackChannel);
                    if (userAdded) {

                    } else {

                    }
                } else {

                    lobby[user.Username] = callbackChannel;

                }

                foreach(var usersInLobby in lobby) {
                    try {
                        usersInLobby.Value.GetMessage(user);
                    } catch (Exception ex) { 

                    }
                }
            } else { 
            }
        }
    }
}
