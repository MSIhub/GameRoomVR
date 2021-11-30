using DG.Tweening;
using UnityEngine;

namespace SceneObjectBehavior
{
    public class ObjectRotate : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            var mySequence = DOTween.Sequence();
            var up = transform.up;
            mySequence.Append(transform.DORotate(up*360f, 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
            mySequence.Append(transform.GetChild(0).transform.DORotate(up*360f, 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear));
            mySequence.SetLoops(-1, LoopType.Incremental);
        }
        
    }
}
