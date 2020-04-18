using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsIndex : MonoBehaviour
{
    [SerializeField]
    List<NamedEffect> effects = new List<NamedEffect>();

    public Effect this[string n]{get{ return Instantiate(effects.Find(delegate (NamedEffect ne) { return ne.first == n; }).second); }}
}
[System.Serializable]
public class NamedEffect: Pair<string, Effect>
{
    public NamedEffect(string s, Effect e) : base(s, e) { }
}