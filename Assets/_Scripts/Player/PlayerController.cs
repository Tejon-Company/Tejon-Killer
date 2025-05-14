using _Scripts.Managers;
using _Scripts.Managers.Audio;
using _Scripts.Menus;
using _Scripts.Weapons;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {
        private CharacterController player;

        public bool IsDashing { get; private set; }

        private bool isWalking;

        public bool IsSliding { get; private set; }

        private bool isStomping;

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
        private bool slideJumpInertiaActive;
        private bool canSlideJump;
        private bool skipGravityNextFrame;

        [Header("JUMP")]
        [SerializeField]
        private float gravity = 25f;

        [SerializeField]
        private float jumpForce = 15f;

        [Header("MULTI JUMP")]
        [SerializeField]
        private int maxJumps = 2;

        private int jumpsRemaining;

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
        private float stompTimeCounter;

        [SerializeField]
        private ParticleSystem stompParticles;

        [Header("JUMP BUFFER")]
        [SerializeField]
        private float jumpBufferTime = 0.15f;

        private float jumpBufferCounter;

        
        private bool jumpInput;
        private GunAnimations gunAnimations;
        private bool canDash = true;

        private bool dashInput,
            slideInputHeld;

        private float fallVelocity;

        private Vector3 axis,
            movePlayer,
            dashDirection,
            slideDirection;

        private float originalHeight;

        private const float CrouchHeight = 1f;

        private float originalCenterY;

        private const float CrouchCenterY = 0.5f;
        private GunAnimations gunAnimation;
        private bool wasGrounded;

        [Header("Rage Parameters")]
        private bool isRaging;
        private float rageEndTime;
        private float originalBaseSpeed;
        private float originalJumpForce;

        private void Awake()
        {
            speedParticles.Stop();
            dashParticles.Stop();
            stompParticles.Stop();
            player = GetComponent<CharacterController>();
            originalHeight = player.height;
            originalCenterY = player.center.y;
            wasGrounded = player.isGrounded;
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
                LoopSfxManager.Instance.StopLoopSfx();
                return;
            }

            HandleRageState();
            UpdateTimers();

            var groundedNow = player.isGrounded;
            var justLanded = !wasGrounded && groundedNow;

            HandleInput();
            HandleMovement();

            if (justLanded)
                OnLand();

            if (!skipGravityNextFrame)
                HandleGravity();

            skipGravityNextFrame = false;

            HandleSlideEnd();

            player.Move(movePlayer * Time.deltaTime);

            isWalking =
                axis.magnitude > 0.1f
                && player.isGrounded
                && !IsDashing
                && !IsSliding
                && !isStomping;

            if (IsSliding)
            {
                LoopSfxManager.Instance.PlayLoopSfx(LoopSfxManager.Instance.SlideSound);
            }
            else if (isWalking)
            {
                LoopSfxManager.Instance.PlayLoopSfx(LoopSfxManager.Instance.GrassSteps);
            }
            else
            {
                LoopSfxManager.Instance.StopLoopSfx();
            }

            wasGrounded = groundedNow;
        }

        private void HandleRageState()
        {
            if (isRaging && Time.time >= rageEndTime)
            {
                baseSpeed = originalBaseSpeed;
                jumpForce = originalJumpForce;
                isRaging = false;
            }
        }

        private void UpdateTimers()
        {
            if (stompTimeCounter > 0f)
                stompTimeCounter -= Time.deltaTime;

            if (jumpBufferCounter > 0f)
                jumpBufferCounter -= Time.deltaTime;
        }

        private void OnLand()
        {
            jumpsRemaining = maxJumps;
            slideJumpInertiaActive = false;
            stompTimeCounter = 0f;

            stompParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void HandleInput()
        {
            jumpInput = Input.GetButtonDown("Jump") && jumpsRemaining > 0;
            if (jumpInput)
            {
                jumpBufferCounter = jumpBufferTime;
                gunAnimations?.TriggerJumpEffect();
            }

            dashInput = Input.GetButtonDown("Sprint");
            slideInputHeld = Input.GetButton("Crouch");

            var stompInput = Input.GetButtonDown("Crouch");
            if (ShouldStomp(stompInput))
                StartStomp();
        }

        private bool ShouldStomp(bool stompInput)
        {
            return stompInput && !player.isGrounded && !isStomping && !IsSliding;
        }

        private void HandleMovement()
        {
            UpdateMovementInput();

            if (IsDashing)
            {
                ProcessDash();
                movePlayer.y = fallVelocity;
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
                movePlayer.y = fallVelocity;
                return;
            }

            ProcessNormalMovement(transform.TransformDirection(axis));
            movePlayer.y = fallVelocity;
        }

        private void UpdateMovementInput()
        {
            axis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (axis.magnitude > 1f)
                axis.Normalize();
        }

        private bool CanPerformSlideJump()
        {
            return canSlideJump && jumpBufferCounter > 0f && jumpsRemaining > 0;
        }

        private void PerformSlideJump()
        {
            EndSlide();
            slideDirection.y = 0f;
            jumpsRemaining--;

            var jumpImpulse = slideDirection * slideJumpInertiaMultiplier;
            movePlayer = jumpImpulse;
            fallVelocity = slideJumpForce;
            movePlayer.y = fallVelocity;

            slideJumpInertiaActive = true;
            canSlideJump = false;
            skipGravityNextFrame = true;

            jumpBufferCounter = 0f;
        }

        private void ProcessNormalMovement(Vector3 rawMovement)
        {
            var currentSpeed = baseSpeed;

            if (slideJumpInertiaActive && !player.isGrounded)
            {
                currentSpeed *= slideJumpInertiaMultiplier;
            }

            movePlayer.x = rawMovement.x * currentSpeed;
            movePlayer.z = rawMovement.z * currentSpeed;

            transform.Rotate(0, Input.GetAxis("Mouse X"), 0);

            if (dashInput && canDash && axis.magnitude > 0.1f)
            {
                StartDash(rawMovement);
            }

            if (player.isGrounded && slideInputHeld && axis.magnitude > 0.1f && !IsSliding)
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
            canDash = false;
            dashDirection = direction.normalized * (baseSpeed * dashMultiplier);
            Invoke(nameof(EndDash), dashDuration);
            Invoke(nameof(ResetDash), dashCooldown);
            dashParticles?.Play();
        }

        private void ProcessDash()
        {
            movePlayer = dashDirection;
        }

        private void EndDash()
        {
            IsDashing = false;
            dashParticles?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void ResetDash()
        {
            canDash = true;
        }

        private void StartSlide(Vector3 direction)
        {
            IsSliding = true;
            canSlideJump = true;
            slideDirection = direction.normalized * (baseSpeed * slideSpeedMultiplier);
            player.height = CrouchHeight;
            player.center = new Vector3(player.center.x, CrouchCenterY, player.center.z);

            speedParticles?.Play();
        }

        private void ProcessSlide()
        {
            movePlayer.x = slideDirection.x;
            movePlayer.z = slideDirection.z;
        }

        private void HandleSlideEnd()
        {
            if (!slideInputHeld && IsSliding)
            {
                EndSlide();
            }
        }

        private void EndSlide()
        {
            IsSliding = false;
            player.height = originalHeight;
            player.center = new Vector3(player.center.x, originalCenterY, player.center.z);
            speedParticles?.Stop();
        }

        private void HandleGravity()
        {
            if (player.isGrounded)
            {
                HandleGroundedGravity();
            }
            else
            {
                HandleAirborneGravity();
            }

            movePlayer.y = fallVelocity;
        }

        private void HandleGroundedGravity()
        {
            slideJumpInertiaActive = false;
            jumpsRemaining = maxJumps;

            if (isStomping)
            {
                CancelStomp();
                return;
            }

            if (jumpBufferCounter <= 0)
            {
                if (fallVelocity <= 0f)
                    fallVelocity = groundedGravity;
                return;
            }

            if (stompTimeCounter > 0f)
            {
                EndSlide();
                fallVelocity = jumpForce * stompJumpForceMultiplier;
                jumpsRemaining = maxJumps - 1;
                stompTimeCounter = 0f;
            }
            else
            {
                fallVelocity = jumpForce;
                jumpsRemaining--;
                SfxManager.Instance.PlaySfx(SfxManager.Instance.Jump);
            }

            jumpBufferCounter = 0f;
        }

        private void HandleAirborneGravity()
        {
            var canJump = jumpBufferCounter > 0 && jumpsRemaining > 0 && !isStomping;
            var isPlayerJumping =
                player.collisionFlags == CollisionFlags.Above && fallVelocity > 0;
            if (canJump)
            {
                fallVelocity = jumpForce;
                jumpsRemaining--;
                jumpBufferCounter = 0f;
                SfxManager.Instance.PlaySfx(SfxManager.Instance.DoubleJump);
            }

            if (isPlayerJumping)
            {
                fallVelocity = -1f;
            }

            if (!isStomping)
            {
                fallVelocity -= (IsSliding ? slideGravity : gravity) * Time.deltaTime;
            }

            if (stompTimeCounter > 0)
            {
                stompTimeCounter -= Time.deltaTime;
            }
        }

        private void CancelStomp()
        {
            stompTimeCounter = stompTimeLimit;
            isStomping = false;
        }

        private void StartStomp()
        {
            gunAnimations?.TriggerStompEffect();
            isStomping = true;
            fallVelocity = -stompForce;
            stompParticles?.Play();
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
            if (!isRaging)
            {
                originalBaseSpeed = baseSpeed;
                originalJumpForce = jumpForce;
            }

            baseSpeed = originalBaseSpeed * playerBaseSpeedMultiplier;
            jumpForce = originalJumpForce * playerJumpForceMultiplier;

            rageEndTime = Time.time + duration;
            isRaging = true;
        }
    }
}
