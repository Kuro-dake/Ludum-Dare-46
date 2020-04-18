﻿using System.Collections;
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
        LoadXTags();
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
    void LoadXTags()
    {
        xtags.Clear();
        foreach(string s in Resources.Load<TextAsset>("xtags").text.Split(new char[] { '\n' }))
        {
            string[] p = s.Split(new char[] { '-' });
            int x = int.Parse(p[0]) * -1;
            xtags.Add(x, p[1]);
        }
    }
    Dictionary<int, string> xtags = new Dictionary<int, string>();
    public void DeleteSkippedXtags()
    {
        CheckXTags(true);
    }
    void CheckXTags(bool delete_only = false)
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
                if (!delete_only)
                {
                    StartCoroutine(PlayXTag(xtags[i]));
                }
                xtags.Remove(i);
            }
            
        }
    }

    IEnumerator PlayXTag(string xtag)
    {
        string[] batch = xtag.Split(new char[] { '|' });
        foreach (string xt in batch)
        {
            Debug.Log(xt);
            string[] pars = xt.Split(new char[] { ':' });
            pars[1] = pars[1].Trim();
            switch (pars[0])
            {
                case "encounter":
                    yield return GM.characters.CreateEncounterFromString(pars[1]);
                    break;
                case "dialogue":
                    yield return GM.cinema.PlayDialogue(pars[1]);
                    break;
                case "env":
                    GM.level_manager.SpawnObject(pars[1]);
                    break;
            }
        }

        Debug.Log("Finished xtag");
    }
    
    void Update()
    {
        
        UpdateParallax();
        CheckXTags();
        last_x = x;        
    }

}

