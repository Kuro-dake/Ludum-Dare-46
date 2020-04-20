using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvEvent : MonoBehaviour
{
    bool activated = false;
    public string data;
    public string dialogue = "";
    public virtual void Initialize()
    {

    }
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
            StartCoroutine(DialogueAndEvent());
        }
    }

    IEnumerator DialogueAndEvent()
    {
        if (dialogue.Length > 0)
        {
            yield return GM.cinema.PlayDialogue(dialogue);
        }
        GM.game.combat_ended = Time.time;
        PlayEvent();
    }

    protected virtual void PlayEvent()
    {
        
    }
}
