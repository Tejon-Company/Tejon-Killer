using _Scripts.Audio.Managers;
using _Scripts.Events;
using _Scripts.Menus;
using _Scripts.Weapons;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Components
        private CharacterController _player;
        private GunAnimations _gunAnimations;
        #endregion

        #region State Tracking
        public bool IsDashing { get; private set; }
        public bool IsSliding { get; private set; }
        private bool _isWalking;
        private bool _isStomping;
        private bool _wasGrounded;
        private bool _canDash = true;
        private bool _slideJumpInertiaActive;
        private bool _canSlideJump;
        private bool _skipGravityNextFrame;
        #endregion

        #region Input Tracking
        private bool _jumpInput;
        private bool _dashInput;
        private bool _slideInputHeld;
        private Vector3 _axis;
        #endregion

        #region Movement
        [SerializeField, Range(0, 40)]
        private float baseSpeed = 10f;
        private Vector3 _movePlayer;
        private float _fallVelocity;
        #endregion

        #region Dash
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
        private Vector3 _dashDirection;
        #endregion

        #region Slide
        [Header("SLIDE")]
        [SerializeField]
        private float slideSpeedMultiplier = 2.5f;

        [SerializeField]
        private float slideGravity = 14f;

        [SerializeField]
        private float slideJumpForce = 20f;

        [SerializeField]
        private float slideJumpInertiaMultiplier = 2f;
        private Vector3 _slideDirection;
        private float _originalHeight;
        private float _originalCenterY;
        private const float CrouchHeight = 1f;
        private const float CrouchCenterY = 0.5f;
        #endregion

        #region Jump
        [Header("JUMP")]
        [SerializeField]
        private float gravity = 25f;

        [SerializeField]
        private float jumpForce = 15f;

        [SerializeField]
        private float groundedGravity = -5f;

        [SerializeField]
        private float jumpBufferTime = 0.15f;
        private float _jumpBufferCounter;

        [Header("MULTI JUMP")]
        [SerializeField]
        private int maxJumps = 2;
        private int _jumpsRemaining;
        #endregion

        #region Stomp
        [Header("STOMP")]
        [SerializeField]
        private float stompForce = 40f;

        [SerializeField]
        private float stompTimeLimit = 0.3f;

        [SerializeField]
        private float stompJumpForceMultiplier = 1.5f;

        [SerializeField]
        private ParticleSystem stompParticles;
        private float _stompTimeCounter;
        #endregion

        #region Rage
        [Header("Rage Parameters")]
        private bool _isRaging;
        private float _rageEndTime;
        private float _originalBaseSpeed;
        private float _originalJumpForce;
        #endregion

        private void Awake() => InitializeComponents();

        private void InitializeComponents()
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
            EventManager.Instance?.rageBerryEvent.AddListener(ApplyRage);
        }

        private void OnDisable()
        {
            EventManager.Instance?.rageBerryEvent.RemoveListener(ApplyRage);
        }

        private void Update()
        {
            if (PauseMenu.IsPaused)
            {
                LoopSfxManager.Instance.StopLoopSfx();
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
            UpdatePlayerState(groundedNow);

            _wasGrounded = groundedNow;
        }

        private void UpdatePlayerState(bool isGrounded)
        {
            _isWalking =
                _axis.magnitude > 0.1f && isGrounded && !IsDashing && !IsSliding && !_isStomping;

            if (IsSliding)
                LoopSfxManager.Instance.PlayLoopSfx(LoopSfxManager.Instance.SlideSound);
            else if (_isWalking)
                LoopSfxManager.Instance.PlayLoopSfx(LoopSfxManager.Instance.Steps);
            else
                LoopSfxManager.Instance.StopLoopSfx();
        }

        private void HandleRageState()
        {
            if (!_isRaging || Time.time < _rageEndTime)
                return;

            baseSpeed = _originalBaseSpeed;
            jumpForce = _originalJumpForce;
            _isRaging = false;
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

            if (_isStomping)
                SfxManager.Instance.PlaySfx(SfxManager.Instance.Stomp);

            stompParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void HandleInput()
        {
            _jumpInput = Input.GetButtonDown("Jump") && _jumpsRemaining > 0;
            if (_jumpInput)
            {
                _jumpBufferCounter = jumpBufferTime;
                _gunAnimations?.TriggerJumpEffect();
            }

            _dashInput = Input.GetButtonDown("Sprint");
            _slideInputHeld = Input.GetButton("Crouch");

            var stompInput = Input.GetButtonDown("Crouch");
            if (stompInput && !_player.isGrounded && !_isStomping && !IsSliding)
                StartStomp();

            _axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (_axis.magnitude > 1f)
                _axis.Normalize();
        }

        private void HandleMovement()
        {
            if (IsDashing)
            {
                ProcessDash();
            }
            else if (IsSliding)
            {
                if (_canSlideJump && _jumpBufferCounter > 0f && _jumpsRemaining > 0)
                    PerformSlideJump();
                else
                    ProcessSlide();
            }
            else
            {
                ProcessNormalMovement();
            }

            _movePlayer.y = _fallVelocity;
        }

        private void ProcessNormalMovement()
        {
            var rawMovement = transform.TransformDirection(_axis);
            var currentSpeed =
                _slideJumpInertiaActive && !_player.isGrounded
                    ? baseSpeed * slideJumpInertiaMultiplier
                    : baseSpeed;

            _movePlayer.x = rawMovement.x * currentSpeed;
            _movePlayer.z = rawMovement.z * currentSpeed;

            transform.Rotate(0, Input.GetAxis("Mouse X"), 0);

            if (_dashInput && _canDash && _axis.magnitude > 0.1f)
                StartDash(rawMovement);

            if (_player.isGrounded && _slideInputHeld && _axis.magnitude > 0.1f && !IsSliding)
                StartSlide(rawMovement);
        }

        private void PerformSlideJump()
        {
            EndSlide();
            _slideDirection.y = 0f;
            _jumpsRemaining--;

            var jumpImpulse = _slideDirection * slideJumpInertiaMultiplier;
            _movePlayer = jumpImpulse;
            _fallVelocity = slideJumpForce;

            _slideJumpInertiaActive = true;
            _canSlideJump = false;
            _skipGravityNextFrame = true;
            _jumpBufferCounter = 0f;
        }

        public void SetPlayerGunAnimation(GunAnimations gunAnimation) =>
            _gunAnimations = gunAnimation;

        #region Dash Methods
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

        private void ProcessDash() => _movePlayer = _dashDirection;

        private void EndDash()
        {
            IsDashing = false;
            dashParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void ResetDash() => _canDash = true;
        #endregion

        #region Slide Methods
        private void StartSlide(Vector3 direction)
        {
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
                EndSlide();
        }

        private void EndSlide()
        {
            IsSliding = false;
            _player.height = _originalHeight;
            _player.center = new Vector3(_player.center.x, _originalCenterY, _player.center.z);
            speedParticles?.Stop();
        }
        #endregion

        #region Gravity & Jump Handling
        private void HandleGravity()
        {
            if (_player.isGrounded)
                HandleGroundedGravity();
            else
                HandleAirborneGravity();
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
                _fallVelocity = -1f;

            if (!_isStomping)
                _fallVelocity -= (IsSliding ? slideGravity : gravity) * Time.deltaTime;
        }
        #endregion

        #region Stomp Methods
        private void StartStomp()
        {
            _gunAnimations?.TriggerStompEffect();
            _isStomping = true;
            _fallVelocity = -stompForce;
            stompParticles?.Play();
        }

        private void CancelStomp()
        {
            _stompTimeCounter = stompTimeLimit;
            _isStomping = false;
        }
        #endregion

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
