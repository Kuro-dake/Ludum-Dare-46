﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    public void ModifiyStat(string stat_name, int value)
    {
        switch (stat_name)
        {
            case "damage":
                damage += value;
                break;
            case "heal":
                heal += value;
                break;
            case "target_number":
                target_number += value;
                break;
            case "buff":
                buff.effects[0].second += value;
                break;
        }
    }
    [System.NonSerialized]
    public Character owner;
    [System.NonSerialized]
    public Color ability_color = Color.white;
    public string name;
    public string sprite_name;
    public List<int> from_positions = new List<int>();
    public List<int> from_positions_bypass
    {
        get
        {
            return new List<int>() { 0, 1, 2, 3 };
        }
    }
    
    public target_type target_type;
    [SerializeField]
    int damage = 1;
    public int GetDamage(List<Buff> buffs)
    {
        return damage + Buff.GetBuffsValue(buffs, buff_type.damage);
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
    public List<int> GetTargetPositionsForAI()
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
        Flash.DoFlash(owner.gameObject);
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
        if (!target.is_alive)
        {
            ret += "Target " + target.name + " is not alive";
            return;
        }

        ret += "\nApplying " + name + " ability on " + target.name + "\n";
        int dmg = damage > 0 ? GetDamage(owner.buffs) : 0;
        if (dmg > 0)
        {
            ret += "\nDealt " + dmg + " damage\n";
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
        
        int dmg = damage;
        if (dmg > 0)
        {
            dmg = GetDamage(owner.buffs);
            ret += "Deals " + dmg + " damage";

        }
        int hpplus = heal;
        if (hpplus > 0)
        {
            ret += " Restore " + hpplus + " hp";

        }
        if (!buff.is_inert)
        {
            NamedBuffEffect nbe = buff.effects[0];
            ret += (ret.Length > 0 ? " and " : "") + " gives + "+ nbe.second + " " + nbe.first.ToString() + " boost";
        }
        string singular = "target";
        string plural = "targets";
        switch (target_type)
        {
            case target_type.enemy:
                singular = " enemy";
                plural = " enemies";
                ret += " to " + target_number + (target_number == 1 ? singular : plural);
                break;
            case target_type.ally:
                singular = " ally";
                plural = " allies";
                ret += " to " + target_number + (target_number == 1 ? singular : plural);
                break;
            case target_type.self:
                ret += " to self";
                break;
        }
        if (!buff.is_inert)
        {
            NamedBuffEffect nbe = buff.effects[0];
            string rounds = "";
            int rnum = Mathf.Clamp(buff.lasts - 1,0,100);
            switch (rnum)
            {
                case 1:
                    rounds = "next round";
                    break;
                case 0:
                    rounds = "this round";
                    break;
                case 100:
                    rounds = "the whole battle";
                    break;
                default:
                    rounds = rnum.ToString() + " rounds";
                    break;
            }
            ret += " for " + rounds;
        }

        return "<b>"+name+"</b>" + (ret.Length > 0 ? "\n" + ret : "");
    }

    public List<Character> TargetCharacters(List<int> poss = null)
    {
        if(poss == null)
        {
            poss = target_positions;
        }
        List<Character> ret = new List<Character>();
        switch (target_type)
        {
            case target_type.self:
                ret.Add( owner );
                break;
                
            case target_type.ally:
                ret = poss.ConvertAll<Character>(delegate (int pos) {
                    return owner.party[pos];
                });
                break;
                
            case target_type.enemy:
                ret =  poss.ConvertAll<Character>(delegate (int pos) {
                    return owner.opposing_party[pos];
                });
                break;
                
        }
        ret.RemoveAll(delegate (Character c) { return c == null; });
        return ret;
    }
    bool EvalRightPosition(int pos, List<int> positions, Party party) {
        if (positions.Contains(pos))
        {
            return true;
        }
        pos -= party.members.alive_members.Count;
        return positions.Contains(pos);
    }
    public bool CanUseFromPosition(int pos)
    {
        return EvalRightPosition(pos, from_positions_bypass, owner.party);
    }

    public bool CanUseAtPosition(int pos)
    {
        return EvalRightPosition(pos, target_positions, target_type == target_type.enemy ? owner.opposing_party : owner.party);
    }

}


[System.Serializable]
public class NamedBuffEffect
{
    public buff_type first;
    public int second;

    public NamedBuffEffect(buff_type b, int i) { first = b;second = i; }
}

public enum buff_type
{
    damage,
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