using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    protected bool moving = false;

    public Character[] characters;
    protected bool orientation_right { get { return Camera.main == null ? false : Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x; } }

    public float GetPositionVector(int v)
    {
        return members.GetPositionVector(v);
    }
    [SerializeField]
    Members _members = null;
    public Members members { get { return _members != null ? _members : (_members = gameObject.GetComponentInChildren<Members>()); } }

    public virtual void Initialize()
    {
        members.InitCharacters(this);
    }

    public Character this[string n]
    {
        get
        {
            return members.transform.Find(n).GetComponent<Character>();
        }
    }
    public Character this[int n]
    {
        get
        {
            return members[n];
        }
    }
    
}
