using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    float time;
    float top_y = .5f;
    public float orig_y;
    float speed = 10f;
    private void Start()
    {
        orig_y = transform.localPosition.y;
        //time = Random.Range(0f, Mathf.PI);
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.localPosition = Vector2.up * (Mathf.Lerp(0f, top_y, (1f+Mathf.Sin(time * speed))*.5f) + orig_y);
    }
}
