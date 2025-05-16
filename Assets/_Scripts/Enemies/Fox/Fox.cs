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
        [SerializeField]
        private float attackRange = .1f;

        [SerializeField]
        private float attackCooldown = 1f;

        [SerializeField]
        private float patrolRadius = 10f;

        private Animator _animator;
        private NavMeshAgent _agent;
        private float _lastAttackTime;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            var distanceToPlayer = Vector3.Distance(Player.position, transform.position);

            if (distanceToPlayer <= attackRange)
                Attack();
            else if (distanceToPlayer <= detectionRange)
                Chase();
            else
                Patrol();
        }

        private protected override void FindReferences()
        {
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Chase()
        {
            _agent.isStopped = false;
            _agent.SetDestination(Player.position);

            if (_animator.GetBool(IsChasing))
                return;

            _animator.SetBool(IsChasing, true);
            _animator.SetBool(IsAttacking, false);
        }

        private protected override void Attack()
        {
            RotateToPlayer();

            _agent.ResetPath();
            _agent.isStopped = true;

            if (Time.time - _lastAttackTime < attackCooldown)
                return;

            _lastAttackTime = Time.time;

            _animator.SetBool(IsChasing, false);
            if (_animator.GetBool(IsAttacking))
                return;

            _animator.SetBool(IsAttacking, true);

            var playerHealth = Player.GetComponent<PlayerHealth>();
            if (playerHealth)
                playerHealth.TakeDamage(1);
        }

        private void Patrol()
        {
            if (!_agent.hasPath || _agent.remainingDistance < 0.5f)
                SetRandomPatrolPoint();

            _animator.SetBool(IsChasing, false);
            _animator.SetBool(IsAttacking, false);
        }

        private void SetRandomPatrolPoint()
        {
            var randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;

            var foundPatrolPoint = NavMesh.SamplePosition(
                randomDirection,
                out var hit,
                patrolRadius,
                NavMesh.AllAreas
            );

            if (!foundPatrolPoint)
                return;

            _agent.SetDestination(hit.position);
            _agent.isStopped = false;
        }
    }
}
