using Multiplayer.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LibraryPersonal;
using System;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform[] previewPlayers;
    [SerializeField] TMP_InputField inputIdLobby;
    [SerializeField] TMP_Dropdown mapLobby;
    [SerializeField] TMP_Dropdown gamemodeLobby;
    [SerializeField] TextMeshProUGUI textPlayButton;
    NetworkClient networkClient;
    internal int indexPlayers = 0;

    public static LobbyManager singletone;

    void Start()
    {
        networkClient = FindObjectOfType<NetworkClient>();
        networkClient.JoinLobby += NetworkClient_JoinLobby;
        if (singletone == null)
            singletone = this;
    }
    bool isLider;
    private void NetworkClient_JoinLobby(JSONObject data, Player p)
    {
        isLider = data["idPlayerMaster"].str == NetworkClient.ClientID;
        gamemodeLobby.interactable = (isLider);
        mapLobby.interactable = (isLider);
        textPlayButton.text = isLider ? "Comenzar Juego" : "Estoy Listo";
        foreach (var t in previewPlayers)
        {
            if (t.childCount > 1)
            {
                Destroy(t.GetChild(1).gameObject);
            }
        }
        inputIdLobby.text = data["idLobby"].f.ToString();
        GameObject g = Instantiate(prefab, previewPlayers[indexPlayers]);

        if (!networkClient.serverObjects.ContainsKey(p.id))
        {
            g.name = string.Format("Player ({0})", p.id);

            PlayerNetworkClases networkClases = new PlayerNetworkClases();
            networkClases.ni = g.GetComponent<NetworkIdentity>();

            networkClases.ni.SetControllerID(p.id, p.username);
            networkClases.ni.SetSocketReference(networkClient);
            networkClases.ni.SetPlayer(p);
            networkClient.serverObjects.Add(p.id, networkClases);
        }
        else
        {
            networkClient.serverObjects[p.id].ni.player = p;
        }
        g.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = p.username;
        g.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = p.playerColor.hexToColor();
        print("Player Connected");
        indexPlayers++;
    }

    public void ChangedSettingsLobbyVisual()
    {
        mapLobby.value = (int) networkClient.settingsLobby.gameMapa;
        gamemodeLobby.value = (int) networkClient.settingsLobby.gameMode;
    }
    public void UpdateReadys()
    {
        int i = 0;
        foreach (var p in networkClient.Players.Values)
        {
            previewPlayers[i].GetChild(0).GetComponentInChildren<Image>().color = p.ready ? Color.green : Color.cyan;
            i++;
        }
    }
    public void JoinInvitedLobby()
    {
        networkClient.JoinInvitedLobby(int.Parse(inputIdLobby.text));
        indexPlayers = 0;
    }

    public void MapChanged(int i)
    {
        networkClient.settingsLobby.gameMapa = (GameMapa) Enum.ToObject(typeof(GameMapa), i);
        networkClient.ChangeSettingsLobby();
    }
    public void ModeGameChanged(int i)
    {
        networkClient.settingsLobby.gameMode = (GameMode) Enum.ToObject(typeof(GameMode), i);
        networkClient.ChangeSettingsLobby();
    }

    bool ready = false;

    public void ReadyOrPlay()
    {
        if (!isLider)
        {
            ready = !ready;
            networkClient.serverObjects[NetworkClient.ClientID].ni.player.ready = ready;
            networkClient.Ready(NetworkClient.ClientID, ready);
        }
        else
        {
            networkClient.PlayGame(((int) networkClient.settingsLobby.gameMapa) + 1);
        }
    }
}