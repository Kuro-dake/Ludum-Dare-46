using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public override List<Ability> GetAvailableAbilitiesFor(Character c)
    {
        target_type type = GetTargetType(c);
        List<Ability> ret =  base.GetAvailableAbilitiesFor(c);
        if (target_type.self == type || target_type.ally == type)
        {
            if (target_type.ally == type)
            {
                Ability switch_ability = new Ability(delegate { SwitchPositions(c); });
                switch_ability.name = "Switch positions with " + c.name;
                switch_ability.owner = this;
                ret.Add(switch_ability);
            }
            return ret;
        }
        return ret;
    }
    public override Coroutine StartRound()
    {
        GM.ui.ShowGlobalAbilityButtons(character_abilities);
        return base.StartRound();
    }
    public override void Interact(Character c)
    {
        // @TODO: make a proxy function that either displays the list of abilities you can use,
        // or uses the only ability available right away
        List<Ability> possible_abilities = GetAvailableAbilitiesFor(c);
        Ability single_ability = possible_abilities.Count == 1 ? possible_abilities[0] : null;
        if(c == this)
        {
            if(single_ability != null) {
                single_ability.ApplyAbility(c);
            }
            
        }
        else if(c is PlayerCharacter)
        {
            // startcoroutine to display switching option, or cancel.
            // add possible abilities
            //SwitchPositions(c);
            if(single_ability != null)
            {
                single_ability.ApplyAbility(c);
            }
            
        }
        else if(c is Enemy)
        {
            if(single_ability != null)
            {
                single_ability.ApplyAbility(c);
            }
            
        }
        has_finished_acting = true;
    }
    public override void EndRound()
    {
        GM.ui.HideGlobalAbilityButtons();
        base.EndRound();
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
