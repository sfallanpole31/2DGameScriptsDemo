using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int nextStage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            FindObjectOfType<AudioSet>().PlaySfX(2);
            FindObjectOfType<GameManager>().NextGmaePanelShow();
        }
    }
}
