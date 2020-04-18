using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Members : MonoBehaviour
{
    protected virtual float[] positions { get { return new float[] { 8.7f, 2.9f, -2.9f, -8.7f }; } }
    public List<Character> members
    {
        get
        {
            return new List<Character>(gameObject.GetComponentsInChildren<Character>());
        }
    }
    public List<Character> alive_members
    {
        get
        {
            return members.FindAll(delegate(Character c){ return c.alive; });

        }
    }
    public List<Character> dead_members
    {
        get
        {
            return members.FindAll(delegate (Character c) { return !c.alive; });

        }
    }
    public Dictionary<int, Character> members_positions
    {
        get
        {
            Dictionary<int, Character> ret = new Dictionary<int, Character>();
            foreach(Character c in members)
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
    List<Character> died_this_round = new List<Character>();
    public void OnCharacterDeath(Character c)
    {
        died_this_round.Add(c);
    }
    public void RemoveDead()
    {
        List<Character> living_order = alive_members;
        living_order.Sort(delegate (Character a, Character b)
        {
            return a.position.CompareTo(b.position);
        });
        List<Character> dead_order = dead_members;
        dead_order.Sort(delegate (Character a, Character b)
        {
            return a.position.CompareTo(b.position);
        });

        int i = 0;
        living_order.ForEach(delegate (Character c)
        {
            if(c.position != i)
            {
                c.GoToPosition(i);
            }
            i++;
        });
        dead_order.ForEach(delegate (Character c)
        {
            if (c.position != i)
            {
                c.GoToPosition(i);
            }
            i++;
        });
    }
}
