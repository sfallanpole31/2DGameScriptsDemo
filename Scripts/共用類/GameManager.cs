using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject GameOverPanel;
    public GameObject NextGamePanel;
    public GameObject ExplodePanel;
    public GameObject WinPanel;

    public  static GameObject WarningSign;
    [SerializeField]private GameObject WarningSignPrefab;


    private void Awake()
    {
        WarningSign = WarningSignPrefab;
    }
    public void NextGmaePanelShow()
    {
        NextGamePanel.SetActive(true);
    }

    public void GameOverPanelShow()
    {
        GameOverPanel.SetActive(true);
    }

    public void WinPanelShow()
    {
        WinPanel.SetActive(true);
    }

    public void EnableExplodePanel()
    {
        ExplodePanel.SetActive(true);
    }
    public void DisableExplodePanel()
    {
        ExplodePanel.SetActive(false);
    }

    public void RestartScene()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void GoNextScene(int sceneNumber)
    {
        SceneManager.LoadSceneAsync(sceneNumber);
    }

}
