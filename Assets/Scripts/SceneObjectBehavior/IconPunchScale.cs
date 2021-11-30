using DG.Tweening;
using UnityEngine;

namespace SceneObjectBehavior
{
    public class IconPunchScale : MonoBehaviour
    {
        private void Start()
        {
            transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 2f, 2, 0.2f).SetLoops(-1, LoopType.Yoyo);
    
        }

    }
}
