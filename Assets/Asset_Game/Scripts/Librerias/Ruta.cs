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
    /// Crea un archivo Tomando la ruta, tomando en cuenta el nombre y formato del archivo a crear
    /// </summary>
    /// <param name="path">La ruta tomando en cuenta el nombre y formato del archivo a crear</param>
    /// <param name="bytes">los bytes que deben de añadirse en el archivo</param>
    public static void CreateFile(string path, byte[] bytes)
    {
        if (!File.Exists(path))
        {
            File.WriteAllBytes(path, bytes);
            return;
        }
        return;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">La Ruta De Los Archivos Locales</param>
    /// <param name="format">El tipo de formatos que eliminará en la ruta especificada anteriormente</param>
    public static void DeleteFile(string path, string format)
    {
        foreach (string p in Directory.GetFiles(path))
        {
            if (p.EndsWith(format))
            {
                File.Delete(p);
            }
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