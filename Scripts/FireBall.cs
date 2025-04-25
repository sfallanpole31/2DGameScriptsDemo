using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Weapon
{

    [Header("火球")]
    [Header("技能落下")]
    public Vector3 shootPoint; //起始位置
    public Vector3 endPoint; //結束位置
    public float fireBallSpeed = 5f;

    AudioSource audioSource;
    public AudioClip FireBallFallClip;
    public AudioClip FireBallHitClip;
    public bool endTrigger = false;
    SpriteRenderer fireballRenderer;


    public override void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(FireBallFallClip);
        animator = GetComponent<Animator>();
        weaponCollider = GetComponent<Collider2D>();
        fireballRenderer = GetComponent<SpriteRenderer>();
    }

    //技能落下
    public override void Update()
    {
        #region 敵人圖片轉向
        if (shootPoint.x < endPoint.x)
        {
            // 向右移動
            fireballRenderer.flipX  = false;
        }
        else 
        {
            // 向左移動
            fireballRenderer.flipX = true;

        }
        #endregion

        // Check if the fireball is near the end point

        if (Vector3.Distance(transform.position, endPoint) > 0.1f) // Using squared distance for optimization
        {
            // Move towards the end point
            transform.position = Vector3.MoveTowards(transform.position, endPoint, fireBallSpeed * Time.deltaTime);
        }
        else
        {
            // Trigger finish animation and play sound once
            AnimatorStateInfo baseLayer = animator.GetCurrentAnimatorStateInfo(0);
            if (baseLayer.fullPathHash != Animator.StringToHash("Base.Layer.FireBallBlast") & endTrigger == false)
            {
                endTrigger = true;
                animator.SetTrigger("finish");
                audioSource.PlayOneShot(FireBallHitClip);
                //// Start destruction coroutine
                //StartCoroutine(DestroyFireBall(2f));
            }
        }

    }

    IEnumerator DestroyFireBall(float DelayTime)
    {
        yield return new WaitForSeconds(DelayTime);
        Destroy(this.gameObject);
    }
}
