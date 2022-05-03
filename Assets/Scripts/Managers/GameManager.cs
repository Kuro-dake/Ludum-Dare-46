using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;
public class GameManager : MonoBehaviour
{
    public List<NamedSprite> sprites = new List<NamedSprite>();
    public Scenery current_scenery;
    public Sprite GetSprite(string sprite_name)
    {
        return sprites.Find(delegate (NamedSprite ns) { return ns.first == sprite_name; }).second;
    }
    [SerializeField]
    ResourcePickup pickup_prefab;
    public ResourcePickup GeneratePickup(Vector2 where, int amount)
    {
        ResourcePickup rp = Instantiate(pickup_prefab);
        rp.transform.SetParent(GM.scenery.transform.Find("pickup_parallax"));
        rp.transform.position = where;
        rp.Play(amount);
        return rp;
    }
    public bool game_over = false;
    public game_phase phase {
        get
        {
            if(GM.characters.alive_enemies.Count == 0)
            {
                return game_phase.movement;
            }
            else if(GM.characters.current_round_character is Enemy)
            {
                return game_phase.enemy_turn;
            }
            else
            {
                return game_phase.player_turn;
            }
        }
    }
    
    public static bool pause { get { return Time.timeScale == 0f; } set { Time.timeScale = value ? 0f : 1f; } }

    
    
    public void UpdateDevout()
    {
        GM.devout.text = _display_resources.ToString();
        return;
        GM.devoutright.text = "";
        if (GM.dev_options.hide_dev_text_output)
        {
            GM.devout.text = "";
            return;
        }
        Party p = GM.party;
        GM.devout.text = string.Format("<color=green>{0}</color> <color=blue>{1}</color> <color=red>{2}</color> <color=gray>{3}</color>  {4}", 
            p["Ranger"].hp, p["Wizard"].hp, p["Warrior"].hp, p["Gray"].hp, _display_resources
            );

        GM.devoutright.text = "";
        if(GM.game.current_enemy_party == null)
        {
            return;
        }
        
    
    }

    

    public void Hit(Vector2 point, Vector2 from, bool blood)
    {
        GM.audio_manager.PlaySound(blood ? "hit_flesh" : "hit", .3f, new FloatRange(.8f,1.2f));
        
        
        Vector2 ppos = point;
        Vector2 mpos = from;
        Vector2 direction = mpos - ppos;
        float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        /*blood_particle_obj.rotation = Quaternion.Euler(0f, 0f, rot_z + 180);
        blood_particle_obj.transform.position += blood_particle_obj.transform.right / 3f;*/
    }

    

    public void Update()
    {
        UpdateDevout();
        if (DevOptions.Key(KeyCode.Space))
        {
            GameManager.pause = !GameManager.pause;
        }

        GM.music.running = game_over ? 0 : ( phase == game_phase.movement ? 2 :3);
    }

    public void LoadLevelParams(int level)
    {
        YamlMappingNode level_node = Setup.GetFile("level" + level.ToString());
        level_node.GetNode<YamlMappingNode>("base_settings");


    }
    Dictionary<string, Queue<EnemyCluster>> cluster_queues = new Dictionary<string, Queue<EnemyCluster>>();
   
    public YAMLParams level_gen_params;

    [SerializeField]
    public Transform combat_camera_target;
    public Character current_round_character
    {
        get
        {
            return GM.characters.current_round_character;
        }
    }

    public class YAMLParams
    {
        YamlMappingNode ymn;
        public YAMLParams(YamlMappingNode _ymn)
        {
            ymn = _ymn;
        }
        public int seed { get { return ymn.GetInt("seed"); } }
        public int complexity { get { return ymn.GetInt("complexity"); } }
        public int[] straight { get { return ymn.GetIntArray("straight"); } }
        public int gen_probability { get { return ymn.GetInt("gen_probability"); } }
        public int gen_probability_decline { get { return ymn.GetInt("gen_probability_decline"); } }
        public override string ToString()
        {
            return string.Format("seed {0};complexity {1};straight {2},{3}; gen probability {4}, gen probability decline {5}"
                , seed, complexity, straight[0], straight[1], gen_probability, gen_probability_decline);
        }
    }

