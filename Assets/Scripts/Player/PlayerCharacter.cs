using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public override void Interact(Character c)
    {
        if(c == this)
        {

        }
        else if(c is PlayerCharacter)
        {
            SwitchPositions(c);
            SpendAP();
        }
        else if(c is Enemy)
        {
            c.Hit(1);
            SpendAP();
        }
        
    }
    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Hit(int damage)
    {

        base.Hit(damage);
        GM.game.UpdateDevout(); 

    }
    public virtual void Movement(direction d) {
    }

    protected Animator anim { get { return GetComponent<Animator>(); } }
}
