using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    private float disapearTimer;
    private Color textColor;

    private void Awake()
    {
        textMeshPro = transform.GetComponent<TextMeshPro>();
        textColor = textMeshPro.color;
        disapearTimer = 1f;
    }

    public void Setup(float damge)
    {
        textMeshPro.SetText(damge.ToString());

    }

    public static DamagePopup Create(Vector3 position, float damage)
    {
        Transform transform = Instantiate(GameAssets.Instance.pfDamagePopup, position, Quaternion.identity); // 取得 GameObject
        DamagePopup damagePopup = transform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);
        return damagePopup;
    }

    private void Update()
    {
        //傷害字形往上移動
        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disapearTimer -= Time.deltaTime;

        //傷害字形消失
        if (disapearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMeshPro.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }

        }
    }


}
