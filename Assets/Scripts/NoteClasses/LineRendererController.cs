using System;
using UnityEngine;



namespace NoteClasses
{
    
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererController : MonoBehaviour
    {
        private LineRenderer _lineRenderer;

        public LineRenderer LineRenderer
        {
            get
            {
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

        private void LateUpdate()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                LineRenderer.SetPosition(i, _points[i].position);
            }
        }
    }
}