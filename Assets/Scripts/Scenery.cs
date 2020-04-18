using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class Scenery : MonoBehaviour
{
    public float x;
    public float y_parallax_multiplier;
    float last_x;
    
    public Transform enemies_parallax { get { return transform.Find("enemies_parallax"); } }

    

    public void Initialize()
    {
        CheckActive();
        last_x = x;
    }

    public void Movement(float _x)
    {
        
        x = -_x;
        
        transform.position = new Vector2(-x, transform.position.y);
        CheckActive();
        
        
        //transform.position = new Vector2(x, transform.position.y);
    }
    void CheckActive()
    {
        foreach (Parallax p in GetComponentsInChildren<Parallax>(true))
        {
            p.x = x;
            p.CheckActive();
        }
    }
    void UpdateParallax()
    {
        foreach (Parallax p in GetComponentsInChildren<Parallax>(true))
        {
            p.x = x;
            p.UpdateParallax();
            
        }
    }

    Dictionary<int, string> xtags = new Dictionary<int, string>() {
        { -65, "encounter:goblin1;goblin1;goblin1;goblin1" },
        { -100, "encounter:goblin2;goblin2;goblin2;goblin2"}
    };

    void CheckXTags()
    {
        int from = Mathf.FloorToInt(last_x);
        int to = Mathf.FloorToInt(x);
        if(from > to)
        {
            int swap = to;
            to = from;
            from = swap;
        }
        for (int i = from; i<to; i++)
        {
            if (xtags.ContainsKey(i))
            {
                PlayXTag(xtags[i]);
                xtags.Remove(i);
            }
            
        }
    }

    void PlayXTag(string xtag)
    {
        string[] pars = xtag.Split(new char[] { ':' });
        switch (pars[0])
        {
            case "encounter":
                GM.characters.CreateEncounterFromString(pars[1]);
                break;
        }
        
    }
    
    void Update()
    {
        
        UpdateParallax();
        CheckXTags();
        last_x = x;        
    }

}

