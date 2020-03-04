using Crest;
using Multiplayer.Networking.Utility.Attributes;
using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer.Networking
{
    public class NetworkIdentity : MonoBehaviour
    {
        [Header("Helpful Values")]
        [SerializeField]
        [GreyOut]
        string id;
        [SerializeField]
        [GreyOut]
        bool isMe;
        [SerializeField]
        [GreyOut]
        string namePlayer;

        public SocketIOComponent socket;

        void Awake()
        {
            isMe = false;
        }

        public void ChangeLevel(int i) => SceneManager.LoadScene(i);

        public void SetControllerID(string ID, string n)
        {
            id = ID;
            isMe = (NetworkClient.ClientID == ID) ? true : false;
            namePlayer = n;
            if (FindObjectOfType<OceanRenderer>() != null)
                if (IsMe())
                {
                    OceanRenderer.Instance.Awake();
                }
        }
        Material myMaterial = null;
        public void ChangeColor(Color c)
        {
            foreach (var m in GetComponentsInChildren<MeshRenderer>())
            {
                if (m.gameObject.name == "Body")
                {
                    myMaterial = new Material(m.materials[0]);
                }
            }
            if (myMaterial != null) myMaterial.color = c;
        }
        public Player player;
        public void SetPlayer(Player p)
        {
            player = p;
        }
        public void SetSocketReference(SocketIOComponent Socket) => socket = Socket;

        public string GetID() { 
            return id; 
        }
        public bool IsMe()
        {
            return isMe;
        }
        public SocketIOComponent GetSocket()
        {
            return socket;
        }
    }
}