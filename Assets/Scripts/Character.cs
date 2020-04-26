using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YamlDotNet.RepresentationModel;
public abstract class Character : MonoBehaviour
{

    public Color color { get { return GetComponent<SpriteRenderer>().color; } }
    [SerializeField]
    protected int _hp = 1;
    public int hp { get { return _hp; } protected set { _hp = value; } }
    public int max_hp;
    [SerializeField]
    int _defense;
    public int defense { get { return _defense + GetBuffsValue(buff_type.defense); } }
    public bool is_alive
    {
        get { return hp > 0; }
    }
    public int initiative;
    [SerializeField]
    List<Ability> abilities = new List<Ability>();
    protected List<Ability> character_abilities
    {
        get
        {
            abilities.ForEach(delegate (Ability a) {
                a.owner = this;
            });
            return abilities;
        }
    }

    public List<Buff> buffs = new List<Buff>();

    int _actions_per_turn;
    public int actions_per_turn { get { return _actions_per_turn + GetBuffsValue(buff_type.actions); } }
    int GetBuffsValue(buff_type bt)
    {
        return Buff.GetBuffsValue(buffs, bt);
    }
    public int position; // 0-3 position on line
    public Party party;
    public Party opposing_party
    {
        get
        {
            if (party == GM.party) {
                return GM.game.current_enemy_party;
            }
            return GM.party;
        }
    }
    
    public void Heal(int amount)
    {
        hp = Mathf.Clamp(hp+amount,0, max_hp);
    }

    public virtual Coroutine GoToPosition(int pos = -1, bool instant = false)
    {
        if(pos < -1 || pos > 3)
        {
            throw new UnityException("Pos can't be " + pos);
        }
        return StartCoroutine(GoToPositionStart(pos, instant));
    }
    IEnumerator GoToPositionStart(int pos, bool instant = false)
    {
        while (go_to_position_routine != null)
        {
            yield return null;
        }
        go_to_position_routine = StartCoroutine(GoToPositionStep(pos, .5f, instant));
        yield return go_to_position_routine;
    }
    Coroutine go_to_position_routine;
    IEnumerator GoToPositionStep(int pos, float duration = 1f, bool instant = false)
    {
        Vector2 from = transform.localPosition;
        Vector2 to = new Vector2(party.GetPositionVector(pos == -1 ? position : pos), 0f);
        float current = 0f;
        float multiplier = 1f / duration;

        int instant_jump = instant ? 1000000 : 1;

        while((current += Time.deltaTime * instant_jump * multiplier) < 1f)
        {
            transform.localPosition = Vector2.Lerp(from, to, current);
            yield return null;
        }

        if (pos != -1)
        {
            position = pos;
        }
        go_to_position_routine = null;
    }

    bool initialized = false;
    bool _moving;
    public bool moving
    {
        get
        {
            return _moving;
        }
        set
        {
            _moving = value;
            anim.speed = (value ? 1.5f : .1f) * anim_multiplier;
        }
    }
    float anim_multiplier;
    public virtual void Initialize()
    {
        GM.characters.AddCharacter(this);
        initialized = true;
        hp = max_hp;
        anim_multiplier = UnityEngine.Random.Range(.8f, 1.2f);
        anim.speed = .1f * anim_multiplier;
        var ca = character_abilities;

        abilities.ForEach(delegate (Ability a) { a.owner = this; });
    }

    public void ApplyBuff(Buff b)
    {
        if(b.lasts < 2)
        {
            b.lasts = 2;
        }
        buffs.Add(b);
    }

    Transform _sprite_transform;
    Transform sprite_transform { get { return _sprite_transform != null ? _sprite_transform : (_sprite_transform = transform.Find("Sprite")); } }
    SpriteRenderer _sr;
    protected SpriteRenderer sr { get { return _sr != null ? _sr : (_sr = sprite_transform.GetComponent<SpriteRenderer>()); } }
    Animator _anim;
    public Animator anim { get { return _anim != null ? _anim : (_anim = GetComponent<Animator>()); } }

