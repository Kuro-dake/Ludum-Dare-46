using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContainer : MonoBehaviour
{
    [SerializeField]
    GameObject game = null;
    GameObject game_inst = null;
    static GameContainer inst;
    
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
                game_inst = gm.transform.parent.gameObject;
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
    void Reboot()
    {
        StartCoroutine(RebootStep());
    }
    IEnumerator RebootStep()
    {
        if(game_inst != null)
        {
            Destroy(game_inst);
        }
        yield return null;
        yield return null;
        game_inst = Instantiate(game);
        game_inst.transform.SetParent(transform);
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
        }
        
    }

}

[System.Serializable]
public class SavedPosition
{
    public float x;
    public List<SavedCharacter> characters = new List<SavedCharacter>();

    public void Save()
    {
        x = GM.scenery.x;
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