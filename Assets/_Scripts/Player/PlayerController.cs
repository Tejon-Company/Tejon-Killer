using _Scripts.Managers;
using _Scripts.Managers.Audio;
using _Scripts.Menus;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController _player;

        public bool IsDashing { get; private set; }

        private bool _isWalking;

        public bool IsSliding { get; private set; }

        private bool _isStomping;

        [SerializeField, Range(0, 40)]
        private float baseSpeed = 10f;

        [Header("DASH")]
        [SerializeField]
        private float dashMultiplier = 10f;

        [SerializeField]
        private float dashDuration = 0.08f;

        [SerializeField]
        private float dashCooldown = 0.4f;

        [SerializeField]
        private ParticleSystem dashParticles;

        [SerializeField]
        private ParticleSystem speedParticles;

        [Header("SLIDE")]
        [SerializeField]
        private float slideSpeedMultiplier = 2.5f;

        [SerializeField]
        private float slideGravity = 14f;

        [SerializeField]
        private float slideJumpForce = 20f;

        [SerializeField]
        private float slideJumpInertiaMultiplier = 2f;
        private bool _slideJumpInertiaActive;
        private bool _canSlideJump;
        private bool _skipGravityNextFrame;

        [Header("JUMP")]
        [SerializeField]
        private float gravity = 25f;

        [SerializeField]
        private float jumpForce = 15f;

        [Header("MULTI JUMP")]
        [SerializeField]
        private int maxJumps = 2;

        private int _jumpsRemaining;

        [Header("GRAVITY IN FREE FALL")]
        [SerializeField]
        private float groundedGravity = -5f;

        [Header("STOMP")]
        [SerializeField]
        private float stompForce = 40f;

        [SerializeField]
        private float stompTimeLimit = 0.3f;

        [SerializeField]
        private float stompJumpForceMultiplier = 1.5f;
        private float _stompTimeCounter;

        [SerializeField]
        private ParticleSystem stompParticles;

        [Header("JUMP BUFFER")]
        [SerializeField]
        private float jumpBufferTime = 0.15f;

        private float _jumpBufferCounter;

        private bool jumpInput;
        private Vector3 slideDirection;
        private GunAnimations gunAnimations;
        private bool _canDash = true;

        private bool _dashInput,
            _slideInputHeld;

        private float _fallVelocity;

        private Vector3 _axis,
            _movePlayer,
            _dashDirection,
            _slideDirection;

        private float _originalHeight;

        private const float CrouchHeight = 1f;

        private float _originalCenterY;

        private const float CrouchCenterY = 0.5f;
        private GunAnimations _gunAnimation;
        private bool _wasGrounded;

        [Header("Rage Parameters")]
        private bool _isRaging;
        private float _rageEndTime;
        private float _originalBaseSpeed;
        private float _originalJumpForce;

        private void Awake()
        {
            speedParticles.Stop();
            dashParticles.Stop();
            stompParticles.Stop();
            _player = GetComponent<CharacterController>();
            _originalHeight = _player.height;
            _originalCenterY = _player.center.y;
            _wasGrounded = _player.isGrounded;
        }

        private void OnEnable()
        {
            EventManager.Current?.rageBerryEvent.AddListener(ApplyRage);
        }

        private void OnDisable()
        {
            EventManager.Current?.rageBerryEvent.RemoveListener(ApplyRage);
        }

        private void Update()
        {
            if (PauseMenu.IsPaused)
            {
                FootstepSfxManager.Instance.StopFootstepSfx();
                return;
            }

            HandleRageState();
            UpdateTimers();

            var groundedNow = _player.isGrounded;
            var justLanded = !_wasGrounded && groundedNow;

            HandleInput();
            HandleMovement();

            if (justLanded)
                OnLand();

            if (!_skipGravityNextFrame)
                HandleGravity();

            _skipGravityNextFrame = false;

            HandleSlideEnd();

            _player.Move(_movePlayer * Time.deltaTime);

            _isWalking =
                _axis.magnitude > 0.1f
                && _player.isGrounded
                && !IsDashing
                && !IsSliding
                && !_isStomping;

            if (_isWalking)
                FootstepSfxManager.Instance.PlayFootstepSfx(FootstepSfxManager.Instance.GrassSteps);
            else
                FootstepSfxManager.Instance.StopFootstepSfx();

            _wasGrounded = groundedNow;
        }

        private void HandleRageState()
        {
            if (_isRaging && Time.time >= _rageEndTime)
            {
                baseSpeed = _originalBaseSpeed;
                jumpForce = _originalJumpForce;
                _isRaging = false;
            }
        }

        private void UpdateTimers()
        {
            if (_stompTimeCounter > 0f)
                _stompTimeCounter -= Time.deltaTime;

            if (_jumpBufferCounter > 0f)
                _jumpBufferCounter -= Time.deltaTime;
        }

        private void OnLand()
        {
            _jumpsRemaining = maxJumps;
            _slideJumpInertiaActive = false;
            _stompTimeCounter = 0f;

            stompParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void HandleInput()
        {
            jumpInput = Input.GetButtonDown("Jump") && _jumpsRemaining > 0;
            if (jumpInput)
            {
                _jumpBufferCounter = jumpBufferTime;
                gunAnimations?.TriggerJumpEffect();
            }

            _dashInput = Input.GetButtonDown("Sprint");
            _slideInputHeld = Input.GetButton("Crouch");

            var stompInput = Input.GetButtonDown("Crouch");
            if (ShouldStomp(stompInput))
                StartStomp();
        }

        private bool ShouldStomp(bool stompInput)
        {
            return stompInput && !_player.isGrounded && !_isStomping && !IsSliding;
        }

        private void HandleMovement()
        {
            UpdateMovementInput();

            if (IsDashing)
            {
                ProcessDash();
                _movePlayer.y = _fallVelocity;
                return;
            }

            if (IsSliding)
            {
                if (CanPerformSlideJump())
                {
                    PerformSlideJump();
                    return;
                }

                ProcessSlide();
                _movePlayer.y = _fallVelocity;
                return;
            }

            ProcessNormalMovement(transform.TransformDirection(_axis));
            _movePlayer.y = _fallVelocity;
        }

        private void UpdateMovementInput()
        {
            _axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (_axis.magnitude > 1f)
                _axis.Normalize();
        }

        private bool CanPerformSlideJump()
        {
            return _canSlideJump && _jumpBufferCounter > 0f && _jumpsRemaining > 0;
        }

        private void PerformSlideJump()
        {
            EndSlide();
            _slideDirection.y = 0f;
            _jumpsRemaining--;

            var jumpImpulse = _slideDirection * slideJumpInertiaMultiplier;
            _movePlayer = jumpImpulse;
            _fallVelocity = slideJumpForce;
            _movePlayer.y = _fallVelocity;

            _slideJumpInertiaActive = true;
            _canSlideJump = false;
            _skipGravityNextFrame = true;

            _jumpBufferCounter = 0f;
        }

        private void ProcessNormalMovement(Vector3 rawMovement)
        {
            var currentSpeed = baseSpeed;

            if (_slideJumpInertiaActive && !_player.isGrounded)
            {
                currentSpeed *= slideJumpInertiaMultiplier;
            }

            _movePlayer.x = rawMovement.x * currentSpeed;
            _movePlayer.z = rawMovement.z * currentSpeed;

            transform.Rotate(0, Input.GetAxis("Mouse X"), 0);

            if (_dashInput && _canDash && _axis.magnitude > 0.1f)
            {
                StartDash(rawMovement);
            }

            if (_player.isGrounded && _slideInputHeld && _axis.magnitude > 0.1f && !IsSliding)
            {
                StartSlide(rawMovement);
            }
        }

        public void SetPlayerGunAnimation(GunAnimations gunAnimation)
        {
            gunAnimations = gunAnimation;
        }

        private void StartDash(Vector3 direction)
        {
            SfxManager.Instance.PlaySfx(SfxManager.Instance.Dash);

            IsDashing = true;
            _canDash = false;
            _dashDirection = direction.normalized * (baseSpeed * dashMultiplier);
            Invoke(nameof(EndDash), dashDuration);
            Invoke(nameof(ResetDash), dashCooldown);
            dashParticles?.Play();
        }

        private void ProcessDash()
        {
            _movePlayer = _dashDirection;
        }

        private void EndDash()
        {
            IsDashing = false;
            dashParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void ResetDash()
        {
            _canDash = true;
        }

        private void StartSlide(Vector3 direction)
        {
            SfxManager.Instance.PlaySfx(SfxManager.Instance.Slide);

            IsSliding = true;
            _canSlideJump = true;
            _slideDirection = direction.normalized * (baseSpeed * slideSpeedMultiplier);
            _player.height = CrouchHeight;
            _player.center = new Vector3(_player.center.x, CrouchCenterY, _player.center.z);

            speedParticles?.Play();
        }

        private void ProcessSlide()
        {
            _movePlayer.x = _slideDirection.x;
            _movePlayer.z = _slideDirection.z;
        }

        private void HandleSlideEnd()
        {
            if (!_slideInputHeld && IsSliding)
            {
                EndSlide();
            }
        }

        private void EndSlide()
        {
            IsSliding = false;
            _player.height = _originalHeight;
            _player.center = new Vector3(_player.center.x, _originalCenterY, _player.center.z);
            speedParticles?.Stop();
        }

        private void HandleGravity()
        {
            if (_player.isGrounded)
            {
                HandleGroundedGravity();
            }
            else
            {
                HandleAirborneGravity();
            }

            _movePlayer.y = _fallVelocity;
        }

        private void HandleGroundedGravity()
        {
            _slideJumpInertiaActive = false;
            _jumpsRemaining = maxJumps;

            if (_isStomping)
            {
                CancelStomp();
                return;
            }

            if (_jumpBufferCounter <= 0)
            {
                if (_fallVelocity <= 0f)
                    _fallVelocity = groundedGravity;
                return;
            }

            if (_stompTimeCounter > 0f)
            {
                EndSlide();
                _fallVelocity = jumpForce * stompJumpForceMultiplier;
                _jumpsRemaining = maxJumps - 1;
                _stompTimeCounter = 0f;
            }
            else
            {
                _fallVelocity = jumpForce;
                _jumpsRemaining--;
                SfxManager.Instance.PlaySfx(SfxManager.Instance.Jump);
            }

            _jumpBufferCounter = 0f;
        }

        private void HandleAirborneGravity()
        {
            var canJump = _jumpBufferCounter > 0 && _jumpsRemaining > 0 && !_isStomping;
            var isPlayerJumping =
                _player.collisionFlags == CollisionFlags.Above && _fallVelocity > 0;
            if (canJump)
            {
                _fallVelocity = jumpForce;
                _jumpsRemaining--;
                _jumpBufferCounter = 0f;
                SfxManager.Instance.PlaySfx(SfxManager.Instance.DoubleJump);
            }

            if (isPlayerJumping)
            {
                _fallVelocity = -1f;
            }

            if (!_isStomping)
            {
                _fallVelocity -= (IsSliding ? slideGravity : gravity) * Time.deltaTime;
            }

            if (_stompTimeCounter > 0)
            {
                _stompTimeCounter -= Time.deltaTime;
            }
        }

        private void CancelStomp()
        {
            _stompTimeCounter = stompTimeLimit;
            _isStomping = false;
        }

        private void StartStomp()
        {
            gunAnimations?.TriggerStompEffect();
            _isStomping = true;
            _fallVelocity = -stompForce;
            if (stompParticles != null)
            {
                stompParticles.Play();
            }
        }

        public Vector3 GetSlideDirection()
        {
            return slideDirection;
        }

        private void ApplyRage(
            float playerBaseSpeedMultiplier,
            float playerJumpForceMultiplier,
            float weaponFireRateMultiplier,
            float duration
        )
        {
            if (!_isRaging)
            {
                _originalBaseSpeed = baseSpeed;
                _originalJumpForce = jumpForce;
            }

            baseSpeed = _originalBaseSpeed * playerBaseSpeedMultiplier;
            jumpForce = _originalJumpForce * playerJumpForceMultiplier;

            _rageEndTime = Time.time + duration;
            _isRaging = true;
        }
    }
}
