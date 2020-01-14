using Multiplayer.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    [SerializeField]
    Transform player;
    [SerializeField]
    float smooth = 5f;

    [SerializeField] Vector3 offset = new Vector3(0, 6, -5);

    void Start()
    {
        if (transform.GetComponentInParent<NetworkIdentity>())
            gameObject.SetActive(transform.GetComponentInParent<NetworkIdentity>().IsMe());
    }

    internal void MoveCam()
    {
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smooth * Time.deltaTime);
    }
}
