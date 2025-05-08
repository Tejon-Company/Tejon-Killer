using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [SerializeField]
    private AudioSource soundEffectPrefab;

    [SerializeField]
    private AudioClip _defaultShootSound;
    public AudioClip defaultShootSound
    {
        get => _defaultShootSound;
    }

    [SerializeField]
    private AudioClip _defaultReloadSound;
    public AudioClip defaultReloadSound
    {
        get => _defaultReloadSound;
    }

    [SerializeField]
    private AudioClip menuMusic;

    private AudioSource currentWalkingSound;
    private AudioSource currentMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReproduceWalkingSound(bool isWalking, AudioClip walkingSound)
    {
        if (!isWalking)
        {
            StopSound(currentWalkingSound);
            return;
        }

        if (currentWalkingSound == null)
        {
            currentWalkingSound = ReproduceLoopSound(walkingSound, transform);
        }
    }

    public void ReproduceMenuMusic()
    {
        ReproduceMusic(menuMusic);
    }

    public void ReproduceMusic(AudioClip music)
    {
        if (currentMusic != null)
        {
            if (currentMusic.clip == music && currentMusic.isPlaying)
            {
                return;
            }
            StopMusic();
        }

        currentMusic = gameObject.AddComponent<AudioSource>();
        currentMusic.clip = music;
        currentMusic.loop = true;
        currentMusic.playOnAwake = false;
        currentMusic.volume = 1f;
        currentMusic.Play();
    }

    public void StopMusic()
    {
        StopSound(currentMusic);
    }

    public void ReproduceSoundEffect(AudioClip audio, Transform spawnTransform)
    {
        AudioSource source = Instantiate(
            soundEffectPrefab,
            spawnTransform.position,
            Quaternion.identity
        );
        source.clip = audio;
        source.Play();
        Destroy(source.gameObject, defaultShootSound.length);
    }

    private AudioSource ReproduceLoopSound(AudioClip sound, Transform spawnTransform)
    {
        AudioSource source = Instantiate(
            soundEffectPrefab,
            spawnTransform.position,
            Quaternion.identity
        );
        source.clip = sound;
        source.loop = true;
        source.Play();
        return source;
    }

    private void StopSound(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            Destroy(source.gameObject);
        }
    }
}
