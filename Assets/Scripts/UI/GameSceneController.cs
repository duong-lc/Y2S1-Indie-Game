using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Core.Patterns;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = Core.Events.EventType;

public class GameSceneController : Singleton<GameSceneController>
{
    [SerializeField] private GameObject endGameOverlay;
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject[] inGameHUDs;

    [SerializeField] private TMP_Text[] artists;
    [SerializeField] private TMP_Text[] songTitles;
    
    private MidiData _midiData;
    private GameModeData _gameModeData;
    
    
    private void Awake()
    {
        _midiData = GameModeManager.Instance.CurrentMidiData;
        _gameModeData = GameModeManager.Instance.GameModeData;
        
        if(!pauseOverlay) NCLogger.Log($"pauseOverlay not assigned", LogLevel.ERROR);
        if(inGameHUDs.Length == 0) NCLogger.Log($"inGameHUD not assigned", LogLevel.ERROR);

        //this.AddListener(EventType.GameEndedEvent, param => LoadEndScreenOverlay());
        
    }

    private void Start()
    {
        GameModeManager.Instance.CurrentGameState = GameState.PlayMode;
        SongManager.PlaySong();
        Time.timeScale = 1;

        ToggleInGameHUD(true);
        pauseOverlay.SetActive(false);
        endGameOverlay.SetActive(false);

        foreach (var artist in artists) {
            artist.text = _midiData.artist;
        }
        foreach (var songTitle in songTitles) {
            songTitle.text = _midiData.songTitle;
        }
    }

    public void LoadEndScreenOverlay()
    {
        GameModeManager.Instance.CurrentGameState = GameState.PauseMode;
        SongManager.PauseSong();
        Time.timeScale = 0;

        ToggleInGameHUD(false);
        endGameOverlay.SetActive(true);
    }
    
    public void LoadPauseOverlay()
    {
        NCLogger.Log($"lmao xd");
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
