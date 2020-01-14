using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibraryPersonal;
using System;

[Serializable]
public class ManagerDataPlayer
{
    public static DataGame DataGame { get; private set; }
    public static bool isLocal;

    public static void Init()
    {
        GeneralInputs.CheckPlatform();
        DataGame = Datos.Load<DataGame>(NameFiles.Data);
    }
    public static void Save()
    {
        DataGame.Save<DataGame>(NameFiles.Data);
        Debug.Log("Data Saved");
    }

    public static GeneralInputs GetInputs { get { return DataGame.settings.iputsPc; } }
    public static void SetInputChange(string s, KeyCode k)
    {
        switch (s)
        {
            case "Avanzar":
                SetAvanza = k;
                break;
            case "Retroceder":
                SetRetrocede = k;
                break;
            case "Izquierda":
                SetIzquierda = k;
                break;
            case "Derecha":
                SetDerecha = k;
                break;
            case "Disparar":
                SetSimpleAttack = k;
                break;
        }
    }
    public static int GetGameVersion { get { return DataGame.game_Version; } }
    public static int GetPackageVersion { get { return DataGame.packages_Version; } }
    public static int SetGameVersion { set => DataGame.game_Version = value; }
    public static int SetPackageVersion { set => DataGame.packages_Version = value; }

    #region PersonajeDatos

    #region Gets
    public static string GetNombre { get => DataGame.personajeDatos.Nombre; }
    public static int GetCoins { get => DataGame.personajeDatos.coins; }
    public static int GetNivel { get => DataGame.personajeDatos.nivel; }
    public static int GetExperiencia { get => DataGame.personajeDatos.experiencia; }
    public static string GetFechaInicio { get => DataGame.personajeDatos.fechaInicio; }
    public static string GetUltimoInicio { get => DataGame.personajeDatos.ultimoInicio; }
    public static int GetDaysTranscurrent { get => DataGame.personajeDatos.daysTranscurrent; }
    public static int GetIndexPlayerMesh { get => DataGame.personajeDatos.indexPlayerMesh; }

    #endregion

    #region Sets
    public static string SetNombre { set { DataGame.personajeDatos.Nombre = value; Save(); } }
    public static int SetCoins { set { DataGame.personajeDatos.coins = value; Save(); } }
    public static int SetNivel { set { DataGame.personajeDatos.nivel = value; Save(); } }
    public static int SetExperiencia { set { DataGame.personajeDatos.experiencia = value; Save(); } }
    public static string SetFechaInicio { set { DataGame.personajeDatos.fechaInicio = value; Save();} }
    public static string SetUltimoInicio { set { DataGame.personajeDatos.ultimoInicio = value; Save(); } }
    public static int SetDaysTranscurrent { set { DataGame.personajeDatos.daysTranscurrent = value; Save(); } }
    public static int SetIndexPlayerMesh { set { DataGame.personajeDatos.indexPlayerMesh = value; Save(); } }

    #endregion

    #endregion

    #region Settings

    #region Sets

    public static int SetQualityLevel { set { DataGame.settings.qualityLevel = value; Save(); } }
    public static bool SetHalfResolution { set { DataGame.settings.halfResolution = value; Save(); } }
    public static float SetMusic { set { DataGame.settings.music = value; Save(); } }
    public static float SetFx { set { DataGame.settings.fx = value; Save(); } }
    public static GeneralInputs SetIputsPc { set { DataGame.settings.iputsPc = value; Save(); } }
    public static HUD SetConfigHUDGame { set { DataGame.settings.configHUDGame = value; Save(); } }
    public static KeyCode SetAvanza { set { DataGame.settings.iputsPc.InputsPc.avanza = value; Save(); } }
    public static KeyCode SetRetrocede { set { DataGame.settings.iputsPc.InputsPc.retrocede = value; Save(); } }
    public static KeyCode SetDerecha { set { DataGame.settings.iputsPc.InputsPc.derecha = value; Save(); } }
    public static KeyCode SetIzquierda { set { DataGame.settings.iputsPc.InputsPc.izquierda = value; Save(); } }
    public static KeyCode SetSimpleAttack { set { DataGame.settings.iputsPc.InputsPc.simpleAttack = value; Save(); } }

    #endregion

    #region Gets

    public static int GetQualityLevel { get => DataGame.settings.qualityLevel; }
    public static bool GetHalfResolution { get => DataGame.settings.halfResolution; }
    public static float GetMusic { get => DataGame.settings.music; }
    public static float GetFx { get => DataGame.settings.fx; }
    public static GeneralInputs GetIputsPc { get => DataGame.settings.iputsPc; }
    public static HUD GetConfigHUDGame { get => DataGame.settings.configHUDGame; }
    public static KeyCode GetAvanza { get => DataGame.settings.iputsPc.InputsPc.avanza; }
    public static KeyCode GetRetrocede { get => DataGame.settings.iputsPc.InputsPc.retrocede; }
    public static KeyCode GetDerecha { get => DataGame.settings.iputsPc.InputsPc.derecha; }
    public static KeyCode GetIzquierda { get => DataGame.settings.iputsPc.InputsPc.izquierda; }
    public static KeyCode GetSimpleAttack { get => DataGame.settings.iputsPc.InputsPc.simpleAttack; }

    #endregion

    #endregion
}