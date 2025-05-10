using UnityEngine;

namespace _Scripts.Managers.Audio
{
    public class FootstepSfxManager : MonoBehaviour
    {
        public static FootstepSfxManager Instance { get; private set; }

        [Header("AUDIO SOURCE")]
        [SerializeField]
        private AudioSource footstepSource;

        [Header("AUDIO CLIP")]
        [SerializeField]
        private AudioClip grassSteps;
        public AudioClip GrassSteps => grassSteps;

        [SerializeField]
        private AudioClip stoneSteps;
        public AudioClip StoneSteps => stoneSteps;

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

        public void PlayFootstepSfx(AudioClip clip)
        {
            if (footstepSource.isPlaying && footstepSource.clip == clip)
                return;

            footstepSource.Stop();
            footstepSource.clip = clip;
            footstepSource.Play();
        }

        public void StopFootstepSfx()
        {
            footstepSource.Stop();
        }
    }
}
