using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YamlDotNet.RepresentationModel;
public class CharacterManager : MonoBehaviour
{
    public NamedObjects named_enemies = new NamedObjects();
    
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
        get { return all_characters.FindAll(delegate (Character en) { return en.alive; }); }
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
    
   
    private void Update()
    {
        if (DevOptions.Key(KeyCode.Q))
        {
            current_round_character_index++;
            Debug.Log(current_round_character_index);
        }
        if (DevOptions.Key(KeyCode.W))
        {
            current_round_character_index--;
            Debug.Log(current_round_character_index);
        }
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

    public void Initialize()
    {
        current_round_character_index = 5;
        Debug.Log(current_round_character_index);
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
        if (!current_round_character.alive)
        {
            return NextCharacterTurn(); 
        }
        if (select_active_character)
        {
            UnityEditor.Selection.activeGameObject = current_round_character.gameObject;
        }
        Debug.Log(current_round_character.name + " is starting his round with an initiative of " + current_round_character.initiative);
        
        dev_turn_marker.SetParent(current_round_character.transform);
        dev_turn_marker.localPosition = Vector3.up * 1.5f;

        return current_round_character.StartRound();

    }

    public Transform dev_turn_marker;
}