    Coroutine shake_routine;
    public Coroutine Shake()
    {
        if (shake_routine != null)
        {
            StopCoroutine(shake_routine);
        }
        return shake_routine = StartCoroutine(ShakeStep());
    }
    IEnumerator ShakeStep()
    {
        if(sprite_transform == null)
        {
            yield break;
        }
        float duration = .6f;
        while ((duration -= Time.deltaTime) > 0f)
        {
            sprite_transform.localPosition = UnityEngine.Random.insideUnitCircle * duration * 1.5f;
            yield return null;
        }
        sprite_transform.localPosition = Vector2.zero;
    }
    protected virtual void ReceiveDamage(int damage)
    {
        damage = Mathf.Clamp(damage - defense, 0, int.MaxValue);

        hp -= damage;
    }
    public virtual void Hit(int damage)
    {
        if (!is_alive)
        {
            return;
        }
        Shake();
        ReceiveDamage(damage);
        if (!is_alive)
        {
            sr.gameObject.AddComponent<Fader>().FadeOut();
            party.members.OnCharacterDeath(this);
            OnDeath();
        }
        GM.audio_manager.PlaySound("hit", 1f, new FloatRange(.8f,1.2f));
    }
    protected virtual void OnDeath() { }
    private void Update()
    {
        if (!initialized)
        {
            throw new UnityException("Character " + name + " wasn't initialized!");
        }
    }
    public CharacterControl control
    {
        get { return GetComponent<CharacterControl>(); }
    }
    public virtual Coroutine StartRound()
    {
        control.has_finished_acting = false;
        ap = max_ap;

        return control.StartRound();
    }
    public virtual void EndRound()
    {
        
        for(int i = 0; i < buffs.Count; i++)
        {
            Buff b = buffs[i];
            b.lasts--;
            buffs[i] = b;
        }

        buffs.RemoveAll(delegate (Buff b)
        {
            
            return b.lasts <= 0;
        });

        opposing_party.members.RemoveDead();
        
    }
    public virtual void StartTurn() { }
    public bool has_actions_left { get { return ap > 0; } }
    
    [SerializeField]
    int _max_ap = 1;
    public int max_ap { get { return _max_ap + GetBuffsValue(buff_type.actions); } }
    public int ap { get; protected set; }
    public bool SpendAP(int amount = 1)
    {
        if (ap >= amount)
        {
            ap -= amount;
            if(ap == 0)
            {
                control.has_finished_acting = true;
            }
            return true;
        }
        //has_finished_acting = true;
        return false;
    }
    
    public void SwitchPositions(Character c)
    {
        if(c.party != party)
        {
            throw new UnityException("Trying to switch with character from another party.");
        }
        int npos = c.position;
        GM.game.Acting(c.GoToPosition(position));
        GoToPosition(npos);
    }

   
    protected virtual void OnMouseExit()
    {
        GM.ui.HideAbilityButtons(this);
        GM.ui.info_panel.HideCharacterDescription(this);
    }
    protected virtual void OnMouseOver()
    {
        if (GM.can_show_ability_buttons && is_alive)
        {
            GM.ui.ShowAbilityButtons(this);
            GM.ui.info_panel.ShowCharacterDescription(this);
            
        }
    }
    protected target_type GetTargetType(Character c)
    {
        target_type type = target_type.enemy;

        if (c == this)
        {
            type = target_type.self;
        }
        else if (c.party == party)
        {
            type = target_type.ally;
        }
        return type;
    }
    public virtual List<Ability> GetAvailableAbilitiesFor(Character c)
    {
        target_type type = GetTargetType(c);
        
        List<Ability> ret = character_abilities.FindAll(delegate (Ability a)
        {
            if(type == target_type.self)
            {
                return a.target_type == target_type.self || a.target_type == target_type.ally;
            }
            return a.target_type == type && a.target_number == 1;
        });
        if(target_type.self == type || target_type.ally == type)
        {
            return ret; 
        }
        ret.RemoveAll(delegate (Ability a)
        {
            return a.target_number != 1;
        });
        return ret.FindAll(delegate (Ability a)
        {
            return a.CanUseFromPosition(position) && a.CanUseAtPosition(c.position) && a.target_number == 1;
        });
    }

    internal void ClearBuffs()
    {
        buffs.Clear();
    }

    internal void WriteSaveData(SavedCharacter savedCharacter)
    {
        savedCharacter.hp = hp;
        savedCharacter.max_hp = max_hp;
        savedCharacter.ap = ap;
        savedCharacter.max_ap = max_ap;
        savedCharacter.position = position;
        savedCharacter.defense = defense;
        savedCharacter.name = name;
        savedCharacter.initiative = initiative;
        savedCharacter.abilities = new List<Ability>(abilities);
    }

