using System;
using System.Collections.Generic;

#region CLASES
[Serializable]
public class RemoteSettings
{
    public List<RemoteData> remoteDatas = new List<RemoteData>();

    public string GetString(string nameValue)
    {
        foreach (RemoteData r in remoteDatas)
        {
            if (r.name == nameValue)
            {
                return r.value;
            }
        }
        return string.Empty;
    }
    public int GetInt(string nameValue)
    {
        foreach(RemoteData r in remoteDatas)
        {
            if(r.name == nameValue)
            {
                return int.Parse(r.value);
            }
        }
        return 0;
    }
    public float GetFloat(string nameValue)
    {
        foreach (RemoteData r in remoteDatas)
        {
            if (r.name == nameValue)
            {
                return float.Parse(r.value);
            }
        }
        return 0.0f;
    }
    public bool GetBool(string nameValue)
    {
        foreach (RemoteData r in remoteDatas)
        {
            if (r.name == nameValue)
            {
                return bool.Parse(r.value);
            }
        }
        return false;
    }
}
[Serializable]
public class RemoteData
{
    public string name;
    public string tipoDeValue;
    public string value;
}
[Serializable]
public class DataGame
{
    public PersonajeDatos personajeDatos;
    public Settings settings;
    public InventoryItems inventory;
    public int game_Version;
    public int packages_Version;

    public DataGame()
    {
        this.personajeDatos = new PersonajeDatos();
        this.settings = new Settings();
        this.inventory = new InventoryItems();
    }
}
[Serializable]
public class PersonajeDatos
{
    public string Nombre;
    public int coins = 0;
    public int nivel = 0;
    public int experiencia = 0;
    public string fechaInicio;
    public string ultimoInicio;
    public int daysTranscurrent;
    public int indexPlayerMesh;

    public override string ToString()
    {
        return string.Format("{0}: {1} nivel {2}", Nombre, nivel, experiencia);
    }
}

[Serializable]
public class Settings
{
    public int qualityLevel = 4;
    public bool halfResolution = false;
    public float music = 1;
    public float fx = 1;
    public GeneralInputs iputsPc;
    public HUD configHUDGame;
}
[Serializable]
public class HUD
{
    public List<ItemHUD> itemsHUD = new List<ItemHUD>();
    public List<ItemHUD> itemsHUDDEFAULT = new List<ItemHUD>();

}
[Serializable]
public class ItemHUD
{
    public int id;
    public float posX, posY;
    public float escX, escY;
}

[Serializable]
public struct NameFiles
{
    public const string Data = "Data";
    public const string RemoteSettings = "RemoteSettings";
}
#endregion