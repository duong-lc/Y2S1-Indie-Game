using System;
using UnityEngine;
using Data_Classes;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Melanchall.DryWetMidi.MusicTheory;
using Sirenix.OdinInspector;



[Serializable]
public class LaneMidiData
{
    [SerializeField] [Range(0, 8)] private int laneOctave;
    [ReadOnly] public List<BaseNoteType> allNoteOnLaneList = new();

    public int LaneOctave => laneOctave;
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Midi Data", order = 1)]
public class MidiData : SerializedScriptableObject
{
   [Header("MIDI Related & Time Offset Data")]
   public AudioClip songClip;
   
   public float songDelayInSeconds; //delay the song after a certain amount of time
   // public double marginOfError;//how much off the player can press the note and still consider to be a hit (in seconds)
   // public int inputDelayInMilliseconds; //it's the issue with the keyboard and we need to have input delay 

   public string fileLocation; //file location for the MIDI file
   // public float noteTime; //how much time the note is going to be on the screen
   // public float noteSpawnZ; //the Z position for the note to be spawned at
   // public float noteTapZ; //the Z position where the player should press the note

   // public float NoteDespawnZ => noteTapZ - (noteSpawnZ - noteTapZ); //De-spawn position for notes

   [Header("Note Prefabs")]
   public GameObject noteNormalPrefab;
   public GameObject noteSliderPrefab;

   // [Header("Note Hit Locations")] 
   // public Vector3 hitPoint1;
   // public Vector3 hitPoint2;
   // public Vector3 hitPoint3;
   // public Vector3 hitPoint4;
   
   [Header("Lane Data")] 
   //public const int LaneCount = 4;
   public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestrictionNormalNote;
   public Melanchall.DryWetMidi.MusicTheory.NoteName noteRestrictionSliderNote;

   public Dictionary<NoteData.LaneOrientation, LaneMidiData> laneMidiData = new ();

  // public ReadOnlyDictionary<NoteData.LaneOrientation, LaneMidiData> LaneMidiData => new (_laneMidiData);
   // [Header("Lane 1")]
   // public KeyCode input1;//key input for that lane
   // //note on midi file to spawn and note object prefab to spawn
   // [Range(0, 8)] public int laneOctave1;
   // [ReadOnly] public List<BaseNoteType> AllNoteOnLaneList1 = new List<BaseNoteType>();//list of timestamps of all the notetypes 
   //
   // [Header("Lane 2")]
   // public KeyCode input2;
   // [Range(0, 8)] public int laneOctave2;
   // [ReadOnly] public List<BaseNoteType> AllNoteOnLaneList2 = new List<BaseNoteType>();
   //
   // [Header("Lane 3")]
   // public KeyCode input3;
   // [Range(0, 8)] public int laneOctave3;
   // [ReadOnly] public List<BaseNoteType> AllNoteOnLaneList3 = new List<BaseNoteType>();
   //
   // [Header("Lane 4")]
   // public KeyCode input4;
   // [Range(0, 8)] public int laneOctave4;
   // [ReadOnly] public List<BaseNoteType> AllNoteOnLaneList4 = new List<BaseNoteType>();
   //
   // public Dictionary<KeyCode, Data_Classes.NoteData.LaneOrientation> InputDict;
   // public Dictionary<Data_Classes.NoteData.LaneOrientation, Vector3> HitPointDict;

   // [Space]
   // [Header("Visuals")]
   // public GameObject hitTextPrefab;
   // public GameObject missTextPrefab;
   
   private void OnEnable()
   {
       // InputDict = new Dictionary<KeyCode, Data_Classes.NoteData.LaneOrientation>()
       // {
       //     { input1, Data_Classes.NoteData.LaneOrientation.One },
       //     { input2, Data_Classes.NoteData.LaneOrientation.Two },
       //     { input3, Data_Classes.NoteData.LaneOrientation.Three },
       //     { input4, Data_Classes.NoteData.LaneOrientation.Four }
       // };
       //
       // HitPointDict = new Dictionary<Data_Classes.NoteData.LaneOrientation, Vector3>()
       // {
       //     { Data_Classes.NoteData.LaneOrientation.One, hitPoint1},
       //     { Data_Classes.NoteData.LaneOrientation.Two, hitPoint2},
       //     { Data_Classes.NoteData.LaneOrientation.Three, hitPoint3},
       //     { Data_Classes.NoteData.LaneOrientation.Four , hitPoint4}
       // };

   }

  
}

