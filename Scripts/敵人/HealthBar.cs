using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Enemy enemy;
    [Header("��Canvas")]
    [SerializeField] GameObject rootCanvas;
    [Header("��q�e��")]
    [SerializeField] Image foreground;

    [Range(0, 1)]
    [SerializeField] float changeHealthRatio;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }


    // Update is called once per frame
    void Update()
    {
        //�p�G��q�ʤ��������0 or 1 �������
        if (Mathf.Approximately(GetHealthRatio(), 0) || Mathf.Approximately(GetHealthRatio(), 1))
        {
            rootCanvas.SetActive(false);
            return;
        }

        rootCanvas.SetActive(true);
        foreground.fillAmount = Mathf.Lerp(foreground.fillAmount,GetHealthRatio(), changeHealthRatio);
    }

    public float GetHealthRatio()
    {
        return enemy.HP / enemy.maxHP;
    }
}
