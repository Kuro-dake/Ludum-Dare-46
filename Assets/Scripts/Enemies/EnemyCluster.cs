using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;
public abstract class EnemyCluster : MonoBehaviour
{
    /// <summary>
    /// maybe use this for respawning encounters?
    /// </summary>
    float time_spawned;
    
    #region direction_vectors
    public static Dictionary<direction, Vector2> direction_bound_positions = new Dictionary<direction, Vector2>() {
        {direction.up, Vector2.up + Vector2.right / 2f },
        {direction.down, Vector2.right / 2f },
        {direction.left, Vector2.up / 2f },
        {direction.right, Vector2.right + Vector2.up / 2f },
        {direction.none, Vector2.up /2f + Vector2.right / 2f }
    };

    
    public static Vector2 DirectionToScreenBound(direction d, Camera cam = null)
    {
        Vector2 bound = direction_bound_positions[d];
        bound.x *= Screen.width;
        bound.y *= Screen.height;
        cam = cam == null ? Camera.main : cam;
        return cam.ScreenToWorldPoint(bound);
    }
    public static Vector2 screen_center
    {
        get { return Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2f, Screen.height / 2f)); }
    }
    protected virtual void Start()
    {
        transform.localPosition = Vector2.zero;
    }
    public class CornerCoordinates
    {
        public Vector2 this[direction d, direction h]{
            get{
                if(d != direction.left && d != direction.right || h != direction.up & h != direction.down)
                {
                    throw new UnityException("Invalid coordinates: " + d.ToString() + ", " + h.ToString());
                }
                return Camera.main.ScreenToWorldPoint(new Vector2(d == direction.right ? Screen.width : 0, h == direction.up ? Screen.height : 0));
            }
        }
            
    }
    static CornerCoordinates _screen_corners = new CornerCoordinates();
    public static CornerCoordinates screen_corners
    {
        get
        {
            return _screen_corners;
        }
    }
    #endregion 
    protected direction dir;
    protected bool fixed_dir = false;
    protected IntRange number = new IntRange(10,20);
    public virtual void Spawn(direction _dir = direction.none) { }

    
    /// <summary>
    /// set or get a yaml node for this bird cluster, and set it up
    /// </summary>
    /// <param name="source"></param>
    public virtual void LoadParameters(YamlMappingNode source)
    {
        try
        {
            skip_cluster = source.GetBool("skip");
        }
        catch (KeyNotFoundException) { }
        try
        {
            int[] num = source.GetIntArray("number");
            number = new IntRange(num);
        }
        catch (KeyNotFoundException) { }
    }
    bool skip_cluster = false;
    public virtual bool active { get {
            cluster_enemies.RemoveAll(delegate(Enemy e) { return e == null; });
            return !skip_cluster && cluster_enemies.Count > 0; 
        } }
    protected List<Enemy> cluster_enemies = new List<Enemy>();
    protected Enemy GenerateEnemy(string type, Vector2 pos)
    {
        Enemy ret = GM.characters.GenerateEnemy(type, pos);
        cluster_enemies.Add(ret);
        return ret;
    }
    
}
