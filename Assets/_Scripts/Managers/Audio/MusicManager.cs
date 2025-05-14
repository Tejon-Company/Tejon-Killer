using UnityEngine;

namespace _Scripts.Managers.Audio
{
    public class MusicManager : MonoBehaviour
    {
        [Header("AUDIO SOURCE")]
        [SerializeField]
        private AudioSource musicSource;

        [Header("AUDIO CLIPS")]
        [SerializeField]
        private AudioClip menuBackgroundMusic;
        [SerializeField]
        private AudioClip level1Music;

        public static MusicManager Instance { get; private set; }
        public AudioClip MenuBackgroundMusic => menuBackgroundMusic;
        public AudioClip Level1Music => level1Music;
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
