using UnityEngine;
using DataClasses = Data_Classes;

namespace NoteClasses
{
    public class NoteSlider : NoteBase
    {
        [Header("Slider Note Attributes")] 
        [SerializeField] private DataClasses.NoteData.SliderData data;

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

        //Start and End Position to lerp start note and end note of slider
        private Vector3 _startPosStartNote;
        private Vector3 _endPosStartNote;
        private Vector3 _startPosEndNote;
        private Vector3 _endPosEndNote;

        private float _sliderLockPointZ;    //position to lock slider start note when holding down 
        private bool _canMoveEndNote = true;

        private void Start()
        {
            SetUpLineControllers();
            SetUpVariables();
            ToggleLineRenderers(true);
        }

        private void Update()
        {
            if (!CanMove) return;
            InterpolateStartNotePos();
            if (!_canMoveEndNote) return;
            InterpolateEndNotePos();
        }

        private void InterpolateStartNotePos()
        {
            
        }
        
        private void SetUpLineControllers()
        {
            _lineControllers = GetComponentsInChildren<LineRendererController>();

            foreach (var line in _lineControllers)
            {
                line.SetUpLine(lineRendererPoints);
            }
        }

        private void ToggleLineRenderers(bool status)
        {
            foreach (var line in _lineControllers)
                line.gameObject.SetActive(status);
        }

        private void SetUpVariables()
        {
            //setting up spawn time stamps
            _startNoteSpawnTime = data.timeStampKeyDown - midiData.noteTime;
            _endNoteSpawnTime = data.timeStampKeyUp - midiData.noteTime;

            var tuple = midiData.GetHitPoint(noteOrientation, Vector3.forward);
            
            _startPosStartNote = tuple.Item1;
            _startPosEndNote = tuple.Item1;
            _endPosStartNote = tuple.Item2;
            _endPosEndNote = tuple.Item2;
            
            _sliderLockPointZ = tuple.Item3;
        }

        public void InitializeDataOnSpawn(ref int octave, ref DataClasses.NoteData.LaneOrientation orientation, ref DataClasses.NoteData.SliderData sliderData)
        {
            octaveNum = octave;
            noteOrientation = orientation;
            sliderData = data;
        }
    }
}