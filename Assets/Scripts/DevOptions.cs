using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DevOptions
{
    public bool hide_dev_text_output = false;
    public bool dev_controls = true;
    public static bool Key(KeyCode k)
    {
        if (!GM.dev_options.dev_controls)
        {
            return false;
        }
        return Input.GetKeyDown(k);
    }
    public void Init()
    {
    }  
    public void Update()
    {

    }
}

public enum direction
{
    none = 0,
    left = 1,
    right = -1,
    up = 2,
    down = -2

}