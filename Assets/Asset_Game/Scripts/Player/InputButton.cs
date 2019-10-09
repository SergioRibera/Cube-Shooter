using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public TypeButton typeButton;
    public Color colorPress;
    public InputMovement inputs;
    public GameObject go;

    internal ItemCollects item;
    internal PlayerController player;

    Color colorDefault = Color.white;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (colorDefault == Color.white)
            colorDefault = GetComponent<Image>().color;
        GetComponent<Image>().color = colorPress;
        if (typeButton != TypeButton.CraftButton && typeButton != TypeButton.CraftButtonClose && typeButton != TypeButton.ItemSugerenceCraft && typeButton != TypeButton.Craft)
        {
            inputs.DoValueBool(typeButton.ToString());
        }
        else if(typeButton == TypeButton.CraftButton)
        {
            go.SetActive(true);
            gameObject.SetActive(false);
            GetComponent<Image>().color = colorDefault;
        }
        else if(typeButton == TypeButton.CraftButtonClose)
        {
            go.SetActive(false);
            gameObject.SetActive(true);
            GetComponent<Image>().color = colorDefault;
        }
        else if (typeButton == TypeButton.ItemSugerenceCraft)
        {
            GetComponent<Image>().color = colorDefault;
            player.RefreshDetailsCraftMenu(item);
        }
        else if (typeButton == TypeButton.Craft)
        {
            player.Craft();
            GetComponent<Image>().color = colorDefault;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<Image>().color = colorDefault;
        if (typeButton != TypeButton.CraftButton && typeButton != TypeButton.CraftButtonClose && typeButton != TypeButton.ItemSugerenceCraft && typeButton != TypeButton.Craft)
        {
            inputs.DoValueBool(typeButton.ToString(), 1);
        }
    }
}

public enum TypeButton { Shoot, Reload, Pause, CraftButton, CraftButtonClose, ItemSugerenceCraft, Craft }