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
    
    
    
    public static bool pause { get { return Time.timeScale == 0f; } set { Time.timeScale = value ? 0f : 1f; } }

    
    
    public void UpdateDevout()
    {
        if (GM.dev_options.hide_dev_text_output)
        {
            GM.devout.text = "";
            return;
        }
        GM.devout.text = "";
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
        

    }

    public void LoadLevelParams(int level)
    {
        YamlMappingNode level_node = Setup.GetFile("level" + level.ToString());
        level_node.GetNode<YamlMappingNode>("base_settings");


    }
    Dictionary<string, Queue<EnemyCluster>> cluster_queues = new Dictionary<string, Queue<EnemyCluster>>();
    public void LoadLevel(int level)
    {
        YamlMappingNode level_node = Setup.GetFile("level" + level.ToString());
        foreach (KeyValuePair<YamlNode, YamlNode> queue in level_node.GetNode<YamlMappingNode>("cluster_queues").Children)
        {
            string queue_name = queue.Key.ToString();
            Queue<EnemyCluster> qbc = new Queue<EnemyCluster>();
            
            YamlSequenceNode ymn = (YamlSequenceNode)queue.Value;
            foreach (YamlMappingNode mn in ymn)
            {
                if(mn.Get("type") == "stop")
                {
                    break;
                }
                EnemyCluster cluster = Instantiate(GM.enemies.clusters[mn.Get("type")]);
                cluster.LoadParameters(mn);
                qbc.Enqueue(cluster);
            }
            
            cluster_queues.Add(queue_name, qbc);

            


        }
       
    }
    Coroutine play_cluster_routine;
    public void PlayCluster(string cluster_name)
    {
        play_cluster_routine = StartCoroutine(PlayClusterStep(cluster_name));
    }
    public EnemyCluster current_cluster_queue;
    IEnumerator PlayClusterStep(string cluster_name)
    {
        Queue<EnemyCluster> play = new Queue<EnemyCluster>(cluster_queues[cluster_name]);

        while(play.Count > 0)
        {
            current_cluster_queue = play.Dequeue();
            current_cluster_queue.Spawn();
            while (current_cluster_queue.active)
            {
                yield return null;
            }
            Destroy(current_cluster_queue);
            
        }
        current_cluster_queue = null;
    }

    public YAMLParams level_gen_params;
    

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
}
