using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance { get; private set; }

    [SerializeField] private AudioSource soundEffectPrefab;

    private AudioSource currentWalkingSound;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip walkingOnGrassSound;

    [SerializeField] private AudioClip walkingOnStoneSound;

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
        if (playerController.isWalking)
        {
            if (currentWalkingSound == null)
            {
                ReproduceWalkingSound(transform);
            }
        }
        else
        {
            StopSound();
        }
    }

    public void ReproduceWalkingSound(Transform spawnTransform)
    {
        currentWalkingSound = Instantiate(soundEffectPrefab, spawnTransform.position, Quaternion.identity);
        currentWalkingSound.clip = walkingOnGrassSound;
        currentWalkingSound.loop = true;
        currentWalkingSound.Play();
    }

    public void StopSound()
    {
        if (currentWalkingSound != null)
        {
            currentWalkingSound.Stop();
            Destroy(currentWalkingSound.gameObject);
            currentWalkingSound = null;
        }
    }
}

