using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AUDIO SOURCE")]
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;
    
    [SerializeField]
    private AudioSource footstepSource;

    [Header("AUDIO CLIP")]
    [SerializeField]
    private AudioClip menuBackgroundMusic;
    public AudioClip MenuBackgroundMusic => menuBackgroundMusic;

    [SerializeField]
    private AudioClip shoot;
    public AudioClip Shoot => shoot;

    [SerializeField]
    private AudioClip reload;
    public AudioClip Reload => reload;

    [SerializeField]
    private AudioClip jump;
    public AudioClip Jump => jump;

    [SerializeField]
    private AudioClip grassSteps;
    public AudioClip GrassSteps => grassSteps;

    [SerializeField]
    private AudioClip stoneSteps;
    public AudioClip StoneSteps => stoneSteps;

    [SerializeField]
    private AudioClip doubleJump;
    public AudioClip DoubleJump => doubleJump;

    [SerializeField]
    private AudioClip slide;
    public AudioClip Slide => slide;

    [SerializeField]
    private AudioClip stomp;
    public AudioClip Stomp => stomp;

    [SerializeField]
    private AudioClip dash;
    public AudioClip Dash => dash;

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

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource.isPlaying && sfxSource.clip == clip)
            return;
            
        sfxSource.PlayOneShot(clip);
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
