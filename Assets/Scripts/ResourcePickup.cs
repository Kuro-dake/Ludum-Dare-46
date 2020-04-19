using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePickup : MonoBehaviour
{
    ParticleSystem _ps;
    ParticleSystem ps { get { return _ps != null ? _ps : (_ps = GetComponent<ParticleSystem>()); } }

    public int amount;

   public void Play(int num)
    {
        amount = num;
        ParticleSystem.MainModule mm = ps.main;
        mm.maxParticles = amount;
        ps.Play();
        StartCoroutine(PlayStep());
    }

    IEnumerator PlayStep()
    {
        yield return new WaitForSeconds(1f);
        while(ps.particleCount > 0)
        {
            yield return null;
        }
        GM.game.resources += amount;
        Destroy(gameObject);
    }
}
