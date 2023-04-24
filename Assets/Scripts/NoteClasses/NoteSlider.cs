using System;
using Core.Events;
using Core.Logging;
using Managers;
using Unity.VisualScripting;
using UnityEngine;
using DataClasses = Data_Classes;
using EventType = Core.Events.EventType;

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
        private float AlphaStart => (float)(TimeSinceStartNoteSpawned / (NoteTime * 2));
        private float  AlphaEnd => (float)(TimeSinceEndNoteSpawned / (NoteTime * 2));
        private bool _runOnce = true;
        private bool _runOnce1 = true;
        private KeyCode _holdKey;
        private bool denyInput = false;

        protected void Awake()
        {
            base.Awake();
            _lineControllers = GetComponentsInChildren<LineRendererController>();
        }
        
        protected override void Start()
        {
            base.Start();
            // SetUpVariables();
            // ToggleLineRenderers(true);
            // SetUpLineControllers();
            // SetLookDir(_startPosStartNote, _endPosStartNote);
        }

        public override void Init(PooledObjectCallbackData data, Action<PooledObjectBase> killAction)
        {
            //Re-arm values for slider
            denyInput = false;
            _isStartNoteHitCorrect = false;
            _canMoveStartNote = true;
            _canMoveEndNote = true;
            
            var noteData = (NoteInitData)data;
            octaveNum = noteData.octave;
            noteOrientation = noteData.orientation;
            _sliderData = noteData.SliderData;

            SetUpVariables();
            ToggleLineRenderers(true);
            SetUpLineControllers();
            SetLookDir(_startPosStartNote, _endPosStartNote);

            KillAction = killAction;
            canRelease = false;
            StartCoroutine(RunRoutine());
        }
        
        private void Update()
        {
            //UpdateStartNoteHoldStatus();
            if (GameModeManager.Instance.CurrentGameState != GameState.PlayMode) {
                NCLogger.Log($"GameState should be PlayMode when it's {GameModeManager.Instance.CurrentGameState}", LogLevel.ERROR);
                return;
            }
            if (_canMoveStartNote ) {
                InterpolateStartNotePos();
            }
            if (_canMoveEndNote ) {
                InterpolateEndNotePos();
            }

            UpdateStartNoteFail();
            UpdateEndNoteHoldStatus();
        }

        private void InterpolateStartNotePos()
        {
            if (AlphaStart > 1) 
            {
                //Destroy(startNote);
                canRelease = true;
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
        public bool OnNoteHitStartNote()
        {
            if (denyInput) return false;
            if (_isStartNoteHitCorrect && _isHolding) return false;
            
            var cond = GetHitCondition(CurrentSongTimeAdjusted , _sliderData.timeStampKeyDown, ref noteHitEvent);
            if (cond != HitCondition.None && cond != HitCondition.Miss) {
                //Hit
                this.FireEvent(noteHitEvent,  new HitMarkInitData(this, cond, noteOrientation));
                
                //Setting condition for endNote evaluation
                _isStartNoteHitCorrect = true;
                _isHolding = true;
                
                _canMoveStartNote = false;
                startNote.transform.position = _sliderLockPoint;
                return true;
            }

            return false;
        }

        private void UpdateStartNoteFail()//put in update
        {
            if (canRelease) return;
            if (_isHolding && _isStartNoteHitCorrect) return;
            //Doesn't press, let start note passes
            var cond = GetHitCondition(CurrentSongTimeAdjusted , _sliderData.timeStampKeyDown, ref noteHitEvent);
            if (cond == HitCondition.Miss && !_isStartNoteHitCorrect) {
                EventDispatcher.Instance.FireEvent(noteHitEvent, new HitMarkInitData(this, cond, noteOrientation));
                // Destroy(gameObject);
                canRelease = true;
            }
        }

        private void UpdateStartNoteHoldStatus()//put in update
        {
            if (_isStartNoteHitCorrect && Input.GetKeyUp(_holdKey))
            {
                _isHolding = false;
                NCLogger.Log($"up!!");
            }
        }

        public bool OnNoteHitEndNote()//when release key
        {
            if (denyInput) return false;
            bool isDestroy = false;
            if (!Input.GetKey(_holdKey) && _isStartNoteHitCorrect && _isHolding)
            {
                _isHolding = false;
                var cond = GetHitCondition(CurrentSongTimeAdjusted , _sliderData.timeStampKeyUp, ref noteHitEvent);
                if (cond != HitCondition.None && cond != HitCondition.Miss)
                {
                    NCLogger.Log($"release the slider NOT miss");
                    //Hit
                    _isEndNoteHitCorrect = true;

                    isDestroy = true;
                    EventDispatcher.Instance.FireEvent(EventType.RemoveSliderFromHoldListEvent, this);
                    EventDispatcher.Instance.FireEvent(noteHitEvent,  new HitMarkInitData(this, cond, noteOrientation));
                    denyInput = true;
                    canRelease = true;
                }
                else if (cond == HitCondition.Miss || cond == HitCondition.None)
                {
                    NCLogger.Log($"release the slider MISS");
                    isDestroy = true;
                    //release too early
                    //miss
                    EventDispatcher.Instance.FireEvent(EventType.RemoveSliderFromHoldListEvent, this);
                    EventDispatcher.Instance.FireEvent(EventType.NoteMissEvent, new HitMarkInitData(this, cond, noteOrientation));
                    denyInput = true;
                    canRelease = true;
                }
            }

            return isDestroy;
        }

        private void UpdateEndNoteHoldStatus()
        {
            if (canRelease) return;
            if (_isStartNoteHitCorrect && _isHolding)
            {
                //release too late <- will probably throw away as this game mode does not account for late releases.
                if (_sliderData.timeStampKeyUp + _gameModeData.SliderHoldStickyTime <= CurrentSongTimeAdjusted) {
                    //hit - since passes the end note, auto hit
                    this.FireEvent(EventType.NoteHitLateEvent,  new HitMarkInitData(this, HitCondition.Late, noteOrientation));
                    this.FireEvent(EventType.RemoveSliderFromHoldListEvent, this);
                    //Destroy(gameObject);
                    canRelease = true;
                }
            }
        }

        public void KillSlider()
        {
            canRelease = true;
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
            
            //setting up spawn time stamps
            _startNoteSpawnTime = _sliderData.timeStampKeyDown - NoteTime;
            _endNoteSpawnTime = _sliderData.timeStampKeyUp - NoteTime;

            GameModeManager.Instance.GameModeData.GetLerpPoints(noteOrientation, ref _startPosStartNote, ref _endPosStartNote);
            _sliderLockPoint = GameModeManager.Instance.GameModeData.GetHitPoint(noteOrientation);
            
            _startPosEndNote = _startPosStartNote;
            _endPosEndNote = _endPosStartNote;
            _runOnce = _runOnce1 = true;
            _holdKey = GameModeManager.Instance.GameModeData.GetKeyCode(noteOrientation);
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