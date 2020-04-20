using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YamlDotNet.RepresentationModel;
public class CharacterManager : MonoBehaviour
{
    public bool speedup_wizard_dev = false;
    List<GameObject> target_marks = new List<GameObject>();
    public void MarkTargets(List<Character> targets = null, Color c = new Color())
    {
        c = c == Color.clear ? new Color(.8f,.6f,.6f): c;
        target_marks.ForEach(delegate (GameObject go) { Destroy(go); });
        target_marks.Clear();
        if (targets == null)
        {
            return;
        }
        foreach(Character target in targets)
        {
            target_marks.Add(CreateMark(target.transform,c));
        }
    }

    public GameObject CreateMark(Transform target, Color c)
    {
        GameObject mark = new GameObject("mark");
        SpriteRenderer sr = mark.AddComponent<SpriteRenderer>();
        sr.sprite = GM.ui.GetIcon("arrow");
        sr.color = c;
        sr.sortingLayerName = "UI";
        mark.transform.localScale = Vector3.one * ARROW_SCALE_MULTIPLIER;
        mark.transform.SetParent(target);
        mark.transform.localPosition = Vector2.up * ARROW_UP_MULTIPLIER / target.lossyScale.y;
        mark.AddComponent<Arrow>();
        return mark;
    }

    public const float ARROW_UP_MULTIPLIER = 10f;
    public const float ARROW_SCALE_MULTIPLIER = 2.5f;
    public NamedObjects named_enemies = new NamedObjects();
    [SerializeField]
    List<EnemyData> enemy_data = new List<EnemyData>();

    #region Clusters
    [System.Serializable]
    public class ClusterTypes
    {
        public List<NamedCluster> cluster_types = new List<NamedCluster>();
        public EnemyCluster this[string n]
        {
            get
            {
                return cluster_types.Find(delegate (NamedCluster nc) { return nc.first == n; }).second;
            }
        }
    }
    public ClusterTypes clusters;
    #endregion
    [SerializeField]
    EnemyParty enemy_party_prefab = null;
    public void SetCurrentCharacter(Character c) {
        if(GM.game.phase != game_phase.movement)
        {
            return;
        }
        List<Character> ioc = GetInitiativeOrderedCharacters();
        for(int i = 0; i < ioc.Count; i++)
        {
            if(ioc[i] == c)
            {
                _current_round_character_index = i;
                RefreshCurrentCharacterMarker();
                return;
            }
        }
        
    }
    public List<Enemy> all_enemies
    {
        get
        {
            return all_characters.FindAll(delegate (Character c)
            {
                return c is Enemy;
            }).ConvertAll<Enemy>(delegate (Character c)
            {
                return c as Enemy;
            });
        }
    }

    int _current_round_character_index;
    public int current_round_character_index { get { return _current_round_character_index; }
        set
        {
            int character_num = all_characters.Count;
            int val = value % character_num;
            val += val < 0 ? character_num : 0;
            _current_round_character_index = val;
        }
    }
    public Character current_round_character
    {
        get
        {
            return GetInitiativeOrderedCharacters()[current_round_character_index];
        }
    }

    List<Character> _all_characters = new List<Character>();
    public List<Character> alive_characters
    {
        get { return all_characters.FindAll(delegate (Character en) { return en.is_alive; }); }
    }
    public List<Enemy> alive_enemies
    {
        get { return alive_characters.FindAll(delegate (Character c) { return c is Enemy; }).ConvertAll<Enemy>(delegate (Character c) { return c as Enemy; }); }
    }

    public List<Character> all_characters
    {
        get
        {
            return new List<Character>(_all_characters);
        }
    }

    public bool any_alive
    {
        get
        {
            return alive_characters.Count > 0;
        }
    }
    public bool any_enemies_alive { get { return alive_enemies.Count > 0; } }

    public void AddCharacter(Character c)
    {
        _all_characters.Add(c);
    }
    public void RemoveCharacter(Character c)
    {
        _all_characters.Remove(c);
    }
    Enemy GetEnemy(string n)
    {
        return enemy_data.Find(delegate (EnemyData e)
        {
            return e.name == n;
        }).enemy;
    }
    public Coroutine CreateEncounterFromString(string pars)
    {
        EnemyParty ep = Instantiate(enemy_party_prefab);
        int i = 0;
        foreach(string s in pars.Split(new char[] { ';' })){ 
            Enemy e = Instantiate(GetEnemy(s));
            e.gameObject.name = e.gameObject.name.Replace("(Clone)", "");
            e.position = i++;
            e.transform.SetParent(ep.members.transform);
            e.transform.localPosition = Vector2.zero;

        }
        Vector2 eppos = Vector2.zero;
        eppos.x = GM.party.transform.position.x + 30f;
        eppos.y = GM.party.transform.position.y;
        ep.transform.position = eppos;
        ep.transform.SetParent(GameContainer.game_inst.transform);
        return GM.game.StartCombat(ep);
        
    }
   
