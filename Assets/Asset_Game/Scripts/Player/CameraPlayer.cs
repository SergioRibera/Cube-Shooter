using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayer : MonoBehaviour
{
    [SerializeField]
    Transform player;
    [SerializeField]
    float smooth = 5f;

    Vector3 offset;

    void Start()
    {
        offset = transform.position - player.position;
    }

    internal void MoveCam()
    {
        Vector3 targetPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smooth * Time.deltaTime);
    }
}
