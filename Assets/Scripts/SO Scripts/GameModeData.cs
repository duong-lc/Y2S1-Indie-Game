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
    [Header("Margin Of Error")]
    [SerializeField] private float _sliderHoldStickyTime;
    [SerializeField] private Dictionary<HitCondition, MarginOfError> noteHitCondDict = new();
    public float SliderHoldStickyTime => _sliderHoldStickyTime;
    
    [Header("Lane Controller Data")] 
    [SerializeField] private Dictionary<NoteData.LaneOrientation, LaneControllerData> laneControllerData = new();

    [Header("Hit Condition Visuals")]
    [SerializeField] private Dictionary<HitCondition, GameObject> hitCondToPrefab = new();

    [Header("Note Data")]
    [SerializeField] private Dictionary<NoteType, string> typeToTag = new();

    public float InputDelayInMS => inputDelayInMS;
    public float SongDelayInSeconds => songDelayInSeconds;
    public float NoteTime => noteTime;
    public float NoteSpawnZ => noteSpawnZ;
    public float NoteTapZ => noteTapZ;

    public ReadOnlyDictionary<HitCondition, MarginOfError> NoteHitCondDict => new (noteHitCondDict);
    public ReadOnlyDictionary<NoteData.LaneOrientation, LaneControllerData> LaneControllerData => new (laneControllerData);
    public ReadOnlyDictionary<HitCondition, GameObject> HitCondToPrefab => new (hitCondToPrefab);
    public ReadOnlyDictionary<NoteType, string> TypeToTag => new (typeToTag);

    public MarginOfError GetMOE (HitCondition cond) {
        if (noteHitCondDict.TryGetValue(cond, out MarginOfError MOE)) return MOE;
        NCLogger.Log($"Hit Condition: {cond} not found");
        return null;
    }

    public HitCondition GetHitCondition(double hitOffset)
    {
        //hitOffset = CurrentSongTimeAdjusted - AssignedTime
        foreach (var kvp in noteHitCondDict) {
            var margin = kvp.Value;
            var offset = Mathf.Abs((float)hitOffset);
            switch (hitOffset) {
                case < 0:
                    switch (kvp.Key) {
                        case HitCondition.None:
                            if (offset > Mathf.Abs(GetMOE(HitCondition.Early).EndMOE)) {
                                return HitCondition.None;
                            }
                            break;
                        case HitCondition.EarlyPerfect | HitCondition.Early:
                            if (offset >= Mathf.Abs(margin.BeginMOE) && offset < Mathf.Abs(margin.EndMOE)) {
                                return kvp.Key;
                            }
                            break;
                        // default:
                        //     NCLogger.Log($"{kvp.Key}");
                        //     break;
                            //throw new ArgumentOutOfRangeException();
                    }
                    break;
                case > 0:
                    switch (kvp.Key) {
                        case HitCondition.LatePerfect | HitCondition.Late:
                            if (offset >= Mathf.Abs(margin.BeginMOE) && offset < Mathf.Abs(margin.EndMOE)) {
                                return kvp.Key;
                            }
                            break;
                        case HitCondition.Miss:
                            if (offset > Mathf.Abs(GetMOE(HitCondition.Late).EndMOE)) {
                                NCLogger.Log($"offset {hitOffset} > {Mathf.Abs(GetMOE(HitCondition.Late).EndMOE)} is {offset > Mathf.Abs(GetMOE(HitCondition.Late).EndMOE)}");
                                return HitCondition.Miss;
                            }
                            break;
                        // default:
                        //     NCLogger.Log($"{kvp.Key}");
                        //     break;
                            // throw new ArgumentOutOfRangeException();
                    }
                    break;
                case 0:
                    return HitCondition.EarlyPerfect;
            }
        }
        return HitCondition.Miss;
    }
    
    public Vector3 GetHitPoint(NoteData.LaneOrientation orientation) {
        if (laneControllerData.TryGetValue(orientation, out LaneControllerData data)) return data.HitPoint;
        NCLogger.Log($"Orientation: {orientation} not found");
        return Vector3.zero;
    }
    
    public KeyCode GetKeyCode(NoteData.LaneOrientation orientation) {
        if (laneControllerData.TryGetValue(orientation, out LaneControllerData data)) return data.Input;
        NCLogger.Log($"Orientation: {orientation} not found");
        return KeyCode.None;
    }

    public GameObject GetHitCondPrefab(HitCondition hitCond)
    {
        if (HitCondToPrefab.TryGetValue(hitCond, out GameObject prefab)) return prefab;
        NCLogger.Log($"HitCondition: {hitCond} not found");
        return new GameObject();
    }
    
 
    
    public LaneControllerData GetControlData (NoteData.LaneOrientation orientation) {
        if (laneControllerData.TryGetValue(orientation, out LaneControllerData data)) return data;
        NCLogger.Log($"Controller Data: {data} not found");
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
