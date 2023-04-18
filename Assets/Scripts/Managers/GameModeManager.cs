using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Core.Patterns;
using SO_Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using EventType = Core.Events.EventType;

public enum GameState
{
    Undefined,
    MainMenu,
    LevelSelection,
    PlayMode,
    PauseMode,
}

public class GameModeManager : Singleton<GameModeManager>
{
    
    //Main Menu 
    [SerializeField] private GameModeData gameModeData;
    [SerializeField] private MidiData currentMidiData;

    public MidiData CurrentMidiData => currentMidiData;
    public GameModeData GameModeData => gameModeData;
    
    private GameState _gameState;
    public GameState CurrentGameState {
        get
        {
            if (_gameState == GameState.Undefined) {
                NCLogger.Log($"game state is {_gameState}", LogLevel.ERROR);
            }
            return _gameState;
        }
        
        set => Instance._gameState = value;
    }
    
    private void Awake() {
        base.Awake();
        if(Instance != null && Instance != this) Destroy(gameObject);
    
        if(!CurrentMidiData) NCLogger.Log($"midiData is {CurrentMidiData}", LogLevel.ERROR);
        if(!GameModeData) NCLogger.Log($"midiData is {GameModeData}", LogLevel.ERROR);
        
        DontDestroyOnLoad(gameObject);
    }
}
