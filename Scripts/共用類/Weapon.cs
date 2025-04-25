using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using UnityEditor;

public class Weapon : MonoBehaviour
{
    public float damage = 5.0f;
    public bool isCombo3;
    [HideInInspector] public Animator animator;


    [Header("一次性觸發技能")]
    public bool oneHitSkill = false;

    // 用来存储已经被击中的物件
    public HashSet<GameObject> hitGameOject = new HashSet<GameObject>();

    public Collider2D weaponCollider;

    [Header("particleSystem特效")]
    ParticleSystem myParticleSystem;
    [SerializeField] GameObject particleObject;


    public virtual void Start()
    {
        animator = GetComponent<Animator>();
        weaponCollider = GetComponent<Collider2D>();
        if (particleObject)
            myParticleSystem = particleObject.GetComponent<ParticleSystem>();
    }

    public virtual void Update()
    {
        if (!transform)
            return;

        if (particleObject)
        {
            if (!myParticleSystem.IsAlive())
                DestroyWeapon();
        }


    }

    /// <summary>
    /// 示例：開啟碰撞器
    /// </summary>
    public void EnableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    /// <summary>
    /// 示例：關閉碰撞器
    /// </summary>
    public void DisableCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    private void DestroyWeapon()
    {
        Destroy(this.gameObject);

    }

}
