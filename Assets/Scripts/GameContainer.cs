using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContainer : MonoBehaviour
{
    [SerializeField]
    GameObject game = null;
    GameObject _game_inst = null;
    static GameContainer inst;
    public static GameObject game_inst { get { return inst._game_inst; } }
    SavedPosition checkpoint;
    GM gm { get { return GetComponentInChildren<GM>(); } }
    public static void SaveData()
    {
        inst.checkpoint = new SavedPosition();
        inst.checkpoint.Save();
    }
    bool boot_on_start = true;
    int init_state = 76137515;
    private void Start()
    {
        
        inst = this;
        if (boot_on_start && gm == null)
        {
            Reboot();
        }
        else
        {
            if(gm != null)
            {
                _game_inst = gm.transform.parent.gameObject;
                gm.Initialize();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Reboot();
        }
    }
    public static void ReloadGame()
    {
        inst.Reboot();
    }
    void Reboot()
    {
        StartCoroutine(RebootStep());
    }
    IEnumerator RebootStep()
    {
        if(_game_inst != null)
        {
            Destroy(_game_inst);
        }
        yield return null;
        yield return null;
        _game_inst = Instantiate(game);
        _game_inst.transform.SetParent(transform);
        int tick = init_state == -1 ? System.Environment.TickCount : init_state;
        Debug.Log(tick);
        Random.InitState(tick);
        gm.Initialize();
        if(checkpoint != null)
        {
            foreach (SavedCharacter sc in checkpoint.characters)
            {
                GM.party[sc.name].LoadSavedData(sc);
            }
            
            
            Vector3 npos = GM.party.transform.position;
            npos.x = -checkpoint.x;
            GM.party.transform.position = npos;
            GM.scenery.Movement(-checkpoint.x);
            GM.scenery.DeleteSkippedXtags();
            GM.level_manager.SpawnObject("checkpoint", 0f).GetComponent<Checkpoint>().ForceActivated();

            GM.characters.show_enemy_targets = checkpoint.show_enemy_targets;
            GM.game.resources = checkpoint.resources;
        }
        
    }

}

[System.Serializable]
public class SavedPosition
{
    public float x;
    public bool show_enemy_targets;
    public int resources;
    public List<SavedCharacter> characters = new List<SavedCharacter>();

    public void Save()
    {
        
        x = GM.scenery.x;
        show_enemy_targets = GM.characters.show_enemy_targets;
        resources = GM.game.resources;
        foreach (Character c in GM.party.members.members)
        {
            characters.Add(new SavedCharacter(c));
        }
    }

}
[System.Serializable]
public class SavedCharacter
{
    public int hp;
    public int max_hp;
    public int ap;
    public int max_ap;
    public int position;
    public int defense;
    public string name;
    public int initiative;
    public List<Ability> abilities = new List<Ability>();

    public SavedCharacter(Character c)
    {
        c.WriteSaveData(this);

    }

}