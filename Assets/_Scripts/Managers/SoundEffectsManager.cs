using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance { get; private set; }

    [SerializeField] private AudioSource soundEffectPrefab;


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
                currentWalkingSound = ReproduceSound(walkingSound, transform);
            }
        }
        else
        {
            StopSound(currentWalkingSound);
        }
    }


    private AudioSource ReproduceSound(AudioClip sound, Transform spawnTransform)
    {
        AudioSource source = Instantiate(soundEffectPrefab, spawnTransform.position, Quaternion.identity);
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
            source = null;
        }
    }
}

