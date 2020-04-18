using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]
    float _multiplier = 1f;
    float multiplier { get { return _multiplier; } set { _multiplier = value; inverse_multiplier = 1f / value; } }
    float inverse_multiplier;
    public float x = 0;
    public float offset = 0f;
    public float width;
    float width_scaled;
    public parallax_mode mode;
    [SerializeField]
    bool item_keep_alive = false;
    ParticleSystem ps { get { return GetComponent<ParticleSystem>(); } }
    SpriteRenderer sr { get { return GetComponent<SpriteRenderer>(); } }

    bool top_level = true;
    void Start()
    {
        multiplier = multiplier;
        if (top_level && mode == parallax_mode.repeating_background)
        {
            Initialize();
        }
        orig_pos = transform.localPosition;
    }
    void Initialize()
    {
        List<Transform> to_destroy = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            to_destroy.Add(transform.GetChild(i));
        }
        if (ps != null)
        {
            width = ps.shape.scale.x;
        }
        else if (sr != null && sr.drawMode == SpriteDrawMode.Tiled)
        {
            width = transform.localScale.x * sr.size.x;
        }

        List<Transform> to_add = new List<Transform>();
        for (int i = 0; i < 2; i++)
        {
            Transform clone = Instantiate(gameObject).transform;
            Parallax p = clone.GetComponent<Parallax>();
            p.top_level = false;
            Destroy(clone.GetComponent<Parallax>());


            if (ps != null)
            {
                ParticleSystem.MainModule mm = clone.GetComponent<ParticleSystem>().main;
                clone.GetComponent<ParticleSystemRenderer>().sortingOrder += 1;
                mm.simulationSpace = ParticleSystemSimulationSpace.Local;
            }
            to_add.Add(clone);
        }

        to_add.ForEach(delegate (Transform clone)
        {
            clone.transform.SetParent(transform, true);
            clone.transform.localScale = Vector3.one;

        });
        if (ps != null)
        {
            Destroy(ps);
            ParticleSystem.MainModule mmm = ps.main;
            mmm.loop = false;
            mmm.simulationSpeed = 10f;
        }
        if (sr != null)
        {
            Destroy(GetComponent<SpriteRenderer>());
        }
        to_destroy.ForEach(delegate (Transform t) { Destroy(t.gameObject); });
        width_scaled = width / transform.localScale.x;
        foreach (ParticleSystem cps in GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.MainModule mmm = cps.main;
            mmm.simulationSpeed = 100f;
        }
        StartCoroutine(RestoreScale());
    }
    IEnumerator RestoreScale()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();


        foreach (ParticleSystem cps in GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.MainModule mmm = cps.main;
            mmm.simulationSpeed = 1f;
        }
    }
    public bool disable_y_parallax;
    public void UpdateParallax()
    {
        float y_parallax_multiplier = disable_y_parallax ? 0 : GM.game.current_scenery.y_parallax_multiplier;
        Vector2 cam_offset = (1f-multiplier) * GM.cine_cam.cam_offset * -1;
        Vector2 npos = new Vector2(0f, cam_offset.y * y_parallax_multiplier);
        switch (mode)
        {
            case parallax_mode.children_only:
                npos.x = (x + offset - cam_offset.x) / transform.localScale.x * multiplier;
                transform.localPosition = npos;
                break;
            case parallax_mode.repeating_background:
                float x_mod = (x + offset - cam_offset.x) / transform.localScale.x * multiplier;

                for (int i = 0; i < transform.childCount; i++)
                {
                    float cx = x_mod + width_scaled * i;
                    cx = Mathf.Repeat(cx, width_scaled * transform.childCount) - width_scaled;
                    npos.x = cx;
                    transform.GetChild(i).localPosition = npos;
                }
                break;
            case parallax_mode.item:
                npos.x = (x + offset) * multiplier - cam_offset.x;
                npos.y = orig_pos.y + cam_offset.y * y_parallax_multiplier;
                transform.localPosition = npos;
                
                break;

        }
    }
    Vector2 orig_pos;
    const float parallax_buffer = 250f;
    public void CheckActive()
    {
        if(mode != parallax_mode.item || item_keep_alive)
        {
            return;
        }
        orig_pos = transform.localPosition;
        gameObject.SetActive((x + offset) * multiplier > -parallax_buffer && (x + offset) * multiplier < parallax_buffer);
        
    }
}

public enum parallax_mode
{
    repeating_background,
    item,
    children_only
}
