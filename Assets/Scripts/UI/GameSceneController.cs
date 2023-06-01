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
    [SerializeField] private GameObject[] inGameHUDs;

    private void Awake()
    {
        if(!pauseOverlay) NCLogger.Log($"pauseOverlay not assigned", LogLevel.ERROR);
        if(inGameHUDs.Length == 0) NCLogger.Log($"inGameHUD not assigned", LogLevel.ERROR);
    }

    private void Start()
    {
        GameModeManager.Instance.CurrentGameState = GameState.PlayMode;
        SongManager.PlaySong();
        Time.timeScale = 1;

        ToggleInGameHUD(true);
        pauseOverlay.SetActive(false);
    }

    public void LoadPauseOverlay()
    {
        // GameModeManager.SetGameState(GameModeManager.GameState.PauseMode);
        GameModeManager.Instance.CurrentGameState = GameState.PauseMode;
        SongManager.PauseSong();
        Time.timeScale = 0;

        ToggleInGameHUD(false);
        pauseOverlay.SetActive(true);    
        this.FireEvent(EventType.PauseTransitionEvent, PauseTransition.RibbonState.Pause);
    }

    public void LoadReturnToGame()
    {
        // GameModeManager.SetGameState(GameModeManager.GameState.PlayMode);
        NCLogger.Log($"asdsadsadawsda");
        GameModeManager.Instance.CurrentGameState = GameState.PlayMode;
        SongManager.PlaySong();
        Time.timeScale = 1;

        ToggleInGameHUD(true);
        //pauseOverlay.SetActive(false);
        
        this.FireEvent(EventType.PauseTransitionEvent, PauseTransition.RibbonState.Playing);
        EventDispatcher.Instance.FireEvent(EventType.UnPauseEvent);
    }

    public void LoadRestartLevel()
    {
        this.FireEvent(EventType.PauseTransitionEvent, PauseTransition.RibbonState.Return);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadReturnToLevelSelectionScene()
    {
        SceneManager.LoadScene(GameModeManager.Instance.GameModeData.levelSelectionSceneName);
    }

    private void ToggleInGameHUD(bool state)
    {
        foreach (var hud in inGameHUDs)
        {
            hud.SetActive(state);
        }
    }
}
