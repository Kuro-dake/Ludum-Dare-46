using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    public override List<Ability> GetAvailableAbilitiesFor(Character c)
    {
        bool combat = GM.game.phase != game_phase.movement;
        target_type type = GetTargetType(c);
        List<Ability> ret =  combat ? base.GetAvailableAbilitiesFor(c) : new List<Ability>();
        if (target_type.self == type || target_type.ally == type)
        {
            if (target_type.ally == type)
            {
                Ability switch_ability = new Ability(delegate { SwitchPositions(c); });
                switch_ability.name = "Switch positions with " + c.name;
                switch_ability.owner = this;
                ret.Add(switch_ability);
                if (!combat)
                {
                    Ability select_ability = new Ability(delegate { GM.characters.SetCurrentCharacter(c); });
                    select_ability.name = "Set active character to " + c.name;
                    select_ability.owner = this;
                    ret.Add(select_ability);
                }
            }
            if(target_type.self == type && combat)
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
        if(character_abilities.Count == 0)
        {
            Debug.Log(name + " skips the turn bacause he has no actions to perform.");
            StartCoroutine(SkipTurnStep());
        }
        GM.ui.ShowGlobalAbilityButtons(character_abilities);
        return base.StartRound();
    }

    IEnumerator SkipTurnStep()
    {
        yield return new WaitForSeconds(.5f);
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

    public override string display_name
    {
        get
        {
            return PlayerParty.display_names[name];
        }
    }
}
