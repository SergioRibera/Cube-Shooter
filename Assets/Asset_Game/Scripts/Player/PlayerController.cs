using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : GameBehaviour
{

    public string Nombre;

    [SerializeField]
    float velocity = 2;
    [SerializeField]
    float turnVelocity = 3;
    [SerializeField]
    VariableJoystick joystick;
    [SerializeField]
    InputMovement movement;
    [SerializeField]
    Transform canonShoot;
    [SerializeField]
    InventoryDataBase databaseWeapons;
    [SerializeField]
    internal InventoryItemsDataBase itemsDataBase;
    [SerializeField]
    internal InventoryItems inventoryItems;
    [SerializeField]
    internal Items weapon;
    [SerializeField] TMPro.TextMeshProUGUI textMuni;
    [SerializeField] Transform msgContainer;
    [SerializeField] GameObject textMsg;
    [SerializeField] Transform sugerenceCraftContainer;
    [SerializeField] GameObject detailsCraftContainer;
    [SerializeField] internal GameObject buttonCraft;
    [SerializeField] internal GameObject sugerencesToCraft;
    [SerializeField] internal GameObject detailsCraft;
    

    [SerializeField] int vida = 100;
    [SerializeField] Image live_Grafic;

    Rigidbody rb;
    Animator anim;
    int municionACargar = 0;
    private float shootCounter;
    bool disparo;
    internal int coins;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        movement.StartedConfig(anim, transform, canonShoot);
        Items it = databaseWeapons.FindItem(0);
        weapon.id = it.id;
        weapon.name = it.name;
        weapon.stats = it.stats;
        weapon.typeItem = it.typeItem;
        textMuni.text = weapon.stats.municion + " / " + weapon.stats.max_amount;
    }
    void FixedUpdate()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;
        movement.MovePlayer(h, v, velocity, turnVelocity);
        if (movement.Shoot)
            Shoot();
        if (weapon.stats.municion == 0 || movement.Reload)
            Reload();
    }

    
    public void Shoot()
    {
        shootCounter -= Time.deltaTime;
        if (shootCounter < 0 && weapon.stats.municion > 0 && !movement.Reload)
        {
            weapon.stats.municion--;
            municionACargar++;
            UpdateMuniGrafics();
            shootCounter = Time.time + weapon.stats.fireRate;
            GameObject proyectil = Instantiate(weapon.stats.proyectilPref, canonShoot.position, Quaternion.identity);
            proyectil.GetComponent<ProyectilWeapon>().Init(weapon.stats.damage, this, weapon.stats.proyectilVelocity);
            movement.Shoot = false;
        }
        else
        {
            movement.Shoot = false;
            shootCounter = 0;
        }
    }
    void Reload()
    {
        if(municionACargar > 0 && weapon.stats.max_amount > 0)
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
        live_Grafic.fillAmount = amount;
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
            go.GetComponent<InputButton>().player = this;
            
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
            go.transform.Find("ButtonCraft").GetComponent<InputButton>().player = this;
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
