using System.Collections;
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
    Transform ability_buttons, global_ability_buttons, sequence_display;
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
        float start = step * nts.Count * -.5f;
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
    public Character character_hover = null;
    private void LateUpdate()
    {
        character_hover = null;
    }
}
