using DG.Tweening;
using UnityEngine;

namespace SceneObjectBehavior
{
    public class IconPunchScale : MonoBehaviour
    {
        private void Start()
        {
            transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 10f, 1, 0.1f).SetLoops(-1, LoopType.Yoyo);
    
        }

    }
}
