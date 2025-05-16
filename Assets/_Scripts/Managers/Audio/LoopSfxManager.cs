using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Managers.Audio
{
    public class LoopSfxManager : MonoBehaviour
    {
        public static LoopSfxManager Instance { get; private set; }

        [Header("AUDIO SOURCE")]
        [SerializeField]
        private AudioSource loopSource;

        [FormerlySerializedAs("grassSteps")]
        [Header("AUDIO CLIP")]
        [SerializeField]
        private AudioClip steps;
        public AudioClip Steps => steps;

        [SerializeField]
        private AudioClip slideSound;
        public AudioClip SlideSound => slideSound;

        private void Awake()
        {
            if (!Instance)
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

        public void StopLoopSfx() => loopSource.Stop();
    }
}
