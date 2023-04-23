using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Core.Patterns;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SO_Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using EventType = Core.Events.EventType;
using NoteData = Data_Classes.NoteData;
[Serializable]
[Flags]
public enum HitCondition {
    None,
    Early,
    EarlyPerfect,
    LatePerfect,
    Late,
    Miss,
}


[Serializable]
public class MarginOfError
{
    [HorizontalGroup]
    [SerializeField] private float beginMOE;
    
    [HorizontalGroup]
    [SerializeField] private float endMOE;

    public float BeginMOE => beginMOE;
    public float EndMOE => endMOE;
}

public class HitMarkInitData : PooledObjectCallbackData
{
    public NoteData.LaneOrientation orientation { get; private set; }
    public HitCondition cond { get; private set; }
    
    public HitMarkInitData (HitCondition cond, NoteData.LaneOrientation orientation) {
        this.cond = cond;
        this.orientation = orientation;
    }
}

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioSource missAudioSource;
    [SerializeField] private TMP_Text comboText;
    [Space] 
    private MidiData _midiData;
    private GameModeData _gameModeData;
        
    private int _currentCombo;
    private int _maxCombo;
    private int _missCount;
    private Camera _mainCam;
    
    public int MissCount => _missCount;
    public int CurrentCombo => _currentCombo;
    public int MaxCombo => _maxCombo;
    
    
    private ObjectPool[] _notePoolArray;
    public ObjectPool[] NotePools {
        get {
            if (_notePoolArray.IsNullOrEmpty()) _notePoolArray = GetComponentsInChildren<ObjectPool>();
            return _notePoolArray;
        }
    }
    
    private void Awake() {
        EventDispatcher.Instance.AddListener(EventType.NoteHitEarlyEvent, param => OnHit((HitMarkInitData) param));
        EventDispatcher.Instance.AddListener(EventType.NoteHitPerfectEvent, param => OnHit((HitMarkInitData) param));
        EventDispatcher.Instance.AddListener(EventType.NoteHitLateEvent, param => OnHit((HitMarkInitData) param));
        
        EventDispatcher.Instance.AddListener(EventType.NoteMissEvent, param => OnMiss((HitMarkInitData) param));
    }

    // Start is called before the first frame update
    private void Start() {
        _midiData = GameModeManager.Instance.CurrentMidiData;
        _gameModeData = GameModeManager.Instance.GameModeData;
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnHit(HitMarkInitData param)
    {
        hitAudioSource.Play();
        
        switch (param.cond)
        {
            case HitCondition.Early:
                break;
            case HitCondition.EarlyPerfect | HitCondition.LatePerfect:
                break;
            case HitCondition.Late:
                break;
        }
        _currentCombo++;
        
        UpdateComboText();
    }

    private void OnMiss(HitMarkInitData param)
    {
        missAudioSource.Play();

        _missCount++;
        if (_currentCombo > _maxCombo) _maxCombo = _currentCombo;
        _currentCombo = 0;
        
        UpdateComboText();
    }

    private void UpdateComboText()
    {
        comboText.text = _currentCombo.ToString();
    }

    // private void SpawnHitText(GameObject obj, NoteData.LaneOrientation laneOrientation)
    // {
    //     var hitPoint = _gameModeData.GetHitPoint(laneOrientation);
    //     var rot = _mainCam.transform.rotation;
    //     obj.transform.position = hitPoint;
    //     obj.transform.rotation = rot;
    //     TweenHitText(obj);
    // }
}
