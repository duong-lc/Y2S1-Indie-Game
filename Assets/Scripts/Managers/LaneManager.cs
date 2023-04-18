using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using Core.Patterns;
using UnityEngine;
using SO_Scripts;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Sirenix.Utilities;
using DataClass = Data_Classes;
using EventType = Core.Events.EventType;


namespace Managers
{
    public class LaneManager : MonoBehaviour
    {
        private MidiData _midiData;
        private Note[] _rawNoteArray;
        private List<int> _ignoreIndexList = new List<int>();

        private void Awake()
        {
            Core.Events.EventDispatcher.Instance.AddListener(EventType.CompileDataFromMidiEvent,
                param => CompileDataFromMidi((MidiFile) param));

            _midiData = GameModeManager.Instance.CurrentMidiData;
        }

        private void Start()
        {
            //_ignoreIndexList.Clear();
        }

        private void CompileDataFromMidi(MidiFile midiFile)
        {
            ICollection<Note> notes = midiFile.GetNotes();
            _rawNoteArray = new Note[notes.Count];
            notes.CopyTo(_rawNoteArray, 0);

            SetTimeStampsAllLanes();

            DistributeNoteToLanes();
        }

        private void SetTimeStampsAllLanes()
        {
            if (_midiData.LaneMidiData.Values.Any(midiData => !midiData.allNoteOnLaneList.IsNullOrEmpty())) {
                NCLogger.Log($"something is NOT empty");
                return;
            }


            for (int index = 0; index < _rawNoteArray.Length; index++) //for every note in the note array
            {
                if (_ignoreIndexList.Contains(index)) continue;

                foreach (var kvp in _midiData.LaneMidiData) {
                    if (_rawNoteArray[index].Octave == kvp.Value.LaneOctave) {
                        AddNoteToLane(ref index, kvp.Value.allNoteOnLaneList, kvp.Key, kvp.Value.LaneOctave);
                    }
                }
                // if (_rawNoteArray[index].Octave == midiData.laneOctave1)
                // {
                //     AddNoteToLane(ref index, midiData.AllNoteOnLaneList1, DataClass.NoteData.LaneOrientation.One,
                //         midiData.laneOctave1);
                // }
                // else if (_rawNoteArray[index].Octave == midiData.laneOctave2)
                // {
                //     AddNoteToLane(ref index, midiData.AllNoteOnLaneList2, DataClass.NoteData.LaneOrientation.Two,
                //         midiData.laneOctave2);
                // }
                // else if (_rawNoteArray[index].Octave == midiData.laneOctave3)
                // {
                //     AddNoteToLane(ref index, midiData.AllNoteOnLaneList3, DataClass.NoteData.LaneOrientation.Three,
                //         midiData.laneOctave3);
                // }
                // else if (_rawNoteArray[index].Octave == midiData.laneOctave4)
                // {
                //     AddNoteToLane(ref index, midiData.AllNoteOnLaneList4, DataClass.NoteData.LaneOrientation.Four,
                //         midiData.laneOctave4);
                // }
            }
        }

        private void AddNoteToLane(ref int index, List<DataClass.BaseNoteType> laneToAdd,
            DataClass.NoteData.LaneOrientation orientation, int octaveIndex)
        {
            if (_rawNoteArray[index].NoteName == _midiData.noteRestrictionNormalNote)
            {
                AddNormalNoteToList(ref index, laneToAdd, orientation, octaveIndex);
            }
            else if (_rawNoteArray[index].NoteName == _midiData.noteRestrictionSliderNote)
            {
                AddSliderNoteToList(ref index, laneToAdd, orientation, octaveIndex);
            }
        }

