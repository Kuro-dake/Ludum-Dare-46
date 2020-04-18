using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvEvent : MonoBehaviour
{
    bool activated = false;
    public void ForceActivated()
    {
        activated = true;
    }
    protected virtual void Update()
    {
        if (activated)
        {
            return;
        }
        if(Mathf.Abs(transform.position.x - GM.party.transform.position.x) < 5f)
        {
            activated = true;
            PlayEvent();
        }
    }

    protected abstract void PlayEvent();
}
