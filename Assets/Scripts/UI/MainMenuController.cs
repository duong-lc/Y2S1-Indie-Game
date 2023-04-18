using System;
using System.Collections;
using System.Collections.Generic;
using SO_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        // GameModeManager.SetGameState(GameModeManager.GameState.MainMenu);
        GameModeManager.Instance.CurrentGameState = GameState.MainMenu;
    }

    public void LoadLevelSelection()
    {
        SceneManager.LoadScene(GameModeManager.Instance.GameModeData.levelSelectionSceneName);
    }

    public void LoadOptions()
    {
        //SceneManager.LoadScene(GameModeManager.Instance.gameModeData.optionSceneName);
    }

    public void LoadQuitApplication()
    {
        Application.Quit();
    }
    
}
