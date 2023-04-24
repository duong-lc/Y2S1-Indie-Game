using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Core.Patterns;
using DG.Tweening;
using NoteClasses;
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
public class ScoreData
{
    [SerializeField] private int score;
    [SerializeField] private bool countTowardsCombo;
    [SerializeField] private bool resetCombo;
    [SerializeField] private GameObject hitMarkPrefab;

    public int Score => score;
    public bool CountTowardsCombo => countTowardsCombo;
    public GameObject Prefab => hitMarkPrefab;
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
    public NoteBase noteRef { get; private set; }
    
    public HitMarkInitData (NoteBase noteRef, HitCondition cond, NoteData.LaneOrientation orientation) {
        this.cond = cond;
        this.orientation = orientation;
        this.noteRef = noteRef;
    }
}

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioSource missAudioSource;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text accuracyText;
    [Space] 
    private MidiData _midiData;
    private GameModeData _gameModeData;

    private float AccuracyFloat => _totalCurrentScore / _totalPerfectScore * 100;
    private int _currentScore;

    private int _totalCurrentScore = 0;
    private int _totalPerfectScore => _midiData.TotalRawNoteCount *
                                     _gameModeData.HitCondToScoreData[HitCondition.EarlyPerfect].Score;
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
        
        EventDispatcher.Instance.AddListener(EventType.NoteMissEvent, param => OnHit((HitMarkInitData) param));
    }

    // Start is called before the first frame update
    private void Start() {
        _midiData = GameModeManager.Instance.CurrentMidiData;
        _gameModeData = GameModeManager.Instance.GameModeData;
        _mainCam = Camera.main;
    }

    private void OnHit(HitMarkInitData param)
    {
        hitAudioSource.Play();

        var scoreData = _gameModeData.GetScoreData(param.cond);
        if(scoreData == null) NCLogger.Log($"null score data", LogLevel.ERROR);
        
        _currentCombo += scoreData.CountTowardsCombo ? 1 : 0;
        _currentScore += scoreData.Score;
        if (_totalCurrentScore == 0) _totalCurrentScore = _totalPerfectScore;
        _totalCurrentScore -= _gameModeData.HitCondToScoreData[HitCondition.EarlyPerfect].Score -
                              _gameModeData.HitCondToScoreData[param.cond].Score;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        
        if (param.cond == HitCondition.Miss && param.cond != HitCondition.None)
        {
            if (_currentCombo > _maxCombo) _maxCombo = _currentCombo;
            missAudioSource.Play();
            _missCount++;
            _currentCombo = 0;
        }
        else if (param.cond != HitCondition.None)
        {
            hitAudioSource.Play();
            
        }

        UpdateScoreText();
        UpdateComboText();
        UpdateAccuracyText();
    }

    // private void OnMiss(HitMarkInitData param)
    // {
    //     missAudioSource.Play();
    //
    //     _missCount++;
    //     if (_currentCombo > _maxCombo) _maxCombo = _currentCombo;
    //     _currentCombo = 0;
    //     
    //     UpdateComboText();
    // }

    private void UpdateScoreText()
    {
        scoreText.text = _currentScore.ToString();
    }
    
    private void UpdateAccuracyText()
    {
       accuracyText.text = AccuracyFloat + $"%";
    }
    
    private void UpdateComboText()
    {
        comboText.text = _currentCombo.ToString();
    }
}
