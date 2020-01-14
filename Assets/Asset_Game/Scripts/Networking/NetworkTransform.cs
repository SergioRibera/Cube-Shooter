using Multiplayer.Networking.Utility.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibraryPersonal;

namespace Multiplayer.Networking
{
    public class NetworkTransform : MonoBehaviour
    {
        [SerializeField]
        [GreyOut]
        Vector3 oldPosition;


        public NetworkIdentity networkIdentity;
        Player player = new Player();
        float stillCount = 0;
        void Start(){
            oldPosition = transform.position;
            enabled = networkIdentity.IsMe();
        }
        public void SetPlayer(Player p)
        {
            player = p;
        }
        void Update()
        {
            if (networkIdentity.IsMe())
            {
                if(oldPosition != transform.position)
                {
                    oldPosition = transform.position;
                    stillCount = 0;
                    sendData();
                }
                else
                {
                    stillCount += Time.deltaTime;

                    if(stillCount >= .01)
                    {
                        stillCount = 0;
                        sendData();
                    }
                }
            }
        }
        public void sendData()
        {
            //Update player Transform
            print("Update player position");

            player.position.x = transform.position.x.ToString();
            player.position.y = transform.position.y.ToString();
            player.position.z = transform.position.z.ToString();

            networkIdentity.GetSocket().Emit("updatePosition", new JSONObject(JsonUtility.ToJson(player)));
        }
    }}