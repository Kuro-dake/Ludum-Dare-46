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
            (character as Enemy).DisplayNextTargets();
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
        if(sequence_mark == null)
        {
            sequence_mark = new GameObject("sequence mark");
            SpriteRenderer sr = sequence_mark.AddComponent<SpriteRenderer>();
            sr.sprite = GM.square;
            sr.color = Color.yellow;
        }
        if(c == null)
        {
            sequence_mark.SetActive(false);
            return;
        }
        sequence_mark.SetActive(true);
        sequence_mark.transform.SetParent(c.transform);
        sequence_mark.transform.localPosition = Vector2.up * 8f / c.transform.localScale.y;
    }
}
