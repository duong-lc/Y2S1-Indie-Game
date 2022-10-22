using System;
using UnityEngine;
using Data_Classes;
using System.Collections;
using System.Collections.Generic;

namespace SO_Scripts
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Midi Data", order = 1)]
    public class MidiData : ScriptableObject
    {
       [Header("MIDI Related & Time Offset Data")]
       public AudioClip songClip;
       
       public float songDelayInSeconds; //delay the song after a certain amount of time
       public double marginOfError;//how much off the player can press the note and still consider to be a hit (in seconds)
       public int inputDelayInMilliseconds; //it's the issue with the keyboard and we need to have input delay 
   
       public string fileLocation; //file location for the MIDI file
       public float noteTime; //how much time the note is going to be on the screen
       public float noteSpawnZ; //the Z position for the note to be spawned at
       public float noteTapZ; //the Z position where the player should press the note

       private Vector3 _startPos;
       private Vector3 _endPos;
       
       public float noteDespawnZ //De-spawn position for notes
       {
           get
           {
               return noteTapZ - (noteSpawnZ - noteTapZ);
           } 
       }
       
       [Header("Note Prefabs")]
       public GameObject noteNormalPrefab;
       public GameObject noteSliderPrefab;

       [Header("Note Hit Locations")] 
       public Vector3 hitPoint1;
       public Vector3 hitPoint2;
       public Vector3 hitPoint3;
       public Vector3 hitPoint4;
       
       [Header("Lane Data")] 
       public const int LaneCount = 4;
       public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestrictionNormalNote;
       public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestrictionSliderNote;

       [Header("Lane 1")]
       public KeyCode input1;//key input for that lane
       //note on midi file to spawn and note object prefab to spawn
       [Range(0, 8)] public int laneOctave1;
       public List<BaseNoteType> AllNoteOnLaneList1 = new List<BaseNoteType>();//list of timestamps of all the notetypes 
       
       [Header("Lane 2")]
       public KeyCode input2;
       [Range(0, 8)] public int laneOctave2;
       public List<BaseNoteType> AllNoteOnLaneList2 = new List<BaseNoteType>();
       
       [Header("Lane 3")]
       public KeyCode input3;
       [Range(0, 8)] public int laneOctave3;
       public List<BaseNoteType> AllNoteOnLaneList3 = new List<BaseNoteType>();

       [Header("Lane 4")]
       public KeyCode input4;
       [Range(0, 8)] public int laneOctave4;
       public List<BaseNoteType> AllNoteOnLaneList4 = new List<BaseNoteType>();

       private void OnValidate()
       {
           
       }
       
       public void Get
    }
}
