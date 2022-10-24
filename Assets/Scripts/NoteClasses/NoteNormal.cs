using System;
using Managers;
using UnityEngine;
using Data_Classes;

namespace NoteClasses
{
    public class NoteNormal : NoteBase
    {
        [Header("Default Note Attributes")]
        private double _timeInstantiated; //time to instantiate the note
        public double assignedTime;//the time the note needs to be tapped by the player

        private Vector3 _startPos;
        private Vector3 _endPos;

        private int _indexOnLaneList;
        
        private void Start()
        {
            _timeInstantiated = SongManager.GetAudioSourceTime();

            var tuple = midiData.GetHitPoint(noteOrientation, Vector3.forward);
            _startPos = tuple.Item1;
            _endPos = tuple.Item2;
        }

        private void Update()
        {
            if (!CanMove) return;
            
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
        }


        public void OnNoteRegisterNormalNote()
        {
            double currAudioTime = SongManager.GetAudioSourceTime() - (midiData.inputDelayInMilliseconds / 1000.0);

            if (Math.Abs(currAudioTime - assignedTime) < MarginOfError) //hitting the note within the margin of error
            {
                
            }
        }
        
        
        //might not use this function so remember to delete if it isn't used
        public void SetIndexOnLaneList(int indexToSet)
        {
            _indexOnLaneList = indexToSet;
        }
    }
}