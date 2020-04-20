using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InfoPanel : MonoBehaviour
{
    [SerializeField]
    GameObject description_panel = null;
    Character showing_character_description;
    const int BUTTON_PRIORITY = 100;
    const int CHARACTER_PRIORITY = 0;
    int priority_displayed
    {
        get
        {
            if (showing_button_description != null)
            {
                return BUTTON_PRIORITY;
            }
            return CHARACTER_PRIORITY; // character description
        }
    }
    ButtonDescription showing_button_description;
    public void ShowButtonDescription(ButtonDescription bd)
    {
        if (showing_button_description != null && showing_button_description != bd)
        {
            return;
        }
        showing_button_description = bd;
        description = bd.description;
    }
    public void HideButtonDescription(ButtonDescription bd)
    {
        if (showing_button_description == bd)
        {
            description = "";
            showing_button_description = null;
        }
    }
    public void ShowCharacterDescription(Character c)
    {
        if (priority_displayed > CHARACTER_PRIORITY)
        {
            return;
        }
        if (showing_character_description != null && showing_character_description != c)
        {
            return;
        }
        showing_character_description = c;
        description = c.ToString();
    }
    public void HideCharacterDescription(Character c)
    {
        if (priority_displayed == 0 && showing_character_description == c)
        {
            description = "";
            showing_character_description = null;
        }
    }

    string description
    {
        set
        {
            if (value.Length == 0)
            {

                SetDescriptionActive(false);
                return;
            }
            SetDescriptionActive(true);
            Text t = GetComponentInChildren<Text>();
            t.text = value;
        }
    }

    Coroutine sda_routine = null;
    bool sda_to;
    void SetDescriptionActive(bool to)
    {
        if (sda_routine != null && !sda_to)
        {
            StopCoroutine(sda_routine);
        }
        sda_to = to;
        sda_routine = StartCoroutine(SetDescriptionActiveStep(to));
    }
    Image img { get { return GetComponent<Image>(); } }
    void SetActive(bool to)
    {
        img.enabled = to;
        transform.Find("Text").GetComponent<Text>().enabled = to;
    }
    IEnumerator SetDescriptionActiveStep(bool to)
    {
        yield return to ? null : new WaitForSeconds(.5f);
        SetActive(to);
    }
    public void Initialize()
    {
        SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void ForceHideDescription()
    {
        showing_button_description = null;
        showing_character_description = null;
        description = "";
    }
}
