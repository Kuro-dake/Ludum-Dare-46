using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CineCam : MonoBehaviour
{
    // Start is called before the first frame update
    FloatRange delay_range = new FloatRange(4f, 8f);
    FloatRange movement_delay_range = new FloatRange(4f, 6f);
    FloatRange modifier_range = new FloatRange(0f, 5f);
    [SerializeField]
    public Vector2 cam_offset { get { return GM.cine_cam.transform.position - GM.party.transform.position; } }
    float random_movement_multiplier = .01f;
    public Transform target { set { cvcam.m_Follow = value; } }
    //FloatRange pos_mod_x_range = new FloatRange()
    void Start()
    {
        screenX = frame.m_ScreenX;
        screenY = frame.m_ScreenY;
        //ZoomTowards(modifier_range.random, delay_range);
        MoveTowards(Random.insideUnitCircle * random_movement_multiplier, movement_delay_range);
        //StartCoroutine(RotateCam());
    }
    [SerializeField]
    float _ortographicSize = 40f;
    public float ortographicSize { get { return _ortographicSize; } set { 
            _ortographicSize = value;
           
        } }
    IEnumerator RotateCam()
    {
        while (true)
        {
            Quaternion new_angle = Quaternion.Euler(0f, 0f, Random.Range(-2f, 2f));
            float angle = Quaternion.Angle(transform.localRotation, new_angle);
            float orig_angle = angle;
            while((angle = Quaternion.Angle(transform.localRotation, new_angle)) > .1f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, new_angle, Time.deltaTime * angle / orig_angle);
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(2f, 4f));
        }
    }
    // Update is called once per frame
    public float paralax_div = 2f;
    public bool move_with_mouse = false;
    void Update()
    {
        Vector3 relpos = (Input.mousePosition * 2 - new Vector3(Screen.width, Screen.height)) / 100f;
        float distance = Vector2.Distance(Vector2.zero, relpos);
        //GetComponent<Camera>().orthographicSize = 13 + distance;
        if (move_with_mouse)
        {
            Vector3 pos = Vector3.MoveTowards(transform.position, relpos, Mathf.Clamp((distance * .2f) * Time.deltaTime, 1f * Time.deltaTime, Mathf.Infinity));
            pos.z = -10f;
            transform.position = pos;
            GameObject.Find("pipes").transform.position = transform.position / paralax_div;
        }
        float multiplyer = Time.deltaTime * 15f;
        cvcam.m_Lens.OrthographicSize = ortographicSize + modifier;
        frame.m_ScreenX = screenX + pos_mod.x;
        frame.m_ScreenY = screenY + pos_mod.y;
    }
    Coroutine moveroutine = null;
    void MoveTowards(Vector2 pos, float duration)
    {
        if (moveroutine != null)
        {
            StopCoroutine(moveroutine);
        }
        moveroutine = StartCoroutine(MoveTowardsStep(pos, duration));
    }
    CinemachineVirtualCamera _cvcam;
    public CinemachineVirtualCamera cvcam { get { return _cvcam != null ? _cvcam : (_cvcam = GetComponent<CinemachineVirtualCamera>()); } }
    CinemachineFramingTransposer _frame;
    public CinemachineFramingTransposer frame { get { return _frame != null ? _frame : (_frame = cvcam.GetCinemachineComponent<CinemachineFramingTransposer>()); } }
    public float screenX, screenY;
    Vector2 pos_mod = Vector2.zero;
    IEnumerator MoveTowardsStep(Vector2 pos, float duration = 1f)
    {
        float duration_inv = 1f / duration;
        Vector2 orig_pos = pos_mod;
        float current = 0f;
        while((current += Time.deltaTime) < duration)
        {
            pos_mod = Vector2.Lerp(orig_pos, pos, current * duration_inv);
            yield return null;
        }
        //yield return new WaitForSeconds(Random.Range(2f, 4f));
        MoveTowards(Random.insideUnitCircle * random_movement_multiplier, movement_delay_range);

    }
    Coroutine zoomroutine = null;
    void ZoomTowards(float size, float duration)
    {
        if(zoomroutine != null)
        {
            StopCoroutine(zoomroutine);
        }
        zoomroutine = StartCoroutine(ZoomTowardsStep(size, duration));
        
    }
    float modifier = 0f;
    IEnumerator ZoomTowardsStep(float nsize, float duration)
    {
        
        float orig_size = modifier;
        float start_time = Time.time;
        float duration_inverted = 1f / duration;
        while(!Mathf.Approximately(modifier, nsize))
        {
            float t = (Time.time - start_time) * duration_inverted / duration;

            modifier = Mathf.SmoothStep(modifier, nsize, t);
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(2f, 4f));

        zoomroutine = null;

        ZoomTowards(modifier_range.random, delay_range);
    }
        
}