    public void LoadSavedData(SavedCharacter savedCharacter)
    {
        hp = savedCharacter.hp;
        max_hp = savedCharacter.max_hp;
        ap = savedCharacter.ap;
        _max_ap = savedCharacter.max_ap;
        position = savedCharacter.position;
        _defense = savedCharacter.defense;
        name = savedCharacter.name;
        initiative = savedCharacter.initiative;
        abilities = new List<Ability>(savedCharacter.abilities);
    }

    public void SetCharacterFromYAMLNode(YamlMappingNode node)
    {
        bool clear_abilities = true;

        try
        {
            clear_abilities = node.GetBool("clear_abilities");
        }
        catch (KeyNotFoundException) { }

        YamlMappingNode stats_node = null;
        try
        {
            stats_node = node.GetNode<YamlMappingNode>("stats");
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log("No global stats provided");
        }
        if (stats_node != null)
        {
            foreach (KeyValuePair<YamlNode, YamlNode> stat in stats_node.Children)
            {
                ModifyStat(stat.Key.ToString(), int.Parse(stat.Value.ToString()), true);
            }
            if (clear_abilities)
            {
                abilities.Clear();
            }
        }
        
        YamlMappingNode abilities_node = null;
        try
        {
            abilities_node = node.GetNode<YamlMappingNode>("abilities");
        }
        catch (KeyNotFoundException e)
        {
            Debug.Log("No global abilities provided");
        }
        if (abilities_node != null)
        {
            foreach (KeyValuePair<YamlNode, YamlNode> ability in abilities_node.Children)
            {
                Ability a = new Ability();
                string a_name = ability.Key.ToString();
                a.name = a_name;
                foreach (KeyValuePair<YamlNode, YamlNode> stat in ((YamlMappingNode)ability.Value).Children)
                {
                    string stat_name = stat.Key.ToString();
                    string stat_value = stat.Value.ToString();
                    switch (stat_name)
                    {
                        case "ability_range":
                            a.ability_range = (ability_range)Enum.Parse(typeof(ability_range), stat_value);
                            break;
                        case "target_type":
                            a.target_type = (target_type)Enum.Parse(typeof(target_type), stat_value);
                            break;
                        case "sprite":
                            a.sprite_name = stat_value;
                            break;
                        default:
                            a.ModifiyStat(stat_name, int.Parse(stat_value), true);
                            break;
                    }

                }
                abilities.Add(a);
            }
        }


    }

    public void ModifyStat(string stat_name, int value, bool set = false)
    {
        switch (stat_name)
        {
            case "maxhp":
                max_hp = (set ? 0 : max_hp) + value;
                break;
            case "defense":
                _defense = (set ? 0 : _defense) + value;
                break;
            case "initiative":
                initiative = (set ? 0 : initiative) + value;
                break;
            default:
                throw new UnityException("Stat " + stat_name + " does not exist");
                
        }
    }


    public Ability GetAbilityByName(string a_name)
    {
        return abilities.Find(delegate (Ability a)
        {
            return a.name == a_name;
        });
    }

    public override string ToString()
    {
        string ret = "<b>{0}</b>\n{1}/{2} HP\n{3} initiative\n{4} defense";
        ret = string.Format(ret, display_name, hp,max_hp, initiative, defense);
        if(character_abilities.Count > 0)
        {
            ret += "\n\n <b>Abilities:</b>";
        }
        else
        {
            ret += "\n\n <b>No abilities</b>";
        }
        foreach(Ability a in character_abilities) {
            ret += "\n" + a.ToString();
        }
        return ret;
    }

    public virtual string display_name
    {
        get { return name; }
    }
    [SerializeField]
    string _icon_name;
    public virtual string icon_name
    {
        get
        {
            return _icon_name;
        }
    }

    public virtual void Step()
    {
        if (moving)
        {
            GM.audio_manager.PlaySound("step", 1f, new FloatRange(.3f, .4f));
        }
    }

    public Ability GetRandomAvailableAbility()
    {
        List<Ability> pick = character_abilities.FindAll(delegate (Ability a) {
            return a.target_positions.Contains(position);
        });
        if (pick.Count == 0)
        {
            return null;
        }
        return character_abilities[UnityEngine.Random.Range(0, character_abilities.Count)];
    }
}
