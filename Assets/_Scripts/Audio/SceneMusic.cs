using System;
using _Scripts.Audio.Managers;
using UnityEngine;

namespace _Scripts.Managers.Audio
{
    public class SceneMusic : MonoBehaviour
    {
        [SerializeField]
        private AudioClip music;

        private void Start() => MusicManager.Instance.PlayMusic(music);
    }
}
