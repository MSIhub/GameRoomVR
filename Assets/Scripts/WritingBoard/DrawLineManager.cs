using System;
using UnityEngine;

namespace WritingBoard
{
    public class DrawLineManager : MonoBehaviour
    {
        [SerializeField] private Material _chalkTipMaterial;
        [SerializeField] private Transform _tipTransform;
        private LineRenderer _currLine;
        private int _numTouch = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            InitiateLineDrawObject(other);
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            DrawLine();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            _numTouch = 0;
        }

        private void InitiateLineDrawObject(Collider other)
        {
            var go = new GameObject
            {
                transform =
                {
                    parent = other.transform.parent
                }
            };
            _currLine = go.AddComponent<LineRenderer>();
            _currLine.startWidth = 0.005f;
            _currLine.endWidth = 0.005f;
            _currLine.material = _chalkTipMaterial;
            _currLine.useWorldSpace = false;
            _numTouch = 0;
        }

        private void DrawLine()
        {
            _currLine.positionCount = _numTouch + 1;
            _currLine.SetPosition(_numTouch, _tipTransform.position);
            _numTouch++;
        }
    }
}
