using System;
using System.Collections;
using Core.Events;
using Core.Logging;
using Managers;
using UnityEngine;
using Data_Classes;
using DG.Tweening;
using Melanchall.DryWetMidi.MusicTheory;
using EventType = Core.Events.EventType;

namespace NoteClasses
{
    public class NoteNormal : NoteBase
    {
        [Header("Default Note Attributes")]
        private double _timeInstantiated; //time to instantiate the note
        public double assignedTime;//the time the note needs to be tapped by the player

        private Vector3 _startPos;
        private Vector3 _endPos;

        private double TimeSinceInstantiated => CurrentSongTimeRaw - _timeInstantiated;
        private float Alpha => ((float)(TimeSinceInstantiated / (NoteTime * 2)));
        private HitCondition hitCond;
        
        [SerializeField] private GameObject outline;
        private SpriteRenderer _outlineSR;
        private bool _doOnce;
        protected void Awake()
        {
            _outlineSR = outline.GetComponent<SpriteRenderer>();
        }
        
        protected override void Start()
        {
            base.Start();
           
        }

       

        public override void Init(PooledObjectCallbackData data, Action<PooledObjectBase> killAction)
        {
            var noteData = (NoteInitData)data;
            octaveNum = noteData.octave;
            noteOrientation = noteData.orientation;
            assignedTime = noteData.timeStamp;

            SetUpVariables();
            SetLookDir(_startPos, _endPos);
            
            KillAction = killAction;
            canRelease = false;

            _outlineSR.color = new Color(_outlineSR.color.r, _outlineSR.color.g, _outlineSR.color.b, 0);
            outline.transform.localScale = Vector3.one * 5;
            _doOnce = true;

            ToggleChildren(true);
            StartCoroutine(RunRoutine());
        }
        
        public override IEnumerator RunRoutine()
        {
            while (!canRelease) {
                if (NoteTime - TimeSinceInstantiated <= 1/* && _doOnce*/)
                {
                    // _doOnce = false;
                    _outlineSR.DOFade(.8f, 1f);
                    outline.transform.DOScale(1.1f, 1f);
                }
                
                yield return null;
            }
            
            KillAction(this);
            yield return null;
        }
        
        
        private void Update()
        {
            // NCLogger.Log($"current note hit cond {hitCond} ");
            
            if (GameModeManager.Instance.CurrentGameState != GameState.PlayMode) {
                // NCLogger.Log($"GameState should be PlayMode when it's {GameModeManager.Instance.CurrentGameState}", LogLevel.ERROR);
                return;
            }
            OnNoteMissNormalNote();
            
            if (!CanMove) return;
            InterpolateNotePos();
        }

        private void InterpolateNotePos()
        {
            if(Alpha <= 1f)//otherwise, the position of note will be lerped between the spawn position and despawn position based on the alpha
            {
                transform.position = Vector3.Lerp(_startPos, _endPos, Alpha);
            }
            else
            {
                NCLogger.Log($"go pass earth bound");
                //Destroy(gameObject);
                canRelease = true;
            }
        }


        public bool OnNoteHitNormalNote() {
            // if (Math.Abs(CurrentSongTimeAdjusted - assignedTime) < MarginOfError) //hitting the note within the margin of error
            // {
            //     //Hit
            //     //NCLogger.Log($"hit normal good");
            //     EventDispatcher.Instance.FireEvent(EventType.OnNoteHitEvent, noteOrientation);
            //     Destroy(gameObject);
            // }
            
            hitCond = GetHitCondition(CurrentSongTimeAdjusted , assignedTime, ref noteHitEvent);
            if (hitCond != HitCondition.None && hitCond != HitCondition.Miss) {
                this.FireEvent(noteHitEvent, new HitMarkInitData(this, hitCond, noteOrientation));
                // NCLogger.Log($"hit the mf wall");
                // Destroy(gameObject);
                canRelease = true;
                return true;
            }

            return false;
        }

        public void OnNoteMissNormalNote() {
            if (canRelease) return;
            
            hitCond = GetHitCondition(CurrentSongTimeAdjusted , assignedTime, ref noteHitEvent);
            if (hitCond == HitCondition.Miss) {
                EventDispatcher.Instance.FireEvent(noteHitEvent,  new HitMarkInitData(this, hitCond, noteOrientation));
                canRelease = true;
            }
        }
        
        
        
        public void InitializeDataOnSpawn(ref int octave, ref NoteData.LaneOrientation laneOrientation, ref double timeStamp)
        {
            octaveNum = octave;
            noteOrientation = laneOrientation;//pass the orientation property
            assignedTime = timeStamp;//get the time the note should be tapped by player and add to the array
        }

        private void SetUpVariables()
        {
            _timeInstantiated = SongManager.Instance.GetAudioSourceTimeRaw();
            GameModeManager.Instance.GameModeData.GetLerpPoints(noteOrientation, ref _startPos, ref _endPos);
        }
    }
}