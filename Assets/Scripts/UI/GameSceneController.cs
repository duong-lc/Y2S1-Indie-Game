using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = Core.Events.EventType;

public class GameSceneController : MonoBehaviour
{
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject inGameHUD;

    private void Awake()
    {
        if(!pauseOverlay) NCLogger.Log($"pauseOverlay not assigned", LogLevel.ERROR);
        if(!inGameHUD) NCLogger.Log($"inGameHUD not assigned", LogLevel.ERROR);
    }

    private void Start()
    {
        LoadReturnToGame();
    }

    public void LoadPauseOverlay()
    {
        // GameModeManager.SetGameState(GameModeManager.GameState.PauseMode);
        GameModeManager.Instance.CurrentGameState = GameState.PauseMode;
        SongManager.PauseSong();
        Time.timeScale = 0;
        
        inGameHUD.SetActive(false);
        pauseOverlay.SetActive(true);    
    }

    public void LoadReturnToGame()
    {
        // GameModeManager.SetGameState(GameModeManager.GameState.PlayMode);
        GameModeManager.Instance.CurrentGameState = GameState.PlayMode;
        SongManager.PlaySong();
        Time.timeScale = 1;
        
        inGameHUD.SetActive(true);
        pauseOverlay.SetActive(false);
        
        EventDispatcher.Instance.FireEvent(EventType.OnUnPauseEvent);
    }

    public void LoadRestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadReturnToLevelSelectionScene()
    {
        SceneManager.LoadScene(GameModeManager.Instance.GameModeData.levelSelectionSceneName);
    }
}
