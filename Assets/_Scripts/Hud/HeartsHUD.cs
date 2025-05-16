using System.Collections;
using System.Collections.Generic;
using _Scripts.Events;
using _Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Hud
{
    public class HeartsHud : MonoBehaviour
    {
        [SerializeField]
        private Sprite heartSprite;

        [SerializeField]
        private PlayerHealth playerHealth;

        [SerializeField]
        private int heartPixelSize = 5;

        private const float FlickerSpeed = 6f;

        private readonly List<GameObject> _hearts = new();
        private int _currentLives;
        private int _totalHearts;
        private Coroutine _flickerCoroutine;
        private float _flickerTime = Mathf.PI / 2f;
        private float _alpha;
        private Color _currentHeartColor;

        private void OnEnable()
        {
            if (!EventManager.Instance)
                return;

            EventManager.Instance.healthChangedEvent.AddListener(UpdateHearts);
            EventManager.Instance.damageCooldownEvent.AddListener(SetFlickerState);
            UpdateHearts();
        }

        private void OnDisable()
        {
            if (!EventManager.Instance)
                return;

            EventManager.Instance.damageCooldownEvent.RemoveListener(SetFlickerState);
            EventManager.Instance.healthChangedEvent.RemoveListener(UpdateHearts);
        }

        private void Start()
        {
            _totalHearts = playerHealth.MaxHealth;

            for (var i = 0; i < _totalHearts; i++)
            {
                var heart = CreateHeart();
                _hearts.Add(heart);
            }

            UpdateHearts();
        }

        private GameObject CreateHeart()
        {
            var heart = new GameObject(
                "Heart",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );
            heart.transform.SetParent(transform, false);
            heart.SetActive(true);

            var heartImage = heart.GetComponent<Image>();
            heartImage.sprite = heartSprite;

            var rt = heart.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(heartPixelSize, heartPixelSize);

            return heart;
        }

        private void UpdateHearts()
        {
            _currentLives = Mathf.Clamp(playerHealth.CurrentHealth, 0, playerHealth.MaxHealth);

            for (var i = 0; i < _hearts.Count; i++)
                _hearts[i].SetActive(i < _currentLives);
        }

        private void SetFlickerState(bool shouldFlicker)
        {
            if (shouldFlicker)
            {
                _flickerCoroutine ??= StartCoroutine(FlickerHearts());
                return;
            }

            if (_flickerCoroutine == null)
                return;

            StopCoroutine(_flickerCoroutine);
            _flickerCoroutine = null;
            SetHeartsAlpha(1f);
        }

        private IEnumerator FlickerHearts()
        {
            while (true)
            {
                _alpha = Mathf.Lerp(0.3f, 1f, (Mathf.Sin(_flickerTime) + 1f) / 2f);
                SetHeartsAlpha(_alpha);
                _flickerTime += Time.deltaTime * FlickerSpeed;
                yield return null;
            }
        }

        private void SetHeartsAlpha(float alpha)
        {
            foreach (var heart in _hearts)
            {
                if (!heart.TryGetComponent(out Image img))
                    continue;

                _currentHeartColor = img.color;
                _currentHeartColor.a = alpha;
                img.color = _currentHeartColor;
            }
        }
    }
}
