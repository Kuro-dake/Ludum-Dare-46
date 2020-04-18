using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticleSystem : MonoBehaviour
{
    public void Play ()
    {
        GetComponent<ParticleSystem>().Play();
        
    }
}
