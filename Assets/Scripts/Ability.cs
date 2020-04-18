using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    public Character owner;
    public string name;
    public string sprite_name;
    public List<int> from_positions = new List<int>();
    public target_type target_type;
    [SerializeField]
    int damage = 1;
    public int GetDamage(List<Buff> buffs)
    {
        return damage + Buff.GetBuffsValue(buffs, buff_type.attack);
    }
    public int heal = 1;
    public List<int> target_positions = new List<int>();
    public int target_number = 1;
    public Buff buff;

    public Ability(System.Action a)
    {
        special_action = a;
        damage = 0;
        heal = 0;
    }
    System.Action special_action = null;
    public List<int> GetTargetsForAI()
    {
        List<int> ret = new List<int>(target_positions);
        ret.Shuffle();
        return ret.GetRange(0, target_number);
            
    }
    public void ApplyAbility()
    {
        if((target_type == target_type.enemy || target_type == target_type.ally) && target_number < 2)
        {
            throw new UnityException("Trying to apply an ability without a target");
        }
        Party target_party = owner.opposing_party;
        if (target_type == target_type.ally)
        {
            target_party = owner.party;
        }
        foreach(int pos in target_positions)
        {
            ApplyAbility(target_party[pos]);
        }
    }
    public void ApplyAbility(List<int> targets)
    {
        if(special_action != null)
        {
            special_action();
            owner.has_finished_acting = true;
            return;
        }
        foreach(int position in targets)
        {
            ApplyAbility(owner.opposing_party[position]);
        }
    }

    public void ApplyAbility(Character target)
    {
        if (special_action != null)
        {
            special_action();
            owner.SpendAP();
            return;
        }
        string ret = "";
        if (target == null)
        {
            ret += "No target provided for " + name + " ability\n";
            return;
        }
        if (!target.alive)
        {
            ret += "Target " + target.name + " is not alive";
            return;
        }

        ret += "\nApplying " + name + " ability on " + target.name + "\n";
        int dmg = damage > 0 ? GetDamage(owner.buffs) : 0;
        if (dmg > 0)
        {
            ret += "\nDealt " + damage + " damage\n";
            target.Hit(dmg);
        }
        int hpplus = heal;
        if (hpplus > 0)
        {
            ret += "\nHealed " + hpplus + " hp\n";
            target.Heal(hpplus);
        }
        if (!buff.is_inert)
        {
            target.ApplyBuff(buff);
        }
        Debug.Log(ret);
        owner.SpendAP();

    }
    public override string ToString()
    {
        string ret = "";
        ret += name + "\n";
        int dmg = damage;
        if (dmg > 0)
        {
            ret += "Deals " + damage + " damage\n";

        }
        int hpplus = heal;
        if (hpplus > 0)
        {
            ret += "Heals " + hpplus + " hp\n";

        }
        if (!buff.is_inert)
        {
            ret += "has active buff\n";
        }

        return ret;
    }

    public List<Character> TargetCharacters(List<int> poss = null)
    {
        if(poss == null)
        {
            poss = target_positions;
        }
        switch (target_type)
        {
            case target_type.self:
                return new List<Character>() { owner };
                
            case target_type.ally:
                return poss.ConvertAll<Character>(delegate (int pos) {
                    return owner.party.members.members_positions[pos];
                });
                
            case target_type.enemy:
                return poss.ConvertAll<Character>(delegate (int pos) {
                    return owner.opposing_party.members.members_positions[pos];
                });
                
        }
        return new List<Character>();
    }
}
[System.Serializable]
public class NamedBuffEffect : Pair<buff_type, int>
{
    public NamedBuffEffect(buff_type b, int i) : base(b, i) { }
}
public enum buff_type
{
    attack,
    defense,
    evade,
    actions,
    initiative
}
[System.Serializable]

public struct Buff
{
    public List<NamedBuffEffect> effects;
    public int this[buff_type t]
    {
        get
        {
            NamedBuffEffect nbe = effects.Find(delegate (NamedBuffEffect nbei) { return nbei.first == t; });
            if (nbe == null)
            {
                return 0;
            }
            return nbe.second;
        }
    }

    public static int GetBuffsValue(List<Buff> buffs, buff_type bt)
    {
        int ret = 0;
        buffs.ForEach(delegate (Buff b) { ret += b[bt]; });
        return ret;
    }

    public bool is_inert { get { return effects == null || effects.Count == 0; } }

    public int lasts;
}
public enum target_type
{
    enemy,
    ally,
    self,
    all
}