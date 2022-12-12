using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectionController : MonoBehaviour
{
    private void Start()
    {
        GameModeManager.SetGameState(GameModeManager.GameState.LevelSelection);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(GameModeManager.Instance.gameModeData.gamePlaySceneName);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(GameModeManager.Instance.gameModeData.mainMenuSceneName);
    }
}
