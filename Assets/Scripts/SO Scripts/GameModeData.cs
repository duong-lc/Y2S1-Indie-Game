using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Logging;
using Data_Classes;
using Sirenix.OdinInspector;
using UnityEngine;


[Serializable]
public class LaneControllerData
{
    [SerializeField] private KeyCode input;
    [SerializeField] private Vector3 hitPoint;
    [ReadOnly] [SerializeField] public LaneCollider collider;
    
    public KeyCode Input => input;
    public Vector3 HitPoint => hitPoint;
}

[CreateAssetMenu(fileName = "GameModeData", menuName = "ScriptableObjects/GameModeData", order = 0)]
public class GameModeData : SerializedScriptableObject
{
    [TitleGroup("Scene Data")]
    public string mainMenuSceneName;
    public string levelSelectionSceneName;
    public string gamePlaySceneName;
    public string optionSceneName;

    [TitleGroup("Gameplay Data")] 
    [SerializeField] private int inputDelayInMS; //it's the issue with the keyboard and we need to have input delay 
    [SerializeField] private float songDelayInSeconds;
    [SerializeField] private float noteTime; //how much time the note is going to be on the screen
    [SerializeField] private float noteSpawnZ; //the Z position for the note to be spawned at
    [SerializeField] private float noteTapZ; //the Z position where the player should press the note
    public float NoteDespawnZ => noteTapZ - (noteSpawnZ - noteTapZ); //De-spawn position for notes
    public LayerMask noteLayerMask;
    [TitleGroup("Margin Of Error")]
    [SerializeField] private float _sliderHoldStickyTime;
    [SerializeField] private Dictionary<HitCondition, MarginOfError> noteHitCondDict = new();
    public float SliderHoldStickyTime => _sliderHoldStickyTime;
    
    [TitleGroup("Lane Controller Data")] 
    [SerializeField] private Dictionary<NoteData.LaneOrientation, LaneControllerData> laneControllerData = new();

    [TitleGroup("Hit Condition Data")]
    [SerializeField] private Dictionary<HitCondition, ScoreData> hitCondToScoreData = new();

    [TitleGroup("Note Data")]
    [SerializeField] private Dictionary<NoteType, string> typeToTag = new();

    public float InputDelayInMS => inputDelayInMS;
    public float SongDelayInSeconds => songDelayInSeconds;
    public float NoteTime => noteTime;
    public float NoteSpawnZ => noteSpawnZ;
    public float NoteTapZ => noteTapZ;

    #region Getters
    private ReadOnlyDictionary<HitCondition, MarginOfError> _noteHitCondDictCache;
    public ReadOnlyDictionary<HitCondition, MarginOfError> NoteHitCondDict {
        get {
            if (_noteHitCondDictCache.Count == 0)
                _noteHitCondDictCache = new ReadOnlyDictionary<HitCondition, MarginOfError>(noteHitCondDict);
            return _noteHitCondDictCache;
        }
    }

    private ReadOnlyDictionary<NoteData.LaneOrientation, LaneControllerData> _laneControllerDataCache;
    public ReadOnlyDictionary<NoteData.LaneOrientation, LaneControllerData> LaneControllerData {
        get {
            if (_laneControllerDataCache.Count == 0 || _laneControllerDataCache == null)
                _laneControllerDataCache = new ReadOnlyDictionary<NoteData.LaneOrientation, LaneControllerData>(laneControllerData);
            return _laneControllerDataCache;
        }
    }
    
    private ReadOnlyDictionary<HitCondition, ScoreData> _hitCondToScoreDataCache;
    public ReadOnlyDictionary<HitCondition, ScoreData> HitCondToScoreData {
        get {
            if (_hitCondToScoreDataCache.Count == 0)
                _hitCondToScoreDataCache = new ReadOnlyDictionary<HitCondition, ScoreData>(hitCondToScoreData);
            return _hitCondToScoreDataCache;
        }
    }
    
    private ReadOnlyDictionary<NoteType, string> _typeToTagCache;
    public ReadOnlyDictionary<NoteType, string> TypeToTag {
        get {
            if (_typeToTagCache.Count == 0)
                _typeToTagCache = new ReadOnlyDictionary<NoteType, string>(typeToTag);
            return _typeToTagCache;
        }
    }

    #endregion


    public MarginOfError GetMOE (HitCondition cond) {
        if (noteHitCondDict.TryGetValue(cond, out MarginOfError MOE)) return MOE;
        NCLogger.Log($"Hit Condition: {cond} not found");
        return null;
    }

    public Vector3 GetHitPoint(NoteData.LaneOrientation orientation) {
        if (laneControllerData.TryGetValue(orientation, out LaneControllerData data)) return data.HitPoint;
        NCLogger.Log($"Orientation: {orientation} not found", LogLevel.ERROR);
        return Vector3.zero;
    }
    
    public KeyCode GetKeyCode(NoteData.LaneOrientation orientation) {
        if (laneControllerData.TryGetValue(orientation, out LaneControllerData data)) return data.Input;
        NCLogger.Log($"Orientation: {orientation} not found", LogLevel.ERROR);
        return KeyCode.None;
    }

    public GameObject GetHitCondPrefab(HitCondition hitCond)
    {
        if (HitCondToScoreData.TryGetValue(hitCond, out ScoreData data)) return data.Prefab;
        NCLogger.Log($"HitCondition: {hitCond} not found", LogLevel.ERROR);
        return new GameObject();
    }
    
    public LaneControllerData GetControlData (NoteData.LaneOrientation orientation) {
        if (laneControllerData.TryGetValue(orientation, out LaneControllerData data)) return data;
        NCLogger.Log($"Controller Data: {data} not found", LogLevel.ERROR);
        return null;
    }
    
    public void GetLerpPoints (Data_Classes.NoteData.LaneOrientation noteOrientation, ref Vector3 startPos, ref Vector3 endPos)
    {
        if(!laneControllerData.TryGetValue(noteOrientation, out var data)) {
            NCLogger.Log($"Orientation: {noteOrientation} is invalid, cannot Get Lerp Points", LogLevel.ERROR);
        }
       
        //Vector3 startPos = hitPoint + (dir * noteSpawnZ); 
        startPos = new Vector3(data.HitPoint.x, data.HitPoint.y, noteSpawnZ);
        //Vector3 endPos = hitPoint + (dir * NoteDespawnZ);
        endPos = new Vector3(data.HitPoint.x, data.HitPoint.y, NoteDespawnZ);
    }
}
