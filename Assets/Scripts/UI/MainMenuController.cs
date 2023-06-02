using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using GabrielBigardi.SpriteAnimator;
using SO_Scripts;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using EventType = Core.Events.EventType;


public class MainMenuController : MonoBehaviour
{
    private TransitionState state;
    private bool _consumeInput = false;
    [SerializeField] private PostProcessVolume volume;
    
    private void Awake()
    {
        this.AddListener(EventType.GlobalTransitionCompleteEvent, param => state = (TransitionState) param);
    }

    private IEnumerator Start()
    {
        Time.timeScale = 1;
        volume.profile = GameModeManager.Instance.noColorGradingProfile;
        state = TransitionState.In;
        _consumeInput = true;
        GameModeManager.Instance.CurrentGameState = GameState.MainMenu;
        
        this.FireEvent(EventType.GlobalTransitionEvent, TransitionState.Out);
        while (state == TransitionState.In) {
            yield return null;
        }
        _consumeInput = false;
    }

    public void LoadLevelSelection()
    {
        if (_consumeInput) return;
        _consumeInput = true;
        StartCoroutine(
            LoadSceneRoutine(
                () => SceneManager.LoadScene(GameModeManager.Instance.GameModeData.levelSelectionSceneName)));
    }

    private IEnumerator LoadSceneRoutine(Action callback)
    {
        this.FireEvent(EventType.GlobalTransitionEvent, TransitionState.In);


        while (state != TransitionState.In) {
            yield return null;
        }
        
        this.FireEvent(EventType.GlobalTransitionEvent, TransitionState.Out);
        callback.Invoke();
    }
    
    public void LoadOptions()
    {
        if (_consumeInput) return;
        _consumeInput = true;
        //SceneManager.LoadScene(GameModeManager.Instance.gameModeData.optionSceneName);
    }

    public void LoadQuitApplication()
    {
        if (_consumeInput) return;
        _consumeInput = true;
        Application.Quit();
    }
    
}
