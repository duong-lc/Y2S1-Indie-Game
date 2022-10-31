using System;
using Managers;
using UnityEngine;
using Data_Classes;
using Melanchall.DryWetMidi.MusicTheory;

namespace NoteClasses
{
    public class NoteNormal : NoteBase
    {
        [Header("Default Note Attributes")]
        private double _timeInstantiated; //time to instantiate the note
        public double assignedTime;//the time the note needs to be tapped by the player

        private Vector3 _startPos;
        private Vector3 _endPos;

        private void Start()
        {
            SetUpVariables();
        }

        private void Update()
        {
            OnNoteMissNormalNote();
            
            if (!CanMove) return;
            InterpolateNotePos();
        }

        private void InterpolateNotePos()
        {
            double timeSinceInstantiated = SongManager.GetAudioSourceTime() - _timeInstantiated;
            //divide that with the time between the spawn Y and despawn Y to get the alpha position of the note relative to its total travel dist
            float alpha = (float)(timeSinceInstantiated / (midiData.noteTime * 2));

            if(alpha <= 1f)//otherwise, the position of note will be lerped between the spawn position and despawn position based on the alpha
            {
                transform.position = Vector3.Lerp(_startPos, _endPos, alpha);
            }
            else
            {
                
            }
        }


        public void OnNoteHitNormalNote()
        {
            //double currAudioTime = SongManager.GetAudioSourceTime() - (midiData.inputDelayInMilliseconds / 1000.0);

            if (Math.Abs(SongManager.Instance.GetCurrentAudioTime() - assignedTime) < MarginOfError) //hitting the note within the margin of error
            {
                //Hit
            }
        }

        public void OnNoteMissNormalNote()
        {
            if (assignedTime + MarginOfError <= SongManager.Instance.GetCurrentAudioTime())
            {
                //Miss
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
            _timeInstantiated = SongManager.GetAudioSourceTime();

            var tuple = midiData.GetHitPoint(noteOrientation, Vector3.forward);
            _startPos = tuple.Item1;
            _endPos = tuple.Item2;
        }
    }
}