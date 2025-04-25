using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform player;
    private void FixedUpdate()
    {
        if (player != null)
            transform.position = new Vector3(player.position.x, player.position.y, -5);
    }
}

