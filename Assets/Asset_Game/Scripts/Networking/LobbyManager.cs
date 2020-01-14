using Multiplayer.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform[] previewPlayers;

    NetworkClient networkClient;
    int indexPlayers = 0;
    void Start()
    {
        networkClient = FindObjectOfType<NetworkClient>();
        networkClient.JoinLobby += NetworkClient_JoinLobby;   
    }

    private void NetworkClient_JoinLobby(JSONObject data)
    {
        GameObject g = Instantiate(prefab, previewPlayers[indexPlayers]);
        indexPlayers++;
        print("Player Connected");
    }
}