using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Weapon
{

    [Header("���y")]
    [Header("�ޯศ�U")]
    public Vector3 shootPoint; //�_�l��m
    public Vector3 endPoint; //������m
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

    //�ޯศ�U
    public override void Update()
    {
        #region �ĤH�Ϥ���V
        if (shootPoint.x < endPoint.x)
        {
            // �V�k����
            fireballRenderer.flipX  = false;
        }
        else 
        {
            // �V������
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
