using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : EnvEvent
{
    protected override void PlayEvent()
    {
        base.PlayEvent();
        GameContainer.SaveData();
    }
}
