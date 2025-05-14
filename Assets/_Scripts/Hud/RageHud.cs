using _Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Hud
{
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

        private void Update()
        {
            UpdateRageBar();
        }

        private void OnEnable()
        {
            if (EventManager.Current)
                EventManager.Current.rageBerryEvent.AddListener(OnRageActivated);
        }

        private void OnDisable()
        {
            if (EventManager.Current)
                EventManager.Current.rageBerryEvent.RemoveListener(OnRageActivated);
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
