using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvResourcePickup : EnvEvent
{
    int amount;
    public override void Initialize()
    {
        amount = int.Parse(data);
    }
    protected override void PlayEvent()
    {
        base.PlayEvent();
        GM.game.GeneratePickup(transform.position, amount);
        Destroy(gameObject);
    }

}

