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
        
    }
    Ability use_this_round;
    List<int> targets;
    public Ability GetRandomAvailableAbility()
    {
        List<Ability> pick = character_abilities.FindAll(delegate (Ability a) {
            return a.from_positions.Contains(position);
        });
        if(pick.Count == 0)
        {
            return null;
        }
        return character_abilities[UnityEngine.Random.Range(0, character_abilities.Count)];
    }
    public override void StartTurn()
    {
        use_this_round = GetRandomAvailableAbility();
        targets = use_this_round == null ? new List<int>() : use_this_round.GetTargetsForAI();
    }
    public override Coroutine StartRound()
    {
        base.StartRound();
        return StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        Debug.Log(name + " starting round ");
        while (!has_finished_acting) {
            if (use_this_round == null)
            {
                Debug.Log(name + " didn't have any ability it could use");
                SpendAP(ap);
            }
            else
            {
                foreach (int target_index in targets)
                {
                    Character c = use_this_round.target_type == target_type.enemy ? opposing_party[target_index] : party[target_index];
                    use_this_round.ApplyAbility(c);
                    
                }
            }
            while (!Input.GetKeyDown(KeyCode.Z))
            {
                yield return null;
            }
            has_finished_acting = true;
            yield return null;
            Debug.Log("has actions left");
        }
        
        Debug.Log(name + " finishing round");
        
    }

}

