using UnityEngine;
using SO_Scripts;
using System;

namespace NoteClasses
{
    public abstract class NoteBase : MonoBehaviour
    {
        [Header("Base Note Attributes")] 
        protected bool CanMove = true;
        protected double MarginOfError;

        [SerializeField] protected MidiData midiData;
        [SerializeField] protected SO_Scripts.NoteData noteData;
        public int octaveNum;
        public Data_Classes.NoteData.LaneOrientation noteOrientation;


        private void Awake()
        {
            MarginOfError = midiData.marginOfError;
        }
    }
}