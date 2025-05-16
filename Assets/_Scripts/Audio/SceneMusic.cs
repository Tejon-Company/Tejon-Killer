using _Scripts.Audio.Managers;
using UnityEngine;

namespace _Scripts.Audio
{
    /// <summary>
    /// Clase encargada de reproducir automáticamente la música asignada cuando se carga una escena.
    /// </summary>
    public class SceneMusic : MonoBehaviour
    {
        [SerializeField]
        private AudioClip music;

        private void Start() => MusicManager.Instance.PlayMusic(music);
    }
}
