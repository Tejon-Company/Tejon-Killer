using System.Threading.Tasks;
using UnityEngine;

public class Sway : MonoBehaviour
{
    private Quaternion originLocalRotation;
    private Vector3 originLocalPosition;
    private Vector3 targetPosition;

    [SerializeField] private float balanceFactor = 1.45f;
    [SerializeField] private float timeFactor = 10f;

    [Header("Slide/Dash Settings")]
    [SerializeField] private Vector3 slideOffset = new Vector3(0, 0, -0.3f);
    [SerializeField] private float slideLerpSpeed = 10f;

    [Header("Player Reference")]
    [SerializeField] private PlayerController playerController;

    private void Start()
    {
        originLocalRotation = transform.localRotation;
        originLocalPosition = transform.localPosition;
        targetPosition = originLocalPosition;
    }

    void Update()
    {
        UpdateSway();

        if (playerController != null && (playerController.sliding || playerController.dashing))
            ApplySlideOffset();
        else
            ResetSlideOffset();

        UpdateSlideOffset();
    }

    private void UpdateSway()
    {
        float x_LookInput = Input.GetAxis("Mouse X");
        float y_LookInput = Input.GetAxis("Mouse Y");

        Quaternion x_AngleAdjustment = Quaternion.AngleAxis(-x_LookInput * balanceFactor, Vector3.up);
        Quaternion y_AngleAdjustment = Quaternion.AngleAxis(y_LookInput * balanceFactor, Vector3.right);

        Quaternion targetRotation = originLocalRotation * x_AngleAdjustment * y_AngleAdjustment;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * timeFactor);
    }

    private void UpdateSlideOffset()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * slideLerpSpeed);
    }

    public void ApplySlideOffset()
    {
        targetPosition = originLocalPosition + slideOffset;
    }

    public void ResetSlideOffset()
    {
        targetPosition = originLocalPosition;
    }
}
