using System;
using Fusion;
using Unity.Mathematics;
using UnityEngine;

namespace WritingBoard
{
    public class DrawLineManager : MonoBehaviour
    {
        [SerializeField] private GameObject _chalkLineRendererPrefab;
        
        private BoardVisualComponent _boardVisualComponent;
        private LineRenderer _currLine;
        private int _numTouch = 0;

        private Vector3 _previousPoint = Vector3.zero;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            var boardParent = board.GetComponentInParent<Rigidbody>();
            _boardVisualComponent = boardParent.gameObject.GetComponentInChildren<BoardVisualComponent>();
            InitiateLineDrawObject(other);
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            DrawLine(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            _numTouch = 0;
        }

        private void InitiateLineDrawObject(Collider other)
        {
            
            var go = Instantiate(_chalkLineRendererPrefab,_boardVisualComponent.transform);
            go.transform.position = transform.position;
            go.transform.localRotation = Quaternion.identity;
            _currLine = go.GetComponent<LineRenderer>();
            _currLine.startWidth = 0.005f;
            _currLine.endWidth = 0.005f;
            _currLine.useWorldSpace = false;
            _numTouch = 0;

        }

        private void DrawLine(Collider other)
        {
            if (_currLine == null) return;
            _currLine.positionCount = _numTouch + 1;
            var writePoint = _currLine.transform.InverseTransformPoint(transform.position);
            var writePointProjected= new Vector3(writePoint.x, writePoint.y, 0); // Projecting to the board surface
            //if (( Vector3.Distance(_previousPoint ,transform.position) < 0.001f)) return;
            _currLine.SetPosition(_numTouch, writePointProjected);
            _numTouch++;
            //_previousPoint = transform.position;
        }

    }
}
//TODO: Check that the point is not the previous point
// The current commented code does the job but added origin to the points when it misses
// TO be explored