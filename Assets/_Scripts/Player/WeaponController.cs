using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("General")]
    public LayerMask hittableLayers;
    public GameObject bulletHolePrefab;

    [Header("Shoot Parameters")]
    public float fireRange = 200;
    public float fireRate = 0.2f;

    [Header("References")]
    [SerializeField] private Sway sway;

    private Transform cameraPlayerTransform;
    private float lastShotTime = 0f;

    private void Start()
    {
        cameraPlayerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire") && Time.time >= lastShotTime + fireRate) // Comprobar cooldown
        {
            Shoot();
            lastShotTime = Time.time; // Actualizar el tiempo del último disparo
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraPlayerTransform.position, cameraPlayerTransform.forward, out hit, fireRange, hittableLayers))
        {
            GameObject bulletHoleClone = Instantiate(bulletHolePrefab, hit.point + hit.normal * -0.01f, Quaternion.LookRotation(hit.normal));
            Destroy(bulletHoleClone, 4f);
        }

        sway?.ApplyRecoil();
    }

    // Función para cambiar la cadencia de disparo (fireRate)
    public void SetFireRate(float newFireRate)
    {
        fireRate = newFireRate; // Establece un nuevo valor para la cadencia
    }
}
