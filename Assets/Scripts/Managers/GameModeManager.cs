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


public class GameModeManager : Singleton<GameModeManager>
{
    public enum GameState
    {
        MainMenu,
        LevelSelection,
        PlayMode,
        PauseMode,
    }
    
    //Main Menu 
    public GameModeData gameModeData;
    private GameState _gameState;

    private void Awake()
    {
        if(Instance != null && Instance != this) Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    public static void SetGameState(GameState newGameState) {
        Instance._gameState = newGameState;
    }

    public GameState GetGameState() {
        return _gameState;
    }

}
