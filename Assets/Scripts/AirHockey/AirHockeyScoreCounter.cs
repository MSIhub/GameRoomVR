using System;
using DG.Tweening;
using UnityEngine;

namespace AirHockey
{
    public class AirHockeyScoreCounter : MonoBehaviour
    {
        public bool isScoreTrigger1 = false;
        public bool isScoreTrigger2 = false;

        [SerializeField] private GameObject _airHockeyPuckPrefab;

        private Transform _baseNode;
        private Vector3 _puckSpawnPosition;
        private Quaternion _puckSpawnRotation = Quaternion.Euler(new Vector3(-90f, 0f,0f));
        private GameObject _puckSpawned;
        private void Awake()
        {
            isScoreTrigger1 = gameObject.name.Contains("1");
            isScoreTrigger2 = gameObject.name.Contains("2");
        }

        private void Start()
        {
            _baseNode = GetComponentInParent<AirHockeyGameManager>().transform;
            Debug.Log(_baseNode.name);
        }

       
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Puck")) return;
            Debug.Log(other.name + "   -> " + gameObject.name);
            if (isScoreTrigger1)
            {
//                Debug.Log("Player2 won");
                _puckSpawnPosition = _baseNode.position + new Vector3(-0.7f, 0.664f, 0f); // Value based on measurement
                
            }

            if (isScoreTrigger2)
            {
             //   Debug.Log("Player1 won");
                _puckSpawnPosition = _baseNode.position + new Vector3(0.7f, 0.664f, 0f); // Value based on measurement
            }
            
            //Using dotween instead of invoke
            DOVirtual.DelayedCall(0.5f, () =>
            {
                Destroy(other.transform.parent.gameObject);
            }).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1f, ()=> _puckSpawned = Instantiate(_airHockeyPuckPrefab, _puckSpawnPosition,_puckSpawnRotation, _baseNode));
            });

        }

        
    }
}
