using System;
using Core.Events;
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
        private void Start()
        {
            SetUpVariables();
            SetLookDir(_startPos, _endPos);
        }

        private void Update()
        {
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
            //double currAudioTime = SongManager.GetAudioSourceTime() - (midiData.inputDelayInMilliseconds / 1000.0);
            if (Math.Abs(SongManager.Instance.GetAudioSourceTimeAdjusted() - assignedTime) < MarginOfError) //hitting the note within the margin of error
            {
                //Hit
                EventDispatcher.Instance.FireEvent(EventType.OnNoteHitEvent);
            }
            
        }

        public void OnNoteMissNormalNote()
        {
            if (assignedTime + MarginOfError <= CurrentSongTimeAdjusted)
            {
                //Miss
                EventDispatcher.Instance.FireEvent(EventType.OnNoteHitEvent);
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
            _timeInstantiated = SongManager.GetAudioSourceTimeRaw();

            var tuple = midiData.GetHitPoint(noteOrientation, Vector3.forward);
            _startPos = tuple.Item1;
            _endPos = tuple.Item2;
        }
    }
}