using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using DG.Tweening;
using Sirenix.OdinInspector;
using SO_Scripts;
using TMPro;
using UnityEngine;
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

public class NoteRegisterParam
{
    private HitCondition _cond;
    private NoteData.LaneOrientation _orientation;

    public HitCondition Cond => _cond;
    public NoteData.LaneOrientation Orientation => _orientation;
    
    public NoteRegisterParam(HitCondition cond, NoteData.LaneOrientation orientation) {
        _cond = cond;
        _orientation = orientation;
    }
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

public class ScoreManager : MonoBehaviour
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
    
    
    private void Awake() {
        EventDispatcher.Instance.AddListener(EventType.OnNoteHitEvent, param => OnHit((NoteRegisterParam) param));
        EventDispatcher.Instance.AddListener(EventType.OnNoteMissEvent, param => OnMiss((NoteData.LaneOrientation) param));
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

    private void OnHit(NoteRegisterParam param)
    {
        hitAudioSource.Play();

        _currentCombo++;
        
        UpdateComboText();
        SpawnHitText(param.Cond, param.Orientation);
    }

    private void OnMiss(NoteData.LaneOrientation laneOrientation)
    {
        missAudioSource.Play();

        _missCount++;
        if (_currentCombo > _maxCombo) _maxCombo = _currentCombo;
        _currentCombo = 0;
        
        UpdateComboText();
        SpawnHitText(HitCondition.Miss, laneOrientation);
    }

    private void UpdateComboText()
    {
        comboText.text = _currentCombo.ToString();
    }

    private void SpawnHitText(HitCondition hitCond, NoteData.LaneOrientation laneOrientation)
    {
        var hitPoint = _gameModeData.GetHitPoint(laneOrientation);

        TweenHitText(Instantiate(_gameModeData.GetHitCondPrefab(hitCond), hitPoint, _mainCam.transform.rotation, transform));
    }

    private void TweenHitText(GameObject obj)
    {
        var t = obj.transform;
        var dest = t.position + t.up * 1.5f;
        var dest2 = dest + t.up * .5f;
        var tmp = t.GetComponent<TMP_Text>();

        //Sequence hitSequence = DOTween.Sequence();
        // hitSequence.Append(t.DOMove(dest, 2f));
        // hitSequence.Append(DOTween.ToAlpha(()=> tmp.color, x=> tmp.color = x, 0, 1));

        t.DOMove(dest, 2f);
        DOTween.ToAlpha(() => tmp.color, x => tmp.color = x, 0, 1);
    }
    
}
