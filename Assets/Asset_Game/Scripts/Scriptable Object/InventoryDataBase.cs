using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Inventario")]
public class InventoryDataBase : ScriptableObject
{
    public List<Items> items = new List<Items>();

    public Items FindItem(int id)
    {
        foreach (Items i in items)
        {
            if (i.id == id)
            {
                return i;
            }
        }

        return null;
    }
}

[Serializable]
public class Inventory
{
    public List<Items> items = new List<Items>();

    public Inventory()
    {
        this.items = new List<Items>();
    }
    public void AddItem(Items item, int muni = 0)
    {
        item.stats.municion = muni;
        items.Add(item);
    }
    public Items FindItem(int id)
    {
        foreach (Items i in items)
        {
            if (i.id == id)
            {
                return i;
            }
        }

        return null;
    }
}
[Serializable]
public class Items
{
    public string name;
    public int id;
    public TypeItem typeItem;
    public StatsItem stats;

}
[Serializable]
public struct StatsItem
{
    public Sprite imgWeapon;
    public GameObject weaponPref;
    public GameObject proyectilPref;
    public float proyectilVelocity;
    public int municion;
    public int max_amount;
    public int damage;
    public float fireRate;
}

public enum TypeItem { Principal, Secundaria, Extra }
