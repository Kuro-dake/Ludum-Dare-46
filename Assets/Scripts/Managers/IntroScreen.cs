using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class IntroScreen : MonoBehaviour
{
    [SerializeField]
    GameObject game = null;
    [SerializeField]
    bool skip_intro = false;
    [SerializeField]
    AudioManager sounds = null;
    Transform canvas { get { return transform.Find("Canvas"); } }
    Image logo { get { return transform.Find("Canvas").Find("logo").GetComponent<Image>(); } }
    private void Start()
    {
        if (skip_intro)
        {
            StartGame();
        }
        else
        {
            foreach (string s in new string[] { "left_char", "right_char", "Title" })
            {
                orig_poses.Add(s, canvas.Find(s).position);
            }
            game.SetActive(false);
            ShowHide(true);
            
        }
    }
    IEnumerator StartIntro()
    {
        yield return new WaitForSeconds(.5f);
        Intro();
    }
    [SerializeField]
    float intro_step_duration = 2f;
    Coroutine intro_routine, show_logo_routine;
    void ShowHide(bool show)
    {
        if(intro_routine != null)
        {
            StopCoroutine(intro_routine);
        }
        intro_routine = StartCoroutine(Intro(show));
        if (show_logo_routine != null)
        {
            StopCoroutine(show_logo_routine);
        }
        show_logo_routine = StartCoroutine(ShowLogo(show));

    }
    IEnumerator ShowLogo(bool show = true)
    {
        float target = show ? 1f : 0f;
        Color c = logo.color;
        while(!Mathf.Approximately(c.a, target))
        {
            c.a = Mathf.MoveTowards(c.a, target, Time.deltaTime);
            logo.color = c;
            yield return null;
        }
        show_logo_routine = null;
    }
    Dictionary<string, Vector2> orig_poses = new Dictionary<string, Vector2>();
    bool revealed = false;
    
    IEnumerator Intro(bool show = true)
    {
        
        float multiplier = 1f / intro_step_duration;

        float current = show ? 0f : 1f;
        float target = show ? 1f : 0f;
        SortedDictionary<string, string> elements = new SortedDictionary<string, string>()
        {
            {"left_char", "left_anchor" }, {"right_char", "right_anchor" }, {"Title", "text_anchor"}
        };
        if (!show) {
            elements.Clear();
            elements = new SortedDictionary<string, string>()
            {
                {"Title", "text_anchor"}, {"right_char", "right_anchor" }, {"left_char", "left_anchor" }
            };
        }
        foreach (KeyValuePair<string, string> kv in elements)
        {
            current = show ? 0f : 1f;
            Vector2 orig_pos = orig_poses[kv.Key];
            Vector2 target_pos = canvas.Find(kv.Value).position;
            Transform current_transform = canvas.Find(kv.Key);
           
            while (!Mathf.Approximately(current = Mathf.MoveTowards(current, target, Time.deltaTime * multiplier), target))
            {
                current_transform.position = Vector2.Lerp(orig_pos, target_pos, current);
                yield return null;
            }
            sounds.PlaySound("hit");
        }
        
        canvas.Find("InsertCoin").gameObject.SetActive(show);
        revealed = show;
        intro_routine = null;
        if (!revealed)
        {
            StartGame();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowHide(!revealed);
            //Application.LoadLevel(Application.loadedLevel);
            //StartGame();
        }
    }

    void StartGame()
    {
        game.SetActive(true);
        GM gm = game.GetComponentInChildren<GM>(true);
        List<Transform> game_children = new List<Transform>();
        for (int i = 0; i < game.transform.childCount; i++)
        {
            game_children.Add(game.transform.GetChild(i));
        }
        
        gm.Initialize();
    }
}
