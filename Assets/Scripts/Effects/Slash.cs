using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : Effect
{
    Coroutine slash_routine;
    public bool orientation_right;
    public override void Play(Vector2 position)
    {
        base.transform.position = position;
        if (slash_routine != null)
        {
            StopCoroutine(slash_routine);

        }
        slash_routine = StartCoroutine(SlashStep());

    }
    [SerializeField]
    float _slash_step = .3f;
    float slash_step { get {

            return _slash_step * (t_to - t_from); 
        } }
    
    public int divide_into = 4;
    public TrailRenderer slash_trail_prefab;
    
    TrailRenderer tr { get { return transform.Find("slash").GetComponent<TrailRenderer>(); } }
    public int trail_lines = 4;
    public float trail_renderer_width = 1f;
    public float trail_width = 10f;
    public float trail_offset;
    public Color trail_color;
    Dictionary<TrailRenderer, float> tr_positions = new Dictionary<TrailRenderer, float>();
    void AddPosition(bool hard = false)
    {
        foreach (TrailRenderer tr in gameObject.GetComponentsInChildren<TrailRenderer>())
        {
            Vector2 pos = tr.transform.position;
            if (horizontal)
            {
                pos.y = (transform.position.y + pos.y) / 2f;
                if (hard)
                {
                    tr.transform.position = pos;
                }
            }
            tr.AddPosition(pos);
        }
    }
    bool tr_enabled
    {
        set
        {
            foreach (TrailRenderer tr in gameObject.GetComponentsInChildren<TrailRenderer>())
            {
                tr.emitting = value;
            }
        }
    }
    bool horizontal = true;
    float t_from = 0f, t_to;
    IEnumerator SlashStep()
    {

        Quaternion start = Quaternion.Euler(Vector3.forward * 34f);
        Quaternion target = Quaternion.Euler(Vector3.forward * 140f);

        tr_positions.Clear();

        horizontal = Random.Range(0, 2) == 1;
        
        
        if (orientation_right)
        {
            start.z *= -1;
            target.z *= -1;
        }

        if(Random.Range(0,2) == 1)
        {
            Quaternion swap = start;
            start = target;
            target = swap;
        }

        
        t_to = 1f;

        if (horizontal)
        {
            t_from = -3f;
            t_to = 1f;
        }
        float current = t_from;

        float current_stop = 0f;
        float stop_at = (t_to - t_from) / divide_into;
        

        float y_step = trail_width / (trail_lines - 1);
        
        transform.rotation = Quaternion.LerpUnclamped(start, target, current);
        float delay_before_destroy = 0f;
        for (int i = 0; i < trail_lines; i++)
        {
            TrailRenderer ntr = Instantiate(slash_trail_prefab, transform);
            float y = trail_offset + y_step * i;
            ntr.transform.localPosition = Vector3.up * y;
            ntr.startWidth = trail_renderer_width;
            ntr.autodestruct = true;
            ntr.startColor = ntr.endColor = trail_color;
            tr_positions.Add(ntr, y);
            if (i == 0) {
                delay_before_destroy = ntr.time;
            }

        }
        AddPosition(true);
        yield return null;
        ResetTRPositions();
        
        do
        {
            Quaternion rotation = Quaternion.LerpUnclamped(start, target, current);
            transform.rotation = rotation;


            if ((current_stop += slash_step) <= stop_at)
            {
                AddPosition();
            }
            else
            {
                AddPosition(horizontal);
                current_stop = 0f;
                yield return null;
                if (horizontal)
                {
                    ResetTRPositions();
                }
            }
        }
        while ((current += slash_step) <= t_to);
        if (horizontal)
        {
            AddPosition(true);
        }
        tr_enabled = false;
        yield return new WaitForSeconds(delay_before_destroy);
        Destroy(gameObject);
    }
  
    void ResetTRPositions()
    {
        foreach (KeyValuePair<TrailRenderer, float> kv in tr_positions)
        {
            kv.Key.transform.localPosition = Vector2.up * kv.Value;
        }
    }
}
