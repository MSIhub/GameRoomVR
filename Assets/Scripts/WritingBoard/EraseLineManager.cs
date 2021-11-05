using UnityEngine;
using Math = System.Math;

namespace WritingBoard
{
    public class EraseLineManager : MonoBehaviour
    {
        private LineRenderer[] _lineRenderers;
        private float _radiusDuster;

        // Start is called before the first frame update
        private void Start()
        {
            var trig = GetComponentsInChildren<SphereCollider>();
            foreach (var col in trig)
            {
                if (!col.isTrigger) return;
                _radiusDuster = col.radius;
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            var parent =  other.gameObject.transform.parent;
            _lineRenderers = parent.gameObject.GetComponentsInChildren<LineRenderer>();
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.TryGetComponent<BoardManager>(out var board)) return;
            if (_lineRenderers.Length < 1) return;
            foreach (var lineRenderer in _lineRenderers)
            {
                if (lineRenderer==null) return;
                var pos = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(pos);
                if (pos.Length <1) return;
                foreach (var position in pos)
                {
                    var dist = Vector3.Distance(transform.position, lineRenderer.transform.TransformPoint(position));
                    if (Math.Round(dist,2) <= _radiusDuster*1.2f)
                    {
                        Destroy(lineRenderer.gameObject);
                    }
                }
                
            }
        }
    }
}
