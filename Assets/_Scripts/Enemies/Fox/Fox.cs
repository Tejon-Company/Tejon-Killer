using _Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace _Scripts.Enemies.Fox
{
    public class Fox : Enemy
    {
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int RunTrigger = Animator.StringToHash("Run");
        private static readonly int WalkTrigger = Animator.StringToHash("Walk");

        [Header("BEHAVIOUR")]
        [SerializeField]
        private Transform[] patrolPoints;
        [SerializeField]
        private float chaseRange = 20f;
        [SerializeField]
        private float attackRange = 2f;
        [SerializeField]
        private float attackCooldown = 1.5f;

        [Header("Componentes")]
        public Animator animator;
        public NavMeshAgent agent;

        private int patrolIndex;
        private float lastAttackTime;

        private void Start()
        {
            if (patrolPoints.Length == 0)
            {
                enabled = false;
                return;
            }

            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    
        private protected override void FindReferences()
        {
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            var distance = Vector3.Distance(transform.position, Player.position);

            if (distance <= attackRange)
            {
                Attack();
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
            animator.SetTrigger(WalkTrigger);
            if (agent.pathPending || !(agent.remainingDistance < 0.5f)) 
                return;
        
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[patrolIndex].position);
        }

        private void ChasePlayer()
        {
            animator.SetTrigger(RunTrigger);
            agent.SetDestination(Player.position);
        }

        private protected override void Attack()
        {
            animator.SetTrigger(AttackTrigger);
            agent.ResetPath();

            if (Time.time - lastAttackTime < attackCooldown)
                return;
        
            lastAttackTime = Time.time;

            var playerHealth = Player.GetComponent<PlayerHealth>();
            if (playerHealth)
            {
                playerHealth.TakeDamage(1);
            }
        
        }
    }
}
