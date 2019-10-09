using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System;
using System.ComponentModel;
using Assets.SimpleAndroidNotifications;

public class DownloadExtraPackage : MonoBehaviour
{

    [Header("Este es el link de donde se descargaran los datos")]
    public string linkDownload = "http://download1076.mediafire.com/fdcliuric96g/2zr8083yar335wu/MultipleBatallaHtml.rar";
    [Header("El peso aproximado del packete en mb")]
    public float pesoAprox = 42.91f;

    [Header("Nombre del packete del APK. Ej.- com.mycompany.myproyect")]
    public string packageName = "com.mycompany.myproyect";



    public string bundleVersionCode = "6";

    string pathToDownload;
    // Start is called before the first frame update
    void Start()
    {
        if (RunningOnAndroid())
        {
            pathToDownload = Raiz;
            if(!File.Exists(pathToDownload + "main." + bundleVersionCode + "." + packageName + ".obb"))
            {
                CreateMenu();
            }
        }
    }

    GameObject objMenu;
    GameObject slider;
    void CreateMenu()
    {
        objMenu = new GameObject("Alerta De Descarga");
        objMenu.AddComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, .3f);
        var textTittle = new GameObject("Tittle");
        textTittle.AddComponent<UnityEngine.UI.Text>().color = Color.black;
        textTittle.GetComponent<UnityEngine.UI.Text>().fontSize = 24;
        textTittle.GetComponent<UnityEngine.UI.Text>().text = string.Format("Esta aplicación necesita de datos extras, ¿Desea continuar? (se requieren al rededor de {0} mb)", pesoAprox);

        slider = new GameObject("progressBar");
        slider.AddComponent<UnityEngine.UI.Slider>().maxValue = 100;

        var btnCancel = new GameObject("btn_Cancel");
        btnCancel.AddComponent<UnityEngine.UI.Button>().onClick.AddListener(()=> { Cancel(); });

        var btnAcept = new GameObject("btn_Acept");
        btnCancel.AddComponent<UnityEngine.UI.Button>().onClick.AddListener(()=> { Acept(); });

        textTittle.transform.SetParent(objMenu.transform);
        slider.transform.SetParent(objMenu.transform);
        btnAcept.transform.SetParent(objMenu.transform);
        btnCancel.transform.SetParent(objMenu.transform);
    }

    void DownloadPackages()
    {
        var client = new WebClient();
        client.Dispose();
        client.DownloadFileAsync(new Uri(linkDownload), pathToDownload + "main." + bundleVersionCode + "." + packageName + ".obb");
        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadPackageProgressChanged);
        client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadPackagesComplete);
    }

    public void Acept()
    {
        DownloadPackages();
    }
    public void Cancel()
    {
        Destroy(objMenu);
        Application.Quit();
    }

    private void DownloadPackagesComplete(object sender, AsyncCompletedEventArgs e)
    {
        NotificationManager.Send(new TimeSpan(0, 0, 0, 0), "Download Package Progress", "Download Finish", Color.green);
    }

    private void DownloadPackageProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        slider.GetComponent<UnityEngine.UI.Slider>().value = e.ProgressPercentage;
        NotificationManager.Send(new TimeSpan(0,0,0,0), "Download Package Progress", e.ProgressPercentage + " %", Color.blue);
    }

    #region Funciones de mi libreria personal
    private static AndroidJavaClass environment;
    /// <summary>
    /// Retornará la ruta completa Más el de la carpeta de la aplicacion
    /// </summary>
    string Raiz
    {
        get
        {
            if (RunningOnAndroid())
            {
                environment = new AndroidJavaClass("android.os.Environment");
                using (AndroidJavaObject externalStorageDirectory = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                {
                    string root = externalStorageDirectory.Call<string>("getPath") + "/" + Application.companyName;
                    return string.Format("{0}/Android/obb/{1}/", root, packageName);
                }
            }
            return "C://Android/obb/" + packageName + "/";
        }
    }
    bool RunningOnAndroid()
    {
        return Application.platform == RuntimePlatform.Android;
    }
    #endregion
}
