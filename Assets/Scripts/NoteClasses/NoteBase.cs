using UnityEngine;
using SO_Scripts;
using System;
using Managers;

namespace NoteClasses
{
    public abstract class NoteBase : MonoBehaviour
    {
        [Header("Base Note Attributes")] 
        protected bool CanMove = true;
        protected double MarginOfError;
        protected static double CurrentSongTimeAdjusted => SongManager.Instance.GetAudioSourceTimeAdjusted();
        protected static double CurrentSongTimeRaw => SongManager.GetAudioSourceTimeRaw();
        
        [SerializeField] protected MidiData midiData;
        [SerializeField] protected SO_Scripts.NoteData noteData;
        public int octaveNum;
        public Data_Classes.NoteData.LaneOrientation noteOrientation;

        private Collider _collider;
        public Collider Collider {
            get {
                if (!_collider) _collider = GetComponent<Collider>();
                return _collider;
            }
        }

        private void Awake()
        {
            MarginOfError = midiData.marginOfError;
        }

        protected virtual void SetLookDir(Vector3 startPos, Vector3 endPos)
        {
            transform.forward = (endPos - startPos).normalized;
        }

    }
}