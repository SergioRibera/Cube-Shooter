using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftTable : MonoBehaviour
{
    public InventoryItemsDataBase db;

    private void OnTriggerEnter(Collider p)
    {
        if (p.CompareTag("Player"))
        {
            PlayerController player = p.GetComponent<PlayerController>();
            player.buttonCraft.SetActive(true);
            player.RefreshDetailsCraftMenu(null);
            player.RefreshCraftSugerenceMenu(player.inventoryItems.GetItemsCraftables(db));
            
        }
    }
    private void OnTriggerExit(Collider p)
    {
        if (p.CompareTag("Player"))
        {
            PlayerController player = p.GetComponent<PlayerController>();
            player.buttonCraft.SetActive(false);
        }
    }
}