using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YamlDotNet.RepresentationModel;
public class EnemyManager : MonoBehaviour
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

    List<Enemy> _all_enemies = new List<Enemy>();
    public List<Enemy> alive_enemies
    {
        get { return all_enemies.FindAll(delegate (Enemy en) { return en.alive; }); }
    }

    public List<Enemy> all_enemies
    {
        get
        {
            return _all_enemies;
        }
    }

    public bool any_alive
    {
        get
        {
            return alive_enemies.Count > 0;
        }
    }
    public void AddEnemy(Enemy en)
    {
        _all_enemies.Add(en);
    }
    public void RemoveEnemy(Enemy en)
    {
        _all_enemies.Remove(en);
    }
    
   
    private void Update()
    {
     
    }

    public void KillAll()
    {
        all_enemies.ForEach(delegate (Enemy obj)
        {
            obj.StopAllCoroutines();
            Destroy(obj.gameObject);
        });
        _all_enemies.Clear();
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

}
