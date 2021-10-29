using System;
using DG.Tweening;
using UnityEngine;

namespace AirHockey
{
    public class AirHockeyScoreHandler : MonoBehaviour
    {
        public bool isScoreTrigger1 = false;
        public bool isScoreTrigger2 = false;

        [SerializeField] private GameObject _airHockeyPuckPrefab;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _audioClipInstantiate;
        [SerializeField] private AudioClip _audioClipGoal;

        private Transform _baseNode;
        private Vector3 _puckSpawnPosition;
        private Quaternion _puckSpawnRotation = Quaternion.Euler(new Vector3(-90f, 0f,0f));
        //private GameObject _puckSpawned;

        private bool _isPuckDetected = false;
        private void Awake()
        {
            isScoreTrigger1 = gameObject.name.Contains("1");
            isScoreTrigger2 = gameObject.name.Contains("2");
        }

        private void Start()
        {
            _baseNode = GetComponentInParent<AirHockeyGameManager>().transform;
        }

       
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Puck")) return;
            if (_isPuckDetected) return;
            _isPuckDetected = true;
            _audioSource.clip = _audioClipGoal;
            _audioSource.Play();
            if (isScoreTrigger1)
            {
                _puckSpawnPosition = _baseNode.position + new Vector3(-0.7f, 0.7f, 0f); // Value based on measurement
                
            }

            if (isScoreTrigger2)
            {
                _puckSpawnPosition = _baseNode.position + new Vector3(0.7f, 0.7f, 0f); // Value based on measurement
            }
            
            //Using dotween instead of invoke
            DOVirtual.DelayedCall(1f, () =>
            {
                Destroy(other.transform.parent.gameObject);
            });
            DOVirtual.DelayedCall(3f, () =>
            {
                Instantiate(_airHockeyPuckPrefab, _puckSpawnPosition, _puckSpawnRotation, _baseNode);
                _audioSource.clip = _audioClipInstantiate;
                _audioSource.Play();
            });
            
        }

        private void OnTriggerExit(Collider other)
        {
            _isPuckDetected = false;
        }
    }
}


/*DOVirtual.DelayedCall(1f, () =>
{
    Destroy(other.transform.parent.gameObject);
}).OnComplete(() =>
{
    DOVirtual.DelayedCall(3f, ()=>
    {
        Instantiate(_airHockeyPuckPrefab, _puckSpawnPosition, _puckSpawnRotation, _baseNode);
    });
});*/