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

        private void Start()
        {
            _timeInstantiated = SongManager.GetAudioSourceTime();

            Tuple<Vector3, Vector3> tuple;
            Vector3 hitPoint;
            switch (noteOrientation)
            {
                case NoteData.LaneOrientation.One:
                    hitPoint = midiData.hitPoint1;
                    break;
                case NoteData.LaneOrientation.Two:
                    hitPoint = midiData.hitPoint2;
                    break;
                case NoteData.LaneOrientation.Three:
                    hitPoint = midiData.hitPoint3;
                    break;
                case NoteData.LaneOrientation.Four:
                    hitPoint = midiData.hitPoint4;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            tuple = GetCorePositions(midiData.hitPoint1, Vector3.forward, midiData.noteSpawnZ, midiData.noteDespawnZ);
            _startPos = tuple.Item1;
            _endPos = tuple.Item2;
        }
        
        protected override Tuple<Vector3, Vector3> GetCorePositions (Vector3 hitPoint, Vector3 dir, float spawnZ, float deSpawnZ)
        {
            Vector3 startPos = hitPoint + (dir * spawnZ);
            Vector3 endPos = hitPoint + (dir * deSpawnZ);
            return new Tuple<Vector3, Vector3>(startPos, endPos);
        }
    }
}