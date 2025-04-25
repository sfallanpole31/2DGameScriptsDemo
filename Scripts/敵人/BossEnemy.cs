using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    /// <summary>
    /// 地刺位置
    /// </summary>
    public Transform[] trapTransforms;

    /// <summary>
    /// 地刺數量
    /// </summary>
    [SerializeField] int trapCount;

    /// <summary>
    /// 植物數量
    /// </summary>
    [SerializeField] int plantCount;

    /// <summary>
    /// 地刺prefab
    /// </summary>
    [Header("地刺prefab")]
    [SerializeField] Weapon groundTrapPrefab;


    /// <summary>
    /// 地刺施放Timer
    /// </summary>
    [Header("地刺施放Timer")]
    float trapAttacktimer = 0;

    /// <summary>
    /// 地刺施放所需時間
    /// </summary>
    [Header("地刺施放所需時間")]
    [SerializeField] float trapCastTime = 20f;

    [Header("植物怪Prefab")]
    [SerializeField] GameObject plantEnemyPrefab;

    public override void MoveBehaviour()
    {



        //不在範圍內巡邏
        if (!IsInRange() & canCheck()) //未發現目標
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
        //在追逐範圍內 靠近 或是 施放遠攻
        else if (IsInRange() & !IsInAttackRange() & canCheck()) //發現目標 不再攻擊距離內
        {

            if (trapAttacktimer >= trapCastTime)
            {
                bool choice = UnityEngine.Random.Range(0, 2) == 0; // 使用 Random.Range 產生隨機值

                if (choice)
                {
                    //召喚地刺
                    nextPos = playerGameObject.transform.position;
                    StartCoroutine(CastingTrap(2f));
                    trapAttacktimer = 0;
                    animator.SetTrigger("LongAttack");
                }
                else
                {
                    //召喚植物
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
        //在攻擊範圍內 直接使用普通攻擊
        else if (IsInAttackRange() & timeSinceLastAttack > timeBetweenAttack)
        {
            nextPos = playerGameObject.transform.position;
            timeSinceLastAttack = 0;
            animator.SetTrigger("CloseAttack");
        }



        //if (IsInRange() & !IsInAttackRange() & CheckHasAttack()) //發現目標 不再攻擊距離內
        //{
        //    nextPos = playerGameObject.transform.position;
        //    transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
        //    animator.SetBool("IsAttack", false);
        //    animator.SetBool("IsStand", false);
        //    animator.SetBool("IsMove", true);
        //}
        //else if (!IsInRange()) //未發現目標
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
        // 確保數組足夠長
        if (posArray.Length < TrapCount)
        {
            Debug.LogError("數組長度小於選取數量！");
            return new Transform[0];
        }

        // Fisher-Yates 洗牌算法
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
            DelayedAction(2f, () => Destroy(enemyPack)); // 1秒後摧毀
            FindObjectOfType<GameManager>().WinPanelShow();
        }
    }

    /// <summary>
    /// 施放地刺
    /// </summary>
    /// <param name="delayTime">播後動畫後延遲時間</param>
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
    /// 施放植物
    /// </summary>
    /// <param name="delayTime">播後動畫後延遲時間</param>
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
        action?.Invoke(); // 執行傳入的動作
    }


}
