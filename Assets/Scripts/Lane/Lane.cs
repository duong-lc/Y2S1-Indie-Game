using System;
using UnityEngine;
using SO_Scripts;
using Data_Classes;
using System.Collections.Generic;
using Core.Logging;
using Managers;
using DataClass = Data_Classes;
using NoteClasses;

public class Lane : MonoBehaviour
{
    private MidiData _midiData;
    private GameModeData _gameModeData;
    
    public List<BaseNoteType> allNotesList = new List<BaseNoteType>();
    
    private bool _isSpawn = true;
    private int _spawnIndex = 0;//index spawn to loop through the timestamp array to spawn notes based on timestamp
    private int _inputIndex;//input index to loop through the timestamp array to form note input queue 
    private DataClass.NoteData.LaneOrientation _orientation;

    public DataClass.NoteData.LaneOrientation LaneOrientation => _orientation;
    //Variables for caching
    //private Vector3 _laneHitPoint;
    private GameObject _normalNotePrefab;
    private GameObject _sliderNotePrefab;
    //private float _travelTime;
    private Vector3 _spawnLocation;
    private double AudioTimeRaw => SongManager.Instance.GetAudioSourceTimeRaw();
    
    private LaneCollider _laneCol;
    public LaneCollider LaneCollider {
        get {
            if (!_laneCol) _laneCol = GetComponent<LaneCollider>();
            return _laneCol;
        }
    }
     
    private void Awake()
    {

        // _laneHitPoint = _gameModeData.GetHitPoint(LaneOrientation);
        
        // _laneHitPoint = gameObject.tag switch
        // {
        //     "Lane1" => _midiData.hitPoint1,
        //     "Lane2" => _midiData.hitPoint2,
        //     "Lane3" => _midiData.hitPoint3,
        //     "Lane4" => _midiData.hitPoint4,
        //     _ => new Vector3(0,0,0)
        // };

        _normalNotePrefab = _midiData.noteNormalPrefab;
        _sliderNotePrefab = _midiData.noteSliderPrefab;
       // _travelTime = _gameModeData.NoteTime;
       // _marginOfError = _midiData.marginOfError;
      //  _inputDelay = _midiData.inputDelayInMilliseconds;
        _spawnLocation = new Vector3(_gameModeData.GetHitPoint(LaneOrientation).x, 0, _gameModeData.NoteSpawnZ);
    }

    private void Start()
    {
        _midiData = GameModeManager.Instance.CurrentMidiData;
        _gameModeData = GameModeManager.Instance.GameModeData;
        
    }

    public void SetLocalListOnLane(List<BaseNoteType> listToSet)
    {
        allNotesList.Clear();
        //NCLogger.Log($"{listToSet.Count}");
        allNotesList = listToSet;
    }

    private void Update()
    {
        SpawningNotesFromList();
    }
    
    private void SpawningNotesFromList()
    {
        if (!_isSpawn) return;
        if (allNotesList.Count <= 0)
        {
            //print($"no notes");
            return;
        }
        else
        {
            //print($"list size {allNoteOnLaneList.Count}");
        }

        switch (allNotesList[_spawnIndex].noteID)
        {
            case DataClass.NoteData.NoteID.NormalNote:
                NoteNormalType noteNormalCast = (NoteNormalType) allNotesList[_spawnIndex];

                //if current song time reaches point to spawn a note
                if (AudioTimeRaw >= noteNormalCast.timeStamp - _gameModeData.NoteTime)
                {
                    //Spawn a note
                    var noteObj = Instantiate(_normalNotePrefab, _spawnLocation , Quaternion.identity, transform);
                    //updating the game object ref in the note
                    allNotesList[_spawnIndex].noteObj = noteObj;
                    var normalComp = noteObj.GetComponent<NoteNormal>();
                    
                    normalComp.InitializeDataOnSpawn(
                        ref noteNormalCast.octaveNum,
                        ref noteNormalCast.laneOrientation,
                        ref noteNormalCast.timeStamp);
                    
                    // normalComp.octaveNum = allNotesList[_spawnIndex].octaveNum;
                    // //pass the orientation property
                    // normalComp.noteOrientation = allNotesList[_spawnIndex].laneOrientation;
                    // //get the time the note should be tapped by player and add to the array
                    // normalComp.assignedTime = noteNormalCast.timeStamp;
                    // normalComp.SetIndexOnLaneList(_spawnIndex);
                    
                    IncrementSpawnIndex();
                }

                break;
            case DataClass.NoteData.NoteID.SliderNote:
                NoteSliderType noteSliderCast = (NoteSliderType) allNotesList[_spawnIndex];

                if (AudioTimeRaw >= noteSliderCast.sliderData.timeStampKeyDown - _gameModeData.NoteTime)
                {
                    //print($"{gameObject.name}");
                    //Spawn a slider prefab
                    var NoteSliderObj = Instantiate(_sliderNotePrefab, _spawnLocation , Quaternion.identity, transform);
                    //updating the game object ref in the note
                    allNotesList[_spawnIndex].noteObj = NoteSliderObj;
                    var sliderComp = NoteSliderObj.GetComponent<NoteSlider>();
                    
                    sliderComp.InitializeDataOnSpawn(
                        ref noteSliderCast.octaveNum,
                        ref noteSliderCast.laneOrientation,
                        ref noteSliderCast.sliderData);
                    
                    // sliderComp.octaveNum = allNotesList[_spawnIndex].octaveNum;
                    // //pass the orientation property
                    // sliderComp.noteOrientation = allNotesList[_spawnIndex].laneOrientation;
                    // //Passing data to the newly spawned slider 
                    // sliderComp.data = noteSliderCast.sliderData;

                   IncrementSpawnIndex();
                }

                break;
        }
    }

    private void IncrementSpawnIndex()
    {
        //increment the index
        if(_spawnIndex + 1 <= allNotesList.Count - 1)
            _spawnIndex++;
        else
            _isSpawn = false;
    }
    
}
