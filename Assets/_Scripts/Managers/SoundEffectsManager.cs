using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance { get; private set; }

    [SerializeField]
    private AudioSource soundEffectPrefab;

    [SerializeField]
    private AudioClip _defaultShootSound;
    public AudioClip defaultShootSound
    {
        get => _defaultShootSound;
    }

    private AudioSource currentWalkingSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ReproduceWalkingSound(bool isWalking, AudioClip walkingSound)
    {
        if (isWalking)
        {
            if (currentWalkingSound == null)
            {
                currentWalkingSound = ReproduceLoopSound(walkingSound, transform);
            }
        }
        else
        {
            StopSound(currentWalkingSound);
        }
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
