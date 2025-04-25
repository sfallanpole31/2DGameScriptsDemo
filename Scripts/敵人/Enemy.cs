using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyPack;
    public Transform PosA, PosB;
    public float speed;
    [HideInInspector] public Vector2 nextPos;
    public float maxHP = 50;
    [HideInInspector] public float HP;

    [HideInInspector] public Animator cam;
    [HideInInspector] public Animator animator;

    //擊飛
    [HideInInspector] public Rigidbody2D rb;
    public float knockbackForce;  // 力的大小
    public bool knockingBack;
    public bool isKnockBackRight;

    //移動計時器
    public float timer = 0f;
    [Header("走多久休息")]
    public float moveRestTime = 10f;
    [Header("休息多久")]
    public float restTime = 2f;

    [Header("怪物圖片")]
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Vector3 initialDirection;  // 初始面向方向

    [Header("追逐距離")]
    [SerializeField] float chaseDistance = 10f;

    [Header("攻擊距離")]
    [SerializeField] float attackDistance = 2f;

    [Header("攻擊目標")]
    [HideInInspector] public GameObject playerGameObject;

    [Header("攻擊時間間距")]
    public float timeBetweenAttack = 2f;
    [HideInInspector] public float timeSinceLastAttack = 0f;

    [Header("武器物件")]
    public Weapon weapon;

    [Header("武器Collider")]
    private BoxCollider2D boxCollider;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = weapon.transform.GetComponent<BoxCollider2D>();
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        float number = Random.Range(0, 2);
        HP = maxHP;
        animator = GetComponent<Animator>();
        if (number == 0)
        {
            nextPos = PosA.position;
        }
        else
        {
            nextPos = PosB.position;
        }

        playerGameObject = GameObject.Find("Player");
        cam = GameObject.Find("Main Camera").GetComponent<Animator>();

        // 設定初始朝向
        // 假設你可以在編輯器中設定或根據圖片朝向初始化
        initialDirection = spriteRenderer.flipX ? Vector3.left : Vector3.right;

    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
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

    public virtual void MoveBehaviour()
    {

        if (IsInRange() & !IsInAttackRange() & CheckHasAttack()) //發現目標 不再攻擊距離內
        {
            nextPos = playerGameObject.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
            animator.SetBool("IsAttack", false);
            animator.SetBool("IsStand", false);
            animator.SetBool("IsMove", true);
        }
        else if (!IsInRange()) //未發現目標
        {
            if (transform.position.x == PosA.position.x)
            {
                nextPos = PosB.position;
            }
            if (transform.position.x == PosB.position.x)
            {
                nextPos = PosA.position;
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(nextPos.x, transform.position.y), speed * Time.deltaTime);
            animator.SetBool("IsAttack", false);
            animator.SetBool("IsStand", false);
            animator.SetBool("IsMove", true);
        }
        else if (IsInAttackRange() & CheckHasAttack())
        {
            AttackBehaviour();
        }

    }

    void AttackFinish()
    {
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsStand", true);
        animator.SetBool("IsMove", false);
    }

    /// <summary>
    /// 檢查是否在播放攻擊動畫
    /// </summary>
    /// <returns></returns>
    public bool CheckHasAttack()
    {
        AnimatorStateInfo baseLayer = animator.GetCurrentAnimatorStateInfo(0);
        if (baseLayer.fullPathHash == Animator.StringToHash("Base Layer.Enemy Attack"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //當有觸發器碰到此物件
    {
        if (collision.CompareTag("Weapon"))
        {

            #region 承受傷害
            if (!collision.GetComponent<Weapon>().hitGameOject.Contains(gameObject)) //若被觸發過不再觸發
            {
                print("被擊中");
                float dmg = collision.GetComponent<Weapon>().damage;
                GetHurt(dmg);

                #region 如果是一次性觸發技能 Trigger設True
                if (collision.GetComponent<Weapon>().oneHitSkill)
                {
                    collision.GetComponent<Weapon>().hitGameOject.Add(gameObject);
                }
                #endregion

            }
            #endregion


            #region 被擊飛
            if (collision.GetComponent<Weapon>().isCombo3 | collision.GetComponent<FireBall>())
            {

                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                knockingBack = true;
                // 判斷碰撞發生在左側還是右側
                if (collision.transform.position.x < transform.position.x)
                {
                    isKnockBackRight = true;
                }
                else if (collision.transform.position.x > transform.position.x)
                {
                    isKnockBackRight = false;
                }
                collision.GetComponent<Weapon>().isCombo3 = false;
            }
            #endregion

            HurtBehaviour();
        }
    }

    public virtual void AttackBehaviour()
    {
        #region 攻擊

        if (timeSinceLastAttack > timeBetweenAttack)
        {
            timeSinceLastAttack = 0;
            animator.SetBool("IsAttack", true);
            animator.SetBool("IsStand", true);
            animator.SetBool("IsMove", false);
        }
        #endregion
    }

    public virtual void HurtBehaviour()
    {

    }

    public void UpdateTimer()
    {
        timeSinceLastAttack += Time.deltaTime;
        timer += Time.deltaTime;
    }

    public virtual void GetHurt(float damage)
    {
        //knockingBack = true;
        cam.Play("Cam_Shake");
        HP -= damage;

        DamagePopup.Create(transform.position, damage);
        if (HP <= 0)
        {
            FindObjectOfType<AudioSet>().PlaySfX(1);
            Destroy(enemyPack);
        }
    }

    /// <summary>
    /// 是否在追趕距離內
    /// </summary>
    /// <returns></returns>
    public bool IsInRange()
    {
        if (playerGameObject)
            return Vector2.Distance(transform.position, playerGameObject.transform.position) < chaseDistance;
        else
            return false;
    }

    /// <summary>
    /// 是否在攻擊距離內
    /// </summary>
    /// <returns></returns>
    public bool IsInAttackRange()
    {
        if (playerGameObject)
            return Vector2.Distance(transform.position, playerGameObject.transform.position) < attackDistance;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        //追逐距離顯示
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        //攻擊距離顯示
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        //{
        //    print("在地上");
        //    isGrounded = true;
        //    // 锁定敌对单位的 X 轴位置
        //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //}
        //else
        //{
        //    print("不在地上");
        //    isGrounded = false;
        //    rb.constraints = RigidbodyConstraints2D.None;
        //}
    }

    void EnabledWeaponBox()
    {
        boxCollider.enabled = true;
    }
    void DisabledWeaponBox()
    {
        boxCollider.enabled = false;
        // weapon.hitGameOject = new HashSet<GameObject>();
    }


}
