using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicPoints : MonoBehaviour
{
    public float MP;
    public float MaxMP;
    public Image foreground;
    [Header("改變速率")]
    [Range(0, 1)]
    [SerializeField] float changeRatio;

    private void Start()
    {
        MP = MaxMP;
    }

    private void Update()
    {
        if (foreground)
            foreground.fillAmount = Mathf.Lerp(foreground.fillAmount, GetMPRatio(), changeRatio);
    }

    public float GetMPRatio()
    {

        return MP / (MaxMP);
    }
}
