using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlantEnemy : Enemy
{
    protected override void FixedUpdate()
    {
        AnimatorStateInfo baseLayer = animator.GetCurrentAnimatorStateInfo(0);
        if (baseLayer.fullPathHash == Animator.StringToHash("Base Layer.Grow") |
            baseLayer.fullPathHash == Animator.StringToHash("Base Layer.Attack") |
            baseLayer.fullPathHash == Animator.StringToHash("Base Layer.Hurt"))
            return;

        #region 巡邏與追逐

        MoveBehaviour();

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

        UpdateTimer();
    }

    public override void MoveBehaviour()
    {

        if (IsInRange() & !IsInAttackRange() & CheckHasAttack()) //發現目標 不再攻擊距離內
        {
            nextPos = playerGameObject.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
        }
        else if (!IsInRange()) //未發現目標
        {
            if (timer > moveRestTime)
            {
                StartCoroutine(Delay(restTime));
                return;
            }

            if (transform.position.x == PosA.position.x)
            {
                nextPos = PosB.position;
            }
            if (transform.position.x == PosB.position.x)
            {
                nextPos = PosA.position;
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);

        }
        else if (IsInAttackRange() & CheckHasAttack())
        {
            AttackBehaviour();
        }

    }

    public override void AttackBehaviour()
    {
        #region 攻擊

        if (timeSinceLastAttack > timeBetweenAttack)
        {
            timeSinceLastAttack = 0;
            animator.SetTrigger("Attack");
        }
        #endregion
    }

    public override void HurtBehaviour()
    {
        animator.SetTrigger("Hurt");
    }

    IEnumerator Delay(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        timer = 0f;
    }
}
