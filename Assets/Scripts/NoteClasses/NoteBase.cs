using UnityEngine;
using SO_Scripts;
using System;
using Core.Events;
using Core.Logging;
using Managers;
using EventType = Core.Events.EventType;

public enum NoteType
{
    Undefined,
    NormalNote,
    SliderNote,
}
namespace NoteClasses
{
    [RequireComponent(typeof(Collider))]
    public abstract class NoteBase : PooledObjectBase
    {
        [Header("Base Note Attributes")] 
        protected bool CanMove = true;

        [SerializeField] protected NoteType type;
        
        protected static double CurrentSongTimeAdjusted => SongManager.Instance.GetAudioSourceTimeAdjusted();
        protected static double CurrentSongTimeRaw => SongManager.Instance.GetAudioSourceTimeRaw();
        
        // [SerializeField] protected MidiData midiData;
        [SerializeField] protected SO_Scripts.NoteData noteData;
        public int octaveNum;
        public Data_Classes.NoteData.LaneOrientation noteOrientation;

        private Collider _collider;
        protected GameModeData _gameModeData;
        protected static float NoteTime => GameModeManager.Instance.GameModeData.NoteTime;
        public Collider Collider {
            get {
                if (!_collider) _collider = GetComponent<Collider>();
                return _collider;
            }
        }

        public NoteType Type => type;
        
        protected void Awake()
        {
        }
        

        protected virtual void SetLookDir(Vector3 startPos, Vector3 endPos)
        {
            transform.forward = (endPos - startPos).normalized;
        }

        protected virtual void Start()
        {
            base.Start();
            _gameModeData = GameModeManager.Instance.GameModeData;
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
    }
}