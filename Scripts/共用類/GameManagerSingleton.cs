using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSingleton
{
    private GameObject gameObject;
    private static GameManagerSingleton m_instance;
    public static GameManagerSingleton Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance.gameObject = new GameObject("GameManagerSingleton");
                return m_instance;
            }
            return m_instance;

        }
    }

    public static GameObject m_warningPrefab;


}
