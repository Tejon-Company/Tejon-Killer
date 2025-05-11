using System.Collections;
using UnityEngine;

public class GunAnimations : MonoBehaviour
{
    private Quaternion originLocalRotation;
    private Vector3 originLocalPosition;
    private Vector3 targetSlideOffset;
    private Quaternion reloadRotation = Quaternion.identity;

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
    private Vector3 maxRecoilOffset = new Vector3(0.2f, 0.2f, 0.2f);

    [SerializeField]
    private Vector3 maxRecoilRotation = new Vector3(20f, 20f, 20f);
    private Vector3 jumpOffset = Vector3.zero;
    private Vector3 currentRecoilOffset;
    private Quaternion currentRecoilRotation;

    [Header("JUMP")]
    [SerializeField]
    private float jumpOffsetposition = -0.2f;

    [SerializeField]
    private float jumpOffsetrotarion = -20f;

    [Header("STOMP")]
    [SerializeField]
    private float stompOffsetPosition = 0.1f;

    [SerializeField]
    private float stompOffsetRotation = 340f;
    private Vector3 stompOffset = Vector3.zero;

    private Vector3 reloadPositionOffset = Vector3.zero;
    private Coroutine reloadAnimationCoroutine;
    private Vector3 recoilOffsetAccum;
    private Vector3 recoilRotationAccum;

    [Header("Player Reference")]
    public PlayerController playerController { get; private set; }

    private void Start()
    {
        originLocalRotation = transform.localRotation;
        originLocalPosition = transform.localPosition;
        targetSlideOffset = Vector3.zero;

        currentRecoilRotation = Quaternion.identity;
        recoilOffsetAccum = Vector3.zero;
        recoilRotationAccum = Vector3.zero;
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
        if (playerController != null && (playerController.IsSliding || playerController.IsDashing))
        {
            targetSlideOffset = slideOffset;
        }
        else
        {
            targetSlideOffset = Vector3.zero;
        }
    }

    private void UpdateSwayWithRecoil()
    {
        float xInput = Input.GetAxis("Mouse X");
        float yInput = Input.GetAxis("Mouse Y");

        Quaternion swayRotation = Quaternion.identity;
        swayRotation *= Quaternion.AngleAxis(-xInput * balanceFactor, Vector3.up);
        swayRotation *= Quaternion.AngleAxis(yInput * balanceFactor, Vector3.right);

        Quaternion targetRotation =
            originLocalRotation * currentRecoilRotation * swayRotation * reloadRotation;

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * timeFactor
        );
    }

    private void UpdatePositionWithSlideAndRecoil()
    {
        Vector3 finalPosition =
            originLocalPosition
            + targetSlideOffset
            + currentRecoilOffset
            + jumpOffset
            + stompOffset
            + reloadPositionOffset;
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            finalPosition,
            Time.deltaTime * slideLerpSpeed
        );

        jumpOffset = Vector3.Lerp(jumpOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);

        stompOffset = Vector3.Lerp(stompOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
    }

    private void RecoverRecoil()
    {
        currentRecoilOffset = Vector3.Lerp(
            currentRecoilOffset,
            Vector3.zero,
            Time.deltaTime * recoilReturnSpeed
        );
        currentRecoilRotation = Quaternion.Lerp(
            currentRecoilRotation,
            Quaternion.identity,
            Time.deltaTime * recoilReturnSpeed
        );

        recoilOffsetAccum = Vector3.Lerp(
            recoilOffsetAccum,
            Vector3.zero,
            Time.deltaTime * recoilReturnSpeed
        );
        recoilRotationAccum = Vector3.Lerp(
            recoilRotationAccum,
            Vector3.zero,
            Time.deltaTime * recoilReturnSpeed
        );
    }

    public void ApplyRecoil()
    {
        Vector3 addedOffset = new Vector3(0, 0, -recoilForce);
        Vector3 targetOffset = recoilOffsetAccum + addedOffset;

        targetOffset.x = Mathf.Clamp(targetOffset.x, -maxRecoilOffset.x, maxRecoilOffset.x);
        targetOffset.y = Mathf.Clamp(targetOffset.y, -maxRecoilOffset.y, maxRecoilOffset.y);
        targetOffset.z = Mathf.Clamp(targetOffset.z, -maxRecoilOffset.z, maxRecoilOffset.z);

        Vector3 offsetDelta = targetOffset - recoilOffsetAccum;
        currentRecoilOffset += offsetDelta;
        recoilOffsetAccum = targetOffset;

        Vector3 addedRotation = new Vector3(0f, 0f, -recoilTilt);
        Vector3 targetRotation = recoilRotationAccum + addedRotation;

        targetRotation.x = Mathf.Clamp(targetRotation.x, -maxRecoilRotation.x, maxRecoilRotation.x);
        targetRotation.y = Mathf.Clamp(targetRotation.y, -maxRecoilRotation.y, maxRecoilRotation.y);
        targetRotation.z = Mathf.Clamp(targetRotation.z, -maxRecoilRotation.z, maxRecoilRotation.z);

        Vector3 rotationDelta = targetRotation - recoilRotationAccum;
        recoilRotationAccum = targetRotation;

        currentRecoilRotation *= Quaternion.Euler(rotationDelta);
    }

    public void TriggerJumpEffect()
    {
        jumpOffset = new Vector3(0, jumpOffsetposition, 0);

        currentRecoilRotation *= Quaternion.Euler(0f, 0f, jumpOffsetrotarion);
    }

    public void TriggerStompEffect()
    {
        stompOffset = new Vector3(0, -stompOffsetPosition, 0);

        currentRecoilRotation *= Quaternion.Euler(0f, 0f, -stompOffsetRotation);
    }

    public void SetPlayerController(PlayerController controller)
    {
        playerController = controller;
    }

    public void PlayReloadAnimation(float duration)
    {
        if (reloadAnimationCoroutine != null)
            StopCoroutine(reloadAnimationCoroutine);

        reloadAnimationCoroutine = StartCoroutine(ReloadAnimation(duration));
    }

    private IEnumerator ReloadAnimation(float duration)
    {
        float elapsed = 0f;
        float quarter = duration / 4f;
        Vector3 downOffset = new Vector3(0f, -0.1f, 0f);

        while (elapsed < quarter)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / quarter);
            float angle = Mathf.Lerp(0f, 45f, t);
            reloadRotation = Quaternion.Euler(0f, 0f, angle);
            reloadPositionOffset = Vector3.zero;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < quarter)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / quarter);
            reloadRotation = Quaternion.Euler(0f, 0f, 45f);
            reloadPositionOffset = Vector3.Lerp(Vector3.zero, downOffset, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < quarter)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / quarter);
            reloadRotation = Quaternion.Euler(0f, 0f, 45f);
            reloadPositionOffset = Vector3.Lerp(downOffset, Vector3.zero, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < quarter)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / quarter);
            float angle = Mathf.Lerp(45f, 0f, t);
            reloadRotation = Quaternion.Euler(0f, 0f, angle);
            reloadPositionOffset = Vector3.zero;
            yield return null;
        }

        reloadRotation = Quaternion.identity;
        reloadPositionOffset = Vector3.zero;
    }
}
