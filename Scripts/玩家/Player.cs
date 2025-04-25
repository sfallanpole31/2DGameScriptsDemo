using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpforce;
    Rigidbody2D MyRig; //宣告物件的Rigbody
    Animator MyAnimator;//動畫

    //判斷有無在地面上
    public bool IsGround; //宣告布林 
    public Transform Checker; // 檢查器的Transform
    float chackRadius = 1f; //檢查器的範圍大小
    public LayerMask GroundLayer; //地板的圖層
    Collider2D collider2d;


    //連擊
    bool IsAttacking;
    bool combo1, combo2;
    public bool bComboCold;
    public float fColdTime;
    [SerializeField] BoxCollider2D boxCollider;

    //受傷狀態
    public bool isHurt;

    //血量
    public float HP;
    public Sprite[] Hp_sprite;
    public Image UI_HP;

    //螢幕晃動
    public Animator Cam_anim;


    //火球法術
    GameObject fireBall_;
    public bool IsCasting;
    public GameObject fireBall;
    public Transform shootPoint;
    public Transform endPoint;

    //被擊飛 
    public Rigidbody2D rb;
    public float knockbackForce;  // 力的大小
    public bool knockingBack;
    private float knockbackEndTime;
    public float knockbackDuration;  // 击飞持续时间

    //武器
    [SerializeField] Weapon weapon;

    [Header("MP")]
    MagicPoints magicPoints;
    float timeMpRegeneration = 0f;

    //範圍技
    [Header("死亡爆炸prefab")]
    [SerializeField] GameObject deathExplodePrefab;

    [Header("爆炸次數")]
    [SerializeField] int explodeCount;

    [Header("爆炸距離")]
    [SerializeField] float explodeDistance;

    [Header("爆炸間隔")]
    [SerializeField] float explodeBetween;

    [Header("生成範圍")]
    private Vector3 explodeRange;

    void Start()
    {

        CheckHPUI();
        MyRig = GetComponent<Rigidbody2D>();
        MyAnimator = GetComponent<Animator>();
        boxCollider = weapon.GetComponent<BoxCollider2D>();
        magicPoints = GetComponent<MagicPoints>();
    }

    // Update is called once per frame
    void Update()
    {

        #region 跳躍功能
        if (Input.GetKeyDown(KeyCode.Space) && IsGround)
        {
            MyRig.velocity = Vector2.up * jumpforce;
            MyAnimator.SetBool("IsGround", false);
        }

        if (rb.velocity.y > 0) // 向上跳躍時
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), true);
        }
        else if (rb.velocity.y <= 0) // 向下移動或靜止時
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Ground"), false);
        }
        #endregion



        #region 火球法術
        if (Input.GetKeyDown(KeyCode.F) & magicPoints.MP >= 50f & IsGround)
        {

            IsCasting = true;
            magicPoints.MP -= 50f;
            MyAnimator.Play("Summner 0");
            StartCoroutine(CastFireballWithDelay(1.0f));  // 延遲0.5秒後生成火球

        }



        #endregion

        #region 爆炸法術
        if (Input.GetKeyDown(KeyCode.G) & magicPoints.MP >= 50f & IsGround)
        {
            IsCasting = true;

            magicPoints.MP -= 50f;
            MyAnimator.Play("Summner 0");

            StartCoroutine(CastSkullDeathDelay(1f));  // 延遲0.5秒後生成火球

        }



        #endregion

        #region Combo普通攻擊
        if (Input.GetKeyDown(KeyCode.Z) & IsGround)
        {

            if (isHurt)
            { return; }

            IsAttacking = true;
            weapon.hitGameOject = new HashSet<GameObject>();//將武器攻擊過的目標清空
            FindObjectOfType<AudioSet>().PlaySfX(0);

            if (!combo1 && !combo2)
            {
                weapon.damage = 10;
                MyAnimator.Play("Player Attack");
                combo1 = true;
                bComboCold = true;

            }
            else if (combo1 && !combo2)
            {
                weapon.damage = 15;
                MyAnimator.Play("Player Attack 2");
                combo2 = true;
                bComboCold = true;
                fColdTime = 0;

            }
            else if (combo1 && combo2)
            {
                weapon.damage = 30;
                weapon.isCombo3 = true;
                MyAnimator.Play("Player Attack 3");
                bComboCold = false;
                fColdTime = 0;
                combo1 = false;
                combo2 = false;

            }
        }

        if (bComboCold)
        {
            fColdTime += Time.deltaTime;
            if (fColdTime > 1f)
            {
                combo1 = false;
                combo2 = false;
                fColdTime = 0;
                bComboCold = false;

            }
        }
        #endregion

        #region 每秒回魔

        if (timeMpRegeneration < 1f)
        {
            timeMpRegeneration += Time.deltaTime;
        }
        else if (magicPoints.MP < magicPoints.MaxMP)
        {
            magicPoints.MP += 5f;
            timeMpRegeneration = 0f;
        }

        #endregion

    }

    private void FixedUpdate()
    {
        #region 地面檢測
        IsGround = Physics2D.OverlapCircle(Checker.position, chackRadius, GroundLayer);
        MyAnimator.SetBool("IsGround", IsGround);
        #endregion

        #region 角色移動
        if (!IsCasting && !IsAttacking && !knockingBack)
        {
            float move = Input.GetAxis("Horizontal");
            float face = Input.GetAxisRaw("Horizontal");
            MyRig.velocity = new Vector2(speed * move, MyRig.velocity.y);  //角色移動
            if (face != 0) //角色面向
            {
                transform.localScale = new Vector3(face, 1, 1);
            }

            MyAnimator.SetFloat("move", Mathf.Abs(move)); //待機 走路切換 
        }
        else
        {
            MyRig.velocity = Vector2.zero;
        }
        #endregion



        #region 被擊飛
        if (knockingBack)
        {
            if (rb != null)
            {
                print("被擊飛" + rb.ToString());
                knockbackEndTime += Time.fixedDeltaTime;

                // 设置新的速度，击飞角色
                rb.AddForce(new Vector2(-knockbackForce * 20 * transform.localScale.x, knockbackForce * 20), ForceMode2D.Force);
                // 调试信息：打印新的速度

            }
            // 检查是否应该结束击飞状态
            if (knockbackEndTime >= knockbackDuration)
            {
                knockingBack = false;
                knockbackEndTime = 0f;
            }
        }
        #endregion

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<Weapon>())
        {
            return;
        }

        #region 承受傷害
        if (!collision.GetComponent<Weapon>().hitGameOject.Contains(gameObject)) //若被觸發過不再觸發
        {
            //碰撞傷害
            if (collision.CompareTag("EnemyWeapon"))
            {
                IsAttacking = false;
                IsCasting = false;

                MyAnimator.Play("Player hurt");
                PlayerHurt();
                Cam_anim.Play("Cam_Shake");

            }

            #region 如果是一次性觸發技能 Trigger設True
            if (collision.GetComponent<Weapon>().oneHitSkill)
            {
                collision.GetComponent<Weapon>().hitGameOject.Add(gameObject);

            }
            #endregion

        }
        #endregion
    }

    // 協程方法，延遲生成火球
    private IEnumerator CastFireballWithDelay(float delay)
    {

        Vector3 currentPos = shootPoint.transform.position;


        yield return new WaitForSeconds(delay);  // 等待指定的延遲時間

        fireBall_ = Instantiate(fireBall, currentPos, fireBall.gameObject.transform.rotation);
        fireBall_.GetComponent<FireBall>().shootPoint = shootPoint.transform.position;
        fireBall_.GetComponent<FireBall>().endPoint = endPoint.transform.position;
    }

    /// <summary>
    /// 範圍技
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator CastSkullDeathDelay(float delay)
    {

        yield return new WaitForSeconds(delay);  // 等待指定的延遲時間
        explodeRange = new Vector3(explodeDistance * 2, explodeDistance, 1);//爆炸範圍
        Vector3 currentPos = this.transform.position;
        float currnetDirection = transform.localScale.x;

        for (int i = 1; i <= explodeCount; i++)
        {
            yield return new WaitForSeconds(explodeBetween);  // 等待指定的延遲時間
            Vector3 position = currentPos + new Vector3(explodeDistance * i * currnetDirection, 0, 0);
            Instantiate(deathExplodePrefab, position, deathExplodePrefab.gameObject.transform.rotation);
        }


        MyAnimator.SetBool("IsExplode", false);
        IsCasting = false;
        FindObjectOfType<GameManager>().DisableExplodePanel();
    }





    public void EndOfCasting()
    {
        IsCasting = false;
    }

    public void EndOfCombo()
    {
        IsAttacking = false;
    }

    public void HurtLayer()
    {

        // Debug.Log("執行HurtLayer");
        isHurt = true;
        knockingBack = true;
        MyAnimator.SetBool("isHurt", true);
        Cam_anim.SetBool("isHurt", true);
        this.gameObject.layer = 9;

    }

    public void ResetLayer()
    {
        //Debug.Log("執行ResetLayer");
        isHurt = false;
        knockingBack = false;
        MyAnimator.SetBool("isHurt", false);
        Cam_anim.SetBool("isHurt", false);
        this.gameObject.layer = 7;
    }

    // 方法用於開啟BoxCollider
    public void EnableBoxCollider()
    {
        boxCollider.enabled = true;
    }

    // 方法用於關閉BoxCollider
    public void DisableBoxCollider()
    {
        boxCollider.enabled = false;
    }

    //腳色受傷
    public void PlayerHurt()
    {
        HP -= 1;
        Debug.Log("腳色遭到1點傷害");
        CheckHPUI();
        if (HP <= 0)
        {
            Debug.Log("腳色死亡");
            Destroy(this.gameObject);
            FindObjectOfType<GameManager>().GameOverPanelShow();
            //GAME OVER
        }
    }

    public void CheckHPUI()
    {
        if (Hp_sprite.Length >= HP)
            UI_HP.sprite = Hp_sprite[(int)HP];
    }

}
