using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ParticleSystemRotationReset : MonoBehaviour
{
    ParticleSystem ps;
    private List<Vector4> starting_rotations = new List<Vector4>();
    private void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        
    }
    
    private void LateUpdate()
    {
        ParticleSystem.Particle[] pss = new ParticleSystem.Particle[ps.particleCount];
        
        ps.GetParticles(pss);
        ps.GetCustomParticleData(starting_rotations, ParticleSystemCustomData.Custom1);
        
        for(int i = 0; i<pss.Length; i++)
        {
            if(starting_rotations[i].x == 0.0f){
                starting_rotations[i] = new Vector4(pss[i].rotation,0f,0f,0f);
            }
            if (pss[i].remainingLifetime < pss[i].startLifetime * .5f)
            {
                pss[i].remainingLifetime = pss[i].startLifetime;
                pss[i].rotation = starting_rotations[i].x;
                pss[i].angularVelocity = 10f;
            }
            
        }
        ps.SetCustomParticleData(starting_rotations, ParticleSystemCustomData.Custom1);
        ps.SetParticles(pss, pss.Length);
    }
}