    public void KillAll()
    {
        all_characters.ForEach(delegate (Character obj)
        {
            obj.StopAllCoroutines();
            Destroy(obj.gameObject);
        });
        _all_characters.Clear();
    }
    
    public Enemy GenerateEnemy(string type, Vector2 pos)
    {
        return PlaceEnemy(type, pos);
    }
    Enemy PlaceEnemy(Enemy e, Vector2 pos)
    {
        Vector3 pos3 = new Vector3(pos.x, pos.y, 0f);
        e.transform.position = pos3;
        return e;
    }
    Enemy PlaceEnemy(string type, Vector2 pos)
    {
        Enemy e = Instantiate(named_enemies.GetByName(type)).GetComponent<Enemy>();
        e.gameObject.name = type + " " + Random.Range(0, 399);
        return PlaceEnemy(e, pos);
    }

    public List<Character> GetInitiativeOrderedCharacters()
    {
        List<Character> chars = all_characters;
        chars.Sort(delegate (Character a, Character b)
        {
           return b.initiative.CompareTo(a.initiative);
        });
        
        return chars;
    }
    [TextArea(10,15)]
    public string randomCombinations;
    public int current_x = 300;
    public int end_x = 5500;
    public IntRange gaps = new IntRange(50, 150);
    void doRandomCombinations()
    {
        
        string[] possible_units = new string[] { "goblin1", "goblin2" };
        randomCombinations = "";
        bool first = true;
        while((current_x += gaps.random) < end_x)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                randomCombinations += "\n";
            }
            int num = Random.Range(0, 2) == 1 ? Random.Range(2,5) : 4;
            randomCombinations += current_x.ToString() + "-encounter:";
            for (int i = 0; i < num; i++)
            {
                randomCombinations += possible_units[Random.Range(0, possible_units.Length)] + (i < num-1 ? ";":"");
            }
        }


    }
    public void Initialize()
    {
        doRandomCombinations();
        current_round_character_index = 5;
        RefreshCurrentCharacterMarker();
        if (speedup_wizard_dev)
        {
            GM.party["Wizard"].initiative = 10;
        }
    }

    public void NewTurn()
    {
        alive_characters.ForEach(delegate (Character c)
        {
            c.StartTurn();
        });
    }
    public bool select_active_character = true;
    public Coroutine NextCharacterTurn(bool first = false)
    {
        if (first)
        {
            current_round_character_index = 0;
            NewTurn();
        }
        else
        {
            current_round_character.EndRound();
            int prev_index = current_round_character_index++;
            if (prev_index > current_round_character_index)
            {
                NewTurn();
            }
        }
        if (!any_enemies_alive)
        {
            return null;
        }
        if (!current_round_character.is_alive)
        {
            return NextCharacterTurn(); 
        }
        Debug.Log(current_round_character.name + " is starting his round with an initiative of " + current_round_character.initiative);

        RefreshCurrentCharacterMarker();
        GM.ui.ShowSequence();
        return current_round_character.StartRound();

    }

    public bool show_enemy_targets = false;

    private void RefreshCurrentCharacterMarker()
    {
        dev_turn_marker.SetParent(current_round_character.transform);
        dev_turn_marker.localPosition = new Vector2(.2f, .6f);
    }

    public List<Character> GetNextTurnSequence()
    {
        if (!show_enemy_targets) {
            return new List<Character>();
        }
        List<Character> ioc = GetInitiativeOrderedCharacters();
        
        List<Character> ret = new List<Character>();
        for(int i = current_round_character_index; i < ioc.Count; i++)
        {
            if (!ioc[i].is_alive)
            {
                continue;
            }
            ret.Add(ioc[i]);
        }
        for (int i = 0; i < current_round_character_index; i++)
        {
            if (!ioc[i].is_alive)
            {
                continue;
            }
            ret.Add(ioc[i]);
        }
        return ret;
    }

    public Transform dev_turn_marker;
}
[System.Serializable]
public class EnemyData
{
    public string name;
    public Enemy enemy;
    public int level;
}