using Multiplayer.Networking;
using System;
using UnityEngine;
[System.Serializable]
public class GeneralInputs{

    public InputsPc InputsPc = InputsPc.Default;

    bool isMine = false;

    public void SetMine(NetworkIdentity n)
    {
        isMine = n.IsMe();
    }
    public GeneralInputs()
    {
        isMine = false;
        InputsPc = InputsPc.Default;
    }

    public static bool isMovil;
    public static void CheckPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
                isMovil = false;
                break;
            case RuntimePlatform.WindowsEditor:
                isMovil = false;
                break;
            case RuntimePlatform.IPhonePlayer:
                isMovil = true;
                break;
            case RuntimePlatform.Android:
                isMovil = true;
                break;
        }
    }

    public float HorizontalAxis
    {
        get
        {
            return !isMovil ? Input.GetKey(InputsPc.izquierda) ? -1.0f : Input.GetKey(InputsPc.derecha) ? 1.0f : 0.0f : 0.0f;
        }
    }
    public float VerticalAxis
    {
        get
        {
            return !isMovil ? Input.GetKey(InputsPc.retrocede) ? -1.0f : Input.GetKey(InputsPc.avanza) ? 1.0f : 0.0f : 0.0f;
        }
    }

    public bool Fire1(bool r)
    {
        return !isMovil && isMine && !r ? Input.GetKey(InputsPc.simpleAttack) : false;
    }
}

[Serializable]
public class InputsPc
{
    public KeyCode avanza = KeyCode.W;
    public KeyCode retrocede = KeyCode.S;
    public KeyCode derecha = KeyCode.D;
    public KeyCode izquierda = KeyCode.A;
    public KeyCode simpleAttack = KeyCode.Mouse0;
    public KeyCode openBag = KeyCode.E;
    public KeyCode closeBag = KeyCode.E;

    public static InputsPc Default
    {
        get
        {
            InputsPc i = new InputsPc();
            i.avanza = KeyCode.W;
            i.retrocede = KeyCode.S;
            i.derecha = KeyCode.D;
            i.izquierda = KeyCode.A;
            i.simpleAttack = KeyCode.Mouse0;
            i.openBag = KeyCode.E;
            i.closeBag = KeyCode.E;
            return i;
        }
    }
}