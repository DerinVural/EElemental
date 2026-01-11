using UnityEngine;

namespace EElemental.Enemies
{
    /// <summary>
    /// Düşman yapay zeka sistemi.
    /// Hedef tespiti, takip, saldırı kararları ve patrol davranışları.
    /// </summary>
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] private bool enableAI = true;
        [SerializeField] private float thinkInterval = 0.2f; // AI karar verme aralığı
        
        [Header("Detection")]
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private bool requireLineOfSight = true;
        
        [Header("Patrol")]
        [SerializeField] private Transform[] patrolPoints;
        private int currentPatrolIndex = 0;
        private bool patrollingForward = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugGizmos = true;
        
        // Components
        private EnemyBase enemy;
        private EnemyStats stats;
        
        // AI State
        public AIState CurrentState { get; private set; } = AIState.Idle;
        public Transform CurrentTarget { get; private set; }
        public Vector2 PatrolTarget { get; private set; }
        public bool HasTarget => CurrentTarget != null;
        public bool IsAggro { get; private set; }
        
        // Timers
        private float lastThinkTime;
        private float aggroTimer;
        private float idleTimer;
        
        // Events
        public System.Action<AIState> OnStateChanged;
        public System.Action<Transform> OnTargetAcquired;
        public System.Action OnTargetLost;
        
        private void Awake()
        {
            enemy = GetComponent<EnemyBase>();
            stats = GetComponent<EnemyStats>();
        }
        
        private void Start()
        {
            SetPatrolTarget();
        }
        
        private void Update()
        {
            if (!enableAI || !enemy.IsAlive) return;
            
            // Throttled thinking
            if (Time.time - lastThinkTime >= thinkInterval)
            {
                Think();
                lastThinkTime = Time.time;
            }
        }
        
        /// <summary>
        /// Ana AI karar verme metodu
        /// </summary>
        private void Think()
        {
            // 1. Hedef tespiti
            CheckForTargets();
            
            // 2. Mevcut duruma göre davran
            switch (CurrentState)
            {
                case AIState.Idle:
                    ThinkIdle();
                    break;
                    
                case AIState.Patrol:
                    ThinkPatrol();
                    break;
                    
                case AIState.Chase:
                    ThinkChase();
                    break;
                    
                case AIState.Attack:
                    ThinkAttack();
                    break;
                    
                case AIState.Retreat:
                    ThinkRetreat();
                    break;
            }
        }
        
        #region State Thinking
        
        private void ThinkIdle()
        {
            idleTimer += thinkInterval;
            
            // Hedef varsa chase'e geç
            if (HasTarget)
            {
                ChangeState(AIState.Chase);
                return;
            }
            
            // Belli süre sonra patrol'a geç
            if (idleTimer >= 2f)
            {
                idleTimer = 0f;
                ChangeState(AIState.Patrol);
            }
        }
        
        private void ThinkPatrol()
        {
            // Hedef varsa chase'e geç
            if (HasTarget)
            {
                ChangeState(AIState.Chase);
                return;
            }
            
            // Patrol noktasına ulaştık mı?
            float distanceToPatrolTarget = Vector2.Distance(transform.position, PatrolTarget);
            if (distanceToPatrolTarget < 0.5f)
            {
                ChangeState(AIState.Idle);
                SetNextPatrolTarget();
            }
        }
        
        private void ThinkChase()
        {
            if (!HasTarget)
            {
                ChangeState(AIState.Patrol);
                return;
            }
            
            float distanceToTarget = enemy.GetDistanceToTarget();
            
            // Saldırı menzilindeyse saldır
            if (distanceToTarget <= stats.AttackRange && stats.CanAttack)
            {
                ChangeState(AIState.Attack);
                return;
            }
            
            // Hedef çok uzaklaştıysa bırak
            if (distanceToTarget > stats.ChaseRange)
            {
                LoseTarget();
                ChangeState(AIState.Patrol);
            }
        }
        
        private void ThinkAttack()
        {
            if (!HasTarget)
            {
                ChangeState(AIState.Chase);
                return;
            }
            
            float distanceToTarget = enemy.GetDistanceToTarget();
            
            // Hedef menzil dışına çıktıysa chase'e dön
            if (distanceToTarget > stats.AttackRange * 1.5f)
            {
                ChangeState(AIState.Chase);
            }
        }
        
        private void ThinkRetreat()
        {
            // Geri çekilme mantığı - health düşükse veya tehlike varsa
            if (stats.CurrentHealth > stats.MaxHealth * 0.3f)
            {
                ChangeState(HasTarget ? AIState.Chase : AIState.Patrol);
            }
        }
        
        #endregion
        
        #region Target Detection
        
        private void CheckForTargets()
        {
            if (HasTarget)
            {
                // Mevcut hedefi doğrula
                ValidateCurrentTarget();
                return;
            }
            
            // Yeni hedef ara
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, 
                stats.DetectionRange, 
                targetLayer
            );
            
            foreach (var hit in hits)
            {
                if (CanSeeTarget(hit.transform))
                {
                    AcquireTarget(hit.transform);
                    break;
                }
            }
        }
        
        private void ValidateCurrentTarget()
        {
            if (CurrentTarget == null)
            {
                LoseTarget();
                return;
            }
            
            // Hedef hala görüş alanında mı?
            float distance = Vector2.Distance(transform.position, CurrentTarget.position);
            
            // Çok uzaklaştıysa hedefi kaybet
            if (distance > stats.ChaseRange)
            {
                LoseTarget();
                return;
            }
            
            // Görüş hattı kontrolü
            if (requireLineOfSight && !CanSeeTarget(CurrentTarget))
            {
                aggroTimer += thinkInterval;
                if (aggroTimer >= 3f) // 3 saniye göremezse hedefi kaybet
                {
                    LoseTarget();
                }
            }
            else
            {
                aggroTimer = 0f;
            }
        }
        
        private bool CanSeeTarget(Transform target)
        {
            if (target == null) return false;
            
            Vector2 direction = (target.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, target.position);
            
            // Raycast ile engel kontrolü
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, 
                direction, 
                distance, 
                obstacleLayer
            );
            
            return hit.collider == null;
        }
        
        private void AcquireTarget(Transform target)
        {
            CurrentTarget = target;
            enemy.SetTarget(target);
            IsAggro = true;
            aggroTimer = 0f;
            
            OnTargetAcquired?.Invoke(target);
        }
        
        private void LoseTarget()
        {
            CurrentTarget = null;
            enemy.LoseTarget();
            IsAggro = false;
            aggroTimer = 0f;
            
            OnTargetLost?.Invoke();
        }
        
        #endregion
        
        #region Patrol
        
        private void SetPatrolTarget()
        {
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                PatrolTarget = patrolPoints[currentPatrolIndex].position;
            }
            else
            {
                // Varsayılan patrol - rastgele nokta
                float randomX = Random.Range(-3f, 3f);
                PatrolTarget = (Vector2)transform.position + new Vector2(randomX, 0);
            }
        }
        
        private void SetNextPatrolTarget()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                SetPatrolTarget();
                return;
            }
            
            // Back and forth patrol
            if (patrollingForward)
            {
                currentPatrolIndex++;
                if (currentPatrolIndex >= patrolPoints.Length)
                {
                    currentPatrolIndex = patrolPoints.Length - 2;
                    patrollingForward = false;
                }
            }
            else
            {
                currentPatrolIndex--;
                if (currentPatrolIndex < 0)
                {
                    currentPatrolIndex = 1;
                    patrollingForward = true;
                }
            }
            
            currentPatrolIndex = Mathf.Clamp(currentPatrolIndex, 0, patrolPoints.Length - 1);
            PatrolTarget = patrolPoints[currentPatrolIndex].position;
        }
        
        #endregion
        
        #region State Management
        
        public void ChangeState(AIState newState)
        {
            if (CurrentState == newState) return;
            
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }
        
        public void ForceState(AIState state)
        {
            ChangeState(state);
        }
        
        #endregion
        
        #region Public API
        
        public void EnableAI(bool enable)
        {
            enableAI = enable;
        }
        
        public void AlertToTarget(Transform target)
        {
            if (target != null && !HasTarget)
            {
                AcquireTarget(target);
                ChangeState(AIState.Chase);
            }
        }
        
        public Vector2 GetMoveDirection()
        {
            switch (CurrentState)
            {
                case AIState.Chase:
                case AIState.Attack:
                    return enemy.GetDirectionToTarget();
                    
                case AIState.Patrol:
                    return ((Vector3)PatrolTarget - transform.position).normalized;
                    
                case AIState.Retreat:
                    return -enemy.GetDirectionToTarget();
                    
                default:
                    return Vector2.zero;
            }
        }
        
        #endregion
        
        #region Debug
        
        private void OnDrawGizmosSelected()
        {
            if (!showDebugGizmos) return;
            
            var statsComponent = GetComponent<EnemyStats>();
            float detectionRange = statsComponent != null ? statsComponent.DetectionRange : 8f;
            float attackRange = statsComponent != null ? statsComponent.AttackRange : 1.5f;
            float chaseRange = statsComponent != null ? statsComponent.ChaseRange : 12f;
            
            // Detection range
            Gizmos.color = new Color(1, 1, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            // Attack range
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
            
            // Chase range
            Gizmos.color = new Color(0, 0, 1, 0.2f);
            Gizmos.DrawWireSphere(transform.position, chaseRange);
            
            // Patrol target
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, PatrolTarget);
            Gizmos.DrawWireSphere(PatrolTarget, 0.3f);
            
            // Current target
            if (CurrentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, CurrentTarget.position);
            }
        }
        
        #endregion
    }
    
    public enum AIState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Retreat,
        Stunned,
        Dead
    }
}
