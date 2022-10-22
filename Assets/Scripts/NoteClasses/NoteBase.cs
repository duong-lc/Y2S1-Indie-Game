using UnityEngine;
using SO_Scripts;
using System;

namespace NoteClasses
{
    public abstract class NoteBase : MonoBehaviour
    {
        [Header("Base Note Attributes")] 
        protected bool CanMove = true;

        [SerializeField] protected MidiData midiData;
        [SerializeField] protected SO_Scripts.NoteData noteData;
        public int octaveNum;
        public Data_Classes.NoteData.LaneOrientation noteOrientation;


        protected virtual Tuple<Vector3, Vector3> GetCorePositions (Vector3 hitPoint, Vector3 dir, float spawnZ, float deSpawnZ)
        {
            Debug.Log("Calling from root class");
            return new Tuple<Vector3, Vector3>(Vector3.zero, Vector3.zero);
        }
    }
}