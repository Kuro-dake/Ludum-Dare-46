using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> clips = new List<AudioClip>();
    List<AudioSource> tracks = new List<AudioSource>();
    bool _is_on = false;
    public bool is_on
    {
        get
        {
            return _is_on;
        }
        set
        {
            _is_on = value;
            max_volume_runtime = value ? max_volume : 0f;
            PlayerPrefs.SetInt("music", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
    public float fade_in_speed = 10f;
    float fis_inv;
    public float max_volume;
    float max_volume_runtime;
    public int running = 0;
    void Start()
    {
        max_volume_runtime = max_volume;
        fis_inv = 1f / fade_in_speed;
        foreach (AudioClip c in clips)
        {
            GameObject g = new GameObject("track", new System.Type[] { typeof(AudioSource) });
            g.transform.SetParent(transform);
            AudioSource t = g.GetComponent<AudioSource>();
            t.clip = c;
            t.Stop();
            t.time = 0f;
            t.loop = true;
            t.volume = 0f;
            t.pitch = 1f;
            t.Play();
            tracks.Add(t);
        }
        this[0] = 1f;
        StartCoroutine(SyncTracks());
        //music_volume += Time.deltaTime * .2f * (alive && movement_speed > music_stars_start_speed ? 1 : -1);
        //music_volume = Mathf.Clamp (music_volume, 0f, 1f);
    }
    [SerializeField]
    float no_game_pitch = .05f, pitch_speed = .2f;
    IEnumerator SyncTracks()
    {
        while (true)
        {
            yield return new WaitForSeconds(16f);
            float ftime = tracks[0].time;
            tracks.ForEach(delegate (AudioSource obj) {
                obj.time = ftime;
            });
        }

    }
    void Update()
    {
        float target_volume = max_volume_runtime;
        switch (running)
        {
            case 0:
            case 1:

                break;
            case 2:
                target_volume *= .9f;
                break;
            case 3:
                target_volume *= .75f;
                break;
            case 4:
                target_volume *= .65f;
                break;
            default:
                target_volume *= .55f;
                break;
        }
        for (int i = 0; i < tracks.Count; i++)
        {
            this[i] = Mathf.MoveTowards(this[i], running > i || i == 0 ? target_volume : 0f, Time.deltaTime * fis_inv);
            this[i] = Mathf.Clamp(this[i], 0f, 1f);
        }
        tracks.ForEach(delegate (AudioSource obj) {
            //obj.pitch = Mathf.MoveTowards(tracks[0].pitch, running == 0 ? no_game_pitch : 1f, Time.deltaTime * pitch_speed);
        });
    }

    float this[int i]
    {
        get
        {
            return tracks[i].volume;
        }
        set
        {
            tracks[i].volume = value;
        }
    }

    /*AudioSource _music;
	AudioSource music {
		get{
			if (_music == null) {
				_music = GameObject.Find ("music").GetComponent<AudioSource>();
			}
			return _music;
		}
	}
	float music_volume{
		get {
			return music.volume;
		}
		set{
			music.volume = value;
		}
	}*/


}
