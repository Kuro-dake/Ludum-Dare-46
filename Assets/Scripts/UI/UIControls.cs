﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIControls : MonoBehaviour
{
    [SerializeField]
    Button button_prefab = null;
    [SerializeField]
    Image sequence_icon_prefab = null;
    Canvas canvas { get { return GetComponent<Canvas>(); } }
    [SerializeField]
    Transform ability_buttons, global_ability_buttons, sequence_display, shop_display, shop_display_buttons;
    Vector2 GetCharacterCanvasPosition(Character target)
    {
        Vector2 offsetPos = target.transform.position + Vector3.up * 1.5f;

        // Calculate *screen* position (note, not a canvas/recttransform position)
        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

        // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);

        // Set
        return canvasPos;
    }
    public void ShowAbilityButtons(Character target, List<Ability> abilities)
    {
        int i = 0;
        foreach(Ability a in abilities)
        {
            Button b = Instantiate(button_prefab);
            b.transform.SetParent(ability_buttons);
            b.GetComponent<RectTransform>().localPosition = GetCharacterCanvasPosition(target) + Vector2.up * 35f * i++;
            b.gameObject.GetComponentInChildren<Text>().text = a.name;
            b.GetComponent<ButtonDescription>().description = a.name;
            b.onClick.AddListener(delegate {
                a.ApplyAbility(target);
                HideAbilityButtons();
            });
            b.gameObject.AddComponent<SkillButtonHover>().ability = a;
        }
    }

    public void ShowGlobalAbilityButtons(List<Ability> abilities)
    { 
        int i = 0;
        foreach (Ability a in abilities)
        {
            if (a.target_number < 2 && (a.target_type == target_type.enemy || a.target_type == target_type.ally))
            {
                continue;
            }
            Button b = Instantiate(button_prefab);
            b.transform.SetParent(global_ability_buttons);
            RectTransform rt = b.GetComponent<RectTransform>();
            rt.anchorMax = Vector2.up; 
            rt.anchorMin = Vector2.up;
            rt.pivot = Vector2.up;
            rt.localPosition = (Vector2.down * 35f * (++i)) + Vector2.right * 35f;
            b.gameObject.GetComponentInChildren<Text>().text = a.name;
            b.GetComponent<ButtonDescription>().description = a.name;
            b.onClick.AddListener(delegate {
                a.ApplyAbility();
                HideAbilityButtons();
            });
            b.gameObject.AddComponent<SkillButtonHover>().ability = a;
        }
    }
    List<GameObject> sequence_icons = new List<GameObject>();
    public void ShowSequence()
    {
        sequence_icons.ForEach(delegate (GameObject go) { Destroy(go); });
        sequence_icons.Clear();
        List<Character> nts = GM.characters.GetNextTurnSequence();
        int i = 0;
        float step = 40f;
        float start = step * (nts.Count -1) * -.5f;
        foreach (Character c in nts)
        {
            Image icon = Instantiate(sequence_icon_prefab);
            icon.gameObject.AddComponent<SequenceIconHover>().character = c;
            icon.transform.SetParent(sequence_display);
            icon.gameObject.GetComponent<RectTransform>().localPosition = Vector2.right * (i++ * step + start) + Vector2.up * 25f;
            
            icon.color = c.color;
            sequence_icons.Add(icon.gameObject);
        }
    }
    public void HideAbilityButtons()
    {
        ability_buttons.DestroyChildren();
    }
    public void HideGlobalAbilityButtons()
    {
        global_ability_buttons.DestroyChildren();
    }
    
    

    [SerializeField]
    ShopButton shop_button_prefab = null;
    public float shop_button_margin = 250f;
    public void OpenShop(string option_string)
    {
        shop_display.gameObject.SetActive(true);
        option_string = option_string.Trim();
        string[] options = option_string.Split(new char[] { '+' }, System.StringSplitOptions.RemoveEmptyEntries);

        int num = options.Length;
        float step = shop_button_margin;
        float start = step * (num - 1) * -.5f;
        int i = 0;
        foreach(string option in options)
        {
            Vector2 pos = Vector2.right * (step * i++ + start);
            ShopButton sb = Instantiate(shop_button_prefab, shop_display_buttons, false);
            RectTransform rt = sb.GetComponent<RectTransform>();
            rt.localPosition = pos;
            
            ShopItemData sid = ShopItemData.Parse(option);

            sb.text = sid.character.display_name 
                + " +" + sid.value 
                + (sid.ability != null ? " "+sid.ability_name : "") 
                + " " + sid.stat 
                + " for " + sid.price + " " + ShopItemData.resource_name;

            sb.char_icon.color = sid.character.GetComponent<SpriteRenderer>().color;
            sb.price = sid.price;
            sb.button.onClick.AddListener(delegate {
                ShopItemData sidin = ShopItemData.Parse(option);
                Debug.Log(sidin.ToString());
                sidin.Apply();
            });
        }

    }

    private class ShopItemData
    {
        public const string resource_name = "demon essence";
        public string character_name;
        public Character character;
        public string ability_name;
        public Ability ability;
        public string stat;
        public int value;
        public int price;

        static List<string> stat_names = new List<string>() { 
            "maxhp","defense","initiative"
        };

        static List<string> ability_stat_names = new List<string>()
        {
            "damage", "heal", "target_number", "buff"
        };

        public bool Apply()
        {
            if(GM.game.resources < price)
            {
                return false;
            }

            GM.game.resources -= price;

            if(ability != null)
            {
                ability.ModifiyStat(stat, value);
            }
            else
            {
                character.ModifyStat(stat, value);
            }

            return true;
        }

        public static ShopItemData Parse(string data)
        {
            ShopItemData ret = new ShopItemData();

            string[] data_frag = data.Split(new char[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < data_frag.Length; i++)
            {
                data_frag[i] = data_frag[i].Trim();
            }

            ret.character_name = data_frag[0];
            ret.character = GM.party[ret.character_name];
            if (!stat_names.Contains(data_frag[1]))
            {
                ret.ability_name = data_frag[1];
                ret.ability = ret.character.GetAbilityByName(ret.ability_name);
                if(ret.ability == null)
                {
                    throw new UnityException("Non extistent shop item " + ret.ability_name);
                }
                ret.stat = data_frag[2];
                ret.value = int.Parse(data_frag[3]);
                ret.price = int.Parse(data_frag[4].Replace("price", ""));
            }
            else
            {
                ret.stat = data_frag[1];
                ret.value = int.Parse(data_frag[2]);
                ret.price = int.Parse(data_frag[3].Replace("price", ""));
            }

            return ret;
        }
        public override string ToString()
        {
            return "name: " + character_name + ";  " + "; ability_name: " + ability_name + "; stat: " + stat + ";  " + "; value: " + value + ";  " + "; price: " + price + ";  ";
        }
    }

    public void CloseShop()
    {
        shop_display_buttons.DestroyChildren();
        shop_display.gameObject.SetActive(false);
    }
    [SerializeField]
    GameObject description_panel = null;
    public string description { set
        {
            if(value.Length == 0)
            {
                SetDescriptionActive(false);
                return;
            }
            SetDescriptionActive(true);
            Text t = description_panel.GetComponentInChildren<Text>();
            t.text = value;
        }
    }
    Coroutine sda_routine = null;
    bool sda_to;
    void SetDescriptionActive(bool to)
    {
        if(sda_routine != null && !sda_to)
        {
            StopCoroutine(sda_routine);
        }
        sda_to = to;
        sda_routine = StartCoroutine(SetDescriptionActiveStep(to));
    }
    IEnumerator SetDescriptionActiveStep(bool to)
    {
        yield return to ? null : new WaitForSeconds(.5f);
        description_panel.SetActive(to);
    }

    public void Initialize()
    {
        CloseShop();
        description_panel.SetActive(false);
    }
}
