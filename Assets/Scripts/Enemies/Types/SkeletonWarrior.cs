using UnityEngine;
using EElemental.Enemies.States;

namespace EElemental.Enemies
{
    /// <summary>
    /// Skeleton Warrior - Orta zorluk düşman
    /// Kılıç saldırısı, blok yapabilir, patrol yapar
    /// </summary>
    public class SkeletonWarrior : EnemyBase
    {
        [Header("Skeleton Settings")]
        [SerializeField] private float blockChance = 0.3f;
        [SerializeField] private float blockDuration = 0.5f;
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRadius = 1f;
        [SerializeField] private LayerMask playerLayer;
        
        private bool isBlocking = false;
        private float blockTimer = 0f;
        
        protected override void InitializeStateMachine()
        {
            stateMachine = new EnemyStateMachine(this, Stats, AI);
            stateMachine.Initialize(new EnemyPatrolState(stateMachine));
        }
        
        protected override void Update()
        {
            base.Update();
            
            // Block timer
            if (isBlocking)
            {
                blockTimer += Time.deltaTime;
                if (blockTimer >= blockDuration)
                {
                    EndBlock();
                }
            }
        }
        
        public override void TakeDamage(float damage, Vector2 knockbackDirection = default)
        {
            // Blok yapıyorsa hasarı azalt
            if (isBlocking)
            {
                damage *= 0.2f; // %80 hasar azaltma
                knockbackDirection *= 0.3f; // Knockback azaltma
            }
            
            base.TakeDamage(damage, knockbackDirection);
        }
        
        public override void Attack()
        {
            if (attackPoint == null) return;
            
            // Saldırı alanındaki hedefleri bul
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                attackPoint.position, 
                attackRadius, 
                playerLayer
            );
            
            foreach (var hit in hits)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Vector2 knockback = (hit.transform.position - transform.position).normalized;
                    damageable.TakeDamage(Stats.GetModifiedAttackDamage(), knockback * 5f);
                }
            }
            
            Stats.RecordAttack();
        }
        
        /// <summary>
        /// Rastgele blok yapma şansı
        /// </summary>
        public void TryBlock()
        {
            if (isBlocking) return;
            
            if (Random.value < blockChance)
            {
                StartBlock();
            }
        }
        
        private void StartBlock()
        {
            isBlocking = true;
            blockTimer = 0f;
            animator?.SetBool("IsBlocking", true);
        }
        
        private void EndBlock()
        {
            isBlocking = false;
            animator?.SetBool("IsBlocking", false);
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Attack range
            if (attackPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
            }
        }
    }
}
