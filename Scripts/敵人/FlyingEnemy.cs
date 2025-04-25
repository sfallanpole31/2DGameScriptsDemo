using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class FlyingEnemy : Enemy
{
    [Header("���W��������")]
    [SerializeField] float ascentHeight = 3f;

    [Header("�W���t��")]
    [SerializeField] float ascentSpeed = 1f;

    [Header("���ĳt��")]
    [SerializeField] float divingSpeed = 1f;

    [SerializeField] bool isAscending = false;//�O�_�b�W�B���A
    [SerializeField] bool isDiving = false;//�O�_����
    private Vector2 ascentTarget;//�W�ɥؼ�
    private Vector2 divingTarget;//���ĥؼ�

    [SerializeField]


    protected override void FixedUpdate()
    {
        #region �p�G��e����Hurt�ʵe return
        AnimatorStateInfo baseLayer = animator.GetCurrentAnimatorStateInfo(0);
        if (baseLayer.fullPathHash == Animator.StringToHash("Base Layer.Eagle-hurt-Animation"))
        {
            return;
        }
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



        #region ���޻P�l�v

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
        if (IsInRange() & !IsInAttackRange() & CheckHasAttack()) //�o�{�ؼ� ���A�����Z����
        {

            nextPos = playerGameObject.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
            animator.SetBool("IsAttack", false);
        }
        else if (!IsInRange()) //���o�{�ؼ�
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
            weapon.hitGameOject = new HashSet<GameObject>();//�N�Z�������L���ؼвM��
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
    /// �W�B�ʵe
    /// </summary>
    void AscendingBehaviour()
    {

        transform.position = Vector2.MoveTowards(transform.position, ascentTarget, speed * ascentSpeed * Time.fixedDeltaTime);


        // �ˬd�O�_��F�ؼа���
        if (Vector2.Distance(transform.position, ascentTarget) < 0.1f)
        {

            isAscending = false;
            isDiving = true; // �i�J���Ķ��q
            animator.SetBool("IsAttack", true);
        }

    }


    /// <summary>
    /// ���İʵe
    /// </summary>
    private IEnumerator DivingBehaviour()
    {



        // �ˬd�O�_��F���a��m
        if (Vector2.Distance(transform.position, divingTarget) < 0.1f)
        {
            animator.SetBool("IsAttack", false); // ��������ʵe
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
