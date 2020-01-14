using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InputMovement {

    public bool Idle;
    public bool IsMoved { get; set; }
    public bool canMove = true;
    public bool Shoot;
    public bool Reload;

    Animator anim;
    Transform player;
    Transform canonToShoot;
    Transform directionalCircle;
    Rigidbody player_Rb;
    CameraPlayer camara;
    PlayerController player_Control;

    Vector3 pos;


    public void StartedConfig(Animator _anim, Transform _player, Transform _canon, Transform _directionalCircle)
    {
        anim = _anim;
        player = _player;
        camara = player.parent.GetComponentInChildren<CameraPlayer>();
        player_Rb = player.GetComponent<Rigidbody>();
        player_Control = _player.GetComponent<PlayerController>();
        canonToShoot = _canon;
        directionalCircle = _directionalCircle;
    }
    void Animating(float h, float v)
    {
        Idle = v == 0 && h == 0;
        anim.SetBool("Walk", !Idle);
    }

    public void Life(int l)
    {
        if (l <= 0)
        {
            anim.SetTrigger("Death");
            player.GetComponent<CapsuleCollider>().enabled = false;
            player_Rb.useGravity = false;
        }
        else if(l > 0)
        {
            anim.SetTrigger("isRevive");
            player.GetComponent<CapsuleCollider>().enabled = true;
            player_Rb.useGravity = true;
        }
    }
    public void IsReviving(bool r)
    {
        anim.SetBool("Reviving", r);
    }

    public void MovePlayer(float h, float v, float velocity, float turnVelocity)
    {
        anim.SetBool("Reviving", false);
        pos.Set(h, 0f, v);
        pos = pos.normalized * velocity * Time.deltaTime;
        Quaternion newrotation = Quaternion.Slerp(player_Rb.rotation, Quaternion.LookRotation(pos), turnVelocity * Time.deltaTime);
        player_Rb.MovePosition(player.position + pos);
        player_Rb.MoveRotation(newrotation);
        Quaternion rotationCircle = Quaternion.Slerp(Quaternion.Euler(90, player_Rb.rotation.y, 0), Quaternion.LookRotation(pos), turnVelocity * Time.deltaTime);
        directionalCircle.SetPositionAndRotation(new Vector3(player_Rb.position.x, player_Rb.position.y + 0.02f, player_Rb.position.z), Quaternion.Euler(90, player_Rb.rotation.y, 0));
        Animating(h, v);
        camara.MoveCam();
    }
    public void DoValueBool(string name, int type = 0)
    {
        anim.SetBool("Reviving", false);
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