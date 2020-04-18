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
            if(target_type.self == type)
            {
                Ability skip_ability = new Ability(delegate { SkipTurn(); });
                skip_ability.name = "Skip turn";
                skip_ability.owner = this;
                ret.Add(skip_ability);
            }
            return ret;
        }
        return ret;
    }
    public void SkipTurn()
    {
        has_finished_acting = true;
    }
    public override Coroutine StartRound()
    {
        GM.ui.ShowGlobalAbilityButtons(character_abilities);
        return base.StartRound();
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
