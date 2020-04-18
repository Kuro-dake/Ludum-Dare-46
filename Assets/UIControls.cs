using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIControls : MonoBehaviour
{
    [SerializeField]
    Button button_prefab = null;
    Canvas canvas { get { return GetComponent<Canvas>(); } }
    [SerializeField]
    Transform ability_buttons;
    [SerializeField]
    Transform global_ability_buttons;
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
}
