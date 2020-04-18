using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    static GM _inst;
    public static bool game_ended = false;
    public static GM inst
    {
        get
        {
            if (_inst == null)
            {
                _inst = GameObject.Find("GM").GetComponent<GM>();
            }
            return _inst;
        }
    }

    public GameObject this[string s]
    {
        get
        {
            Transform[] ret = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (Transform g in ret)
            {
                if (g.gameObject.name == s)
                {
                    return g.gameObject;
                }
            }
            throw new UnityException(s + " not found");

        }
    }


    static Dictionary<string, Component> scripts = new Dictionary<string, Component>();

    public static T GetScript<T>(string name) where T : Component
    {

        string script_key = name + typeof(T).ToString();
        if (!scripts.ContainsKey(script_key))
        {
            T ret = inst[name].GetComponent<T>();
            if (ret == null)
            {
                throw new UnityException("script " + script_key + " not found");
            }
            scripts.Add(script_key, ret);
        }
        return scripts[script_key] as T;
    }

    public static LevelManager level_manager { get { return GetScript<LevelManager>("LevelManager"); } }
    public static EnemyManager enemies { get { return GetScript<EnemyManager>("EnemyManager"); } }
    public static Text devout { get { return GetScript<Text>("DevOut"); } }
    
    public static bool devout_visible {
        set
        {
            devout.GetComponentInParent<Canvas>().enabled = value;
        }
    }
    public static EffectsIndex effects { get { return GetScript<EffectsIndex>("EffectsIndex"); } }
    public static GameManager game { get { return GetScript<GameManager>("GameManager"); } }
    public static Controls controls { get { return GetScript<Controls>("Controls"); } }
    public static DialogueManager cinema { get { return GetScript<DialogueManager>("DialogueManagerCamera"); } }
    public static AudioManager audio_manager { get { return GetScript<AudioManager>("AudioManager"); } }
    public static Scenery scenery { get { return game.current_scenery; } }
    
    [SerializeField]
    Walker _walker = null;
    public static Walker walker { get { return inst._walker; } }
    public static CineCam cine_cam { get { return GetScript<CineCam>("CM vcam1"); } }
    [SerializeField]
    Sprite _square = null, _circle = null;
    public static Sprite square { get { return inst._square; } }
    public static Sprite circle { get { return inst._circle; } }

    public DevOptions _dev_options = new DevOptions();
    public static DevOptions dev_options { get { return inst._dev_options; } }
    public void Initialize()
    {
        
        game.LoadLevelParams(1);
      
        game.LoadLevel(1);
        
        
        dev_options.Init();


        /*terrace.Initialize();
        terrace.from.VisitNode();*/
        walker.Initialize();
        level_manager.GenerateLevel("World");
        game.current_scenery.Initialize();
        //cinema.PlayLevelString(0); 
    }

    

    private void Update()
    {
        dev_options.Update();
        if(DevOptions.Key(KeyCode.R)){
            cinema.PlayLevelString(0, "", true); 
        }
    }
    public static void ReloadScene()
    {
        //Enemy.KillAll();
        scripts.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Awake()
    {
        /*return;
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 12;*/
    }
}
