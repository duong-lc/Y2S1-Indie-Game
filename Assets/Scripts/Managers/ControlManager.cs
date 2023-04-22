using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Logging;
using NoteClasses;
using Sirenix.OdinInspector;
using UnityEngine;
using SO_Scripts;
using UnityEngine.InputSystem;
using EventType = Core.Events.EventType;
using NoteData = Data_Classes.NoteData;

namespace Managers
{
    public class ControlManager : MonoBehaviour
    {
        private MidiData _midiData;
        private GameModeData _gameModeData;
        [Space]
        // // [SerializeField] private List<Transform> anchorPoints = new List<Transform>();
        // [Space]
        // [SerializeField] private float castDist;
        // [SerializeField] private Vector3 boxSize;
        // [SerializeField] private LayerMask noteLayer;

        // [TitleGroup("Touch Input Properties")] 
        // [SerializeField] private PlayerInput playerInput;
        // [ReadOnly] public List<InputAction> multiTouchInputActions;
        
        // private Dictionary<NoteData.LaneOrientation, Vector3> _castOriginDict = new Dictionary<NoteData.LaneOrientation, Vector3>();
        private List<NoteSlider> _currentHoldSliders = new List<NoteSlider>();

        private Camera _camera;
        private Camera camera
        {
            get
            {
                if(_camera != null) return _camera;
                _camera = Camera.main;
                return _camera;
            }
        }

        private void Awake()
        {
            EventDispatcher.Instance.AddListener(EventType.OnUnPauseEvent, param => CheckHoldStatus());
            EventDispatcher.Instance.AddListener(EventType.OnRemoveSliderFromHoldList, param => RemoveSliderFromList((NoteSlider) param));
            
            
            // if(!playerInput) NCLogger.Log($"playerInput is not assigned", LogLevel.ERROR);
        }

        private void Start()
        {
            _midiData = GameModeManager.Instance.CurrentMidiData;
            _gameModeData = GameModeManager.Instance.GameModeData;
            if(!_midiData) NCLogger.Log($"midiData is {_midiData}", LogLevel.ERROR);
            if(!_gameModeData) NCLogger.Log($"midiData is {_gameModeData}", LogLevel.ERROR);
            
            // multiTouchInputActions.Add(playerInput.actions["Touch0"]);
            // multiTouchInputActions.Add(playerInput.actions["Touch1"]);
            // multiTouchInputActions.Add(playerInput.actions["Touch2"]);
            // multiTouchInputActions.Add(playerInput.actions["Touch3"]);
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (var kvp in _gameModeData.LaneControllerData)
            {
                if (GameModeManager.Instance.CurrentGameState != GameState.PlayMode) return;
                
                if (Input.GetKeyDown(kvp.Value.Input)) {
                    if (!NoteInteractInputDown(kvp.Value.collider)) continue;
                }

                if (Input.GetKeyUp(kvp.Value.Input)) {
                    if (!NoteInteractInputUp(kvp.Value.collider)) continue;
                }

            }
            // if(Input.touchCount > 0 && Input.touches[0].phase.Equals(UnityEngine.InputSystem.TouchPhase.Began))
            // {
            //     Ray ray = camera.ScreenPointToRay(Input.touches[0].position);
            //     RaycastHit hit;
            //     if(Physics.Raycast(ray, out hit))
            //     {
            //         if(hit.collider != null)
            //         {
            //             if (!NoteInteractInputDown(entry)) continue;
            //         }
            //     }
            // }
        }

        private bool NoteInteractInputDown(LaneCollider laneCollider)
        {
            // _castOriginDict.TryGetValue(entry.Value, out var castOrigin);
            //  if (!Physics.BoxCast(castOrigin,
            //          boxSize,
            //          Vector3.forward,
            //          out var hit,
            //          Quaternion.identity,
            //          castDist,
            //          noteLayer)) return false;
            //         
            //TODO: Instead of getting component, get the lane based on the keyEntry.
            //In said lane, there will be a List that store all Active Notes in scene  -List<NoteBase>
            //use linq to compare hit.object with notebase.object in list. If matches, return notebase with
            //game object tag, then use switch to cast to normal or slider. 
            //the purpose is to have less performance intensive code but haven't really measured - just a theory.
            var note = laneCollider.GetApproachingNote();
            switch (note.Type)
            {
                case NoteType.NormalNote:
                    ((note as NoteNormal)!).OnNoteHitNormalNote();
                    break;
                case NoteType.SliderNote:
                    ((note as NoteSlider)!).OnNoteHitStartNote();
                    _currentHoldSliders.Add(note as NoteSlider);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        private bool NoteInteractInputUp(LaneCollider laneCollider)
        {
            var slider = _currentHoldSliders.Find(x => x.noteOrientation == laneCollider.LaneOrientation);
            if (!slider) {
                _currentHoldSliders.Remove(slider);
                return false;
            }
                    
            var status = slider.OnNoteHitEndNote();

            _currentHoldSliders.Remove(slider);
            //TODO: replace with object pooling
            if(status) 
                Destroy(slider.gameObject);
            return true;
        }
        
        private void RemoveSliderFromList(NoteSlider slider)
        {
            StartCoroutine(DelayedRemoveSliderRoutine(slider));
        }

        /// <summary>
        /// Delay removing the slider note from holding list by 1 frame due to
        /// CheckHoldStatus() still iterating through the list.
        /// </summary>
        /// <param name="slider"></param>
        /// <returns></returns>
        private IEnumerator DelayedRemoveSliderRoutine(NoteSlider slider)
        {
            yield return null;
            _currentHoldSliders.Remove(slider);
            Destroy((slider.gameObject));
        }

        private void CheckHoldStatus()
        {
            foreach (var slider in _currentHoldSliders) {
                slider.CheckHoldingStatus();
            }
        }
        
        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     foreach (var anchor in anchorPoints) {
        //         Gizmos.DrawWireCube(anchor.position, boxSize*2);
        //         Gizmos.DrawWireCube(anchor.position + Vector3.forward * castDist, boxSize*2);
        //     }
        // }

        // private Transform GetAnchorPoint(string tag) => anchorPoints.Find(t => t.CompareTag(tag));
    }
}
