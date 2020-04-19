using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{

    public Animator mouth;
    public string actor_name_display;
    Animator anim
    {
        get
        {
            return GetComponent<Animator>();
        }
    }

    
    public bool talking
    {
        set
        {
            if (mouth != null)
            {
                mouth.SetBool("talking", value);
            }
        }
    }
    float current_rot = 0f;
    float target_rot = 1f;
    float multiply_rot = 3f;
    float turn_takes = 3f;
    float turn_takes_inverse;
    private void Update()
    {
        transform.localRotation = Quaternion.LerpUnclamped(Quaternion.identity, Quaternion.Euler(Vector3.forward * multiply_rot), current_rot);
        current_rot = Mathf.MoveTowards(current_rot, target_rot, Time.deltaTime * turn_takes_inverse);
        if(Mathf.Abs(current_rot) >= Mathf.Abs(target_rot) && Mathf.Sign(current_rot) == Mathf.Sign(target_rot))
        {
            target_rot *= -1;
        }
    }
    private void Start()
    {
        turn_takes += Random.Range(0f, 2f);
        turn_takes_inverse = 1f / turn_takes;
        if (anim != null)
        {
            anim.SetBool("acting", true);
            anim.speed = Random.Range(.9f, 1.1f);
        }
    }
    Coroutine movement_routine;
    public void Goto(direction from, bool leave = false)
    {
        Vector2 v_from = Vector2.zero, v_to = Vector2.zero;
        
        switch (from)
        {
            case direction.left:
                v_from = GM.cinema.la_gone_pos;
                v_to = GM.cinema.la_pos;
                break;
            case direction.right:
                v_from = GM.cinema.ra_gone_pos;
                v_to = GM.cinema.ra_pos;
                
                break;
            default:
                throw new UnityException("Direction " + from.ToString() + " not defined.");
                
        }
        v_from.y = v_to.y;
        if (leave)
        {
            Vector2 switcher = v_from;
            v_from = v_to;
            v_to = switcher;
        }
        /*Debug.Log("actor " + actor_name_display + (leave ? " leaving to " : " coming from ") + from.ToString());
        Debug.Log("from: " + v_from);
        Debug.Log("to: " +v_to);*/
        
        //anim.SetBool("acting", true);
        
        if (movement_routine != null)
        {
            StopCoroutine(movement_routine);
        }
        movement_routine = StartCoroutine(GotoStep(v_from, v_to,leave));
    }

    IEnumerator GotoStep(Vector3 from, Vector3 to, bool disable = false, float duration = .5f)
    {

        transform.position = from;
        
        float current = 0f;
        float duration_inverse = 1f / .5f;
        while((current += Time.deltaTime) < duration)
        {
            transform.position = Vector2.Lerp(from, to, current * duration_inverse);
            yield return null;
        }
        movement_routine = null;
        if (disable)
        {
            gameObject.SetActive(false);
        }
    }
}
