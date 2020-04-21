using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public List<NamedAudioClip> clips = new List<NamedAudioClip>();
    
    private void Start()
    {
        
    }

    public AudioSource PlaySound(string sound_name, float volume = .3f, FloatRange pitch = null)
    {
        return PlayTrack(clips.Find(delegate (NamedAudioClip ac)
        {
            return ac.first == sound_name;
        }).second, volume, pitch);
    }

    public int max_sources = 5;

    public AudioSource PlayTrack(AudioClip ac, float volume = 1f, FloatRange pitch = null )
    {
        pitch = pitch == null ? new FloatRange(1f, 1f) : pitch;
        if (!gameObject.activeInHierarchy) { return null; }
        AudioSource c_as = null;
        foreach(AudioSource asc in GetComponents<AudioSource>())
        {
            if (!asc.isPlaying)
            {
                Destroy(asc);
            }
        }
        if(c_as == null && GetComponents<AudioSource>().Length < max_sources) 
        {
            c_as = gameObject.AddComponent<AudioSource>();
        }
        if(c_as != null)
        {
            c_as.clip = ac;
            c_as.pitch = pitch;
            c_as.time = 0f;
            c_as.loop = false;
            c_as.volume = volume;
            c_as.Play();
            return c_as;
        }
        else
        {
            Debug.Log("Too many clips playing");
            return null;
            //throw new UnityException("Too many AudioSources.");
        }
    }

    public void FadeOutSource(AudioSource asc, float duration = 1f)
    {
        StartCoroutine(FadeOutSourceStep(asc, duration));
    }

    IEnumerator FadeOutSourceStep(AudioSource asc, float duration) {

        float starting_volume = asc.volume;
        float current = 0f;
        while((current += Time.deltaTime / duration ) < 1f) {
            
            asc.volume = Mathf.Lerp(starting_volume, 0f, current);
            yield return null;
        }
        asc.Stop();
    }

}
