using Multiplayer.Networking.Utility.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibraryPersonal;

namespace Multiplayer.Networking
{
    public class NetworkRotation : MonoBehaviour
    {
        [Header("Reference Values")]
        [SerializeField]
        [GreyOut]
        Vector3 oldRotation;

        [Header("Class References")]
        NetworkIdentity networkIdentity;
        Player player = new Player();
        float stillCount = 0;

        public void SetPlayer(Player p)
        {
            player = new Player();
            player = p;
            networkIdentity = GetComponentInParent<NetworkIdentity>();

            enabled = networkIdentity.IsMe();
        }
        void Update()
        {
            if (networkIdentity.IsMe())
            {
                if (oldRotation != transform.localEulerAngles)
                {
                    oldRotation = transform.localEulerAngles;
                    stillCount = 0;
                    sendData();
                }
                else
                {
                    stillCount += Time.deltaTime;

                    if (stillCount >= .01)
                    {
                        stillCount = 0;
                        sendData();
                    }
                }
            }
        }

        void sendData()
        {
            print("Update player rotation");

            networkIdentity.GetSocket().Emit("updateRotation", new JSONObject(JsonUtility.ToJson(player)));
        }
    }
}