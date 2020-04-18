using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : PlayerCharacter
{
    
    // Update is called once per frame
    Dictionary<KeyCode, direction> direction_keys = new Dictionary<KeyCode, direction>()
    {
        {KeyCode.LeftArrow, direction.left},
        {KeyCode.RightArrow, direction.right},
        {KeyCode.UpArrow, direction.up},
        {KeyCode.DownArrow, direction.down},
    };
    
    bool moving = false;
    
    Transform _static_anim;
    Transform _sa_parent;
    Transform sa_parent { get { return _sa_parent != null ? _sa_parent : (_sa_parent = transform.Find("sa_parent")); } }
    bool orientation_right { get { return Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x; } }
    
    void Update()
    {
        if(anim != null)
        {
            anim.SetBool("moving", moving);
        }
        
        moving = false;

    }
    [SerializeField]
    float movement_speed = 10f;
    public override void Movement(direction d)
    {
        moving = true;
        
        switch (d)
        {
            case direction.left:
                transform.position += Vector3.left * Time.deltaTime * movement_speed;
                GM.cine_cam.screenX = orientation_right ? .65f : .95f;

                break;
            case direction.right:

                transform.position += Vector3.right * Time.deltaTime * movement_speed;
                GM.cine_cam.screenX = !orientation_right ? .35f : .05f;
                
               
                break;
            case direction.up:

                //transform.position += Vector3.up * Time.deltaTime * mspeed * .3f;
                break;
            case direction.down:

                //transform.position += Vector3.down * Time.deltaTime * mspeed * .3f;
                break;
        }
        moving = true;
        GM.scenery.Movement(transform.position.x);
        
    }

   
    public override void Initialize()
    {
        base.Initialize();
        GM.cine_cam.target = transform;
    }
    
    [SerializeField]
    float melee_range = 10f;
    [SerializeField]
    Transform torso;
    Vector3 torso_position { get { return torso.position; } }
    public void MeleeAttack()
    {
        Slash slash = GM.effects["slash"] as Slash;
        slash.trail_offset = melee_range * .33f;
        slash.trail_width = melee_range * .66f;
        slash.orientation_right = orientation_right;
        slash.Play(torso_position);
        

    }
}
