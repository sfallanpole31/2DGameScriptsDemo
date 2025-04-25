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

        #region ���޻P�l�v

        MoveBehaviour();

        #endregion

        #region �ĤH�Ϥ���V
        if (transform.position.x < nextPos.x)
        {
            // �V�k����
            spriteRenderer.flipX = initialDirection == Vector3.right ? false : true;
            weapon.transform.rotation = Quaternion.Euler(0, 0, 0);  // �Z���¦V�k��
        }
        else if (transform.position.x > nextPos.x)
        {
            // �V������
            spriteRenderer.flipX = initialDirection == Vector3.right ? true : false;
            weapon.transform.rotation = Quaternion.Euler(0, 180, 0);  // �Z���¦V����
        }
        #endregion

        #region �Q����
        if (knockingBack)
        {

            if (isKnockBackRight == true)
            {
                //�V�k����
                rb.AddForce(new Vector2(knockbackForce * transform.localScale.x, knockbackForce), ForceMode2D.Impulse);
            }
            else
            {
                //�V������
                rb.AddForce(new Vector2(-knockbackForce * transform.localScale.x, knockbackForce), ForceMode2D.Impulse);
            }
            knockingBack = false;
        }
        #endregion

        UpdateTimer();
    }

    public override void MoveBehaviour()
    {

        if (IsInRange() & !IsInAttackRange() & CheckHasAttack()) //�o�{�ؼ� ���A�����Z����
        {
            nextPos = playerGameObject.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
        }
        else if (!IsInRange()) //���o�{�ؼ�
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
        #region ����

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
