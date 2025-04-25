using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    /// <summary>
    /// �a���m
    /// </summary>
    public Transform[] trapTransforms;

    /// <summary>
    /// �a��ƶq
    /// </summary>
    [SerializeField] int trapCount;

    /// <summary>
    /// �Ӫ��ƶq
    /// </summary>
    [SerializeField] int plantCount;

    /// <summary>
    /// �a��prefab
    /// </summary>
    [Header("�a��prefab")]
    [SerializeField] Weapon groundTrapPrefab;


    /// <summary>
    /// �a��I��Timer
    /// </summary>
    [Header("�a��I��Timer")]
    float trapAttacktimer = 0;

    /// <summary>
    /// �a��I��һݮɶ�
    /// </summary>
    [Header("�a��I��һݮɶ�")]
    [SerializeField] float trapCastTime = 20f;

    [Header("�Ӫ���Prefab")]
    [SerializeField] GameObject plantEnemyPrefab;

    public override void MoveBehaviour()
    {



        //���b�d�򤺨���
        if (!IsInRange() & canCheck()) //���o�{�ؼ�
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
        //�b�l�v�d�� �a�� �άO �I�񻷧�
        else if (IsInRange() & !IsInAttackRange() & canCheck()) //�o�{�ؼ� ���A�����Z����
        {

            if (trapAttacktimer >= trapCastTime)
            {
                bool choice = UnityEngine.Random.Range(0, 2) == 0; // �ϥ� Random.Range �����H����

                if (choice)
                {
                    //�l��a��
                    nextPos = playerGameObject.transform.position;
                    StartCoroutine(CastingTrap(2f));
                    trapAttacktimer = 0;
                    animator.SetTrigger("LongAttack");
                }
                else
                {
                    //�l��Ӫ�
                    nextPos = playerGameObject.transform.position;
                    StartCoroutine(CastingPlant(2f));
                    animator.SetTrigger("LongAttack");
                    trapAttacktimer = 0;
                }

            }
            else
            {
                nextPos = playerGameObject.transform.position;
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
            }


        }
        //�b�����d�� �����ϥδ��q����
        else if (IsInAttackRange() & timeSinceLastAttack > timeBetweenAttack)
        {
            nextPos = playerGameObject.transform.position;
            timeSinceLastAttack = 0;
            animator.SetTrigger("CloseAttack");
        }



        //if (IsInRange() & !IsInAttackRange() & CheckHasAttack()) //�o�{�ؼ� ���A�����Z����
        //{
        //    nextPos = playerGameObject.transform.position;
        //    transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
        //    animator.SetBool("IsAttack", false);
        //    animator.SetBool("IsStand", false);
        //    animator.SetBool("IsMove", true);
        //}
        //else if (!IsInRange()) //���o�{�ؼ�
        //{
        //    if (transform.position.x == PosA.position.x)
        //    {
        //        nextPos = PosB.position;
        //    }
        //    if (transform.position.x == PosB.position.x)
        //    {
        //        nextPos = PosA.position;
        //    }
        //    transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
        //    animator.SetBool("IsAttack", false);
        //    animator.SetBool("IsStand", false);
        //    animator.SetBool("IsMove", true);
        //}
        //else if (IsInAttackRange() & CheckHasAttack())
        //{
        //    AttackBehaviour();
        //}
        trapAttacktimer += Time.deltaTime;
    }


    private Transform[] SelectRandomTransform(Transform[] posArray, int TrapCount)
    {
        // �T�O�Ʋը�����
        if (posArray.Length < TrapCount)
        {
            Debug.LogError("�Ʋժ��פp�����ƶq�I");
            return new Transform[0];
        }

        // Fisher-Yates �~�P��k
        for (int i = posArray.Length - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            Transform temp = posArray[i];
            posArray[i] = posArray[randomIndex];
            posArray[randomIndex] = temp;
        }

        Transform[] transArray = new Transform[TrapCount];

        for (int i = 0; i < TrapCount; i++)
        {
            transArray[i] = posArray[i];
        }

        return transArray;


    }

    bool canCheck()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Boss_idle"))
            return true;
        else
            return false;

    }

    void GroundTrapAttack()
    {

    }

    void CloseAttack()
    {

    }

    void MagicAttack()
    {

    }

    public override void GetHurt(float damage)
    {
        //knockingBack = true;
        //cam.Play("Cam_Shake");
        HP -= damage;

        DamagePopup.Create(transform.position, damage);
        animator.SetTrigger("Hurt");
        if (HP <= 0)
        {
            animator.Play("Boss_death");
            FindObjectOfType<AudioSet>().PlaySfX(1);
            DelayedAction(2f, () => Destroy(enemyPack)); // 1���R��
            FindObjectOfType<GameManager>().WinPanelShow();
        }
    }

    /// <summary>
    /// �I��a��
    /// </summary>
    /// <param name="delayTime">����ʵe�᩵��ɶ�</param>
    /// <returns></returns>
    IEnumerator CastingTrap(float delayTime)
    {
        Transform[] randomTransform = SelectRandomTransform(trapTransforms, trapCount);

        for (int i = 0; i < trapCount; i++)
        {
            Instantiate(GameManager.WarningSign, randomTransform[i].position, GameManager.WarningSign.transform.rotation);
        }

        yield return new WaitForSecondsRealtime(delayTime);


        for (int i = 0; i < trapCount; i++)
        {
            Instantiate(groundTrapPrefab, randomTransform[i].position, groundTrapPrefab.transform.rotation);
        }

    }

    /// <summary>
    /// �I��Ӫ�
    /// </summary>
    /// <param name="delayTime">����ʵe�᩵��ɶ�</param>
    /// <returns></returns>
    IEnumerator CastingPlant(float delayTime)
    {
        Transform[] randomTransform = SelectRandomTransform(trapTransforms, plantCount);

        //for (int i = 0; i < plantCount; i++)
        //{
        //    Instantiate(GameManager.WarningSign, randomTransform[i].position, GameManager.WarningSign.transform.rotation);
        //}

        yield return new WaitForSecondsRealtime(delayTime);


        for (int i = 0; i < plantCount; i++)
        {
            Instantiate(plantEnemyPrefab, randomTransform[i].position, plantEnemyPrefab.transform.rotation);
        }



    }

    IEnumerator Delay(float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        timer = 0f;
    }

    public void DelayedAction(float delay, Action action)
    {
        StartCoroutine(ExecuteAfterDelay(delay, action));
    }

    private IEnumerator ExecuteAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke(); // ����ǤJ���ʧ@
    }


}
