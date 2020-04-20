using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterDescriptionUIHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Character c;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(c == null)
        {
            Debug.Log("Character for " + name + " not set");
        }
        GM.ui.info_panel.ShowCharacterDescription(c);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GM.ui.info_panel.HideCharacterDescription(c);
    }
}
