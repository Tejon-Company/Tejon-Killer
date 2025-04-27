using UnityEngine;

public abstract class BerryEffect : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other);
            Destroy(gameObject);
        }
    }

    protected abstract void ApplyEffect(Collider player);
}
