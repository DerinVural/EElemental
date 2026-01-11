using UnityEngine;

namespace EElemental.Enemies
{
    /// <summary>
    /// Tüm düşmanların temel sınıfı.
    /// Combat ve hareket sistemleri bu sınıftan türer.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] protected Rigidbody2D rb;
        [SerializeField] protected Animator animator;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        
        [Header("Ground Check")]
        [SerializeField] protected Transform groundCheck;
        [SerializeField] protected float groundCheckRadius = 0.2f;
        [SerializeField] protected LayerMask groundLayer;
        
        [Header("Wall Check")]
        [SerializeField] protected Transform wallCheck;
        [SerializeField] protected float wallCheckDistance = 0.5f;
        
        [Header("Edge Check")]
        [SerializeField] protected Transform edgeCheck;
        [SerializeField] protected float edgeCheckDistance = 0.5f;
        
        // State Machine
        protected EnemyStateMachine stateMachine;
        
        // Properties
        public EnemyStats Stats { get; protected set; }
        public EnemyAI AI { get; protected set; }
        public bool IsGrounded { get; protected set; }
        public bool IsFacingRight { get; protected set; } = true;
        public bool IsAlive => Stats != null && Stats.CurrentHealth > 0;
        public Transform Target { get; set; }
        
        // Events
        public System.Action<float> OnDamageTaken;
        public System.Action OnDeath;
        public System.Action OnTargetLost;
        public System.Action<Transform> OnTargetDetected;
        
        protected virtual void Awake()
        {
            // Get components
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            
            // Initialize stats and AI
            Stats = GetComponent<EnemyStats>();
            AI = GetComponent<EnemyAI>();
            
            // Initialize state machine
            InitializeStateMachine();
        }
        
        protected virtual void Start()
        {
            // Find player as initial target
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Target = player.transform;
            }
        }
        
        protected virtual void Update()
        {
            if (!IsAlive) return;
            
            CheckGround();
            stateMachine?.Update();
        }
        
        protected virtual void FixedUpdate()
        {
            if (!IsAlive) return;
            
            stateMachine?.FixedUpdate();
        }
        
        /// <summary>
        /// State machine'i başlat - Alt sınıflar override edebilir
        /// </summary>
        protected abstract void InitializeStateMachine();
        
        #region Movement
        
        public virtual void Move(float direction)
        {
            if (Mathf.Abs(direction) > 0.1f)
            {
                rb.linearVelocity = new Vector2(direction * Stats.MoveSpeed, rb.linearVelocity.y);
                
                // Flip sprite
                if ((direction > 0 && !IsFacingRight) || (direction < 0 && IsFacingRight))
                {
                    Flip();
                }
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
        }
        
        public virtual void Flip()
        {
            IsFacingRight = !IsFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        
        public virtual void Stop()
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        
        #endregion
        
        #region Checks
        
        protected virtual void CheckGround()
        {
            if (groundCheck != null)
            {
                IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            }
        }
        
        public virtual bool IsWallAhead()
        {
            if (wallCheck == null) return false;
            
            Vector2 direction = IsFacingRight ? Vector2.right : Vector2.left;
            return Physics2D.Raycast(wallCheck.position, direction, wallCheckDistance, groundLayer);
        }
        
        public virtual bool IsEdgeAhead()
        {
            if (edgeCheck == null) return false;
            
            return !Physics2D.Raycast(edgeCheck.position, Vector2.down, edgeCheckDistance, groundLayer);
        }
        
        #endregion
        
        #region Combat
        
        public virtual void TakeDamage(float damage, Vector2 knockbackDirection = default)
        {
            if (!IsAlive) return;
            
            Stats.TakeDamage(damage);
            OnDamageTaken?.Invoke(damage);
            
            // Apply knockback
            if (knockbackDirection != default && Stats.CanBeKnockedBack)
            {
                ApplyKnockback(knockbackDirection);
            }
            
            // Play hurt animation
            animator?.SetTrigger("Hurt");
            
            // Check death
            if (!IsAlive)
            {
                Die();
            }
        }
        
        protected virtual void ApplyKnockback(Vector2 direction)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * Stats.KnockbackResistance, ForceMode2D.Impulse);
        }
        
        public virtual void Die()
        {
            OnDeath?.Invoke();
            
            // Play death animation
            animator?.SetTrigger("Death");
            
            // Disable collider
            var collider = GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            
            // Stop movement
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
            
            // Destroy after animation
            Destroy(gameObject, 2f);
        }
        
        /// <summary>
        /// Saldırı yap - Alt sınıflar implement etmeli
        /// </summary>
        public abstract void Attack();
        
        #endregion
        
        #region Target Detection
        
        public virtual void SetTarget(Transform target)
        {
            Target = target;
            if (target != null)
            {
                OnTargetDetected?.Invoke(target);
            }
        }
        
        public virtual void LoseTarget()
        {
            Target = null;
            OnTargetLost?.Invoke();
        }
        
        public virtual float GetDistanceToTarget()
        {
            if (Target == null) return float.MaxValue;
            return Vector2.Distance(transform.position, Target.position);
        }
        
        public virtual Vector2 GetDirectionToTarget()
        {
            if (Target == null) return Vector2.zero;
            return (Target.position - transform.position).normalized;
        }
        
        #endregion
        
        #region Gizmos
        
        protected virtual void OnDrawGizmosSelected()
        {
            // Ground check
            if (groundCheck != null)
            {
                Gizmos.color = IsGrounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
            
            // Wall check
            if (wallCheck != null)
            {
                Gizmos.color = Color.blue;
                Vector2 direction = IsFacingRight ? Vector2.right : Vector2.left;
                Gizmos.DrawRay(wallCheck.position, direction * wallCheckDistance);
            }
            
            // Edge check
            if (edgeCheck != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(edgeCheck.position, Vector2.down * edgeCheckDistance);
            }
        }
        
        #endregion
    }
}
