using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    public IntRange resources;
    public EnemyControl enemy_control { get { return control as EnemyControl; } }
    public override Coroutine GoToPosition(int pos = -1, bool instant = false)
    {
        Coroutine ret = base.GoToPosition(pos, instant);
        enemy_control.PrepareRandomAction(true);
        return ret;
    }
    public override void Hit(int damage)
    {
        
        base.Hit(damage);
        
    }
    protected override void OnDeath()
    {
        GM.game.GeneratePickup(transform.position + Vector3.up * 4f, resources.random);
    }
    
   
    
    public override void StartTurn()
    {
        enemy_control.PrepareRandomAction();
        
    }
    
    public override Coroutine StartRound()
    {
        return base.StartRound();
        
    }

    
    bool showing_next_targets = false;
    protected override void OnMouseOver()
    {
        
        base.OnMouseOver();
        if (!showing_next_targets) {
            showing_next_targets = true;
            enemy_control.DisplayNextTargets();
        }
        
        
    }

    protected override void OnMouseExit()
    {
        if (showing_next_targets)
        {
            showing_next_targets = false;
            GM.characters.MarkTargets();
        }
        base.OnMouseExit();
    }

    public override void Initialize()
    {
        if (control == null)
        {
            gameObject.AddComponent<EnemyControl>();
        }
        base.Initialize();
        
    }

}

