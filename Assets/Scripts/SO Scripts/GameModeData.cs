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
    [SerializeField] private string laneTag;
    [SerializeField] private KeyCode input;
    [SerializeField] private Vector3 hitPoint;
    [ReadOnly] [SerializeField] public LaneCollider collider;

    public string LaneTag => laneTag;
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
    
    [Header("Margin Of Error")]
    [SerializeField] private Dictionary<HitCondition, MarginOfError> noteHitCondDict = new();

    [Header("Lane Controller Data")] 
    [SerializeField] private Dictionary<NoteData.LaneOrientation, LaneControllerData> laneControllerData = new();

    public float InputDelayInMS => inputDelayInMS;
    public float SongDelayInSeconds => songDelayInSeconds;
    public float NoteTime => noteTime;
    public float NoteSpawnZ => noteSpawnZ;
    public float NoteTapZ => noteTapZ;

    public ReadOnlyDictionary<HitCondition, MarginOfError> NoteHitCondDict => new(noteHitCondDict);
    public ReadOnlyDictionary<NoteData.LaneOrientation, LaneControllerData> LaneControllerData => new (laneControllerData);
    
    public MarginOfError GetMOE (HitCondition cond) {
        if (noteHitCondDict.TryGetValue(cond, out MarginOfError MOE)) return MOE;
        NCLogger.Log($"Hit Condition: {cond} not found");
        return null;
    }

    public Vector3 GetHitPoint(NoteData.LaneOrientation orientation) {
        if (laneControllerData.TryGetValue(orientation, out LaneControllerData data)) return data.HitPoint;
        NCLogger.Log($"Orientation: {orientation} not found");
        return Vector3.zero;
    }
    
    public HitCondition GetHitCondition(float hitOffset)
    {
        //hitOffset = CurrentSongTimeAdjusted - AssignedTime
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

    public HitCondition CalculateHitCondition(float regOffset)
    {
        
    }
}
