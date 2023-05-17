using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectionController : MonoBehaviour
{
    private void Start()
    {
        GameModeManager.Instance.CurrentGameState = GameState.LevelSelection;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(GameModeManager.Instance.GameModeData.gamePlaySceneName);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(GameModeManager.Instance.GameModeData.mainMenuSceneName);
    }

    public void PlayTest()
    {
        SceneManager.LoadScene(GameModeManager.Instance.GameModeData.gamePlaySceneName);
    }
}
