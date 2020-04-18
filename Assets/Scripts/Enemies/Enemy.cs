using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    public override void Interact(Character c)
    {
        if(c is PlayerCharacter)
        {
            c.Hit(1);
            SpendAP();
        }
        else
        {
            SpendAP(ap);
        }
    }
    public override void Hit(int damage)
    {
        base.Hit(damage);
        if (!alive)
        {
            Destroy(gameObject);
        }
    }
    Ability use_this_round;
    List<int> targets;
    public Ability GetRandomAbility()
    {
        return abilities[UnityEngine.Random.Range(0, abilities.Count)];
    }
    public override void StartTurn()
    {
        use_this_round = GetRandomAbility();
        targets = use_this_round.GetTargetsForAI();
    }
    public override void StartRound()
    {
        base.StartRound();
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        Debug.Log(name + " starting round ");
        while (has_actions_left) { 
            yield return new WaitForSeconds(.7f);
            Interact(GM.party["Warrior"]);
        }
        yield return new WaitForSeconds(.7f);
        Debug.Log(name + " finishing round");
        
    }

}

