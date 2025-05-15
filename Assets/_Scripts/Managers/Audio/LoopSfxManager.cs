using UnityEngine;

namespace _Scripts.Managers.Audio
{
    public class LoopSfxManager : MonoBehaviour
    {
        public static LoopSfxManager Instance { get; private set; }

        [Header("AUDIO SOURCE")]
        [SerializeField]
        private AudioSource loopSource;

        [Header("AUDIO CLIP")]
        [SerializeField]
        private AudioClip grassSteps;
        public AudioClip GrassSteps => grassSteps;

        [SerializeField]
        private AudioClip stoneSteps;
        public AudioClip StoneSteps => stoneSteps;
        
        [SerializeField]
        private AudioClip slideSound;
        public AudioClip SlideSound => slideSound;

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
        
        public void PlayLoopSfx(AudioClip clip)
        {
            if (loopSource.isPlaying && loopSource.clip == clip)
                return;

            loopSource.Stop();
            loopSource.clip = clip;
            loopSource.Play();
        }
        
        public void StopLoopSfx()
        {
            loopSource.Stop();
        }
    }
}
