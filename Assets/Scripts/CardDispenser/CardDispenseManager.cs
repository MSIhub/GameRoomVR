using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardDispenser
{
    public class CardDispenseManager : SimulationBehaviour, ISpawned
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _distMovedForNextSpawn = 0.1f;
        [SerializeField] private float _buttonThrowForce = 1f;
        private List<GameObject> _cardsList;
        private bool _isCardSlotFree;
        private GameObject _currentCardSpawned;
        private ButtonController _button;
        
        public void Spawned()
        {
            var networkCardParent = GetComponentInChildren<NetworkCardParent>();
            var cardSet = networkCardParent.GetComponentsInChildren<Rigidbody>();
            _cardsList = new List<GameObject>();
            foreach (var rb in cardSet)
            {
                _cardsList.Add(rb.gameObject);
                rb.gameObject.SetActive(false);
            }
            _isCardSlotFree = true;

            _button = GetComponentInChildren<ButtonController>();
            Debug.Log(_button.gameObject.name);
        }

        public override void FixedUpdateNetwork()
        {
            if (_cardsList.Count <= 0) return;
            if (_isCardSlotFree)
            {
                _currentCardSpawned = SpawnRandomCard();
            }
            CheckSlotAvailability();
            PushCardOnButtonPress();
            Debug.Log(_button.IsButtonPressed);
        }

        private void PushCardOnButtonPress()
        {
            //Add force if button pressed
            if (_button.IsButtonPressed)
            {
                _currentCardSpawned.GetComponentInChildren<Rigidbody>().AddForce(Vector3.right * _buttonThrowForce, ForceMode.Impulse);
            }
        }

        private void CheckSlotAvailability()
        {
            var dist = Vector3.Distance(_currentCardSpawned.transform.position, _spawnPoint.position);
            _isCardSlotFree = Math.Round(dist, 2) > _distMovedForNextSpawn;
        }

        private GameObject SpawnRandomCard()
        {
            var cardIndex = Random.Range(0, _cardsList.Count);
            _cardsList[cardIndex].SetActive(true);
            _cardsList[cardIndex].transform.position = _spawnPoint.position;
            _cardsList[cardIndex].transform.rotation = _spawnPoint.rotation;
            var currentCard = _cardsList[cardIndex];
            _cardsList.RemoveAt(cardIndex);
            _isCardSlotFree = false;
            return currentCard;
        }
    }
}
