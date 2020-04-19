using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParty : Party
{
    public static Dictionary<string, string> display_names = new Dictionary<string, string>() {
        { "Warrior" , "Shield: the warrior" },
        { "Wizard" , "Sigil: the wizard" },
        { "Ranger" , "Dusk: the deceiver" },
        { "Gray" , "Gray: the demon" },
    };
    // Update is called once per frame
    Dictionary<KeyCode, direction> direction_keys = new Dictionary<KeyCode, direction>()
    {
        {KeyCode.LeftArrow, direction.left},
        {KeyCode.RightArrow, direction.right},
        {KeyCode.UpArrow, direction.up},
        {KeyCode.DownArrow, direction.down},
    };

    public Transform aim;
    public Transform aim_parent;

    Transform _static_anim;
    Transform _sa_parent;
    Transform sa_parent { get { return _sa_parent != null ? _sa_parent : (_sa_parent = transform.Find("sa_parent")); } }
    
    void Update()
    {
        
        foreach(Character c in members.members)
        {
            c.moving = moving;
        }
        
        
        moving = false;

    }
    [SerializeField]
    float movement_speed = 10f;
    public void Movement(direction d)
    {
        
        
        switch (d)
        {
            /*case direction.left:
                Debug.Log("work left");
                transform.position += Vector3.left * Time.deltaTime * movement_speed;
                GM.cine_cam.screenX = orientation_right ? .65f : .95f;

                break;*/
            case direction.right:
                moving = true;
                transform.position += Vector3.right * Time.deltaTime * movement_speed;
                GM.cine_cam.screenX = !orientation_right ? .35f : .35f;
                
               
                break;
            case direction.up:

                //transform.position += Vector3.up * Time.deltaTime * mspeed * .3f;
                break;
            case direction.down:

                //transform.position += Vector3.down * Time.deltaTime * mspeed * .3f;
                break;
        }
        
        GM.scenery.Movement(transform.position.x);
        
    }

    /// <summary>
    /// initialization makes sure camera has something to follow for a fluid camera movement
    /// </summary>
    
    public override void Initialize()
    {
        aim = new GameObject("aim").transform;

        SpriteRenderer sr = aim.gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = null;// GM.circle;
        sr.color = Color.green;
        sr.sortingLayerName = "UI";
        aim.localScale *= 1f;

        aim_parent = new GameObject("aim_parent").transform;
        aim.SetParent(aim_parent);
        aim.localPosition = Vector2.up * 7f;
        
        aim_parent.SetParent(transform);
        aim_parent.localPosition = Vector2.zero;
        GM.cine_cam.target = transform;

        string[] starting_order = new string[] { "Warrior", "Ranger", "Gray", "Wizard" };
        
        for (int i = 0; i < starting_order.Length; i++)
        {
            Character c = members.transform.Find(starting_order[i]).GetComponent<Character>();
            c.position = i;
            
        }
        base.Initialize();



    }

    

    public void Face(Vector2 mpos)
    {
        Vector2 ppos = transform.position;
        Vector2 direction = mpos - ppos;
        float target = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        aim_parent.rotation = Quaternion.Euler(Vector3.forward * (target - 90f));
        
        
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
