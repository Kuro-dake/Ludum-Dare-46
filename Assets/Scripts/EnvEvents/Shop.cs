using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : EnvEvent
{

    protected override void PlayEvent()
    {
        base.PlayEvent();
        GM.ui.OpenShop(data);
    }

}

