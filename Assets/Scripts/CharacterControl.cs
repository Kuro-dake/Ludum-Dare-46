using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterControl : MonoBehaviour
{
    public abstract Coroutine StartRound();

    public bool has_finished_acting = false;

    protected Character character { get { return GetComponent<Character>(); } }
    public virtual void StartTurn()
    {
    
    }

    

    

    

}
