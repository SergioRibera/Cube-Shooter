using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Random = UnityEngine.Random;
using System.Collections;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

[Serializable]
public class Datos {

    public static void ShowToast(string note)
    {
        if (Ruta.RunningOnAndroid())
        {
            var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toast = new AndroidJavaClass("android.widget.Toast").CallStatic<AndroidJavaObject>("makeText", activity, note, 1);
                toast.Call("show");
            }));
            return;
        }
        Debug.Log(note);
    }
    public static void OptimizeGraphics(bool value = false)
    {
        if (!value)
        {
            float factor = .5f;
            Screen.SetResolution(
                Mathf.CeilToInt(Screen.currentResolution.width * factor),
                Mathf.CeilToInt(Screen.currentResolution.height * factor), true);
        }
        else
        {
            Screen.SetResolution(
                Mathf.CeilToInt(Screen.currentResolution.width),
                Mathf.CeilToInt(Screen.currentResolution.height), true);
        }
    }

    public static string RandomId(int LengthNewId = 15)
    {
        string newId = "";
        string abecedarioMayus = "A-B-C-D-E-F-G-H-I-J-K-L-M-N-Ñ-O-P-Q-R-S-T-U-V-W-X-Y-Z";
        string abecedarioMinus = "a-b-c-d-e-f-g-h-i-j-k-l-m-n-ñ-o-p-q-r-s-t-u-v-w-x-y-z";
        string caracteres = "-_-/--&-";
        for (int i = 0; i < LengthNewId; i++)
        {
            int value = Random.Range(0, 1);
            string letra = "";
            switch (value)
            {
                case 0:
                    int proba_Char = Random.Range(0, 5);
                    if (proba_Char == 2)
                    {
                        letra = caracteres.Split('-')[Random.Range(0, 4)] + abecedarioMayus.Split('-')[Random.Range(0, 26)];
                    }
                    else
                    {
                        letra = abecedarioMayus.Split('-')[Random.Range(0, 26)];
                    }
                    break;
                case 1:
                    int proba_Char2 = Random.Range(0, 5);
                    if (proba_Char2 == 3)
                    {
                        letra = caracteres.Split('-')[Random.Range(0, 4)] + abecedarioMinus.Split('-')[Random.Range(0, 26)];
                    }
                    else
                    {
                        letra = abecedarioMinus.Split('-')[Random.Range(0, 26)];
                    }
                    break;
            }
            newId = newId + letra;
        }

        return newId;
    }

    public static void Init()
    {
        if (!Directory.Exists(Ruta.Raiz))
            Directory.CreateDirectory(Ruta.Raiz);
        if (!Directory.Exists(Ruta.DirAssets))
            Directory.CreateDirectory(Ruta.DirAssets);
    }
    public static void CreateFilesToDownload(string path, string dataText)
    {
        string directoryPath = path.Replace(Path.GetFileName(path), "");
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);
        File.WriteAllText(path, dataText);
    }

    public static RemoteSettings remoteSettings = null;

    internal static string[] GetIndex()
    {
        string dex = File.ReadAllText(Ruta.DirAssets + "dex");
        string[] splits = new string[dex.Split('\n').Length - 1];
        for (int i = 0; i < dex.Split('\n').Length-1; i++)
        {
            splits[i] = dex.Split('\n')[i];
            string directoryPath = splits[i].Split('|')[1].Replace(Path.GetFileName(splits[i].Split('|')[1]), "");
            if (!Directory.Exists(Ruta.DirAssets + directoryPath))
                Directory.CreateDirectory(Ruta.DirAssets + directoryPath);
            if (File.Exists(Ruta.DirAssets + splits[i].Split('|')[1]))
                File.Delete(Ruta.DirAssets + splits[i].Split('|')[1]);
        }
        return splits;
    }

    public static IEnumerator ConfigRemoteSettings(DataManager d)
    {
        WWW conexion = new WWW("https://sergioribera.000webhostapp.com/RemoteSettings/php/datos.php?proyect=CubeShooter");
        yield return conexion;
        if(conexion.bytesDownloaded == 0 || conexion.text == "404" || conexion.text == "302" || conexion.error != null)
        {
            //error de coneccion
            ShowToast("Revisa Tu Coneccion a Internet");
            if(Ruta.ExistFile(NameFiles.RemoteSettings))
            {
                remoteSettings = Json.Load<RemoteSettings>(NameFiles.RemoteSettings);
            }
        }
        else
        {
            string jsonString = "{" + '"' + "remoteDatas" + '"' + ':' + conexion.text + '}';
            remoteSettings = JsonUtility.FromJson<RemoteSettings>(jsonString);
            Json.CrearArchivos(remoteSettings, NameFiles.RemoteSettings);
        }
        d.remoteSettings = remoteSettings;
        d.Compare_Version();
    }

    #region JSON
    [Serializable]
    public class Json
    {
        static string filePath;
        static string jsonString;
        static internal string path = Ruta.Raiz;

        public static void Save<T>(T datos, string nameFile, bool encrypt = true)
        {
            filePath = path + nameFile;
            if (!File.Exists(filePath))
            {
                CrearArchivos(datos, nameFile);
            }

            if (encrypt)
            {
                jsonString = Encrypt(JsonUtility.ToJson(datos));
            }
            else
            {
                jsonString = JsonUtility.ToJson(datos);
            }
            File.WriteAllText(filePath, jsonString);
        }
        public static T Load<T>(string nameFile, bool desEncrypt = true)
        {
            filePath = path + nameFile;
            if (desEncrypt)
                jsonString = Desencrypt(File.ReadAllText(filePath));
            else
                jsonString = File.ReadAllText(filePath);
            T personaje = JsonUtility.FromJson<T>(jsonString);
            return personaje;
        }
        public static ItemCollects Load(int ID)
        {
            return Load<DataGame>(NameFiles.Data).inventory.items.Find(i => i.id == ID);
        }
        public static void CrearArchivos<T>(T datos, string nameFile)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string jsonPlayer = Encrypt(JsonUtility.ToJson(datos));
            File.WriteAllText(path + nameFile, jsonPlayer);
        }
        public static void GuardarDatosDeHUD(int Id, Vector3 position, Vector3 escala)
        {
            filePath = path + NameFiles.Data;
            try
            {
                HUD listaArmas = JsonUtility.FromJson<DataGame>(jsonString).settings.configHUDGame;
                listaArmas.itemsHUD.Find(i => i.id == Id).posX = position.x;
                listaArmas.itemsHUD.Find(i => i.id == Id).posY = position.y;
                listaArmas.itemsHUD.Find(i => i.id == Id).escX = escala.x;
                listaArmas.itemsHUD.Find(i => i.id == Id).escY = escala.y;
                jsonString = Encrypt(JsonUtility.ToJson(listaArmas));
                File.WriteAllText(filePath, jsonString);
                PlayerPrefs.GetString("DatosHUD", "TRUE");
            }
            catch (NullReferenceException e)
            {
                Debug.LogError("No Hay el elemento/n" + e);
            }
        }
        public static bool ExistePlayerData()
        {
            return Ruta.ExistFile(NameFiles.Data);
        }
        public static ItemHUD RecuperarDatosHUD(int type, int Id)
        {
            if (type == 0)
            {
                filePath = path + NameFiles.Data;
                jsonString = Desencrypt(File.ReadAllText(filePath));

                HUD itemHUD = JsonUtility.FromJson<DataGame>(jsonString).settings.configHUDGame;

                return itemHUD.itemsHUDDEFAULT.Find(i => i.id == Id);
            }
            else if (type == 1)
            {
                filePath = path + NameFiles.Data;
                jsonString = Desencrypt(File.ReadAllText(filePath));

                HUD itemHUD = JsonUtility.FromJson<DataGame>(jsonString).settings.configHUDGame;

                return itemHUD.itemsHUD.Find(i => i.id == Id);
            }
            return null;
        }

        public static string SaveAssetText<T>(T datos, string lenguaje)
        {
            filePath = "D:\\Sergio\\Proyectos\\Cube Shooter\\Assets\\Asset_Game\\ScriptableObject\\Lenguajes";
            string pathText = filePath + "\\" + lenguaje + ".txt";
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

            }
            jsonString = Encrypt(JsonUtility.ToJson(datos));
            File.WriteAllText(pathText, jsonString);
            Debug.Log("Datos Guardados.");
            return jsonString;
        }
        public static T LoadAssetText<T>(string texto)
        {
            jsonString = Desencrypt(texto);
            T t = JsonUtility.FromJson<T>(jsonString);
            Debug.Log("Datos Cargados Correctamente.");
            return t;
        }
        public static string Encrypt(string text)
        {
            string resultado = string.Empty;
            byte[] encriptar = Encoding.Unicode.GetBytes(text);
            resultado = Convert.ToBase64String(Encoding.Unicode.GetBytes(Convert.ToBase64String(encriptar)));

            return resultado;
        }
        public static string Desencrypt(string text)
        {
            string resultado = string.Empty;
            string aux = Encoding.Unicode.GetString(Convert.FromBase64String(text));
            byte[] desencriptar = Convert.FromBase64String(aux);
            resultado = Encoding.Unicode.GetString(desencriptar);

            return resultado;
        }
    }
    #endregion
}
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
    public string nombre;
    public int coins = 0;
    public int nivel = 0;
    public int experiencia = 0;
    public string fechaInicio;
    public string ultimoInicio;
    public int daysTranscurrent;

    public override string ToString()
    {
        return string.Format("{0}: {1} nivel {2}", nombre, nivel, experiencia);
    }
}

[Serializable]
public class Settings
{
    public int qualityLevel = 2;
    public bool halfResolution = false;
    public float music = 1;
    public float fx = 1;
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