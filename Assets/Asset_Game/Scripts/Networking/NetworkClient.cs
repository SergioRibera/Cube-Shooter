using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using LibraryPersonal;
using System.Globalization;
using Multiplayer.Networking.Utility.Attributes;
using UnityEngine.SceneManagement;

namespace Multiplayer.Networking
{
    public delegate void EventNetwork(JSONObject data, Player p);
    public class NetworkClient : SocketIOComponent {

        public const float SERVER_UPDATE_TIME = 10;

        [Header("NetwokClient")]
        [SerializeField]
        Transform networkContainer;
        [SerializeField]
        GameObject[] playerPrefab;

        public event EventNetwork JoinLobby;
        public event EventNetwork LeftLobby;

        public static string ClientID { get; private set; }
        public static string LobbyID { get; private set; }

        [SerializeField]
        [GreyOut]
        public Dictionary<string, PlayerNetworkClases> serverObjects;

        internal SettingsLobby settingsLobby;
        public Dictionary<string, Player> Players;

        public override void Start()
        {
            base.Start();
            Initialize();
            SetupEvents();
            SetupLobbyEvents();
            Application.quitting += () => {
                Emit("disconnect");
            };
        }
        public override void Update()
        {
            base.Update();
        }
        void Initialize()
        {
            serverObjects = new Dictionary<string, PlayerNetworkClases>();
            Players = new Dictionary<string, Player>();
        }
        private void SetupEvents()
        {
            On("open", (E) => {
                print("Conection made to server");
            });
            On("register", (E) => {
                ClientID = E.data["id"].ToString().RemoveQuotes();
                LobbyID = E.data["lobbyId"].ToString().RemoveQuotes();

                ManagerDataPlayer.Init();
                Player p = new Player();
                p.id = ClientID;
                p.username = ManagerDataPlayer.DataGame.personajeDatos.Nombre;
                p.playerColor = ExtraToolColor.GetRandomColor();
                p.indexPlayerMesh = ManagerDataPlayer.DataGame.personajeDatos.indexPlayerMesh;
                p.position = Position.Zero;
                p.h = 0f;
                p.v = 0f;

                Emit("JoinGame", new JSONObject(JsonUtility.ToJson(new JoinLobbyData(p, false, 0, GameMode.Cooperativo, GameMapa.Playa) )));
                print("Player Registered");
            });
            On("spawn", (E) => {
                foreach (var player in E.data["players"].list)
                {
                    string id = player["id"].ToString().RemoveQuotes();
                    string idL = player["lobby"].ToString().RemoveQuotes();

                    if (idL == LobbyID)
                    {

                        float x = float.Parse(player["position"]["x"].str, CultureInfo.InvariantCulture);
                        float y = float.Parse(player["position"]["y"].str, CultureInfo.InvariantCulture);
                        float z = float.Parse(player["position"]["z"].str, CultureInfo.InvariantCulture);

                        Player p = new Player();
                        p.username = player["username"].str;
                        p.Life = 100;
                        p.indexPlayerMesh = (int) player["indexPlayerMesh"].f;
                        p.playerColor = player["playerColor"].str;
                        p.id = id;

                        GameObject go = Instantiate(playerPrefab[0], networkContainer);
                        go.name = string.Format("Player ({0})", id);
                        go.transform.position = new Vector3(x, y, z);

                        PlayerNetworkClases networkClases = new PlayerNetworkClases();
                        networkClases.ni = go.GetComponent<NetworkIdentity>();
                        networkClases.pn = go.transform.Find("VisiblePeronaje").GetComponent<PlayerControllerNetwork>();

                        networkClases.pn.SetPlayer(p);
                        networkClases.ni.SetControllerID(id, p.username);
                        networkClases.ni.SetSocketReference(this);
                        networkClases.ni.SetPlayer(p);
                        serverObjects.Add(id, networkClases);
                        print("Player  " + player["username"].str + "  spawned");
                    }
                }
            });

            On("updatePosition", (E) => {
                print("Player  " + E.data["username"].str + "  update position");
                string id = E.data["id"].ToString().RemoveQuotes();
                float h = E.data["h"].f;
                float v = E.data["v"].f;

                float x = float.Parse(E.data["position"]["x"].str, CultureInfo.InvariantCulture);
                float y = float.Parse(E.data["position"]["y"].str, CultureInfo.InvariantCulture);
                float z = float.Parse(E.data["position"]["z"].str, CultureInfo.InvariantCulture);

                PlayerControllerNetwork pn = serverObjects[id].pn;
                pn.SetNewValueMovement(new Vector3(x, y, z), h, v);
                print("Player  " + E.data["username"].str + "  update position");
            });
            On("updateLife", (E) => {
                string id = E.data["id"].ToString().RemoveQuotes();
                int life = (int) E.data["Life"].f;

                PlayerControllerNetwork pn = serverObjects[id].pn;
                print("player:" + id + " Life: " + life);
                pn.SetLife(life);
                print("Player  " + E.data["username"].str + "  update Life");
            });
            On("updateReviving", (E) => {
                string id = E.data["id"].ToString().RemoveQuotes();
                bool isReviving = E.data["isreviving"].b;

                PlayerControllerNetwork pn = serverObjects[id].pn;
                pn.SetReviving(isReviving);
                print("Player  " + E.data["username"].str + "  update Life");
            });

            On("Shoot", (E) => {
                string id = E.data["id"].ToString().RemoveQuotes();
                bool s = E.data["shoot"].b;
                PlayerControllerNetwork pn = serverObjects[id].pn;
                pn.SetNewValueShoot(s);

                print("Player  " + E.data["username"].str + "  Shoot");
            });
            On("disconnected", (E) => {
                string id = E.data["id"].ToString().RemoveQuotes();

                GameObject go = serverObjects[id].ni.gameObject;
                Destroy(go);
                serverObjects.Remove(id);
                print("Connection is Broken!!!");
            });
        }
        void SetupLobbyEvents()
        {
            On("JoinGameLobby", (E) => {
                LobbyManager.singletone.indexPlayers = 0;
                print(E.data["players"].list.Count);
                foreach (var player in E.data["players"].list)
                {
                    string id = player["id"].ToString().RemoveQuotes();
                    Player p = new Player();
                    p.username = player["username"].str;
                    p.Life = 100;
                    p.indexPlayerMesh = (int)player["indexPlayerMesh"].f;
                    p.playerColor = player["playerColor"].str;
                    p.id = id;

                    Players.Add(id, p);

                    LobbyID = E.data["idLobby"].str;
                    JoinLobby?.Invoke(E.data, p);
                    print("Player Connected");
                }
            });
            On("ChangeSettingsLobby", (E) => {
                int mapa = (int) E.data["gameMapa"].f;
                int mode = (int) E.data["gameMode"].f;
                settingsLobby.gameMapa = (GameMapa)Enum.ToObject(typeof(GameMapa), mapa);
                settingsLobby.gameMode = (GameMode)Enum.ToObject(typeof(GameMode), mode);
                LobbyManager.singletone.ChangedSettingsLobbyVisual();
            });
            On("PlayerReady", (E) => {
                string id = E.data["IdPlayer"].ToString().RemoveQuotes();
                bool ready = E.data["Ready"].b;

                serverObjects[id].ni.player.ready = ready;
                LobbyManager.singletone.UpdateReadys();
            });
            On("LeftGameLobby", (E) => {
                LobbyManager.singletone.indexPlayers = 0;
                string id = E.data["playerId"].ToString().RemoveQuotes();
                if (serverObjects.ContainsKey(id))
                {
                    GameObject go = serverObjects[id].ni.gameObject;
                    print("Connection is Broken!!!   2");
                    Destroy(go);
                    print("Connection is Broken!!!   3");
                    serverObjects.Remove(id);
                    Players.Remove(id);
                    print("Connection is Broken!!!");
                }
            });
        }
        public void Ready(string id, bool r)
        {
            Emit("PlayerReady", new JSONObject("{ id:" + id + ", ready:" + r + "}"));
        }
        public void PlayGame(int i)
        {
            bool allReady = false;
            foreach(var p in Players.Values)
            {
                if (!p.ready)
                    break;
                allReady = true;
            }
            if (allReady)
            {
                SceneManager.LoadScene(i);
                SceneManager.sceneLoaded += (s, m) => {
                    if (s.isLoaded)
                    {
                        Emit("PlayGame");
                    }
                };
            }
        }
        public void JoinInvitedLobby(int id)
        {
            Player p = serverObjects[ClientID].ni.player;
            Emit("JoinGame", new JSONObject(JsonUtility.ToJson(new JoinLobbyData(p, true, id, GameMode.Cooperativo, GameMapa.Playa))));
        }
        public void ChangeSettingsLobby()
        {
            Emit("LobbySettingsChanged", new JSONObject(JsonUtility.ToJson(settingsLobby)));
        }
    }

