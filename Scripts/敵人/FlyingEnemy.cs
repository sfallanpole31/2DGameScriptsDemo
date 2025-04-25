using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FlyingEnemy : Enemy
{
    [Header("往上飛的高度")]
    [SerializeField] float ascentHeight = 3f;

    [Header("上飛速度")]
    [SerializeField] float ascentSpeed = 1f;

    [Header("俯衝速度")]
    [SerializeField] float divingSpeed = 1f;

    [SerializeField] bool isAscending = false;//是否在上浮狀態
    [SerializeField] bool isDiving = false;//是否俯衝
    private Vector2 ascentTarget;//上升目標
    private Vector2 divingTarget;//俯衝目標

    [SerializeField]


    protected override void FixedUpdate()
    {
        #region 如果當前播放Hurt動畫 return
        AnimatorStateInfo baseLayer = animator.GetCurrentAnimatorStateInfo(0);
        if (baseLayer.fullPathHash == Animator.StringToHash("Base Layer.Eagle-hurt-Animation"))
        {
            return;
        }
        #endregion

        #region 敵人圖片轉向
        if (transform.position.x < nextPos.x)
        {
            // 向右移動
            spriteRenderer.flipX = initialDirection == Vector3.right ? false : true;
            weapon.transform.rotation = Quaternion.Euler(0, 0, 0);  // 武器朝向右側
        }
        else if (transform.position.x > nextPos.x)
        {
            // 向左移動
            spriteRenderer.flipX = initialDirection == Vector3.right ? true : false;
            weapon.transform.rotation = Quaternion.Euler(0, 180, 0);  // 武器朝向左側
        }
        #endregion

        #region 被擊飛
        if (knockingBack)
        {

            if (isKnockBackRight == true)
            {
                //向右擊飛
                rb.AddForce(new Vector2(knockbackForce * transform.localScale.x, knockbackForce), ForceMode2D.Impulse);
            }
            else
            {
                //向左擊飛
                rb.AddForce(new Vector2(-knockbackForce * transform.localScale.x, knockbackForce), ForceMode2D.Impulse);
            }
            knockingBack = false;
        }
        #endregion



        #region 巡邏與追逐

        if (isAscending == true | isDiving == true)
        {

            AttackBehaviour();
        }
        else
        {

            MoveBehaviour();
        }

        #endregion

        UpdateTimer();
    }

    public override void MoveBehaviour()
    {
        if (IsInRange() & !IsInAttackRange() & CheckHasAttack()) //發現目標 不再攻擊距離內
        {

            nextPos = playerGameObject.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
            animator.SetBool("IsAttack", false);
        }
        else if (!IsInRange()) //未發現目標
        {

            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(PosA.position.x, PosA.position.y)) < 0.1f)
            {
                nextPos = PosB.position;
            }
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(PosB.position.x, PosB.position.y)) < 0.1f)
            {
                nextPos = PosA.position;
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
            animator.SetBool("IsAttack", false);
        }
        else if (IsInAttackRange() & CheckHasAttack() & !isAscending & !isDiving)
        {
            ascentTarget = new Vector2(transform.position.x, transform.position.y + ascentHeight);
            divingTarget = new Vector2(playerGameObject.transform.position.x, playerGameObject.transform.position.y);
            weapon.hitGameOject = new HashSet<GameObject>();//將武器攻擊過的目標清空
            isAscending = true;


        }

    }


    public override void AttackBehaviour()
    {
        nextPos = playerGameObject.transform.position;
        if (isAscending & !isDiving)
        {
            AscendingBehaviour();
        }
        else if (isDiving & !isAscending)
        {
            StartCoroutine(DivingBehaviour());
        }
    }

    /// <summary>
    /// 上浮動畫
    /// </summary>
    void AscendingBehaviour()
    {

        transform.position = Vector2.MoveTowards(transform.position, ascentTarget, speed * ascentSpeed * Time.fixedDeltaTime);


        // 檢查是否到達目標高度
        if (Vector2.Distance(transform.position, ascentTarget) < 0.1f)
        {

            isAscending = false;
            isDiving = true; // 進入俯衝階段
            animator.SetBool("IsAttack", true);
        }

    }


    /// <summary>
    /// 俯衝動畫
    /// </summary>
    private IEnumerator DivingBehaviour()
    {



        // 檢查是否到達玩家位置
        if (Vector2.Distance(transform.position, divingTarget) < 0.1f)
        {
            animator.SetBool("IsAttack", false); // 停止攻擊動畫
            weapon.DisableCollider();
            yield return new WaitForSecondsRealtime(2f);
            isDiving = false;


        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, divingTarget, speed * divingSpeed * Time.fixedDeltaTime);
        }
    }


    //public override void HurtBehaviour()
    //{
    //    animator.SetBool("IsHurt", true);
    //}

    void HurtFinish()
    {
        animator.SetBool("IsHurt", false);
    }
}
