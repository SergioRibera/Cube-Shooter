using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeCollecionable { Armor, Coin, Wood, Iron, Radiactive, Glass }

public class ItemsColleccionables : MonoBehaviour
{
    public float turnVelocity = .2f;
    public TypeCollecionable typeCollecionable;
    public InventoryItemsDataBase db;
    public int cantidad;
    public int monedas;

    Vector3 rot;

    private void Start()
    {
        rot = transform.rotation.eulerAngles;
        cantidad = Random.Range(12, 50);
        monedas = Random.Range(30, 150);
    }


    private void LateUpdate()
    {
        transform.Rotate(rot + Vector3.up * 360 * Time.deltaTime * turnVelocity);
    }

    private void OnTriggerEnter(Collider o)
    {
        if (o.GetComponent<PlayerController>())
        {
            PlayerController player = o.GetComponent<PlayerController>();
            switch (typeCollecionable)
            {
                case TypeCollecionable.Armor:
                    player.weapon.stats.max_amount += cantidad;
                    player.UpdateMuniGrafics();
                    break;
                case TypeCollecionable.Coin:
                    player.coins += monedas;
                    break;
                case TypeCollecionable.Wood:
                    ItemCollects wood = db.FindItem("Madera");
                    wood.stats.cantidad = cantidad;
                    player.inventoryItems.AddItem(wood);
                    break;
                case TypeCollecionable.Iron:
                    ItemCollects iron = db.FindItem("Metal");
                    iron.stats.cantidad = cantidad;
                    player.inventoryItems.AddItem(iron);
                    break;
                case TypeCollecionable.Radiactive:
                    ItemCollects radiactive = db.FindItem("Contenedor Radioactivo");
                    radiactive.stats.cantidad = cantidad;
                    player.inventoryItems.AddItem(radiactive);
                    break;
                case TypeCollecionable.Glass:
                    ItemCollects glass = db.FindItem("Vidrio");
                    glass.stats.cantidad = cantidad;
                    player.inventoryItems.AddItem(glass);
                    break;
            }
            Destroy(gameObject);
        }
    }
}
