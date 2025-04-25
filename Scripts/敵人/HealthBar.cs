using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Enemy enemy;
    [Header("根Canvas")]
    [SerializeField] GameObject rootCanvas;
    [Header("血量前景")]
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
        //如果血量百分比約等於0 or 1 不做顯示
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
