using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Character : MonoBehaviour
{
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
    [SerializeField]
    protected int _hp = 1;
    public int hp { get { return _hp; } protected set { _hp = value; } }
    public int max_hp;
    [SerializeField]
    int _defense;
    public int defense { get { return _defense + GetBuffsValue(buff_type.defense); } }
    public bool alive
    {
        get { return hp > 0; }
    }
    public int initiative;
    public void Heal(int amount)
    {
        hp = Mathf.Clamp(hp+amount,0, max_hp);
    }

    public Coroutine GoToPosition(int pos = -1)
    {
        if(pos < -1 || pos > 3)
        {
            throw new UnityException("Pos can't be " + pos);
        }
        return StartCoroutine(GoToPositionStart(pos));
    }
    IEnumerator GoToPositionStart(int pos)
    {
        while (go_to_position_routine != null)
        {
            yield return null;
        }
        go_to_position_routine = StartCoroutine(GoToPositionStep(pos));
        yield return go_to_position_routine;
    }
    Coroutine go_to_position_routine;
    IEnumerator GoToPositionStep(int pos, float duration = 1f)
    {
        Vector2 from = transform.localPosition;
        Vector2 to = new Vector2(party.GetPositionVector(pos == -1 ? position : pos), 0f);
        float current = 0f;
        float multiplier = 1f / duration;

        while((current += Time.deltaTime) < 1f)
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
    public virtual void Initialize()
    {
        GM.characters.AddCharacter(this);
        initialized = true;
        hp = max_hp;
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
    Transform sprite_transform { get { return _sprite_transform != null ? _sprite_transform : (_sprite_transform = transform.Find("sprite")); } }
    SpriteRenderer _sr;
    protected SpriteRenderer sr { get { return _sr != null ? _sr : (_sr = sprite_transform.GetComponent<SpriteRenderer>()); } }
    Animator _anim;
    protected Animator anim { get { return _anim != null ? _anim : (_anim = sprite_transform.GetComponent<Animator>()); } }

    Coroutine shake_routine;
    Coroutine Shake()
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
        float duration = .2f;
        while ((duration -= Time.deltaTime) > 0f)
        {
            sprite_transform.localPosition = UnityEngine.Random.insideUnitCircle * duration * 2f;
            yield return null;
        }
        sprite_transform.localPosition = Vector2.zero;
    }
    protected virtual void ReceiveDamage(int damage)
    {
        damage -= defense;
        hp -= damage;
    }
    public virtual void Hit(int damage)
    {
        if (!alive)
        {
            return;
        }
        Shake();
        ReceiveDamage(damage);
        if (!alive)
        {
            GetComponent<SpriteRenderer>().color -= Color.black * .5f;
            party.members.OnCharacterDeath(this);
        }
    }

    private void Update()
    {
        if (!initialized)
        {
            throw new UnityException("Character " + name + " wasn't initialized!");
        }
    }
    public virtual Coroutine StartRound()
    {
        has_finished_acting = false;
        ap = max_ap;

        return null;
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
    public bool has_finished_acting = false;
    [SerializeField]
    int _max_ap = 1;
    public int max_ap { get { return _max_ap + GetBuffsValue(buff_type.actions); } }
    protected int ap;
    public bool SpendAP(int amount = 1)
    {
        if (ap >= amount)
        {
            ap -= amount;
            if(ap == 0)
            {
                has_finished_acting = true;
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

    private void OnMouseDown()
    {
        //GM.controls.character_clicked = this;
    }
    private void OnMouseEnter()
    {
        GM.ui.ShowAbilityButtons(this, GM.game.current_round_character.GetAvailableAbilitiesFor(this));
    }
    private void OnMouseExit()
    {
        GM.ui.HideAbilityButtons();
    }
    private void OnMouseOver()
    {
        GM.ui.character_hover = this;
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
            return a.target_type == type;
        });
        if(target_type.self == type || target_type.ally == type)
        {
            return ret; 
        }
        return ret.FindAll(delegate (Ability a)
        {
            return a.from_positions.Contains(position) && a.target_positions.Contains(c.position) && a.target_number == 1;
        });
    }

    internal void ClearBuffs()
    {
        buffs.Clear();
    }

    internal void WriteSaveData(SavedCharacter savedCharacter)
    {
        hp = savedCharacter.hp;
        max_hp = savedCharacter.max_hp;
        ap = savedCharacter.ap;
        _max_ap = savedCharacter.max_ap;
        position = savedCharacter.position;
        _defense = savedCharacter.defense;
        
        initiative = savedCharacter.initiative;
        abilities = new List<Ability>(savedCharacter.abilities);
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
}
