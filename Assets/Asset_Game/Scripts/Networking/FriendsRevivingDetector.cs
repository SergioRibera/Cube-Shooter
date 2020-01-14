using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsRevivingDetector : MonoBehaviour
{
    public PlayerControllerNetwork myPlayer;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (myPlayer.Died)
                other.gameObject.GetComponent<PlayerControllerNetwork>().SetReviving(true, myPlayer);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (myPlayer.Died)
                other.gameObject.GetComponent<PlayerControllerNetwork>().SetReviving(false, myPlayer);
        }
    }
}