using System;
using System.Collections;
using System.Collections.Generic;
using Core.Events;
using DG.Tweening;
using SO_Scripts;
using TMPro;
using UnityEngine;
using EventType = Core.Events.EventType;
using NoteData = Data_Classes.NoteData;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioSource missAudioSource;
    [SerializeField] private TMP_Text comboText;
    [Space] 
    [SerializeField] private MidiData midiData;
    
    private int _currentCombo;
    private int _maxCombo;
    private int _missCount;
    
    public int MissCount => _missCount;
    public int CurrentCombo => _currentCombo;
    public int MaxCombo => _maxCombo;
    
    private void Awake()
    {
        EventDispatcher.Instance.AddListener(EventType.OnNoteHitEvent, param => OnHit((NoteData.LaneOrientation) param));
        EventDispatcher.Instance.AddListener(EventType.OnNoteMissEvent, param => OnMiss((NoteData.LaneOrientation) param));
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnHit(NoteData.LaneOrientation laneOrientation)
    {
        hitAudioSource.Play();

        _currentCombo++;
        
        UpdateComboText();
        SpawnHitText(true, laneOrientation);
    }

    private void OnMiss(NoteData.LaneOrientation laneOrientation)
    {
        missAudioSource.Play();

        _missCount++;
        if (_currentCombo > _maxCombo) _maxCombo = _currentCombo;
        _currentCombo = 0;
        
        UpdateComboText();
        SpawnHitText(false, laneOrientation);
    }

    private void UpdateComboText()
    {
        comboText.text = _currentCombo.ToString();
    }

    private void SpawnHitText(bool hitStatus, NoteData.LaneOrientation laneOrientation)
    {
        Vector3 hitPoint = Vector3.zero;
        foreach (var entry in midiData.HitPointDict) {
            if (laneOrientation == entry.Key) hitPoint = entry.Value;
        }

        TweenHitText(hitStatus
            ? Instantiate(midiData.hitTextPrefab, hitPoint, Quaternion.identity, transform)
            : Instantiate(midiData.missTextPrefab, hitPoint, Quaternion.identity, transform));
    }

    private void TweenHitText(GameObject obj)
    {
        var t = obj.transform;
        var dest = t.up * 5;

        t.DOMove(dest, .7f);
        t.GetComponent<TMP_Text>().material.DOFade(255, 0.7f);

    }
    
}
