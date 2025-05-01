using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance { get; private set; }

    [SerializeField] private AudioSource soundEffectPrefab;

    private AudioSource currentWalkingSound;

    private AudioSource shootSound;


    [Header("Sound Effects")]
    [SerializeField] private AudioClip walkingOnGrassSound;

    [SerializeField] private AudioClip walkingOnStoneSound;

    [SerializeField] private AudioClip shootEffectSound;


    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        ReproduceWalkingSound();
    }

    private void ReproduceWalkingSound()
    {
        if (playerController.isWalking)
        {
            if (currentWalkingSound == null)
            {
                ReproduceSound(walkingOnGrassSound, transform);
            }
        }
        else
        {
            StopSound(currentWalkingSound);
        }
    }

    private void ReproduceSound(AudioClip sound, Transform spawnTransform)
    {
        currentWalkingSound = Instantiate(soundEffectPrefab, spawnTransform.position, Quaternion.identity);
        currentWalkingSound.clip = sound;
        currentWalkingSound.loop = true;
        currentWalkingSound.Play();
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

