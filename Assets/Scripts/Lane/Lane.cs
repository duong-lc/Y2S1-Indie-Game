using System;
using UnityEngine;
using SO_Scripts;
using Data_Classes;
using System.Collections.Generic;
using Core.Events;
using Core.Logging;
using Managers;
using DataClass = Data_Classes;
using NoteClasses;
using EventType = Core.Events.EventType;

public class NoteInitData : PooledObjectCallbackData
{
    public int octave { get; private set; }
    public DataClass.NoteData.LaneOrientation orientation { get; private set; }
    public double timeStamp { get; private set; }
    public DataClass.NoteData.SliderData SliderData { get; private set; }

    public Lane lane { get; private set; }

    public NoteInitData(Lane lane, Vector3 position, Transform parent, int octave, 
        DataClass.NoteData.LaneOrientation orientation, double timeStamp) {
        // this.eventType = eventType;
        this.position = position;
        this.parent = parent;
        this.octave = octave;
        this.orientation = orientation;
        this.timeStamp = timeStamp;
        this.lane = lane;
        // this.spawnIndex = spawnIndex;
    }
    
    public NoteInitData(Lane lane, Vector3 position, Transform parent, int octave, 
        DataClass.NoteData.LaneOrientation orientation, DataClass.NoteData.SliderData data) {
        // this.eventType = eventType;
        this.position = position;
        this.parent = parent;
        this.octave = octave;
        this.orientation = orientation;
        this.SliderData = data;
        this.lane = lane;
        // this.spawnIndex = spawnIndex;
    }
}
[RequireComponent(typeof(ObjectPool))]
public class Lane : MonoBehaviour
{
    private MidiData _midiData;
    private GameModeData _gameModeData;
    
    public List<BaseNoteType> allNotesList = new List<BaseNoteType>();
    
    private bool _isSpawn = true;
    private int _spawnIndex = 0;//index spawn to loop through the timestamp array to spawn notes based on timestamp
    private int _inputIndex;//input index to loop through the timestamp array to form note input queue 
    [SerializeField] private DataClass.NoteData.LaneOrientation orientation;

    public DataClass.NoteData.LaneOrientation LaneOrientation => orientation;
    //Variables for caching

    private GameObject _normalNotePrefab;
    private GameObject _sliderNotePrefab;

    private Vector3 _spawnLocation;
    private double AudioTimeRaw => SongManager.Instance.GetAudioSourceTimeRaw();
    
    private LaneCollider _laneCol;
    public LaneCollider LaneCollider {
        get {
            if (!_laneCol) _laneCol = GetComponent<LaneCollider>();
            return _laneCol;
        }
    }

    private ObjectPool _notePool;

    public ObjectPool NotePool {
        get {
            if (!_notePool) _notePool = GetComponent<ObjectPool>();
            return _notePool;
        }
    }
     
    private void Awake()
    {
    }

    private void Start() {
        _midiData = GameModeManager.Instance.CurrentMidiData;
        _gameModeData = GameModeManager.Instance.GameModeData;
        
        if(!_midiData) NCLogger.Log($"midiData is {_midiData}", LogLevel.ERROR);
        if(!_gameModeData) NCLogger.Log($"midiData is {_gameModeData}", LogLevel.ERROR);
        
        _normalNotePrefab = _midiData.noteNormalPrefab;
        _sliderNotePrefab = _midiData.noteSliderPrefab;
        _spawnLocation = new Vector3(_gameModeData.GetHitPoint(LaneOrientation).x, 0, _gameModeData.NoteSpawnZ);
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
                    // var noteObj = Instantiate(_normalNotePrefab, _spawnLocation , Quaternion.identity, transform);
                    //updating the game object ref in the note
                    // allNotesList[_spawnIndex].noteObj = noteObj;
                    // var normalComp = noteObj.GetComponent<NoteNormal>();
                    //
                    // normalComp.InitializeDataOnSpawn(
                    //     ref noteNormalCast.octaveNum,
                    //     ref noteNormalCast.laneOrientation,
                    //     ref noteNormalCast.timeStamp);

                    this.FireEvent(EventType.SpawnNoteNormal, new NoteInitData(this, _spawnLocation,
                        transform, noteNormalCast.octaveNum,noteNormalCast.laneOrientation,noteNormalCast.timeStamp));
                    
                    IncrementSpawnIndex();
                }

                break;
            case DataClass.NoteData.NoteID.SliderNote:
                NoteSliderType noteSliderCast = (NoteSliderType) allNotesList[_spawnIndex];

                if (AudioTimeRaw >= noteSliderCast.sliderData.timeStampKeyDown - _gameModeData.NoteTime)
                {
                    //print($"{gameObject.name}");
                    //Spawn a slider prefab
                    // var NoteSliderObj = Instantiate(_sliderNotePrefab, _spawnLocation , Quaternion.identity, transform);
                    //updating the game object ref in the note
                    //allNotesList[_spawnIndex].noteObj = NoteSliderObj;
                    // var sliderComp = NoteSliderObj.GetComponent<NoteSlider>();
                    //
                    // sliderComp.InitializeDataOnSpawn(
                    //     ref noteSliderCast.octaveNum,
                    //     ref noteSliderCast.laneOrientation,
                    //     ref noteSliderCast.sliderData);
                    
                    // sliderComp.octaveNum = allNotesList[_spawnIndex].octaveNum;
                    // //pass the orientation property
                    // sliderComp.noteOrientation = allNotesList[_spawnIndex].laneOrientation;
                    // //Passing data to the newly spawned slider 
                    // sliderComp.data = noteSliderCast.sliderData;

                    this.FireEvent(EventType.SpawnNoteSlider, new NoteInitData( this, _spawnLocation, 
                        transform, noteSliderCast.octaveNum,noteSliderCast.laneOrientation,noteSliderCast.sliderData));
                    
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