    Coroutine combat_routine = null;
    public bool is_combat_runnig { get { return combat_routine != null; } }
    public EnemyParty current_enemy_party;
    public Coroutine StartCombat(EnemyParty party)
    {
        current_enemy_party = party;
        
        
        if(combat_routine != null)
        {
            throw new UnityException("Trying to start combat when a battle is already going on");
        }
        combat_camera_target.transform.position = Vector2.Lerp(GM.party.transform.position, current_enemy_party.transform.position, .5f);
        //GM.cine_cam.target = combat_camera_target;
        return (combat_routine = StartCoroutine(CombatStep()));
    }
    Coroutine acting_routine = null;
    public bool is_acting { get { return acting_routine != null; } }
    public void Acting(Coroutine c)
    {
        if(is_acting)
        {
            throw new UnityException("Tried to start acting before the old act was finished");
        }
        acting_routine = StartCoroutine(ActingStep(c));
    }
    IEnumerator ActingStep(Coroutine c)
    {
        yield return c;
        Debug.Log("finished acting");
        acting_routine = null;
    }
    
    protected virtual IEnumerator CombatStep()
    {
        GM.characters.NewTurn();
        
        yield return GM.characters.NextCharacterTurn(true);
    
        while (GM.characters.any_enemies_alive)
        {

            while (!GM.characters.current_round_character.control.has_finished_acting || is_acting)
            {
                yield return null;
            }
            yield return new WaitForSeconds(.7f);
            yield return GM.characters.NextCharacterTurn();
            
            yield return null;

        }
        
        GM.cine_cam.target = GM.party.transform;
        Destroy(current_enemy_party.gameObject);
        GM.party.members.members.ForEach(delegate (Character c)
        {
            c.ClearBuffs();
        });
        GM.ui.HideSequence();
        combat_ended = Time.time;
        combat_routine = null;

        while(!(GM.characters.current_round_character is PlayerCharacter))
        {
            GM.characters.NextCharacterTurn();
            yield return null;
        }
        
        foreach(Character c in GM.party.members.members)
        {
            c.Heal(100);
            if (c.name == "Wizard")
            {
                Flash.DoFlash(c.gameObject);
                continue;
            }
            
            c.Shake();

            
            c.gameObject.AddComponent<Fader>().FadeIn();
        }
        GM.audio_manager.PlaySound("hit");
        GM.characters.RefreshCurrentCharacterMarker();

    }
    public float combat_ended = 0f;
    private void Start()
    {
        combat_ended = Time.time;
    }

    Coroutine display_amount_routine = null;
    int _display_resources = 0;
    int _resources = 0;
    public int resources
    {
        get
        {
            return _resources;
        }
        set
        {
            _resources = Mathf.Clamp(value, 0, int.MaxValue);

            if (display_amount_routine == null)
            {
                display_amount_routine = StartCoroutine(DisplayAmount());
            }
            
        }
    }

    AudioSource res_as;
    IEnumerator DisplayAmount()
    {
        FloatRange fr = new FloatRange(.9f, 1f);
        Debug.Log("up");
        while (_resources != _display_resources)
        {
            int difference = Mathf.Abs(_display_resources - _resources);
            int increment = Mathf.Clamp(Mathf.RoundToInt((float)difference / 10f), 0, int.MaxValue);

            if (increment == 0)
            {
                yield return null;
                increment = 1;
            }
            bool increase = _display_resources < _resources;
            _display_resources += increment * (increase ? 1 : -1);
            if (res_as == null)
            {
                res_as = GM.audio_manager.PlaySound("pickup");
            }
            if (res_as != null)
            {
                if (!increase) { res_as.Stop(); }
                res_as.time = 0f;
                res_as.pitch = fr;
            }
            yield return null;
        }
        display_amount_routine = null;
    }
    


}

public enum game_phase
{
    player_turn,
    enemy_turn,
    movement
}
