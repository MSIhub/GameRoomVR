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
        [SerializeField] private float _buttonThrowForce = 0.5f;
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
                rb.isKinematic = true; //removing
                _cardsList.Add(rb.gameObject);
               rb.gameObject.SetActive(false);
            }
            _isCardSlotFree = true;

            _button = GetComponentInChildren<ButtonController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (_cardsList.Count > 0)
            {
                if (_isCardSlotFree)//On grab the object becomes kinematics
                {
                    _currentCardSpawned = SpawnRandomCard();
                }
                CheckSlotAvailability();
                PushCardOnButtonPress();
            }
        }
        

        private void PushCardOnButtonPress()
        {
            //Add force if button pressed
            if (_button.IsButtonPressed & _currentCardSpawned!=null)
            {
                _currentCardSpawned.GetComponentInChildren<Rigidbody>().isKinematic = false;
                _currentCardSpawned.GetComponentInChildren<Rigidbody>().AddForce(_currentCardSpawned.transform.forward * _buttonThrowForce, ForceMode.Impulse);
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
            _cardsList[cardIndex].transform.position = _spawnPoint.position;
            _cardsList[cardIndex].transform.rotation = _spawnPoint.rotation;
            _cardsList[cardIndex].SetActive(true);
            var currentCard = _cardsList[cardIndex];
            _cardsList.RemoveAt(cardIndex);
            _isCardSlotFree = false;
            return currentCard;
        }
    }
}