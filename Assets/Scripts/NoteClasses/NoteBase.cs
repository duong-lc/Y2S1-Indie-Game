using UnityEngine;
using SO_Scripts;
using System;
using Core.Events;
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
        
        
    }
}