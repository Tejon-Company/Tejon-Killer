using UnityEngine;
using UnityEngine.AI;

public class Fox : MonoBehaviour, IDamageableVisual
{
    [Header("Comportamiento")]
    public Transform[] patrolPoints;
    public Transform player;
    public float chaseRange = 20f;
    public float attackRange = 2f;
    public int damageToPlayer = 5;
    public float attackCooldown = 1.5f;

    [Header("Componentes")]
    public Animator animator;
    public NavMeshAgent agent;

    [Header("Flash Rojo")]
    public SkinnedMeshRenderer[] scratRenderers;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private MaterialPropertyBlock propBlock;
    private Color[] originalColors;

    private int patrolIndex = 0;
    private bool isDead = false;
    private float lastAttackTime = 0f;

    private void Start()
    {
        if (patrolPoints.Length == 0)
        {
            enabled = false;
            return;
        }

        agent.SetDestination(patrolPoints[patrolIndex].position);

        propBlock = new MaterialPropertyBlock();
        SaveOriginalColors();
    }

    private void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            AttackPlayer();
        }
        else if (distance <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        animator.SetTrigger("Walk");
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }

    private void ChasePlayer()
    {
        animator.SetTrigger("Run");
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        animator.SetTrigger("Attack");
        agent.ResetPath();

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageToPlayer);
            }
        }
    }

    public void FlashRed()
    {
        for (int i = 0; i < scratRenderers.Length; i++)
        {
            scratRenderers[i].GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", flashColor);
            scratRenderers[i].SetPropertyBlock(propBlock);
        }

        Invoke(nameof(RestoreOriginalColor), flashDuration);
    }

    private void RestoreOriginalColor()
    {
        for (int i = 0; i < scratRenderers.Length; i++)
        {
            scratRenderers[i].GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", originalColors[i]);
            scratRenderers[i].SetPropertyBlock(propBlock);
        }
    }

    private void SaveOriginalColors()
    {
        originalColors = new Color[scratRenderers.Length];
        for (int i = 0; i < scratRenderers.Length; i++)
        {
            originalColors[i] = scratRenderers[i].sharedMaterial.GetColor("_BaseColor");
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        agent.enabled = false;
        Destroy(gameObject);
    }
}
