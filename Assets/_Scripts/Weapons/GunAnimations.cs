using System.Collections;
using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Weapons
{
    public class GunAnimations : MonoBehaviour
    {
        #region References
        private Quaternion _originLocalRotation;
        private Vector3 _originLocalPosition;
        private PlayerController _playerController;
        #endregion

        #region Animation State
        private Vector3 _targetSlideOffset;
        private Vector3 _jumpOffset = Vector3.zero;
        private Vector3 _stompOffset = Vector3.zero;
        private Vector3 _reloadPositionOffset = Vector3.zero;
        private Quaternion _reloadRotation = Quaternion.identity;
        private Coroutine _reloadAnimationCoroutine;
        #endregion

        #region Recoil Properties
        private Vector3 _currentRecoilOffset;
        private Quaternion _currentRecoilRotation = Quaternion.identity;
        private Vector3 _recoilOffsetAccum;
        private Vector3 _recoilRotationAccum;
        #endregion

        #region Sway Settings
        [Header("Sway Settings")]
        [SerializeField]
        private float balanceFactor = 3f;

        [SerializeField]
        private float timeFactor = 10f;
        #endregion

        #region Movement Effect Settings
        [Header("Slide/Dash Settings")]
        [SerializeField]
        private Vector3 slideOffset = new Vector3(0, 0, -0.3f);

        [SerializeField]
        private float slideLerpSpeed = 10f;

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
        #endregion

        #region Recoil Settings
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
        #endregion

        private void Start()
        {
            InitializeWeaponState();
        }

        private void InitializeWeaponState()
        {
            _originLocalRotation = transform.localRotation;
            _originLocalPosition = transform.localPosition;
            _targetSlideOffset = Vector3.zero;
            _recoilOffsetAccum = Vector3.zero;
            _recoilRotationAccum = Vector3.zero;
        }

        private void Update()
        {
            UpdateMovementState();
            ApplyGunSway();
            UpdateGunPosition();
            RecoverRecoil();
        }

        #region Movement State Updates
        private void UpdateMovementState()
        {
            _targetSlideOffset = ShouldApplyMovementOffset() ? slideOffset : Vector3.zero;
        }

        private bool ShouldApplyMovementOffset()
        {
            return _playerController
                && (_playerController.IsSliding || _playerController.IsDashing);
        }
        #endregion

        #region Animation Updates
        private void ApplyGunSway()
        {
            var mouseInput = GetMouseInput();
            var swayRotation = CalculateSwayRotation(mouseInput);
            ApplyRotationToWeapon(swayRotation);
        }

        private static Vector2 GetMouseInput()
        {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        private Quaternion CalculateSwayRotation(Vector2 mouseInput)
        {
            var swayRotation = Quaternion.identity;
            swayRotation *= Quaternion.AngleAxis(-mouseInput.x * balanceFactor, Vector3.up);
            swayRotation *= Quaternion.AngleAxis(mouseInput.y * balanceFactor, Vector3.right);
            return swayRotation;
        }

        private void ApplyRotationToWeapon(Quaternion swayRotation)
        {
            var targetRotation =
                _originLocalRotation * _currentRecoilRotation * swayRotation * _reloadRotation;
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                Time.deltaTime * timeFactor
            );
        }

        private void UpdateGunPosition()
        {
            var targetPosition = CalculateTargetPosition();
            MoveWeaponToTargetPosition(targetPosition);
            FadeOutTemporaryOffsets();
        }

        private Vector3 CalculateTargetPosition()
        {
            return _originLocalPosition
                + _targetSlideOffset
                + _currentRecoilOffset
                + _jumpOffset
                + _stompOffset
                + _reloadPositionOffset;
        }

        private void MoveWeaponToTargetPosition(Vector3 targetPosition)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetPosition,
                Time.deltaTime * slideLerpSpeed
            );
        }

        private void FadeOutTemporaryOffsets()
        {
            var returnSpeed = Time.deltaTime * recoilReturnSpeed;
            _jumpOffset = Vector3.Lerp(_jumpOffset, Vector3.zero, returnSpeed);
            _stompOffset = Vector3.Lerp(_stompOffset, Vector3.zero, returnSpeed);
        }

        private void RecoverRecoil()
        {
            RecoverRecoilPosition();
            RecoverRecoilRotation();
        }

        private void RecoverRecoilPosition()
        {
            var returnSpeed = Time.deltaTime * recoilReturnSpeed;
            _currentRecoilOffset = Vector3.Lerp(_currentRecoilOffset, Vector3.zero, returnSpeed);
            _recoilOffsetAccum = Vector3.Lerp(_recoilOffsetAccum, Vector3.zero, returnSpeed);
        }

        private void RecoverRecoilRotation()
        {
            var returnSpeed = Time.deltaTime * recoilReturnSpeed;
            _currentRecoilRotation = Quaternion.Lerp(
                _currentRecoilRotation,
                Quaternion.identity,
                returnSpeed
            );
            _recoilRotationAccum = Vector3.Lerp(_recoilRotationAccum, Vector3.zero, returnSpeed);
        }
        #endregion

        #region Public Methods
        public void ApplyRecoil()
        {
            ApplyPositionRecoil();
            ApplyRotationRecoil();
        }

        private void ApplyPositionRecoil()
        {
            var recoilPositionDelta = new Vector3(0, 0, -recoilForce);
            var targetOffset = _recoilOffsetAccum + recoilPositionDelta;

            targetOffset = ClampRecoilOffset(targetOffset);

            var offsetChange = targetOffset - _recoilOffsetAccum;
            _currentRecoilOffset += offsetChange;
            _recoilOffsetAccum = targetOffset;
        }

        private Vector3 ClampRecoilOffset(Vector3 offset)
        {
            offset.x = Mathf.Clamp(offset.x, -maxRecoilOffset.x, maxRecoilOffset.x);
            offset.y = Mathf.Clamp(offset.y, -maxRecoilOffset.y, maxRecoilOffset.y);
            offset.z = Mathf.Clamp(offset.z, -maxRecoilOffset.z, maxRecoilOffset.z);
            return offset;
        }

        private void ApplyRotationRecoil()
        {
            var recoilRotationDelta = new Vector3(0f, 0f, -recoilTilt);
            var targetRotation = _recoilRotationAccum + recoilRotationDelta;

            targetRotation = ClampRecoilRotation(targetRotation);

            var rotationChange = targetRotation - _recoilRotationAccum;
            _recoilRotationAccum = targetRotation;
            _currentRecoilRotation *= Quaternion.Euler(rotationChange);
        }

        private Vector3 ClampRecoilRotation(Vector3 rotation)
        {
            rotation.x = Mathf.Clamp(rotation.x, -maxRecoilRotation.x, maxRecoilRotation.x);
            rotation.y = Mathf.Clamp(rotation.y, -maxRecoilRotation.y, maxRecoilRotation.y);
            rotation.z = Mathf.Clamp(rotation.z, -maxRecoilRotation.z, maxRecoilRotation.z);
            return rotation;
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
        #endregion

        #region Reload Animation
        private IEnumerator ReloadAnimation(float duration)
        {
            float quarterDuration = duration / 4f;
            Vector3 downPosition = new Vector3(0f, -0.1f, 0f);

            yield return AnimateReloadDown(quarterDuration);
            yield return AnimateReloadBack(downPosition, quarterDuration);
            yield return AnimateReloadUp(downPosition, quarterDuration);
            yield return AnimateReloadReturn(quarterDuration);

            ResetReloadAnimationState();
        }

        private IEnumerator AnimateReloadDown(float duration)
        {
            return RotateToAngle(0f, 45f, duration);
        }

        private IEnumerator AnimateReloadBack(Vector3 downPosition, float duration)
        {
            return MoveToOffset(Vector3.zero, downPosition, 45f, duration);
        }

        private IEnumerator AnimateReloadUp(Vector3 downPosition, float duration)
        {
            return MoveToOffset(downPosition, Vector3.zero, 45f, duration);
        }

        private IEnumerator AnimateReloadReturn(float duration)
        {
            return RotateToAngle(45f, 0f, duration);
        }

        private void ResetReloadAnimationState()
        {
            _reloadRotation = Quaternion.identity;
            _reloadPositionOffset = Vector3.zero;
        }

        private IEnumerator RotateToAngle(float startAngle, float endAngle, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float currentAngle = Mathf.Lerp(startAngle, endAngle, progress);

                _reloadRotation = Quaternion.Euler(0f, 0f, currentAngle);
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
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);

                _reloadRotation = Quaternion.Euler(0f, 0f, fixedAngle);
                _reloadPositionOffset = Vector3.Lerp(start, end, progress);

                yield return null;
            }
        }
        #endregion
    }
}