    [Serializable]
    public class JoinLobbyData
    {
        public Player player;
        public bool invited;
        public int idNewLobby;
        public GameMode gameMode;
        public GameMapa gameMapa;

        public JoinLobbyData(Player p, bool invited, int idNewLobby, GameMode gameMode, GameMapa gameMapa)
        {
            this.player = p;
            this.invited = invited;
            this.idNewLobby = idNewLobby;
            this.gameMode = gameMode;
            this.gameMapa = gameMapa;
        }
    }
    [Serializable]
    public class SettingsLobby
    {
        public GameMode gameMode;
        public GameMapa gameMapa;
        public string[] playersId;
        public bool playersWaiting;
    }
    public enum GameMode { Cooperativo = 0, FreeForAll = 1 }
    public enum GameMapa { Carretera = 0, Playa = 1 }

    [Serializable]
    public class PlayerNetworkClases
    {
        public NetworkIdentity ni;
        public PlayerControllerNetwork pn;
    }
    [Serializable]
    public class Player
    {
        public string username;
        public int Life;
        public int lobby = 0;
        public bool ready = false;
        public string playerColor;
        public string id;
        public int indexPlayerMesh = 0;
        public Position position = Position.Zero;
        public float h;
        public float v;
        public bool shoot;
        public bool isReviving;
    }
    [Serializable]
    public class Position
    {
        public string x;
        public string y;
        public string z;

        public Position()
        {
            this.x = "0.0";
            this.y = "0.0";
            this.z = "0.0";
        }
        public Position(float x, float y, float z)
        {
            this.x = x.ToString();
            this.y = y.ToString();
            this.z = z.ToString();
        }

        public static Position Zero { get { return new Position(0.0f, 0.0f, 0.0f); } }
    }
}