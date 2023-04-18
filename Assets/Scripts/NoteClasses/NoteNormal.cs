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
        private float Alpha => ((float)(TimeSinceInstantiated / (midiData.noteTime * 2)));
        protected override void Start()
        {
            base.Start();
            SetUpVariables();
            SetLookDir(_startPos, _endPos);
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
                Destroy(gameObject);
            }
        }


        public void OnNoteHitNormalNote()
        {
            //NCLogger.Log($"current: {CurrentSongTimeAdjusted} assigned: {assignedTime}\n{Math.Abs(CurrentSongTimeAdjusted - assignedTime)} startPos: {_startPos} endPos: {_endPos}");
            //double currAudioTime = SongManager.GetAudioSourceTime() - (midiData.inputDelayInMilliseconds / 1000.0);
            if (Math.Abs(CurrentSongTimeAdjusted - assignedTime) < MarginOfError) //hitting the note within the margin of error
            {
                //Hit
                //NCLogger.Log($"hit normal good");
                EventDispatcher.Instance.FireEvent(EventType.OnNoteHitEvent, noteOrientation);
                Destroy(gameObject);
            }
            
        }

        public void OnNoteMissNormalNote()
        {
            if (assignedTime + MarginOfError <= CurrentSongTimeAdjusted)
            {
                //Miss
                EventDispatcher.Instance.FireEvent(EventType.OnNoteMissEvent, noteOrientation);
                Destroy(gameObject);
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

            var tuple = midiData.GetHitPoint(noteOrientation, Vector3.forward);
            _startPos = tuple.Item1;
            _endPos = tuple.Item2;
        }
    }
}