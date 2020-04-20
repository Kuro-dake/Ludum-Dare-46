using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    public IntRange resources;
    public override Coroutine GoToPosition(int pos = -1, bool instant = false)
    {
        PrepareRandomAction(true);
        return base.GoToPosition(pos, instant);
    }
    public override void Hit(int damage)
    {
        
        base.Hit(damage);
        
    }
    protected override void OnDeath()
    {
        GM.game.GeneratePickup(transform.position + Vector3.up * 4f, resources.random);
    }
    Ability use_this_round;
    List<int> targets;
    public Ability GetRandomAvailableAbility()
    {
        List<Ability> pick = character_abilities.FindAll(delegate (Ability a) {
            return a.from_positions_bypass.Contains(position);
        });
        if(pick.Count == 0)
        {
            return null;
        }
        return character_abilities[UnityEngine.Random.Range(0, character_abilities.Count)];
    }
    
    public override void StartTurn()
    {
        PrepareRandomAction();
        
    }
    public void PrepareRandomAction(bool force = false)
    {
        if (use_this_round == null || force)
        {
            use_this_round = GetRandomAvailableAbility();
            targets = use_this_round == null ? new List<int>() : use_this_round.GetTargetPositionsForAI();
        }
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
                Debug.Log(name + " didn't have any ability it could use. Trying to refresh.");
                PrepareRandomAction();
                
            }
            if (use_this_round == null)
            {
                Debug.Log(name + " still no action, spending APs.");
                SpendAP(ap);
            }
            else
            {
                foreach (int target_index in targets)
                {
                    Character c = use_this_round.target_type == target_type.enemy ? opposing_party[target_index] : party[target_index];
                    use_this_round.ApplyAbility(c);
                    
                }
                PrepareRandomAction(true);
            }
            
            has_finished_acting = true;
            yield return null;
            
        }
        
        Debug.Log(name + " finishing round");
        
    }
    bool showing_next_targets = false;
    protected override void OnMouseOver()
    {
        
        base.OnMouseOver();
        if (!showing_next_targets) {
            showing_next_targets = true;
            DisplayNextTargets();
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

    public void DisplayNextTargets()
    {
        if (use_this_round != null && GM.characters.show_enemy_targets)
        {
            GM.characters.MarkTargets(use_this_round.TargetCharacters(targets));
        }
    }
}

