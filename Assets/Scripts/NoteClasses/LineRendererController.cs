using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


namespace NoteClasses
{
    
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererController : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private float _heightOffset = 0.01f;
        public LineRenderer LineRenderer {
            get {
                if (!_lineRenderer) _lineRenderer = GetComponent<LineRenderer>();
                return _lineRenderer;
            }
        }

        private Transform[] _points;

        public void SetUpLine(Transform[] points)
        {

            LineRenderer.positionCount = points.Length;
            _points = points;
            
        }

        private void Update() {
            for(var i = 0; i < _points.Length; i++) {
                var oldPos = _points[i].position;
                LineRenderer.SetPosition(i, new Vector3(oldPos.x, oldPos.y + .01f, oldPos.z));
            }
        }
    }
}