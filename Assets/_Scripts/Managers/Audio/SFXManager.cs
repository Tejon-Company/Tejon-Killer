using UnityEngine;

namespace _Scripts.Managers.Audio
{
    public class SfxManager : MonoBehaviour
    {
        public static SfxManager Instance { get; private set; }
        
        [Header("AUDIO SOURCE")]
        [SerializeField]
        private AudioSource sfxSource;
        
        [Header("AUDIO CLIP")]
        [SerializeField]
        private AudioClip shoot;
        public AudioClip Shoot => shoot;

        [SerializeField]
        private AudioClip reload;
        public AudioClip Reload => reload;

        [SerializeField]
        private AudioClip jump;
        public AudioClip Jump => jump;

        [SerializeField]
        private AudioClip doubleJump;
        public AudioClip DoubleJump => doubleJump;

        [SerializeField]
        private AudioClip slide;
        public AudioClip Slide => slide;

        [SerializeField]
        private AudioClip stomp;
        public AudioClip Stomp => stomp;

        [SerializeField]
        private AudioClip dash;
        public AudioClip Dash => dash;
        
        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void PlaySfx(AudioClip clip)
        {
            if (sfxSource.isPlaying && sfxSource.clip == clip)
                return;
            
            sfxSource.PlayOneShot(clip);
        }
    }
}