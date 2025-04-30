using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    public static SoundEffectsManager instance;

    [SerializeField] private AudioSource soundEffectObject;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void Walking(AudioClip walkingSound, Transform spawnTransform)
    {
        
        if (!soundEffectObject.isPlaying) 
        {
            AudioSource audioSource = Instantiate(
            soundEffectObject, spawnTransform.position, Quaternion.identity
            );
            audioSource.clip = walkingSound;
            audioSource.Play();
            
        }

    }
}