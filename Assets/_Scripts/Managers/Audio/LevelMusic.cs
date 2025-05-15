using System;
using UnityEngine;

namespace _Scripts.Managers.Audio
{
    public class LevelMusic : MonoBehaviour
    {
        [SerializeField] private AudioClip music;
        private void Start()
        {
            MusicManager.Instance.PlayMusic(music);
        }
    }
}