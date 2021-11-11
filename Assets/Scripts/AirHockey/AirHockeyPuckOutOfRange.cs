using DG.Tweening;
using UnityEngine;

namespace AirHockey
{
    public class AirHockeyPuckOutOfRange : MonoBehaviour
    {
        [SerializeField] private GameObject _puckPrefab;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClipInstantiate;
        [SerializeField] private Transform _spawnPoint1;
        private Transform _baseNode;
        private bool _isPuckDetected = false;
        private void Start()
        {
            _baseNode = GetComponentInParent<AirHockeyGameManager>().transform;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Puck")) return;
            if (_isPuckDetected) return;
            _isPuckDetected = true;
            DOVirtual.DelayedCall(1.5f, () =>Destroy(other.transform.parent.gameObject));
            DOVirtual.DelayedCall(3f, () =>
            {
                Instantiate(_puckPrefab, _spawnPoint1.position, _spawnPoint1.rotation, _baseNode);
                _audioSource.clip = _audioClipInstantiate;
                _audioSource.Play();
            }).OnComplete(() =>
            {
                _isPuckDetected = false;
            });
        }
    }
}
