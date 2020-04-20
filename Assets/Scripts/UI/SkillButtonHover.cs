using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Ability ability;
    public void OnPointerEnter(PointerEventData eventData)
    {
        GM.characters.MarkTargets(ability.TargetCharacters());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GM.characters.MarkTargets();
    }

    private void OnDestroy()
    {
        GM.characters.MarkTargets();
    }
}
