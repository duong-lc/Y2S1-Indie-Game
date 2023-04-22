using System;
using Core.Events;
using Core.Logging;
using Managers;
using UnityEngine;
using Data_Classes;
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
        protected override void Start()
        {
            base.Start();
            SetUpVariables();
            SetLookDir(_startPos, _endPos);
        }

        public override void Init(PooledObjectCallbackData data, Action<PooledObjectBase> killAction)
        {
            var noteData = (NoteInitData)data;
            octaveNum = noteData.octave;
            noteOrientation = noteData.orientation;
            assignedTime = noteData.timeStamp;

            KillAction = killAction;
            canRelease = false;
            StartCoroutine(RunRoutine());
        }
        
        private void Update()
        {
            if (GameModeManager.Instance.CurrentGameState != GameState.PlayMode) {
                NCLogger.Log($"GameState should be PlayMode when it's {GameModeManager.Instance.CurrentGameState}", LogLevel.ERROR);
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


        public void OnNoteHitNormalNote() {
            // if (Math.Abs(CurrentSongTimeAdjusted - assignedTime) < MarginOfError) //hitting the note within the margin of error
            // {
            //     //Hit
            //     //NCLogger.Log($"hit normal good");
            //     EventDispatcher.Instance.FireEvent(EventType.OnNoteHitEvent, noteOrientation);
            //     Destroy(gameObject);
            // }
            
            var cond = GetHitCondition(CurrentSongTimeAdjusted - assignedTime);
            if (cond != HitCondition.None && cond != HitCondition.Miss) {
                EventDispatcher.Instance.FireEvent(EventType.OnNoteHitEvent, new NoteRegisterParam(cond, noteOrientation));
                NCLogger.Log($"hit the mf wall");
                // Destroy(gameObject);
                canRelease = true;
            }
        }

        public void OnNoteMissNormalNote() {
            // if (assignedTime + MarginOfError <= CurrentSongTimeAdjusted)
            // {
            //     //Miss
            //     EventDispatcher.Instance.FireEvent(EventType.OnNoteMissEvent, noteOrientation);
            //     Destroy(gameObject);
            // }
            if (CurrentSongTimeAdjusted >= assignedTime) return;
            if (GetHitCondition(CurrentSongTimeAdjusted - assignedTime) == HitCondition.Miss) {
                NCLogger.Log($"current {CurrentSongTimeAdjusted} assigned {assignedTime} hit cond {_gameModeData.GetHitCondition(CurrentSongTimeAdjusted - assignedTime)}");
                Debug.Break();
                var exm = GetHitCondition(CurrentSongTimeAdjusted - assignedTime);
                EventDispatcher.Instance.FireEvent(EventType.OnNoteMissEvent, noteOrientation);
                // Destroy(gameObject);
                canRelease = true;
            }
        }
        
        
        public HitCondition GetHitCondition(double hitOffset)
        {
            //hitOffset = CurrentSongTimeAdjusted - AssignedTime
            foreach (var kvp in _gameModeData.NoteHitCondDict) {
                var margin = kvp.Value;
                var offset = Mathf.Abs((float)hitOffset);
                switch (hitOffset) {
                    case < 0:
                        switch (kvp.Key) {
                            case HitCondition.None:
                                if (offset > Mathf.Abs(_gameModeData.GetMOE(HitCondition.Early).EndMOE)) {
                                    return HitCondition.None;
                                }
                                break;
                            case HitCondition.EarlyPerfect:
                                if (offset >= Mathf.Abs(margin.BeginMOE) && offset < Mathf.Abs(margin.EndMOE)) {
                                    return HitCondition.EarlyPerfect;
                                }
                                break;
                            case HitCondition.Early:
                                if (offset >= Mathf.Abs(margin.BeginMOE) && offset < Mathf.Abs(margin.EndMOE)) {
                                    return HitCondition.Early;
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
                            case HitCondition.LatePerfect:
                                if (offset >= Mathf.Abs(margin.BeginMOE) && offset < Mathf.Abs(margin.EndMOE)) {
                                    return HitCondition.LatePerfect;
                                }
                                break;
                            case HitCondition.Late:
                                if (offset >= Mathf.Abs(margin.BeginMOE) && offset < Mathf.Abs(margin.EndMOE)) {
                                    return HitCondition.Late;
                                }
                                break;
                            case HitCondition.Miss:
                                if (offset > Mathf.Abs(_gameModeData.GetMOE(HitCondition.Late).EndMOE)) {
                                    NCLogger.Log($"offset {hitOffset} > {Mathf.Abs(_gameModeData.GetMOE(HitCondition.Late).EndMOE)} is {offset > Mathf.Abs(_gameModeData.GetMOE(HitCondition.Late).EndMOE)}");
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
            return HitCondition.None;
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