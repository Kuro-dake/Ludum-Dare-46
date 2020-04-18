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
    
    void Update()
    {
        
        UpdateParallax();
        
    }

}