        private void AddNormalNoteToList(ref int index, List<DataClass.BaseNoteType> laneToAdd,
            DataClass.NoteData.LaneOrientation orientation, int octaveIndex)
        {
            //get the time stamp for that note. (note.Time does not return the format we want as it uses time stamp temp map in midi, so conversion is needed)
            var metricTimeSpan =
                TimeConverter.ConvertTo<MetricTimeSpan>(_rawNoteArray[index].Time, SongManager.MidiFile.GetTempoMap());
            DataClass.NoteNormalType noteNormalLocal = new DataClass.NoteNormalType
            {
                octaveNum = octaveIndex,
                noteID = DataClass.NoteData.NoteID.NormalNote,
                timeStamp = (double) metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds +
                            (double) metricTimeSpan.Milliseconds / 1000f,
                laneOrientation = orientation
            };
            //adding the time stamp (in seconds) to the array of time stamp
            laneToAdd.Add(noteNormalLocal);
        }

        private void AddSliderNoteToList(ref int index, List<DataClass.BaseNoteType> laneToAdd,
            DataClass.NoteData.LaneOrientation orientation, int octaveIndex)
        {
            //get the time stamp for that note. (note.Time does not return the format we want as it uses time stamp temp map in midi, so conversion is needed)
            var metricTimeSpan =
                TimeConverter.ConvertTo<MetricTimeSpan>(_rawNoteArray[index].Time, SongManager.MidiFile.GetTempoMap());
            /*Instead of relying on local val instantiated outside foreach loop
            create another loop inside when see current is slider note. Keep looping in the 
            note stream until you hit a note with the same octave and note restriction. That will
            be the end note
            
            Create a ignore note list so if the outer loop hit the same end note from the slider note
            it will ignore and continue
            */
            //For each 2 notes which is a slider, reset data for new slider note
            DataClass.NoteData.SliderData sliderNoteData = new DataClass.NoteData.SliderData
            {
                timeStampKeyDown = (double) metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds +
                                   (double) metricTimeSpan.Milliseconds / 1000f
            };
            for (int j = index + 1; j < _rawNoteArray.Length; j++)
            {
                //Check for next note on the same octave and on same line
                if (_rawNoteArray[j].Octave != octaveIndex ||
                    _rawNoteArray[j].NoteName != _midiData.noteRestrictionSliderNote) continue;
                var metricTimeSpan2 =
                    TimeConverter.ConvertTo<MetricTimeSpan>(_rawNoteArray[j].Time, SongManager.MidiFile.GetTempoMap());
                sliderNoteData.timeStampKeyUp = (double) metricTimeSpan2.Minutes * 60f + metricTimeSpan2.Seconds +
                                                (double) metricTimeSpan2.Milliseconds / 1000f;
                DataClass.NoteSliderType noteSliderLocal = new DataClass.NoteSliderType
                {
                    octaveNum = octaveIndex,
                    noteID = DataClass.NoteData.NoteID.SliderNote,
                    sliderData = sliderNoteData,
                    //Assigning note's lane orientation
                    laneOrientation = orientation
                };
                laneToAdd.Add(noteSliderLocal);
                _ignoreIndexList.Add(j);
                break;
            }
        }

        public void DistributeNoteToLanes()
        {
            var laneArray = GetComponentsInChildren<Lane>();
            foreach (Lane lane in laneArray)
            {
                lane.SetLocalListOnLane(_midiData.LaneMidiData[lane.LaneOrientation].allNoteOnLaneList);
                
                // if (lane.LaneOrientation == DataClass.NoteData.LaneOrientation.One)
                // {
                //     lane.SetLocalListOnLane(midiData.AllNoteOnLaneList1);
                // }
                // else if (lane.CompareTag("Lane2"))
                // {
                //     lane.SetLocalListOnLane(midiData.AllNoteOnLaneList2);
                // }
                // else if (lane.CompareTag("Lane3"))
                // {
                //     lane.SetLocalListOnLane(midiData.AllNoteOnLaneList3);
                // }
                // else if (lane.CompareTag("Lane4"))
                // {
                //     lane.SetLocalListOnLane(midiData.AllNoteOnLaneList4);
                // }
                //
            }
        }
    }
}
