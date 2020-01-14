using System;
using System.IO;
using UnityEngine;

public class Ruta
{

    private static AndroidJavaClass environment;
    /// <summary>
    /// Retornará la ruta completa Más el de la carpeta de la aplicacion
    /// </summary>
    public static string Raiz
    {
        get
        {
            if (RunningOnAndroid())
            {
                environment = new AndroidJavaClass("android.os.Environment");
                using (AndroidJavaObject externalStorageDirectory = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                {
                    string root = externalStorageDirectory.Call<string>("getPath") + "/" + Application.companyName;

                    return string.Format("{0}/{1}/", root, Application.productName + "/");
                }
            }
            return "C://Sergio_Ribera/" + Application.productName + "/";
        }
    }
    public static string DirAssets
    {
        get
        {
            if (RunningOnAndroid())
            {
                environment = new AndroidJavaClass("android.os.Environment");
                using (AndroidJavaObject externalStorageDirectory = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                {
                    string root = externalStorageDirectory.Call<string>("getPath") + "/" + Application.companyName;

                    return string.Format("{0}/{1}/", root, Application.productName + "/resources/");
                }
            }
            return "C://Sergio_Ribera/" + Application.productName + "/resources/";
        }
    }

    public static bool RunningOnAndroid()
    {
        return Application.platform == RuntimePlatform.Android;
    }
    public static void Init()
    {
        CrearDrirectorio(Raiz);
        CrearDrirectorio(DirAssets);
    }
    static void CrearDrirectorio(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                return;
            }
            else
            {
                Directory.CreateDirectory(path);
                return;
            }
        }
        catch (Exception e)
        {

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="folderName">nombre de la carpeta en la que se aloja el archivo</param>
    /// <param name="fileName">Nombre Del Archivo a buscar</param>
    /// <returns>Verifica si existe un archivo en específico</returns>
    public static bool ExistFile(string fileName)
    {
        return File.Exists(Raiz + fileName);
    }
}