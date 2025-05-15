using _Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Enemies.Fox
{
    public class Fox : Enemy
    {
        private static readonly int IsChasing = Animator.StringToHash("IsChasing");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

        [Header("Behavior")]
        [SerializeField] private float attackRange = .1f;
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float patrolRadius = 10f;

        private Animator animator;
        private NavMeshAgent agent;
        private float lastAttackTime;

        private void Start()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            var distanceToPlayer = Vector3.Distance(Player.position, transform.position);

            if (distanceToPlayer <= attackRange)
            {
                RotateToPlayer();
                Attack();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                Chase();
            }
            else
            {
                Patrol();
            }
        }

        private protected override void FindReferences()
        {
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Chase()
        {
            agent.isStopped = false;
            agent.SetDestination(Player.position);
            
            if (animator.GetBool(IsChasing))
                return;
            
            animator.SetBool(IsChasing, true);
            animator.SetBool(IsAttacking, false);
        }

         private protected override void Attack()
        {
            agent.ResetPath();
            agent.isStopped = true;

            if (Time.time - lastAttackTime < attackCooldown)
                return;

            lastAttackTime = Time.time;

            animator.SetBool(IsChasing, false);
            if (animator.GetBool(IsAttacking))
                return;
            
            animator.SetBool(IsAttacking, true);

            var playerHealth = Player.GetComponent<PlayerHealth>();
            if (playerHealth)
            {
                playerHealth.TakeDamage(1);
            }
        }

        private void Patrol()
        {
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                SetRandomPatrolPoint();
            }

            animator.SetBool(IsChasing, false);
            animator.SetBool(IsAttacking, false);
        }

        private void SetRandomPatrolPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                agent.isStopped = false;
            }
        }
    }
}