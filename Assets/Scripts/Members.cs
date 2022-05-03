using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Members : MonoBehaviour
{
    protected virtual float[] positions { 
        get {
            float margin = 22.4f / members.Count;
            int numpos = members.Count;
            float start = margin * (numpos - 1) / 2f;
            List<float> ret = new List<float>();
            for(int i = 0; i < numpos; i++)
            {
                ret.Add(start - margin * i);
                Debug.Log(start - margin * i);
            }
            return ret.ToArray();
            //return new float[] { 8.7f, 2.9f, -2.9f, -8.7f }; 
        } 
    }
    
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
            return members.FindAll(delegate(Character c){ return c.is_alive; });

        }
    }
    public List<Character> dead_members
    {
        get
        {
            return members.FindAll(delegate (Character c) { return !c.is_alive; });

        }
    }
    public Character this[int n]
    {
        get
        {
            if (n < 0)
            {
                n += members_positions.Count;
            }
            if (!members_positions.ContainsKey(n))
            {
                return null;
            }
            return members_positions[n];
        }
    }
    Dictionary<int, Character> members_positions
    {
        get
        {
            Dictionary<int, Character> ret = new Dictionary<int, Character>();
            foreach(Character c in alive_members)
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
            c.Initialize();
            c.GoToPosition();
            
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
