using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopButton : MonoBehaviour
{
    public Image char_icon
    {
        get { return transform.Find("charicon").GetComponent<Image>(); }
    }
    public Sprite char_image
    {
        set
        {
            char_icon.sprite = value;
        }
    }
    public Button button
    {
         get{ return transform.Find("button").GetComponent<Button>(); } 
    }

    public Image button_image
    {
        get
        {
            return button.GetComponent<Image>();
        }
    }
    public Sprite button_sprite
    {
        set
        {
            button_image.sprite = value;
        }
    }

    public string text {
        
        set
        {
            bd_script.description = value;
        }

    }

    public ButtonDescription bd_script
    {
        get
        {
            return button.GetComponentInChildren<ButtonDescription>();
        }
    }

    public int price
    {
        set
        {
            transform.Find("priceText").GetComponent<Text>().text = value.ToString();
        }
    }

    public Character character
    {
        set
        {
            transform.GetComponentInChildren<CharacterDescriptionUIHover>().c = value;
        }
    }
}
