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
        [SerializeField] private ButtonController _spawnButtonController;
        [SerializeField] private ButtonController _resetButtonController;
        [SerializeField] private float _distMovedForNextSpawn = 0.1f;
        [SerializeField] private float _buttonThrowForce = 0.5f;
        [SerializeField] private float _resetButtonWaitTime = 4f;
        private List<GameObject> _cardsStack;
        private bool _isCardSlotFree;
        private GameObject _currentCardSpawned;

        private float _resetTime = 0.0f;
        private bool _didReset = false;


        public void Spawned()
        {
           
            AddAllCardsToStack();
            _isCardSlotFree = true;

            
        }
        

        public override void FixedUpdateNetwork()
        {
            
            if (_cardsStack.Count > 0)
            {
                if (_isCardSlotFree)//On grab the object becomes kinematics
                {
                    _currentCardSpawned = SpawnRandomCard();
                    _currentCardSpawned.GetComponentInChildren<Rigidbody>().isKinematic = true; //removing
                }
                CheckSlotAvailability();
                PushCardOnButtonPress();
            }
            //Handling reset button
            if (_resetButtonController.IsButtonPressed)
            {
                _resetTime += Runner.DeltaTime;
                if (_resetTime >= _resetButtonWaitTime)
                {
                    if (!_didReset)
                    {
                        _didReset = ResetCards();    
                    }
                    if (_didReset)
                    {
                        _resetTime = 0.0f;
                    }
                }
            }
            
            if (!_resetButtonController.IsButtonPressed)
            {
                _resetTime = 0.0f;
                _didReset = false;
            }
        }
        
        private void AddAllCardsToStack()
        {
            var networkCardParent = GetComponentInChildren<NetworkCardParent>();
            var cardSet = networkCardParent.GetComponentsInChildren<Rigidbody>();
            _cardsStack = new List<GameObject>();
            foreach (var rb in cardSet)
            {
                _cardsStack.Add(rb.gameObject);
                rb.gameObject.SetActive(false);
            }
        }


        private void PushCardOnButtonPress()
        {
            //Add force if button pressed
            if (_spawnButtonController.IsButtonPressed & _currentCardSpawned!=null)
            {
                _currentCardSpawned.GetComponentInChildren<Rigidbody>().isKinematic = false;
                _currentCardSpawned.GetComponentInChildren<Rigidbody>().AddForce(_currentCardSpawned.transform.forward * _buttonThrowForce, ForceMode.Impulse);
                _currentCardSpawned.transform.parent = null; // parenting to the world frame
            }
        }

        private void CheckSlotAvailability()
        {
            var dist = Vector3.Distance(_currentCardSpawned.transform.position, _spawnPoint.position);
            _isCardSlotFree = Math.Round(dist, 2) > _distMovedForNextSpawn;
        }

        private GameObject SpawnRandomCard()
        {
            var cardIndex = Random.Range(0, _cardsStack.Count);
            _cardsStack[cardIndex].transform.position = _spawnPoint.position;
            _cardsStack[cardIndex].transform.rotation = _spawnPoint.rotation;
            _cardsStack[cardIndex].SetActive(true);
            var currentCard = _cardsStack[cardIndex];
            _cardsStack.RemoveAt(cardIndex);
            _isCardSlotFree = false;
            return currentCard;
        }
        
        private bool ResetCards()
        {
            //TODO: REmove all the cards in the scene spawned. Maintain Another list and delete it
            AddAllCardsToStack();
            return true;
        }

    }
}