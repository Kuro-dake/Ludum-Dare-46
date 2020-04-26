using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SequenceIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    static GameObject sequence_mark;
    public Character character;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (character is Enemy)
        {
            (character as Enemy).enemy_control.DisplayNextTargets();
        }
        SetMark(character);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GM.characters.MarkTargets();
        SetMark();
    }

    void SetMark(Character c = null)
    {
        if (c == null)
        {
            sequence_mark.SetActive(false);
            return;
        }
        if (sequence_mark == null)
        {
            sequence_mark = GM.characters.CreateMark(c.transform, new Color(.6f, .6f, .9f));
        }
        
        sequence_mark.SetActive(true);
        sequence_mark.transform.SetParent(c.transform);
        sequence_mark.transform.localPosition = Vector2.up * CharacterManager.ARROW_UP_MULTIPLIER / c.transform.localScale.y;
    }
}
