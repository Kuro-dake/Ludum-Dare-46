using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    
    void Strip()
    {
        foreach(MonoBehaviour mono in GetComponents<MonoBehaviour>())
        {
            if(mono == this)
            {
                continue;
            }
            Destroy(mono);
        }
        foreach (MonoBehaviour mono in GetComponentsInChildren<MonoBehaviour>())
        {
            if (mono == this)
            {
                continue;
            }
            Destroy(mono);
        }
        Transform tm = transform.Find("turn marker");
        if(tm != null)
        {
            Destroy(tm.gameObject);
        }
        Animator anim = GetComponent<Animator>();
        if(anim != null)
        {
            Destroy(anim);
        }
    }
    float scale_speed = 7f;
    float fade_speed = 3f;
    private void Update()
    {
        Vector3 basev = Vector3.one;
        basev.x *= Mathf.Sign(transform.localScale.x);
        transform.localScale += basev * Time.deltaTime * scale_speed;
        float min_a = 1f;
        foreach(SpriteRenderer sr in srs)
        {
            sr.color -= Color.black * Time.deltaTime * fade_speed;
            if (sr.color.a < min_a)
            {
                min_a = sr.color.a;
            }
        }
        if(min_a <= 0f)
        {
            Destroy(gameObject);
        }
        
    }
    List<SpriteRenderer> srs = new List<SpriteRenderer>();
    public static void DoFlash(GameObject go, float scale_speed = 7f, float fade_speed = 3f)
    {

        GameObject ggo = Instantiate(go);
        ggo.transform.position = go.transform.position;

        List<SpriteRenderer> _srs = new List<SpriteRenderer>(ggo.GetComponentsInChildren<SpriteRenderer>());
        if(ggo.GetComponent<SpriteRenderer>() != null)
        {
            _srs.Add(ggo.GetComponent<SpriteRenderer>());
        }
        
        foreach (SpriteRenderer sr in _srs)
        {
            sr.sortingOrder -= 1;
            sr.color -= Color.black * .2f;
        }

        Flash f = ggo.AddComponent<Flash>();
        f.srs = _srs;
        f.scale_speed = scale_speed;
        f.fade_speed = fade_speed;
        f.Strip();
    }
}
