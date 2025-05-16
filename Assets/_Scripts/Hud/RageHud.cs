using System.Collections;
using _Scripts.Events;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Hud
{
    /// <summary>
    /// Gestiona la visualización de la barra de rabia en la interfaz, mostrando el tiempo
    /// restante cuando se activa el estado de rabia y proporcionando efectos visuales.
    /// </summary>
    public class RageHud : MonoBehaviour
    {
        [SerializeField]
        private GameObject frameRoot;

        [SerializeField]
        private Image rageBarFill;

        private float _maxDuration;
        private float _endTime;
        private bool _isActive;
        private bool _listenerRegistered;
        private float _remainingTime;

        [Header("PULSE EFFECT")]
        [SerializeField]
        private float pulseSpeed = 0.2f;

        [SerializeField]
        private float pulseForce = 0.05f;
        private Vector3 _originalScale;

        private void Start()
        {
            _originalScale = frameRoot.transform.localScale;
            frameRoot.SetActive(false);
        }

        private void Update() => UpdateRageBar();

        private void OnEnable() => StartCoroutine(RegisterEventListener());

        private void OnDisable()
        {
            if (!EventManager.Instance)
                return;

            EventManager.Instance.rageBerryEvent.RemoveListener(OnRageActivated);
            _listenerRegistered = false;
        }

        private IEnumerator RegisterEventListener()
        {
            yield return null;

            if (_listenerRegistered || !EventManager.Instance)
                yield break;

            EventManager.Instance.rageBerryEvent.AddListener(OnRageActivated);
            _listenerRegistered = true;
        }

        private void UpdateRageBar()
        {
            if (!_isActive)
                return;

            _remainingTime = _endTime - Time.time;

            if (EndRage())
                return;

            rageBarFill.fillAmount = _remainingTime / _maxDuration;

            var pulse = Mathf.PingPong(Time.time * pulseSpeed, pulseForce);
            frameRoot.transform.localScale = _originalScale + new Vector3(pulse, pulse, pulse);
        }

        private bool EndRage()
        {
            if (_remainingTime > 0f)
                return false;

            rageBarFill.fillAmount = 0f;
            _isActive = false;
            frameRoot.SetActive(false);
            frameRoot.transform.localScale = _originalScale;
            return true;
        }

        private void OnRageActivated(
            float baseSpeed,
            float jumpForce,
            float fireRate,
            float duration
        )
        {
            frameRoot.SetActive(true);
            InitializeRageBar(duration);
        }

        private void InitializeRageBar(float duration)
        {
            _maxDuration = duration;
            _endTime = Time.time + duration;
            rageBarFill.fillAmount = 1f;
            _isActive = true;
        }
    }
}
