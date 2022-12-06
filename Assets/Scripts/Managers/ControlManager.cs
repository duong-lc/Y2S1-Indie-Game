using System;
using System.Collections.Generic;
using System.Linq;
using Core.Logging;
using NoteClasses;
using UnityEngine;
using SO_Scripts;
using NoteData = Data_Classes.NoteData;

namespace Managers
{
    public class ControlManager : MonoBehaviour
    {
        [SerializeField] private MidiData midiData;
        [Space]
        [SerializeField] private List<Transform> anchorPoints = new List<Transform>();
        [Space]
        [SerializeField] private float castDist;
        [SerializeField] private Vector3 boxSize;
        [SerializeField] private LayerMask noteLayer;
        
        private Dictionary<NoteData.LaneOrientation, Vector3> _castOriginDict = new Dictionary<NoteData.LaneOrientation, Vector3>();
        private List<NoteSlider> _currentHoldSliders = new List<NoteSlider>();
        private void Start()
        {
            _castOriginDict = new Dictionary<NoteData.LaneOrientation, Vector3>()
            {
                { NoteData.LaneOrientation.One, GetAnchorPoint("Lane1").position},
                { NoteData.LaneOrientation.Two, GetAnchorPoint("Lane2").position},
                { NoteData.LaneOrientation.Three, GetAnchorPoint("Lane3").position},
                { NoteData.LaneOrientation.Four, GetAnchorPoint("Lane4").position}
            };
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (var entry in midiData.InputDict)
            {
                if (Input.GetKeyDown(entry.Key))
                {
                    NCLogger.Log($"{midiData.NoteDespawnZ}");
                    _castOriginDict.TryGetValue(entry.Value, out var castOrigin);
                    if (!Physics.BoxCast(castOrigin,
                            boxSize,
                            Vector3.forward,
                            out var hit,
                            Quaternion.identity,
                            castDist,
                            noteLayer)) continue;
                    
                    //TODO: Instead of getting component, get the lane based on the keyEntry.
                    //In said lane, there will be a List that store all Active Notes in scene  -List<NoteBase>
                    //use linq to compare hit.object with notebase.object in list. If matches, return notebase with
                    //game object tag, then use switch to cast to normal or slider. 
                    //the purpose is to have less performance intensive code but haven't really measured - just a theory.
                    var note = hit.collider.gameObject.GetComponent<NoteBase>();
                    if (note.CompareTag("NoteNormal"))
                    {
                        var filteredNote = (NoteNormal)note;
                        filteredNote.OnNoteHitNormalNote();
                    }
                    else if (note.CompareTag("NoteSlider"))
                    {
                        var filteredNote = (NoteSlider)note;
                        filteredNote.OnNoteHitStartNote();
                        _currentHoldSliders.Add(filteredNote);
                    }
                }

                if (Input.GetKeyUp(entry.Key))
                {
                    var slider = _currentHoldSliders.Find(x => x.noteOrientation == entry.Value);
                    if(!slider) continue;
                    
                    var status = slider.OnNoteHitEndNote();

                    _currentHoldSliders.Remove(slider);
                    if(status) Destroy(slider.gameObject);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (var anchor in anchorPoints) {
                Gizmos.DrawWireCube(anchor.position, boxSize*2);
                Gizmos.DrawWireCube(anchor.position + Vector3.forward * castDist, boxSize*2);
            }
        }

        private Transform GetAnchorPoint(string tag) => anchorPoints.Find(t => t.CompareTag(tag));
    }
}
