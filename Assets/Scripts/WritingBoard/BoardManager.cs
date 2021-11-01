using UnityEngine;

namespace WritingBoard
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private AudioClip _audioClip;
        private AudioSource _audioSource;
        private bool _audioExist;
    
        private void Start()
        {
            _audioExist =  gameObject.TryGetComponent<AudioSource>(out _audioSource);
            if (!_audioExist) return;
            _audioSource.clip = _audioClip;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Chalk")) return;
            if (!_audioExist) return;
            _audioSource.Play();
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.gameObject.CompareTag("Chalk")) return;
            if (!_audioExist) return;
            _audioSource.Stop();
        }
    }
}
