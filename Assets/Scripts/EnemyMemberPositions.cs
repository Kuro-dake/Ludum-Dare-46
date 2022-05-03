using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemberPositions : Members
{
    protected override float[] positions { 
        get {
            float[] ret = base.positions;
            for(int i = 0; i < ret.Length; i++)
            {
                ret[i] *= -1;
            }
            return ret;
        } 
    }

}
