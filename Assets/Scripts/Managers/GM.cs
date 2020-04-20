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

   
    [SerializeField]
    LevelManager _level_manager = null;
    public static LevelManager level_manager { get { return _inst._level_manager; } }
    [SerializeField]
    CharacterManager _characters = null;
    public static CharacterManager characters { get { return _inst._characters; } }
    [SerializeField]
    Text _devout = null;
    public static Text devout { get { return _inst._devout; } }
    [SerializeField]
    Text _devoutright = null;
    public static Text devoutright { get { return _inst._devoutright; } }
    [SerializeField]
    UIControls _ui = null;
    public static UIControls ui { get { return _inst._ui; } }
    public static bool devout_visible
    {
        set
        {
            devout.GetComponentInParent<Canvas>().enabled = value;
        }
    }
    [SerializeField]
    EffectsIndex _effects = null;
    public static EffectsIndex effects { get { return _inst._effects; } }
    [SerializeField]
    GameManager _game = null;
    public static GameManager game { get { return _inst._game; } }
    [SerializeField]
    Controls _controls = null;
    public static Controls controls { get { return _inst._controls; } }
    [SerializeField]
    DialogueManager _cinema = null;
    public static DialogueManager cinema { get { return _inst._cinema; } }
    [SerializeField]
    AudioManager _audio_manager = null;
    public static AudioManager audio_manager { get { return _inst._audio_manager; } }
    public static Scenery scenery { get { return game.current_scenery; } }
    
    [SerializeField]
    PlayerParty _party = null;
    public static PlayerParty party { get { return inst._party; } }
    [SerializeField]
    CineCam _cine_cam;
    public static CineCam cine_cam { get { return _inst._cine_cam; } }
    [SerializeField]
    Sprite _square = null, _circle = null;
    [SerializeField]
    Music _music;
    public static Music music { get { return inst._music; } }
    public static Sprite square { get { return inst._square; } }
    public static Sprite circle { get { return inst._circle; } }

    public DevOptions _dev_options = new DevOptions();
    public static DevOptions dev_options { get { return inst._dev_options; } }
    bool initialized = false;
    public void Initialize()
    {
        Debug.Log("Initializing GM");
        if (initialized)
        {
            throw new UnityException("The GM was already initialized.");
        }
        initialized = true;
        _inst = GameObject.Find("GM").GetComponent<GM>();
        game.LoadLevelParams(1);
        
        
        dev_options.Init();


        /*terrace.Initialize();
        terrace.from.VisitNode();*/
        party.Initialize();
        level_manager.GenerateLevel("World");
        game.current_scenery.Initialize();
        cinema.PlayLevelString(0); 
        characters.Initialize();
        //game.StartCombat(_enemy_party);
        ui.Initialize();
    }

   
    public static bool can_show_description
    {
        get
        {
            return !cinema.active && !game.game_over;
        }
    }
    public static bool can_show_ability_buttons
    {
        get
        {
            
            return !cinema.active && !ui.shop_open && !game.game_over;
        }
    }

    public static bool can_walk
    {
        get
        {
            return game.combat_ended < Time.time + 1f && !cinema.active && !ui.shop_open && !game.is_combat_runnig && !game.game_over;
        }
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
