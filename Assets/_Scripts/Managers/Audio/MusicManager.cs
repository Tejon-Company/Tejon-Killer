using UnityEngine;

namespace _Scripts.Managers.Audio
{
    public class MusicManager : MonoBehaviour
    {
        [Header("AUDIO SOURCE")] [SerializeField]
        private AudioSource musicSource;

        [Header("AUDIO CLIP")] [SerializeField]
        private AudioClip menuBackgroundMusic;

        public static MusicManager Instance { get; private set; }
        public AudioClip MenuBackgroundMusic => menuBackgroundMusic;

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

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource.isPlaying && musicSource.clip == clip)
                return;

            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
}