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
    
    private void Update()
    {
        transform.localScale += Vector3.one * Time.deltaTime * 7f;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.color -= Color.black * Time.deltaTime * 3f;
        if(sr.color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public static void DoFlash(GameObject go)
    {
        GameObject ggo = Instantiate(go);
        ggo.transform.position = go.transform.position;
        ggo.GetComponentInChildren<SpriteRenderer>().sortingOrder -= 1;
        ggo.GetComponentInChildren<SpriteRenderer>().color -= Color.black * .2f;
        ggo.AddComponent<Flash>().Strip();
    }
}
