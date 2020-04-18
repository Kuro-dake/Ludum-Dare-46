using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    protected bool moving = false;

    public Character[] characters;
    protected bool orientation_right { get { return Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x; } }

    public float GetPositionVector(int v)
    {
        return member_positions.GetPositionVector(v);
    }
    [SerializeField]
    MemberPositions _member_positions = null;
    public MemberPositions member_positions { get { return _member_positions != null ? _member_positions : (_member_positions = gameObject.GetComponentInChildren<MemberPositions>()); } }

    public virtual void Initialize()
    {
        member_positions.InitCharacters(this);
    }

    public Character this[string n]
    {
        get
        {
            return member_positions.transform.Find(n).GetComponent<Character>();
        }
    }
    public Character this[int n]
    {
        get
        {
            return member_positions.members_positions[n];
        }
    }
}
