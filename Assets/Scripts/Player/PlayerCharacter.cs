using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    #region Special Abilities
    Ability _switch_ability = null;
    Ability switch_ability
    {
        get
        {
            if (_switch_ability == null)
            {
                _switch_ability = new Ability(delegate { SwitchPositions(current_ability_target); });
                _switch_ability.name = "Switch positions with another party member";
                _switch_ability.owner = this;
                _switch_ability.sprite_name = "switch";
                _switch_ability.target_number = 1;
                _switch_ability.ability_range = ability_range.global;
                _switch_ability.target_type = target_type.ally;
            }
            return _switch_ability;
        }
    }
    Ability _select_ability = null;
    Ability select_ability
    {
        get
        {
            if (_select_ability == null)
            {
                _select_ability = new Ability(delegate { GM.characters.SetCurrentCharacter(current_ability_target); });
                _select_ability.name = "Set active character to another party member";
                //ColorUtility.TryParseHtmlString("#A7FFA7", out _select_ability.ability_color);
                _select_ability.owner = this;
                _select_ability.sprite_name = "arrow";
                _select_ability.target_number = 1;
                _select_ability.ability_range = ability_range.global;
                _select_ability.target_type = target_type.ally;
            }
            return _select_ability;
        }
    }
    Ability _skip_ability = null;
    Ability skip_ability
    {
        get
        {
            if (_skip_ability == null)
            {
                _skip_ability = new Ability(delegate { SkipTurn(); });
                _skip_ability.name = "Skip turn";
                _skip_ability.owner = this;
                _skip_ability.sprite_name = "skip";
                _skip_ability.target_number = 1;
                _skip_ability.target_type = target_type.self;
            }

            return _skip_ability;
        }
        
    }

    #endregion
    public override List<Ability> GetAvailableAbilitiesFor(Character c)
    {
        bool combat = GM.game.phase != game_phase.movement;
        target_type type = GetTargetType(c);
        List<Ability> ret =  combat ? base.GetAvailableAbilitiesFor(c) : new List<Ability>();
        if (target_type.self == type || target_type.ally == type)
        {
            if (target_type.ally == type)
            {
                
                ret.Add(switch_ability);
                if (!combat)
                {
                    
                    ret.Add(select_ability);
                }
            }
            if(target_type.self == type && combat)
            {   
                ret.Add(skip_ability);
            }
            return ret;
        }
        return ret;
    }
    public void SkipTurn()
    {
        control.has_finished_acting = true;
    }
    public List<Ability> currently_usable_abilities
    {
        get
        {
            List<Ability> ret = new List<Ability>();
            bool combat = GM.game.phase != game_phase.movement;

            if (combat)
            {
                ret.AddRange(character_abilities);
            }
            ret.Add(switch_ability);
            ret.Add(skip_ability);

            return ret;
        }
    }
    public override Coroutine StartRound()
    {
        GM.ui.ShowGlobalAbilityButtons(currently_usable_abilities);
        return base.StartRound();
    }
    
    IEnumerator SkipTurnStep()
    {
        yield return new WaitForSeconds(.5f);
        control.has_finished_acting = true;
    }
    
    public override void EndRound()
    {
        GM.ui.HideGlobalAbilityButtons();
        base.EndRound();
    }
    public override void Initialize()
    {
        if(control == null)
        {
            gameObject.AddComponent<PlayerControl>();
        }
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

    public override string icon_name
    {
        get
        {
            return name.ToLower();
        }
    }

    protected override void OnDeath()
    {
        if(name == "Gray")
        {
            GM.ui.GameOver();
        }
    }

}
