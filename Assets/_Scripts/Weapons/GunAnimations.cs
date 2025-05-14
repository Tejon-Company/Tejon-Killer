using System.Collections;
using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Weapons
{
    public class GunAnimations : MonoBehaviour
    {
        private Quaternion _originLocalRotation;
        private Vector3 _originLocalPosition;
        private Vector3 _targetSlideOffset;
        private Quaternion _reloadRotation = Quaternion.identity;

        [Header("Sway Settings")]
        [SerializeField]
        private float balanceFactor = 3f;

        [SerializeField]
        private float timeFactor = 10f;

        [Header("Slide/Dash Settings")]
        [SerializeField]
        private Vector3 slideOffset = new Vector3(0, 0, -0.3f);

        [SerializeField]
        private float slideLerpSpeed = 10f;

        [Header("Recoil Settings")]
        [SerializeField]
        private float recoilForce = 0.3f;

        [SerializeField]
        private float recoilTilt = -20f;

        [SerializeField]
        private float recoilReturnSpeed = 5f;

        [Header("Recoil Limits")]
        [SerializeField]
        private Vector3 maxRecoilOffset = new(0.2f, 0.2f, 0.2f);

        [SerializeField]
        private Vector3 maxRecoilRotation = new(20f, 20f, 20f);
        private Vector3 _jumpOffset = Vector3.zero;
        private Vector3 _currentRecoilOffset;
        private Quaternion _currentRecoilRotation;

        [Header("JUMP")]
        [SerializeField]
        private float jumpOffsetPosition = -0.2f;

        [SerializeField]
        private float jumpOffSetRotation = -20f;

        [Header("STOMP")]
        [SerializeField]
        private float stompOffsetPosition = 0.1f;

        [SerializeField]
        private float stompOffsetRotation = 340f;
        private Vector3 _stompOffset = Vector3.zero;

        private Vector3 _reloadPositionOffset = Vector3.zero;
        private Coroutine _reloadAnimationCoroutine;
        private Vector3 _recoilOffsetAccum;
        private Vector3 _recoilRotationAccum;

        [Header("Player Reference")]
        private PlayerController _playerController;

        private void Start()
        {
            _originLocalRotation = transform.localRotation;
            _originLocalPosition = transform.localPosition;
            _targetSlideOffset = Vector3.zero;

            _currentRecoilRotation = Quaternion.identity;
            _recoilOffsetAccum = Vector3.zero;
            _recoilRotationAccum = Vector3.zero;
        }

        private void Update()
        {
            UpdateSlideState();
            UpdateSwayWithRecoil();
            UpdatePositionWithSlideAndRecoil();
            RecoverRecoil();
        }

        private void UpdateSlideState()
        {
            if (_playerController && (_playerController.IsSliding || _playerController.IsDashing))
                _targetSlideOffset = slideOffset;
            else
                _targetSlideOffset = Vector3.zero;
        }

        private void UpdateSwayWithRecoil()
        {
            var xInput = Input.GetAxis("Mouse X");
            var yInput = Input.GetAxis("Mouse Y");

            var swayRotation = Quaternion.identity;
            swayRotation *= Quaternion.AngleAxis(-xInput * balanceFactor, Vector3.up);
            swayRotation *= Quaternion.AngleAxis(yInput * balanceFactor, Vector3.right);

            var targetRotation =
                _originLocalRotation * _currentRecoilRotation * swayRotation * _reloadRotation;

            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * timeFactor
            );
        }

        private void UpdatePositionWithSlideAndRecoil()
        {
            var finalPosition =
                _originLocalPosition
                + _targetSlideOffset
                + _currentRecoilOffset
                + _jumpOffset
                + _stompOffset
                + _reloadPositionOffset;

            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                finalPosition,
                Time.deltaTime * slideLerpSpeed
            );

            _jumpOffset = Vector3.Lerp(
                _jumpOffset,
                Vector3.zero,
                Time.deltaTime * recoilReturnSpeed
            );

            _stompOffset = Vector3.Lerp(
                _stompOffset,
                Vector3.zero,
                Time.deltaTime * recoilReturnSpeed
            );
        }

        private void RecoverRecoil()
        {
            _currentRecoilOffset = Vector3.Lerp(
                _currentRecoilOffset,
                Vector3.zero,
                Time.deltaTime * recoilReturnSpeed
            );
            _currentRecoilRotation = Quaternion.Lerp(
                _currentRecoilRotation,
                Quaternion.identity,
                Time.deltaTime * recoilReturnSpeed
            );

            _recoilOffsetAccum = Vector3.Lerp(
                _recoilOffsetAccum,
                Vector3.zero,
                Time.deltaTime * recoilReturnSpeed
            );
            _recoilRotationAccum = Vector3.Lerp(
                _recoilRotationAccum,
                Vector3.zero,
                Time.deltaTime * recoilReturnSpeed
            );
        }

        public void ApplyRecoil()
        {
            var addedOffset = new Vector3(0, 0, -recoilForce);
            var targetOffset = _recoilOffsetAccum + addedOffset;

            targetOffset.x = Mathf.Clamp(targetOffset.x, -maxRecoilOffset.x, maxRecoilOffset.x);
            targetOffset.y = Mathf.Clamp(targetOffset.y, -maxRecoilOffset.y, maxRecoilOffset.y);
            targetOffset.z = Mathf.Clamp(targetOffset.z, -maxRecoilOffset.z, maxRecoilOffset.z);

            var offsetDelta = targetOffset - _recoilOffsetAccum;
            _currentRecoilOffset += offsetDelta;
            _recoilOffsetAccum = targetOffset;

            var addedRotation = new Vector3(0f, 0f, -recoilTilt);
            var targetRotation = _recoilRotationAccum + addedRotation;

            targetRotation.x = Mathf.Clamp(
                targetRotation.x,
                -maxRecoilRotation.x,
                maxRecoilRotation.x
            );
            targetRotation.y = Mathf.Clamp(
                targetRotation.y,
                -maxRecoilRotation.y,
                maxRecoilRotation.y
            );
            targetRotation.z = Mathf.Clamp(
                targetRotation.z,
                -maxRecoilRotation.z,
                maxRecoilRotation.z
            );

            var rotationDelta = targetRotation - _recoilRotationAccum;
            _recoilRotationAccum = targetRotation;

            _currentRecoilRotation *= Quaternion.Euler(rotationDelta);
        }

        public void TriggerJumpEffect()
        {
            _jumpOffset = new Vector3(0, jumpOffsetPosition, 0);

            _currentRecoilRotation *= Quaternion.Euler(0f, 0f, jumpOffSetRotation);
        }

        public void TriggerStompEffect()
        {
            _stompOffset = new Vector3(0, -stompOffsetPosition, 0);

            _currentRecoilRotation *= Quaternion.Euler(0f, 0f, -stompOffsetRotation);
        }

        public void SetPlayerController(PlayerController controller)
        {
            _playerController = controller;
        }

        public void PlayReloadAnimation(float duration)
        {
            if (_reloadAnimationCoroutine != null)
                StopCoroutine(_reloadAnimationCoroutine);

            _reloadAnimationCoroutine = StartCoroutine(ReloadAnimation(duration));
        }

        private IEnumerator ReloadAnimation(float duration)
        {
            var quarter = duration / 4f;
            var downOffset = new Vector3(0f, -0.1f, 0f);

            yield return RotateToAngle(0f, 45f, quarter);
            yield return MoveToOffset(Vector3.zero, downOffset, 45f, quarter);
            yield return MoveToOffset(downOffset, Vector3.zero, 45f, quarter);
            yield return RotateToAngle(45f, 0f, quarter);

            _reloadRotation = Quaternion.identity;
            _reloadPositionOffset = Vector3.zero;
        }

        private IEnumerator RotateToAngle(float startAngle, float endAngle, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var angle = Mathf.Lerp(startAngle, endAngle, t);
                _reloadRotation = Quaternion.Euler(0f, 0f, angle);
                _reloadPositionOffset = Vector3.zero;
                yield return null;
            }
        }

        private IEnumerator MoveToOffset(
            Vector3 start,
            Vector3 end,
            float fixedAngle,
            float duration
        )
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                _reloadRotation = Quaternion.Euler(0f, 0f, fixedAngle);
                _reloadPositionOffset = Vector3.Lerp(start, end, t);
                yield return null;
            }
        }
    }
}
