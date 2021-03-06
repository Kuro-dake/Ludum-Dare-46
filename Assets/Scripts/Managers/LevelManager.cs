﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    protected class EnvItemLevelOccurenceData
    {
        public string type;
        public FloatRange distance_range = new FloatRange(5f,20f);
        public FloatRange scale = new FloatRange(.85f, 1.15f);
        public FloatRange euler_rotation = new FloatRange(-10f, 10f);
        public bool flips = true;
        
    }
    [SerializeField]
    List<EnvItemLevelOccurenceData> occurence_data = new List<EnvItemLevelOccurenceData>();
    public void GenerateLevel(string lands)
    {
        float dist;
        foreach (EnvItemLevelOccurenceData data in occurence_data)
        {
            float clear_dist = 20f;
            dist = -20;
            while (dist < 3000)
            {
                dist += data.distance_range.random + 10f;
                if (dist < clear_dist && dist > -clear_dist)
                {
                    continue;
                }
                Generate(lands, data, dist);
            }
            
        }
       
    }
    GameObject Generate(string section, EnvItemLevelOccurenceData type_data, float offset)
    {
        GameObject go = Instantiate(GetRandomPrefab(section, type_data.type));
        Parallax p = go.GetComponent<Parallax>();
        if (p == null)
        {
            p = go.AddComponent<Parallax>();
        }
        p.mode = parallax_mode.item;
        p.offset = offset;
        go.transform.SetParent(GM.game.current_scenery.env_parallax);
        go.transform.localScale = Random.Range(0, 2) == 1 && type_data.flips ? go.transform.localScale : new Vector3(go.transform.localScale.x * -1, go.transform.localScale.y, go.transform.localScale.z);
        go.transform.localScale *= type_data.scale.random;
        go.transform.localRotation = Quaternion.Euler(Vector3.forward * type_data.euler_rotation);
        go.transform.localPosition += Vector3.up * Random.Range(0f, .5f);
        return go;
    }
    public GameObject SpawnObject(string param_string, float offset = 40f)
    {
        string[] pars = param_string.Split(new char[] { ';' });
        string obj_name = pars[0];
        GameObject go = Instantiate(Resources.Load("EnvItems/Global/" + obj_name) as GameObject);
        Parallax p = go.GetComponent<Parallax>();
        if (p == null)
        {
            p = go.AddComponent<Parallax>();
            p.mode = parallax_mode.item;
            p.offset = GM.scenery.x *-1 + offset;
        }
        EnvEvent ee = go.GetComponent<EnvEvent>();
        if(ee != null)
        {
            if (pars.Length > 1)
            {
                ee.data = pars[1];
            }
            if (pars.Length > 2)
            {
                ee.dialogue = pars[2];
            }
            ee.Initialize();
        }
        go.transform.localPosition = Vector3.zero;
        go.transform.SetParent(GM.game.current_scenery.env_parallax);

        return go;
    }
    public GameObject GetRandomPrefab(string environment, string type)
    {
        Object[] env_items = Resources.LoadAll(string.Format("EnvItems/{0}/{1}", environment, type), typeof(GameObject));
        if (env_items.Length == 0)
        {
            throw new UnityException("Environment/Type not found (" + environment + "/" + type + ")");
        }
        return (GameObject)env_items[Random.Range(0, env_items.Length)];
    }
}