using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SO_Scripts;
using UnityEditor.PackageManager;
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
        
        private Dictionary<NoteData.LaneOrientation, Vector3> _castOriginDict;

        private void Start()
        {
            _castOriginDict = new Dictionary<NoteData.LaneOrientation, Vector3>()
            {
                { NoteData.LaneOrientation.One, GetAnchorPoint("Lane1").position },
                { NoteData.LaneOrientation.Two, GetAnchorPoint("Lane2").position },
                { NoteData.LaneOrientation.Three, GetAnchorPoint("Lane3").position },
                { NoteData.LaneOrientation.Four, GetAnchorPoint("Lane4").position }
            };
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (var entry in midiData.InputDict)
            {
                if (Input.GetKeyDown(entry.Key))
                {
                    _castOriginDict.TryGetValue(entry.Value, out var castOrigin);
                    bool cast = Physics.BoxCast(castOrigin, 
                        boxSize, 
                        Vector3.forward, 
                        out var hit, 
                        Quaternion.identity, 
                        castDist, 
                        noteLayer);

                    if (!cast) continue;

                    var note = hit.collider.gameObject;
                    //if(note)

                }
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var anchor in anchorPoints) {
                Gizmos.DrawSphere(anchor.position, 1);
            }
        }

        private Transform GetAnchorPoint(string tag) => anchorPoints.Where(t => t.CompareTag(tag)) as Transform;
    }
}
