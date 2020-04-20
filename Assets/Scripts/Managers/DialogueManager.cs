using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    
    [SerializeField]
    Text dialogue = null;
    [SerializeField]
    Transform actors = null;
    Camera camera_component;
    private void Start()
    {
        camera_component = GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            
            Progress();
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse1))
        {

            Progress(true);
        }
    }
    string speech
    {
        get
        {
            return dialogue.text;
        }
        set
        {
            dialogue.text = value;
        }
    }

    [TextArea(10, 15)]
    public List<string> level_intros = new List<string>();
    [SerializeField]
    bool dev_shutdown_instead = false;
    public void PlayLevelString(int string_num, string prefix = "", bool force_reload = false)
    {
        if (dev_shutdown_instead)
        {
            active = false;
            return;
        }
        active = true;
        LoadLevelIntros(force_reload);
        PlayString(prefix + level_intros[string_num]);
    }
    
    public Coroutine PlayDialogue(string d_name)
    {
        active = true;
        // coroutine running through gamemanager, because dialogue manager object is disabled during inactivity
        return GM.game.StartCoroutine(PlayDialogueStep(d_name));
    }
    IEnumerator PlayDialogueStep(string d_name)
    {
        
        string dialogue = Resources.Load<TextAsset>("dialogues/" + d_name).text;
        PlayString(dialogue);
        while (active)
        {
            yield return null;
        }
        
    }

    public void LoadLevelIntros(bool force_reload = false)
    {
        if (level_intros.Count != 0 && !force_reload)
        {
            return;
        }

        string text = Resources.Load<TextAsset>("dialogues/dialogues").text;
        level_intros = new List<string>(text.Split(new string[] { "\n--\n" }, System.StringSplitOptions.RemoveEmptyEntries));
    }
    string[] lines;
    int current_line = 0;
    public bool active
    {
        get
        {
            return gameObject.activeSelf;
        }
        set
        {
            GM.devout_visible = !value;
            gameObject.SetActive(value);
            if (!value)
            {
                GM.game.combat_ended = Time.time;
            }
           // GM.canvas.SetActive(!value);
        }
    }
    
    public void Initialize()
    {
        GetComponentInChildren<Canvas>().enabled = true;
        if (left_actor != null)
        {
            left_actor.transform.position = la_gone_pos;
        }
        if (right_actor != null)
        {
            right_actor.transform.position = ra_gone_pos;
        }
        left_actor = right_actor = null;
    }

    public void PlayString(string s)
    {

        /*
                Initialize();

                active = true;
                transform.parent.localScale = Vector3.one * GM.inst.cam_size_to_cinema_scale_and_genie_positions[GM.cam.ortosize][0]; // I'm going to hell for this as well
                MoveTo(lord, g_gone + Vector3.right * 25f, true);
                if (cinema_phase == 1)
                {
                    MoveTo(peasant, p_gone, true);
                    MoveTo(genie, g_gone, true);

                    MoveTo(peasant, p_talk);
                    MoveTo(genie, g_talk);
                }
                */
        Initialize();
        lines = s.Split(new char[] { '|' });
        current_line = 0;
        Progress();
    }
    Coroutine progress_routine;
    
    Actor left_actor, right_actor;
    [SerializeField]
    Transform left_anchor = null, right_anchor = null;
    public Vector2 la_pos { get { return left_anchor.transform.position; } }
    public Vector2 ra_pos { get { return right_anchor.transform.position; } }
    public Vector2 la_gone_pos { get { return EnemyCluster.DirectionToScreenBound(direction.left, camera_component) + Vector2.left * 10f; } }
    public Vector2 ra_gone_pos { get { return EnemyCluster.DirectionToScreenBound(direction.right, camera_component) + Vector2.right * 10f; } }
    public bool Progress(bool force_end = false)
    {

        /*if (progress_routine != null)
        {
            return true;
        }*/
        
        if (force_end)
        {
            current_line = lines.Length;
        }
        if (current_line >= lines.Length)
        {
            if (!GM.game_ended)
            {
                active = false;
            }
            return false;
         }

        progress_routine = StartCoroutine(ProgressStep(lines[current_line]));

        current_line++;
        return true;
    }
    AudioSource ads
    {
        get
        {
            return GetComponent<AudioSource>(); 
        }
    }
    string TextToFileName(string text)
    {
        new List<string>() { "<color=green>", "<b>", "=", "<", ">", "\n", " ", ".", ",", "!", "*", "?", "'" }.ForEach(delegate (string find)
        {
            text = text.Replace(find, "");
        });
        int namelen = 15;
        return text.Substring(0, text.Length < namelen ? text.Length : namelen).ToLower();
    }
    void StopActorsTalking()
    {
        if (left_actor != null)
        {
            left_actor.talking = false;
        }
        if (right_actor != null)
        {
            right_actor.talking = false;
        }
        ads.Stop();
    }
    Dictionary<string, string> names = new Dictionary<string, string>() {
        
        { "none", "" }

    };
    Coroutine talkwatch_routine;
    Actor GetActor(string actor_name)
    {
        Transform actor_transform = actors.Find(actor_name);
        if(actor_transform == null)
        {
            if (!names.ContainsKey(actor_name))
            {
                throw new UnityException("Actor named '" + actor_name + "' does not exist, and is not registered amongst the faceless actors.");
            }
            return null;
        }
        Actor ret = actor_transform.GetComponent<Actor>();
        if (ret == null)
        {
            throw new UnityException("Actor named '" + actor_name + "' has a transform, but doesn't contain the Actor script.");
        }
        return ret; 
    }
    IEnumerator ProgressStep(string line)
    {
        if (talkwatch_routine != null)
        {
            StopCoroutine(talkwatch_routine);
        }
        string[] line_params = line.Split(new char[] { ':' });
        string[] side_id = line_params[0].ToLower().Split(new char[] { ';' });

        string text = line_params[1];
        string char_id = side_id[0];


        Actor current_actor = GetActor(char_id);
        StopActorsTalking();
        if (current_actor != null)
        {
            current_actor.gameObject.SetActive(true);
            current_actor.talking = true;
        }
        
        if (side_id[1] == "l")
        {
            if (left_actor != null && (left_actor != current_actor || current_actor == null))
            {
                left_actor.Goto(direction.left, true);
            }
            if (current_actor != null)
            {
                Vector3 ls = current_actor.transform.localScale;
                current_actor.transform.localScale = new Vector3(Mathf.Abs(ls.x), ls.y, ls.z);

                if (left_actor != current_actor)
                {
                    current_actor.Goto(direction.left);
                }
                
            }
            left_actor = current_actor;


        }
        else
        {
            if (right_actor != null && (right_actor != current_actor || current_actor == null))
            {
                right_actor.Goto(direction.right, true);
            }
            if (current_actor != null)
            {
                Vector3 ls = current_actor.transform.localScale;
                current_actor.transform.localScale = new Vector3(Mathf.Abs(ls.x) * -1f, ls.y, ls.z);

                if (right_actor != current_actor)
                {
                    current_actor.Goto(direction.right);
                }
                
            }
            right_actor = current_actor;
        }


        string textname = (current_actor != null ? current_actor.actor_name_display : ( names.ContainsKey(char_id) ? names[char_id] : char_id));
        speech = (textname.Length > 0 ? "<b>" + textname + "</b>\n\n" : "\n" ) + text; /* + ";fn:"+TextToFileName(text);*/

        AudioClip clip = (AudioClip)Resources.Load("Audio/Speech/"+ TextToFileName(text));
        if (clip != null)
        {
            ads.Stop();
            ads.PlayOneShot(clip);
            
        }
        talkwatch_routine = StartCoroutine(WatchTalking(clip == null ? text.Length : -1));

        yield return null;

        progress_routine = null;

        if (text.Length == 0)
        {
            Progress();
        }
    }

    IEnumerator WatchTalking(int length)
    {
        if (length == -1)
        {
            while (ads.isPlaying)
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(length * .1f);
        }
        StopActorsTalking();
        talkwatch_routine = null;
    }

    // Start is called before the first frame update
    

}
