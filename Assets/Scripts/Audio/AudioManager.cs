using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _audioClipList;
        [Range(0,0.1f)][SerializeField] private float _bgmVolume = 0.005f;//as it background volume is tuned down to 0.1

        [Header("Input Action Reference:")] 
        [SerializeField] public InputActionReference VolumeUp;
        [SerializeField] public InputActionReference VolumeDown;
    
        private AudioSource _audioSource;
        private int _index = 0;


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);//to pass the audio continuously across scene
        }

        private void Start()
        {
        
            _index = 0;
            if (TryGetComponent<AudioSource>(out _audioSource))
            {
                if (_audioClipList.Count >0)
                {
                    _audioSource.clip = _audioClipList[_index];
                    _audioSource.volume = _bgmVolume;
                    _audioSource.Play();
                }
            }
        }
    

        private void Update()
        {
            if (_audioSource.time >= _audioClipList[_index].length)
            {
                _index++;
                _index  %= (_audioClipList.Count);
                _audioSource.clip = _audioClipList[_index];
                _audioSource.volume = _bgmVolume;
                _audioSource.Play();
            }
        
            if( VolumeDown.action.triggered)
            {
                _bgmVolume-=0.005f;
                _audioSource.volume = _bgmVolume;
            
            }
            if( VolumeUp.action.triggered)
            {
                _bgmVolume+=0.005f;
                _audioSource.volume = _bgmVolume;
            }
        
        }
    }
}
