using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected int hp = 1;
    static List<Enemy> _all_enemies = new List<Enemy>();
    public static List<Enemy> all_enemies
    {
        get
        {
            _all_enemies.RemoveAll(delegate (Enemy db) { return db == null; });
            return _all_enemies;
        }
    }

    

    public bool alive
    {
        get { return hp > 0; }
    }
    float knockback_multiplyer = 2f;
    Rigidbody2D rb2d { get { return GetComponent<Rigidbody2D>(); } }
    Coroutine knockback_routine;
    Coroutine shake_routine;
    
    Coroutine Shake()
    {
        if (shake_routine != null)
        {
            StopCoroutine(shake_routine);
        }
        return shake_routine = StartCoroutine(ShakeStep());
    }
    IEnumerator ShakeStep()
    {
        float duration = .2f;
        while ((duration -= Time.deltaTime) > 0f)
        {
            sprite_transform.localPosition = UnityEngine.Random.insideUnitCircle * duration * 2f;
            yield return null;
        }
        sprite_transform.localPosition = Vector2.zero;
    }
    IEnumerator KnockbackStep(Vector2 dir)
    {
        dir.Normalize();
        knockback_multiplyer /= 2f;
        for(int i = 0; i < 3; i++)
        {
            transform.position += dir.Vector3() * Time.deltaTime * 150f * knockback_multiplyer;
            dir /= 2;
            yield return null;
        }
        
        knockback_routine = null;
    }
    
    public bool shielded { get; protected set; }
    
    [SerializeField]
    protected int coins_min,coins_max;
    
    public virtual void Hit(int damage)
    {
        
        if (!shielded)
        {
            hp -= damage;
        }
        Shake();
        if (!alive)
        {
            Destroy(gameObject);
        }
    }
    Transform _sprite_transform;
    Transform sprite_transform { get { return _sprite_transform != null ? _sprite_transform : (_sprite_transform = transform.Find("sprite")); } }
    SpriteRenderer _sr;
    protected SpriteRenderer sr { get { return _sr != null ? _sr : (_sr = sprite_transform.GetComponent<SpriteRenderer>()); } }
    Animator _anim;
    protected Animator anim { get { return _anim != null ? _anim : (_anim = sprite_transform.GetComponent<Animator>()); } }
    protected virtual void Start()
    {
        anim.speed = UnityEngine.Random.Range(.6f, .8f);
        all_enemies.Add(this);
    }


}

