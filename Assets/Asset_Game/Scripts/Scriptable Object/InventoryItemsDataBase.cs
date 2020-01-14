using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObject/Items DataBase")]
public class InventoryItemsDataBase : ScriptableObject
{
    public List<ItemCollects> items = new List<ItemCollects>();

    public ItemCollects FindItem(int id)
    {
        foreach (ItemCollects i in items)
        {
            if (i.id == id)
            {
                return i;
            }
        }
        return null;
    }
    public ItemCollects FindItem(string name)
    {
        foreach (ItemCollects items in items)
        {
            if (items.name == name)
                return items;
        }
        return null;
    }

}
[System.Serializable]
public class InventoryItems
{
    public List<ItemCollects> items = new List<ItemCollects>();

    public ItemCollects FindItem(int id)
    {
        foreach (ItemCollects i in items)
        {
            if (i.id == id)
            {
                return i;
            }
        }
        return null;
    }
    public ItemCollects FindItem(string name)
    {
        foreach(ItemCollects items in items)
        {
            if (items.name == name)
                return items;
        }
        return null;

    }
    public void AddItem(ItemCollects newItem)
    {
        items.Add(newItem);
    }
    public int GetCount()
    {
        return items.Count;
    }
    public List<ItemCollects> GetItemsCraftables(InventoryItemsDataBase db)
    {
        List<ItemCollects> itemsCraftables = new List<ItemCollects>();
        foreach (var item in db.items)
        {
            if (item.craftable)
            {
                int c = 0;
                foreach (var required in item.itemsRequired)
                {
                    if (FindItem(required.idItem) != null)
                        c++;
                }
                if (c == item.itemsRequired.Count)
                    itemsCraftables.Add(item);
            }                
        }
        return itemsCraftables;
    }
    public void CraftItem(string nameItemToCraft, PlayerController player)
    {
        int craft = 0;
        List<ItemsRequireForCrafting> itemsRequired = new List<ItemsRequireForCrafting>();
        foreach (var item in player.itemsDataBase.FindItem(nameItemToCraft).itemsRequired)
        {
            if(FindItem(item.idItem).stats.cantidad >= item.cantidad)
            {
                craft++;
                itemsRequired.Add(item);
            }
            else
            {
                player.ShowMesage("No Tienes Suficientes Materiales", Color.red);
            }
        }
        if(craft == player.itemsDataBase.FindItem(nameItemToCraft).itemsRequired.Count)
        {
            foreach (var item in itemsRequired)
            {
                FindItem(item.idItem).stats.cantidad -= item.cantidad;
                if (FindItem(item.idItem).stats.cantidad <= 0)
                    FindItem(item.idItem).stats.cantidad = 0;
                if (FindItem(item.idItem).stats.cantidad == 0)
                {
                    Debug.Log(FindItem(item.idItem).name + " Remove Item");
                    player.RefreshDetailsCraftMenu(null);
                    player.RefreshCraftSugerenceMenu(GetItemsCraftables(player.itemsDataBase));
                }
            }
            ItemCollects newItem = player.itemsDataBase.FindItem(nameItemToCraft);
            newItem.stats.cantidad = newItem.cant_craftado;
            if (newItem.name == "Munición")
            {
                player.weapon.stats.max_amount += newItem.cant_craftado;
            }
            else
                AddItem(newItem);
        }
    }
    public void CraftItem(int idItemToCraft, PlayerController player)
    {
        int craft = 0;
        List<ItemsRequireForCrafting> itemsRequired = new List<ItemsRequireForCrafting>();
        foreach (var item in player.itemsDataBase.FindItem(idItemToCraft).itemsRequired)
        {
            if (FindItem(item.idItem).stats.cantidad >= item.cantidad)
            {
                craft++;
                itemsRequired.Add(item);
            }
        }
        if (craft == player.itemsDataBase.FindItem(idItemToCraft).itemsRequired.Count)
        {
            foreach (var item in itemsRequired)
            {
                FindItem(item.idItem).stats.cantidad -= item.cantidad;
                if (FindItem(item.idItem).stats.cantidad <= 0)
                    FindItem(item.idItem).stats.cantidad = 0;
                if (FindItem(item.idItem).stats.cantidad == 0)
                {
                    Debug.Log(FindItem(item.idItem).name + " Remove Item");
                }
            }
            ItemCollects newItem = player.itemsDataBase.FindItem(idItemToCraft);
            newItem.stats.cantidad = newItem.cant_craftado;
            AddItem(newItem);
        }
    }
    public void CraftItem(string nameItemToCraft, PlayerControllerNetwork player)
    {
        int craft = 0;
        List<ItemsRequireForCrafting> itemsRequired = new List<ItemsRequireForCrafting>();
        foreach (var item in player.itemsDataBase.FindItem(nameItemToCraft).itemsRequired)
        {
            if (FindItem(item.idItem).stats.cantidad >= item.cantidad)
            {
                craft++;
                itemsRequired.Add(item);
            }
            else
            {
                player.ShowMesage("No Tienes Suficientes Materiales", Color.red);
            }
        }
        if (craft == player.itemsDataBase.FindItem(nameItemToCraft).itemsRequired.Count)
        {
            foreach (var item in itemsRequired)
            {
                FindItem(item.idItem).stats.cantidad -= item.cantidad;
                if (FindItem(item.idItem).stats.cantidad <= 0)
                    FindItem(item.idItem).stats.cantidad = 0;
                if (FindItem(item.idItem).stats.cantidad == 0)
                {
                    Debug.Log(FindItem(item.idItem).name + " Remove Item");
                    player.RefreshDetailsCraftMenu(null);
                    player.RefreshCraftSugerenceMenu(GetItemsCraftables(player.itemsDataBase));
                }
            }
            ItemCollects newItem = player.itemsDataBase.FindItem(nameItemToCraft);
            newItem.stats.cantidad = newItem.cant_craftado;
            if (newItem.name == "Munición")
            {
                player.weapon.stats.max_amount += newItem.cant_craftado;
            }
            else
                AddItem(newItem);
        }
    }
    public void CraftItem(int idItemToCraft, PlayerControllerNetwork player)
    {
        int craft = 0;
        List<ItemsRequireForCrafting> itemsRequired = new List<ItemsRequireForCrafting>();
        foreach (var item in player.itemsDataBase.FindItem(idItemToCraft).itemsRequired)
        {
            if (FindItem(item.idItem).stats.cantidad >= item.cantidad)
            {
                craft++;
                itemsRequired.Add(item);
            }
        }
        if (craft == player.itemsDataBase.FindItem(idItemToCraft).itemsRequired.Count)
        {
            foreach (var item in itemsRequired)
            {
                FindItem(item.idItem).stats.cantidad -= item.cantidad;
                if (FindItem(item.idItem).stats.cantidad <= 0)
                    FindItem(item.idItem).stats.cantidad = 0;
                if (FindItem(item.idItem).stats.cantidad == 0)
                {
                    Debug.Log(FindItem(item.idItem).name + " Remove Item");
                }
            }
            ItemCollects newItem = player.itemsDataBase.FindItem(idItemToCraft);
            newItem.stats.cantidad = newItem.cant_craftado;
            AddItem(newItem);
        }
    }
}
[System.Serializable]
public class ItemCollects
{
    public int id;
    public string name;
    public bool craftable;
    public ItemsStats stats;
    public List<ItemsRequireForCrafting> itemsRequired = new List<ItemsRequireForCrafting>();
    public int cant_craftado;
    public void RemoveItem(int cantidad)
    {
        stats.cantidad -= cantidad;
        if (stats.cantidad <= 0)
            stats.cantidad = 0;
        if(stats.cantidad == 0)
        {

        }
    }
}
[System.Serializable]
public struct ItemsStats
{
    public Sprite itemImage;
    public int cantidad;
    public int max_cantidad;
}
[System.Serializable]
public struct ItemsRequireForCrafting
{
    public int idItem;
    public int cantidad;
}