using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public DataGame dataGame;
    public RemoteSettings remoteSettings;

    MenuManager menu;
    public static DataManager singleton;

    string[] indexToDownload;
    float progress;
    bool download;
    int i = 0;
    WebClient client;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            dataGame.personajeDatos.daysTranscurrent++;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
        }
        StartCoroutine(Datos.ConfigRemoteSettings(this));
        Datos.Init();
        DontDestroyOnLoad(this);
        menu = FindObjectOfType<MenuManager>();
        if (Datos.Json.ExistePlayerData())
        {
            dataGame = Datos.Json.Load<DataGame>(NameFiles.Data);
            /*if (string.IsNullOrEmpty(dataGame.personajeDatos.nombre))
                menu.ConfigureName();*/
            StartCoroutine(CompareVersions());
        }
        else
        {
            //menu.ConfigureName();
            StartCoroutine(DownloadPackages());
        }
    }
    internal void Compare_Version()
    {
        StartCoroutine(CompareVersions());
    }
    //Falta Url de Descarga De APK
    IEnumerator CompareVersions()
    {
        if (remoteSettings != null && remoteSettings.remoteDatas.Count != 0)
        {
            if (remoteSettings.remoteDatas.Find(i => i.name == "Game_Version") != null)
            {
                if (dataGame.game_Version < remoteSettings.GetInt("Game_Version"))
                {
                    dataGame.game_Version = remoteSettings.GetInt("Game_Version");
                    Application.OpenURL(remoteSettings.GetString("link_Download_Apk"));
                    SaveData();
                }
            }
            if (remoteSettings.remoteDatas.Find(i => i.name == "Game_Version") != null)
            {
                if (remoteSettings.GetBool("packages"))
                {
                    if (dataGame.packages_Version < int.Parse(remoteSettings.remoteDatas.Find(i => i.name == "Packages_Version").value))
                    {
                        dataGame.packages_Version = int.Parse(remoteSettings.remoteDatas.Find(i => i.name == "Packages_Version").value);
                        StartCoroutine(DownloadPackages());
                        SaveData();
                    }
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(2f);
            WWW con = new WWW("https://www.google.com/");
            yield return con;
            if (con.error != null)
            {
                StartCoroutine(CompareVersions());
            }
            else
                Datos.ShowToast("Revisa Tu Coneccion A Internet");
        }
    }
    IEnumerator DownloadPackages()
    {
        //menu.ActiveDownloadScreen();

        //Download Packages Code
        WWW downloadIndex = new WWW("http://www.sergioriberadesarrollo.tk/RemoteSettings/dex.php");
        yield return downloadIndex;
        Datos.CreateFilesToDownload(Ruta.DirAssets + "dex", downloadIndex.text);
        indexToDownload = Datos.GetIndex();

        print(indexToDownload[i].Split('|')[2]);
        client = new WebClient();
        client.Dispose();
        client.DownloadFileAsync(new Uri(indexToDownload[i].Split('|')[0]), Ruta.DirAssets + indexToDownload[i].Split('|')[1]);
        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadPackageProgressChanged);
        client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadPackagesComplete);
    }

    void DownloadPackageProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        progress = (e.BytesReceived * 100 / e.TotalBytesToReceive);
    }
    void DownloadPackagesComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        if (e.Error == null)
        {
            if (i < indexToDownload.Length - 1)
            {
                i++;
                download = true;
                print(indexToDownload[i].Split('|')[2]);
                client = new WebClient();
                client.Dispose();
                client.DownloadFileAsync(new Uri(indexToDownload[i].Split('|')[0]), Ruta.DirAssets + indexToDownload[i].Split('|')[1]);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadPackageProgressChanged);
                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadPackagesComplete);
                Debug.Log("Completed");
            }
            else
            {
                download = false;
            }
        }
        else
            Debug.Log(e.Error.ToString());
    }

    private void FixedUpdate()
    {
        if (!download)
            return;
        /*if (indexToDownload != null)
            menu.ChangeTextProgress(indexToDownload[i].Split('|')[2], progress, i, indexToDownload.Length);
        if (indexToDownload != null)
            if (i == indexToDownload.Length - 1)
                menu.ChangeTextProgress("Descarga Completada. Reinicie la aplicación.", 100f, i + 1, indexToDownload.Length);
    */
    }

    public void ConfigurePrimerInicio()
    {
        dataGame = new DataGame();
        dataGame.personajeDatos.ultimoInicio = DateTime.Today.ToString("dd-mm-yy");
        dataGame.personajeDatos.fechaInicio = DateTime.Today.ToString("dd-mm-yy");
        SaveData();
    }
    public void SaveData()
    {
        Datos.Json.Save(dataGame, NameFiles.Data);
    }
}
