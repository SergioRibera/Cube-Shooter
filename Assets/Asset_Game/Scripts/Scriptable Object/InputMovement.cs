using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Move Inputs")]
public class InputMovement : ScriptableObject {

    public bool Idle;
    public bool IsMoved { get; set; }
    public bool canMove = true;
    public bool Shoot;
    public bool Reload;

    Animator anim;
    Transform player;
    Transform canonToShoot;
    Rigidbody player_Rb;
    CameraPlayer camara;
    PlayerController player_Control;

    Vector3 pos;


    public void StartedConfig(Animator _anim, Transform _player, Transform _canon)
    {
        anim = _anim;
        player = _player;
        camara = player.parent.GetComponentInChildren<CameraPlayer>();
        player_Rb = player.GetComponent<Rigidbody>();
        player_Control = _player.GetComponent<PlayerController>();
        canonToShoot = _canon;
    }
    void Animating(float h, float v)
    {
        Idle = v == 0 && h == 0;
        anim.SetBool("Walk", !Idle);
    }
    public void MovePlayer(float h, float v, float velocity, float turnVelocity)
    {
        pos.Set(h, 0f, v);
        pos = pos.normalized * velocity * Time.deltaTime;
        Quaternion newrotation = Quaternion.Slerp(player_Rb.rotation, Quaternion.LookRotation(pos), turnVelocity * Time.deltaTime);
        player_Rb.MovePosition(player.position + pos);
        player_Rb.MoveRotation(newrotation);
        Animating(h, v);
        camara.MoveCam();
    }
    public void DoValueBool(string name, int type = 0)
    {

        if (type == 0)
        {
            switch (name)
            {
                case "Shoot":
                    Shoot = true;
                    break;
            }
        }
        if (type == 1)
        {
            switch (name)
            {
                case "Shoot":
                    Shoot = false;
                    break;
            }
        }
    }
}