using UnityEngine;
using SO_Scripts;
using Data_Classes;
using System.Collections.Generic;
using Managers;
using DataClass = Data_Classes;

public class Lane : MonoBehaviour
{
    [SerializeField] private MidiData _midiData;
    public List<BaseNoteType> allNotesList = new List<BaseNoteType>();
    
    private KeyCode _input;
    private bool _isSpawn = true;
    private int _spawnIndex = 0;//index spawn to loop through the timestamp array to spawn notes based on timestamp
    private int _inputIndex;//input index to loop through the timestamp array to form note input queue 
    private DataClass.NoteData.LaneOrientation _orientation;
    
    //Variables for caching
    private Vector3 _laneHitPoint;
    private GameObject _normalNotePrefab;
    private GameObject _sliderNotePrefab;
    private float _travelTime;
    private double _marginOfError;
    private float _inputDelay;

    private void Awake()
    {
        _laneHitPoint = gameObject.tag switch
        {
            "Lane1" => _midiData.hitPoint1,
            "Lane2" => _midiData.hitPoint2,
            "Lane3" => _midiData.hitPoint3,
            "Lane4" => _midiData.hitPoint4,
            _ => new Vector3(0,0,0)
        };

        _normalNotePrefab = _midiData.noteNormalPrefab;
        _sliderNotePrefab = _midiData.noteSliderPrefab;
        _travelTime = _midiData.noteTime;
        _marginOfError = _midiData.marginOfError;
        _inputDelay = _midiData.inputDelayInMilliseconds;
    }
    
    public void SetLocalListOnLane(List<BaseNoteType> listToSet)
    {
        allNotesList = listToSet;
        SetKeyInput();
    }
    
    private void SetKeyInput()
    {
        if (gameObject.CompareTag("Lane1"))
        {
            _orientation = DataClass.NoteData.LaneOrientation.One;
            _input = _midiData.input1;
        }
        else if (gameObject.CompareTag("Lane2"))
        {
            _orientation = DataClass.NoteData.LaneOrientation.Two;
            _input = _midiData.input2;
        }
        else if (gameObject.CompareTag("Lane3"))
        {
            _orientation = DataClass.NoteData.LaneOrientation.Three;
            _input = _midiData.input3;
        }
        else if (gameObject.CompareTag("Lane4"))
        {
            _orientation = DataClass.NoteData.LaneOrientation.Four;
            _input = _midiData.input4;
        }
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
                if (SongManager.GetAudioSourceTime() >= noteNormalCast.timeStamp - _travelTime)
                {
                    //Spawn a note
                    var noteObj = Instantiate(_normalNotePrefab, transform);
                    //updating the game object ref in the note
                    allNotesList[_spawnIndex].noteObj = noteObj;
                    var normalComp = noteObj.GetComponent<NoteNormal>();
                    
                    normalComp.octaveNum = allNotesList[_spawnIndex].octaveNum;
                    //pass the orientation property
                    normalComp.noteOrientation = allNotesList[_spawnIndex].laneOrientation;
                    //get the time the note should be tapped by player and add to the array
                    normalComp.assignedTime = noteNormalCast.timeStamp;
                    
                    //increment the index
                    if(_spawnIndex + 1 <= allNotesList.Count - 1)
                        _spawnIndex++;
                    else
                        _isSpawn = false;

                }

                break;
            case DataClass.NoteData.NoteID.SliderNote:
                NoteSliderType noteSliderCast = (NoteSliderType) allNotesList[_spawnIndex];

                if (SongManager.GetAudioSourceTime() >=
                    noteSliderCast.sliderData.timeStampKeyDown - _travelTime)
                {
                    //print($"{gameObject.name}");
                    //Spawn a slider prefab
                    var NoteSliderObj = Instantiate(_sliderNotePrefab, transform);
                    //updating the game object ref in the note
                    allNotesList[_spawnIndex].noteObj = NoteSliderObj;
                    var sliderComp = NoteSliderObj.GetComponent<NoteSlider>();
                    
                    sliderComp.octaveNum = allNotesList[_spawnIndex].octaveNum;
                    //pass the orientation property
                    sliderComp.noteOrientation = allNotesList[_spawnIndex].laneOrientation;
                    //Passing data to the newly spawned slider 
                    sliderComp.data = noteSliderCast.sliderData;
                    
                    //increment the index
                    if(_spawnIndex + 1 <= allNotesList.Count - 1)
                        _spawnIndex++;
                    else
                        _isSpawn = false;

                }

                break;
        }
    }
    
}
