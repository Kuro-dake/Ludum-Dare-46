using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIControls : MonoBehaviour
{
    List<NamedSprite> icons { get { return GetComponentInChildren<IconContainer>().icons; } }
    public Sprite GetIcon(string icon_name)
    {
        try
        {
            return icons.Find(delegate (NamedSprite ns) { return ns.first == icon_name; }).second;
        }
        catch
        {
            Debug.Log("Icon named " + icon_name + " does not exist");
            return null;
        }
    }
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
    Character showing_abilities_for;
    public void ShowAbilityButtons(Character target)
    {

        if (!GM.can_show_ability_buttons)
        {
            return;
        }

        if (target == showing_abilities_for)
        {
            return;
        }

        if(!(GM.characters.current_round_character is PlayerCharacter))
        {
            return;
        }

        showing_abilities_for = target;


        List<Ability> abilities = GM.characters.current_round_character.GetAvailableAbilitiesFor(target);

        int i = 0;
        float step = 65f;
        float start = step * (abilities.Count - 1) * -.5f;



        foreach (Ability a in abilities)
        {
            Button b = Instantiate(button_prefab);
            b.transform.SetParent(ability_buttons);
            b.GetComponent<RectTransform>().localPosition = GetCharacterCanvasPosition(target) + Vector2.right * (i++ * step + start) + Vector2.down * 2f;

            b.GetComponent<ButtonDescription>().description = a.ToString();
            Image img = b.transform.Find("Image").GetComponent<Image>();
            img.sprite = GetIcon(a.sprite_name);

            b.onClick.AddListener(delegate {
                a.ApplyAbility(target);
                HideAbilityButtons(showing_abilities_for);
            });
            b.gameObject.AddComponent<SkillButtonHover>().ability = a;
        }
    }

    public void HideAbilityButtons(Character c)
    {
        
        if(showing_abilities_for != c) {
            return;
        }
        foreach(ButtonDescription bd in ability_buttons.GetComponentsInChildren<ButtonDescription>())
        {
            info_panel.HideButtonDescription(bd);
        }

        showing_abilities_for = null;
        ability_buttons.DestroyChildren();
    }
    public InfoPanel info_panel;
    public void HideGlobalAbilityButtons()
    { 
        global_ability_buttons.DestroyChildren();
    }
    public void ShowGlobalAbilityButtons(List<Ability> abilities)
    {
        int i = 0;
        foreach (Ability a in abilities)
        {
            if (a.target_number < 2 && (a.target_type == target_type.enemy || a.target_type == target_type.ally || a.target_type == target_type.self))
            {
                continue;
            }
            Button b = Instantiate(button_prefab);
            b.transform.SetParent(global_ability_buttons);
            RectTransform rt = b.GetComponent<RectTransform>();
            rt.anchorMax = Vector2.up;
            rt.anchorMin = Vector2.up;
            rt.pivot = Vector2.up;
            rt.localPosition = (Vector2.down * 65 * (++i)) + Vector2.right * 35;

            b.GetComponent<ButtonDescription>().description = a.ToString();
            b.transform.Find("Image").GetComponent<Image>().sprite = GetIcon(a.sprite_name);
            b.onClick.AddListener(delegate {
                a.ApplyAbility();
                HideGlobalAbilityButtons();
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
        float step = 70f;
        float start = step * (nts.Count - 1) * -.5f;
        foreach (Character c in nts)
        {
            Image iicon = Instantiate(sequence_icon_prefab);
            Image icon = iicon.transform.Find("Image").GetComponent<Image>();
            iicon.gameObject.AddComponent<SequenceIconHover>().character = c;
            iicon.transform.SetParent(sequence_display);
            iicon.gameObject.GetComponent<RectTransform>().localPosition = Vector2.right * (i++ * step + start) + Vector2.up * 25f;
            icon.sprite = GetIcon(c.icon_name);
            sequence_icons.Add(iicon.gameObject);
        }
    }
    public void HideSequence()
    {
        sequence_icons.ForEach(delegate (GameObject go) { Destroy(go); });
        sequence_icons.Clear();
    }
   



    [SerializeField]
    ShopButton shop_button_prefab = null;
    public float shop_button_margin = 250f;
    public bool shop_open = false;
    public string current_shop_string;
    public void OpenShop(string option_string)
    {
        current_shop_string = option_string;
        shop_open = true;
        shop_display.gameObject.SetActive(true);
        option_string = option_string.Trim();
        string[] options = option_string.Split(new char[] { '+' }, System.StringSplitOptions.RemoveEmptyEntries);

        int num = options.Length;
        float step = shop_button_margin;
        float start = step * (num - 1) * -.5f;
        int i = 0;
        foreach (string option in options)
        {
            Vector2 pos = Vector2.right * (step * i++ + start);
            ShopButton sb = Instantiate(shop_button_prefab, shop_display_buttons, false);

            RectTransform rt = sb.GetComponent<RectTransform>();
            rt.localPosition = pos;

            ShopItemData sid = ShopItemData.Parse(option);
            sb.button_image.sprite = GetIcon(sid.ability != null ? sid.ability.sprite_name : sid.stat);
            sb.text = sid.ToString();

            sb.character = sid.character;

            sb.char_icon.sprite = GetIcon(sid.character.icon_name);

            sb.price = sid.price;
            if (sid.price <= GM.game.resources)
            {
                sb.button.onClick.AddListener(delegate
                {
                    ShopItemData sidin = ShopItemData.Parse(option);
                    Debug.Log(sidin.ToString());
                    sidin.Apply();
                    sb.text = sidin.ToString();

                    info_panel.HideButtonDescription(sb.bd_script);
                    info_panel.ShowButtonDescription(sb.bd_script);

                    current_shop_string = current_shop_string.Replace(sidin.data_string, "");
                    CloseShop();
                    OpenShop(current_shop_string);
                });
            }
            else
            {
                foreach(Image img in sb.GetComponentsInChildren<Image>())
                {
                    img.color = new Color(.9f, .7f, .7f);
                }
            }
        }

    }

    private class ShopItemData
    {
        public string data_string;
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
            if (GM.game.resources < price)
            {
                return false;
            }

            GM.game.resources -= price;

            if (ability != null)
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

            for (int i = 0; i < data_frag.Length; i++)
            {
                data_frag[i] = data_frag[i].Trim();
            }

            ret.character_name = data_frag[0];
            ret.character = GM.party[ret.character_name];
            ret.character.ToString();
            if (!stat_names.Contains(data_frag[1]))
            {
                ret.ability_name = data_frag[1];
                ret.ability = ret.character.GetAbilityByName(ret.ability_name);
                if (ret.ability == null)
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
            ret.data_string = data;
            
            return ret;
        }
        public override string ToString()
        {
            return character.display_name
                + (ability != null ? "\n\n" + ability.ToString() : "")
                + "\n\n +" + value + " " + stat + (ability != null ? " for this ability" : "");
        }
    }

    public void CloseShop()
    {
        shop_display_buttons.DestroyChildren();
        shop_display.gameObject.SetActive(false);
        shop_open = false;
        GM.game.combat_ended = Time.time;
    }
    

    public void Initialize()
    {
        CloseShop();
        info_panel.Initialize();
    }


    public void GameOver()
    {
        GM.game.game_over = true;
        gameover_panel.gameObject.SetActive(true);
        StartCoroutine(GameOverStep());
    }
    [SerializeField]
    Transform gameover_panel;
    Image go_image { get { return gameover_panel.GetComponent<Image>(); } }
    Text go_text { get { return gameover_panel.GetComponentInChildren<Text>(); } }
    IEnumerator GameOverStep()
    {
        while(go_image.color.a < 1f)
        {
            go_image.color += Color.black * Time.deltaTime;
            go_text.color = Color.white - Color.black * (1f - go_image.color.a);
            yield return null;
        }
        Destroy(GM.scenery.gameObject);
        Destroy(GM.game.current_enemy_party);
        yield return new WaitForSeconds(3f);
        while (go_text.color.a > 0f)
        {
            go_text.color -= Color.black * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        
        GameContainer.ReloadGame();
    }
    
}
