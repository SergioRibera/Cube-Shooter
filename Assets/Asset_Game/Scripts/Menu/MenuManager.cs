using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public GameObject dialogConfigureName;
    public TMP_InputField inputFirstName;
    public TMP_Dropdown qualityLevels;
    public AudioMixer audioSettings;
    public Slider music;
    public Slider fx;
    public Transform inputsContent;
    public GameObject inputsPref;

    List<GameObject> prefs = new List<GameObject>();
    Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    Dictionary<string, Action> actions = new Dictionary<string, Action>();

    bool editKey = false;
    string key;

    void Start()
    {
        qualityLevels.AddOptions(QualitySettings.names.ToList());
        qualityLevels.value = ManagerDataPlayer.GetQualityLevel;
        music.value = ManagerDataPlayer.GetMusic;
        fx.value = ManagerDataPlayer.GetFx;

        audioSettings.SetFloat("MusicVolume", ManagerDataPlayer.GetMusic);
        audioSettings.SetFloat("Fx", ManagerDataPlayer.GetFx);

        QualitySettings.SetQualityLevel(ManagerDataPlayer.GetQualityLevel);

        /*keys.Add("Avanzar", ManagerDataPlayer.GetAvanza);
        keys.Add("Retroceder", ManagerDataPlayer.GetRetrocede);
        keys.Add("Izquierda", ManagerDataPlayer.GetIzquierda);
        keys.Add("Derecha", ManagerDataPlayer.GetDerecha);
        keys.Add("Disparar", ManagerDataPlayer.GetSimpleAttack);

        foreach (var i in keys)
        {
            GameObject g = Instantiate(inputsPref, inputsContent);
            g.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = i.Key;
            g.transform.Find("Button").Find("Text").GetComponent<TextMeshProUGUI>().text = i.Value.ToString();
            g.transform.Find("Button").GetComponent<Button>().onClick.AddListener(new UnityAction(()=> {
                editKey = true;
                key = i.Key;
            }));
            prefs.Add(g);
        }
        */
    }

    void RemapInputs()
    {
        keys.Clear();
        keys.Add("Avanzar", ManagerDataPlayer.GetAvanza);
        keys.Add("Retroceder", ManagerDataPlayer.GetRetrocede);
        keys.Add("Izquierda", ManagerDataPlayer.GetIzquierda);
        keys.Add("Derecha", ManagerDataPlayer.GetDerecha);
        keys.Add("Disparar", ManagerDataPlayer.GetSimpleAttack);

        int x = 0;
        foreach (var i in keys)
        {
            GameObject g = prefs[x];
            g.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = i.Key;
            g.transform.Find("Button").Find("Text").GetComponent<TextMeshProUGUI>().text = i.Value.ToString();
            g.transform.Find("Button").GetComponent<Button>().onClick.AddListener(new UnityAction(() => {
                editKey = true;
                key = i.Key;
            }));
            x++;
        }
    }

    Event keyEvent;

    void Update()
    {
        if (!editKey) return;

        keyEvent = Event.current;
        if (keyEvent.isKey)
        {
            keys[key] = keyEvent.keyCode;
            ManagerDataPlayer.SetInputChange(key, keyEvent.keyCode);
            RemapInputs();
            editKey = false;
        }
    }

    #region Settings

    public void ChangeQualityLevel(int i)
    {
        QualitySettings.SetQualityLevel(i);
        ManagerDataPlayer.SetQualityLevel = i;
    }
    public void ChangeMusicVolume(float v)
    {
        audioSettings.SetFloat("MusicVolume", v);
        ManagerDataPlayer.SetMusic = v;
    }
    public void ChangeFxVolume(float v)
    {
        audioSettings.SetFloat("FxVolume", v);
        ManagerDataPlayer.SetFx = v;
    }

    #endregion

    public void ConfigureName() => dialogConfigureName.SetActive(true);
    public void SetName()
    {
        ManagerDataPlayer.SetNombre = inputFirstName.text;
    }
    public void ChangeLevel(int i) => SceneManager.LoadScene(i);
    public void Quit() => Application.Quit();
}