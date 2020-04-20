using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string description;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(description.Length == 0)
        {
            Debug.Log("description for " + name + " not set");
        }
        GM.ui.info_panel.ShowButtonDescription(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GM.ui.info_panel.HideButtonDescription(this);
    }

    public void OnDestroy()
    {
        GM.ui.info_panel.HideButtonDescription(this);
    }
}
