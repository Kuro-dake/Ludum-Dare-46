using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    
    Dictionary<KeyCode, direction> keycode_dir = new Dictionary<KeyCode, direction>() {
        {KeyCode.UpArrow, direction.up },
        {KeyCode.DownArrow, direction.down },
        {KeyCode.RightArrow, direction.right },
        {KeyCode.LeftArrow, direction.left },
        {KeyCode.W, direction.up },
        {KeyCode.S, direction.down },
        {KeyCode.D, direction.right },
        {KeyCode.A, direction.left }
    };
    public float interact_radius = 40f;
    public Character character_clicked = null;
    internal Character character_hover = null;

    void Update()
    {
        Character cchar = GM.game.current_round_character;
        GM.party.Face(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        /// mouse scroll controls
        if (Input.mouseScrollDelta.y > 0f) {  }
        else if (Input.mouseScrollDelta.y < 0f) {  }

        GM.cine_cam.screenX = .5f;
        GM.cine_cam.screenY = .5f;
        
        if(GM.game.phase == game_phase.player_turn)
        {
            if(character_clicked != null) {
                cchar.Interact(character_clicked);
                
            }
        }
        if (GM.game.phase == game_phase.movement)
        {
            foreach (KeyValuePair<KeyCode, direction> kv in keycode_dir)
            {

                if (Input.GetKey(kv.Key))
                {
                    InputControls(kv.Value);
                }
            }
        }

        character_clicked = null;
        character_hover = null;
    }

    
    void InputControls(direction d)
    {
            if (new List<direction>() { direction.up, direction.down }.Contains(d))
            {
                return;
            }
            

            GM.party.Movement(d);
            
        
        
    }
}
