using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : CharacterControl
{
    Ability use_this_round;
    List<int> targets;

    Enemy enemy { get { return character as Enemy; } }

    public void PrepareRandomAction(bool force = false)
    {
        if (use_this_round == null || force)
        {
            use_this_round = character.GetRandomAvailableAbility();
            targets = use_this_round == null ? new List<int>() : use_this_round.GetTargetPositionsForAI();
        }
    }


    public override void StartTurn()
    {
        base.StartTurn();
        PrepareRandomAction();
    }

    IEnumerator UsePreparedAbilities()
    {
        Debug.Log(name + " starting round ");
        while (!has_finished_acting)
        {
            if (use_this_round == null)
            {
                Debug.Log(name + " didn't have any ability it could use. Trying to refresh.");
                PrepareRandomAction();

            }
            if (use_this_round == null)
            {
                Debug.Log(name + " still no action, spending APs.");
                character.SpendAP(character.ap);
            }
            else
            {
                if (use_this_round.target_type == target_type.self)
                {
                    use_this_round.ApplyAbility(character);
                }
                else
                {

                    foreach (int target_index in targets)
                    {
                        Debug.Log(target_index);
                        Character c = use_this_round.target_type == target_type.enemy ? character.opposing_party[target_index] : character.party[target_index];
                        use_this_round.ApplyAbility(c);

                    }
                }
                PrepareRandomAction(true);
            }

            has_finished_acting = true;
            yield return null;

        }

        Debug.Log(name + " finishing round");

    }

    public void DisplayNextTargets()
    {
        if (use_this_round != null && GM.characters.show_enemy_targets)
        {
            GM.characters.MarkTargets(use_this_round.TargetCharacters(targets));
        }
    }

    public override Coroutine StartRound()
    {
        return StartCoroutine(UsePreparedAbilities());
    }

}
