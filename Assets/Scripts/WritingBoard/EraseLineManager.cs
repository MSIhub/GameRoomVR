using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace WritingBoard
{
    public class EraseLineManager : MonoBehaviour
    {
        private LineRenderer[] _lineRenderers;
        private Vector3[] _positions;
        private Vector3[] _modifiedPositions;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            var parent =  other.gameObject.transform.parent;
            Debug.Log(parent.name);
            _lineRenderers = parent.gameObject.GetComponentsInChildren<LineRenderer>();
            if (_lineRenderers.Length < 1) return;
            foreach (var lineRenderer in _lineRenderers)
            {
                lineRenderer.GetPositions(_positions);
                lineRenderer.GetPositions(_modifiedPositions);
            }

        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            if (_positions.Length < 1) return;
            foreach (var position in _positions)
            {
                    var dist = Vector3.Distance(transform.position, position);
                    Debug.Log(dist);

            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            
        }

        
        
    }
}
