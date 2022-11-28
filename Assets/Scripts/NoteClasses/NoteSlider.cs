using System;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using DataClasses = Data_Classes;

namespace NoteClasses
{
    public class NoteSlider : NoteBase
    {
        [Header("Slider Note Attributes")] 
        private DataClasses.NoteData.SliderData _sliderData;

        [SerializeField] private GameObject startNote;
        [SerializeField] private GameObject endNote;

        private double _startNoteSpawnTime;
        private double _endNoteSpawnTime;

        [SerializeField] private Transform[] lineRendererPoints;
        private LineRendererController[] _lineControllers;

        //Booleans to check note hit reg status
        private bool _isStartNoteHitCorrect = false;//is pressed down on start note correctly?
        private bool _isHolding = false;            //is slider being held on correctly?
        private bool _isEndNoteHitCorrect = false;  //is released on end note correctly?
        private bool _canMoveEndNote = true;
        private bool _canMoveStartNote = true;
        
        
        //Start and End Position to lerp start note and end note of slider
        private Vector3 _startPosStartNote;
        private Vector3 _endPosStartNote;
        private Vector3 _startPosEndNote;
        private Vector3 _endPosEndNote;

        private Vector3 _sliderLockPoint;    //position to lock slider start note when holding down 
        
        //caching to lighten garbage collectors
        private double TimeSinceStartNoteSpawned => CurrentSongTimeRaw - _startNoteSpawnTime;
        private double TimeSinceEndNoteSpawned => CurrentSongTimeRaw - _endNoteSpawnTime;
        private float AlphaStart => (float)(TimeSinceStartNoteSpawned / (midiData.noteTime * 2));
        private float  AlphaEnd => (float)(TimeSinceEndNoteSpawned / (midiData.noteTime * 2));
        private bool _runOnce = true;
        private bool _runOnce1 = true;
        

        private void Start()
        {
            SetUpVariables();
            ToggleLineRenderers(true);
            SetUpLineControllers();
        }

        private void Update()
        {
            if (CanMove) {
                InterpolateStartNotePos();
            }
            if (_canMoveEndNote) {
                InterpolateEndNotePos();
            }
            
            
            
        }

        private void InterpolateStartNotePos()
        {
            if (AlphaStart > 1) 
            {
                Destroy(startNote);
            }
            else if (startNote)
            {
                ActivateStartNote();
                startNote.transform.position = Vector3.Lerp(_startPosStartNote, _endPosStartNote, AlphaStart);
            }
        }

        private void InterpolateEndNotePos()
        {
            //when the end note is spawnable/movable
            if(CurrentSongTimeRaw >= _endNoteSpawnTime)
            {
                if (!endNote) return; 
                //otherwise, the position of note will be lerped between the spawn position and despawn position based on the alpha
                if (Math.Abs(endNote.transform.position.z - _sliderLockPoint.z) > 0 && AlphaEnd < 0.5f)
                {
                    endNote.transform.position = Vector3.Lerp(_startPosEndNote, _endPosEndNote, AlphaEnd);
                    ActivateEndNote();
                }
                else
                {
                    _canMoveEndNote = false;
                    endNote.transform.position = startNote.transform.position;
                }
            }
            else
            {
                endNote.transform.position = _startPosEndNote;
            }
        }

        /// <summary>
        /// Called when pressing hit button or smt
        /// </summary>
        private void OnNoteHitStartNote()
        {
            if (Math.Abs(CurrentSongTimeAdjusted - _sliderData.timeStampKeyDown) < midiData.marginOfError)
            {
                //Hit
                
                //Setting condition for endNote evaluation
                _isStartNoteHitCorrect = true;
                _canMoveStartNote = false;
                //Locking note position to hit point side based on its current side (left or right)
                startNote.transform.position = _sliderLockPoint;
            }
        }

        private void UpdateStartNoteFail()//put in update
        {
            //Doesn't press, let start note passes
            if (_sliderData.timeStampKeyDown + MarginOfError <= CurrentSongTimeAdjusted && !_isStartNoteHitCorrect)
            {
                //Miss
            }
        }

        private void UpdateStartNoteHoldStatus()//put in update
        {
            //if(Input.GetKey(holdKey)){}
            if (_isStartNoteHitCorrect)
            {
                _isHolding = true;
            }
        }

        private void OnNoteHitEndNote()
        {
            if (_isStartNoteHitCorrect && _isHolding)
            {
                if (Math.Abs(CurrentSongTimeAdjusted - _sliderData.timeStampKeyUp) < MarginOfError)
                {
                    //Hit

                    _isEndNoteHitCorrect = true;
                }
            }
        }

        private void UpdateEndNoteFailStatus()
        {
            if (_isStartNoteHitCorrect && _isHolding)
            {
                //release too early
                if (Math.Abs(CurrentSongTimeAdjusted - _sliderData.timeStampKeyUp) >= MarginOfError)
                {
                    //miss
                }
                //release too late <- will probably throw away as this game mode does not account for late releases.
                else if (_sliderData.timeStampKeyUp + MarginOfError <= CurrentSongTimeAdjusted)
                {
                    //miss
                }
            }
        }


        #region Initializer Methods
        private void ActivateStartNote()
        {
            if (!_runOnce) return;
            startNote.SetActive(true);
            foreach (var line in _lineControllers)
                line.gameObject.SetActive(true);
            _runOnce = false;
        }

        private void ActivateEndNote()
        {
            if (!_runOnce1) return;
            endNote.SetActive(false);
            _runOnce1 = false;
        }
        
        private void SetUpLineControllers()
        {
            foreach (var line in _lineControllers) {
                line.SetUpLine(lineRendererPoints);
            }
        }

        private void ToggleLineRenderers(bool status)
        {
            foreach (var line in _lineControllers)
            {
                line.gameObject.SetActive(status);
            }
        }

        private void SetUpVariables()
        {
            _lineControllers = GetComponentsInChildren<LineRendererController>();
            //setting up spawn time stamps
            _startNoteSpawnTime = _sliderData.timeStampKeyDown - midiData.noteTime;
            _endNoteSpawnTime = _sliderData.timeStampKeyUp - midiData.noteTime;

            var tuple = midiData.GetHitPoint(noteOrientation, Vector3.forward);
            
            _startPosStartNote = tuple.Item1;
            _startPosEndNote = tuple.Item1;
            _endPosStartNote = tuple.Item2;
            _endPosEndNote = tuple.Item2;
            
            _sliderLockPoint = tuple.Item3;

            _runOnce = _runOnce1 = true;
        }

        public void InitializeDataOnSpawn(ref int octave, ref DataClasses.NoteData.LaneOrientation orientation, ref DataClasses.NoteData.SliderData sliderData)
        {
            octaveNum = octave;
            noteOrientation = orientation;
            _sliderData = sliderData;
        }
        
        #endregion
    }
}