using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberPositions : MonoBehaviour
{
    protected virtual float[] positions { get { return new float[] { 8.7f, 2.9f, -2.9f, -8.7f }; } }
       
    public Dictionary<int, Character> members_positions
    {
        get
        {
            Dictionary<int, Character> ret = new Dictionary<int, Character>();
            foreach(Character c in gameObject.GetComponentsInChildren<Character>())
            {
                ret.Add(c.position, c);
            }
            return ret;
        }
    }
    public void InitCharacters(Party party)
    {
        foreach(Character c in gameObject.GetComponentsInChildren<Character>())
        {
            c.party = party;
            c.GoToPosition();
            c.Initialize();
        }

    }

    public float GetPositionVector(int v)
    {
        return positions[v];
    }
}
