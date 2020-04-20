using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    protected Animator anim
    {
        get
        {
            return GetComponent<Animator>();
        }
    }
    Coroutine fade_routine;
    void FadeOne(IEnumerator step)
    {
        if(fade_routine != null)
        {
            StopCoroutine(fade_routine);
        }
        fade_routine = StartCoroutine(step);
    }
    public void FadeOut(bool destroy_after_fade = false)
    {
        FadeOne(FadeOutStep(true));
    }
    public void FadeIn()
    {
        FadeOne(FadeOutStep(false));
    }
    IEnumerator FadeOutStep(bool fade_out, float speed = 1f, bool destroy = false) {

        List<SpriteRenderer> srs = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        srs.Add(GetComponent<SpriteRenderer>());
        if(srs.Count == 0)
        {
            yield break;
        }

        float target = fade_out ? 0f : 1f;
        float a = Mathf.Clamp(srs[0].color.a, 0f, 1f);
        while(!Mathf.Approximately(a, target))
        {
            srs.RemoveAll(delegate (SpriteRenderer sr) { return sr == null; });
            a = Mathf.MoveTowards(a, target, Time.deltaTime);
            Color c = srs[0].color;
            c.a = a;
            srs.ForEach(delegate (SpriteRenderer sr)
            {
                sr.color = c;
            });
            yield return null;
        }
        Destroy(this);
    }
      

}
