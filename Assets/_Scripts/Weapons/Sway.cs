using UnityEngine;

public class Sway : MonoBehaviour
{
    private Quaternion originLocalRotation;
    private Vector3 originLocalPosition;
    private Vector3 targetSlideOffset;

    [Header("Sway Settings")]
    [SerializeField] private float balanceFactor = 3f;
    [SerializeField] private float timeFactor = 10f;

    [Header("Slide/Dash Settings")]
    [SerializeField] private Vector3 slideOffset = new Vector3(0, 0, -0.3f); // Desplazamiento del slide
    [SerializeField] private float slideLerpSpeed = 10f; // Velocidad de interpolación para el slide

    [Header("Recoil Settings")]
    [SerializeField] private float recoilForce = 0.3f;
    [SerializeField] private float recoilTilt = -20f;
    [SerializeField] private float recoilReturnSpeed = 5f;

    [Header("Recoil Limits")]
    [SerializeField] private Vector3 maxRecoilOffset = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private Vector3 maxRecoilRotation = new Vector3(20f, 20f, 20f);
    private Vector3 jumpOffset = Vector3.zero;
    private Vector3 currentRecoilOffset;
    private Quaternion currentRecoilRotation;

    [Header("JUMP")]
    [SerializeField] private float jumpOffsetposition = -0.2f;
    [SerializeField] private float jumpOffsetrotarion = -20f;

    [Header("STOMP")]
    [SerializeField] private float stompOffsetPosition = 0.1f;  // Efecto de impulso hacia arriba
    [SerializeField] private float stompOffsetRotation = 340f;
    private Vector3 stompOffset = Vector3.zero;


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
        if (playerController != null && (playerController.sliding || playerController.dashing))
        {
            // Desplazar durante el slide/dash
            targetSlideOffset = slideOffset;
        }
        else
        {
            // Volver a la posición original si no se está deslizándose
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

        Quaternion targetRotation = originLocalRotation * currentRecoilRotation * swayRotation;

        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * timeFactor);
    }

    private void UpdatePositionWithSlideAndRecoil()
    {
        // Apply the jump offset along with slide, recoil, and stomp
        Vector3 finalPosition = originLocalPosition + targetSlideOffset + currentRecoilOffset + jumpOffset + stompOffset;
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * slideLerpSpeed);

        // Optionally, recover the jump effect over time if needed
        jumpOffset = Vector3.Lerp(jumpOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);

        // Smooth recovery of the stomp effect over time
        stompOffset = Vector3.Lerp(stompOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
    }
    
    private void RecoverRecoil()
    {
        // Recuperar el recoil de manera suave
        currentRecoilOffset = Vector3.Lerp(currentRecoilOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
        currentRecoilRotation = Quaternion.Lerp(currentRecoilRotation, Quaternion.identity, Time.deltaTime * recoilReturnSpeed);

        recoilOffsetAccum = Vector3.Lerp(recoilOffsetAccum, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
        recoilRotationAccum = Vector3.Lerp(recoilRotationAccum, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
    }

    // 🔫 Se llama desde WeaponController al disparar
    public void ApplyRecoil()
    {
        // Aplicar y limitar desplazamiento por recoil
        Vector3 addedOffset = new Vector3(0, 0, -recoilForce);
        Vector3 targetOffset = recoilOffsetAccum + addedOffset;

        targetOffset.x = Mathf.Clamp(targetOffset.x, -maxRecoilOffset.x, maxRecoilOffset.x);
        targetOffset.y = Mathf.Clamp(targetOffset.y, -maxRecoilOffset.y, maxRecoilOffset.y);
        targetOffset.z = Mathf.Clamp(targetOffset.z, -maxRecoilOffset.z, maxRecoilOffset.z);

        Vector3 offsetDelta = targetOffset - recoilOffsetAccum;
        currentRecoilOffset += offsetDelta;
        recoilOffsetAccum = targetOffset;

        // Aplicar y limitar rotación por recoil
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
        // Apply a small downward offset when the player jumps
        jumpOffset = new Vector3(0, jumpOffsetposition, 0); // Small downward movement

        // Apply a small downward rotation to simulate the opposite of recoil
        currentRecoilRotation *= Quaternion.Euler(0f, 0f, jumpOffsetrotarion); // Small downward rotation on the X-axis
    }
        public void TriggerStompEffect()
    {
        // Apply a small upward offset when the player stomps (opposite of the jump effect)
        stompOffset = new Vector3(0, -stompOffsetPosition, 0); // Small upward movement

        // Apply a small upward rotation to simulate the opposite of recoil when stomping
        currentRecoilRotation *= Quaternion.Euler(0f, 0f, -stompOffsetRotation); // Small upward rotation on the Z-axis
    }
    public void SetPlayerController(PlayerController controller)
    {
        playerController = controller;
    }
}
