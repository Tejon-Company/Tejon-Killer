using UnityEngine;

namespace _Scripts.Audio.Managers
{
    
    /// <summary>
    /// Gestor singleton para reproducir música de fondo.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource musicSource;

        public static MusicManager Instance { get; private set; }
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
