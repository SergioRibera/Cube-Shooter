using LibraryPersonal;
using Multiplayer.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerNetwork : MonoBehaviour
{
    [SerializeField]
    float velocity = 2;
    [SerializeField]
    float turnVelocity = 3;
    [SerializeField]
    VariableJoystick joystick;
    [SerializeField]
    Transform canonShoot;
    [SerializeField]
    InventoryDataBase databaseWeapons;
    [SerializeField]
    internal InventoryItemsDataBase itemsDataBase;
    [SerializeField] TMPro.TextMeshProUGUI textMuni;
    [SerializeField] Transform msgContainer;
    [SerializeField] GameObject textMsg;
    [SerializeField] Transform sugerenceCraftContainer;
    [SerializeField] GameObject detailsCraftContainer;
    [SerializeField] internal GameObject buttonCraft;
    [SerializeField] internal GameObject sugerencesToCraft;
    [SerializeField] internal GameObject detailsCraft;
    [SerializeField] internal GameObject btnRevivingPlayer;

    [SerializeField] TextMeshPro showName;
    [SerializeField] Transform directionalCircle;

    [SerializeField] int vida = 100;
    [SerializeField] Image live_Grafic;
    [SerializeField] internal Image live_GraficInCircle;


    [SerializeField]
    internal InputMovement movement = new InputMovement();
    [SerializeField]
    internal InventoryItems inventoryItems;
    [SerializeField]
    internal Items weapon;

    Rigidbody rb;
    Animator anim;
    int municionACargar = 0;
    private float shootCounter;
    bool disparo;
    internal int coins;

    float h = 0.0f;
    float v = 0.0f;
    float ph = 0.0f;
    float pv = 0.0f;
    NetworkIdentity ni;
    float stillCount;

    GeneralInputs inputPc;


    internal Player player = new Player();
    Color myColor;
    Vector3 oldPosition;
    public void SetPlayer(Player p)
    {
        player = p;
        live_GraficInCircle.color = p.playerColor.hexToColor();
        myColor = p.playerColor.hexToColor();
        showName.text = p.username;
        inputPc = ManagerDataPlayer.GetInputs;
        GetComponent<bl_Hud>().HudInfo.m_Color = myColor;
        ni = GetComponentInParent<NetworkIdentity>();
        GetComponent<bl_Hud>().HudInfo.m_Text = p.username;
        GetComponent<bl_Hud>().enabled = !ni.IsMe();
    }
    void Start()
    {
        ni = GetComponentInParent<NetworkIdentity>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        movement.StartedConfig(anim, transform, canonShoot, directionalCircle);
        Items it = databaseWeapons.FindItem(0);
        weapon.id = it.id;
        weapon.name = it.name;
        weapon.stats = it.stats;
        weapon.typeItem = it.typeItem;
        textMuni.text = weapon.stats.municion + " / " + weapon.stats.max_amount;
        SetLife(vida);

        transform.parent.Find("Canvas").Find("Inputs").Find("Shoot").GetComponent<InputButton>().SetPlayer(ni, this);
        showName.gameObject.SetActive(!ni.IsMe());
        transform.parent.GetComponentInChildren<Camera>().gameObject.SetActive(ni.IsMe());
        transform.parent.Find("Canvas").Find("Inputs").gameObject.SetActive(GeneralInputs.isMovil);
        if (!ni.IsMe())
        {
            Destroy(transform.parent.Find("Camera").gameObject);
            Destroy(transform.parent.Find("Canvas").gameObject);
            foreach (var s in GetComponentsInChildren<MeshRenderer>())
            {
                s.gameObject.layer = 0;
            }
            foreach (var s in GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.gameObject.layer = 0;
            }
        }
        else
        {
            GameObject.FindGameObjectWithTag("loadingScreen").SetActive(false);
        }
        oldPosition = transform.position;
        inputPc.SetMine(ni);
        btnRevivingPlayer.SetActive(false);
    }
    void FixedUpdate()
    {
        if (ni.IsMe())
        {
            h = GeneralInputs.isMovil ? joystick.Horizontal : inputPc.HorizontalAxis;
            v = GeneralInputs.isMovil ? joystick.Vertical : inputPc.VerticalAxis;
            
            movement.MovePlayer(h, v, velocity, turnVelocity);
            if (movement.Shoot || inputPc.Fire1(r))
                Shoot();
            if (weapon.stats.municion == 0 || movement.Reload)
                Reload();

            if (ph != h || pv != v || oldPosition != transform.position)
            {
                ph = h;
                pv = v;
                stillCount = 0;
                sendDataPosition();
            }
            else
            {
                stillCount += Time.deltaTime;
                if (stillCount >= .01f)
                {
                    stillCount = 0;
                    sendDataPosition();
                }
            }
        }
    }

    public void Shoot()
    {
        shootCounter -= Time.deltaTime;
        if (shootCounter < 0 && weapon.stats.municion > 0 && !movement.Reload)
        {
            player.shoot = true;
            ni.GetSocket().Emit("Shoot", new JSONObject(JsonUtility.ToJson(player)));
            print("Shoot sending");
            weapon.stats.municion--;
            municionACargar++;
            UpdateMuniGrafics();
            shootCounter = Time.time + weapon.stats.fireRate;
            GameObject proyectil = Instantiate(weapon.stats.proyectilPref, canonShoot.position, Quaternion.identity);
            proyectil.GetComponent<ProyectilWeaponNetwork>().Init(weapon.stats.damage, this, weapon.stats.proyectilVelocity, myColor, true);
            movement.Shoot = false;
        }
        else
        {
            player.shoot = false;
            ni.GetSocket().Emit("Shoot", new JSONObject(JsonUtility.ToJson(player)));
            print("Shoot sending");
            movement.Shoot = false;
            shootCounter = 0;
        }
    }
    void Reload()
    {
        if (municionACargar > 0 && weapon.stats.max_amount > 0)
        {
            weapon.stats.max_amount -= municionACargar;
            weapon.stats.municion += municionACargar;
            if (weapon.stats.max_amount <= 0) weapon.stats.max_amount = 0;
            municionACargar = 0;
            UpdateMuniGrafics();
        }
    }
    internal void TakeDamage(int d)
    {
        vida = (vida > 0) ? vida -= d : 0;
        float amount = vida * .01f;
        movement.Life(vida);
        live_Grafic.fillAmount = amount;
        live_GraficInCircle.fillAmount = amount;
        SendDataHeath();
    }
    internal void SetAumentLife(int l)
    {
        vida = (vida < 100) ? vida += l : 100;
        float amount = vida * .01f;
        movement.Life(vida);
        live_Grafic.fillAmount = amount;
        live_GraficInCircle.fillAmount = amount;
        SendDataHeath();
    }
    internal void SetLife(int l)
    {
        vida = l;
        float amount = vida * .01f;
        movement.Life(vida);
        if (live_Grafic != null)
            live_Grafic.fillAmount = amount;
        live_GraficInCircle.fillAmount = amount;
        SendDataHeath();
    }

    public bool Died { get { return (vida <= 0); } }


    public void SetNewValueMovement(Vector3 p, float x, float y)
    {
        h = x;
        v = y;
        transform.position = p;
        print("Moviendo al jugador de manera remota");
        //print(p.ToString());
        movement.MovePlayer(h, v, velocity, turnVelocity);
    }
    public void SetNewValueShoot(bool s)
    {
        movement.Shoot = s;
        if (movement.Shoot)
            Shoot();
    }

    bool r = false;
    PlayerControllerNetwork pRev = null;
    // Collision Set Value
    public void SetReviving(bool _r, PlayerControllerNetwork p = null)
    {
        r = _r;
        pRev = p;
        if (ni.IsMe())
            btnRevivingPlayer.SetActive(true);
        StopCoroutine(TimerReviving(pRev));
    }
    // Buton Reviving
    public void Reviving()
    {
        movement.IsReviving(r);
        if (ni.IsMe())
        {
            SendReviving(r);
            if (r)
            {
                StartCoroutine(TimerReviving(pRev));
            }
            else
            {
                StopCoroutine(TimerReviving(pRev));
            }
        }
    }
    IEnumerator TimerReviving(PlayerControllerNetwork p)
    {
        int s = 0;
        while (s <= 180)
        {
            yield return new WaitForSecondsRealtime(.1f);
            s += 3;
            if (ni.IsMe())
            {
                btnRevivingPlayer.transform.Find("Image").GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, s);
                btnRevivingPlayer.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(s / 60) + " s";
            }
        }
        r = false;
        if (ni.IsMe())
        {
            btnRevivingPlayer.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Revivir";
            btnRevivingPlayer.transform.Find("Image").GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
            btnRevivingPlayer.SetActive(false);
        }
        movement.IsReviving(r);
        p?.SetLife(100);
    }


    
    void SendDataHeath()
    {
        player.Life = vida;
        //print(JsonUtility.ToJson(player));
        ni?.GetSocket()?.Emit("updateLife", new JSONObject(JsonUtility.ToJson(player)));
    }
    void sendDataPosition()
    {
        player.h = h;
        player.v = v;
        player.position = new Position
        {
            x = transform.position.x.ToString(CultureInfo.InvariantCulture),
            y = transform.position.y.ToString(CultureInfo.InvariantCulture),
            z = transform.position.z.ToString(CultureInfo.InvariantCulture)
        };

        //print(JsonUtility.ToJson(player));
        ni?.GetSocket().Emit("updatePosition", new JSONObject(JsonUtility.ToJson(player)));
    }
    void SendReviving(bool r)
    {
        player.isReviving = r;
        //print(JsonUtility.ToJson(player));
        ni?.GetSocket().Emit("updateLife", new JSONObject(JsonUtility.ToJson(player)));
    }


    internal void UpdateMuniGrafics()
    {
        textMuni.text = weapon.stats.municion + " / " + weapon.stats.max_amount;
    }
    internal void ShowMesage(string msg, Color c)
    {
        GameObject txtmg = Instantiate(textMsg);
        txtmg.GetComponent<TMPro.TextMeshProUGUI>().SetText(msg);
        txtmg.GetComponent<TMPro.TextMeshProUGUI>().color = c;
        txtmg.transform.parent = msgContainer;
        Destroy(txtmg, 4.5f);
    }
    public void RefreshCraftSugerenceMenu(List<ItemCollects> itemsCraftables)
    {
        for (int i = 0; i < sugerenceCraftContainer.transform.childCount; i++)
        {
            Destroy(sugerenceCraftContainer.GetChild(i).gameObject);
        }
        foreach (var item in itemsCraftables)
        {
            GameObject go = Instantiate(sugerencesToCraft);
            go.transform.SetParent(sugerenceCraftContainer.transform);
            go.GetComponent<Image>().sprite = item.stats.itemImage;
            go.GetComponent<InputButton>().item = item;
            go.GetComponent<InputButton>().SetPlayer(ni, this);

        }
    }
    public void RefreshDetailsCraftMenu(ItemCollects item)
    {
        if (detailsCraftContainer.transform.Find("CraftDetail"))
            Destroy(detailsCraftContainer.transform.Find("CraftDetail").gameObject);
        if (item != null)
        {
            GameObject go = Instantiate(detailsCraft, detailsCraftContainer.transform);
            go.transform.SetParent(detailsCraftContainer.transform);
            go.name = "CraftDetail";
            go.transform.Find("itemIcon").GetComponent<Image>().sprite = item.stats.itemImage;
            go.transform.Find("itemName").GetComponent<TMPro.TextMeshProUGUI>().text = item.name;
            go.transform.Find("itemName").GetComponent<TMPro.TextMeshProUGUI>().GraphicUpdateComplete();
            go.transform.Find("ButtonCraft").GetComponent<InputButton>().SetPlayer(ni, this);
            for (int i = 0; i < item.itemsRequired.Count; i++)
            {
                go.transform.Find("Panel").Find("required_" + i).gameObject.SetActive(true);
                go.transform.Find("Panel").Find("required_" + i).Find("Image").GetComponentInChildren<Image>().sprite = itemsDataBase.FindItem(item.itemsRequired[i].idItem).stats.itemImage;
                go.transform.Find("Panel").Find("required_" + i).GetComponentInChildren<TMPro.TextMeshProUGUI>().text = itemsDataBase.FindItem(item.itemsRequired[i].idItem).name + "  x" + item.itemsRequired[i].cantidad;
            }

        }
    }
    internal void Craft()
    {
        inventoryItems.CraftItem(detailsCraftContainer.transform.Find("CraftDetail").Find("itemName").GetComponent<TMPro.TextMeshProUGUI>().text, this);
        UpdateMuniGrafics();
    }
}