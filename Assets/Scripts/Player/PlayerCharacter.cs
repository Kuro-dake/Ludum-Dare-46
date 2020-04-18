using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
   
    
    int _hp = 10;
    public int hp { get { return _hp; } set { _hp = value; } }
    

    public virtual void Initialize()
    {
        
    }

    public virtual void Hit(int damage)
    {
        
        hp -= damage;


    }
    public virtual void Movement(direction d) { }

    protected Animator anim { get { return GetComponent<Animator>(); } }
}
