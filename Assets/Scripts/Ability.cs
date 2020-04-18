using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability
{
    public string name;
    public Sprite sprite;
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

    public List<int> GetTargetsForAI()
    {
        List<int> ret = new List<int>(target_positions);
        ret.Shuffle();
        return ret.GetRange(0, target_number);
            
    }

    public void ApplyAbility(Character owner, List<int> targets)
    {
        foreach(int position in targets)
        {
            ApplyAbility(owner, owner.opposing_part[position]);
        }
    }

    public void ApplyAbility(Character owner, Character target)
    {
        if(target == null)
        {
            Debug.Log("No target provided for " + name + " ability");
            return;
        }
        if (!target.alive)
        {
            Debug.Log("Target " + target.name + " is not alive");
            return;
        }

        Debug.Log("Applying " + name + " ability on " + target.name);
        int dmg = damage > 0 ? damage + GetDamage(owner.buffs) : 0;
        if (dmg > 0)
        {
            Debug.Log("Dealt " + damage + " damage");
            target.Hit(dmg);
        }
        int hpplus = heal;
        if (hpplus > 0) {
            Debug.Log("Healed " + hpplus + " hp");
            target.Heal(hpplus);
        }
        if (!buff.is_inert)
        {
            target.ApplyBuff(buff);
        }


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

    public bool is_inert { get { return effects.Count == 0; } }

    public int lasts;
}
public enum target_type
{
    enemy,
    ally,
    self,
    all
}