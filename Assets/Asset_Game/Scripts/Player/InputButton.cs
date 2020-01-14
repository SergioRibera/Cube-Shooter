using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Multiplayer.Networking;
using UnityEngine.Serialization;

public class InputButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool isPlayerInteractable = true;
    public TypeButton typeButton;
    public Color colorPress;
    public GameObject go;

    [FormerlySerializedAs("onClick")]
    [SerializeField]
    public Button.ButtonClickedEvent m_OnClick = new Button.ButtonClickedEvent();
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    public Button.ButtonClickedEvent m_OnClickUp = new Button.ButtonClickedEvent();

    internal ItemCollects item;
    internal PlayerController player;
    internal PlayerControllerNetwork playerN;

    NetworkIdentity n;
    InputMovement inputs;
    Color colorDefault = Color.white;
    Image i;

    public void SetPlayer(NetworkIdentity ni, PlayerControllerNetwork p)
    {
        n = ni;
        playerN = p;
        inputs = playerN.movement;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        m_OnClick?.Invoke();
        if (colorDefault == Color.white)
            colorDefault = GetComponent<Image>().color;
        i = GetComponent<Image>();
        i.color = colorPress;
        StartCoroutine(ChangeColor(.3f));
        if (!isPlayerInteractable) return;
        if (n != null && playerN != null)
        {
            if (n.IsMe())
            {
                if (typeButton != TypeButton.CraftButton && typeButton != TypeButton.CraftButtonClose && typeButton != TypeButton.ItemSugerenceCraft && typeButton != TypeButton.Craft)
                {
                    inputs?.DoValueBool(typeButton.ToString());
                }
                else if (typeButton == TypeButton.CraftButton)
                {
                    go.SetActive(true);
                    gameObject.SetActive(false);
                    GetComponent<Image>().color = colorDefault;
                }
                else if (typeButton == TypeButton.CraftButtonClose)
                {
                    go.SetActive(false);
                    gameObject.SetActive(true);
                    GetComponent<Image>().color = colorDefault;
                }
                else if (typeButton == TypeButton.ItemSugerenceCraft)
                {
                    GetComponent<Image>().color = colorDefault;
                    playerN.RefreshDetailsCraftMenu(item);
                }
                else if (typeButton == TypeButton.Craft)
                {
                    playerN.Craft();
                    GetComponent<Image>().color = colorDefault;
                }
            }
            else
            {
                if (typeButton != TypeButton.CraftButton && typeButton != TypeButton.CraftButtonClose && typeButton != TypeButton.ItemSugerenceCraft && typeButton != TypeButton.Craft)
                {
                    inputs?.DoValueBool(typeButton.ToString());
                }
                else if (typeButton == TypeButton.CraftButton)
                {
                    go.SetActive(true);
                    gameObject.SetActive(false);
                    GetComponent<Image>().color = colorDefault;
                }
                else if (typeButton == TypeButton.CraftButtonClose)
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
        }
        else
        {
            if (colorDefault == Color.white)
                colorDefault = GetComponent<Image>().color;
            GetComponent<Image>().color = colorPress;
            if (typeButton != TypeButton.CraftButton && typeButton != TypeButton.CraftButtonClose && typeButton != TypeButton.ItemSugerenceCraft && typeButton != TypeButton.Craft)
            {
                inputs?.DoValueBool(typeButton.ToString());
            }
            else if (typeButton == TypeButton.CraftButton)
            {
                go.SetActive(true);
                gameObject.SetActive(false);
                GetComponent<Image>().color = colorDefault;
            }
            else if (typeButton == TypeButton.CraftButtonClose)
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
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        m_OnClickUp?.Invoke();
        StartCoroutine(ChangeColor(0.2f));
        if (!isPlayerInteractable) return;
        if (n != null && playerN != null)
        {
            if (n.IsMe())
            {
                if (typeButton != TypeButton.CraftButton && typeButton != TypeButton.CraftButtonClose && typeButton != TypeButton.ItemSugerenceCraft && typeButton != TypeButton.Craft)
                {
                    inputs?.DoValueBool(typeButton.ToString(), 1);
                }
            }
            else
            {
                if (typeButton != TypeButton.CraftButton && typeButton != TypeButton.CraftButtonClose && typeButton != TypeButton.ItemSugerenceCraft && typeButton != TypeButton.Craft)
                {
                    inputs?.DoValueBool(typeButton.ToString(), 1);
                }
            }
        }
        else
        {
            GetComponent<Image>().color = colorDefault;
            if (typeButton != TypeButton.CraftButton && typeButton != TypeButton.CraftButtonClose && typeButton != TypeButton.ItemSugerenceCraft && typeButton != TypeButton.Craft)
            {
                inputs?.DoValueBool(typeButton.ToString(), 1);
            }
        }
    }

    IEnumerator ChangeColor(float f)
    {
        yield return new WaitForSeconds(f);
        i.color = colorDefault;
    }
}
public enum TypeButton { Shoot, Reload, Pause, Revivir, CraftButton, CraftButtonClose, ItemSugerenceCraft, Craft }